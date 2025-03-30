using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public abstract class AbstractProduct
    {
       



        public abstract string ProductId { get; set; }
        public abstract string Name { get; set; }
        public abstract string Description { get; set; }
        public abstract decimal PricePerUnit { get; set; }
        public abstract bool Color { get; set; }
        public abstract string PaperSize { get; set; }
        public abstract string PaperType { get; set; }
       
        
    }
}
