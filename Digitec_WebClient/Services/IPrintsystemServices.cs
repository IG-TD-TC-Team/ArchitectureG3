using Digitec_WebClient.Models;

namespace MVC_Faculties.Services
{
    public interface IPrintsystemServices
    {
        Task<AuthentificationM> AuthenticateByUsername(AuthentificationM userAuth);

        Task<UserM> creditUsernameWithQuotaCHF(UserM user);
    }
}
