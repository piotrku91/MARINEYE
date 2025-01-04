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

        private async Task<IActionResult> ViewAllDues() {
            if (User.IsInRole("Admin") || User.IsInRole("Boatswain")) {
                var clubTransactions = await _context.ClubDueTransactions
                    .Include(d => d.ClubDue)
                    .ToListAsync();

                // Get all charter transactions
                var charterTransactions = await _context.CharterDueTransactions
                .Include(t => t.BoatCalendarEvent)
                .ThenInclude(be => be.User)
                .ToListAsync();

                // Return the view with both transaction lists
                return View(Tuple.Create(clubTransactions, charterTransactions));
            }
            else {
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

                // Get the current user's club transactions
                var clubTransactions = await _context.ClubDueTransactions
                    .Include(d => d.ClubDue)
                    .Where(t => t.UserId == currentUser.Id)
                    .ToListAsync();

                // Get the current user's charter transactions
                var charterTransactions = await _context.CharterDueTransactions
                    .Include(t => t.BoatCalendarEvent)
                    .ThenInclude(be => be.User)
                    .Where(t => t.BoatCalendarEvent.User.Id == currentUser.Id) 
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

            var totalClubAmount = _context.ClubDueTransactions.Sum(t => t.AmountPaid);
            var totalCharterAmount = _context.CharterDueTransactions.Sum(t => t.AmountPaid);
            var totalAmount = totalClubAmount + totalCharterAmount;
            ViewData["ClubCash"] = totalAmount;

            return await ViewAllDues();
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dueTransactionModel == null)
            {
                return NotFound();
            }

            return View(dueTransactionModel);
        }

        // POST: DueTransactions/Delete/5
        [Authorize(Roles = Constants.EditClubDuesRoles)]
        [HttpPost, ActionName("RollbackClubDue")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RollbackClubDueConfirmed(int id)
        {
            var dueTransactionModel = await _context.ClubDueTransactions.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
            if (dueTransactionModel != null)
            {
                var user = dueTransactionModel.User;
                user.Deposit(dueTransactionModel.AmountPaid);
                _context.ClubDueTransactions.Remove(dueTransactionModel);
            }

            await _context.SaveChangesAsync();
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

        // POST: DueTransactions/Delete/5
        [Authorize(Roles = Constants.EditClubDuesRoles)]
        [HttpPost, ActionName("RollbackCharterDue")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RollbackCharterDueConfirmed(int id) {
            var dueTransactionModel = await _context.CharterDueTransactions
               .Include(d => d.BoatCalendarEvent)
               .ThenInclude(be => be.User)
               .FirstOrDefaultAsync(m => m.Id == id);

            if (dueTransactionModel != null) {
                var user = dueTransactionModel.BoatCalendarEvent.User;
                user.Deposit(dueTransactionModel.AmountPaid);
                _context.CharterDueTransactions.Remove(dueTransactionModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
