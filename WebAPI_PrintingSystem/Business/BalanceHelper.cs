
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;


namespace WebAPI_PrintingSystem.Business
{
    public class BalanceHelper : IBalanceHelper
    {
        private readonly PrintingSystemContext _repo;
        private readonly IAuthentificationHelper _authHelper; // ASK ALAIN

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

        public async Task<Guid> getUIDByUsername(string username)
        {
            var user = await _repo.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                throw new InvalidOperationException($"User with username {username} not found.");
            }

            return user.UserID;
        }

        public async Task<bool> updateCopyQuotaByUID(Guid userID, int copyQuota)
        {
            var user = await _repo.Users.FirstOrDefaultAsync(u => u.UserID == userID);
            if (user != null)
            {
                user.CopyQuota = copyQuota;
                await _repo.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> updateQuotaCHFByUID(Guid userID, decimal quotaCHF)
        {
            var user = await _repo.Users.FirstOrDefaultAsync(u => u.UserID == userID);
            if (user != null)
            {
                user.QuotaCHF = quotaCHF;
                await _repo.SaveChangesAsync();
                return true;
            }
            return false;
        }

        //--------------Exposed Methods----------------
        public async Task<(decimal , int, bool )> creditUIDWithQuotaCHF(Guid userID, decimal quotaCHF)
        {
            decimal actualQuotaCHF = await getQuotaCHFByUID(userID);
            decimal newQuotaCHF = additionQuotaCHF(quotaCHF, actualQuotaCHF);
            await updateQuotaCHFByUID(userID, newQuotaCHF);
            int copyQuota = convertQuotaCHFToCopyQuota(newQuotaCHF);
            await updateCopyQuotaByUID(userID, copyQuota);

            return (newQuotaCHF,copyQuota, true);
        }

        public async Task<(decimal ,bool )> creditUsernameWithQuotaCHF(string username, decimal quotaCHF)
        {
            if (await _authHelper.findByUsername(username))
            {
                Guid userID = await getUIDByUsername(username);
                decimal actualQuotaCHF = await getQuotaCHFByUID(userID);
                decimal newQuotaCHF = additionQuotaCHF(quotaCHF, actualQuotaCHF);
                await updateQuotaCHFByUID(userID, newQuotaCHF);
                int copyQuota = convertQuotaCHFToCopyQuota(newQuotaCHF);
                await updateCopyQuotaByUID(userID, copyQuota);
                return (quotaCHF ,true);
            }
            else
            {
                return (0, false);
            }

        }


    }
}
