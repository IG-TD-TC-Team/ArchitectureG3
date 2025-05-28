using MVC_POS.Models;

namespace MVC_POS.Services
{
    public interface IBalanceService
    {
        Task<UserM> CreditUIDWithQuotaCHF(UserM quotaRequest);
    }
}
