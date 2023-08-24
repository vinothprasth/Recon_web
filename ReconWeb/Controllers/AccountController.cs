using Microsoft.AspNetCore.Mvc;

namespace ReconWeb.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Accountview()
        {
            return View();
        }
    }
}
