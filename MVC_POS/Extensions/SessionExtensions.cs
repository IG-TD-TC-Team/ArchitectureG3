namespace MVC_POS.Extensions
{
    /// <summary>
    /// Session extensions for POS operations.
    /// Simplified compared to Faculties since POS has simpler authentication needs.
    /// These methods provide type-safe, null-safe access to session data.
    /// </summary>
    public static class SessionExtensions
    {
        private const string USER_ID_KEY = "UserId";
        private const string USERNAME_KEY = "Username";
        private const string CARD_ID_KEY = "CardId";

        /// <summary>
        /// Stores user authentication for POS operations.
        /// POS doesn't need staff/group information since it's card-based.
        /// </summary>
        /// <param name="session">The current HTTP session</param>
        /// <param name="userId">The authenticated user's ID</param>
        /// <param name="cardId">Optional: The card ID that was used for authentication</param>
        /// <param name="username">Optional: Username if available from the API response</param>
        public static void SetUserAuthentication(this ISession session, Guid userId, Guid? cardId = null, string? username = null)
        {
            
            session.SetString(USER_ID_KEY, userId.ToString());
            

            // Store optional data if provided
            if (cardId.HasValue)
            {
                session.SetString(CARD_ID_KEY, cardId.Value.ToString());
            }

            if (!string.IsNullOrEmpty(username))
            {
                session.SetString(USERNAME_KEY, username);
            }
        }

        /// <summary>
        /// Safely retrieves the user ID from session.
        /// Returns null if not found or invalid, preventing crashes.
        /// </summary>
        public static Guid? GetUserId(this ISession session)
        {
            try
            {
                var userIdString = session.GetString(USER_ID_KEY);
                return Guid.TryParse(userIdString, out var userId) ? userId : null;
            }
            catch
            {
                // If there's any error accessing session data, treat as not authenticated
                return null;
            }
        }

        /// <summary>
        /// Retrieves the username if it was stored during authentication.
        /// </summary>
        public static string? GetUsername(this ISession session)
        {
            try
            {
                return session.GetString(USERNAME_KEY);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves the card ID that was used for authentication.
        /// Useful for logging or displaying to users.
        /// </summary>
        public static Guid? GetCardId(this ISession session)
        {
            try
            {
                var cardIdString = session.GetString(CARD_ID_KEY);
                return Guid.TryParse(cardIdString, out var cardId) ? cardId : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if a user is currently authenticated.
        /// Enhanced version that verifies the user ID is valid.
        /// </summary>
        public static bool IsAuthenticated(this ISession session)
        {
            try
            {
                var userId = session.GetUserId();

                // Must have a valid, non-empty user ID to be considered authenticated
                return userId.HasValue && userId.Value != Guid.Empty;
            }
            catch
            {
                // If any error occurs checking authentication, fail safe
                return false;
            }
        }

        /// <summary>
        /// Clears all authentication data from the session.
        /// Use this when a user wants to scan a different card or end their session.
        /// </summary>
        public static void ClearAuthentication(this ISession session)
        {
            try
            {
                session.Remove(USER_ID_KEY);
                session.Remove(USERNAME_KEY);
                session.Remove(CARD_ID_KEY);
            }
            catch
            {
                // If there's an error clearing, try to clear the entire session
                try
                {
                    session.Clear();
                }
                catch
                {
                    // If even that fails, we can't do much more
                    // The session will eventually expire on its own
                }
            }
        }

        /// <summary>
        /// Gets a summary of the current session for debugging or display.
        /// Useful for showing users what card/account is currently active.
        /// </summary>
        public static string GetSessionSummary(this ISession session)
        {
            try
            {
                var userId = session.GetUserId();
                var cardId = session.GetCardId();
                var username = session.GetUsername();

                if (!userId.HasValue)
                {
                    return "No active session";
                }

                var summary = $"User: {userId}";
                if (cardId.HasValue)
                {
                    summary += $", Card: {cardId}";
                }
                if (!string.IsNullOrEmpty(username))
                {
                    summary += $", Username: {username}";
                }

                return summary;
            }
            catch
            {
                return "Session data unavailable";
            }
        }
    }
}