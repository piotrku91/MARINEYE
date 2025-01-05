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

    public DateTime RegistrationDate { get; set; }

    // Personal account operations
    [PersonalData]
    public int CashAmount { get; set; }

    public void Deposit(int amount) {
        CashAmount += amount;
    }

    public bool Withdraw(int amount) {
        if (amount > CashAmount) {
            return false;
        }

        CashAmount -= amount;
        return true;
    }

    public int GetCashAmount() {
        return CashAmount;
    }

}

