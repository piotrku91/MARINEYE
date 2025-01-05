using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MARINEYE.Areas.Identity.Data;
using System.Net.NetworkInformation;

namespace MARINEYE.Models
{
    public class ClosedDueTransactionModel
    {
        [Key]
        [Display(Name = "Identyfikator")]
        public int Id { get; set; }

        [Display(Name = "Id transakcji (historyczne)")]
        public int HistoricalDueTransactionId;

        [Display(Name = "Zapłacona kwota")]
        public int AmountPaid { get; set; }

        [Display(Name = "Data płatności")]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "Data zamknięcia")]
        public DateTime ClosedDate { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Display(Name = "Płacący")]
        public string FullName { get; set; }

        [Display(Name = "Login zamykającego")]
        public string CloserUserName { get; set; }

    }
}