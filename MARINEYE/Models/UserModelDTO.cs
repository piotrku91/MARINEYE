using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MARINEYE.Models
{

    [NotMapped]
    public class UserModelDTO
    {
        [Display(Name = "Identyfikator")]
        public string Id { get; set; }
        [Display(Name = "Adres e-mail")]
        public string Email { get; set; }
        [Display(Name = "Imię")]
        public string FirstName { get; set; }
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }
        [Display(Name = "Rola")]
        public string Role { get; set; }
        [Display(Name = "Data rejestracji")]
        public DateTime RegistrationDate { get; set; }
        [Display(Name = "Opłaty")]
        public bool AllDuesPaid { get; set; }

    }
}
