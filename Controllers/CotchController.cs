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
        private FitnessContext f_db = new FitnessContext();


        // GET: Cotch
        public async Task<IActionResult> Index()
        {
            /////// important all cotches is saved in man so ...
            return View(await f_db.cotches.ToListAsync());
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
            return View(cotch);
        }

        // POST: Cotch/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("cotch_status,available_times,manId,name,userName,email,passwordHash,boy,wight,age,number,whoIam")] Cotch cotch)
        {
            if (id != cotch.manId)
            {
                return NotFound();
            }

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
    }
}
