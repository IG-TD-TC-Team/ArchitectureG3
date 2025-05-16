namespace Digitec_WebClient.Models
{
    public class ProductM
    {
        public Guid ProductID { get; set; }

        public string Name { get; set; }

        public decimal PricePerUnit { get; set; }

        public decimal PrintQuotaCost { get; set; }
    }
}
