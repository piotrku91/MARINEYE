using MARINEYE.Utilities;
using System.ComponentModel.DataAnnotations;

namespace MARINEYE.Models
{
    public class BoatModel
    {
        public int Id { get; set; }

        [Display(Name = "Nazwa jednostki")]
        public string Name { get; set; }
        [Display(Name = "Opis")]
        public string Description { get; set; }
        [Display(Name = "Długość jednostki")]
        [Range(1, 100, ErrorMessage = "Długość musi być w zakresie od 1 do 100 m")]
        public int Length { get; set; }
        [Display(Name = "Rok produkcji")]
        [Range(1900, 3000, ErrorMessage = "Rok produkcji musi być w zakresie od 1900 do 3000")]
        public int Year { get; set; }

        [Display(Name = "Klasa jednostki")]
        public string Class { get; set; }

        [Display(Name = "Stan")]
        public BoatState State { get; set; }
        [Display(Name = "Zdjęcie")]
        public string ImageName { get; set; }
        [Display(Name = "Koszt czarteru (1 dzień)")]
        public int OneDayCharterCost { get; set; }
    }
}
