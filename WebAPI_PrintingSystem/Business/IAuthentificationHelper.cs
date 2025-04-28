
using DAL.Models;

namespace WebAPI_PrintingSystem.Business
{
    public interface IAuthentificationHelper
    {
        Task <IEnumerable<User>> GetUsers();
    }
}
