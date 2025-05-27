using System.ComponentModel.DataAnnotations;

namespace MVC_POS.Models
{
    public class BalanceM
    {
        public Guid UserID { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 1000.00, ErrorMessage = "Amount must be between 0.01 and 1000.00 CHF")]
        [Display(Name = "Amount (CHF)")]
        [DataType(DataType.Currency)]
        public decimal QuotaCHF { get; set; }

        // Response properties
        public decimal NewQuotaCHF { get; set; }
        public int NewPrintQuota { get; set; }
        public bool Done { get; set; }
    }
}
