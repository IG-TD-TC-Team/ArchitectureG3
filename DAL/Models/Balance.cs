using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public class Balance
    {
     //JULIO BEST
        public Guid BalanceId { get; set; }
        public decimal MoneyCHF { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal StandardQuotaCHF { get; set; }
        public int printQty { get; set; }

        public int CopyQuota { get; set; }

        // Use string type for the user foreign key
        public Guid UserID { get; set; }

        // Navigation property to the User (which is an abstract type )
        public User User { get; set; }

        // One Balance has many products.
        public ICollection<Product> Products { get; } = new List<Product>();
    }
}
