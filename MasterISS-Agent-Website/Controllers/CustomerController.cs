using MasterISS_Agent_Website_Enums.Enums;
using Microsoft.Extensions.Logging;
using NLog;
using RadiusR.DB.Enums;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Agent_Website.Controllers
{
    public class CustomerController : Controller
    {
        private static Logger Logger = LogManager.GetLogger("AppLogger");
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");
        // GET: Customer
        public ActionResult NewCustomer()
        {
            ViewBag.SubscriptionRegistrationType = SubscriptionRegistrationType(null);
            ViewBag.TckTypeList = TCKTypeList(null);
            ViewBag.SexexListByCorporative = SexesList(null);
            ViewBag.SexexListByIndividual = SexesList(null);
            ViewBag.CultureList = CultureList("tr-tr");
            ViewBag.ProfessionListByCorporative = ProfessionList((int)ProfessionType.DefaultJob);
            ViewBag.ProfessionListByIndividual = ProfessionList((int)ProfessionType.DefaultJob);
            ViewBag.NationalityListByCorporative = NationalityList((int)MasterISS_Agent_Website_Enums.Enums.NationalityList.Turkey);
            ViewBag.NationalityListByIndividual = NationalityList((int)MasterISS_Agent_Website_Enums.Enums.NationalityList.Turkey);
            ViewBag.PartnerTariffList = PartnerTariffList(null);
            ViewBag.CustomerTypeList = CustomerTypeList(null);

            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetProvinces();

            if (response.ResponseMessage.ErrorCode == 0)
            {
                ViewBag.ProvincesByGeneralInfo = new SelectList(response.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");
                ViewBag.ProvincesByCorporativeResidency = new SelectList(response.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");
                ViewBag.ProvincesByCorporativeCompany = new SelectList(response.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");
                ViewBag.ProvincesByIndividual = new SelectList(response.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");
                ViewBag.ProvincesBySetup = new SelectList(response.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");
            }
            else
            {
                LoggerError.Fatal($"An error occurred while NewCustomer(HttpGet) GetProvinces , ErrorCode: {response.ResponseMessage.ErrorCode} , ErrorMessage: {response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.UserEmail()}");
            }

            ViewBag.DistrictsByGeneralInfo = new SelectList("");
            ViewBag.RuralRegionsByGeneralInfo = new SelectList("");
            ViewBag.NeigboorHoodsByGeneralInfo = new SelectList("");
            ViewBag.StreetsByGeneralInfo = new SelectList("");
            ViewBag.BuildingsByGeneralInfo = new SelectList("");
            ViewBag.ApartmentsByGeneralInfo = new SelectList("");
            ViewBag.DistrictsBySetup = new SelectList("");
            ViewBag.RuralRegionsBySetup = new SelectList("");
            ViewBag.NeigboorHoodsBySetup = new SelectList("");
            ViewBag.StreetsBySetup = new SelectList("");
            ViewBag.BuildingsBySetup = new SelectList("");
            ViewBag.ApartmentsBySetup = new SelectList("");
            ViewBag.DistrictsByIndividual = new SelectList("");
            ViewBag.RuralRegionsByIndividual = new SelectList("");
            ViewBag.NeigboorHoodsByIndividual = new SelectList("");
            ViewBag.StreetsByIndividual = new SelectList("");
            ViewBag.BuildingsByIndividual = new SelectList("");
            ViewBag.ApartmentsByIndividual = new SelectList("");
            ViewBag.DistrictsByCorporativeResidency = new SelectList("");
            ViewBag.RuralRegionsByCorporativeResidency = new SelectList("");
            ViewBag.NeigboorHoodsByCorporativeResidency = new SelectList("");
            ViewBag.StreetsByCorporativeResidency = new SelectList("");
            ViewBag.BuildingsByCorporativeResidency = new SelectList("");
            ViewBag.ApartmentsByCorporativeResidency = new SelectList("");
            ViewBag.DistrictsByCorporativeCompany = new SelectList("");
            ViewBag.RuralRegionsByCorporativeCompany = new SelectList("");
            ViewBag.NeigboorHoodsByCorporativeCompany = new SelectList("");
            ViewBag.StreetsByCorporativeCompany = new SelectList("");
            ViewBag.BuildingsByCorporativeCompany = new SelectList("");
            ViewBag.ApartmentsByCorporativeCompany = new SelectList("");
            ViewBag.BillingPeriod = new SelectList("");

            ViewBag.DayList = DayList(null);
            ViewBag.MonthList = Monthlist(null);
            ViewBag.YearList = YearList(null, false);

            ViewBag.DayListByExpiryDate = DayList(null);
            ViewBag.MonthListByExpiryDate = Monthlist(null);
            ViewBag.YearListByExpiryDate = YearList(null, true);

            ViewBag.DayListByIssueDate = DayList(null);
            ViewBag.MonthListByIssueDate = Monthlist(null);
            ViewBag.YearListByIssueDate = YearList(null, false);

            ViewBag.HavePSTN = HavePSTN(null);

            return View();
        }







        private string ConvertCultureDisplayName(string convertedCulture)
        {
            var localizedList = new LocalizedList<CultureList, MasterISS_Agent_Website_Localization.Generic.CultureList>();
            if (convertedCulture == "tr-tr")
            {
                return localizedList.GetDisplayText((int)MasterISS_Agent_Website_Enums.Enums.CultureList.Turkish, CultureInfo.CurrentCulture);
            }
            else
            {
                return localizedList.GetDisplayText((int)MasterISS_Agent_Website_Enums.Enums.CultureList.English, CultureInfo.CurrentCulture);
            }
        }

        private List<KeyValuePair<int, string>> GetFormTypes(int? selectedValue)
        {
            var formTypesLocalized = new LocalizedList<GeneralPDFFormTypes, RadiusR.Localization.Lists.GeneralPDFFormTypes>().GetList(CultureInfo.CurrentCulture).Where(fl => fl.Key != (int)GeneralPDFFormTypes.TransferForm && fl.Key != (int)GeneralPDFFormTypes.TransportForm).ToList();
            return formTypesLocalized;
        }
        private SelectList PaymentDayList(long? tariffId, long? selectedValue)
        {
            if (tariffId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var paymentDayListResponse = wrapper.GetPaymentDays(tariffId.Value);

                if (paymentDayListResponse.ResponseMessage.ErrorCode == 0)
                {
                    var list = new SelectList(paymentDayListResponse.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }), "Value", "Name", selectedValue);
                    return list;
                }
            }
            return null;
        }

        private SelectList CustomerTypeList(int? selectedValue)
        {
            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetCustomerTypes();

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var list = new SelectList(response.KeyValueItemResponse.Where(ctl => ctl.Key != (long)CustomerType.PrivateCompany).Select(tck => new { Name = tck.Value, Value = tck.Key }), "Value", "Name", selectedValue);
                return list;
            }
            LoggerError.Fatal($"An error occurred while CustomerTypeList , ErrorCode: {response.ResponseMessage.ErrorCode} , ErrorMessage: {response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.UserEmail()}");
            return null;
        }

        private SelectList PartnerTariffList(int? selectedValue)
        {
            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetAgentTariffs();
            if (response.ResponseMessage.ErrorCode == 0)
            {
                var list = new SelectList(response.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }), "Value", "Name", selectedValue);
                return list;
            }
            LoggerError.Fatal($"An error occurred while PartnerTariffList , ErrorCode: {response.ResponseMessage.ErrorCode} , ErrorMessage: {response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.UserEmail()}");
            return null;
        }

        private SelectList NationalityList(int? selectedValue)
        {
            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetNationalities();
            if (response.ResponseMessage.ErrorCode == 0)
            {
                var list = new SelectList(response.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }), "Value", "Name", selectedValue);
                return list;
            }
            LoggerError.Fatal($"An error occurred while GetNationalities , ErrorCode: {response.ResponseMessage.ErrorCode} , ErrorMessage: {response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.UserEmail()}");
            return null;
        }

        private SelectList ProfessionList(int? selectedValue)
        {
            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetProfessions();
            if (response.ResponseMessage.ErrorCode == 0)
            {
                var list = new SelectList(response.KeyValueItemResponse.Select(tck => new { Name = tck.Value.ToUpper(), Value = tck.Key }), "Value", "Name", selectedValue);
                return list;
            }
            LoggerError.Fatal($"An error occurred while ProfessionList , ErrorCode: {response.ResponseMessage.ErrorCode} , ErrorMessage: {response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.UserEmail()}");
            return null;
        }

        private SelectList CultureList(string selectedValue)
        {
            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetCultures();
            if (response.ResponseMessage.ErrorCode == 0)
            {
                var list = new SelectList(response.KeyValueItemResponse.Select(tck => new { Name = ConvertCultureDisplayName(tck.Value), Value = tck.Value }), "Value", "Name", selectedValue);
                return list;
            }
            LoggerError.Fatal($"An error occurred while CultureList , ErrorCode: {response.ResponseMessage.ErrorCode} , ErrorMessage: {response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.UserEmail()}");
            return null;
        }

        private SelectList SubscriptionRegistrationType(int? selectedValue)
        {
            var registrationTypeLocalized = new LocalizedList<SubscriptionRegistrationType, RadiusR.Localization.Lists.SubscriptionRegistrationType>().GetList(CultureInfo.CurrentCulture).Where(s => s.Key != (int)RadiusR.DB.Enums.SubscriptionRegistrationType.Transfer);
            var list = new SelectList(registrationTypeLocalized.Select(r => new { Value = r.Key, Name = r.Value }), "Value", "Name", selectedValue);
            return list;
        }
        private SelectList TCKTypeList(int? selectedValue)
        {
            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetTCKTypes();

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var list = new SelectList(response.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }), "Value", "Name", selectedValue);
                return list;
            }
            LoggerError.Fatal($"An error occurred while TCKTypeList , ErrorCode: {response.ResponseMessage.ErrorCode} , ErrorMessage: {response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.UserEmail()}");
            return null;
        }
        private SelectList SexesList(int? selectedValue)
        {
            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetSexes();
            if (response.ResponseMessage.ErrorCode == 0)
            {
                var list = new SelectList(response.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }), "Value", "Name", selectedValue);
                return list;
            }
            LoggerError.Fatal($"An error occurred while SexesList , ErrorCode: {response.ResponseMessage.ErrorCode} , ErrorMessage: {response.ResponseMessage.ErrorMessage} by:{AgentClaimInfo.UserEmail()}");
            return null;
        }

        private SelectList HavePSTN(int? selectedValue)
        {
            var havePstnList = new LocalizedList<HavePSTN, MasterISS_Agent_Website_Localization.Generic.HavePSTN>().GetList(CultureInfo.CurrentCulture);

            var selectList = new SelectList(havePstnList.Select(pstnL => new { Name = pstnL.Value, Value = pstnL.Key }), "Value", "Name", selectedValue);

            return selectList;
        }

        private SelectList YearList(int? selectedValue, bool isExpiryDate)
        {
            var startDate = 1940;
            var yearList = new List<int>();

            if (isExpiryDate)
            {
                var expiryStartDate = 2021;
                for (int i = expiryStartDate; i <= DateTime.Now.Year + 10; i++)
                {
                    yearList.Add(i);
                }
            }
            else
            {
                for (int i = startDate; i <= DateTime.Now.Year; i++)
                {
                    yearList.Add(i);
                }
            }

            var list = new SelectList(yearList.Select(yl => new { Name = yl.ToString(), Value = yl }), "Value", "Name", selectedValue ?? null);
            return list;
        }

        private SelectList Monthlist(int? selectedValue)
        {
            var monthList = new List<int>();

            for (int i = 1; i <= 12; i++)
            {
                monthList.Add(i);
            }

            var list = new SelectList(monthList.Select(yl => new { Name = yl.ToString(), Value = yl }), "Value", "Name", selectedValue ?? null);
            return list;
        }

        private SelectList CustomerStateList(int? selectedValue)
        {
            var list = new LocalizedList<CustomerState, RadiusR.Localization.Lists.CustomerState>().GetList(CultureInfo.CurrentCulture);
            var CustomerStateList = new SelectList(list.Select(m => new { Name = m.Value, Value = m.Key }).ToArray(), "Value", "Name", selectedValue);
            return CustomerStateList;
        }

        private SelectList DayList(int? selectedValue)
        {
            var dayList = new List<int>();

            for (int i = 1; i <= 31; i++)
            {
                dayList.Add(i);
            }

            var list = new SelectList(dayList.Select(yl => new { Name = yl.ToString(), Value = yl }), "Value", "Name", selectedValue ?? null);
            return list;
        }
    }
}