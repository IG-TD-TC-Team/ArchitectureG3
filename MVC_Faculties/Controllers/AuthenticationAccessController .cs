using MVC_Faculties.Models;
using Microsoft.AspNetCore.Mvc;
using MVC_Faculties.Services;

namespace MVC_Faculties.Controllers
{
    /// <summary>
    /// Controller responsible for handling all authentication-related operations.
    /// This separation allows us to focus solely on user login, logout, and session management,
    /// following the Single Responsibility Principle by keeping authentication logic separate from business operations.
    /// </summary>
    public class AuthenticationAccessController : Controller
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationAccessController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Displays the main authentication page where users can choose their authentication method.
        /// This serves as the entry point for users who need to authenticate before accessing other features.
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays the username/password authentication form.
        /// This method handles the initial GET request when users want to authenticate with credentials.
        /// </summary>
        /// <param name="returnUrl">The URL to redirect to after successful authentication</param>
        [HttpGet]
        public IActionResult AuthenticateByUsername(string returnUrl = null)
        {
            // Store the return URL so we know where to redirect after successful authentication
            ViewBag.ReturnUrl = returnUrl;

            // Provide a fresh authentication model for the user to fill out
            return View(new AuthentificationM());
        }

        /// <summary>
        /// Processes the username/password authentication attempt.
        /// This method validates the credentials, manages the authentication session,
        /// and determines where to redirect the user based on their permissions and requested destination.
        /// </summary>
        /// <param name="userAuth">The authentication credentials submitted by the user</param>
        /// <param name="returnUrl">The URL to redirect to after successful authentication</param>
        [HttpPost]
        public async Task<IActionResult> AuthenticateByUsername(AuthentificationM userAuth, string returnUrl = null)
        {
            // Validate that the submitted form data is complete and correct
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(userAuth);
            }

            try
            {
                // Attempt to authenticate the user using the authentication service
                var authResponse = await _authService.AuthenticateByUsername(userAuth);

                if (authResponse.IsSuccessful)
                {
                    // Authentication succeeded - store user information in TempData for use across requests
                    // TempData persists data for exactly one additional request
                    TempData["UID"] = authResponse.UID.ToString();
                    TempData["Username"] = userAuth.Username;
                    TempData["IsStaff"] = authResponse.IsStaff.ToString();
                    TempData["Group"] = authResponse.Group;
                    TempData["SuccessMessage"] = authResponse.Message;

                    // Determine where to redirect the user based on their requested destination and permissions
                    if (!string.IsNullOrWhiteSpace(returnUrl))
                    {
                        // User was trying to access a specific page before being prompted to authenticate
                        return await HandleReturnUrlRedirection(returnUrl, authResponse.IsStaff);
                    }
                    else
                    {
                        // No specific destination requested - redirect to the default post-authentication page
                         return RedirectToAction("AddQuotaByUsername", "BalanceAccess");
                    }
                }
                else
                {
                    // Authentication failed - display the error message to the user
                    ViewBag.ReturnUrl = returnUrl;
                    ModelState.AddModelError("", authResponse.Message);
                    return View(userAuth);
                }
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors during the authentication process
                ViewBag.ReturnUrl = returnUrl;
                ModelState.AddModelError("", $"Authentication error: {ex.Message}");
                return View(userAuth);
            }
        }

        /// <summary>
        /// Handles the logic for redirecting users to their requested destination after authentication.
        /// This method ensures that users are only redirected to pages they have permission to access.
        /// </summary>
        /// <param name="returnUrl">The originally requested URL</param>
        /// <param name="isStaff">Whether the authenticated user has staff privileges</param>
        private async Task<IActionResult> HandleReturnUrlRedirection(string returnUrl, bool isStaff)
        {
            // Check if the user is trying to access group management features
            if (returnUrl.Contains("AddQuotaByGroup", StringComparison.OrdinalIgnoreCase))
            {
                if (isStaff)
                {
                    // User has staff permissions - allow access to group management
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    // User lacks required permissions - deny access and redirect to home
                    TempData["ErrorMessage"] = "Access denied: Only staff members can manage group quotas.";
                    return RedirectToAction("Index", "AuthenticationAccess");
                }
            }
            else
            {
                // For other destinations, redirect normally (assuming they don't require special permissions)
                return LocalRedirect(returnUrl);
            }
        }

        /// <summary>
        /// Allows users to switch to a different account without completely logging out.
        /// This is useful in scenarios where multiple users might use the same workstation.
        /// </summary>
        [HttpGet]
        public IActionResult ChangeUser()
        {
            // Clear current authentication data but keep a helpful message
            TempData.Clear();
            TempData["InfoMessage"] = "Please authenticate as a different user.";

            // Redirect back to the authentication form
            return RedirectToAction("AuthenticateByUsername");
        }

        /// <summary>
        /// Completely logs out the current user and clears all session data.
        /// This provides a clean slate for the next user or session.
        /// </summary>
        [HttpGet]
        public IActionResult Logout()
        {
            // Clear all authentication and session data
            TempData.Clear();

            // Redirect to the main system entry point
            return RedirectToAction("Index", "AuthenticationAccess");
        }
    }
}