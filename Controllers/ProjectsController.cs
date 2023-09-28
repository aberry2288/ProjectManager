using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Identity;
using BugTracker.Services.Interfaces;
using BugTracker.Models.ViewModels;
using BugTracker.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using BugTracker.Services;
using System.Net.Sockets;

namespace BugTracker.Controllers
{
    [Authorize]
    public class ProjectsController : BTBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTFileService _BTFileService;
        private readonly IProjectService _projectService;
        private readonly IBTRolesService _rolesService;
        private readonly IBTTicketHistoryService _ticketHistoryService;
        private readonly IBTTicketService _ticketService;

        public ProjectsController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTFileService bTFileService, IProjectService projectService, IBTRolesService rolesService, IBTTicketHistoryService ticketHistoryService, IBTTicketService ticketService)
        {
            _context = context;
            _userManager = userManager;
            _BTFileService = bTFileService;
            _projectService = projectService;
            _rolesService = rolesService;
            _ticketHistoryService = ticketHistoryService;
            _ticketService = ticketService;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            IEnumerable<Project> projects = await _projectService.GetAllProjectsByCompanyIdAsync(_companyId);
            return View(projects);
        }

        public async Task<IActionResult> ArchivedProjects()
        {
            IEnumerable<Project> projects = await _projectService.GetArchivedProjectsByCompanyIdAsync(_companyId);
            return View(projects);
        }


        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project? project = await _projectService.GetProjectAsync(id);

            

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            //Project project = new Project();

            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Created,StartDate,EndDate,ProjectPriorityId,ImageFile,Archived")] Project project)
        {
            if (ModelState.IsValid)
            {
                BTUser? user = await _userManager.GetUserAsync(User);

                project.CompanyId = user!.CompanyId;

                project.Created = DateTime.Now;

                if (project.ImageFile != null)
                {
                    project.ImageData = await _BTFileService.ConvertFileToByteArrayAsync(project.ImageFile);

                    project.ImageType = project.ImageFile.ContentType;
                }

                _context.Add(project);
                await _context.SaveChangesAsync();

                //Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id, _companyId);

                //await _ticketHistoryService.AddHistoryAsync(null!, newTicket, ticket.SubmitterUserId);

                return RedirectToAction(nameof(Index));
            }

            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id", project.ProjectPriorityId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id", project.ProjectPriorityId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,Name,Description,Created,StartDate,EndDate,ProjectPriorityId,ImageData,ImageType,Archived")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id", project.ProjectPriorityId);
            return View(project);
        }

        //GET: Projects/Delete/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            Project project = await _projectService.GetProjectByIdAsync(id, _companyId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        //POST: Projects/Delete/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Archive(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
            }
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
               project.Archived = true;                
               
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ArchivedProjects));
        }

        public async Task<IActionResult> UnArchive(int id)
        {
			if (_context.Projects == null)
			{
				return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
			}
			var project = await _context.Projects.FindAsync(id);
			if (project != null)
			{
				project.Archived = false;

			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));

		}

        [HttpGet]
        public async Task<IActionResult> AssignPM(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project? project = await _projectService.GetProjectByIdAsync(id, _companyId);

            if (project == null)
            {
                return NotFound();
            }

            //Get list of project managers
            IEnumerable<BTUser> projectManagers = await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), _companyId);

            BTUser? currentPM = await _projectService.GetProjectManagerAsync(id);

            AssignPMViewModel viewModel = new()
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                PMList = new SelectList(projectManagers, "Id", "FullName", currentPM?.Id),
                PMId = currentPM?.Id,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPM(AssignPMViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.PMId))
            {
                if (await _projectService.AddProjectManagerAsync(viewModel.PMId, viewModel.ProjectId))
                {
                    return RedirectToAction(nameof(Details), new { id = viewModel.ProjectId });
                }
                else
                {
                    ModelState.AddModelError("PMId", "Error assigning PM.");
                }

                //ModelState.AddModelError("PMId", "No Project Manager chosen. Please select a PM.");

            }


            IEnumerable<BTUser> projectManagers = await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), _companyId);

            BTUser? currentPM = await _projectService.GetProjectManagerAsync(viewModel.ProjectId);

            viewModel.PMList = new SelectList(projectManagers, "Id", "FullName", currentPM?.Id);


            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AssignProjectMembers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project? project = await _projectService.GetProjectByIdAsync(id, _companyId);

            if (project == null)
            {
                return NotFound();
            }

            List<BTUser> submitters = await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.Submitter), _companyId);

            List<BTUser> developers = await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.Developer), _companyId);

            List<BTUser> usersList = submitters.Concat(developers).ToList();

            IEnumerable<string> currentMembers = project.Members.Select(u => u.Id);

            ProjectMembersViewModel viewModel = new()
            {
                Project = project,
                UsersList = new MultiSelectList(usersList, "Id", "FullName", currentMembers)
            };


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignProjectMembers(ProjectMembersViewModel viewModel)
        {
            if (viewModel.SelectedMembers != null)
            {
                await _projectService.RemoveMembersFromProjectAsync(viewModel.Project?.Id, _companyId);

                await _projectService.AddMembersToProjectAsync(viewModel.SelectedMembers, viewModel.Project?.Id, _companyId);

                return RedirectToAction(nameof(Details), new { id = viewModel.Project?.Id });
            }

            //Handle the error
            ModelState.AddModelError("SelectedMembers", "No users chosen. Please select users.");

            viewModel.Project = await _projectService.GetProjectByIdAsync(viewModel.Project?.Id, _companyId);

            List<BTUser> submitters = await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.Submitter), _companyId);

            List<BTUser> developers = await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.Developer), _companyId);

            List<BTUser> usersList = submitters.Concat(developers).ToList();

            IEnumerable<string> currentMembers = viewModel.Project!.Members.Select(u => u.Id);

            viewModel.UsersList = new MultiSelectList(usersList, "Id", "FullName", currentMembers);

            return View();
        }

        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
