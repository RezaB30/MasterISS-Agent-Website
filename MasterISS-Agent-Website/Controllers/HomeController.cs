using MasterISS_Agent_Website.ViewModels.Home;
using MasterISS_Agent_Website_Enums.Enums;
using MasterISS_Agent_Website_Localization.Generic;
using MasterISS_Agent_Website_Localization.Home;
using MasterISS_Agent_Website_WebServices.AgentWebService;
using NLog;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Agent_Website.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        public ActionResult Index()
        {
            ViewBag.Error = "Error";
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index([Bind(Prefix = "search")] GetSubcriberBillsViewModel getSubcriberBillsViewModel)
        {
            getSubcriberBillsViewModel = getSubcriberBillsViewModel ?? new GetSubcriberBillsViewModel();

            if (ModelState.IsValid)
            {
                var wrapper = new WebServiceWrapper();
                var response = wrapper.GetBills(getSubcriberBillsViewModel.CustomerCode);
                if (response.ResponseMessage.ErrorCode == 0)
                {
                    if (response.BillListResponse.Bills != null)
                    {
                        ViewBag.Search = getSubcriberBillsViewModel;
                        return View(CustomerBills(response));
                    }
                }

                LoggerError.Fatal($"An error occurred while GetBills , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage: {response.ResponseMessage.ErrorMessage} by: {AgentClaimInfo.UserEmail()}");

                ViewBag.ResponseError = new LocalizedList<ErrorCodes, ErrorCodeList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);

            }
            ViewBag.Error = "Error";
            return View();
        }

        [HttpPost]
        public ActionResult ValidBillsBySubcriberNo(long[] selectedBills, string customerCode, string subscriberNo)
        {
            if (ValidBills(selectedBills, customerCode, subscriberNo))
            {
                return Json(new { status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "Failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ConfirmBills(long[] selectedBills, string customerCode, string subscriberNo)
        {
            if (selectedBills != null)
            {
                var wrapper = new WebServiceWrapper();
                var response = wrapper.GetBills(customerCode);
                if (response.ResponseMessage.ErrorCode == 0)
                {
                    var userBillList = response.BillListResponse.Bills.Where(blr => blr.SubscriberNo == subscriberNo).OrderBy(blr => blr.IssueDate).Take(selectedBills.Length).Select(blr => new { blr.ID, blr.Total });

                    if (ValidBills(selectedBills, customerCode, subscriberNo) == false)
                    {
                        ViewBag.PayOldBillError = HomeView.PayOldBillError;

                        return View("Index", CustomerBills(response));
                    }

                    var billTotalCount = userBillList.Select(ubl => ubl.Total).Sum();
                    var billList = userBillList.Select(ubl => new CustomerBillIdAndCost { BillId = ubl.ID, Cost = ubl.Total }).ToArray();
                    Session["BillsSumCount"] = billTotalCount;
                    Session["BillList"] = billList;
                    Session["SubsNo"] = customerCode;
                    Session["SubsName"] = response.BillListResponse.SubscriberName;

                    ViewBag.SumCount = billTotalCount;
                    return View("ConfirmBills");
                }
                return View("Index", CustomerBills(response));
            }
            return RedirectToAction("Index");
        }

        public ActionResult PayBill()
        {
            var wrapper = new WebServiceWrapper();
            var selectedBills = Session["BillList"] as CustomerBillIdAndCost[];
            var billSubscriberName = Session["SubsName"].ToString();
            var billSubscriberNo = Session["SubsNo"].ToString();

            if (selectedBills != null && selectedBills.Count() > 0)
            {
                var billsId = selectedBills.Select(sb => sb.BillId).ToArray();
                var responsePayBill = wrapper.PayBills(billsId);


                if (responsePayBill.ResponseMessage.ErrorCode == 0)
                {
                    RemoveSessionsByBillOperations();

                    var message = MasterISS_Agent_Website_Localization.View.Successful;
                    return Json(new { status = "Success", message = message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //LOG
                    LoggerError.Fatal($"An error occurred while PayBill, PayBillErrorCode: {responsePayBill.ResponseMessage.ErrorCode}, PayBillErrorMessage: {responsePayBill.ResponseMessage.ErrorMessage}, by: {AgentClaimInfo.UserEmail()}");
                    //LOG

                    RemoveSessionsByBillOperations();

                    var errorMessage = new LocalizedList<ErrorCodes, ErrorCodeList>().GetDisplayText(responsePayBill.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
                    return Json(new { status = "FailedAndRedirect", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                RemoveSessionsByBillOperations();

                //LOG
                LoggerError.Fatal($"An error occurred while PayBill => Selected Bills Null,  by: {AgentClaimInfo.UserEmail()}");
                //LOG

                var notDefined = MasterISS_Agent_Website_Localization.View.GenericErrorMessage;
                return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
            }
        }

        private void RemoveSessionsByBillOperations()
        {
            Session.Remove("BillsSumCount");
            Session.Remove("BillList");
            Session.Remove("SubsNo");
            Session.Remove("SubsName");
        }
        private bool ValidBills(long[] selectedBills, string customerCode, string subscriberNo)
        {
            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetBills(customerCode);
            var userBillList = response.BillListResponse.Bills.Where(blr => blr.SubscriberNo == subscriberNo).OrderBy(blr => blr.IssueDate).Take(selectedBills.Length).Select(blr => new { blr.ID, blr.Total });

            var isMatchedBills = userBillList.Select(ubl => ubl.ID).SequenceEqual(selectedBills);
            return isMatchedBills;
        }

        private CustomerBillCollectionViewModel CustomerBills(AgentServiceBillListResponse response)
        {
            var billList = new CustomerBillCollectionViewModel()
            {
                SubscriberName = response.BillListResponse.SubscriberName,
                Bills = response.BillListResponse.Bills == null ? Enumerable.Empty<BillsViewModel>() : response.BillListResponse.Bills.Select(b => new BillsViewModel()
                {
                    IssueDate = b.IssueDate,
                    BillID = b.ID,
                    DueDate = b.DueDate,
                    Cost = b.Total,
                    BillName = b.ServiceName,
                    SubscriptionNo = b.SubscriberNo,
                }).OrderBy(bl => bl.SubscriptionNo).ThenBy(bl => bl.IssueDate)
            };
            return billList;
        }


    }
}