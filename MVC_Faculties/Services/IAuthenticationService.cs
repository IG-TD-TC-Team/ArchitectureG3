using MVC_Faculties.Models;

namespace MVC_Faculties.Services
{
    /// <summary>
    /// Interface for handling user authentication operations.
    /// Separated from balance operations to follow Single Responsibility Principle.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates a user by username and password, returning authentication details including staff status.
        /// </summary>
        /// <param name="userAuth">Authentication credentials containing username and password</param>
        /// <returns>Authentication result with user details and staff permissions</returns>
        Task<AuthentificationM> AuthenticateByUsername(AuthentificationM userAuth);
    }
}