namespace MARINEYE.Models
{
    public class ClubDueModel
    {
        public int Id { get; set; }
        public DateTime PeriodBegin { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string? Description { get; set; }
        public int Amount { get; set; }
    }
}
