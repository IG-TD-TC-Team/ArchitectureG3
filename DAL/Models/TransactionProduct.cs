using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    /// <summary>
    /// Represents a product within a transaction, managing the many-to-many relationship.
    /// </summary>
    public class TransactionProduct
    {
        /// <summary>
        /// Private constructor for Entity Framework.
        /// </summary>
        private TransactionProduct() { }

        /// <summary>
        /// Creates a new transaction product entry.
        /// </summary>
        /// <param name="transactionId">The ID of the parent transaction.</param>
        /// <param name="productId">The ID of the product being purchased.</param>
        /// <param name="quantity">Number of copies/prints of this product.</param>
        public TransactionProduct(Guid transactionId, Guid productId, int quantity)
        {
            TransactionProductID = Guid.NewGuid();
            TransactionID = transactionId;
            ProductID = productId;
            Quantity = quantity;
        }

        /// <summary>
        /// Unique identifier for the transaction-product relationship.
        /// </summary>
        //[Key]
        public Guid TransactionProductID { get; set; }

        /// <summary>
        /// Foreign key to the parent transaction.
        /// </summary>
        public Guid TransactionID { get; set; }

        /// <summary>
        /// Foreign key to the product being purchased.
        /// </summary>
        public Guid ProductID { get; set; }

        /// <summary>
        /// Navigation property to access the parent transaction.
        /// </summary>
        public Transaction Transaction { get; set; }

        /// <summary>
        /// Navigation property to access the associated product.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Number of copies/prints of this product.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Copy quota cost for this specific product in the transaction.
        /// </summary>
        public int CopyQuotaCost { get; set; }

        /// <summary>
        /// Monetary cost (CHF) for this specific product in the transaction.
        /// </summary>
        public decimal CHFCost { get; set; }

        /// <summary>
        /// Quota cost in CHF for this specific product in the transaction.
        /// </summary>
        public decimal QuotaCHFCost { get; set; }
    }
}