using fitnessCenter.Models;
using fitnessCenter.Services;
using fitnessCenter.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fitnessCenter.Controllers
{
    // rote user
    [Role("user")]
    public class UserController : Controller
    {
        private FitnessContext f_db = new FitnessContext();
        private readonly AIService _aiService;

        public UserController(AIService aiService)
        {
            _aiService = aiService;
        }

        public IActionResult Index()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null)
            {
                return RedirectToAction("Login", "Home"); // Redirect if not logged in
            }

            User? user = f_db.men.OfType<User>()
                .Include(u => u.dailyGoal)
                .Include(u => u.exercise)
                .FirstOrDefault(x => x.manId == id);

            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            return View(user);
        }

        public IActionResult PlanSelection(string type)
        {
            ViewBag.CurrentPlanType = type;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePlan(string planType, int duration, IFormFile image)
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction("Login", "Home");

            var user = f_db.men.OfType<User>().FirstOrDefault(u => u.manId == id);
            if (user == null) return RedirectToAction("Login", "Home");

            // Generate Plan using AI Service
            var (planText, afterImageUrl) = await _aiService.GenerateFitnessPlan(user, planType, duration, image);

            // Handle "Before" Image (converting to base64 for display to avoid storage complexity for now, or could save to wwwroot)
            string beforeImageBase64 = null;
            if (image != null && image.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await image.CopyToAsync(ms);
                    beforeImageBase64 = $"data:{image.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";
                }
            }

            var viewModel = new ShowPlanViewModel
            {
                PlanText = planText,
                BeforeImageUrl = beforeImageBase64,
                AfterImageUrl = afterImageUrl,
                PlanType = planType,
                Duration = duration
            };

            return View("ShowPlan", viewModel); 
        }

        public IActionResult ShowPlan()
        {
            return View(); // Ideally shouldn't be called directly without model, or could show history
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

        public IActionResult MakeAppointment()
        {
            // Placeholder: In real app, load Exercises and Cotchs here
            return View();
        }

    }
}
