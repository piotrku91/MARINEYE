using MARINEYE.Areas.Identity.Data;
using MARINEYE.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MARINEYE.Models
{
    public class BoatCalendarEventModel {
        public int Id { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual MARINEYEUser User { get; set; }

        public int BoatId { get; set; }

        [ForeignKey("BoatId")]
        public virtual BoatModel Boat { get; set; }

        public BoatCalendarEventState EventState  { get; set; }
    }
}
