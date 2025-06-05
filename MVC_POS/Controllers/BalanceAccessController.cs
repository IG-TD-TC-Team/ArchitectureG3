using Microsoft.AspNetCore.Mvc;
using MVC_POS.Extensions;
using MVC_POS.Models;
using MVC_POS.Services;

namespace MVC_POS.Controllers
{
    public class BalanceAccessController : Controller
    {
        private readonly IBalanceService _balanceService;

        public BalanceAccessController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        //----------------------Add Credit------------------------//
        [HttpGet]
        public IActionResult AddQuotaByUID()
        {
            // Check if user is authenticated via session (not TempData)
            if (!HttpContext.Session.IsAuthenticated())
            {
                TempData["ErrorMessage"] = "Please scan your card first to authenticate.";
                return RedirectToAction("AuthenticateByCard", "AuthenticationAccess");
            }

            // Retrieve the user ID from session with null checking
            var userId = HttpContext.Session.GetUserId();

            // Double-check that we have valid session data
            if (!userId.HasValue || userId.Value == Guid.Empty)
            {
                // Session data is corrupted, force re-authentication
                HttpContext.Session.ClearAuthentication();
                TempData["ErrorMessage"] = "Authentication session expired. Please scan your card again.";
                return RedirectToAction("AuthenticateByCard", "AuthenticationAccess");
            }

            var model = new BalanceM
            {
                UserID = userId.Value
            };

            // Display any success messages from the authentication step
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.InfoMessage = TempData["InfoMessage"];


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuotaByUID(BalanceM model)
        {

            // Re-verify authentication for POST operations
            if (!HttpContext.Session.IsAuthenticated())
            {
                TempData["ErrorMessage"] = "Authentication session expired. Please scan your card again.";
                return RedirectToAction("AuthenticateByCard", "AuthenticationAccess");
            }

            //Validate the submitted form data
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Additional verification that the UserID in the form matches the session
            var sessionUserId = HttpContext.Session.GetUserId();
            if (!sessionUserId.HasValue || sessionUserId.Value != model.UserID)
            {
                ModelState.AddModelError("", "Session data mismatch. Please scan your card again.");
                HttpContext.Session.ClearAuthentication();
                return RedirectToAction("AuthenticateByCard", "AuthenticationAccess");
            }

            try
            {
                // Create a request object with the amount to credit
                var creditRequest = new UserM
                {
                    UserID = model.UserID,
                    QuotaCHF = model.QuotaCHF
                };

                var result = await _balanceService.CreditUIDWithQuotaCHF(creditRequest);

                // Store success information
                TempData["TransactionSuccess"] = "true";
                TempData["AmountAdded"] = model.QuotaCHF.ToString("F2");      // Convert decimal to string
                TempData["NewQuotaCHF"] = result.QuotaCHF.ToString("F2");     // Convert decimal to string
                TempData["NewPrintQuota"] = result.CopyQuota.ToString();

                return RedirectToAction("Summary");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error adding credit: {ex.Message}");
                return View(model);
            }
        }

        //----------------------Summary------------------------//
        [HttpGet]
        public IActionResult Summary()
        {
            if (TempData["TransactionSuccess"] == null)
            {
                TempData["ErrorMessage"] = "No transaction data found. Please start a new transaction.";
                // Redirect to AuthenticationAccessController's Index action
                return RedirectToAction("Index", "AuthenticationAccess");
            }

            var model = new TransactionM
            {
                AmountCredited = decimal.Parse(TempData["AmountAdded"]?.ToString() ?? "0"),
                NewQuotaCHF = decimal.Parse(TempData["NewQuotaCHF"]?.ToString() ?? "0"),
                NewPrintQuota = int.Parse(TempData["NewPrintQuota"]?.ToString() ?? "0"),
                UserId = Guid.Parse(TempData["TransactionUserID"]?.ToString() ?? Guid.Empty.ToString()),
                IsSuccessful = true,
                Message = "Transaction completed successfully"
            };

            // Provide user options for what to do next
            // Since session is still active, they can perform another transaction
            ViewBag.CanContinue = HttpContext.Session.IsAuthenticated();
            ViewBag.CurrentUserId = HttpContext.Session.GetUserId();

            return View(model);
        }

        /// <summary>
        /// Allows users to add more credit to the same card
        /// Takes advantage of the persistent session authentication
        /// </summary>
        [HttpGet]
        public IActionResult AddMoreCredit()
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                TempData["ErrorMessage"] = "Please scan your card first.";
                return RedirectToAction("AuthenticateByCard", "AuthenticationAccess");
            }

            TempData["InfoMessage"] = "Ready to add more credit to the same card.";
            return RedirectToAction("AddQuotaByUID");
        }

        /// <summary>
        /// Completes the session and clears authentication
        /// Useful when the transaction is completely finished
        /// </summary>
        [HttpPost]
        public IActionResult CompleteSession()
        {
            HttpContext.Session.ClearAuthentication();
            TempData["SuccessMessage"] = "Session completed successfully. Thank you for using the printing system!";
            return RedirectToAction("Index", "AuthenticationAccess");
        }

    }
}