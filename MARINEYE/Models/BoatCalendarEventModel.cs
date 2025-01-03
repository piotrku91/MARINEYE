using MARINEYE.Areas.Identity.Data;
using MARINEYE.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MARINEYE.Models
{
    public class BoatCalendarEvent
    {
        public int Id { get; set; }

        [Required]
        public DateTime BeginDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual MARINEYEUser User { get; set; }

        [Required]
        public int BoatId { get; set; }

        [ForeignKey("BoatId")]
        public virtual BoatModel Boat { get; set; }

        public BoatCalendarEventState EventState { get; set; }

        public BoatCalendarEventType EventType { get; set; }
    }
}
