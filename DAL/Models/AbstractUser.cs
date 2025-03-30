using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
   public abstract class AbstractUser
    {
      


        public abstract string UserID { get; set; }
        public abstract string CardID { get; set; }
        public abstract string UserName { get; set; }
        public abstract string FirstName { get; set; }
        public abstract string LastName { get; set; }

        public abstract string Password { get; set; }

        public ICollection<Balance> Balances { get; } = new List<Balance>();


    }

    

}
