using Microsoft.AspNetCore.Mvc;
using MVC_POS.Models;
using MVC_POS.Services;
using System;
using System.Threading.Tasks;

namespace MVC_POS.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        // GET: Authentication/Card
        [HttpGet]
        public IActionResult Card()
        {
            return View(new CardM());
        }

        // POST: Authentication/Card
        [HttpPost]
        public async Task<IActionResult> Card(CardM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var authResult = await _authService.AuthenticateByCardAsync(model.CardId);

                if (authResult.IsSuccessful)
                {
                    // Store the authenticated user's ID in TempData for use in next step
                    TempData["UserId"] = authResult.UID.ToString();

                    // Redirect to the balance controller to add credit
                    return RedirectToAction("CreditUser", "Balance", new { userId = authResult.UID });
                }
                else
                {
                    // Authentication failed, show error message
                    ModelState.AddModelError("", authResult.Message);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors
                ModelState.AddModelError("", $"Authentication error: {ex.Message}");
                return View(model);
            }
        }
    }
}