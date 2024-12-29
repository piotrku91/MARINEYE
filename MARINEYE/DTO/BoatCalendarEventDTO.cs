using MARINEYE.Areas.Identity.Data;
using MARINEYE.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MARINEYE.Models
{

    [NotMapped]
    public class BoatCalendarEventDTO
    {
        public int Id { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public int BoatId { get; set; }

    }
}
