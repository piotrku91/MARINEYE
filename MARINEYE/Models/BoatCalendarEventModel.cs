using MARINEYE.Areas.Identity.Data;
using MARINEYE.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MARINEYE.Models
{
    public class BoatCalendarEvent
    {
        [Display(Name = "Identyfikator")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Data rozpoczęcia")]
        public DateTime BeginDate { get; set; }

        [Required]
        [Display(Name = "Data zakończenia")]
        public DateTime EndDate { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual MARINEYEUser User { get; set; }

        [Required]
        public int BoatId { get; set; }

        [ForeignKey("BoatId")]
        public virtual BoatModel Boat { get; set; }

        [Display(Name = "Status")]
        public BoatCalendarEventState EventState { get; set; }

        [Display(Name = "Typ")]
        public BoatCalendarEventType EventType { get; set; }
    }
}
