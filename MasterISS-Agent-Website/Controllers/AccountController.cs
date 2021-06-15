using MasterISS_Agent_Website.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MasterISS_Agent_Website_Localization;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using MasterISS_Agent_Website.ViewModels.Account;
using NLog;
using MasterISS_Agent_Website_Business.Abstract;
using MasterISS_Agent_Website_DataAccess.Concrete.EntityFramework;
using System.Security.Cryptography;
using MasterISS_Partner_Website_Database;
using MasterISS_Agent_Website_Business.Concrete;

namespace MasterISS_Agent_Website.Controllers
{
    public class AccountController : BaseController
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        IUserService _userService;

        public AccountController()
        {
            _userService = new UserManager(new EfUserDal());
        }

        public ActionResult SignIn()
        {
            return View();
        }

        public ActionResult SubUserSignIn()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SubUserSignIn(SubUserSignInViewModel subUserSignInViewModel)
        {
            if (ModelState.IsValid)
            {
                var wrapper = new WebServiceWrapper();

                var response = wrapper.GetAgentInfo(subUserSignInViewModel.AdminUsername);

                if (response.ResponseMessage.ErrorCode == 0)
                {
                    wrapper = new WebServiceWrapper();

                    var hashedPassword = wrapper.CalculateHash<SHA256>(subUserSignInViewModel.Password);

                    var result = _userService.Get(u => u.IsEnabled == true && u.Password == hashedPassword && u.Username == subUserSignInViewModel.Username && u.AgentId == response.AuthenticationResponse.AgentId);

                    if (result.IsSuccess)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email,subUserSignInViewModel.AdminUsername),
                            new Claim("SubMail",subUserSignInViewModel.Username),
                            new Claim("AgentId",response.AuthenticationResponse.AgentId.ToString()),
                            new Claim(ClaimTypes.Name,result.Data.NameSurname),
                            new Claim(ClaimTypes.NameIdentifier,result.Data.Id.ToString()),
                            new Claim("SetupServiceUser",response.AuthenticationResponse.SetupServiceUser),
                            new Claim("SetupServiceHash",response.AuthenticationResponse.SetupServiceHash),
                        };

                        using (var db = new MasterISSAgentWebSiteEntities())
                        {
                            var subUserPermission = db.User.Find(result.Data.Id).Role.RolePermission.Select(m => new Claim(ClaimTypes.Role, m.Permission.PermissionName)).ToList();
                            claims.AddRange(subUserPermission);
                        }

                        var authManager = Request.GetOwinContext().Authentication;
                        var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                        authManager.SignIn(identity);

                        return RedirectToAction("Index", "Home");
                    }

                    ViewBag.ErrorMessage = result.Message;
                    return View();
                }

                LoggerError.Fatal($"An error occurred while SubUserSignIn(POST)  , ErrorCode:{response.ResponseMessage.ErrorCode} ErrorMessage:{response.ResponseMessage.ErrorMessage}");
                ViewBag.ErrorMessage = ExtensionMethods.EnumDescription<MasterISS_Agent_Website_Enums.Enums.ErrorCodes, MasterISS_Agent_Website_Localization.Generic.ErrorCodeList>(response.ResponseMessage.ErrorCode);
                return View();
            }
            return View(subUserSignInViewModel);
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
                            new Claim("SubMail",signInViewModel.Username),
                            new Claim("AgentId",response.AuthenticationResponse.AgentId.ToString()),
                            new Claim(ClaimTypes.Name,response.AuthenticationResponse.DisplayName),
                            new Claim(ClaimTypes.NameIdentifier,response.AuthenticationResponse.AgentId.ToString()),
                            new Claim("SetupServiceUser",response.AuthenticationResponse.SetupServiceUser),
                            new Claim("SetupServiceHash",response.AuthenticationResponse.SetupServiceHash),
                            new Claim(ClaimTypes.Role,"Admin"),
                        };

                        var authManager = Request.GetOwinContext().Authentication;
                        var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                        authManager.SignIn(identity);

                        return RedirectToAction("Index", "Home");
                    }

                    LoggerError.Fatal($"An error occurred while SignIn(POST)  ,IsAuthenticated is false ");
                    ViewBag.ErrorMessage = MasterISS_Agent_Website_Localization.View.GenericErrorMessage;
                }

                LoggerError.Fatal($"An error occurred while SignIn(POST)  , ErrorCode:{response.ResponseMessage.ErrorCode} ErrorMessage:{response.ResponseMessage.ErrorMessage}");
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