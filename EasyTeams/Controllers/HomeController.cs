using EasyTeams.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace EasyTeams.Controllers
{
    // HomeController class
    public class HomeController : Controller
    {

        // logger field
        private readonly ILogger<HomeController> _logger;

        // HomeController constructor
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Index action
        public IActionResult Index()
        {
            var currentUserId = HttpContext.Session.GetString("currentUserId");
            return View();
        }

        // Privacy action
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        // Error action
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
