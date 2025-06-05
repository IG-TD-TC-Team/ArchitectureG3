namespace MVC_Faculties.Extensions
{
    /// <summary>
    /// Extension methods for working with user session data in a type-safe way.
    /// These methods encapsulate the complexity of session management and provide
    /// clear interfaces for storing and retrieving authentication information.
    /// </summary>
    public static class SessionExtensions
    {
        // Define constants 
        private const string USER_ID_KEY = "UserId";
        private const string USERNAME_KEY = "Username";
        private const string IS_STAFF_KEY = "IsStaff";
        private const string GROUP_KEY = "Group";

        /// <summary>
        /// Stores complete user authentication information in the session.
        /// This replaces the multiple TempData assignments you currently use.
        /// </summary>
        public static void SetUserAuthentication(this ISession session,
            Guid userId, string username, bool isStaff, string group)
        {
            session.SetString(USER_ID_KEY, userId.ToString());
            session.SetString(USERNAME_KEY, username);
            session.SetString(IS_STAFF_KEY, isStaff.ToString());
            session.SetString(GROUP_KEY, group);
        }

        /// <summary>
        /// Retrieves the authenticated user's ID from the session.
        /// Returns null if no user is authenticated or session has expired.
        /// </summary>
        public static Guid? GetUserId(this ISession session)
        {
            var userIdString = session.GetString(USER_ID_KEY);
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }

        /// <summary>
        /// Retrieves the authenticated user's username from the session.
        /// </summary>
        public static string? GetUsername(this ISession session)
        {
            return session.GetString(USERNAME_KEY);
        }

        /// <summary>
        /// Checks if the current user has staff privileges.
        /// Returns false if user is not authenticated or not staff.
        /// </summary>
        public static bool IsUserStaff(this ISession session)
        {
            var isStaffString = session.GetString(IS_STAFF_KEY);
            return bool.TryParse(isStaffString, out var isStaff) && isStaff;
        }

        /// <summary>
        /// Retrieves the user's group membership from the session.
        /// </summary>
        public static string? GetUserGroup(this ISession session)
        {
            return session.GetString(GROUP_KEY);
        }

        /// <summary>
        /// Checks if a user is currently authenticated.
        /// This is safer than checking individual properties.
        /// </summary>
        public static bool IsAuthenticated(this ISession session)
        {
            return session.GetUserId().HasValue &&
                   !string.IsNullOrEmpty(session.GetUsername());
        }

        /// <summary>
        /// Clears all authentication data from the session.
        /// Use this for logout functionality.
        /// </summary>
        public static void ClearAuthentication(this ISession session)
        {
            session.Remove(USER_ID_KEY);
            session.Remove(USERNAME_KEY);
            session.Remove(IS_STAFF_KEY);
            session.Remove(GROUP_KEY);
        }
    }
}