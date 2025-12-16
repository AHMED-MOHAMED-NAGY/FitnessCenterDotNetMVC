using fitnessCenter.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace fitnessCenter.Controllers
{
    // admin role
    [Role("admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
    }
}
