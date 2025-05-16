using Microsoft.AspNetCore.Mvc;
using MVC_POS.Models;
using MVC_POS.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MVC_POS.Controllers
{
    public class BalanceController : Controller
    {
        private readonly IBalanceService _balanceService;

        public BalanceController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        // GET: Balance/CreditUser
        [HttpGet]
        public IActionResult CreditUser(Guid userId)
        {
            // Check if we have a userId - this should come from a successful authentication
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Card", "Authentication");
            }

            var model = new CreditViewModel
            {
                UserId = userId  // Use the parameter, not this.userId
            };

            return View(model);
        }

        // POST: Balance/CreditUser
        [HttpPost]
        public async Task<IActionResult> CreditUser(CreditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var updatedUser = await _balanceService.CreditUserWithQuotaCHFAsync(model.UserId, model.Amount);

                // Pass the updated user to the result view
                return View("CreditResult", updatedUser);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error adding credit: {ex.Message}");
                return View(model);
            }
        }

        // GET: Balance/UserBalance
        [HttpGet]
        public async Task<IActionResult> UserBalance(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Card", "Authentication");
            }

            try
            {
                var user = await _balanceService.GetUserBalanceAsync(userId);
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving balance: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}