using Microsoft.AspNetCore.Mvc;
using SmartSaver.Domain.CustomAttributes;

namespace SmartSaver.MVC.Controllers
{
    [AnonymousOnly]
    public class HomeController : Controller
    {
        public HomeController() { }

        public IActionResult Index()
        {
            return View();
        }
    }
}
