using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{

    public class Balance
    {
        public decimal MoneyCHF { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal StandardQuota { get; set; }
        public required AbstractUser User { get; set; }
        public ICollection<AbstractProduct> Products { get; } = new List<AbstractProduct>();
    }

}
