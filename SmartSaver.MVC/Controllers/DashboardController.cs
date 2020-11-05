using System;
using SmartSaver.EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace SmartSaver.MVC.Controllers {
    public class DashboardController : Controller {

        private readonly ILogger<DashboardController> _logger;
        private readonly ApplicationDbContext _context;

        public DashboardController(ILogger<DashboardController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public IActionResult Index() 
        {
            Account account = _context.Accounts.FirstOrDefault(x => x.Id == 1);
            DashboardViewModel viewModel = new DashboardViewModel() 
            {
                
            }
            return View();
        }
    }
}