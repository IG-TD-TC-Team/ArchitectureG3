namespace MVC_POS.Models
{
    public class AuthenticationM
    {
        public string Message { get; set; }
        public Guid? UserID { get; set; }
        public bool IsSuccessful => Message.ToLower().Contains("successful access");
    }
}
