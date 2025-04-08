using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    class A4Color : AbstractProduct
    {
        public A4Color(string description, decimal pricePerUnit)
        {
            Name = "A4 Color";
            Description = description;
            PricePerUnit = pricePerUnit;
            Color = true;
            PaperSize = "A4";
            PaperType = "EcoPaper";
        }
       
        public override string Name { get; }
        public override string Description { get; set; }
        public override decimal PricePerUnit { get; set; }
        public override bool Color { get; }
        public override string PaperSize { get; }
        public override string PaperType { get; }
    }

}

