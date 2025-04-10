using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Product
    {
        public Product(string description, decimal pricePerUnit)
        {
            Name = "A4 black and White";
            Description = description;
            PricePerUnit = pricePerUnit;
            Color = false;
            PaperSize = "A4";
            PaperType = "EcoPaper";
        }




        public Guid ProductId { get; }
        public string Name { get;}
        public  string Description { get; set; }
        public  decimal PricePerUnit { get; set; }
        public  bool Color { get; }
        public  string PaperSize { get; }
        public  string PaperType { get; }

        public Guid? BalanceId { get; set; }  // Foreign key linking to Balance table
        public virtual Balance Balance { get; set; }


    }
}
