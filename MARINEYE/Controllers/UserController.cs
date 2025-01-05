
using Microsoft.AspNetCore.Mvc;
using MARINEYE.Areas.Identity.Data;
using MARINEYE.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MARINEYE.Controllers
{
    public class UserController : Controller
    {
        private readonly MARINEYEContext _context;
        private readonly UserManager<MARINEYEUser> _userManager;

        public UserController(MARINEYEContext context, UserManager<MARINEYEUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = Constants.UserListAccessRoles)]
        private async Task<string> FindRole(MARINEYEUser user) {
            var userRoles = await _userManager.GetRolesAsync(user);
            var availableRoles = Constants.Roles;

            // Ensure the user has only one role
            if (userRoles.Count != 1) {
                throw new InvalidOperationException("Użytkownik nie ma dokładnie jednej roli");
            }

            // Get the single user role value
            var userRole = userRoles.FirstOrDefault();

            // Validate that the role exists in the availableRoles list
            if (!availableRoles.Contains(userRole)) {
                throw new InvalidOperationException($"Rola '{userRole}' nie znaleziona.");
            }

            return userRole;
        }

        [Authorize(Roles = Constants.UserListAccessRoles)]
        private async Task<bool> SetRole(MARINEYEUser user, string newRole) {
            var userRoles = await _userManager.GetRolesAsync(user);
            var availableRoles = Constants.Roles;

            // Ensure the user has only one role
            if (userRoles.Count != 1) {
                throw new InvalidOperationException("Użytkownik nie ma dokładnie jednej roli.");
            }

            // Get the single user role value
            var userRole = userRoles.FirstOrDefault();

            // Validate that the role exists in the availableRoles list
            if (!availableRoles.Contains(userRole) || !availableRoles.Contains(newRole)) {
                throw new InvalidOperationException($"Rola '{userRole}' nie znaleziona.");
            }

            if (userRole != newRole) {
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

                if (currentUser != null && await FindRole(currentUser) != Constants.MainAdminRole) {
                    if (newRole == Constants.MainAdminRole) {
                        return true;
                    }

                    if (userRole == Constants.MainAdminRole && newRole != Constants.MainAdminRole) {
                        return true;
                    }
                }

                foreach (var role in userRoles) { // Remove all roles
                    await _userManager.RemoveFromRoleAsync(user, role);
                    }
            } else {
                return true;
            }

            await _userManager.AddToRoleAsync(user, newRole); // Add to new role
            return true;
        }

        // GET: UserEdit
        [Authorize(Roles = Constants.UserListAccessRoles)]
        public async Task<IActionResult> Index()
        {
            var usersWithRoles = await _context.Users

            .Join(_context.UserRoles,
            user => user.Id,
            userRole => userRole.UserId,
            (user, userRole) => new { user, userRole })

            .Join(_context.Roles,
            userWithRole => userWithRole.userRole.RoleId,
            role => role.Id,

            (userWithRole, role) => new UserModelDTO {
            Id = userWithRole.user.Id,
            Email = userWithRole.user.Email,
            FirstName = userWithRole.user.FirstName,
            LastName = userWithRole.user.LastName,
            Role = role.Name,
            RegistrationDate = userWithRole.user.RegistrationDate
            })
            .ToListAsync();

            
            foreach (var userWithRole in usersWithRoles) {
                var userDues = await _context.ClubDueModel.Where(d => d.PeriodBegin >= userWithRole.RegistrationDate).ToListAsync();

                bool allPaid = true;

                foreach (var due in userDues) {
                    var userTransaction = await _context.ClubDueTransactions
                        .Where(dt => dt.UserId == userWithRole.Id && dt.ClubDueId == due.Id)
                        .FirstOrDefaultAsync();

                    // If there's no transaction or the payment amount is less than the due amount, mark as not paid
                    if (userTransaction == null || (userTransaction.Closed == false && userTransaction.AmountPaid < due.Amount)) {
                        allPaid = false;
                        break;
                    }
                }

                // Store the status indicating whether all dues are paid
                userWithRole.AllDuesPaid = allPaid;
            }

            return View(usersWithRoles);
        }

        // GET: Users/Edit/{id}
        [Authorize(Roles = Constants.MainAdminRole)]
        public async Task<IActionResult> Edit(string id) {
            if (string.IsNullOrEmpty(id)) {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) {
                return NotFound();
            }

            var userRole = await FindRole(user);

            // Create a model or directly pass user to the view
            var model = new UserModelDTO {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = userRole
            };

            ViewData["AvailableRoles"] = new SelectList(Constants.Roles, userRole);
            return View(model);
        }

        // POST: Users/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.MainAdminRole)]
        public async Task<IActionResult> Edit(UserModelDTO model) {
            if (!ModelState.IsValid) {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) {
                return NotFound();
            }
  
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded && await SetRole(user, model.Role)) {
                return RedirectToAction(nameof(Index)); 
            }

            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

    }
}
