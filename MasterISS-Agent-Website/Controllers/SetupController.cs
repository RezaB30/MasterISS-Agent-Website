using MasterISS_Agent_Website.ViewModels.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MasterISS_Agent_Website_Localization.Setup;
using MasterISS_Agent_Website_WebServices.SetupWebService;
using NLog;
using RadiusR.DB.Enums.CustomerSetup;
using PagedList;
using RadiusR.DB.Enums;
using MasterISS_Agent_Website_Enums.Enums;
using System.IO;
using MasterISS_Agent_Website_Business.Abstract;
using MasterISS_Agent_Website_Business.Concrete;
using MasterISS_Agent_Website_DataAccess.Concrete.EntityFramework;

namespace MasterISS_Agent_Website.Controllers
{
    [Authorize(Roles = "Admin,SetupManager")]
    public class SetupController : BaseController
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        SetupServiceWrapper _setupWrapper;

        public SetupController()
        {
            _setupWrapper = new SetupServiceWrapper();
        }

        public ActionResult Index(GetTaskListViewModel getTaskListViewModel, int page = 1, int pageSize = 20)
        {
            getTaskListViewModel = getTaskListViewModel ?? new GetTaskListViewModel();

            ViewBag.TaskTypes = ExtensionMethods.EnumSelectList<TaskTypes, RadiusR.Localization.Lists.CustomerSetup.TaskType>(getTaskListViewModel.TaskType ?? null);
            ViewBag.TaskStatus = ExtensionMethods.EnumSelectList<TaskStatuses, RadiusR.Localization.Lists.CustomerSetup.TaskStatuses>(getTaskListViewModel.TaskStatus ?? null);

            ViewBag.Search = getTaskListViewModel;

            if (ModelState.IsValid)
            {
                if (DateParseOperations.DateIsCorrrect(false, getTaskListViewModel.StartDate, getTaskListViewModel.EndDate))
                {
                    var startDate = DateParseOperations.ConvertDateTime(getTaskListViewModel.StartDate);
                    var endDate = DateParseOperations.ConvertDateTime(getTaskListViewModel.EndDate);

                    if (string.IsNullOrEmpty(getTaskListViewModel.StartDate) || string.IsNullOrEmpty(getTaskListViewModel.EndDate))
                    {
                        startDate = DateTime.Now.AddDays(-29);
                        endDate = DateTime.Now;
                    }
                    if (startDate <= endDate)
                    {
                        if (startDate.Value.AddDays(Properties.Settings.Default.SearchLimit) >= endDate)
                        {
                            getTaskListViewModel.StartDate = startDate.Value.ToString("dd.MM.yyyy HH:mm");
                            getTaskListViewModel.EndDate = endDate.Value.ToString("dd.MM.yyyy HH:mm");

                            var taskList = FilteredTaskList(getTaskListViewModel);
                            if (taskList.Data != null)
                            {
                                var list = taskList.Data.Select(tl => new ListTasksViewModel
                                {
                                    TaskIssueDate = Convert.ToDateTime(tl.TaskIssueDate),
                                    CustomerName = tl.ContactName,
                                    ReservationDate = Convert.ToDateTime(tl.ReservationDate),
                                    Province = tl.Province,
                                    City = tl.City,
                                    SubscriberNo = tl.SubscriberNo,
                                    TaskType = ExtensionMethods.EnumDescription<TaskTypes, RadiusR.Localization.Lists.CustomerSetup.TaskType>(tl.TaskType),
                                    TaskStatus = ExtensionMethods.EnumDescription<TaskStatuses, RadiusR.Localization.Lists.CustomerSetup.TaskStatuses>(tl.TaskStatus),
                                    TaskNo = tl.TaskNo
                                });

                                var totalCount = list.Count();

                                var pagedListmodel = new StaticPagedList<ListTasksViewModel>(list.Skip((page - 1) * pageSize).Take(pageSize), page, pageSize, totalCount);

                                return View(pagedListmodel);

                            }
                            else
                            {
                                ViewBag.ErrorMessage = taskList.ErrorMessage;
                                return View();
                            }
                        }

                        ViewBag.Max30Days = SetupView.Max30Days;
                        return View();
                    }

                    ViewBag.StartTimeBiggerThanEndTime = SetupView.StartDateBiggerThanEndDateError;
                    return View();

                }

                ViewBag.DateFormatIsNotCorrect = SetupView.DateFormatIsNotCorrect;
                return View();

            }

            ViewBag.ValidationError = "Error";
            return View();
        }

        public ActionResult GetTaskDetails(long taskNo)
        {
            var credentialsResponse = _setupWrapper.GetCustomerCredentials(taskNo);

            if (credentialsResponse.ResponseMessage.ErrorCode == 0)
            {
                _setupWrapper = new SetupServiceWrapper();

                var response = _setupWrapper.GetTaskDetails(taskNo);

                if (response.ResponseMessage.ErrorCode == 0)
                {
                    var taskDetails = new GetTaskDetailsViewModel
                    {
                        Address = response.SetupTask.Address,
                        BBK = response.SetupTask.BBK,
                        CustomerPhoneNo = response.SetupTask.CustomerPhoneNo,
                        CustomerType = ExtensionMethods.EnumDescription<CustomerType, RadiusR.Localization.Lists.CustomerType>(response.SetupTask.CustomerType),
                        Details = response.SetupTask.Details,
                        HasModem = response.SetupTask.HasModem,
                        ModemName = response.SetupTask.ModemName,
                        PSTN = response.SetupTask.PSTN,
                        TaskNo = response.SetupTask.TaskNo,
                        XDSLNo = response.SetupTask.XDSLNo,
                        XDSLType = ExtensionMethods.EnumDescription<XDSLTypes, RadiusR.Localization.Lists.XDSLType>(response.SetupTask.XDSLType),
                        Password = credentialsResponse.CustomerCredentials.Password,
                        Username = credentialsResponse.CustomerCredentials.Username,
                        TaskUpdates = response.SetupTask.TaskUpdates.Select(tu => new ViewModels.Customer.TaskUpdatesDetailListViewModel
                        {
                            CreationDate = Convert.ToDateTime(tu.CreationDate),
                            Description = tu.Description,
                            FaultCodes = ExtensionMethods.EnumDescription<FaultCodes, RadiusR.Localization.Lists.CustomerSetup.FaultCodes>(tu.FaultCode),
                            ReservationDate = Convert.ToDateTime(tu.ReservationDate),
                        })
                    };

                    return PartialView("_GetTaskDetails", taskDetails);
                }
                else
                {
                    LoggerError.Fatal($"An error occurred while GetTaskDetails  , ErrorCode:{response.ResponseMessage.ErrorCode} ErrorMessage:{response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.SubUserMail()}");
                    ViewBag.ErrorMessage = response.ResponseMessage.ErrorMessage;
                    return PartialView("_GetTaskDetails");
                }
            }
            else
            {
                LoggerError.Fatal($"An error occurred while GetCustomerCredentials  , ErrorCode:{credentialsResponse.ResponseMessage.ErrorCode} ErrorMessage:{credentialsResponse.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.SubUserMail()}");
                ViewBag.ErrorMessage = credentialsResponse.ResponseMessage.ErrorMessage;
                return PartialView("_GetTaskDetails");
            }

        }

        public ActionResult UpdateTaskStatus(long taskNo)
        {
            ViewBag.FaultCodes = ExtensionMethods.EnumSelectList<FaultCodes, RadiusR.Localization.Lists.CustomerSetup.FaultCodes>(null);
            var addTaskStatusUpdateViewModel = new AddTaskStatusUpdateViewModel
            {
                TaskNo = taskNo
            };

            return PartialView("_UpdateTaskStatus", addTaskStatusUpdateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateTaskStatus(AddTaskStatusUpdateViewModel addTaskStatusUpdateViewModel)
        {
            if (Enum.IsDefined(typeof(FaultCodes), addTaskStatusUpdateViewModel.FaultCode))
            {
                if (ModelState.IsValid)
                {
                    if ((addTaskStatusUpdateViewModel.FaultCode != FaultCodes.RendezvousMade) && !string.IsNullOrEmpty(addTaskStatusUpdateViewModel.ReservationDate))
                    {
                        addTaskStatusUpdateViewModel.ReservationDate = null;
                    }
                    if ((addTaskStatusUpdateViewModel.FaultCode == FaultCodes.RendezvousMade) && string.IsNullOrEmpty(addTaskStatusUpdateViewModel.ReservationDate))
                    {
                        return Json(new { status = "Failed", ErrorMessage = SetupView.CannotBeLeftBlankrReservationDate }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var response = _setupWrapper.AddTaskStatusUpdate(addTaskStatusUpdateViewModel);
                        if (response.ResponseMessage.ErrorCode == 0)
                        {
                            return Json(new { status = "Success", message = MasterISS_Agent_Website_Localization.View.Successful }, JsonRequestBehavior.AllowGet);
                        }

                        LoggerError.Fatal($"An error occurred while UpdateTaskStatus AddTaskStatusUpdate , ErrorCode:{response.ResponseMessage.ErrorCode} ErrorMessage:{response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.SubUserMail()}");
                        return Json(new { status = "Failed", ErrorMessage = response.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
                    }

                }

                var errorMessage = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = "FailedAndRedirect", ErrorMessage = MasterISS_Agent_Website_Localization.View.GenericErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        TimeSpan firtSessionTime;
        TimeSpan lastSessionTime;
        public ActionResult CustomerSessionInfo(long taskNo)
        {
            var response = _setupWrapper.GetCustomerSessionInfo(taskNo);
            if (response.ResponseMessage.ErrorCode == 0)
            {
                var sessionInfo = new ListCustomerSessionInfo
                {
                    FirstSessionInfo = new SessionInfo()
                    {
                        IPAddress = response.CustomerSessionBundle.FirstSession.IPAddress,
                        IsOnline = response.CustomerSessionBundle.FirstSession.IsOnline,
                        NASIPAddress = response.CustomerSessionBundle.FirstSession.NASIPAddress,
                        SessionId = response.CustomerSessionBundle.FirstSession.SessionId,
                        SessionStart = Convert.ToDateTime(response.CustomerSessionBundle.FirstSession.SessionStart),
                        SessionTime = TimeSpan.TryParse(response.CustomerSessionBundle.FirstSession.SessionTime, out firtSessionTime) == true ? firtSessionTime : TimeSpan.Zero
                    },
                    LastSessionInfo = new SessionInfo()
                    {
                        IPAddress = response.CustomerSessionBundle.LastSession.IPAddress,
                        IsOnline = response.CustomerSessionBundle.LastSession.IsOnline,
                        NASIPAddress = response.CustomerSessionBundle.LastSession.NASIPAddress,
                        SessionId = response.CustomerSessionBundle.LastSession.SessionId,
                        SessionStart = Convert.ToDateTime(response.CustomerSessionBundle.LastSession.SessionStart),
                        SessionTime = TimeSpan.TryParse(response.CustomerSessionBundle.LastSession.SessionTime, out lastSessionTime) == true ? lastSessionTime : TimeSpan.Zero
                    },
                };
                return PartialView("_CustomerSessionInfo", sessionInfo);
            }
            LoggerError.Fatal($"An error occurred while GetCustomerSessionInfo , ErrorCode:{response.ResponseMessage.ErrorCode} ErrorMessage:{response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.SubUserMail()}");
            ViewBag.ErrorMessage = response.ResponseMessage.ErrorMessage;
            return PartialView("_CustomerSessionInfo");
        }

        public ActionResult CustomerLineInfo(long taskNo)
        {
            var response = _setupWrapper.GetCustomerLineDetails(taskNo);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var lineInfo = new GetTaskLineInfoViewModel
                {
                    CurrentDownloadSpeed = response.CustomerLineDetails.CurrentDownloadSpeed,
                    CurrentUploadSpeed = response.CustomerLineDetails.CurrentUploadSpeed,
                    DownloadNoiseMargin = response.CustomerLineDetails.DownloadNoiseMargin,
                    DownloadSpeedCapasity = response.CustomerLineDetails.DownloadSpeedCapacity,
                    IsActive = response.CustomerLineDetails.IsActive,
                    ShelfCardPort = response.CustomerLineDetails.ShelfCardPort,
                    UploadNoiseMargin = response.CustomerLineDetails.UploadNoiseMargin,
                    UploadSpeedCapasity = response.CustomerLineDetails.UploadSpeedCapacity,
                    XDSLNo = response.CustomerLineDetails.XDSLNo
                };
                return PartialView("_CustomerLineInfo", lineInfo);
            }
            LoggerError.Fatal($"An error occurred while GetCustomerLineDetails , ErrorCode:{response.ResponseMessage.ErrorCode} ErrorMessage:{response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.SubUserMail()}");
            ViewBag.ErrorMessage = response.ResponseMessage.ErrorMessage;
            return PartialView("_CustomerLineInfo");
        }

        public ActionResult GetCustomerContract(long taskNo)
        {
            var response = _setupWrapper.GetCustomerContract(taskNo);
            if (response.ResponseMessage.ErrorCode == 0)
            {
                var fileCode = Convert.FromBase64String(response.CustomerContract.FileCode);
                var fileName = response.CustomerContract.FileName;

                return File(fileCode, "application/pdf", fileName);

            }
            TempData["GenericErrorMessage"] = response.ResponseMessage.ErrorMessage;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AddCustomerAttachment(long taskNo)
        {
            var addCustomerAttachmentViewModel = new AddCustomerAttachmentViewModel
            {
                TaskNo = taskNo,
            };

            ViewBag.AttachmentTypes = ExtensionMethods.EnumSelectList<AttachmentType, MasterISS_Agent_Website_Localization.Generic.AttachmentType>(null);

            return PartialView("_AddCustomerAttachment", addCustomerAttachmentViewModel);
        }

        [HttpPost]
        public ActionResult AddCustomerAttachment(AddCustomerAttachmentViewModel addCustomerAttachmentViewModel, IEnumerable<HttpPostedFileBase> uploadingFiles)
        {
            if (ModelState.IsValid)
            {
                var isValidAttachmentType = Enum.IsDefined(typeof(MasterISS_Agent_Website_Enums.Enums.AttachmentType), addCustomerAttachmentViewModel.AttachmentType);
                if (isValidAttachmentType)
                {
                    var validFiles = FileOperations.ValidFiles(uploadingFiles);
                    if (validFiles.Key)
                    {
                        foreach (var item in uploadingFiles)
                        {
                            addCustomerAttachmentViewModel.FileData = ExtensionMethods.ConvertToBase64(item);
                            addCustomerAttachmentViewModel.Extension = new FileInfo(item.FileName).Extension.Replace(".", "");

                            _setupWrapper = new SetupServiceWrapper();
                            var response = _setupWrapper.AddCustomerAttachment(addCustomerAttachmentViewModel);

                            if (response.ResponseMessage.ErrorCode != 0)
                            {
                                LoggerError.Fatal($"An error occurred while AddCustomerAttachment(HttpPost) AddCustomerAttachment , ErrorCode:{response.ResponseMessage.ErrorCode} ErrorMessage:{response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.SubUserMail()}");

                                return Json(new { status = "Failed", ErrorMessage = response.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        return Json(new { status = "Success", message = MasterISS_Agent_Website_Localization.View.Successful }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { status = "Failed", ErrorMessage = validFiles.Value }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { status = "FailedAndRedirect", ErrorMessage = MasterISS_Agent_Website_Localization.View.GenericErrorMessage }, JsonRequestBehavior.AllowGet);
            }

            var errorMessage = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        private ServiceResponse<List<SetupTask>> FilteredTaskList(GetTaskListViewModel getTaskListViewModel)
        {
            var list = new List<SetupTask>();

            var response = _setupWrapper.GetTaskList(getTaskListViewModel);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                list = response.SetupTasks.Where(st => Convert.ToDateTime(st.TaskIssueDate) > Convert.ToDateTime(getTaskListViewModel.StartDate) && Convert.ToDateTime(st.TaskIssueDate) <= Convert.ToDateTime(getTaskListViewModel.EndDate)).ToList();

                if (getTaskListViewModel.SearchedTaskNo != null)
                {
                    var filteredList = list.Where(fl => fl.TaskNo == getTaskListViewModel.SearchedTaskNo).ToList();
                    list = filteredList;
                }
                if (!string.IsNullOrEmpty(getTaskListViewModel.SearchedName))
                {
                    var filteredList = list.Where(fl => fl.ContactName.Contains(getTaskListViewModel.SearchedName)).ToList();
                    list = filteredList;
                }
                if (getTaskListViewModel.TaskStatus != null)
                {
                    var filteredList = list.Where(fl => fl.TaskStatus == getTaskListViewModel.TaskStatus).ToList();
                    list = filteredList;
                }
                if (getTaskListViewModel.TaskType != null)
                {
                    var filteredList = list.Where(fl => fl.TaskType == getTaskListViewModel.TaskType).ToList();
                    list = filteredList;
                }

                return new ServiceResponse<List<SetupTask>>
                {
                    Data = list
                };
            }

            LoggerError.Fatal($"An error occurred while FilteredTaskList GetTaskList , ErrorCode:{response.ResponseMessage.ErrorCode} ErrorMessage:{response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.SubUserMail()}");

            return new ServiceResponse<List<SetupTask>>
            {
                ErrorMessage = response.ResponseMessage.ErrorMessage
            };
        }

    }
}