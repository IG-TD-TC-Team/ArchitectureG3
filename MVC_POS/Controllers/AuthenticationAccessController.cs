using Microsoft.AspNetCore.Mvc;
using MVC_POS.Services;
using MVC_POS.Models;

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

            return View();
        }

        //----------------------Authentification------------------------//
        [HttpGet]
        public async Task<IActionResult> AuthenticateByCard()
        {
            //Step0
            return View(new AuthenticationM());
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticateByCard(AuthenticationM userAuth)
        {
            //Step1

            if (!ModelState.IsValid)
            {
                return View(userAuth);
            }

            try
            {
                var authResponse = await _authService.AuthenticateByCard(userAuth);

                if (authResponse.IsSuccessful)
                {
                    // Store the user info in TempData for the next view
                    TempData["UID"] = authResponse.UID.ToString();
                    TempData["Username"] = userAuth.Username;
                    TempData["SuccessMessage"] = authResponse.Message;

                    // Redirect to the AddQuotaByUID view
                    return RedirectToAction("AddQuotaByUID");
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
    }

   
    }
