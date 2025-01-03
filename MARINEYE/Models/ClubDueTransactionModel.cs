using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MARINEYE.Areas.Identity.Data;

namespace MARINEYE.Models
{
    public class ClubDueTransactionModel
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } 

        [ForeignKey("UserId")]
        public virtual MARINEYEUser User { get; set; }
        public int ClubDueId { get; set; }

        [ForeignKey("ClubDueId")]
        public virtual ClubDueModel ClubDue { get; set; }
        public int AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        
    }
}
