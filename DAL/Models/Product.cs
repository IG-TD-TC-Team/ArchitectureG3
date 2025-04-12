using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    /// <summary>
    /// Represents a product (printing option) in the printing system.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Default constructor initializes with A4 black and white print settings.
        /// </summary>
        public Product()
        {
            ProductID = Guid.NewGuid();                // Automatically generate unique ID
            Name = "A4_BW";                            // Default product name
            Description = "A4 black and White";        // Description of the product
            PricePerUnit = 0.1M;                       // Cost per print unit
            PrintQuotaCost = 1;                        // Quota cost (1 for B/W)
            Color = false;                             // Indicates black and white printing
            PaperSize = "A4";                          // Paper size
            PaperType = "StandardPaper";               // Paper type
        }

        /// <summary>
        /// Custom constructor for creating various printing products.
        /// </summary>
        /// <param name="name">Name of the product.</param>
        /// <param name="description">Description of the product.</param>
        /// <param name="pricePerUnit">Price per print unit.</param>
        /// <param name="color">Indicates whether the print is in color.</param>
        /// <param name="paperSize">Size of the paper (e.g., A4, A3).</param>
        /// <param name="paperType">Type of paper used.</param>
        public Product(string name, string description, decimal pricePerUnit, bool color, string paperSize, string paperType)
        {
            ProductID = Guid.NewGuid();                // Generate unique ID
            Name = name;
            Description = description;
            PricePerUnit = pricePerUnit;
            PrintQuotaCost = color ? 2 : 1;            // Set quota cost: 2 for color, 1 for B/W
            Color = color;
            PaperSize = paperSize;
            PaperType = paperType;
        }

        /// <summary>
        /// Unique identifier for the product.
        /// </summary>
        public Guid ProductID { get; set; }

        /// <summary>
        /// Name of the printing product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description providing details about the product.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Price to charge per unit of printing.
        /// </summary>
        public decimal PricePerUnit { get; set; }

        /// <summary>
        /// Cost in terms of print quota (used for tracking usage).
        /// </summary>
        public decimal PrintQuotaCost { get; set; }

        /// <summary>
        /// True if the print is in color; false if black and white.
        /// </summary>
        public bool Color { get; set; }

        /// <summary>
        /// Size of the paper (e.g., A4, A3).
        /// </summary>
        public string PaperSize { get; set; }

        /// <summary>
        /// Type of paper (e.g., StandardPaper, Glossy).
        /// </summary>
        public string PaperType { get; set; }
    }
}
