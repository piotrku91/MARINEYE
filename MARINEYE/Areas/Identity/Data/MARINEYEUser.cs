using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MARINEYE.Areas.Identity.Data;

public class MARINEYEUser : IdentityUser
{
    [PersonalData]
    [Display(Name = "Imię")]
    public string? FirstName { get; set; }

    [PersonalData]
    [Display(Name = "Nazwisko")]
    public string? LastName { get; set; }

    [PersonalData]
    [Display(Name = "Data urodzenia")]
    public DateTime DOB { get; set; }

    [Display(Name = "Data rejestracji")]
    public DateTime RegistrationDate { get; set; }

    // Personal account operations
    [PersonalData]
    [Display(Name = "Stan konta")]
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

