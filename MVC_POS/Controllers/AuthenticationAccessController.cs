using Microsoft.AspNetCore.Mvc;
using MVC_POS.Models;
using MVC_POS.Services;
using System;
using System.Threading.Tasks;

namespace MVC_POS.Controllers
{
    public class AuthenticationAccessController : Controller
    {
        private readonly IAuthenticationService _authService;
        private readonly IBalanceService _balanceService;

        public AuthenticationAccessController(IAuthenticationService authService, IBalanceService balanceService)
        {
            _authService = authService;
            _balanceService = balanceService;
        }

        // GET: Authentication/Card
        [HttpGet]
        public IActionResult Card()
        {
            return View(new CardM());
        }

        // POST: Authentication/Card
        [HttpPost]
        public async Task<IActionResult> Card(CardM card)
        {
            if (!ModelState.IsValid)
            {
                return View(card);
            }

            try
            {
                var authResult = await _authService.AuthenticateByCardAsync(card.CardId);

                if (authResult.IsSuccessful)
                {
                    // Store the authenticated user's ID in TempData for use in next step
                    TempData["UserId"] = authResult.UID.ToString();
                    return RedirectToAction("AddCredit");
                }
                else
                {
                    ModelState.AddModelError("", authResult.Message);
                    return View(card);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Authentication error: {ex.Message}");
                return View(card);
            }
        }

        // GET: Authentication/AddCredit
        [HttpGet]
        public IActionResult AddCredit()
        {
            // Check if we have a user ID from previous authentication
            if (TempData["UserId"] == null)
            {
                return RedirectToAction("Card");
            }

            var model = new BalanceM
            {
                UserId = Guid.Parse(TempData["UserId"].ToString())
            };

            return View(model);
        }

        // POST: Authentication/AddCredit
        [HttpPost]
        public async Task<IActionResult> AddCredit(BalanceM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _balanceService.CreditUserWithQuotaCHFAsync(model.UserId, model.Amount);

                // Store the updated user information for display in the summary
                TempData["UserFirstName"] = user.FirstName;
                TempData["UserLastName"] = user.LastName;
                TempData["QuotaCHF"] = user.QuotaCHF.ToString();
                TempData["CopyQuota"] = user.CopyQuota.ToString();

                return RedirectToAction("Summary");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error adding credit: {ex.Message}");
                return View(model);
            }
        }

        // GET: Authentication/Summary
        public IActionResult Summary()
        {
            // Create a view model with the information from TempData
            var model = new UserM
            {
                FirstName = TempData["UserFirstName"]?.ToString(),
                LastName = TempData["UserLastName"]?.ToString(),
                QuotaCHF = decimal.Parse(TempData["QuotaCHF"]?.ToString() ?? "0"),
                CopyQuota = int.Parse(TempData["CopyQuota"]?.ToString() ?? "0")
            };

            return View(model);
        }
    }
}