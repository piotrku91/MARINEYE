using System.ComponentModel.DataAnnotations;

namespace MARINEYE.Models
{
    public class ClubDueModel
    {
        [Display(Name = "Identyfikator")]
        public int Id { get; set; }
        [Display(Name = "Data początku okresu")]
        public DateTime PeriodBegin { get; set; }
        [Display(Name = "Data końca okresu")]
        public DateTime PeriodEnd { get; set; }
        [Display(Name = "Opis")]
        public string? Description { get; set; }
        [Display(Name = "Kwota")]
        public int Amount { get; set; }
    }
}
