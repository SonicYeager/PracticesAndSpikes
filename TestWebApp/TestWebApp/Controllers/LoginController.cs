using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using TestWebApp.Models;

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
            return View(new LoginViewModel() { Password = "Finn the Human", Username = "Jake the Dog" });
        }

        //// GET: /<controller>/Login
        //public async Task<string> Login(string name, int count = 1)
        //{
        //    return HtmlEncoder.Default.Encode($"Hello {name}, Count is: {count}"); ;
        //}
    }
}
