using Microsoft.AspNetCore.Mvc;

namespace fitnessCenter.Controllers
{
    // admin role
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
    }
}
