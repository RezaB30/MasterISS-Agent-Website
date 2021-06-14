using MasterISS_Agent_Website.ViewModels.User;
using MasterISS_Agent_Website_Business.Abstract;
using MasterISS_Agent_Website_Business.Concrete;
using MasterISS_Agent_Website_DataAccess.Concrete.EntityFramework;
using MasterISS_Agent_Website_Enums.Enums;
using MasterISS_Partner_Website_Database;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Agent_Website.Controllers
{
    public class UserController : BaseController
    {
        IRoleService _roleService;
        IRolePermissionService _rolePermissionService;
        IUserService _userService;
        IPermissionService _permissionService;
        WebServiceWrapper _wrapper;

        private static Logger Logger = LogManager.GetLogger("AppLogger");
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        public UserController()
        {
            _roleService = new RoleManager(new EfRoleDal());
            _rolePermissionService = new RolePermissionManager(new EfRolePermissionDal());
            _userService = new UserManager(new EfUserDal());
            _wrapper = new WebServiceWrapper();
            _permissionService = new PermissionManager(new EfPermissionDal());
        }

        public ActionResult Index()
        {
            var agentId = AgentClaimInfo.AgentId();

            var resultRole = _roleService.GetByFilter(r => r.AgentId == agentId);

            if (resultRole.IsSuccess)
            {
                ViewBag.RoleList = ExtensionMethods.NameValuePairSelectList(resultRole.Data.Select(arl => new KeyValuePair<int, string>(arl.Id, arl.RoleName)), null);
            }
            else
            {
                LoggerError.Fatal($"An error occurred while user Index resultRole : {resultRole.Message} by:{AgentClaimInfo.UserEmail()}");
            }

            using (var db = new MasterISSAgentWebSiteEntities())
            {
                var agentUserList = db.User.Where(u => u.AgentId == agentId).Select(u => new ListAgentUsersViewModel
                {
                    IsEnabled = u.IsEnabled,
                    NameSurname = u.NameSurname,
                    PhoneNumber = u.PhoneNumber,
                    RoleName = db.Role.Where(r => r.Id == u.RoleId).FirstOrDefault().RoleName,
                    UserEmail = u.Username,
                    UserId = u.Id
                }).ToList();

                return View(agentUserList);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateRolePermission(AvailablePermissionList availablePermissionList, int? roleId)
        {
            if (roleId.HasValue)
            {
                var resultRPGet = _rolePermissionService.GetByFilter(rp => rp.RoleId == roleId.Value);

                if (resultRPGet.IsSuccess)
                {
                    var resultRPRemoveRange = _rolePermissionService.RemoveRange(resultRPGet.Data);

                    if (resultRPRemoveRange.IsSuccess)
                    {
                        var addedRolePermission = availablePermissionList.SelectedPermissions.Select(selectedP => new RolePermission
                        {
                            PermissionId = selectedP,
                            RoleId = roleId.Value
                        });

                        var resultRPAddRange = _rolePermissionService.AddRange(addedRolePermission);

                        if (resultRPAddRange.IsSuccess)
                        {
                            return Json(new { status = "Success", message = resultRPAddRange.Message }, JsonRequestBehavior.AllowGet);
                        }

                        LoggerError.Fatal($"An error occurred while UpdateRolePermission(POST) resultRPAddRange : {resultRPAddRange.Message} by:{AgentClaimInfo.UserEmail()}");
                        return Json(new { status = "Failed", ErrorMessage = resultRPAddRange.Message }, JsonRequestBehavior.AllowGet);

                    }

                    LoggerError.Fatal($"An error occurred while UpdateRolePermission(POST) resultRPRemoveRange : {resultRPRemoveRange.Message} by:{AgentClaimInfo.UserEmail()}");
                    return Json(new { status = "Failed", ErrorMessage = resultRPRemoveRange.Message }, JsonRequestBehavior.AllowGet);

                }

                LoggerError.Fatal($"An error occurred while UpdateRolePermission(POST) resultRPGet : {resultRPGet.Message} by:{AgentClaimInfo.UserEmail()}");
                return Json(new { status = "Failed", ErrorMessage = resultRPGet.Message }, JsonRequestBehavior.AllowGet);

            }

            LoggerError.Fatal($"An error occurred while UpdateRolePermission(POST) RoleId Not Found by:{AgentClaimInfo.UserEmail()}");
            return Json(new { status = "Failed", ErrorMessage = MasterISS_Agent_Website_Localization.View.GenericErrorMessage }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult RolePermissionList(int roleId)
        {
            var rolePermissionResult = _rolePermissionService.GetByFilter(rp => rp.RoleId == roleId);


            if (rolePermissionResult.IsSuccess)
            {
                var permissonResult = _permissionService.GetAll();

                if (permissonResult.IsSuccess)
                {
                    var permissionList = permissonResult.Data.Select(p => new AvailablePermissionList
                    {
                        PermissionId = p.Id,
                        PermissionName = p.PermissionName,
                        IsSelected = rolePermissionResult.Data.Select(rp => rp.PermissionId).Contains(p.Id) == true ? "checked" : null
                    }).ToArray();


                    foreach (var item in permissionList)
                    {
                        item.PermissionName = ExtensionMethods.EnumDescription<PermissionList, MasterISS_Agent_Website_Localization.Generic.PermissionList>(item.PermissionId);

                    }
                    return Json(new { status = "Success", list = permissionList }, JsonRequestBehavior.AllowGet);

                }
                LoggerError.Fatal($"An error occurred while RolePermissionList permissonResult : {permissonResult.Message} by:{AgentClaimInfo.UserEmail()}");
                return Json(new { status = "Failed", ErrorMessage = permissonResult.Message }, JsonRequestBehavior.AllowGet);

            }

            LoggerError.Fatal($"An error occurred while RolePermissionList rolePermissionResult : {rolePermissionResult.Message} by:{AgentClaimInfo.UserEmail()}");
            return Json(new { status = "Failed", ErrorMessage = rolePermissionResult.Message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateUser(long userId)
        {
            var resultUser = _userService.GetById(userId);

            if (resultUser.IsSuccess)
            {
                var agentId = AgentClaimInfo.AgentId();

                var resultRole = _roleService.GetByFilter(r => r.IsEnabled == true && r.AgentId == agentId);

                if (resultRole.IsSuccess)
                {
                    ViewBag.RoleList = ExtensionMethods.NameValuePairSelectList(resultRole.Data.Select(arl => new KeyValuePair<int, string>(arl.Id, arl.RoleName)), resultUser.Data.RoleId);

                    var viewModel = new AddAndUpdateUserViewModel
                    {
                        PhoneNumber = resultUser.Data.PhoneNumber,
                        RoleId = resultUser.Data.RoleId,
                        UserEmail = resultUser.Data.Username,
                        UserNameSurname = resultUser.Data.NameSurname,
                        UserId = resultUser.Data.Id,
                        IsEnabled = resultUser.Data.IsEnabled
                    };

                    return PartialView("_UpdateUser", viewModel);
                }

                LoggerError.Fatal($"An error occurred while UpdateUser(Get) resultRole : {resultRole.Message} by:{AgentClaimInfo.UserEmail()}");
                ViewBag.ErrorMessage = resultRole.Message;
                return View();
            }

            LoggerError.Fatal($"An error occurred while UpdateUser(Get) resultUser : {resultUser.Message} by:{AgentClaimInfo.UserEmail()}");
            ViewBag.ErrorMessage = resultUser.Message;

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateUser(AddAndUpdateUserViewModel addAndUpdateUserViewModel)
        {
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                var resultUser = _userService.GetById(addAndUpdateUserViewModel.UserId);

                if (resultUser.IsSuccess)
                {
                    resultUser.Data.IsEnabled = addAndUpdateUserViewModel.IsEnabled;
                    resultUser.Data.NameSurname = addAndUpdateUserViewModel.UserNameSurname;

                    if (!string.IsNullOrEmpty(addAndUpdateUserViewModel.Password))
                    {
                        resultUser.Data.Password = _wrapper.CalculateHash<SHA256>(addAndUpdateUserViewModel.Password);
                    }

                    resultUser.Data.PhoneNumber = addAndUpdateUserViewModel.PhoneNumber;
                    resultUser.Data.RoleId = addAndUpdateUserViewModel.RoleId;
                    resultUser.Data.Username = addAndUpdateUserViewModel.UserEmail;

                    var resultUpdateUser = _userService.Update(resultUser.Data);

                    if (resultUpdateUser.IsSuccess)
                    {
                        return Json(new { status = "Success", message = resultUpdateUser.Message }, JsonRequestBehavior.AllowGet);
                    }

                    LoggerError.Fatal($"An error occurred while UpdateUser(Post) resultUpdateUser : {resultUpdateUser.Message} by:{AgentClaimInfo.UserEmail()}");
                    return Json(new { status = "Failed", ErrorMessage = resultUpdateUser.Message }, JsonRequestBehavior.AllowGet);

                }

                LoggerError.Fatal($"An error occurred while UpdateUser(Post) resultUser : {resultUser.Message} by:{AgentClaimInfo.UserEmail()}");
                return Json(new { status = "Failed", ErrorMessage = resultUser.Message }, JsonRequestBehavior.AllowGet);

            }

            var errorMessageModelState = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessageModelState }, JsonRequestBehavior.AllowGet);
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
        public ActionResult AddUser(AddAndUpdateUserViewModel addUserViewModel)
        {
            ModelState.Remove("IsEnabled");

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
                    LoggerError.Fatal($"An error occurred while AddUser(Post) result : {result.Message} by:{AgentClaimInfo.UserEmail()}");
                    return Json(new { status = "Failed", ErrorMessage = result.Message }, JsonRequestBehavior.AllowGet);
                }

            }

            var errorMessageModelState = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessageModelState }, JsonRequestBehavior.AllowGet);
        }
    }
}