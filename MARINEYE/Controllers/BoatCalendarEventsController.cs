using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MARINEYE.Areas.Identity.Data;
using MARINEYE.Models;

namespace MARINEYE.Controllers
{
    public class BoatCalendarEventsController : Controller
    {
        private readonly MARINEYEContext _context;

        public BoatCalendarEventsController(MARINEYEContext context)
        {
            _context = context;
        }

        // GET: BoatCalendarEvents
        public async Task<IActionResult> Index()
        {
            var mARINEYEContext = _context.BoatCalendarEventModel.Include(b => b.Boat).Include(b => b.User);
            return View(await mARINEYEContext.ToListAsync());
        }

        // GET: BoatCalendarEvents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatCalendarEventModel = await _context.BoatCalendarEventModel
                .Include(b => b.Boat)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatCalendarEventModel == null)
            {
                return NotFound();
            }

            return View(boatCalendarEventModel);
        }

        // GET: BoatCalendarEvents/Create
        public IActionResult Create()
        {
            ViewData["BoatId"] = new SelectList(_context.BoatModel, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: BoatCalendarEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BeginDate,EndDate,UserId,BoatId,EventState")] BoatCalendarEventModel boatCalendarEventModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boatCalendarEventModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BoatId"] = new SelectList(_context.BoatModel, "Id", "Id", boatCalendarEventModel.BoatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", boatCalendarEventModel.UserId);
            return View(boatCalendarEventModel);
        }

        // GET: BoatCalendarEvents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatCalendarEventModel = await _context.BoatCalendarEventModel.FindAsync(id);
            if (boatCalendarEventModel == null)
            {
                return NotFound();
            }
            ViewData["BoatId"] = new SelectList(_context.BoatModel, "Id", "Id", boatCalendarEventModel.BoatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", boatCalendarEventModel.UserId);
            return View(boatCalendarEventModel);
        }

        // POST: BoatCalendarEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BeginDate,EndDate,UserId,BoatId,EventState")] BoatCalendarEventModel boatCalendarEventModel)
        {
            if (id != boatCalendarEventModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boatCalendarEventModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatCalendarEventModelExists(boatCalendarEventModel.Id))
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
            ViewData["BoatId"] = new SelectList(_context.BoatModel, "Id", "Id", boatCalendarEventModel.BoatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", boatCalendarEventModel.UserId);
            return View(boatCalendarEventModel);
        }

        // GET: BoatCalendarEvents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boatCalendarEventModel = await _context.BoatCalendarEventModel
                .Include(b => b.Boat)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boatCalendarEventModel == null)
            {
                return NotFound();
            }

            return View(boatCalendarEventModel);
        }

        // POST: BoatCalendarEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boatCalendarEventModel = await _context.BoatCalendarEventModel.FindAsync(id);
            if (boatCalendarEventModel != null)
            {
                _context.BoatCalendarEventModel.Remove(boatCalendarEventModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatCalendarEventModelExists(int id)
        {
            return _context.BoatCalendarEventModel.Any(e => e.Id == id);
        }
    }
}
