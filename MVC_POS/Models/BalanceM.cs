using System;
using System.ComponentModel.DataAnnotations;

namespace MVC_POS.Models
{
    public class BalanceM
    {
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 1000, ErrorMessage = "Amount must be positive and less than 1000")]
        [Display(Name = "Amount (CHF)")]
        public decimal Amount { get; set; }
    }
}
