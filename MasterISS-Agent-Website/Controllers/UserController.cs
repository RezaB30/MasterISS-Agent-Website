using MasterISS_Agent_Website.ViewModels.User;
using MasterISS_Agent_Website_Business.Abstract;
using MasterISS_Agent_Website_Business.Concrete;
using MasterISS_Agent_Website_DataAccess.Concrete.EntityFramework;
using MasterISS_Agent_Website_Enums.Enums;
using MasterISS_Partner_Website_Database;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Agent_Website.Controllers
{
    public class UserController : Controller
    {
        IRoleService _roleService;
        IRolePermissionService _rolePermissionService;
        IUserService _userService;
        WebServiceWrapper _wrapper;

        private static Logger Logger = LogManager.GetLogger("AppLogger");
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        public UserController()
        {
            _roleService = new RoleManager(new EfRoleDal());
            _rolePermissionService = new RolePermissionManager(new EfRolePermissionDal());
            _userService = new UserManager(new EfUserDal());
            _wrapper = new WebServiceWrapper();
        }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AddRole()
        {
            ViewBag.PermissionList = ExtensionMethods.EnumSelectList<PermissionList, MasterISS_Agent_Website_Localization.Generic.PermissionList>(null);
            return PartialView("_AddRole");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddRole(AddRoleViewModel addRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                if (ExtensionMethods.EnumIsDefinedforList<PermissionList>(addRoleViewModel.Permissions))
                {
                    var agentId = AgentClaimInfo.AgentId();
                    var role = new Role
                    {
                        IsEnabled = true,
                        RoleName = addRoleViewModel.RoleName,
                        AgentId = agentId
                    };

                    var resultRole = _roleService.Add(role);

                    var rolePermissions = addRoleViewModel.Permissions.Select(ar => new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = ar
                    });

                    var resultRolePermission = _rolePermissionService.AddRange(rolePermissions);

                    if (resultRole.IsSuccess && resultRolePermission.IsSuccess)
                    {
                        Logger.Info("Added role: " + role.RoleName + ", by: " + AgentClaimInfo.UserEmail());

                        return Json(new { status = "Success", message = MasterISS_Agent_Website_Localization.View.Successful }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        LoggerError.Fatal($"An error occurred while AddRole resultRole : {resultRole.Message} ,resultRolePermission : {resultRolePermission.Message}  by:{AgentClaimInfo.UserEmail()}");

                        return Json(new { status = "FailedAndRedirect", ErrorMessage = MasterISS_Agent_Website_Localization.View.GenericErrorMessage }, JsonRequestBehavior.AllowGet);
                    }
                }

                var errorMessage = MasterISS_Agent_Website_Localization.View.GenericErrorMessage;
                return Json(new { status = "FailedAndRedirect", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);

            }
            var errorMessageModelState = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessageModelState }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddUser()
        {
            var agentId = AgentClaimInfo.AgentId();

            var avaibleRoleList = _roleService.GetByFilter(r => r.IsEnabled == true && r.AgentId == agentId);

            ViewBag.RoleList = ExtensionMethods.NameValuePairSelectList(avaibleRoleList.Data.Select(arl => new KeyValuePair<int, string>(arl.Id, arl.RoleName)), null);

            return PartialView("_AddUser");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddUser(AddUserViewModel addUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var agentId = AgentClaimInfo.AgentId();

                var user = new User
                {
                    IsEnabled = true,
                    NameSurname = addUserViewModel.UserNameSurname,
                    AgentId = agentId,
                    Password = _wrapper.CalculateHash<SHA256>(addUserViewModel.Password),
                    PhoneNumber = addUserViewModel.PhoneNumber,
                    RoleId = addUserViewModel.RoleId,
                    Username = addUserViewModel.UserEmail,
                };

                var result = _userService.Add(user);

                if (result.IsSuccess)
                {
                    return Json(new { status = "Success", message = result.Message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "Failed", ErrorMessage = result.Message }, JsonRequestBehavior.AllowGet);
                }

            }

            var errorMessageModelState = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessageModelState }, JsonRequestBehavior.AllowGet);
        }
    }
}