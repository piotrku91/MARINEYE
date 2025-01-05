using MARINEYE.Areas.Identity.Data;
using MARINEYE.Models;
using MARINEYE.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MARINEYE.Controllers
{
    [Authorize]
    public class BoatCalendarEventsController : Controller
    {
        private readonly MARINEYEContext _context;
        private readonly Transactions _transactions;

        public BoatCalendarEventsController(MARINEYEContext context) {
            _context = context;
            _transactions = new Transactions(_context);
        }

        // GET: BoatCalendarEvents

        private async Task<bool> ValidateEvent(BoatCalendarEvent boatCalendarEvent) {
            if (boatCalendarEvent.Boat == null) {
                return false;
            }

            if (boatCalendarEvent.BeginDate < DateTime.Now) {
                TempData["Error"] = "Błąd. Podany termin rozpoczęcia już minał!";
                return false;
            }

            if (boatCalendarEvent.BeginDate > boatCalendarEvent.EndDate) {
                TempData["Error"] = "Błąd. Podany termin rozpoczęcia nieprawidłowy (Data występuje po dacie zakończenia)";
                return false;
            }

            if (boatCalendarEvent.Boat.State == Utilities.BoatState.Naprawa) {
                TempData["Error"] = "Sprzęt aktualnie jest uszkodzony. Nie można stwierdzić czy w tym terminie będzie dostępny. Skontaktuj się z Bosmanem.";
                return false;
            }

            if (await ReservationExists(boatCalendarEvent)) {
                TempData["Error"] = "Wybrany termin jest już zajęty. Wybierz inny.";
                return false;
            }

            return true;
        }

     
        public async Task<IActionResult> Index() {

            var boatCalendarEvents = _context.BoatCalendarEventModel.Include(b => b.Boat).Include(b => b.User).Where(b => b.EndDate >= DateTime.Now);
            return View(await boatCalendarEvents.ToListAsync());
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
        public IActionResult Create() {
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
            .Include(be => be.Boat)
            .Include(be => be.User)
            .FirstOrDefaultAsync(m => m.Id == id);

            if (boatCalendarEvent == null || boatCalendarEvent.Boat == null || boatCalendarEvent.User == null) {
                return NotFound();
            }

            if (boatCalendarEvent.Boat.State == Utilities.BoatState.Naprawa) {
                TempData["Error"] = "Sprzęt aktualnie jest uszkodzony. Nie można stwierdzić czy w tym terminie będzie dostępny. Skontaktuj się z Bosmanem.";
                return RedirectToAction(nameof(Index));
            }

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (currentUser == null) {
                return Unauthorized();
            }

            
            var clubDueModels = await _context.ClubDueModel.Where(m => m.PeriodBegin >= boatCalendarEvent.User.RegistrationDate).ToListAsync();

            var paid = true;
            foreach (var clubDueModel in clubDueModels) {
                paid = paid && await _context.ClubDueTransactions.Where(dt => dt.Closed == false)
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
        public async Task<IActionResult> Create([Bind("Id,BeginDate,EndDate,BoatId")] BoatCalendarEventDTO boatCalendarEventDTO) {
            if (ModelState.IsValid) {
                BoatCalendarEvent boatCalendarEvent = new BoatCalendarEvent();
                boatCalendarEvent.Id = boatCalendarEventDTO.Id;
                boatCalendarEvent.BeginDate = boatCalendarEventDTO.BeginDate;
                boatCalendarEvent.EndDate = boatCalendarEventDTO.EndDate;
                boatCalendarEvent.BoatId = boatCalendarEventDTO.BoatId;
                boatCalendarEvent.Boat = _context.BoatModel.FirstOrDefault(b => b.Id == boatCalendarEvent.BoatId);
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
                boatCalendarEvent.UserId = currentUser.Id;
                boatCalendarEvent.User = currentUser;
                boatCalendarEvent.EventState = Utilities.BoatCalendarEventState.Reserved;

                if (!await ValidateEvent(boatCalendarEvent)) {
                    return RedirectToAction(nameof(Index));
                };

                if (User.IsInRole("Klient")) {
                    boatCalendarEvent.EventType = Utilities.BoatCalendarEventType.Charter;
                    var result = await _transactions.PayForCharter(boatCalendarEvent, currentUser);

                    if (!result.success) {
                        TempData["Error"] = result.errorMessage;
                        return RedirectToAction(nameof(Index));
                    }
                } else {
                    boatCalendarEvent.EventType = Utilities.BoatCalendarEventType.Internal;
                }

                // Dodanie nowej rezerwacji, jeśli termin jest wolny
                _context.Add(boatCalendarEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BoatId"] = new SelectList(_context.BoatModel, "Id", "Name", boatCalendarEventDTO.BoatId);
            return View(boatCalendarEventDTO);
        }

        // GET: BoatCalendarEvents/Edit/5
        [Authorize(Roles = Constants.EditBoatListAccessRoles)]
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var boatCalendarEvent = await _context.BoatCalendarEventModel.FindAsync(id);
            if (boatCalendarEvent == null) {
                return NotFound();
            }

            BoatCalendarEventDTO boatCalendarEventDTO = new BoatCalendarEventDTO();

            boatCalendarEventDTO.Id = boatCalendarEvent.Id;
            boatCalendarEventDTO.BeginDate = boatCalendarEvent.BeginDate;
            boatCalendarEventDTO.EndDate = boatCalendarEvent.EndDate;
            boatCalendarEventDTO.BoatId = boatCalendarEvent.BoatId;

            ViewData["BoatId"] = new SelectList(_context.BoatModel, "Id", "Name", boatCalendarEventDTO.BoatId);
            return View(boatCalendarEventDTO);
        }

        // POST: BoatCalendarEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.EditBoatListAccessRoles)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BeginDate,EndDate,BoatId")] BoatCalendarEventDTO boatCalendarEventDTO) {
            if (id != boatCalendarEventDTO.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                var boatCalendarEvent = _context.BoatCalendarEventModel.FirstOrDefault(b => b.Id == boatCalendarEventDTO.BoatId);
                if (boatCalendarEvent == null) {
                    return NotFound();
                }

                boatCalendarEvent.Id = boatCalendarEventDTO.Id;
                boatCalendarEvent.BeginDate = boatCalendarEventDTO.BeginDate;
                boatCalendarEvent.EndDate = boatCalendarEventDTO.EndDate;
                boatCalendarEvent.BoatId = boatCalendarEventDTO.BoatId;
                boatCalendarEvent.Boat = _context.BoatModel.FirstOrDefault(b => b.Id == boatCalendarEvent.BoatId);

                if (!await ValidateEvent(boatCalendarEvent)) {
                    return RedirectToAction(nameof(Index));
                };

                bool isEventStared = await _context.BoatCalendarEventModel
               .AnyAsync(e => e.BoatId == id && e.BeginDate <= DateTime.Now && e.EndDate <= DateTime.Now);
                if (isEventStared) {
                    TempData["Error"] = "Nie można zmienić. Wydarzenie już się rozpoczęło";
                    return RedirectToAction(nameof(Index));
                }

                _context.Update(boatCalendarEvent);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: BoatCalendarEvents/Delete/5
        [Authorize(Roles = Constants.EditBoatListAccessRoles)]
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var boatCalendarEvent = await _context.BoatCalendarEventModel
                .FirstOrDefaultAsync(m => m.Id == id);

            if (boatCalendarEvent == null) {
                return NotFound();
            }

            return View(boatCalendarEvent);
        }

        // POST: BoatCalendarEvents/Delete/5
        [Authorize(Roles = Constants.EditBoatListAccessRoles)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var boatCalendarEvent = await _context.BoatCalendarEventModel.FindAsync(id);

            bool isEventStared = await _context.BoatCalendarEventModel
                .AnyAsync(e => e.BoatId == id && e.BeginDate <= DateTime.Now && e.EndDate <= DateTime.Now);

            var hasTransactions = await _context.CharterDueTransactions
            .AnyAsync(t => t.BoatCalendarEventId == id);


            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (boatCalendarEvent.UserId == currentUser.Id || User.IsInRole("Admin") || User.IsInRole("Bosman")) {

                if (isEventStared) {
                    TempData["Error"] = "Nie można usunąć. Wydarzenie już się rozpoczęło";
                    return RedirectToAction(nameof(Index));
                }

                if (hasTransactions) {
                    TempData["Error"] = "Nie można usunąć. Cofnij powiązanie transakcję (czarter) i spróbuj ponownie";
                    return RedirectToAction(nameof(Index));
                }

                if (boatCalendarEvent != null) {
                    _context.BoatCalendarEventModel.Remove(boatCalendarEvent);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            else {
                return Unauthorized();
            }
        }

    }
}
