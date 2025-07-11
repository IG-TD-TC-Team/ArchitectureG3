﻿using DAL;
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

        public async Task<bool> cardIDExists(Guid cardID)
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

        public async Task<bool> usernameExists(string username)
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

        public async Task<bool> isUserActive(Guid cardID)
        {
            var card = await _repo.Cards
                .Include(c => c.User)
                .FirstOrDefaultAsync(u => u.CardID == cardID);
            if (card != null && card.User != null)
            {
                return card.User.IsActive;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> isUserActive(string username)
        {
            var user = await _repo.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                return user.IsActive;
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

        public async Task<Guid> getUIDByUsername(string username)
        {
            var user = await _repo.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                throw new InvalidOperationException($"User with username {username} not found.");
            }

            return user.UserID;
        }

        public async Task<bool> isUserStaff(Guid userId)
        {
            try
            {
                var user = await _repo.Users.FirstOrDefaultAsync(u => u.UserID == userId);
                return user?.Group?.ToLower() == "staff" || user?.Group?.ToLower() == "admin";
            }
            catch (Exception)
            {
                return false;
            }
        }


        //--------- Exposed Methods------------------

        public async Task<(string, Guid?)> authenticateByCard(Guid cardId)
        {
            if (await cardIDExists(cardId)) // Check if the card exists
            {
                if (await isCardActive(cardId)) // Check if the card is active
                {
                    if (await isUserActive(cardId)) // Check if the user is active
                    {
                        try
                        {
                            var userId = await getUIDByCardID(cardId);
                            return ("Successful access", userId);
                        }
                        catch (InvalidOperationException)
                        {
                            return ("Card has no associated user", null);
                        }
                    }
                    else
                    {
                        return ("User is not active", null);
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


        public async Task<(string, Guid?)> authenticateByUsername(string username, string password)
        {
            if (await usernameExists(username))
            {
                if (await isUserActive(username))
                {
                    if (await verifyPasswordWithUsername(username, password))
                    {
                        try
                        {
                            var userId = await getUIDByUsername(username);
                            return ("Successful access", userId);
                        }
                        catch (InvalidOperationException)
                        {
                            return ("Username has no associated user ID", null);
                        }
                    }
                    else
                    {
                        return ("Incorrect password", null);
                    }
                }
                else
                {
                    return ("User is not active", null);
                }
            }
            else
            {
                return ("Username not found", null);
            }
        }

        public async Task <bool> checkUsername(string username)
        {
            if (await usernameExists(username))
            {
                if (await isUserActive(username))
                {
                    
                    return true;
                }
                else
                {   
                    throw new InvalidOperationException($"User with username {username} is not active.");
                    
                }

            }
            else
            {
                throw new InvalidOperationException($"User with username {username} not found.");
            }
        }

        public async Task<(string, Guid?, bool, string)> authenticateByUsernameWithStaffCheck(string username, string password)
        {
            if (await usernameExists(username))
            {
                if (await isUserActive(username))
                {
                    if (await verifyPasswordWithUsername(username, password))
                    {
                        try
                        {
                            var userId = await getUIDByUsername(username);
                            var isStaff = await isUserStaff(userId);
                            var user = await _repo.Users.FirstOrDefaultAsync(u => u.UserID == userId);
                            var userGroup = user?.Group ?? "unknown";


                            return ("Successful access", userId, isStaff, userGroup);
                        }
                        catch (InvalidOperationException)
                        {
                            return ("Username has no associated user ID", null, false, "unknown");
                        }
                    }
                    else
                    {
                        return ("Incorrect password", null, false, "unknown");
                    }
                }
                else
                {
                    return ("User is not active", null, false, "unknown");
                }
            }
            else
            {
                return ("Username not found", null, false, "unknown");
            }
        }

    }
}
