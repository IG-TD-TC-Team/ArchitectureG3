using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public abstract class AbstractProduct
    {
        public AbstractProduct()
        {
            ProductId = Guid.NewGuid().ToString();
        }


        public string ProductId { get; }
        public abstract string Name { get;}
        public abstract string Description { get; set; }
        public abstract decimal PricePerUnit { get; set; }
        public abstract bool Color { get; }
        public abstract string PaperSize { get; }
        public abstract string PaperType { get; }

        public string? BalanceId { get; set; }  // Foreign key linking to Balance table
        public virtual Balance Balance { get; set; }


    }
}
