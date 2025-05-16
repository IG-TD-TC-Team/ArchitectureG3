
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;


namespace WebAPI_PrintingSystem.Business
{
    public class BalanceHelper : IBalanceHelper
    {
        private readonly PrintingSystemContext _repo;
        private readonly IAuthentificationHelper _authHelper;

        public BalanceHelper(PrintingSystemContext repo, IAuthentificationHelper authHelper)
        {
            _repo = repo;
            _authHelper = authHelper;
        }

        //-----------Internal Methods------------------
        public decimal additionQuotaCHF(decimal quotaCHF, decimal actualQuotaCHF)
        {
            decimal newQuotaCHF = quotaCHF + actualQuotaCHF;
            
            return newQuotaCHF;

        }

        public int convertQuotaCHFToCopyQuota(decimal quotaCHF)
        {

            decimal quotaCHFToCopyQuota = 0.08m; // 0.08CHF = 1 copy

            decimal decimalResult = quotaCHF / quotaCHFToCopyQuota;

            
            int copyQuota = (int)Math.Ceiling(decimalResult);

            return copyQuota;
        }

  
    public async Task<decimal> getQuotaCHFByUID(Guid userID)
        {
            var user = await _repo.Users.FirstOrDefaultAsync(u => u.UserID == userID);

            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userID} not found.");
            }

            return user.QuotaCHF;
        }


        public async Task<int> updateCopyQuotaByUID(Guid userID, int copyQuota)
        {
            var user = await _repo.Users.FirstOrDefaultAsync(u => u.UserID == userID);
            if (user != null)
            {
                user.CopyQuota = copyQuota;
                await _repo.SaveChangesAsync();
                var savedCopyQuota = user.CopyQuota; 
                return savedCopyQuota; ///Changes: Return the updatedQuota that will bi writed in the DB
            }
            return 0;
        }

        public async Task<decimal> updateQuotaCHFByUID(Guid userID, decimal quotaCHF)
        {
            var user = await _repo.Users.FirstOrDefaultAsync(u => u.UserID == userID);
            if (user != null)
            {
                user.QuotaCHF = quotaCHF;
                await _repo.SaveChangesAsync();
                var savedQuotaCHF = user.QuotaCHF;
                return savedQuotaCHF; ///Changes: Return the updatedQuota that will bi writed in the DB
            }
            return 0;
        }

        //--------------Exposed Methods----------------
        public async Task<(decimal , int, bool )> creditUIDWithQuotaCHF(Guid userID, decimal quotaCHF)
        {
            decimal actualQuotaCHF = await getQuotaCHFByUID(userID);
            decimal NewQuotaCHF = additionQuotaCHF(quotaCHF, actualQuotaCHF);
            decimal resultUpdateQuotaCHF = await updateQuotaCHFByUID(userID, NewQuotaCHF);
            int copyQuota = convertQuotaCHFToCopyQuota(NewQuotaCHF);
            int resultUpdateCopyQuota = await updateCopyQuotaByUID(userID, copyQuota);

            return (NewQuotaCHF,copyQuota, true);
        }

        public async Task<(decimal ,bool )> creditUsernameWithQuotaCHF(string username, decimal quotaCHF)
        {
            if (await _authHelper.usernameExists(username))
            {
                Guid userID = await _authHelper.getUIDByUsername(username); //check username
                decimal actualQuotaCHF = await getQuotaCHFByUID(userID); // get by username
                decimal newQuotaCHF = additionQuotaCHF(quotaCHF, actualQuotaCHF); 
                await updateQuotaCHFByUID(userID, newQuotaCHF); // by username
                int copyQuota = convertQuotaCHFToCopyQuota(newQuotaCHF);
                await updateCopyQuotaByUID(userID, copyQuota); // by username
                
                return (quotaCHF ,true);
            }
            else
            {
                return (0, false);
            }

        }


    }
}
