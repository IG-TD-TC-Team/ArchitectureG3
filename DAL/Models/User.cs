using System;
using System.Collections.Generic;
using DAL;
using System.Linq;


namespace DAL.Models
{
    public class User
    {
        public User(string firstName, string lastName, string password, PrintingSystemContext context)
        {

            UserID = Guid.NewGuid();

            FirstName = firstName;
            LastName = lastName;
            Password = password;
            Username = GenerateUsername(firstName, lastName, context);

            Card = new Card(UserID);

            CopyQuota = 0;
            CHF = 0m;
            QuotaCHF = 0m;
        }

        // Basis for user
        public Guid UserID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public Card Card { get; set; }

        // Unique username generation
        private string GenerateUsername(string firstName, string lastName, PrintingSystemContext context)
        {
            string firstPart = firstName.Length >= 3 ? firstName.Substring(0, 3) : firstName.PadRight(3, 'x');
            string lastPart = lastName.Length >= 3 ? lastName.Substring(0, 3) : lastName.PadRight(3, 'x');

            string baseUsername = (firstPart + lastPart).ToLower();
            string username = baseUsername;
            int counter = 1;

            // Check for existing usernames
            while (context.Users.Any(u => u.Username == username))
            {
                username = baseUsername + counter;
                counter++;
            }

            return username;
        }

        // Printing data
        public int CopyQuota { get; set; }
        public decimal CHF { get; set; }
        public decimal QuotaCHF { get; set; }
    }
}