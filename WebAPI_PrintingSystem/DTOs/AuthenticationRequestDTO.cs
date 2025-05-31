using System.ComponentModel.DataAnnotations;

namespace WebAPI_PrintingSystem.DTOs
{
    public class AuthenticationRequestDTO
    {
        public string Username { get; set; }

        
        public string Password { get; set; }
    }
}
