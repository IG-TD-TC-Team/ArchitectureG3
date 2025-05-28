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
        public async Task<IActionResult> AuthenticateByUsername()
        {
            //Step0
            return View(new AuthentificationM());
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticateByUsername(AuthentificationM userAuth)
        {
            //Step1

            if (!ModelState.IsValid)
            {
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
                    TempData["SuccessMessage"] = authResponse.Message;

                    // Redirect to the AddQuotaByUsername view
                    return RedirectToAction("AddQuotaByUsername");
                }
                else
                {
                    // Authentication failed, show error message
                    ModelState.AddModelError("", (authResponse.Message));
                    return View(userAuth);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Authentication error: {ex.Message}");
                return View(userAuth);
            }

        }

        //------------------------AddQuotaByUsername-----------------------------//
        [HttpGet]
        public IActionResult AddQuotaByUsername()
        {
            // Check if we have user info from authentication
            if (TempData["UID"] == null || TempData["Username"] == null)
            {
                TempData["ErrorMessage"] = "Please authenticate first.";
                return RedirectToAction("AuthenticateByUsername");
            }

            var model = new UserM
            {
                UserID = Guid.Parse(TempData["UID"].ToString()),
                Username = TempData["Username"].ToString()
            };

            // Keep the data for the POST request
            TempData.Keep("UID");
            TempData.Keep("Username");

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
                TempData["ErrorMessage"] = "Please authenticate first.";
                return RedirectToAction("AuthenticateByUsername");
            }

            // Create a model for group quota operations
            var model = new GroupQuotaM
            {
                // We can pre-populate with common group names if desired
                GroupName = "",
                QuotaCHF = 0,
                AuthenticatedUser = TempData["Username"].ToString()
            };

            // Keep the authentication data for the POST request
            TempData.Keep("UID");
            TempData.Keep("Username");

            ViewBag.SuccessMessage = TempData["SuccessMessage"];

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
