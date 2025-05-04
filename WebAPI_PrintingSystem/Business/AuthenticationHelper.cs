using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace WebAPI_PrintingSystem.Business
{
    public class AuthenticationHelper : IAuthentificationHelper
    {
        private readonly PrintingSystemContext _repo;

        public AuthenticationHelper(PrintingSystemContext repo)
        {
            _repo = repo;
        }

        public async Task<bool> findByCardID(Guid cardID)
        {
            var card = await _repo.Cards
                .Include(c => c.User)
                .FirstOrDefaultAsync(u => u.CardID == cardID);
            
            if (card != null && card.User != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> findByUsername(string username)
        {
            var user = await _repo.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> isCardActive(Guid cardID)
        {
            var card = await _repo.Cards.FirstOrDefaultAsync(u => u.CardID == cardID);
            if (card != null)
            {
                return card.IsActive;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> verifyPasswordWithUsername(string username, string password)
        {
            var user = await _repo.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user.Password == password)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _repo.Users.ToListAsync();

            return users;
        }

        public async Task<Guid> getUIDByCardID(Guid cardID)
        {
            Guid userID;

            var card = await _repo.Cards
               .Include(c => c.User)
               .FirstOrDefaultAsync(c => c.CardID == cardID);

            userID = card.User.UserID; // No error management if user is not found

            return userID;
        }


        //--------- Exposed Methods------------------

        public async Task<(string, Guid?)> authenticateByCard(Guid cardId) // Updated to match the interface signature
        {


            if (await findByCardID(cardId)) // Check if the card exists
            {
                if (await isCardActive(cardId)) // Check if the card is active
                {
                    try
                    {
                        var userId = await getUIDByCardID(cardId);
                        return ("Successfull access", userId);
                    }
                    catch (InvalidOperationException)
                    {
                        return ("Card has no associated user", null);
                    }
                }
                else
                {
                    return ("Card is not active", null);
                }
            }
            else
            {
                return ("Card not found", null);
            }
        }

        public async Task<string> authenticateByUsername(string username, string password)
        {


            if (await findByUsername(username))
            {
                if (await verifyPasswordWithUsername(username, password))
                {
                    return "Successfull access";
                }
                else
                {
                    return "Password incorrect";
                }
            }
            else
            {
                return "Username not found";

            }
        }


    }
}
