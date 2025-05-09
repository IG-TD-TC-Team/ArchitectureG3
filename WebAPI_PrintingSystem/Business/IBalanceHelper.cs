using System;

namespace WebAPI_PrintingSystem.Business
{
    public interface IBalanceHelper
    {


        //-----------Exposed Methods------------------

        /// <summary>
        /// Retrieves the current quota balance in CHF
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="quotaCHF"></param>
        /// <returns></returns>
        Task<(decimal, int, bool)> creditUIDWithQuotaCHF(Guid userID, decimal quotaCHF);

        /// <summary>
        /// Credits a user's account by username
        /// </summary>

        Task<(decimal, bool)> creditUsernameWithQuotaCHF(string username, decimal quotaCHF);





        //--------------Internal Methods----------------
        /// <summary>
        /// Retrieves current quota balance in CHF
        /// </summary>
        Task<decimal> getQuotaCHFByUID(Guid userID);

        /// <summary>
        /// Calculates new quota balance
        /// </summary>
        /// <param name="quotaCHF"></param>
        /// <param name="actualQuotaCHF"></param>
        /// <returns></returns>
        decimal additionQuotaCHF (decimal quotaCHF, decimal actualQuotaCHF);

        /// <summary>
        /// Updates quota balance in database
        /// </summary>
        Task<bool> updateQuotaCHFByUID(Guid userID, decimal quotaCHF);

        /// <summary>
        /// Converts CHF amount to copy quota
        /// </summary>
        int convertQuotaCHFToCopyQuota(decimal quotaCHF);

        /// <summary>
        /// Updates copy quota in database
        /// </summary>
        Task<bool> updateCopyQuotaByUID(Guid userID, int copyQuota);











    }
}
