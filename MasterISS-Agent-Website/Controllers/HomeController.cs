using MasterISS_Agent_Website.ViewModels.Home;
using MasterISS_Agent_Website_Enums.Enums;
using MasterISS_Agent_Website_Localization.Generic;
using MasterISS_Agent_Website_Localization.Home;
using MasterISS_Agent_Website_WebServices.AgentWebService;
using Microsoft.Extensions.Caching.Memory;
using NLog;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
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
                    var userBillList = response.BillListResponse.Bills.Where(blr => blr.SubscriberNo == subscriberNo).OrderBy(blr => blr.IssueDate).Take(selectedBills.Length).Select(blr => new { blr.ID, blr.Total, billName = blr.ServiceName });

                    if (ValidBills(selectedBills, customerCode, subscriberNo) == false)
                    {
                        ViewBag.PayOldBillError = HomeView.PayOldBillError;

                        return View("Index", CustomerBills(response));
                    }

                    var billTotalCount = userBillList.Select(ubl => ubl.Total).Sum();
                    var billList = userBillList.Select(ubl => new CustomerBillIdAndCost { BillId = ubl.ID, Cost = ubl.Total, BillName = ubl.billName }).ToArray();
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
                    var billList = response.BillListResponse.PrePaidSubscriptionInfoes.Select(ubl => new CustomerBillIdAndCost { Cost = ubl.Total, BillName = ubl.ServiceName }).ToArray();
                    Session["BillsSumCount"] = billTotalCount;
                    Session["BillList"] = billList;
                    Session["SubsNo"] = subscriberNo;
                    Session["SubsName"] = response.BillListResponse.SubscriberName;

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

        public MemoryCache cache = MemoryCache.Default;
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

                    var agentBills = string.Format("AgentBills+{0}", AgentClaimInfo.UserEmail());

                    var paidBillsViewModel = new List<ListAgentPaidBillsViewModel>();

                    var cacheItemPolicy = new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddHours(18)
                    };

                    if (cache.Get(agentBills) == null)
                    {
                        paidBillsViewModel.Add(new ListAgentPaidBillsViewModel
                        {
                            SubscriberName = billSubscriberName,
                            SubscriberNo = billSubscriberNo,
                            CustomerBillIdsAndCosts = selectedBills
                        });

                        var result = cache.Add(agentBills, paidBillsViewModel, cacheItemPolicy);
                    }
                    else
                    {
                        var data = cache.Get(agentBills);
                        var results = (List<ListAgentPaidBillsViewModel>)data;

                        results.Add(new ListAgentPaidBillsViewModel
                        {
                            SubscriberName = billSubscriberName,
                            SubscriberNo = billSubscriberNo,
                            CustomerBillIdsAndCosts = selectedBills
                        });

                        var resultCache = cache.Add(agentBills, results, cacheItemPolicy);
                    }


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

        public ActionResult PayBillPrePaid()
        {
            var wrapper = new WebServiceWrapper();
            var selectedBills = Session["BillList"] as CustomerBillIdAndCost[];
            var billSubscriberName = Session["SubsName"].ToString();
            var billSubscriberNo = Session["SubsNo"].ToString();

            if (selectedBills != null && selectedBills.Count() > 0)
            {
                var response = wrapper.PayBillsPrePaid(billSubscriberNo);
                if (response.ResponseMessage.ErrorCode == 0)
                {
                    RemoveSessionsByBillOperations();

                    var agentBills = string.Format("AgentBills+{0}", AgentClaimInfo.UserEmail());

                    var paidBillsViewModel = new List<ListAgentPaidBillsViewModel>();

                    var cacheItemPolicy = new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddHours(18)
                    };

                    if (cache.Get(agentBills) == null)
                    {
                        paidBillsViewModel.Add(new ListAgentPaidBillsViewModel
                        {
                            SubscriberName = billSubscriberName,
                            SubscriberNo = billSubscriberNo,
                            CustomerBillIdsAndCosts = selectedBills
                        });

                        var result = cache.Add(agentBills, paidBillsViewModel, cacheItemPolicy);
                    }
                    else
                    {
                        var data = cache.Get(agentBills);
                        var results = (List<ListAgentPaidBillsViewModel>)data;

                        results.Add(new ListAgentPaidBillsViewModel
                        {
                            SubscriberName = billSubscriberName,
                            SubscriberNo = billSubscriberNo,
                            CustomerBillIdsAndCosts = selectedBills
                        });

                        var resultCache = cache.Add(agentBills, results, cacheItemPolicy);
                    }


                    var message = MasterISS_Agent_Website_Localization.View.Successful;
                    return Json(new { status = "Success", message = message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    LoggerError.Fatal($"An error occurred while PrePaidPayBill, PayBillErrorCode: {response.ResponseMessage.ErrorCode}, PayBillErrorMessage: {response.ResponseMessage.ErrorMessage}, by: {AgentClaimInfo.UserEmail()}");
                    //LOG

                    RemoveSessionsByBillOperations();

                    var errorMessage = new LocalizedList<ErrorCodes, ErrorCodeList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
                    return Json(new { status = "FailedAndRedirect", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
                }
            }

            RemoveSessionsByBillOperations();
            //LOG
            LoggerError.Fatal($"An error occurred while PrePaidPayBill => Selected Bills Null,  by: {AgentClaimInfo.UserEmail()}");
            //LOG

            var notDefined = MasterISS_Agent_Website_Localization.View.GenericErrorMessage;
            return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAgentsPaidBills()
        {
            var agentBills = string.Format("AgentBills+{0}", AgentClaimInfo.UserEmail());
            var data = cache.Get(agentBills);
            var results = (List<ListAgentPaidBillsViewModel>)data;
            if (results != null)
            {
                return View(results);
            }

            ViewBag.NotFoundPaidBills = "Ödenmiş Fatura Bulunamadı";
            return View();
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