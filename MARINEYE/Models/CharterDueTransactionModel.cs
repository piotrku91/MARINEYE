using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MARINEYE.Areas.Identity.Data;

namespace MARINEYE.Models
{
    public class CharterDueTransactionModel
    {
        [Key]
        public int Id { get; set; }
        public int BoatCalendarEventId { get; set; }

        [ForeignKey("BoatCalendarEventId")]
        public virtual BoatCalendarEvent BoatCalendarEvent { get; set; }
        public int AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        
    }
}
