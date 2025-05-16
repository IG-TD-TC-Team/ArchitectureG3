using Digitec_WebClient.Models;
using Microsoft.AspNetCore.Mvc;
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


        //----------------------Authentification------------------------//
        [HttpGet]
        public async Task<IActionResult> AuthenticateByUsername()
        {
            //Step0
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticateByUsername(AuthentificationM userAuth)
        {
            //Step1
            var auth = await _printServices.AuthenticateByUsername(userAuth);
            return View(userAuth);

        }

        //------------------------AddCredits-----------------------------//
        [HttpGet]
        public async Task<IActionResult> creditUsernameWithQuotaCHF()
        {
            //Step0
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> creditUsernameWithQuotaCHF(UserM user)
        {
            var creditAddedToUser = await _printServices.creditUsernameWithQuotaCHF(user);
            return View(creditAddedToUser);
        }

    }
}
