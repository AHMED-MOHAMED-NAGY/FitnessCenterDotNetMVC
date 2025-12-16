using fitnessCenter.Attributes;
using fitnessCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fitnessCenter.Controllers
{
    // rote user
    [Role("user")]
    public class UserController : Controller
    {
        private FitnessContext f_db = new FitnessContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Test()
        {
            return RedirectToAction("EditProfile");
        }
        public IActionResult EditProfile() 
        {
            int id = (int)HttpContext.Session.GetInt32("UserId"); // get id from session
            // select user from db with user id
            User? u1 = f_db.men.OfType<User>()
                //.Include(u => u.dailyGoal)   // Now you can include User-specific items
                //.Include(u => u.exercise)    // And another...
                .FirstOrDefault(x => x.manId == id);            //if (u1 == null) 
            if (u1 == null)
            {
                TempData["err"] = "user is not found !!";
                return RedirectToAction("Err");
            }
            return View(u1);
        }
        public IActionResult EditProfileResult(User u1)
        {
            ModelState.Remove("compare_password");
            ModelState.Remove("password");
            if (ModelState.IsValid)
            {
                f_db.Update(u1);
                f_db.SaveChanges();
                return RedirectToAction("Index");
            }
            TempData["err"] = "Please fill all input correctly!!";
            return RedirectToAction("EditProfile");
        }

        public IActionResult Err()
        {
            return View();
        }

    }
}
