using System.ComponentModel.DataAnnotations.Schema;

namespace MARINEYE.Models
{

    [NotMapped]
    public class EditableUserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }

        public bool AllDuesPaid { get; set; }

    }
}
