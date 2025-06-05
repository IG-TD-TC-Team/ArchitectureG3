using Microsoft.AspNetCore.Mvc;
using MVC_POS.Services;
using MVC_POS.Models;
using System;
using System.Threading.Tasks;
using MVC_POS.Extensions;

namespace MVC_POS.Controllers
{
    public class AuthenticationAccessController : Controller
    {
        private IAuthenticationService _authService;

        public AuthenticationAccessController(IAuthenticationService authService)
        {
            _authService = authService;
        }


        //----------------------INDEX-----------------------------------//
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //Check if someone is already authenticated via session
            if (HttpContext.Session.IsAuthenticated())
            {
                var currentUserId = HttpContext.Session.GetUserId();
                ViewBag.AuthenticatedMessage = $"Card already authenticated (User ID: {currentUserId}). You can proceed to add credit or scan a different card.";
            }

            return View();
        }

        //----------------------Authentification------------------------//
        [HttpGet]
        public async Task<IActionResult> AuthenticateByCard()
        {
            if (HttpContext.Session.IsAuthenticated())
            {
                var currentUserId = HttpContext.Session.GetUserId();
                ViewBag.CurrentUserMessage = $"Card already authenticated (User ID: {currentUserId}).";
                ViewBag.ShowContinueOption = true;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticateByCard(Guid cardID)
        {
            //Step1

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var authResponse = await _authService.AuthenticateByCard(cardID);

                if (authResponse.IsSuccessful)
                {
                    // This persists until the session expires or is manually cleared
                    HttpContext.Session.SetUserAuthentication((Guid)authResponse.UserID);

                    // This will be shown once on the next page and then disappear
                    TempData["SuccessMessage"] = authResponse.Message;

                    // Redirect to the AddQuotaByUID view
                    return RedirectToAction("AddQuotaByUID", "BalanceAccess");
                }
                else
                {
                    // Authentication failed, show error message
                    ModelState.AddModelError("", (authResponse.Message));
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Authentication error: {ex.Message}");
                return View();
            }

        }

        /// <summary>
        /// Allows users to clear their current session and scan a different card
        /// This is useful in POS scenarios where multiple people might use the same terminal
        /// </summary>
        [HttpPost]
        public IActionResult ScanDifferentCard()
        {
            // Clear the current session authentication
            HttpContext.Session.ClearAuthentication();

            // Provide feedback to the user
            TempData["InfoMessage"] = "Previous card session cleared. Please scan the new card.";

            return RedirectToAction("AuthenticateByCard");
        }

        /// <summary>
        /// Allows users to continue with their current authenticated session
        /// Useful when they navigated away and want to return to adding credit
        /// </summary>
        [HttpGet]
        public IActionResult ContinueWithCurrentCard()
        {
            // Verify that there's actually an authenticated session
            if (!HttpContext.Session.IsAuthenticated())
            {
                TempData["ErrorMessage"] = "No authenticated card found. Please scan your card first.";
                return RedirectToAction("AuthenticateByCard");
            }

            // Continue to the credit addition process
            return RedirectToAction("AddQuotaByUID", "BalanceAccess");
        }
    }
}
    

   
    
