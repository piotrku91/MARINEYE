using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MARINEYE.Models;
using System.Reflection.Emit;

namespace MARINEYE.Areas.Identity.Data;

public class MARINEYEContext : IdentityDbContext<MARINEYEUser>
{
    public MARINEYEContext(DbContextOptions<MARINEYEContext> options)
        : base(options) {
    }

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);
    }

public DbSet<MARINEYE.Models.UserModelDTO> EditableUserModel { get; set; } = default!;

public DbSet<MARINEYE.Models.BoatModel> BoatModel { get; set; } = default!;

public DbSet<MARINEYE.Models.ClubDueModel> ClubDueModel { get; set; } = default!;

public DbSet<ClubDueTransactionModel> ClubDueTransactions { get; set; } = default!;

public DbSet<CharterDueTransactionModel> CharterDueTransactions { get; set; } = default!;

public DbSet<ClosedDueTransactionModel> ClosedDueTransactions { get; set; } = default!;

public DbSet<MARINEYE.Models.BoatCalendarEvent> BoatCalendarEventModel { get; set; } = default!;
}
