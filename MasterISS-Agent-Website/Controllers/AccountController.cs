using MasterISS_Agent_Website.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MasterISS_Agent_Website_Localization;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace MasterISS_Agent_Website.Controllers
{
    public class AccountController : BaseController
    {
        public ActionResult SignIn()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SignIn(SignInViewModel signInViewModel)
        {
            if (ModelState.IsValid)
            {
                var wrapper = new WebServiceWrapper();
                var response = wrapper.Authenticate(signInViewModel);
                if (response.ResponseMessage.ErrorCode == 0)
                {
                    if (response.AuthenticationResponse.IsAuthenticated)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email,signInViewModel.Username),
                            new Claim("AgentId",response.AuthenticationResponse.AgentId.ToString()),
                            new Claim(ClaimTypes.Name,response.AuthenticationResponse.DisplayName),
                            new Claim(ClaimTypes.NameIdentifier,response.AuthenticationResponse.AgentId.ToString())
                        };

                        var authManager = Request.GetOwinContext().Authentication;
                        var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                        authManager.SignIn(identity);

                        return RedirectToAction("Index", "Home");
                    }
                    ViewBag.ErrorMessage = MasterISS_Agent_Website_Localization.View.GenericErrorMessage;
                }
                ViewBag.ErrorMessage = response.ResponseMessage.ErrorMessage;
                return View(signInViewModel);
            }
            return View(signInViewModel);
        }

        public ActionResult SignOut()
        {
            var authManager = Request.GetOwinContext().Authentication;
            authManager.SignOut();
            return RedirectToAction("SignIn", "Account");
        }
    }
}