using Digitec_WebClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MVC_Faculties.Services;

namespace MVC_Faculties.Controllers
{
    public class PrintsystemAccessController : Controller
    {
        private IPrintsystemServices _printServices;
       
        public PrintsystemAccessController (IPrintsystemServices printServices)
        {
            _printServices = printServices;
        }

        //----------------------INDEX-----------------------------------//
        [HttpGet]
        public async Task<IActionResult> Index()
        {         
            return View();
        }

        //----------------------Authentification------------------------//
        [HttpGet]
        public async Task<IActionResult> AuthenticateByUsername(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            //Step0
            return View(new AuthentificationM());
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticateByUsername(AuthentificationM userAuth, string returnUrl = null)
        {
            //Step1

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(userAuth);
            }

            try
            {
                var authResponse = await _printServices.AuthenticateByUsername(userAuth);

                if (authResponse.IsSuccessful)
                {
                    // Store the user info in TempData for the next view
                    TempData["UID"] = authResponse.UID.ToString();
                    TempData["Username"] = userAuth.Username;
                    TempData["IsStaff"] = authResponse.IsStaff.ToString();
                    TempData["SuccessMessage"] = authResponse.Message;

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        // Check if user has permission for the requested URL
                        if (returnUrl.Contains("AddQuotaByGroup", StringComparison.OrdinalIgnoreCase))
                        {
                            if (authResponse.IsStaff)
                            {
                                return LocalRedirect(returnUrl);
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Access denied: Only staff members can manage group quotas.";
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            // For other URLs, redirect normally
                            return LocalRedirect(returnUrl);
                        }
                    }
                    ///DEFAULT REDIRECT
                    // Redirect to the AddQuotaByUsername view
                    return RedirectToAction("AddQuotaByUsername");
                }
                else
                {
                    // Authentication failed, show error message
                    ViewBag.ReturnUrl = returnUrl;
                    ModelState.AddModelError("", (authResponse.Message));
                    return View(userAuth);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ReturnUrl = returnUrl;
                ModelState.AddModelError("", $"Authentication error: {ex.Message}");
                return View(userAuth);
            }

        }
        [HttpGet]
        public IActionResult ChangeUser()
        {
            TempData.Clear();
            TempData["InfoMessage"] = "Please authenticate as a different user.";
            return RedirectToAction("AuthenticateByUsername");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Clear all authentication data
            TempData.Clear();

            // Redirect back to index 
            return RedirectToAction("Index");
        }


        //------------------------AddQuotaByUsername-----------------------------//
        [HttpGet]
        public IActionResult AddQuotaByUsername()
        {
            // Check if we have user info from authentication
            if (TempData["UID"] == null || TempData["Username"] == null)
            {
                var returnUrl = Url.Action("AddQuotaByUsername", "PrintsystemAccess");
                return RedirectToAction("AuthenticateByUsername", new { returnUrl = returnUrl });
            }

            var model = new UserM
            {
                UserID = Guid.Parse(TempData["UID"].ToString()),
                Username = TempData["Username"].ToString()
            };

            // Keep the data for the POST request
            TempData.Keep("UID");
            TempData.Keep("Username");
            TempData.Keep("IsStaff");

            ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuotaByUsername(UserM quotaRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(quotaRequest);
            }

            try
            {
                var result = await _printServices.creditUsernameWithQuotaCHF(quotaRequest);

                
                TempData["SuccessMessage"] = $"Successfully added {quotaRequest.QuotaCHF:C} CHF quota to {quotaRequest.Username}'s account.";

                return RedirectToAction("QuotaAddedSuccess", new
                {
                    username = quotaRequest.Username,
                    amount = quotaRequest.QuotaCHF  // Amount added
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error adding quota: {ex.Message}");
                return View(quotaRequest);
            }
        }

        //------------------------Success Page-----------------------------//
        [HttpGet]
        public IActionResult QuotaAddedSuccess(string username, decimal amount)
        {
            ViewBag.Username = username;
            ViewBag.Amount = amount;
            return View();
        }


        //------------------------AddQuotaByGroup-----------------------------//
        [HttpGet]
        public IActionResult AddQuotaByGroup()
        {
            // Check if we have authenticated user info
            // This assumes the user has been authenticated and we have their info
            if (TempData["UID"] == null || TempData["Username"] == null)
            {
                var returnUrl = Url.Action("AddQuotaByGroup", "PrintsystemAccess");
                return RedirectToAction("AuthenticateByUsername", new { returnUrl = returnUrl });
            }

            //Check if is staff
            if (TempData["IsStaff"]?.ToString() != "True")
            {
                TempData["ErrorMessage"] = "Access denied: Only staff members can manage group quotas.";
                return RedirectToAction("Index");
            }




            // Create a model for group quota operations
            var model = new GroupQuotaM
            {
                // Default
                GroupName = "",
                QuotaCHF = 0,
                AuthenticatedUser = TempData["Username"].ToString()
            };

            // Keep the authentication data for the POST request
            TempData.Keep("UID");
            TempData.Keep("Username");
            TempData.Keep("IsStaff");

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.StaffMessage = "Staff access confirmed - you can manage group quotas.";

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuotaByGroup(GroupQuotaM model)
        {
            // Validate the submitted form data
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Ensure we have valid data
            if (string.IsNullOrWhiteSpace(model.GroupName) || model.QuotaCHF <= 0)
            {
                ModelState.AddModelError("", "Please provide a valid group name and amount.");
                return View(model);
            }

            try
            {
                // Call your service method to credit the entire group
                var creditedUsers = await _printServices.creditGroupWithQuotaCHF(
                    model.GroupName,
                    model.QuotaCHF
                );

                // Provide success feedback
                TempData["SuccessMessage"] = $"Successfully added {model.QuotaCHF:C} CHF quota to {creditedUsers.Count} users in the '{model.GroupName}' group.";

                return RedirectToAction("GroupQuotaSuccess", new
                {
                    groupName = model.GroupName,
                    amount = model.QuotaCHF,
                    userCount = creditedUsers.Count
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error adding quota to group: {ex.Message}");
                return View(model);
            }
        }

        //------------------------Success Page for Group Operations-----------------------------//
        [HttpGet]
        public IActionResult GroupQuotaSuccess(string groupName, decimal amount, int userCount)
        {
            ViewBag.GroupName = groupName;
            ViewBag.Amount = amount;
            ViewBag.UserCount = userCount;
            return View();
        }



    }
}
