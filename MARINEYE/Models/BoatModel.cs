using MARINEYE.Utilities;

namespace MARINEYE.Models
{

    public class BoatModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }
        public int Year { get; set; }
        public BoatState State { get; set; }
        public string ImageName { get; set; }
        public int OneDayCharterCost { get; set; }
    }
}
