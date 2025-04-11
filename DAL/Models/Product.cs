using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Product
    {

        //Default constructor
        public Product()
        {
            ProductID = Guid.NewGuid();

            Name = "A4_BW";
            Description = "A4 black and White";
            PricePerUnit = 0.1M;
            PrintQuotaCost = 1;
            Color = false;
            PaperSize = "A4";
            PaperType = "StandardPaper";
        }

        //For eventual other products
        public Product(string name, string description, decimal pricePerUnit, bool color, string papersize, string papertype)
        {
            ProductID = Guid.NewGuid();

            Name = name;
            Description = description;
            PricePerUnit = pricePerUnit;
            Color = color;
            PaperSize = papersize;
            PaperType = papertype;
        }


        public Guid ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal PrintQuotaCost { get; set; }
        public bool Color { get; set; }
        public string PaperSize { get; set; }
        public string PaperType { get; set; }


    }
}