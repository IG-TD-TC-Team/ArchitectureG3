namespace MVC_POS.Models
{
    public class TransactionM
    {
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        //User information
        public Guid CardId { get; set; }
        public Guid UserId { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }

        //Transaction details
        public decimal AmountCredited { get; set; }
        public decimal NewQuotaCHF { get; set; }
        public int NewPrintQuota { get; set; }

        //Status
        public bool IsSuccessful { get; set; }
        public string? Message { get; set; }
    }
}
