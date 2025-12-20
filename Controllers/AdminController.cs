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
                f_db.cotches.Remove(cotch);
                f_db.SaveChanges();
                TempData["msg"] = "cotch deleted successfully.";
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
    }
}
