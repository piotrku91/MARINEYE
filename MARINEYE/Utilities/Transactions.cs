using MARINEYE.Areas.Identity.Data;
using MARINEYE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MARINEYE.Utilities
{
    public struct TransactionResult {
        public bool success;
        public string? errorMessage;
    }
    public class Transactions
    {
        private readonly MARINEYEContext _context;
        public Transactions(MARINEYEContext context) {
            _context = context;
        }

        public async Task<TransactionResult> PayForCharter(BoatCalendarEvent boatCalendarEvent, MARINEYEUser? currentUser) {
            var charterDays = (boatCalendarEvent.EndDate - boatCalendarEvent.BeginDate).Days;
            var totalCost = charterDays * boatCalendarEvent.Boat.OneDayCharterCost;

            if (!currentUser.Withdraw(totalCost)) {
                TransactionResult errorOutResult = new TransactionResult {
                    errorMessage = "Brak wystarczających funduszy. (" + charterDays + " dni x " + boatCalendarEvent.Boat.OneDayCharterCost + " = " + totalCost,
                    success = false
                };
                return errorOutResult;
            }

            _context.Users.Update(currentUser);

            // Create a new DueTransaction
            var dueTransaction = new CharterDueTransactionModel {
                BoatCalendarEvent = boatCalendarEvent,
                BoatCalendarEventId = boatCalendarEvent.Id,
                AmountPaid = totalCost,
                PaymentDate = DateTime.Now
            };

            _context.CharterDueTransactions.Add(dueTransaction);
            
            TransactionResult outResult = new TransactionResult {
                errorMessage = null,
                success = true
            };
            return outResult;
        }

        public async Task<TransactionResult> PayForClub(ClubDueModel clubDueModel, MARINEYEUser? currentUser) {
            if (!currentUser.Withdraw(clubDueModel.Amount)) {
                TransactionResult errorOutResult = new TransactionResult {
                    errorMessage = "Brak wystarczających funduszy.",
                    success = false
                };
                return errorOutResult;
            }
            _context.Users.Update(currentUser);

            // Create a new DueTransaction
            var dueTransaction = new ClubDueTransactionModel {
                UserId = currentUser.Id,
                ClubDueId = clubDueModel.Id,
                AmountPaid = clubDueModel.Amount,
                PaymentDate = DateTime.Now,
            };

            _context.ClubDueTransactions.Add(dueTransaction);
            
            TransactionResult outResult = new TransactionResult {
                errorMessage = null,
                success = true
            };

            return outResult;
        }
    }
}
