using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MARINEYE.Areas.Identity.Data;

namespace MARINEYE.Models
{
    public class CharterDueTransactionModel
    {
        [Key]
        [Display(Name = "Identyfikator")]
        public int Id { get; set; }
        [Display(Name = "Identyfikator rezerwacji")]
        public int BoatCalendarEventId { get; set; }

        [ForeignKey("BoatCalendarEventId")]
        public virtual BoatCalendarEvent BoatCalendarEvent { get; set; }
        [Display(Name = "Zapłacona kwota")]
        public int AmountPaid { get; set; }
        [Display(Name = "Data płatności")]
        public DateTime PaymentDate { get; set; }
        public bool Closed { get; set; }

    }
}
