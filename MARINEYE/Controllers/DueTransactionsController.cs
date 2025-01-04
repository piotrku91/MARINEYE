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
using Microsoft.AspNetCore.Identity;

namespace MARINEYE.Controllers
{
    public class DueTransactionsController : Controller
    {
        private readonly MARINEYEContext _context;

        public DueTransactionsController(MARINEYEContext context)
        {
            _context = context;
        }

        private async Task<IActionResult> ViewAllClosedDues() {
            var closedTransactions = await _context.ClosedDueTransactions
                .ToListAsync();

            return View(closedTransactions);
        }
        private async Task<IActionResult> ViewAllDues() {
            if (User.IsInRole("Admin") || User.IsInRole("Boatswain")) {
                var clubTransactions = await _context.ClubDueTransactions
                    .Include(t => t.ClubDue)
                    .Where(t => t.Closed == false)
                    .ToListAsync();

                // Get all charter transactions
                var charterTransactions = await _context.CharterDueTransactions
                .Include(t => t.BoatCalendarEvent)
                .ThenInclude(be => be.User)
                .Where(t => t.Closed == false)
                .ToListAsync();

                // Return the view with both transaction lists
                return View(Tuple.Create(clubTransactions, charterTransactions));
            }
            else {
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

                // Get the current user's club transactions
                var clubTransactions = await _context.ClubDueTransactions
                    .Include(t => t.ClubDue)
                    .Where(t => t.UserId == currentUser.Id)
                    .Where(t => t.Closed == false)
                    .ToListAsync();

                // Get the current user's charter transactions
                var charterTransactions = await _context.CharterDueTransactions
                    .Include(t => t.BoatCalendarEvent)
                    .ThenInclude(be => be.User)
                    .Where(t => t.BoatCalendarEvent.User.Id == currentUser.Id)
                    .Where(t => t.Closed == false)
                    .ToListAsync();

                return View(Tuple.Create(clubTransactions, charterTransactions));
            }
        }


        // GET: DueTransactions
        [Authorize]
        public async Task<IActionResult> Index() {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (currentUser != null) {
                ViewData["Cash"] = currentUser.GetCashAmount();
            }

            var totalClubAmount = _context.ClubDueTransactions.Where(t => t.Closed == false).Sum(t => t.AmountPaid);
            var totalCharterAmount = _context.CharterDueTransactions.Where(t => t.Closed == false).Sum(t => t.AmountPaid);
            var totalAmount = totalClubAmount + totalCharterAmount;
            ViewData["ClubCash"] = totalAmount;

            return await ViewAllDues();
        }

        // GET: DueTransactions
        [Authorize(Roles = Constants.EditClubDuesRoles)]
        public async Task<IActionResult> IndexClosed() {
            var totalAmount = _context.ClosedDueTransactions.Sum(t => t.AmountPaid);
            ViewData["ClosedClubCash"] = totalAmount;

            return await ViewAllClosedDues();
        }

        [Authorize]
        public async Task<IActionResult> TopUpAccount() { // This is temporary function, just for system test purpose (should be implementation of some payment system)
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (currentUser != null && currentUser.GetCashAmount() <= 2500) {
                currentUser.Deposit(500);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Constants.EditClubDuesRoles)]
        public async Task<IActionResult> RollbackClubDue(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dueTransactionModel = await _context.ClubDueTransactions
                .Include(d => d.ClubDue)
                .Include(d => d.User)
                .Where(d => d.Closed == false)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dueTransactionModel == null)
            {
                return NotFound();
            }

            return View(dueTransactionModel);
        }

        [Authorize(Roles = Constants.EditClubDuesRoles)]
        [HttpPost, ActionName("RollbackClubDue")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RollbackClubDueConfirmed(int id)
        {
            var dueTransactionModel = await _context.ClubDueTransactions.Include(t => t.User).Where(t => t.Closed == false).FirstOrDefaultAsync(t => t.Id == id);
            if (dueTransactionModel != null)
            {
                var user = dueTransactionModel.User;
                user.Deposit(dueTransactionModel.AmountPaid);
                _context.ClubDueTransactions.Remove(dueTransactionModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Constants.EditClubDuesRoles)]
        public async Task<IActionResult> RollbackCharterDue(int? id) {
            if (id == null) {
                return NotFound();
            }

            var dueTransactionModel = await _context.CharterDueTransactions
                .Include(d => d.BoatCalendarEvent)
                .ThenInclude(be => be.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (dueTransactionModel == null) {
                return NotFound();
            }

            return View(dueTransactionModel);
        }

        [Authorize(Roles = Constants.EditClubDuesRoles)]
        [HttpPost, ActionName("RollbackCharterDue")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RollbackCharterDueConfirmed(int id) {
            var dueTransactionModel = await _context.CharterDueTransactions
               .Include(t => t.BoatCalendarEvent)
               .ThenInclude(be => be.User)
               .Where(t => t.Closed == false)
               .FirstOrDefaultAsync(m => m.Id == id);

            if (dueTransactionModel != null) {
                var user = dueTransactionModel.BoatCalendarEvent.User;
                user.Deposit(dueTransactionModel.AmountPaid);
                _context.CharterDueTransactions.Remove(dueTransactionModel);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Constants.EditClubDuesRoles)]
        public async Task<IActionResult> CloseClubDue(int? id) {
            if (id == null) {
                return NotFound();
            }

            var dueTransactionModel = await _context.ClubDueTransactions
                .Include(t => t.ClubDue)
                .Include(t => t.User)
                .Where(t => t.Closed == false)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (dueTransactionModel == null) {
                return NotFound();
            }

            return View(dueTransactionModel);
        }

        [Authorize(Roles = Constants.EditClubDuesRoles)]
        [HttpPost, ActionName("CloseClubDue")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseClubDueConfirmed(int id) {
            var dueTransactionModel = await _context.ClubDueTransactions
               .Include(t => t.ClubDue)
               .Include(t => t.User)
               .Where(t => t.Closed == false)
               .FirstOrDefaultAsync(m => m.Id == id);

            if (dueTransactionModel != null) {
                ClosedDueTransactionModel closedModel = new ClosedDueTransactionModel();
                closedModel.AmountPaid = dueTransactionModel.AmountPaid;
                closedModel.HistoricalDueTransactionId = dueTransactionModel.Id;
                closedModel.PaymentDate = dueTransactionModel.PaymentDate;
                closedModel.ClosedDate = DateTime.Now;
                closedModel.Description = "TRANSAKCJA KLUBOWA -> Id: " + dueTransactionModel.ClubDue.Id + " | " + dueTransactionModel.ClubDue.Description;
                closedModel.UserName = dueTransactionModel.User.UserName;
                closedModel.FullName = dueTransactionModel.User.FirstName + " " + dueTransactionModel.User.LastName;
                closedModel.CloserUserName = User.Identity.Name;

                _context.ClosedDueTransactions.Add(closedModel);

                dueTransactionModel.Closed = true;
                _context.ClubDueTransactions.Update(dueTransactionModel);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CloseCharterDue(int? id) {
            if (id == null) {
                return NotFound();
            }

            var dueTransactionModel = await _context.CharterDueTransactions
                .Include(t => t.BoatCalendarEvent)
                .ThenInclude(be => be.User)
                .Where(t => t.Closed == false)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (dueTransactionModel == null) {
                return NotFound();
            }

            return View(dueTransactionModel);
        }

        [Authorize(Roles = Constants.EditClubDuesRoles)]
        [HttpPost, ActionName("CloseCharterDue")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseCharterDueConfirmed(int id) {
            var dueTransactionModel = await _context.CharterDueTransactions
               .Include(t => t.BoatCalendarEvent)
               .ThenInclude(be => be.User)
               .Where(t => t.Closed == false)
               .FirstOrDefaultAsync(m => m.Id == id);

            if (dueTransactionModel != null) {

                ClosedDueTransactionModel closedModel = new ClosedDueTransactionModel();
                closedModel.AmountPaid = dueTransactionModel.AmountPaid;
                closedModel.HistoricalDueTransactionId = dueTransactionModel.Id;
                closedModel.PaymentDate = dueTransactionModel.PaymentDate;
                closedModel.ClosedDate = DateTime.Now;
                closedModel.Description = "TRANSAKCJA CZARTEROWA -> Id: " 
                    + dueTransactionModel.BoatCalendarEvent.Id 
                    + " | " 
                    + dueTransactionModel.BoatCalendarEvent.BeginDate 
                    + "/" + dueTransactionModel.BoatCalendarEvent.EndDate;
                closedModel.UserName = dueTransactionModel.BoatCalendarEvent.User.UserName;
                closedModel.FullName = dueTransactionModel.BoatCalendarEvent.User.FirstName + " " + dueTransactionModel.BoatCalendarEvent.User.LastName;
                closedModel.CloserUserName = User.Identity.Name;

                _context.ClosedDueTransactions.Add(closedModel);

                dueTransactionModel.Closed = true;
                _context.CharterDueTransactions.Update(dueTransactionModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
