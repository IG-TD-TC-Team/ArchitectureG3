using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public class Balance
    {
     //JULIO BEST
        public string BalanceId { get; set; } = Guid.NewGuid().ToString();
        public decimal MoneyCHF { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal StandardQuotaCHF { get; set; }

        // Use string type for the user foreign key
        public string UserID { get; set; }

        // Navigation property to the User (which is an abstract type — you may use TPH later)
        public virtual AbstractUser User { get; set; }

        // One Balance has many products.
        public ICollection<AbstractProduct> Products { get; } = new List<AbstractProduct>();
    }
}
