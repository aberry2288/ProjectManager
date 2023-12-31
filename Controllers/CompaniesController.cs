﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BugTracker.Controllers
{
    [Authorize]
    public class CompaniesController : BTBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTCompanyService _companyService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTRolesService _rolesService;

        public CompaniesController(ApplicationDbContext context, IBTCompanyService companyService, UserManager<BTUser> userManager, IBTRolesService bTRolesService)
        {
            _context = context;
            _companyService = companyService;
            _userManager = userManager;
            _rolesService = bTRolesService;
        }

        //// GET: Companies
        //public async Task<IActionResult> Index()
        //{
        //      return _context.Companies != null ? 
        //                  View(await _context.Companies.ToListAsync()) :
        //                  Problem("Entity set 'ApplicationDbContext.Companies'  is null.");
        //}

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            // 1 - Add an instance of the ViewModel as a List (model)
            List<ManageUserRolesViewModel> model = new List<ManageUserRolesViewModel>();
            // 2 - Get CompanyId

            // 3 - Get all company users
            List<BTUser> members = await _companyService.GetMembersAsync(_companyId);
            // 4 - Loop over the users to populate the ViewModel
            //      - instantiate single ViewModel
            //      - use _rolesService
            //      - Create multiselect
            //      - viewmodel to model
            string? btUserId = _userManager.GetUserId(User);

            foreach (BTUser member in members)
            {
                if (string.Equals(btUserId, member.Id) == false)
                {
                    ManageUserRolesViewModel viewModel = new();

                    IEnumerable<string>? currentRoles = await _rolesService.GetUserRolesAsync(member);

                    viewModel.BTUser = member;

                    viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", currentRoles);

                    model.Add(viewModel);
                }
            }

            // 5 - Return the model to the View
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel viewModel)
        {

            // 2 - Instantiate the BTUser
            BTUser btUser = (await _companyService.GetMembersAsync(_companyId)).FirstOrDefault(m => m.Id == viewModel.BTUser?.Id);

            // 3 - Get Roles for the User
            IEnumerable<string>? currentRoles = await _rolesService.GetUserRolesAsync(btUser);

            // 4 - Get Selected Role(s) for the User
            string? selectedRole = viewModel.SelectedRoles!.FirstOrDefault();

            // 5 - Remove current role(s) and Add new role
            if (!string.IsNullOrEmpty(selectedRole))
            {
                if (await _rolesService.RemoveUserFromRolesAsync(btUser, currentRoles))
                {
                    await _rolesService.AddUserToRoleAsync(btUser, selectedRole);
                }
            }

            // 6 - Navigate
            return RedirectToAction(nameof(ManageUserRoles));
        }

        [HttpPost]
        public async Task<IActionResult> ClearNotifications()
        {
            try
            {
                string? userId = _userManager.GetUserId(User);

                await _companyService.ClearNotificationsByUserIdAsync(userId);

                return RedirectToAction("AdmintoIndex", "Home");
            }
            catch (Exception)
            {

                throw;
            }
        }


        // GET: Companies/Details/5
        public async Task<IActionResult> Details()
        {

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == _companyId);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ImageData,ImageType")] Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageData,ImageType")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Companies == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Companies'  is null.");
            }
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
