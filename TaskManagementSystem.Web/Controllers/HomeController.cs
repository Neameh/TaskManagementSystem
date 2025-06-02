using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Web.Models;
using TaskManagementSystem.Web.ViewModels;

namespace TaskManagementSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Route("Home/Error")]
        public IActionResult Error(string? statusCode = null)
        {
            return View("Error", new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = statusCode switch
                {
                    "404" => "Page not found.",
                    "403" => "Access denied.",
                    _ => "An unexpected error occurred."
                }
            });
        }
        public IActionResult Index()
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
