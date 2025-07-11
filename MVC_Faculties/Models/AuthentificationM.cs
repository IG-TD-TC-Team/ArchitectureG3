﻿namespace MVC_Faculties.Models
{
    public class AuthentificationM
    {
        public string Username { get; set; }
        public string Password { get; set; }
        
        public Guid UID { get; set; }
        
        public string Message { get; set; } = string.Empty;
        public bool IsSuccessful => Message?.ToLower().Contains("successful access") == true;
        public bool IsStaff { get; set; }

        public string Group { get; set; } = string.Empty;
    }
}
