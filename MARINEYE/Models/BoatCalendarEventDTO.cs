using MARINEYE.Areas.Identity.Data;
using MARINEYE.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MARINEYE.Models
{

    [NotMapped]
    public class BoatCalendarEventDTO
    {
        [Display(Name = "Identyfikator")]
        public int Id { get; set; }

        [Display(Name = "Data rozpoczęcia")]
        public DateTime BeginDate { get; set; }

        [Display(Name = "Data zakończenia")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Jednostka")]
        public int BoatId { get; set; }

    }
}
