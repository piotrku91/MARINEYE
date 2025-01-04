using System.ComponentModel.DataAnnotations.Schema;

namespace MARINEYE.Models.Statistics
{
    [NotMapped]
    public class MostUsedBoatDTO
    {
        public int BoatId { get; set; }

        [ForeignKey("BoatId")]
        public virtual BoatModel Boat { get; set; }

        public int SumOfUseTimes;
    }
}
