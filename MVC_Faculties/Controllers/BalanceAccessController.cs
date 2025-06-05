using MVC_Faculties.Models;
using Microsoft.AspNetCore.Mvc;
using MVC_Faculties.Services;
using MVC_Faculties.Extensions;

namespace MVC_Faculties.Controllers
{
    /// <summary>
    /// Controller responsible for handling all balance and quota management operations.
    /// This separation allows us to focus solely on financial transactions and quota adjustments,
    /// </summary>
    public class BalanceAccessController : Controller
    {
        private readonly IBalanceService _balanceService;

        public BalanceAccessController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        /// <summary>
        /// Displays the form for adding quota to an individual user.
        /// This method first verifies that the user is authenticated, then presents the quota addition interface.
        /// If authentication is missing, it redirects to the authentication controller with a return URL.
        /// </summary>
        [HttpGet]
        public IActionResult AddQuotaByUsername()
        {
            // Users stay logged in across multiple operations
            if (!HttpContext.Session.IsAuthenticated())
            {
                var returnUrl = Url.Action("AddQuotaByUsername", "BalanceAccess");
                return RedirectToAction("AuthenticateByUsername", "AuthenticationAccess",
                    new { returnUrl = returnUrl });
            }

            // Create a model with the authenticated user's information
            var model = new UserM
            {
                UserID = HttpContext.Session.GetUserId()!.Value,
                Username = HttpContext.Session.GetUsername()!,
                Group = HttpContext.Session.GetUserGroup() ?? "unknown"
            };


            // Display any success messages from previous operations
            ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View(model);
        }

        /// <summary>
        /// Processes the quota addition request for an individual user.
        /// This method validates the request, calls the balance service to add the quota,
        /// and redirects to a success page with transaction details.
        /// </summary>
        /// <param name="quotaRequest">Contains the user information and quota amount to add</param>
        [HttpPost]
        public async Task<IActionResult> AddQuotaByUsername(UserM quotaRequest)
        {

            // Verify authentication is still valid
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToAction("AuthenticateByUsername", "AuthenticationAccess");
            }

            // Validate that all required information is present and correct
            if (!ModelState.IsValid)
            {
                return View(quotaRequest);
            }

            try
            {
                // Use the balance service to credit the user's account
                var result = await _balanceService.CreditUsernameWithQuotaCHF(quotaRequest);

                // Store success information for display on the confirmation page
                TempData["SuccessMessage"] = $"Successfully added {quotaRequest.QuotaCHF:C} CHF quota to {quotaRequest.Username}'s account.";

                // Redirect to a dedicated success page that shows transaction details
                return RedirectToAction("QuotaAddedSuccess", new
                {
                    username = quotaRequest.Username,
                    amount = quotaRequest.QuotaCHF
                });
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the quota addition process
                // Display the error to the user while keeping them on the same page
                ModelState.AddModelError("", $"Error adding quota: {ex.Message}");
                return View(quotaRequest);
            }
        }

        /// <summary>
        /// Displays the success confirmation page after a successful individual quota addition.
        /// This page provides clear feedback to the user about what operation was completed.
        /// </summary>
        /// <param name="username">The username of the user who received the quota</param>
        /// <param name="amount">The amount of quota that was added</param>
        [HttpGet]
        public IActionResult QuotaAddedSuccess(string username, decimal amount)
        {
            // Pass the transaction details to the view for display
            ViewBag.Username = username;
            ViewBag.Amount = amount;
            return View();
        }

        /// <summary>
        /// Displays the form for adding quota to all users in a specific group.
        /// This method checks for both authentication and staff permissions since group operations require elevated privileges.
        /// </summary>
        [HttpGet]
        public IActionResult AddQuotaByGroup()
        {
            // Check authentication
            if (!HttpContext.Session.IsAuthenticated())
            {
                var returnUrl = Url.Action("AddQuotaByGroup", "BalanceAccess");
                return RedirectToAction("AuthenticateByUsername", "AuthenticationAccess",
                    new { returnUrl = returnUrl });
            }

            // Check staff privileges using session
            if (!HttpContext.Session.IsUserStaff())
            {
                TempData["ErrorMessage"] = "Access denied: Only staff members can manage group quotas.";
                return RedirectToAction("Index", "AuthenticationAccess");
            }

            // Create a model for the group quota operation
            var model = new GroupQuotaM
            {
                GroupName = "", // User will fill this in
                QuotaCHF = 0,   // User will specify the amount
                AuthenticatedUser = HttpContext.Session.GetUsername()!
            };

      
            // Display any success messages from previous operations
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.StaffMessage = "Staff access confirmed - you can manage group quotas.";

            return View(model);
        }

        /// <summary>
        /// Processes the group quota addition request.
        /// This method validates the request, uses the balance service to credit all users in the specified group,
        /// and redirects to a success page showing the operation results.
        /// </summary>
        /// <param name="model">Contains the group name, quota amount, and authenticated user information</param>
        [HttpPost]
        public async Task<IActionResult> AddQuotaByGroup(GroupQuotaM model)
        {
            // Verify authentication and authorization
            if (!HttpContext.Session.IsAuthenticated() || !HttpContext.Session.IsUserStaff())
            {
                return RedirectToAction("Index", "AuthenticationAccess");
            }

            // Validate the submitted form data
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Additional validation to ensure we have meaningful data
            if (string.IsNullOrWhiteSpace(model.GroupName) || model.QuotaCHF <= 0)
            {
                ModelState.AddModelError("", "Please provide a valid group name and amount.");
                return View(model);
            }

            try
            {
                // Use the balance service to credit all users in the specified group
                var creditedUsers = await _balanceService.CreditGroupWithQuotaCHF(
                    model.GroupName,
                    model.QuotaCHF
                );

                // Store success information for the confirmation page
                TempData["SuccessMessage"] = $"Successfully added {model.QuotaCHF:C} CHF quota to {creditedUsers.Count} users in the '{model.GroupName}' group.";

                // Redirect to a success page with operation details
                return RedirectToAction("GroupQuotaSuccess", new
                {
                    groupName = model.GroupName,
                    amount = model.QuotaCHF,
                    userCount = creditedUsers.Count
                });
            }
            catch (Exception ex)
            {
                // Handle errors in group quota operations
                ModelState.AddModelError("", $"Error adding quota to group: {ex.Message}");
                return View(model);
            }
        }

        /// <summary>
        /// Displays the success confirmation page after a successful group quota operation.
        /// This page provides detailed information about how many users were affected and the total impact.
        /// </summary>
        /// <param name="groupName">The name of the group that received quota</param>
        /// <param name="amount">The amount added to each user in the group</param>
        /// <param name="userCount">The total number of users who received quota</param>
        [HttpGet]
        public IActionResult GroupQuotaSuccess(string groupName, decimal amount, int userCount)
        {
            // Pass the operation details to the view for comprehensive reporting
            ViewBag.GroupName = groupName;
            ViewBag.Amount = amount;
            ViewBag.UserCount = userCount;
            return View();
        }
    }
}