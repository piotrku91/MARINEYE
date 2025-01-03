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
    public class DueTransactionsController : Controller
    {
        private readonly MARINEYEContext _context;

        public DueTransactionsController(MARINEYEContext context)
        {
            _context = context;
        }

        // GET: DueTransactions
        [Authorize]
        public async Task<IActionResult> Index() {
            if (User.IsInRole("Admin") || User.IsInRole("Boatswain")) {
                var transactions = await _context.DueTransactions
                    .Include(d => d.ClubDue)
                    .Join(
                        _context.Users,
                        transaction => transaction.UserId,
                        user => user.Id,
                        (transaction, user) => new
                        {
                            transaction.Id,
                            UserFirstName = user.FirstName,
                            UserLastName = user.LastName,
                            transaction.UserId,
                            transaction.ClubDueId,
                            transaction.AmountPaid,
                            transaction.PaymentDate,
                            ClubDueAmount = transaction.ClubDue.Amount,
                            ClubDueDescription = transaction.ClubDue.Description
                        })
                    .ToListAsync();

                return View(transactions);
            } else {
                var userId = User.Identity.Name;

                var transactions = await _context.DueTransactions
                    .Include(d => d.ClubDue)
                    .Join(
                        _context.Users,
                        transaction => transaction.UserId,
                        user => user.Id,
                        (transaction, user) => new
                        {
                            transaction.Id,
                            UserFirstName = user.FirstName,
                            UserLastName = user.LastName,
                            transaction.UserId,
                            transaction.ClubDueId,
                            transaction.AmountPaid,
                            transaction.PaymentDate,
                            ClubDueAmount = transaction.ClubDue.Amount,
                            ClubDueDescription = transaction.ClubDue.Description
                        })
                    .Where(t => t.UserId == userId) // Pokaż tylko transakcje usera jeżeli nie jest administratorem lub bosmanem
                    .ToListAsync();
                return View(transactions);
            }
        }

        [Authorize(Roles = Constants.EditClubDuesRoles)]
        public async Task<IActionResult> Rollback(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dueTransactionModel = await _context.DueTransactions
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
        [HttpPost, ActionName("Rollback")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RollbackConfirmed(int id)
        {
            var dueTransactionModel = await _context.DueTransactions.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
            if (dueTransactionModel != null)
            {
                var user = dueTransactionModel.User;
                user.CashAmount = dueTransactionModel.User.CashAmount + dueTransactionModel.AmountPaid;
                _context.DueTransactions.Remove(dueTransactionModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DueTransactionModelExists(int id)
        {
            return _context.DueTransactions.Any(e => e.Id == id);
        }
    }
}
