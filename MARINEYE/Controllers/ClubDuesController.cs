using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MARINEYE.Areas.Identity.Data;
using MARINEYE.Models;
using Microsoft.AspNetCore.Authorization;

namespace MARINEYE.Controllers
{
    public class ClubDuesController : Controller
    {
        private readonly MARINEYEContext _context;

        public ClubDuesController(MARINEYEContext context)
        {
            _context = context;
        }

        // GET: ClubDues
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClubDueModel.ToListAsync());
        }

        // GET: ClubDues/Create
        [Authorize(Roles = Constants.EditClubDuesRoles)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClubDues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PeriodBegin,PeriodEnd,Description,Amount")] ClubDueModel clubDueModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clubDueModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clubDueModel);
        }

        // GET: ClubDues/Edit/5
        [Authorize(Roles = Constants.EditClubDuesRoles)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubDueModel = await _context.ClubDueModel.FindAsync(id);
            if (clubDueModel == null)
            {
                return NotFound();
            }
            return View(clubDueModel);
        }

        // POST: ClubDues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.EditClubDuesRoles)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PeriodBegin,PeriodEnd,Description,Amount")] ClubDueModel clubDueModel)
        {
            if (id != clubDueModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clubDueModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClubDueModelExists(clubDueModel.Id))
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
            return View(clubDueModel);
        }

        // GET: ClubDues/Delete/5
        [Authorize(Roles = Constants.EditClubDuesRoles)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubDueModel = await _context.ClubDueModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clubDueModel == null)
            {
                return NotFound();
            }

            return View(clubDueModel);
        }

        // POST: ClubDues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.EditClubDuesRoles)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clubDueModel = await _context.ClubDueModel.FindAsync(id);
            if (clubDueModel != null)
            {
                _context.ClubDueModel.Remove(clubDueModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClubDueModelExists(int id)
        {
            return _context.ClubDueModel.Any(e => e.Id == id);
        }
    }
}
