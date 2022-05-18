using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace TestWebApp.Controllers
{
    public class LoginController : Controller
    {

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: /<controller>/Login
        public async Task<IActionResult> Login()
        {
            ViewData["Username"] = "Jacke the Dog";
            ViewData["Password"] = "Finn the Human";
            return View();
        }

        //// GET: /<controller>/Login
        //public async Task<string> Login(string name, int count = 1)
        //{
        //    return HtmlEncoder.Default.Encode($"Hello {name}, Count is: {count}"); ;
        //}
    }
}
