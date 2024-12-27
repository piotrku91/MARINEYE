using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MARINEYE.Models;

namespace MARINEYE.Areas.Identity.Data;

public class MARINEYEContext : IdentityDbContext<MARINEYEUser>
{
    public MARINEYEContext(DbContextOptions<MARINEYEContext> options)
        : base(options) {
    }

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);
    }

public DbSet<MARINEYE.Models.EditableUserModel> EditableUserModel { get; set; } = default!;
}
