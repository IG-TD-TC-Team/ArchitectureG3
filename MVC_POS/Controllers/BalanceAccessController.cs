using Microsoft.AspNetCore.Mvc;
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
            // Check if we have authenticated user
            if (TempData["UserID"] == null)
            {
                // Redirect to AuthenticationAccessController's Card action
                return RedirectToAction("AuthenticateByCard", "AuthenticationAccess");
            }

            var model = new BalanceM
            {
                UserID = Guid.Parse(TempData["UserID"].ToString())
            };

            // Keep the data for the POST request
            TempData.Keep("UserID");
            ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuotaByUID(BalanceM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
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
                // Redirect to AuthenticationAccessController's Index action
                return RedirectToAction("Index", "AuthenticationAccess");
            }

            var model = new TransactionM
            {
                AmountCredited = decimal.Parse(TempData["AmountAdded"]?.ToString() ?? "0"),
                NewQuotaCHF = decimal.Parse(TempData["NewQuotaCHF"]?.ToString() ?? "0"),
                NewPrintQuota = int.Parse(TempData["NewPrintQuota"]?.ToString() ?? "0"),
                IsSuccessful = true,
                Message = "Transaction completed successfully"
            };

            return View(model);
        }
    }
}