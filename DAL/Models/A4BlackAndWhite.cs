using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class A4BlackAndWhite : AbstractProduct
    {
        public A4BlackAndWhite(string description, decimal pricePerUnit)
        {
            Name = "A4 black and White";
            Description = description;
            PricePerUnit = pricePerUnit;
            Color = false;
            PaperSize = "A4";
            PaperType = "EcoPaper";

        }
        
        public override  string Name { get ; }
        public override string Description { get; set; }
        public override  decimal PricePerUnit { get ; set ; }
        public override  bool Color { get ; }
        public override  string PaperSize { get ; }
        public override  string PaperType { get ;}
    }
}
