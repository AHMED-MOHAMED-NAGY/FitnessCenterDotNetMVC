using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fitnessCenter.Models;
using fitnessCenter.Attributes;

namespace fitnessCenter.Controllers
{
    [Role("cotch")]
    public class CotchController : Controller
    {
        private readonly FitnessContext f_db;

        public CotchController(FitnessContext context)
        {
            f_db = context;
        }


        // GET: Cotch
        public async Task<IActionResult> Index()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction("Login", "Home");

            var cotch = await f_db.cotches.FirstOrDefaultAsync(m => m.manId == id);
            
            // Fallback: If user is logged in as Coach but not in Cotches table (Data inconsistency)
            if (cotch == null)
            {
                var man = await f_db.men.FindAsync(id);
                if (man != null && man.whoIam == Roles.cotch)
                {
                    // Create a temporary Cotch object to allow the view to render
                    cotch = new Cotch
                    {
                        manId = man.manId,
                        name = man.name,
                        userName = man.userName,
                        email = man.email,
                        whoIam = Roles.cotch,
                        cotch_status = "Active (Fallback)",
                        available_times = new List<string> { "Not set" }
                    };
                }
                else
                {
                     return RedirectToAction("Login", "Home");
                }
            }

            return View(cotch);
        }

        // GET: Cotch/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotch = await f_db.cotches
                .FirstOrDefaultAsync(m => m.manId == id);
            if (cotch == null)
            {
                return NotFound();
            }

            return View(cotch);
        }

        // GET: Cotch/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cotch/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("cotch_status,available_times,manId,name,userName,email,passwordHash,boy,wight,age,number,whoIam")] Cotch cotch)
        {
            if (ModelState.IsValid)
            {
                f_db.Add(cotch);
                await f_db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cotch);
        }

        // GET: Cotch/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotch = await f_db.cotches.FindAsync(id);
            if (cotch == null)
            {
                return NotFound();
            }

            // Convert List<string> to newline-separated string for the textarea
            ViewBag.AvailableTimesStr = cotch.available_times != null 
                ? string.Join("\n", cotch.available_times) 
                : "";
                
            ViewData["ExerciseId"] = new SelectList(f_db.exercises, "exId", "exerciseType", cotch.ExerciseId);

            return View(cotch);
        }

        // POST: Cotch/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("cotch_status,manId,name,userName,email,passwordHash,boy,wight,age,number,whoIam,ExerciseId")] Cotch cotch, string availableTimesStr)
        {
            if (id != cotch.manId)
            {
                return NotFound();
            }

            // Handle Available Times conversion
            if (!string.IsNullOrEmpty(availableTimesStr))
            {
                cotch.available_times = availableTimesStr
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }
            else
            {
                cotch.available_times = new List<string>();
            }

            // Remove available_times from ModelState validation since we handle it manually
            ModelState.Remove("available_times");
            ModelState.Remove("password");
            ModelState.Remove("compare_password");
            ModelState.Remove("Exercise");

            if (ModelState.IsValid)
            {
                try
                {
                    f_db.Update(cotch);
                    await f_db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CotchExists(cotch.manId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExerciseId"] = new SelectList(f_db.exercises, "exId", "exerciseType", cotch.ExerciseId);
            return View(cotch);
        }

        // GET: Cotch/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotch = await f_db.cotches
                .FirstOrDefaultAsync(m => m.manId == id);
            if (cotch == null)
            {
                return NotFound();
            }

            return View(cotch);
        }

        // POST: Cotch/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cotch = await f_db.cotches.FindAsync(id);
            if (cotch != null)
            {
                f_db.cotches.Remove(cotch);
            }

            await f_db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CotchExists(int id)
        {
            return f_db.cotches.Any(e => e.manId == id);
        }

        public IActionResult MyStudents()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null) return RedirectToAction("Login", "Home");

            // Assuming relation is via User.cotch/CotchId ? 
            // Wait, looking at User model: public int? CotchId { get; set; }
            // So we find users where CotchId == id
            
            var students = f_db.users
                .Include(u => u.dailyGoal)
                .Where(u => u.CotchId == id).ToList();
            return View(students);
        }

        public IActionResult SendNotification(int id)
        {
            var user = f_db.users.FirstOrDefault(u => u.manId == id);
            if (user == null)
            {
                TempData["err"] = "Student not found";
                return RedirectToAction("MyStudents");
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult SendNotificationResult(int id, string title, string msj)
        {
            var user = f_db.users.FirstOrDefault(u => u.manId == id);
            
            if (user != null)
            {
                Notification n = new Notification(title, msj);
                n.ManId = user.manId;
                f_db.notifications.Add(n);
                f_db.SaveChanges();
                
                TempData["msg"] = "Message sent successfully";
                return RedirectToAction("MyStudents");
            }
            
            TempData["err"] = "Failed to send message";
            return RedirectToAction("MyStudents");
        }

        public IActionResult SendToAdmin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendToAdminResult(string title, string msj)
        {
            // Broadcast to all admins
            var admins = f_db.men.Where(m => m.whoIam == Roles.admin).ToList();
            
            if (admins.Any())
            {
                int? id = HttpContext.Session.GetInt32("UserId");
                var me = f_db.cotches.Find(id);
                string myName = me?.name ?? "Unknown Coach";
                string fullTitle = $"From Cotch {myName}: {title}";

                foreach (var admin in admins)
                {
                    Notification n = new Notification(fullTitle, msj);
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
    }
}
