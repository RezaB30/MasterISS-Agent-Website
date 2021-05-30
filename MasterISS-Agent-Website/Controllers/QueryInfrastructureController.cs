using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Agent_Website.Controllers
{
    [Authorize]
    public class QueryInfrastructureController : BaseController
    {
        // GET: QueryInfrastructure
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        public ActionResult Index()
        {
            var wrapper = new WebServiceWrapper();
            var provinceList = wrapper.GetProvinces();

            if (provinceList.ResponseMessage.ErrorCode != 0)
            {
                ViewBag.ErrorMessage = provinceList.ResponseMessage.ErrorMessage;
            }

            ViewBag.Provinces = new SelectList(provinceList.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult DistrictList(long id)
        {
            var wrapper = new WebServiceWrapper();
            var districtList = wrapper.GetDistricts(id);
            var list = districtList.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }).ToArray();

            return Json(new { list = list, errorMessage = districtList.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RuralRegionsList(long id)
        {
            var wrapper = new WebServiceWrapper();
            var ruralRegionsList = wrapper.GetRuralRegions(id);
            var list = ruralRegionsList.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }).ToArray();

            return Json(new { list = list, errorMessage = ruralRegionsList.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult NeighborhoodList(long id)
        {
            var wrapper = new WebServiceWrapper();
            var neighborhoodList = wrapper.GetNeighbourhoods(id);
            var list = neighborhoodList.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }).ToArray();

            return Json(new { list = list, errorMessage = neighborhoodList.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult StreetList(long id)
        {
            var wrapper = new WebServiceWrapper();
            var streetList = wrapper.GetStreets(id);
            var list = streetList.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }).ToArray();

            return Json(new { list = list, errorMessage = streetList.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BuildingList(long id)
        {
            var wrapper = new WebServiceWrapper();
            var buildList = wrapper.GetBuildings(id);
            var list = buildList.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }).ToArray();

            return Json(new { list = list, errorMessage = buildList.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ApartmentList(long id)
        {
            var wrapper = new WebServiceWrapper();
            var apartmentList = wrapper.GetApartments(id);
            var list = apartmentList.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }).ToArray();

            return Json(new { list = list, errorMessage = apartmentList.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }
    }
}