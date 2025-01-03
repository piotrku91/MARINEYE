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
    public class ClubDuesController : Controller
    {
        private readonly MARINEYEContext _context;
        private readonly UserManager<MARINEYEUser> _userManager;

        public ClubDuesController(MARINEYEContext context, UserManager<MARINEYEUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ClubDues
        public async Task<IActionResult> Index() {
            // Retrieve the current logged-in user
            var userId = User.Identity.Name; // or User.FindFirstValue(ClaimTypes.NameIdentifier) depending on how user id is stored
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);

            if (currentUser == null) {
                return Unauthorized();
            }

            // Retrieve all ClubDueModel records
            var clubDueModels = await _context.ClubDueModel.ToListAsync();

            // For each ClubDueModel, check if the user has already paid
            foreach (var clubDueModel in clubDueModels) {
                // Check if there's a transaction for the current user for this particular ClubDue
                var paid = await _context.DueTransactions
                    .AnyAsync(dt => dt.UserId == currentUser.Id && dt.ClubDueId == clubDueModel.Id && dt.AmountPaid >= clubDueModel.Amount);

                // Store the paid status in ViewData for each ClubDueModel
                ViewData[$"Paid_{clubDueModel.Id}"] = paid;
            }

            return View(clubDueModels);
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

        [Authorize]
        public async Task<IActionResult> Pay(int? id) {
            if (id == null) {
                return NotFound();
            }

            var clubDueModel = await _context.ClubDueModel.FindAsync(id);
            if (clubDueModel == null) {
                return NotFound();
            }

            var userId = User.Identity.Name; 
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId); 

            if (currentUser == null) {
                return Unauthorized();
            }

            if (!currentUser.Withdraw(clubDueModel.Amount)) {
                TempData["Error"] = "Brak wystarczających funduszy.";
                return RedirectToAction(nameof(Index));
            } else {
                var result = await _userManager.UpdateAsync(currentUser);
            }

            // Create a new DueTransaction
            var dueTransaction = new DueTransactionModel {
                UserId = currentUser.Id,
                ClubDueId = clubDueModel.Id, 
                AmountPaid = clubDueModel.Amount, 
                PaymentDate = DateTime.Now, 
            };

            _context.DueTransactions.Add(dueTransaction);


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> DeleteConfirmed(int id) {
            // Find the ClubDue record
            var clubDueModel = await _context.ClubDueModel.FindAsync(id);

            if (clubDueModel == null) {
                return NotFound();
            }

            // Check if there are linked transactions
            var hasLinkedTransactions = await _context.DueTransactions
                .AnyAsync(dt => dt.ClubDueId == clubDueModel.Id);

            if (hasLinkedTransactions) {
                // Inform the user that the ClubDue cannot be deleted
                TempData["Error"] = "Nie można usunąć. Najpierw cofnij powiązane transakcje.";
                return RedirectToAction(nameof(Index));
            }

            // If no linked transactions, proceed with deletion
            _context.ClubDueModel.Remove(clubDueModel);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool ClubDueModelExists(int id)
        {
            return _context.ClubDueModel.Any(e => e.Id == id);
        }
    }
}
