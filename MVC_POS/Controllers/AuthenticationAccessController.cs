using Microsoft.AspNetCore.Mvc;
using MVC_POS.Services;
using MVC_POS.Models;
using System;
using System.Threading.Tasks;

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
                    // Store the user info in TempData for the next view
                    TempData["UID"] = authResponse.UserID.ToString();
                    TempData["SuccessMessage"] = authResponse.Message;

                    // Redirect to the AddQuotaByUID view
                    return RedirectToAction("AddQuotaByUID");
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
    }

   
    }
