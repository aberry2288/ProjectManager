using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;

namespace BugTracker.Controllers
{
    public class HomeController : BTBaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBTCompanyService _btCompanyService;
        private readonly IProjectService _projectService;
        private readonly IBTTicketService _ticketService;

        public HomeController(ILogger<HomeController> logger, IBTCompanyService bTCompanyService, IProjectService projectService, IBTTicketService bTTicketService)
        {
            _logger = logger;
            _btCompanyService = bTCompanyService;
            _projectService = projectService;
            _ticketService = bTTicketService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AdmintoIndex()
        {
            DashboardViewModel dashboardViewModel = new DashboardViewModel();

            dashboardViewModel.Company = await _btCompanyService.GetCompanyInfoAsync(_companyId);
            dashboardViewModel.Projects = await _projectService.GetAllProjectsByCompanyIdAsync(_companyId);
            dashboardViewModel.Tickets = await _ticketService.GetAllTicketsByCompanyIdAsync(_companyId);
            dashboardViewModel.Members = await _btCompanyService.GetMembersAsync(_companyId);



            return View(dashboardViewModel);
        }

        public async Task<IActionResult> Landing()
        {
            return View();
        } 

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}