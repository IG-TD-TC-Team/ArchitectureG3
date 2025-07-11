﻿
using DAL.Models;

namespace WebAPI_PrintingSystem.Business
{
    public interface IAuthentificationHelper
    {
    //-----------External Methods------------------
        /// <summary>
        /// Authenticates a user by their card ID
        /// </summary>
        Task<(string, Guid?)> authenticateByCard(Guid cardId);

        /// <summary>
        /// Authenticates a user by their username and password
        /// </summary>
        Task<(string, Guid?)> authenticateByUsername(string username, string password);

        /// <summary>
        /// Retrieves the user ID associated with a username
        /// </summary>
        Task<bool> checkUsername(string username);

        ///

        Task<(string message, Guid? uid, bool isStaff, string group)> authenticateByUsernameWithStaffCheck(string username, string password);

        //------------Internal Methods----------------
        /// <summary>
        /// Checks if a card exists in the database
        /// </summary>
        Task<bool> cardIDExists(Guid cardID);

        /// <summary>
        /// Verifies if a card is currently active
        /// </summary>
        Task<bool> isCardActive(Guid cardID);


        /// <summary>
        ///  Retrieves the user ID associated with a card
        ///  </summary>
        Task<Guid> getUIDByCardID(Guid cardID);


        /// <summary>
        /// Retrieves the user ID associated with a username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<Guid> getUIDByUsername(string username);

        /// <summary>
        /// Checks if a username exists
        /// </summary>
        Task<bool> usernameExists(string username);

        /// <summary>
        /// alidates password for a username
        /// </summary>
        Task<bool> verifyPasswordWithUsername(string username, string password);

        /// <summary>
        /// Checks if a user is a staff member based on their user ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> isUserStaff(Guid userId);

        



    }
}
