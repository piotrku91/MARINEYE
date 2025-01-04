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
using MARINEYE.Utilities;

namespace MARINEYE.Controllers
{
    public class BoatsController : Controller
    {
        private readonly MARINEYEContext _context;

        public BoatsController(MARINEYEContext context)
        {
            _context = context;
        }

        // GET: BoatModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.BoatModel.ToListAsync());
        }

        // GET: BoatModels/Details/5
        [Authorize]
        public async Task<IActionResult> Report(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatModel = await _context.BoatModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatModel == null)
            {
                return NotFound();
            }

            boatModel.State = BoatState.Naprawa;

            _context.Update(boatModel);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: BoatModels/Create
        [Authorize(Roles = Constants.EditBoatListAccessRoles)]
        public IActionResult Create()
        {
            BoatState defaultState = BoatState.Sprawny;
            ViewData["State"] = new SelectList(BoatStateUtils.GetBoatStateAllStrings(), BoatStateUtils.GetBoatStateString(defaultState));
            return View();
        }

        // POST: BoatModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Length,Year,State,ImageName,OneDayCharterCost")] BoatModel boatModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boatModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boatModel);
        }

        // GET: BoatModels/Edit/5
        [Authorize(Roles = Constants.EditBoatListAccessRoles)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatModel = await _context.BoatModel.FindAsync(id);
            if (boatModel == null)
            {
                return NotFound();
            }

            ViewData["State"] = new SelectList(BoatStateUtils.GetBoatStateAllStrings(), BoatStateUtils.GetBoatStateString(boatModel.State));
            return View(boatModel);
        }

        // POST: BoatModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.EditBoatListAccessRoles)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Length,Year,State,ImageName,OneDayCharterCost")] BoatModel boatModel)
        {
            if (id != boatModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatModelExists(boatModel.Id))
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
            return View(boatModel);
        }

        // GET: BoatModels/Delete/5
        [Authorize(Roles = Constants.EditBoatListAccessRoles)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatModel = await _context.BoatModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatModel == null)
            {
                return NotFound();
            }

            return View(boatModel);
        }

        // POST: BoatModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.EditBoatListAccessRoles)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boatModel = await _context.BoatModel.FindAsync(id);

            bool isBoatReserved = await _context.BoatCalendarEventModel
                .AnyAsync(e => e.BoatId == id && e.BeginDate <= DateTime.Now && e.EndDate >= DateTime.Now);

            if (isBoatReserved) {
                TempData["Error"] = "Łódź jest obecnie zarezerwowana i nie można jej usunąć. Rozwiąż wszystkie rezerwację a potem spróbuj ponownie.";
                return RedirectToAction(nameof(Index));
            }

            if (boatModel != null)
            {
                _context.BoatModel.Remove(boatModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatModelExists(int id)
        {
            return _context.BoatModel.Any(e => e.Id == id);
        }
    }
}
