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

        private async Task<bool> ReservationExists(BoatCalendarEvent boatCalendarEvent) {
            var existingReservation = await _context.BoatCalendarEventModel
                    .Where(b => b.BoatId == boatCalendarEvent.BoatId &&
                                ((boatCalendarEvent.BeginDate >= b.BeginDate && boatCalendarEvent.BeginDate < b.EndDate) ||
                                 (boatCalendarEvent.EndDate > b.BeginDate && boatCalendarEvent.EndDate <= b.EndDate)))
                    .FirstOrDefaultAsync();

            return existingReservation != null;
        }

        // GET: BoatCalendarEvents/Create
        public IActionResult Create()
        {
            ViewData["BoatId"] = new SelectList(_context.BoatModel, "Id", "Name");
            return View();
        }

        // GET: BoatCalendarEvents/Confirm
        [Authorize(Roles = Constants.EditBoatListAccessRoles)]
        public async Task<IActionResult> Confirm(int? id) {
            if (id == null) {
                return NotFound();
            }

            var boatCalendarEvent = await _context.BoatCalendarEventModel
                .FirstOrDefaultAsync(m => m.Id == id);

            if (boatCalendarEvent == null) {
                return NotFound();
            }

            boatCalendarEvent.Boat = _context.BoatModel.FirstOrDefault(b => b.Id == boatCalendarEvent.BoatId);

            if (boatCalendarEvent.Boat == null) {
                return NotFound();
            }

            if (boatCalendarEvent.Boat.State == Utilities.BoatState.Repair) {
                TempData["Error"] = "Sprzęt aktualnie jest uszkodzony. Nie można stwierdzić czy w tym terminie będzie dostępny. Skontaktuj się z Bosmanem.";
                return RedirectToAction(nameof(Index));
            }

            var userId = User.Identity.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);

            if (currentUser == null) {
                return Unauthorized();
            }

            var clubDueModels = await _context.ClubDueModel.ToListAsync();

            var paid = true;
            foreach (var clubDueModel in clubDueModels) {
                paid = paid && await _context.DueTransactions
                    .AnyAsync(dt => dt.UserId == currentUser.Id && dt.ClubDueId == clubDueModel.Id && dt.AmountPaid >= clubDueModel.Amount);
            }

            if (!paid) {
                TempData["Error"] = "Nie można potwierdzić rezerwacji. Prawdopodobnie użytkownik ma zaległe składki do opłacenia.";
                return RedirectToAction(nameof(Index));
            }

            boatCalendarEvent.EventState = Utilities.BoatCalendarEventState.Confirmed;
            _context.Update(boatCalendarEvent);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
                    return NotFound();
                }

                if (boatCalendarEvent.Boat.State == Utilities.BoatState.Repair) {
                    TempData["Error"] = "Sprzęt aktualnie jest uszkodzony. Nie można stwierdzić czy w tym terminie będzie dostępny. Skontaktuj się z Bosmanem.";
                    return RedirectToAction(nameof(Index));
                }
                
                if (await ReservationExists(boatCalendarEvent)) {
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
