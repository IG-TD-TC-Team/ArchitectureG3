using Digitec_WebClient.Models;
using Microsoft.AspNetCore.Mvc;

namespace MVC_Faculties.Services
{
    public interface IPrintsystemServices
    {
        Task<AuthentificationM> AuthenticateByUsername(AuthentificationM userAuth);

        Task<UserM> creditUsernameWithQuotaCHF(UserM quotaRequest);

        Task<List<UserM>> creditGroupWithQuotaCHF(string groupname, decimal amount);

        




    }
}
