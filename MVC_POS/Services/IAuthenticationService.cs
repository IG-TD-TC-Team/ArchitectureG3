using MVC_POS.Models;

namespace MVC_POS.Services
{
    public class IAuthenticationService
    {
        Task<AuthenticationM> AuthenticateByCardAsync(Guid cardId);
    }
}
