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

namespace MasterISS_Agent_Website.Controllers
{
    [Authorize/*(Roles = "Admin")*/]
    public class SetupController : BaseController
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        SetupServiceWrapper _setupWrapper;
        public SetupController()
        {
            _setupWrapper = new SetupServiceWrapper();
        }
        // GET: Setup
        public ActionResult Index(GetTaskListViewModel getTaskListViewModel, int page = 1, int pageSize = 20)
        {
            getTaskListViewModel = getTaskListViewModel ?? new GetTaskListViewModel();

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
                            getTaskListViewModel.StartDate = startDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                            getTaskListViewModel.EndDate = endDate.Value.ToString("yyyy-MM-dd HH:mm:ss");

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
            LoggerError.Fatal($"An error occurred while GetTaskDetails GetTaskDetails , ErrorCode:{response.ResponseMessage.ErrorCode} ErrorMessage:{response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.UserEmail()}");

            ViewBag.ErrorMessage = response.ResponseMessage.ErrorMessage;
            return PartialView("_GetTaskDetails");
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

                        LoggerError.Fatal($"An error occurred while UpdateTaskStatus AddTaskStatusUpdate , ErrorCode:{response.ResponseMessage.ErrorCode} ErrorMessage:{response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.UserEmail()}");
                        return Json(new { status = "Failed", ErrorMessage = response.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
                    }

                }

                var errorMessage = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = "FailedAndRedirect", ErrorMessage = MasterISS_Agent_Website_Localization.View.GenericErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        private ServiceResponse<List<SetupTask>> FilteredTaskList(GetTaskListViewModel getTaskListViewModel)
        {
            var list = new List<SetupTask>();

            var response = _setupWrapper.GetTaskList(getTaskListViewModel);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                list = response.SetupTasks.Where(st => Convert.ToDateTime(st.TaskIssueDate) > Convert.ToDateTime(getTaskListViewModel.StartDate) && Convert.ToDateTime(st.TaskIssueDate) <= Convert.ToDateTime(getTaskListViewModel.EndDate)).ToList();

                return new ServiceResponse<List<SetupTask>>
                {
                    Data = list
                };
            }

            LoggerError.Fatal($"An error occurred while FilteredTaskList GetTaskList , ErrorCode:{response.ResponseMessage.ErrorCode} ErrorMessage:{response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.UserEmail()}");

            return new ServiceResponse<List<SetupTask>>
            {
                ErrorMessage = response.ResponseMessage.ErrorMessage
            };
        }

    }
}