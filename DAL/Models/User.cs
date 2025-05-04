using System;
using System.Collections.Generic;
using DAL;
using System.Linq;

namespace DAL.Models
{
    /// <summary>
    /// Represents a user of the printing system.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Parameterless constructor required by Entity Framework Core.
        /// Protected to prevent misuse outside EF context.
        /// </summary>
        protected User() { }

        /// <summary>
        /// Creates a new user with personal details and initializes default values.
        /// </summary>
        /// <param name="firstName">User's first name.</param>
        /// <param name="lastName">User's last name.</param>
        /// <param name="password">Password for authentication (should be hashed in production).</param>
        /// <param name="username">Unique username for login.</param>
        public User(string firstName, string lastName, string password, string username, string group)
        {
            UserID = Guid.NewGuid();                  // Generate a unique user ID
            FirstName = firstName;
            LastName = lastName;
            Password = password;                      // NOTE: Store hashed passwords in real systems
            Username = username;

            Card = new Card(UserID);                  // Create a new card assigned to the user

            Transactions = new List<Transaction>();   // Initialize the collection of transactions

            Group = group;

            // Initialize account values
            CopyQuota = 0;                            // Number of copies available
            CHF = 0m;                                 // Monetary balance
            QuotaCHF = 0m;                            // Quota balance in CHF
            
        }

        // ───── Identity Information ─────

        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// Username used to log in.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// First name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Password used for authentication.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// User group (e.g., student, staff, etc.)
        /// </summary>
        public string Group { get; set; } 

        /// <summary>
        /// The card associated with the user.
        /// </summary>
        public Card Card { get; set; }

        /// <summary>
        /// Collection of transactions made by the user.
        /// </summary>
        public ICollection<Transaction> Transactions { get; set; }


       
        // ───── Printing Quotas and Account Balances ─────

        /// <summary>
        /// Number of pages the user can still print/copy.
        /// </summary>
        public int CopyQuota { get; set; }

        /// <summary>
        /// Real money balance (CHF) available on the user’s account.
        /// </summary>
        public decimal CHF { get; set; }

        /// <summary>
        /// Balance in CHF that corresponds to the user's print quota.
        /// </summary>
        public decimal QuotaCHF { get; set; }
        
    }
}
