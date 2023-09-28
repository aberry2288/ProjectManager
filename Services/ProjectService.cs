using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;

        public ProjectService(ApplicationDbContext context, IBTRolesService bTRolesService)
        {
            _context = context;
            _rolesService = bTRolesService;
        }

        public async Task AddMembersToProjectAsync(IEnumerable<string>? userIds, int? projectId, int? companyId)
        {
            try
            {
                if (userIds != null)
                {
                    Project? project = await GetProjectByIdAsync(projectId, companyId);

                    foreach (string userId in userIds)
                    {
                        BTUser? btUser = await _context.Users.FindAsync(userId);

                        if (project != null && btUser != null)
                        {
                            bool IsOnProject = project.Members.Any(m => m.Id == userId);

                            if (!IsOnProject)
                            {
                                project.Members.Add(btUser);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> AddMemberToProjectAsync(BTUser? member, int? projectId)
        {
            try
            {
                if (member != null && projectId != null)
                {
                    Project? project = await GetProjectByIdAsync(projectId, member.CompanyId);

                    if (project != null)
                    {
                        //project instance must include members to do the following
                        bool IsOnProject = project.Members.Any(m => m.Id == member.Id);

                        if (!IsOnProject)
                        {
                            project.Members.Add(member);
                            await _context.SaveChangesAsync();
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task AddProjectAsync(Project project)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddProjectManagerAsync(string? userId, int? projectId)
        {
            try
            {
                if (userId != null && projectId != null)
                {
                    BTUser? currentPM = await GetProjectManagerAsync(projectId);
                    BTUser? selectedPM = await _context.Users.FindAsync(userId);

                    //Remove current pm
                    if (currentPM != null)
                    {
                        await RemoveProjectManagerAsync(projectId);
                    }

                    //Add new PM
                    try
                    {
                        await AddMemberToProjectAsync(selectedPM!, projectId);
                        return true;
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task ArchiveProjectAsync(Project? project, int? companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetAllProjectsByCompanyIdAsync(int? companyId)
        {
            try
            {
                List<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == false).Include(p => p.Members)
                                                                .Include(p => p.ProjectPriority).Include(p => p.Tickets).ToListAsync();

                return projects;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Project>> GetArchivedProjectsByCompanyIdAsync(int? companyId)
        {
			try
			{
				List<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == true).Include(p => p.Members)
																.Include(p => p.ProjectPriority).Include(p => p.Tickets).ToListAsync();

				return projects;
			}
			catch (Exception)
			{

				throw;
			}
		}

        public async Task<Project> GetProjectAsync(int? id)
        {
            if (id == null) { return new Project(); }

            try
            {

                Project? project = await _context.Projects
                  .Include(p => p.Company)
                  .Include(p => p.Tickets).ThenInclude(t => t.History)
                  .Include(p => p.Tickets).ThenInclude(t => t.SubmitterUser)
                  .Include(p => p.Tickets).ThenInclude(t => t.DeveloperUser)
                  .Include(p => p.Tickets).ThenInclude(t => t.TicketPriority)
                  .Include(p => p.Tickets).ThenInclude(t => t.TicketStatus)
                  .Include(p => p.Tickets).ThenInclude(t => t.TicketType)
                  .Include(p => p.Members)
                  .Include(p => p.ProjectPriority)
                  .FirstOrDefaultAsync(m => m.Id == id);

                return project!;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<Project> GetProjectByIdAsync(int? projectId, int? companyId)
        {
            try
            {
                Project? project = new();

                if (projectId != null && companyId != null)
                {

                    project = await _context.Projects
                                    .Include(p => p.ProjectPriority)
                                    .Include(p => p.Company).Include(p => p.Members)
                                    .Include(p => p.Tickets).ThenInclude(t => t.Comments)
                                    .ThenInclude(c => c.User).Include(p => p.Tickets)
                                    .ThenInclude(t => t.Attachments).Include(p => p.Tickets)
                                    .ThenInclude(t => t.History).FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

                }

                return project!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<BTUser> GetProjectManagerAsync(int? projectId)
        {
            try
            {
                Project? project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

                if (project != null)
                {
                    foreach (BTUser member in project.Members)
                    {
                        if (await _rolesService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
                        {
                            return member;
                        }
                    }
                }

                return null!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int? projectId, string? roleName, int? companyId)
        {
            try
            {
                List<BTUser> members = new();

                if (projectId != null && companyId != null && !string.IsNullOrEmpty(roleName))
                {
                    Project? project = await GetProjectByIdAsync(projectId, companyId);

                    if (project != null)
                    {
                        foreach (BTUser member in project.Members)
                        {
                            if (await _rolesService.IsUserInRoleAsync(member, roleName))
                            {
                                members.Add(member);
                            }
                        }

                        //Testing purposes
                        List<string> developers = (await _rolesService.GetUsersInRoleAsync(roleName, companyId))?.Select(u => u.Id).ToList()!;

                        List<BTUser> users = project.Members.Where(m => developers.Contains(m.Id)).ToList();

                    }

                }

                return members;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<IEnumerable<ProjectPriority>> GetProjectPrioritiesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Project>?> GetUserProjectsAsync(string? userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveMemberFromProjectAsync(BTUser? member, int? projectId)
        {
            try
            {
                if (member != null && projectId != null)
                {
                    Project? project = await GetProjectByIdAsync(projectId, member.CompanyId);

                    if (project != null)
                    {
                        bool IsOnProject = project.Members.Any(m => m.Id == member.Id);

                        if (IsOnProject)
                        {
                            project.Members.Remove(member);
                            await _context.SaveChangesAsync();
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveMembersFromProjectAsync(int? projectId, int? companyId)
        {
            try
            {

                Project? project = await GetProjectByIdAsync(projectId, companyId);

                foreach (var member in project.Members)
                {
                    if (!await _rolesService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
                    {
                        project.Members.Remove(member);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveProjectManagerAsync(int? projectId)
        {
            try
            {
                if (projectId != null)
                {
                    Project? project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

                    if (project != null)
                    {
                        foreach (BTUser member in project!.Members)
                        {
                            if (await _rolesService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
                            {
                                //Remove BTUser from Project
                                await RemoveMemberFromProjectAsync(member, projectId);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task RestoreProjectAsync(Project? project, int? companyId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProjectAsync(Project? project)
        {
            throw new NotImplementedException();
        }
    }

}
