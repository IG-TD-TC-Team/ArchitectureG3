using MVC_POS.Models;

namespace MVC_POS.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationM> AuthenticateByCard(Guid cardId);
    }
}
