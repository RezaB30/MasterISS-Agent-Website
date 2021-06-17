using MasterISS_Agent_Website.ViewModels.Report;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Agent_Website.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ReportController : BaseController
    {
        private readonly WebServiceWrapper _wrapper;

        public ReportController()
        {
            _wrapper = new WebServiceWrapper();
        }

        // GET: Report
        public ActionResult Index(int page = 1, int pageSize = 20)
        {
            var response = _wrapper.GetAgentAllowances(page, pageSize);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var list = response.AgentAllowances.Collections.Select(ac => new ListAgentAllowenceViewModel
                {
                    AllowanceAmount = ac.AllowanceAmount,
                    CollectionId = ac.CollectionID,
                    CreationDate = Convert.ToDateTime(ac.CreationDate),
                    PaymentDate = Convert.ToDateTime(ac.PaymentDate),
                    PaymentStatus = ac.PaymentStatus
                });

                var totalPageCount = response.AgentAllowances.TotalPageCount;

                var totalItemCount = response.AgentAllowances.TotalPageCount == 1 ? list.Count() : totalPageCount * pageSize;

                var pagedList = new StaticPagedList<ListAgentAllowenceViewModel>(list, page, pageSize, totalItemCount);

                return View(pagedList);
            }

            ViewBag.ErrorMessage = ExtensionMethods.GetConvertedErrorMessage(response.ResponseMessage.ErrorCode);

            return View();
        }
    }
}