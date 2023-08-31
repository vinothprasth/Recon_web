using Microsoft.AspNetCore.Mvc;

namespace ReconWeb.Controllers
{
    public class ReconController : Controller
    {
        public IActionResult ReconView()
        {
            return View();
        }
		public IActionResult ReconList()
		{
			return View();
		}
	}
}
