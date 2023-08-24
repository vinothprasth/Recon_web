using Microsoft.AspNetCore.Mvc;

namespace ReconWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
