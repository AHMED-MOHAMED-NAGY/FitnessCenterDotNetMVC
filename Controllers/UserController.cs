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
        private readonly FitnessContext f_db;
        private readonly AIService _aiService;

        public UserController(FitnessContext context, AIService aiService)
        {
            f_db = context;
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
                .Include(u => u.cotch)
                .FirstOrDefault(x => x.manId == id);

            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Enforce Profile Completion (Weight & Height) for Active Users
            if (user.subscribeStatus == "Active" && (user.wight == 0 || user.boy == 0))
            {
                TempData["msg"] = "Please complete your profile (Weight & Height) to access the dashboard.";
                return RedirectToAction("EditProfile");
            }
            
            // Check for active appointment
            var activeAppt = f_db.appointments
                .Include(a => a.Cotch)
                .Include(a => a.Exercise)
                .FirstOrDefault(a => a.UserId == id && a.Status != "Rejected");
                
            ViewBag.CurrentAppointment = activeAppt;

            return View(user);
        }

        public IActionResult MyAppointments()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction("Login", "Home");



            // Enforce Profile Completion
            var user = f_db.users.FirstOrDefault(u => u.manId == id);
            if (user != null && user.subscribeStatus == "Active" && (user.wight == 0 || user.boy == 0))
            {
                TempData["msg"] = "Please complete your profile (Weight & Height) first.";
                return RedirectToAction("EditProfile");
            }

            var appt = f_db.appointments
                .Include(a => a.Cotch)
                .Include(a => a.Exercise)
                .FirstOrDefault(a => a.UserId == id && a.Status != "Rejected");

            if (appt == null)
            {
                TempData["msg"] = "You don't have any active appointments.";
                return RedirectToAction("Index");
            }

            return View(appt);
        }

        public IActionResult CancelAppointment(int id)
        {
            var appt = f_db.appointments.Find(id);
            if (appt != null)
            {
                appt.Status = "CancellationPending";
                f_db.SaveChanges();
                TempData["msg"] = "Cancellation requested. Waiting for Admin approval.";
            }
            return RedirectToAction("MyAppointments");
        }

        [HttpPost]
        public IActionResult RescheduleAppointment(int id, string newDate)
        {
            var appt = f_db.appointments
                .Include(a => a.Cotch)
                .FirstOrDefault(a => a.Id == id);

            if (appt != null && !string.IsNullOrEmpty(newDate))
            {
                // If previously approved, return the old slot to the coach
                if (appt.Status == "Approved" && appt.Cotch != null)
                {
                    if (appt.Cotch.available_times == null) appt.Cotch.available_times = new List<string>();
                    
                    // Add old date back if not present
                    if (!string.IsNullOrEmpty(appt.AppointmentDate) && !appt.Cotch.available_times.Contains(appt.AppointmentDate))
                    {
                        appt.Cotch.available_times.Add(appt.AppointmentDate);
                        f_db.Update(appt.Cotch);
                    }
                }

                appt.AppointmentDate = newDate;
                appt.Status = "Pending"; // Needs re-approval
                f_db.SaveChanges();
                TempData["msg"] = "Reschedule requested. Waiting for Admin approval.";
            }
            return RedirectToAction("MyAppointments");
        }

        public IActionResult PlanSelection(string type)
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction("Login", "Home");

            // Enforce Profile Completion
            var user = f_db.users.FirstOrDefault(u => u.manId == id);
            if (user != null && user.subscribeStatus == "Active" && (user.wight == 0 || user.boy == 0))
            {
                TempData["msg"] = "Please complete your profile (Weight & Height) first.";
                return RedirectToAction("EditProfile");
            }

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
        [HttpPost]
        public IActionResult EditProfileResult(User u1)
        {
            // Remove fields not present in form or handled manually
            ModelState.Remove("password");
            ModelState.Remove("compare_password");
            ModelState.Remove("passwordHash");
            ModelState.Remove("dailyGoal");
            ModelState.Remove("cotch");
            ModelState.Remove("exercise");
            ModelState.Remove("notifications");

            if (ModelState.IsValid)
            {
                int? id = HttpContext.Session.GetInt32("UserId");
                var dbUser = f_db.users.FirstOrDefault(u => u.manId == id);
                
                if (dbUser != null)
                {
                    // Enforce Height/Weight validation
                    if (u1.wight <= 0 || u1.boy <= 0)
                    {
                        TempData["err"] = "Weight and Height are required and must be valid numbers.";
                        return RedirectToAction("EditProfile");
                    }

                    // Update only editable fields
                    dbUser.name = u1.name;
                    dbUser.email = u1.email;
                    dbUser.age = u1.age;
                    dbUser.wight = u1.wight;
                    dbUser.boy = u1.boy; // Height
                    dbUser.number = u1.number;
                    
                    f_db.Update(dbUser);
                    f_db.SaveChanges();
                    TempData["msg"] = "Profile updated successfully!";
                    return RedirectToAction("Index");
                }
            }
            
            // Capture errors for debugging if needed
            var errors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            TempData["err"] = "Update failed: " + (string.IsNullOrWhiteSpace(errors) ? "Please check your input." : errors);
            return RedirectToAction("EditProfile");
        }

        public IActionResult SendToAdmin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendToAdminResult(string title, string msj)
        {
            // Broadcast to all admins (checking Role enum)
            var admins = f_db.men.Where(m => m.whoIam == Roles.admin).ToList();
            
            if (admins.Any())
            {
                foreach (var admin in admins)
                {
                    Notification n = new Notification(title, msj);
                    n.ManId = admin.manId;
                    f_db.notifications.Add(n);
                }
                f_db.SaveChanges();
                TempData["msg"] = "Message sent to Admins successfully";
                return RedirectToAction("Index");
            }
            
            TempData["err"] = "No Admins found to message";
            return RedirectToAction("Index");
        }

        public IActionResult SendToCotch()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            var user = f_db.users.Include(u => u.cotch).FirstOrDefault(u => u.manId == id);
            
            if (user == null || user.cotch == null)
            {
                TempData["err"] = "You don't have a coach assigned yet.";
                return RedirectToAction("Index"); 
            }

            return View(user.cotch);
        }

        [HttpPost]
        public IActionResult SendToCotchResult(string title, string msj)
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            var user = f_db.users.FirstOrDefault(u => u.manId == id);

            if (user != null && user.CotchId != null)
            {
                // Customize message to show who sent it
                string fullTitle = $"From Student {user.name}: {title}";
                
                Notification n = new Notification(fullTitle, msj);
                n.ManId = user.CotchId.Value;
                
                f_db.notifications.Add(n);
                f_db.SaveChanges();
                
                TempData["msg"] = "Message sent to Coach successfully";
                return RedirectToAction("Index");
            }

            TempData["err"] = "Failed to send message";
            return RedirectToAction("Index");
        }

        public IActionResult Err()
        {
            return View();
        }

        public IActionResult MakeAppointment()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction("Login", "Home");

            // Enforce Profile Completion
            var user = f_db.users.FirstOrDefault(u => u.manId == id);
            if (user != null && user.subscribeStatus == "Active" && (user.wight == 0 || user.boy == 0))
            {
                TempData["msg"] = "Please complete your profile (Weight & Height) first.";
                return RedirectToAction("EditProfile");
            }

            ViewBag.Exercises = f_db.exercises.ToList();
            ViewBag.Coaches = f_db.cotches.ToList();
            return View();
        }

        [HttpGet]
        public IActionResult GetCoachTimes(int id)
        {
            var coach = f_db.cotches.FirstOrDefault(c => c.manId == id);
            if (coach != null && coach.available_times != null)
            {
                return Json(coach.available_times);
            }
            return Json(new List<string>());
        }

        [HttpGet]
        public IActionResult GetCoachesByExercise(int exerciseId)
        {
            var coaches = f_db.cotches
                .Where(c => c.ExerciseId == exerciseId) 
                .Select(c => new { value = c.manId, text = c.name })
                .ToList();
            return Json(coaches);
        }

        [HttpPost]
        public IActionResult BookAppointment(int exerciseId, int coachId, string appointmentDate)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Home");

            if (exerciseId != 0 && coachId != 0 && !string.IsNullOrEmpty(appointmentDate))
            {
                Appointment appt = new Appointment
                {
                    UserId = userId.Value,
                    CotchId = coachId,
                    ExerciseId = exerciseId,
                    AppointmentDate = appointmentDate,
                    Status = "Pending"
                };

                f_db.appointments.Add(appt);
                f_db.SaveChanges();
                TempData["msg"] = "Appointment requested successfully! Waiting for Admin approval.";
                return RedirectToAction("Index");
            }
            
            TempData["err"] = "Please select all required fields.";
            return RedirectToAction("MakeAppointment");
        }




        public IActionResult SetDailyGoal()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction("Login", "Home");
            
            var user = f_db.users
                .Include(u => u.dailyGoal)
                .FirstOrDefault(u => u.manId == id);
            
            if (user == null) return RedirectToAction("Login", "Home");

            // Pass existing goal if any, for editing
            return View(user.dailyGoal);
        }

        [HttpPost]
        public IActionResult SetDailyGoalResult(string goal)
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction("Login", "Home");
            
            var user = f_db.users
                .Include(u => u.dailyGoal)
                .FirstOrDefault(u => u.manId == id);

            if (user != null)
            {
                if (user.dailyGoal != null)
                {
                    // Update existing
                    user.dailyGoal.goal = goal;
                    user.dailyGoal.date = DateTime.Now.ToString("yyyy-MM-dd");
                    user.dailyGoal.status = false;
                    f_db.dailyGoals.Update(user.dailyGoal);
                }
                else
                {
                    // Create new
                    DailyGoal dg = new DailyGoal(goal);
                    f_db.dailyGoals.Add(dg);
                    user.dailyGoal = dg;
                }
                f_db.SaveChanges();
                TempData["msg"] = "Daily Goal set successfully!";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public IActionResult ToggleGoalStatus()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction("Login", "Home");

            var user = f_db.users.Include(u => u.dailyGoal).FirstOrDefault(u => u.manId == id);
            
            if (user != null && user.dailyGoal != null)
            {
                user.dailyGoal.status = !user.dailyGoal.status; // Toggle status
                f_db.Update(user.dailyGoal);
                f_db.SaveChanges();
                TempData["msg"] = user.dailyGoal.status ? "Goal marked as Completed! Great job!" : "Goal marked as In Progress.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Subscribe()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Subscribe(string planType)
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction("Login", "Home");

            var user = f_db.users.FirstOrDefault(u => u.manId == id);
            if (user != null)
            {
                user.SubscriptionPlan = planType;
                user.subscribeStatus = "PendingPayment";
                // Optionally set start date here or wait for admin approval
                f_db.Update(user);
                f_db.SaveChanges();
                TempData["msg"] = "Subscription requested! Please wait for Admin approval.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
