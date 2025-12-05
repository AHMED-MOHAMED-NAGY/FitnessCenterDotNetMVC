using fitnessCenter.Models;
using Microsoft.AspNetCore.Mvc;

namespace fitnessCenter.Controllers
{
    // rote user
    public class UserController : Controller
    {
        private FitnessContext f_db = new FitnessContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult EditProfile(int i) 
        {
            User u1 = f_db.users.FirstOrDefault(x => x.manId == i);
            if (u1 == null) 
            {
                TempData["HataMsj"] = "user is not found !!";
                return RedirectToAction("Hata");
            }
            // select user from db with user id
            return View(u1);
        }
    }
}
