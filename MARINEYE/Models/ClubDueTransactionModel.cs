using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MARINEYE.Areas.Identity.Data;

namespace MARINEYE.Models
{
    public class ClubDueTransactionModel
    {
        [Key]
        [Display(Name = "Identyfikator")]
        public int Id { get; set; }

        public string UserId { get; set; } 

        [ForeignKey("UserId")]
        [Display(Name = "Użytkownik")]
        public virtual MARINEYEUser User { get; set; }
        [Display(Name = "Identyfikator opłaty klubowej")]
        public int ClubDueId { get; set; }

        [ForeignKey("ClubDueId")]
        public virtual ClubDueModel ClubDue { get; set; }

        [Display(Name = "Zapłacona kwota")]
        public int AmountPaid { get; set; }

        [Display(Name = "Data płatności")]
        public DateTime PaymentDate { get; set; }

        public bool Closed { get; set; }

    }
}
