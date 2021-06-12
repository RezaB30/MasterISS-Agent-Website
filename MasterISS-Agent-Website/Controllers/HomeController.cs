using MasterISS_Agent_Website.ViewModels;
using MasterISS_Agent_Website.ViewModels.Home;
using MasterISS_Agent_Website_Enums.Enums;
using MasterISS_Agent_Website_Localization.Generic;
using MasterISS_Agent_Website_Localization.Home;
using MasterISS_Agent_Website_WebServices.AgentWebService;
using Microsoft.Extensions.Caching.Memory;
using NLog;
using PagedList;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using static MasterISS_Agent_Website_WebServices.AgentWebService.RelatedPaymentsResponse;

namespace MasterISS_Agent_Website.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        WebServiceWrapper _wrapper;

        public HomeController()
        {
            _wrapper = new WebServiceWrapper();
        }

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
                    var userBillList = response.BillListResponse.Bills.Where(blr => blr.SubscriberNo == subscriberNo).OrderBy(blr => blr.IssueDate).Take(selectedBills.Length).Select(blr => new { blr.ID, blr.Total, billName = blr.ServiceName });

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

        [HttpPost]
        public ActionResult ConfirmPrePaid(string subscriberNo, string customerCode)
        {
            var wrapper = new WebServiceWrapper();

            var response = wrapper.GetBills(customerCode);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var validSubscriber = response.BillListResponse.PrePaidSubscriptionInfoes.Where(pps => pps.SubscriberNo == subscriberNo).FirstOrDefault();

                if (validSubscriber != null)
                {
                    var billTotalCount = validSubscriber.Total;
                    var billList = response.BillListResponse.Bills.Select(ubl => new CustomerBillIdAndCost { BillId = ubl.ID, Cost = ubl.Total }).ToArray();
                    Session["BillsSumCount"] = billTotalCount;
                    Session["BillList"] = billList;
                    Session["SubsNo"] = subscriberNo;
                    Session["SubsName"] = response.BillListResponse.SubscriberName;
                    Session["CustomerCode"] = customerCode;
                    ViewBag.SumCount = billTotalCount;
                    ViewBag.PrePaid = "Pre Paid";

                    return View("ConfirmBills");
                }

                ViewBag.ResponseError = MasterISS_Agent_Website_Localization.View.GenericErrorMessage;
                return View("Index", CustomerBills(response));
            }

            ViewBag.ResponseError = new LocalizedList<ErrorCodes, ErrorCodeList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
            return View("Index");
        }


        public ActionResult PayBill(string type)
        {
            var selectedBills = Session["BillList"] as CustomerBillIdAndCost[];
            var billSubscriberName = Session["SubsName"].ToString();
            var billSubscriberNo = Session["SubsNo"].ToString();
            var billsIds = selectedBills.Select(sb => sb.BillId).ToArray();
            var customerCode = Session["CustomerCode"]?.ToString();

            var filterPaidBillList = new FilterAgentPaidBillsViewModel
            {
                PaymentDayStartDate = DateTime.Now.AddDays(-2).ToString("dd.MM.yyyy HH:mm"),
                PaymentDayEndDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                CustomerName = billSubscriberName,
            };

            if (type == "prePaid")
            {
                var response = _wrapper.PayBills(customerCode);

                if (response.ResponseMessage.ErrorCode == 0)
                {
                    RemoveSessionsByBillOperations();


                    return RedirectToAction("GetAgentsPaidBills", "Home", filterPaidBillList);
                }
                else
                {
                    LoggerError.Fatal($"An error occurred while PayBills, response: {response.ResponseMessage.ErrorCode}, PayBillErrorMessage: {response.ResponseMessage.ErrorMessage}, by: {AgentClaimInfo.UserEmail()}");
                    ViewBag.ErrorMessage = ExtensionMethods.GetConvertedErrorMessage(response.ResponseMessage.ErrorCode);

                    return View("ConfirmBills");
                }
            }
            else
            {
                if (selectedBills != null && selectedBills.Count() > 0)
                {
                    var responsePayBill = _wrapper.PayBills(billsIds);

                    if (responsePayBill.ResponseMessage.ErrorCode == 0)
                    {
                        RemoveSessionsByBillOperations();

                        return RedirectToAction("GetAgentsPaidBills", "Home", filterPaidBillList);
                    }
                    else
                    {
                        LoggerError.Fatal($"An error occurred while responsePayBill, PayBillErrorCode: {responsePayBill.ResponseMessage.ErrorCode}, PayBillErrorMessage: {responsePayBill.ResponseMessage.ErrorMessage}, by: {AgentClaimInfo.UserEmail()}");
                        ViewBag.ErrorMessage = ExtensionMethods.GetConvertedErrorMessage(responsePayBill.ResponseMessage.ErrorCode);
                        return View("ConfirmBills");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = MasterISS_Agent_Website_Localization.View.GenericErrorMessage;
                    return View("ConfirmBills");
                }
            }

        }

        private ServiceResponseRelatedPayment<RelatedPayments[]> FilteredAgentPaidBillList(FilterAgentPaidBillsViewModel filterAgentPaidBills, int page = 1, int pageSize = 20)
        {
            var response = _wrapper.GetRelatedPayments(page, pageSize, filterAgentPaidBills.CustomerName);

            var filteredList = new List<RelatedPayments>();

            if (response.ResponseMessage.ErrorCode == 0)
            {
                filteredList = response.RelatedPayments.RelatedPaymentList.ToList();

                if (!string.IsNullOrEmpty(filterAgentPaidBills.PaymentDayStartDate) && string.IsNullOrEmpty(filterAgentPaidBills.PaymentDayEndDate))
                {
                    var list = filteredList.Where(l => Convert.ToDateTime(l.PayDate) >= Convert.ToDateTime(filterAgentPaidBills.PaymentDayStartDate));
                    filteredList = list.ToList();
                }

                if (!string.IsNullOrEmpty(filterAgentPaidBills.PaymentDayStartDate) && !string.IsNullOrEmpty(filterAgentPaidBills.PaymentDayEndDate))
                {
                    var list = filteredList.Where(fl => Convert.ToDateTime(fl.PayDate) >= Convert.ToDateTime(filterAgentPaidBills.PaymentDayStartDate) && Convert.ToDateTime(fl.PayDate) <= Convert.ToDateTime(filterAgentPaidBills.PaymentDayEndDate).AddMinutes(1));
                    filteredList = list.ToList();
                }

                return new ServiceResponseRelatedPayment<RelatedPayments[]>
                {
                    Data = filteredList.ToArray(),
                    TotalPageCount = response.RelatedPayments.TotalPageCount
                };

            }

            LoggerError.Fatal($"An error occurred while GetAgentsPaidBills, GetRelatedPaymentsErrorCode: {response.ResponseMessage.ErrorCode}, GetRelatedPaymentsErrorMessage: {response.ResponseMessage.ErrorMessage}, by: {AgentClaimInfo.UserEmail()}");
            return new ServiceResponseRelatedPayment<RelatedPayments[]>
            {
                ErrorMessage = ExtensionMethods.GetConvertedErrorMessage(response.ResponseMessage.ErrorCode),
            };
        }

        public ActionResult GetAgentsPaidBills(FilterAgentPaidBillsViewModel filterAgentPaidBills, int page = 1, int pageSize = 20)
        {
            filterAgentPaidBills = filterAgentPaidBills ?? new FilterAgentPaidBillsViewModel();
            ViewBag.Search = filterAgentPaidBills;
            if (ModelState.IsValid)
            {
                var response = FilteredAgentPaidBillList(filterAgentPaidBills, page = 1, pageSize = 20);

                if (response.Data != null)
                {
                    var list = response.Data.Select(rpl => new ListAgentPaidBillsViewModel
                    {
                        SubscriberNo = rpl.SubscriberNo,
                        SubscriberName = rpl.ValidDisplayName,
                        BillId = rpl.BillID,
                        Cost = rpl.Cost,
                        Description = rpl.Description,
                        IssueDate = Convert.ToDateTime(rpl.IssueDate),
                        PayDate = Convert.ToDateTime(rpl.PayDate),
                    });

                    var totalPageCount = response.TotalPageCount;
                    var totalItemCount = response.TotalPageCount == 1 ? response.Data.Count() : totalPageCount * pageSize;
                    var pagedList = new StaticPagedList<ListAgentPaidBillsViewModel>(list, page, pageSize, totalItemCount);

                    return View(pagedList);

                }

                ViewBag.ErrorMessage = response.ErrorMessage;
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = MasterISS_Agent_Website_Localization.Setup.SetupView.DateFormatIsNotCorrect;
                return View();
            }
        }

        public ActionResult GetBillReceipt(long billId)
        {
            var response = _wrapper.GetBillReceipt(billId);
            if (response.ResponseMessage.ErrorCode == 0)
            {
                return File(response.BillReceiptResult.FileContent, response.BillReceiptResult.MIMEType);
            }
            else
            {
                LoggerError.Fatal($"An error occurred while GetBillReceipt, GetBillReceipt ErrorCode: {response.ResponseMessage.ErrorCode}, GetBillReceipt ErrorMessage: {response.ResponseMessage.ErrorMessage}, by: {AgentClaimInfo.UserEmail()}");

                TempData["GenericErrorMessage"] = ExtensionMethods.GetConvertedErrorMessage(response.ResponseMessage.ErrorCode);

                return RedirectToAction("Index", "Home");
            }
        }

        private void RemoveSessionsByBillOperations()
        {
            Session.Remove("BillsSumCount");
            Session.Remove("BillList");
            Session.Remove("SubsNo");
            Session.Remove("SubsName");
            Session.Remove("CustomerCode");
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
                    GenericBillInfoViewModel = new GenericBillInfoViewModel
                    {
                        Cost = b.Total,
                        BillName = b.ServiceName,
                        SubscriptionNo = b.SubscriberNo,
                    },
                }).OrderBy(bl => bl.GenericBillInfoViewModel.SubscriptionNo).ThenBy(bl => bl.IssueDate),
                PrePaidSubscriberInfos = response.BillListResponse.PrePaidSubscriptionInfoes == null ? Enumerable.Empty<GenericBillInfoViewModel>() : response.BillListResponse.PrePaidSubscriptionInfoes
                .Select(pps => new GenericBillInfoViewModel
                {
                    BillName = pps.ServiceName,
                    Cost = pps.Total,
                    SubscriptionNo = pps.SubscriberNo
                })
            };
            return billList;
        }


    }
}