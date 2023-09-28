using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTCompanyService : IBTCompanyService
    {
        private readonly ApplicationDbContext _context;

        public BTCompanyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BTUser>> GetMembersAsync(int? companyId)
        {
            try
            {
                List<BTUser>? members = new();

                members = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();

                return members;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Company> GetCompanyInfoAsync(int? companyId)
        {
            //Company? company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == companyId);

            try
            {             
                
                               
                   Company? company = await _context.Companies.Include(c => c.Projects).Include(c => c.Invites).Include(c => c.Members).FirstOrDefaultAsync(c => c.Id == companyId);



                return company!;
                
                
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

