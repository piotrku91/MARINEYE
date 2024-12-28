using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MARINEYE.Areas.Identity.Data;

public class MARINEYEUser : IdentityUser
{
    [PersonalData]
    public string? FirstName { get; set; }

    [PersonalData]
    public string? LastName { get; set; }

    [PersonalData]
    public DateTime DOB { get; set; }

    [PersonalData]
    public int CashAmount { get; set; }

}

