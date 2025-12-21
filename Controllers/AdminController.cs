using fitnessCenter.Attributes;
using Microsoft.AspNetCore.Mvc;
using fitnessCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace fitnessCenter.Controllers
{
    // admin role
    [Role("admin")]
    public class AdminController : Controller
    {
        private FitnessContext f_db = new FitnessContext();
        public IActionResult Index()
        {
            return View();
        }
        // for user 
        public IActionResult ListUsers()
        {
            List<User> users = f_db.users.ToList();
            return View(users);
        }
        public IActionResult DetailsUser(int id)
        {
            User? user = f_db.users.OfType<User>()
                .Include(u => u.dailyGoal)   // Now you can include User-specific items
                .Include(u => u.exercise)    // And another...
                .FirstOrDefault(x => x.manId == id);

            return View(user);
        }
        public IActionResult DeleteUser(int id) 
        {
            bool isAdminAndLogin = HttpContext.Session.GetString("Role") == "admin" && HttpContext.Session.GetInt32("UserId") == id;
            User? user = f_db.users.FirstOrDefault(x => x.manId == id);
            if (user != null)
            {
                f_db.users.Remove(user);
                f_db.SaveChanges();
                TempData["msg"] = "User deleted successfully.";
            }
            else
            {
                TempData["err"] = "User not found.";
            }
            if (isAdminAndLogin)
            {
                // if admin delete himself logout
                return RedirectToAction("Logout", "Home");
            }
            return RedirectToAction("ListUsers");
        }
        public IActionResult MakeUsrAdmin(int id)
        {
            User? user = f_db.users.FirstOrDefault(x => x.manId == id);
            if (user != null)
            {
                Admin a = new Admin();
                a.manId = id;
                a.name = user.name;
                a.email = user.email;
                a.userName = user.userName;
                a.passwordHash = user.passwordHash;
                a.boy = user.boy;
                a.wight = user.wight;
                a.age = user.age;
                a.number = user.number;
                a.whoIam = Roles.admin;

                f_db.Remove(user);
                f_db.SaveChanges();
                f_db.admins.Add(a);
                f_db.SaveChanges();

                TempData["msg"] = "User promoted to admin successfully.";
            }
            else
            {
                TempData["err"] = "User not found.";
            }
            return RedirectToAction("ListUsers");
        }
        public IActionResult MakeUsrCotch(int id)
        {
            User? user = f_db.users.FirstOrDefault(x => x.manId == id);
            if (user != null)
            {
                Cotch c = new Cotch();
                c.manId = user.manId;
                c.name = user.name;
                c.email = user.email;
                c.userName = user.userName;
                c.passwordHash = user.passwordHash;
                c.boy = user.boy;
                c.wight = user.wight;
                c.age = user.age;
                c.number = user.number;
                c.whoIam = Roles.cotch;

                c.cotch_status = "new";

                f_db.Remove(user);
                f_db.SaveChanges();
                f_db.cotches.Add(c);
                f_db.SaveChanges();
                TempData["msg"] = "User promoted to admin successfully.";
                if (HttpContext.Session.GetInt32("UserId") == id) 
                {
                    HttpContext.Session.SetString("Role", user.whoIam.ToString());
                }
            }
            else
            {
                TempData["err"] = "User not found.";
            }
            return RedirectToAction("ListUsers");
        }

        // for cotch
        public IActionResult ListCotches()
        {
            List<Cotch> cotches = f_db.cotches.ToList();
            return View(cotches);
        }
        public IActionResult DetailsCotch(int id)
        {
            Cotch? cotch = f_db.cotches.OfType<Cotch>()
                .Include(u => u.manId)   // Now you can include User-specific items
                .FirstOrDefault(x => x.manId == id);

            return View(cotch);
        }
        public IActionResult DeleteCotch(int id)
        {
            bool isAdminAndLogin = HttpContext.Session.GetString("Role") == "admin" && HttpContext.Session.GetInt32("UserId") == id;
            Cotch? cotch = f_db.cotches.FirstOrDefault(x => x.manId == id);
            if (cotch != null)
            {
                // 1. Delete all appointments for this coach
                var appointments = f_db.appointments.Where(a => a.CotchId == id).ToList();
                if (appointments.Any())
                {
                    f_db.appointments.RemoveRange(appointments);
                }

                // 2. Delete all notifications sent to this coach
                var notifications = f_db.notifications.Where(n => n.ManId == id).ToList();
                if (notifications.Any())
                {
                    f_db.notifications.RemoveRange(notifications);
                }

                // 3. Handle Students
                var students = f_db.users.Where(u => u.CotchId == id).ToList();
                if (students.Any())
                {
                    foreach (var student in students)
                    {
                        // Notify student
                        Notification n = new Notification(
                            "Coach Update", 
                            $"Your coach {cotch.name} is no longer available. Please select a new coach from the dashboard."
                        );
                        n.ManId = student.manId;
                        f_db.notifications.Add(n);

                        // Unassign coach
                        student.CotchId = null;
                        // student.Cotch = null; // EF Core handles invalidation
                    }
                }

                // 4. Remove the coach
                f_db.cotches.Remove(cotch);
                f_db.SaveChanges();
                TempData["msg"] = "Coach deleted, students notified, and data cleaned up.";
            }
            else
            {
                TempData["err"] = "cotch not found.";
            }
            if (isAdminAndLogin)
            {
                // if admin delete himself logout
                return RedirectToAction("Logout", "Home");
            }
            return RedirectToAction("ListCotches");
        }
        public IActionResult MakeCtchAdmin(int id)
        {
            Cotch? cotch = f_db.cotches.FirstOrDefault(x => x.manId == id);
            if (cotch != null)
            {
                Admin a = new Admin();
                a.manId = id;
                a.name = cotch.name;
                a.email = cotch.email;
                a.userName = cotch.userName;
                a.passwordHash = cotch.passwordHash;
                a.boy = cotch.boy;
                a.wight = cotch.wight;
                a.age = cotch.age;
                a.number = cotch.number;
                a.whoIam = Roles.admin;

                f_db.Remove(cotch);
                f_db.SaveChanges();
                f_db.admins.Add(a);
                f_db.SaveChanges();

                TempData["msg"] = "Cotch promoted to admin successfully.";
            }
            else
            {
                TempData["err"] = "Cotch not found.";
            }
            return RedirectToAction("ListCotches");
        }
        // for admin
        public IActionResult ListAdmins()
        {
            List<Admin> admins= f_db.admins.ToList();
            return View(admins);
        }
        public IActionResult DetailsAdmin(int id)
        {
            Admin? admin = f_db.admins.OfType<Admin>()
                .FirstOrDefault(x => x.manId == id);

            return View(admin);
        }

        public IActionResult Edit()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction("Login", "Home");

            Admin? admin = f_db.admins.FirstOrDefault(a => a.manId == id);
            
            if (admin == null)
            {
                TempData["err"] = "Admin profile not found.";
                return RedirectToAction("Index");
            }
            return View(admin);
        }

        [HttpPost]
        public IActionResult EditResult(Admin adminData)
        {
            // Remove validation for fields not in the form
            ModelState.Remove("passwordHash");
            ModelState.Remove("password");
            ModelState.Remove("compare_password");
            ModelState.Remove("wight");
            ModelState.Remove("boy");
            ModelState.Remove("age");

            if (ModelState.IsValid)
            {
                var dbAdmin = f_db.admins.FirstOrDefault(a => a.manId == adminData.manId);
                
                if (dbAdmin == null)
                {
                     int? id = HttpContext.Session.GetInt32("UserId");
                     dbAdmin = f_db.admins.FirstOrDefault(a => a.manId == id);
                }

                if (dbAdmin != null)
                {
                    dbAdmin.name = adminData.name;
                    dbAdmin.email = adminData.email;
                    dbAdmin.userName = adminData.userName;
                    dbAdmin.number = adminData.number;
                    
                    f_db.Update(dbAdmin);
                    f_db.SaveChanges();
                    TempData["msg"] = "Profile updated successfully.";
                    return RedirectToAction("Index");
                }
            }
            
            var errors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            TempData["err"] = "Update failed: " + (string.IsNullOrWhiteSpace(errors) ? "Invalid input" : errors);

            return View("Edit", adminData);
        }
        public IActionResult DeleteAdmin(int id)
        {
            bool isAdminAndLogin = HttpContext.Session.GetString("Role") == "admin" && HttpContext.Session.GetInt32("UserId") == id;
            Admin? admin = f_db.admins.FirstOrDefault(x => x.manId == id);
            if (admin != null)
            {
                f_db.admins.Remove(admin);
                f_db.SaveChanges();
                TempData["msg"] = "admin deleted successfully.";
            }
            else
            {
                TempData["err"] = "admin not found.";
            }
            if (isAdminAndLogin)
            {
                // if admin delete himself logout
                return RedirectToAction("Logout", "Home");
            }
            return RedirectToAction("ListAdmins");
        }
        public IActionResult MakeAdminCtch(int id)
        {
            Admin? admin = f_db.admins.FirstOrDefault(x => x.manId == id);
            if (admin != null)
            {
                Cotch c = new Cotch();
                c.manId = id;
                c.name = admin.name;
                c.email = admin.email;
                c.userName = admin.userName;
                c.passwordHash = admin.passwordHash;
                c.boy = admin.boy;
                c.wight = admin.wight;
                c.age = admin.age;
                c.number = admin.number;
                c.whoIam = Roles.admin;

                f_db.Remove(admin);
                f_db.SaveChanges();
                f_db.cotches.Add(c);
                f_db.SaveChanges();

                TempData["msg"] = "Admin promoted to admin successfully.";
            }
            else
            {
                TempData["err"] = "Admin not found.";
            }
            return RedirectToAction("ListAdmins");
        }
        public IActionResult SendNotification(int id)
        {
            var man = f_db.men.FirstOrDefault(u => u.manId == id);
            if (man == null)
            {
                TempData["err"] = "User not found";
                return RedirectToAction("Index");
            }
            return View(man);
        }

        [HttpPost]
        public IActionResult SendNotificationResult(int id, string title, string msj)
        {
            var man = f_db.men.FirstOrDefault(u => u.manId == id);
            
            if (man != null)
            {
                Notification n = new Notification(title, msj);
                n.ManId = man.manId;
                f_db.notifications.Add(n);
                f_db.SaveChanges();
                
                TempData["msg"] = "Notification sent successfully";
                
                if (man.whoIam == Roles.cotch) return RedirectToAction("ListCotches");
                if (man.whoIam == Roles.user) return RedirectToAction("ListUsers");
                return RedirectToAction("ListAdmins");
            }
            
            TempData["err"] = "Failed to send notification";
            return RedirectToAction("Index");
        }

        // Applications Management
        public IActionResult Applications()
        {
            var vm = new AdminApplicationsViewModel
            {
                NewUsers = f_db.users.Where(u => u.subscribeStatus == "New").ToList(),
                AppointmentHistory = f_db.appointments
                    .Include(a => a.User)
                    .Include(a => a.Cotch)
                    .Include(a => a.Exercise)
                    .Where(a => a.Status != "Pending")
                    .OrderByDescending(a => a.Id)
                    .ToList()
            };
            return View(vm);
        }

        public IActionResult ApproveUser(int id)
        {
            var user = f_db.users.FirstOrDefault(u => u.manId == id);
            if (user != null)
            {
                user.subscribeStatus = "Active";
                f_db.Update(user);
                f_db.SaveChanges();
                TempData["msg"] = $"User {user.name} approved successfully.";
            }
            return RedirectToAction("Applications");
        }

        public IActionResult RejectUser(int id)
        {
            var user = f_db.users.FirstOrDefault(u => u.manId == id);
            if (user != null)
            {
                f_db.users.Remove(user);
                f_db.SaveChanges();
                TempData["msg"] = "Application removed.";
            }
            return RedirectToAction("Applications");
        }

        public IActionResult ApplyAll()
        {
            var newUsers = f_db.users.Where(u => u.subscribeStatus == "New").ToList();
            if (newUsers.Any())
            {
                foreach (var user in newUsers)
                {
                    user.subscribeStatus = "Active";
                }
                f_db.SaveChanges();
                TempData["msg"] = "All applications approved.";
            }
            return RedirectToAction("Applications");
        }

        public IActionResult RemoveAll()
        {
            var newUsers = f_db.users.Where(u => u.subscribeStatus == "New").ToList();
            if (newUsers.Any())
            {
                f_db.users.RemoveRange(newUsers);
                f_db.SaveChanges();
                TempData["msg"] = "All applications removed.";
            }
            return RedirectToAction("Applications");
        }

        // Exercise Management
        public IActionResult Exercises()
        {
            var exercises = f_db.exercises.ToList();
            return View(exercises);
        }

        [HttpPost]
        public IActionResult AddExercise(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Exercise newEx = new Exercise
                {
                    exerciseType = name
                };
                f_db.exercises.Add(newEx);
                f_db.SaveChanges();
                TempData["msg"] = "Exercise added successfully.";
            }
            else
            {
                TempData["err"] = "Exercise name cannot be empty.";
            }
            return RedirectToAction("Exercises");
        }

        public IActionResult DeleteExercise(int id)
        {
            var ex = f_db.exercises.Find(id);
            if (ex != null)
            {
                f_db.exercises.Remove(ex);
                f_db.SaveChanges();
                TempData["msg"] = "Exercise deleted successfully.";
            }
            return RedirectToAction("Exercises");
        }

        // Appointment Management
        public IActionResult Appointments()
        {
            var appointments = f_db.appointments
                .Include(a => a.User)
                .Include(a => a.Cotch)
                .Include(a => a.Exercise)
                .Where(a => a.Status == "Pending")
                .OrderByDescending(a => a.Id)
                .ToList();
            return View(appointments);
        }

        public IActionResult ApproveAppointment(int id)
        {
            var appt = f_db.appointments
                .Include(a => a.Cotch)
                .Include(a => a.User)
                .FirstOrDefault(a => a.Id == id);

            if (appt != null)
            {
                ProcessApproval(appt);
                f_db.SaveChanges();
                TempData["msg"] = "Appointment approved, time slot removed, and coach notified.";
            }
            return RedirectToAction("Appointments");
        }

        public IActionResult RejectAppointment(int id)
        {
            var appt = f_db.appointments.Find(id);
            if (appt != null)
            {
                appt.Status = "Rejected"; 
                f_db.SaveChanges();
                TempData["msg"] = "Appointment rejected.";
            }
            return RedirectToAction("Appointments");
        }

        public IActionResult ApproveAllAppointments()
        {
            var pendingAppts = f_db.appointments
                .Include(a => a.Cotch)
                .Include(a => a.User)
                .Where(a => a.Status == "Pending")
                .ToList();

            if (pendingAppts.Any())
            {
                foreach (var appt in pendingAppts)
                {
                    ProcessApproval(appt);
                }
                f_db.SaveChanges();
                TempData["msg"] = "All pending appointments approved.";
            }
            return RedirectToAction("Appointments");
        }

        public IActionResult RejectAllAppointments()
        {
            var pendingAppts = f_db.appointments
                .Where(a => a.Status == "Pending")
                .ToList();

            if (pendingAppts.Any())
            {
                foreach (var appt in pendingAppts)
                {
                    appt.Status = "Rejected";
                }
                f_db.SaveChanges();
                TempData["msg"] = "All pending appointments rejected.";
            }
            return RedirectToAction("Appointments");
        }

        private void ProcessApproval(Appointment appt)
        {
            appt.Status = "Approved";

            // 1. Remove booked time from Coach's available times
            if (appt.Cotch != null && appt.Cotch.available_times != null)
            {
                if (appt.Cotch.available_times.Contains(appt.AppointmentDate))
                {
                    appt.Cotch.available_times.Remove(appt.AppointmentDate);
                    f_db.Update(appt.Cotch);
                }
            }

            // 2. Notify the Coach
            if (appt.Cotch != null && appt.User != null)
            {
                string notifTitle = "New Appointment Confirmed";
                string notifBody = $"System: You have a new session with {appt.User.name} on {appt.AppointmentDate}.";

                Notification n = new Notification(notifTitle, notifBody);
                n.ManId = appt.Cotch.manId;
                f_db.notifications.Add(n);

                // 3. Assign Coach to User
                appt.User.CotchId = appt.CotchId;
                f_db.Update(appt.User);
            }
        }
    }
}
