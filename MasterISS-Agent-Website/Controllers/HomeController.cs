using MasterISS_Agent_Website.ViewModels.Home;
using MasterISS_Agent_Website_Enums.Enums;
using MasterISS_Agent_Website_Localization.Generic;
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
    public class HomeController : Controller
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        public ActionResult Index()
        {
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
                var response = wrapper.GetBills(getSubcriberBillsViewModel.GetBillsInputParameter);
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