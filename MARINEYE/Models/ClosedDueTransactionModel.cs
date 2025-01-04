using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MARINEYE.Areas.Identity.Data;
using System.Net.NetworkInformation;

namespace MARINEYE.Models
{
    public class ClosedDueTransactionModel
    {
        [Key]
        public int Id { get; set; }

        public int HistoricalDueTransactionId;
        public int AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }

        public DateTime ClosedDate { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string CloserUserName { get; set; }

    }
}