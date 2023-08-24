using Microsoft.AspNetCore.Mvc;

namespace ReconWeb.Controllers
{
    public class ReconController : Controller
    {
        public IActionResult ReconView()
        {
            return View();
        }
    }
}
