using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    /// <summary>
    /// Represents a printing transaction in the system.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Private constructor for Entity Framework.
        /// </summary>
        private Transaction() { }

        /// <summary>
        /// Creates a new transaction for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user making the transaction.</param>
        public Transaction(Guid userId)
        {
            TransactionID = Guid.NewGuid();
            Date = DateTime.UtcNow;
            UserID = userId;

            // Initialize default values
            TotalCopyQuota = 0;
            TotalCHF = 0m;
            TotalQuotaCHF = 0m;

            // Initialize collection
            TransactionProducts = new List<TransactionProduct>();
        }

        /// <summary>
        /// Unique identifier for the transaction.
        /// </summary>
        public Guid TransactionID { get; set; }

        /// <summary>
        /// Date and time when the transaction occurred.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Foreign key to the user who made the transaction.
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// Navigation property to access the associated User.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Collection of products included in this transaction.
        /// </summary>
        public ICollection<TransactionProduct> TransactionProducts { get; set; }

        
        // ───── Transaction Totals ─────

        /// <summary>
        /// Total printing quota change in this transaction.
        /// </summary>
        public int TotalCopyQuota { get; set; }

        /// <summary>
        /// Total monetary amount (CHF) in this transaction.
        /// </summary>
        public decimal TotalCHF { get; set; }

        /// <summary>
        /// Total quota value in CHF for this transaction.
        /// </summary>
        public decimal TotalQuotaCHF { get; set; }

        ///<result>
            ///RECEIPT #1234 (Transaction)
            ///-----------------------------
            ///2x Paper A4 @ $5.00 = $10.00  (TransactionProduct 1)
            ///1x Color Toner @ $15.00 = $15.00 (Transactionproduct 2)
            ///-----------------------------
            ///TOTAL: $25.00
        ///</result>
    }
}