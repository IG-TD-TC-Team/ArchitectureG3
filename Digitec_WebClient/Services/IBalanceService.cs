using Digitec_WebClient.Models;

namespace MVC_Faculties.Services
{
    /// <summary>
    /// Interface for handling balance and quota management operations.
    /// This service is responsible for all financial transactions and quota adjustments,
    /// separated from authentication concerns for better code organization.
    /// </summary>
    public interface IBalanceService
    {
        /// <summary>
        /// Credits a specific user's account with printing quota based on their username.
        /// This method is used for individual user quota management.
        /// </summary>
        /// <param name="quotaRequest">Contains user information and the quota amount to add</param>
        /// <returns>Updated user information with new quota balance</returns>
        Task<UserM> CreditUsernameWithQuotaCHF(UserM quotaRequest);

        /// <summary>
        /// Credits all users in a specific group with printing quota.
        /// This method is used for bulk quota operations affecting multiple users simultaneously.
        /// Typically requires staff-level permissions to execute.
        /// </summary>
        /// <param name="groupName">The name of the group to credit (e.g., "student", "staff", "faculty")</param>
        /// <param name="quotaAmount">The amount of quota to add to each user in the group</param>
        /// <returns>List of all users who were credited in the operation</returns>
        Task<List<UserM>> CreditGroupWithQuotaCHF(string groupName, decimal quotaAmount);
    }
}