using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace WebAPI_PrintingSystem.Business
{
    public class AuthentificationHelper : IAuthentificationHelper
    {
        private readonly PrintingSystemContext _repo;

        public AuthentificationHelper(PrintingSystemContext repo)
        {
            _repo = repo;
        }
        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _repo.Users.ToListAsync();
 
            return users;
        }
    }
}
