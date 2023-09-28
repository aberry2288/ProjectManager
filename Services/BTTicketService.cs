using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace BugTracker.Services
{
    public class BTTicketService : IBTTicketService
    {
        private readonly ApplicationDbContext _context;

        public BTTicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddTicketAsync(Ticket? ticket)
        {
            throw new NotImplementedException();
        }


        public async Task AddTicketAttachmentAsync(TicketAttachment? ticketAttachment)
        {
            try
            {
                await _context.AddAsync(ticketAttachment);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task AddTicketCommentAsync(TicketComment? ticketComment)
        {
            throw new NotImplementedException();
        }

        public Task ArchiveTicketAsync(Ticket? ticket)
        {
            throw new NotImplementedException();
        }

        public async Task AssignTicketAsync(int? ticketId, string? userId)
        {
            try
            {
                if (ticketId != null && !string.IsNullOrEmpty(userId))
                {
                    BTUser? btUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                    Ticket? ticket = await GetTicketByIdAsync(ticketId, btUser!.CompanyId);

                    if (ticket != null && btUser != null)
                    {
                        ticket!.DeveloperUserId = userId;

                        await UpdateTicketAsync(ticket);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByCompanyIdAsync(int? companyId)
        {
            try
            {
                List<Ticket> tickets = await _context.Projects.Where(p => p.CompanyId == companyId)
                                                             .Include(p => p.Tickets).ThenInclude(t => t.TicketPriority)
                                                             .Include(p => p.Tickets).ThenInclude(t => t.TicketStatus)
                                                             .Include(p => p.Tickets).ThenInclude(t => t.TicketType)
                                                                .Include(p => p.Tickets).ThenInclude(t => t.Comments)
                                                                .ThenInclude(c => c.User)
                                                                .Include(p => p.Tickets).ThenInclude(t => t.Attachments)
                                                                .Include(p => p.Tickets).ThenInclude(t => t.History)
                                                                .SelectMany(p => p.Tickets)
                                                                .ToListAsync();

                return tickets;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Ticket> GetTicketAsNoTrackingAsync(int? ticketId, int? companyId)
        {
            try
            {
                Ticket? ticket = await _context.Tickets.Include(t => t.Project).ThenInclude(p => p!.Company)
                                                       .Include(t => t.Attachments).Include(t => t.Comments)
                                                       .Include(t => t.TicketPriority).Include(t => t.TicketStatus)
                                                       .Include(t => t.TicketType)
                                                       .Include(t => t.DeveloperUser).Include(t => t.History)
                                                       .Include(t => t.SubmitterUser).AsNoTracking()
                                                       .FirstOrDefaultAsync(t => t.Id == ticketId && t.Project!.CompanyId == companyId && t.Archived == false);

                return ticket!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<TicketAttachment> GetTicketAttachmentByIdAsync(int? ticketAttachmentId)
        {
            try
            {
                TicketAttachment? ticketAttachment = await _context.TicketAttachments
                                                                  .Include(t => t.BTUser)
                                                                  .FirstOrDefaultAsync(t => t.Id == ticketAttachmentId);
                return ticketAttachment;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<Ticket> GetTicketByIdAsync(int? ticketId, int? companyId)
        {

            try
            {
                Ticket? ticket = new();

                if (ticketId != null && companyId != null)
                {
                    ticket = await _context.Tickets.Where(t => t.Project!.CompanyId == companyId && t.Archived == false)
                                                            .Include(t => t.Project).ThenInclude(p => p!.Company)
                                                            .Include(t => t.Attachments).Include(t => t.Comments)
                                                            .Include(t => t.DeveloperUser).Include(t => t.History)
                                                            .Include(t => t.TicketType).Include(t => t.TicketPriority)
                                                            .Include(t => t.TicketStatus)
                                                            .Include(t => t.SubmitterUser).FirstOrDefaultAsync(t => t.Id == ticketId);
                }
                return ticket!;

            }
            catch (Exception)
            {

                throw;
            }


        }

        public Task<BTUser?> GetTicketDeveloperAsync(int? ticketId, int? companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByPriorityAsync(BTTicketPriorities priority, int? companyId)
        {
            try
            {
                IEnumerable<Ticket> tickets = new List<Ticket>();

                tickets = await _context.Tickets.Where(t => t.Project.CompanyId == companyId && t.TicketPriority.Name == priority.ToString()).ToListAsync();

                return tickets;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<IEnumerable<TicketPriority>> GetTicketPrioritiesAsync()
        {
            throw new NotImplementedException();
        }

        //public async Task<TicketPriority> GetTicketPrioritiesAsync(int? ticketId)
        //{
        //    try
        //    {
        //        Ticket? ticketPriority = new();

        //        if (ticketId != null)
        //        {
        //            ticketPriority = await _context.Tickets
        //                                                    .Include(t => t.Project).ThenInclude(p => p!.Company)
        //                                                    .Include(t => t.Attachments).Include(t => t.Comments)
        //                                                    .Include(t => t.DeveloperUser).Include(t => t.History)
        //                                                    .Include(t => t.TicketType).Include(t => t.TicketPriority)
        //                                                    .Include(t => t.TicketStatus)
        //                                                    .Include(t => t.SubmitterUser).FirstOrDefaultAsync(t => t.Id == ticketId);
        //        }
        //        return ticketPriority;

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public Task<List<Ticket>> GetTicketsByUserIdAsync(string? userId, int? companyId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TicketStatus>> GetTicketStatusesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TicketType>> GetTicketTypesAsync()
        {
            throw new NotImplementedException();
        }

        public Task RestoreTicketAsync(Ticket? ticket)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateTicketAsync(Ticket? ticket)
        {
            try
            {
                if (ticket != null)
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
