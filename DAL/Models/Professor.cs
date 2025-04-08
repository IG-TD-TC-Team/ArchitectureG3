using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    class Professor : AbstractUser
    {
        public Professor(string userName, string firstName, string lastName, string password)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            Password = password;
        }
        public override string UserName { get; set; }
        public override string FirstName { get; set; }
        public override string LastName { get; set; }
        public override string Password { get; set; }
    
    }
}
