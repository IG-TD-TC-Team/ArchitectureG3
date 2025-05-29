namespace MVC_Faculties.Models
{
    public class UserM
    {
        public Guid UserID { get; set; }

        public string Username { get; set; }

        public string Group { get; set; }

        public int CopyQuota { get; set; }

        public decimal QuotaCHF { get; set; }
    }
}
