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
    [Authorize]
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


        // GET: BoatCalendarEvents/Create
        public IActionResult Create()
        {
            ViewData["BoatId"] = new SelectList(_context.BoatModel, "Id", "Name");
            return View();
        }

        // POST: BoatCalendarEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BeginDate,EndDate,BoatId")] BoatCalendarEventDTO boatCalendarEventDTO)
        {
            if (ModelState.IsValid) {
                BoatCalendarEvent boatCalendarEvent = new BoatCalendarEvent();
                boatCalendarEvent.Id = boatCalendarEventDTO.Id;
                boatCalendarEvent.BeginDate = boatCalendarEventDTO.BeginDate;
                boatCalendarEvent.EndDate = boatCalendarEventDTO.EndDate;
                boatCalendarEvent.BoatId = boatCalendarEventDTO.BoatId;
                boatCalendarEvent.Boat = _context.BoatModel.FirstOrDefault(b => b.Id == boatCalendarEvent.BoatId);
                var userId = User.Identity.Name;
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);
                boatCalendarEvent.UserId = userId;
                boatCalendarEvent.User = currentUser;
                boatCalendarEvent.EventState = Utilities.BoatCalendarEventState.Reserved;

                if (boatCalendarEvent.Boat == null) {
                    return View(boatCalendarEventDTO);
                }

                if (boatCalendarEvent.Boat.State == Utilities.BoatState.Repair) {
                    TempData["Error"] = "Sprzęt aktualnie jest uszkodzony. Nie można stwierdzić czy w tym terminie będzie dostępny. Skontaktuj się z Bosmanem.";
                    return RedirectToAction(nameof(Index));
                }

                var existingReservation = await _context.BoatCalendarEventModel
                    .Where(b => b.BoatId == boatCalendarEvent.BoatId &&
                                ((boatCalendarEvent.BeginDate >= b.BeginDate && boatCalendarEvent.BeginDate < b.EndDate) ||
                                 (boatCalendarEvent.EndDate > b.BeginDate && boatCalendarEvent.EndDate <= b.EndDate)))
                    .FirstOrDefaultAsync();

                if (existingReservation != null) {
                    TempData["Error"] = "Wybrany termin jest już zajęty. Wybierz inny.";
                    return RedirectToAction(nameof(Index));
                }

                // Dodanie nowej rezerwacji, jeśli termin jest wolny
                _context.Add(boatCalendarEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BoatId"] = new SelectList(_context.BoatModel, "Id", "Name", boatCalendarEventDTO.BoatId);
            return View(boatCalendarEventDTO);
        }

    }
}
