using Microsoft.AspNetCore.Mvc;

namespace TestWebApp.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
