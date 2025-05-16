namespace MVC_POS.Models
{
    public class UserM
    {
        public Guid UID { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public decimal QuotaCHF { get; set; }
        public int CopyQuota { get; set; }
    }
}
