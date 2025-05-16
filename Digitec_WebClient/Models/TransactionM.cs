namespace Digitec_WebClient.Models
{
    public class TransactionM
    {
        public Guid TransactionID { get; set; }

        public DateTime Date { get; set; }

        public Guid UserID { get; set; }

        public int PageCount { get; set; }

        public Guid ProductID { get; set; }

        public int TotalCopyQuota { get; set; }

        public decimal TotaQuotalCHF { get; set; }
    }
}
