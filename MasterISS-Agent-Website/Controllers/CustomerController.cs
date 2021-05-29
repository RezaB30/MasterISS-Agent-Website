using MasterISS_Agent_Website.ViewModels.Customer;
using MasterISS_Agent_Website_Enums.Enums;
using MasterISS_Agent_Website_Localization.Generic;
using MasterISS_Agent_Website_WebServices.AgentWebService;
using Microsoft.Extensions.Logging;
using NLog;
using PagedList;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewCustomer(NewCustomerViewModel addCustomerViewModel)
        {
            if (addCustomerViewModel.ExtraInfo.SubscriptionRegistrationTypeId.HasValue && addCustomerViewModel.IDCard.CardTypeId.HasValue && addCustomerViewModel.GeneralInfo.CustomerTypeId.HasValue && !string.IsNullOrEmpty(addCustomerViewModel.SubscriptionInfo.SetupAddress.Floor) && addCustomerViewModel.SubscriptionInfo.SetupAddress.PostalCode.HasValue && addCustomerViewModel.SubscriptionInfo.SetupAddress.ApartmentId.HasValue)
            {
                if (IsCustomerTypeIndividual((int)addCustomerViewModel.GeneralInfo.CustomerTypeId))
                {
                    RemoveModel("CorporateInfo");
                    if (addCustomerViewModel.Individual.SameSetupAddressByIndividual == true)
                    {
                        ChangeAddressInfo(addCustomerViewModel.Individual.ResidencyAddress, addCustomerViewModel.SubscriptionInfo.SetupAddress);
                        RemoveModel("Individual.ResidencyAddress");
                    }
                }
                else
                {
                    RemoveModel("Individual");
                    if (addCustomerViewModel.CorporateInfo.SameSetupAddressByCorporativeCompanyAddress == true)
                    {
                        ChangeAddressInfo(addCustomerViewModel.CorporateInfo.CompanyAddress, addCustomerViewModel.SubscriptionInfo.SetupAddress);
                        RemoveModel("CorporateInfo.CompanyAddress");
                    }
                    if (addCustomerViewModel.CorporateInfo.SameSetupAddressByCorporativeResidencyAddress == true)
                    {
                        ChangeAddressInfo(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress, addCustomerViewModel.SubscriptionInfo.SetupAddress);
                        RemoveModel("CorporateInfo.ExecutiveResidencyAddress");
                    }
                }

                if (addCustomerViewModel.GeneralInfo.SameSetupAddressByBilling == true)
                {
                    ChangeAddressInfo(addCustomerViewModel.GeneralInfo.BillingAddress, addCustomerViewModel.SubscriptionInfo.SetupAddress);
                    RemoveModel("GeneralInfo.BillingAddress");
                }

                IdCardValidationAndRemoveModelState((int)addCustomerViewModel.IDCard.CardTypeId, addCustomerViewModel.IDCard);

                if (Enum.IsDefined(typeof(SubscriptionRegistrationType), addCustomerViewModel.ExtraInfo.SubscriptionRegistrationTypeId))
                {
                    if (addCustomerViewModel.ExtraInfo.SubscriptionRegistrationTypeId != (int)RadiusR.DB.Enums.SubscriptionRegistrationType.Transition)
                    {
                        addCustomerViewModel.ExtraInfo.XDSLNo = null;
                        ModelState.Remove("ExtraInfo.XDSLNo");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                if (ModelState.IsValid)
                {

                    addCustomerViewModel.GeneralInfo.ContactPhoneNo = ReplacePhoneNo(addCustomerViewModel.GeneralInfo.ContactPhoneNo);
                    addCustomerViewModel.ExtraInfo.PSTN = ReplacePhoneNo(addCustomerViewModel.ExtraInfo.PSTN);

                    DateTime birthDay = new DateTime(addCustomerViewModel.IDCard.SelectedBirthYear.Value, addCustomerViewModel.IDCard.SelectedBirthMonth.Value, addCustomerViewModel.IDCard.SelectedBirthDay.Value);
                    addCustomerViewModel.IDCard.BirthDate = birthDay.ToString("dd.MM.yyyy");

                    var wrapperForIdCardValidation = new WebServiceWrapper();
                    var validationIdCard = wrapperForIdCardValidation.IDCardValidation(new IDCardValidationViewModel
                    {
                        BirtDate = DateParseOperations.ConvertDatetimeByWebService(addCustomerViewModel.IDCard.BirthDate),
                        FirstName = addCustomerViewModel.IDCard.FirstName,
                        IdCardType = addCustomerViewModel.IDCard.CardTypeId.Value,
                        LastName = addCustomerViewModel.IDCard.LastName,
                        TCKNo = addCustomerViewModel.IDCard.TCKNo,
                        RegistirationNo = addCustomerViewModel.IDCard.SerialNo
                    });

                    if (validationIdCard.IDCardValidationResponse)
                    {
                        if (IsCustomerTypeIndividual((int)addCustomerViewModel.GeneralInfo.CustomerTypeId))
                        {
                            var individualResidencyBBK = addCustomerViewModel.Individual.ResidencyAddress.ApartmentId;

                            var serviceWrapper = new WebServiceWrapper();
                            var individualAddress = serviceWrapper.GetApartmentAddress((long)individualResidencyBBK);

                            if (individualAddress.ResponseMessage.ErrorCode == 0)
                            {
                                addCustomerViewModel.Individual.ResidencyAddress.NewCustomerAddressInfoRequest = NewCustomerAddressInfoRequest(individualAddress.AddressDetailsResponse);
                            }
                            else
                            {
                                serviceWrapper = new WebServiceWrapper();
                                LoggerError.Fatal($"An error occurred while GetApartmentAddress , individualAddress: {individualAddress.ResponseMessage.ErrorCode} , by: {AgentClaimInfo.UserEmail()}");

                            }
                        }
                        else//This Is Corporative
                        {
                            var companyBBK = addCustomerViewModel.CorporateInfo.CompanyAddress.ApartmentId;
                            var executiveResidencyBBK = addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.ApartmentId;

                            var webServiceWrapper = new WebServiceWrapper();
                            var companyApartmentAddress = webServiceWrapper.GetApartmentAddress((long)companyBBK);

                            webServiceWrapper = new WebServiceWrapper();
                            var executiveResidencyAddress = webServiceWrapper.GetApartmentAddress((long)executiveResidencyBBK);

                            if (companyApartmentAddress.ResponseMessage.ErrorCode == 0 && executiveResidencyAddress.ResponseMessage.ErrorCode == 0)
                            {
                                addCustomerViewModel.CorporateInfo.CompanyAddress.NewCustomerAddressInfoRequest = NewCustomerAddressInfoRequest(companyApartmentAddress.AddressDetailsResponse);

                                addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.NewCustomerAddressInfoRequest = NewCustomerAddressInfoRequest(executiveResidencyAddress.AddressDetailsResponse);
                            }
                            else
                            {
                                webServiceWrapper = new WebServiceWrapper();
                                LoggerError.Fatal($"An error occurred while GetApartmentAddress , CompanyApartmentAddressResponse: {companyApartmentAddress.ResponseMessage.ErrorCode} ExecutiveResidencyBBKResponseCode: {executiveResidencyAddress.ResponseMessage.ErrorCode}, by: {AgentClaimInfo.UserEmail()}");
                            }
                        }


                        var billingAddressBBK = addCustomerViewModel.GeneralInfo.BillingAddress.ApartmentId;
                        var setupAddressBBK = addCustomerViewModel.SubscriptionInfo.SetupAddress.ApartmentId;

                        var wrapperByQueryApartmentAddress = new WebServiceWrapper();
                        var billingAddress = wrapperByQueryApartmentAddress.GetApartmentAddress((long)billingAddressBBK);

                        wrapperByQueryApartmentAddress = new WebServiceWrapper();
                        var setupAddress = wrapperByQueryApartmentAddress.GetApartmentAddress((long)setupAddressBBK);


                        if (setupAddress.ResponseMessage.ErrorCode == 0 && billingAddress.ResponseMessage.ErrorCode == 0 && (addCustomerViewModel.Individual.ResidencyAddress.ApartmentId != null || addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.ApartmentId != null))
                        {
                            addCustomerViewModel.GeneralInfo.BillingAddress.NewCustomerAddressInfoRequest = NewCustomerAddressInfoRequest(billingAddress.AddressDetailsResponse);

                            addCustomerViewModel.SubscriptionInfo.SetupAddress.NewCustomerAddressInfoRequest = NewCustomerAddressInfoRequest(setupAddress.AddressDetailsResponse);


                            var wrapperBySMSConfirmation = new WebServiceWrapper();

                            var smsConfirmation = wrapperBySMSConfirmation.SendConfirmationSMS(addCustomerViewModel.GeneralInfo.ContactPhoneNo);

                            if (smsConfirmation.ResponseMessage.ErrorCode == 0)
                            {
                                Session["CustomerApplicationInfo"] = addCustomerViewModel;
                                Session["SMSCode"] = smsConfirmation.SMSCodeResponse.Code;
                                return View("SmsConfirmation");
                            }
                            else
                            {
                                //LOG
                                wrapperBySMSConfirmation = new WebServiceWrapper();
                                LoggerError.Fatal("An error occurred while SMSConfirmation , ErrorCode: " + smsConfirmation.ResponseMessage.ErrorCode + ", by: " + AgentClaimInfo.UserEmail());
                                //LOG

                                ViewBag.NewCustomerError = new LocalizedList<ErrorCodes, ErrorCodeList>().GetDisplayText(smsConfirmation.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);

                            }
                        }
                        else
                        {
                            //LOG
                            LoggerError.Fatal($"An error occurred while GetApartmentAddress , ErrorCode: BillingAddressResponse : {billingAddress.ResponseMessage.ErrorCode} SetupAddressResponse : {setupAddress.ResponseMessage.ErrorCode}, by: {AgentClaimInfo.UserEmail()}");
                            //LOG

                            ViewBag.NewCustomerError = MasterISS_Agent_Website_Localization.View.GenericErrorMessage;
                        }
                    }
                    ViewBag.InvalidCredentials = MasterISS_Agent_Website_Localization.View.InvalidCredentials;
                }
            }

            ViewBag.SubscriptionRegistrationType = SubscriptionRegistrationType(addCustomerViewModel.ExtraInfo.SubscriptionRegistrationTypeId ?? null);
            ViewBag.TckTypeList = TCKTypeList(addCustomerViewModel.IDCard.CardTypeId ?? null);
            ViewBag.SexexListByCorporative = SexesList(addCustomerViewModel.CorporateInfo.ExecutiveSexId ?? null);
            ViewBag.SexexListByIndividual = SexesList(addCustomerViewModel.Individual.SexId ?? null);
            ViewBag.CultureList = CultureList(addCustomerViewModel.GeneralInfo.Culture);
            ViewBag.ProfessionListByCorporative = ProfessionList(addCustomerViewModel.CorporateInfo.ExecutiveProfessionId ?? null);
            ViewBag.ProfessionListByIndividual = ProfessionList(addCustomerViewModel.Individual.ProfessionId ?? null);
            ViewBag.NationalityListByCorporative = NationalityList(addCustomerViewModel.CorporateInfo.ExecutiveNationalityId ?? null);
            ViewBag.NationalityListByIndividual = NationalityList(addCustomerViewModel.Individual.ProfessionId ?? null);
            ViewBag.PartnerTariffList = PartnerTariffList(null);
            ViewBag.CustomerTypeList = CustomerTypeList(addCustomerViewModel.GeneralInfo.CustomerTypeId ?? null);

            ViewBag.BillingPeriod = PaymentDayListByViewBag(addCustomerViewModel.SubscriptionInfo.PartnerTariffID ?? null, addCustomerViewModel.SubscriptionInfo.BillingPeriodId ?? null);

            //GeneralInfo=>Billing
            ViewBag.ProvincesByGeneralInfo = AddressInfo.ProvincesList(addCustomerViewModel.GeneralInfo.BillingAddress.ProvinceId ?? null);
            ViewBag.DistrictsByGeneralInfo = AddressInfo.DistrictList(addCustomerViewModel.GeneralInfo.BillingAddress.ProvinceId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.DistrictId ?? null);
            ViewBag.RuralRegionsByGeneralInfo = AddressInfo.RuralRegionsList(addCustomerViewModel.GeneralInfo.BillingAddress.DistrictId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.RuralRegionsId ?? null);
            ViewBag.NeigboorHoodsByGeneralInfo = AddressInfo.NeighborhoodList(addCustomerViewModel.GeneralInfo.BillingAddress.RuralRegionsId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.NeighborhoodId ?? null);
            ViewBag.StreetsByGeneralInfo = AddressInfo.StreetList(addCustomerViewModel.GeneralInfo.BillingAddress.NeighborhoodId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.StreetId ?? null);
            ViewBag.BuildingsByGeneralInfo = AddressInfo.BuildingList(addCustomerViewModel.GeneralInfo.BillingAddress.StreetId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.BuildingId ?? null);
            ViewBag.ApartmentsByGeneralInfo = AddressInfo.ApartmentList(addCustomerViewModel.GeneralInfo.BillingAddress.BuildingId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.ApartmentId ?? null);

            //SubscriptionInfo=>Setup
            ViewBag.ProvincesBySetup = AddressInfo.ProvincesList(addCustomerViewModel.SubscriptionInfo.SetupAddress.ProvinceId ?? null);
            ViewBag.DistrictsBySetup = AddressInfo.DistrictList(addCustomerViewModel.SubscriptionInfo.SetupAddress.ProvinceId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.DistrictId ?? null);
            ViewBag.RuralRegionsBySetup = AddressInfo.RuralRegionsList(addCustomerViewModel.SubscriptionInfo.SetupAddress.DistrictId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.RuralRegionsId ?? null);
            ViewBag.NeigboorHoodsBySetup = AddressInfo.NeighborhoodList(addCustomerViewModel.SubscriptionInfo.SetupAddress.RuralRegionsId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.NeighborhoodId ?? null);
            ViewBag.StreetsBySetup = AddressInfo.StreetList(addCustomerViewModel.SubscriptionInfo.SetupAddress.NeighborhoodId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.StreetId ?? null);
            ViewBag.BuildingsBySetup = AddressInfo.BuildingList(addCustomerViewModel.SubscriptionInfo.SetupAddress.StreetId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.BuildingId ?? null);
            ViewBag.ApartmentsBySetup = AddressInfo.ApartmentList(addCustomerViewModel.SubscriptionInfo.SetupAddress.BuildingId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.ApartmentId ?? null);

            //Individual=>ResidencyAddress
            ViewBag.ProvincesByIndividual = AddressInfo.ProvincesList(addCustomerViewModel.Individual.ResidencyAddress.ProvinceId ?? null);
            ViewBag.DistrictsByIndividual = AddressInfo.DistrictList(addCustomerViewModel.Individual.ResidencyAddress.ProvinceId ?? null, addCustomerViewModel.Individual.ResidencyAddress.DistrictId ?? null);
            ViewBag.RuralRegionsByIndividual = AddressInfo.RuralRegionsList(addCustomerViewModel.Individual.ResidencyAddress.DistrictId ?? null, addCustomerViewModel.Individual.ResidencyAddress.RuralRegionsId ?? null);
            ViewBag.NeigboorHoodsByIndividual = AddressInfo.NeighborhoodList(addCustomerViewModel.Individual.ResidencyAddress.RuralRegionsId ?? null, addCustomerViewModel.Individual.ResidencyAddress.NeighborhoodId ?? null);
            ViewBag.StreetsByIndividual = AddressInfo.StreetList(addCustomerViewModel.Individual.ResidencyAddress.NeighborhoodId ?? null, addCustomerViewModel.Individual.ResidencyAddress.StreetId ?? null);
            ViewBag.BuildingsByIndividual = AddressInfo.BuildingList(addCustomerViewModel.Individual.ResidencyAddress.StreetId ?? null, addCustomerViewModel.Individual.ResidencyAddress.BuildingId ?? null);
            ViewBag.ApartmentsByIndividual = AddressInfo.ApartmentList(addCustomerViewModel.Individual.ResidencyAddress.BuildingId ?? null, addCustomerViewModel.Individual.ResidencyAddress.ApartmentId ?? null);

            //CorporateInfo=>ExecutiveResidencyAddress
            ViewBag.ProvincesByCorporativeResidency = AddressInfo.ProvincesList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.ProvinceId ?? null);
            ViewBag.DistrictsByCorporativeResidency = AddressInfo.DistrictList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.ProvinceId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.DistrictId ?? null);
            ViewBag.RuralRegionsByCorporativeResidency = AddressInfo.RuralRegionsList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.DistrictId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.RuralRegionsId ?? null);
            ViewBag.NeigboorHoodsByCorporativeResidency = AddressInfo.NeighborhoodList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.RuralRegionsId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.NeighborhoodId ?? null);
            ViewBag.StreetsByCorporativeResidency = AddressInfo.StreetList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.NeighborhoodId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.StreetId ?? null);
            ViewBag.BuildingsByCorporativeResidency = AddressInfo.BuildingList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.StreetId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.BuildingId ?? null);
            ViewBag.ApartmentsByCorporativeResidency = AddressInfo.ApartmentList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.BuildingId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.ApartmentId ?? null);

            //CorporateInfo=>CompanyAddress
            ViewBag.ProvincesByCorporativeCompany = AddressInfo.ProvincesList(addCustomerViewModel.CorporateInfo.CompanyAddress.ProvinceId ?? null);
            ViewBag.DistrictsByCorporativeCompany = AddressInfo.DistrictList(addCustomerViewModel.CorporateInfo.CompanyAddress.ProvinceId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.DistrictId ?? null);
            ViewBag.RuralRegionsByCorporativeCompany = AddressInfo.RuralRegionsList(addCustomerViewModel.CorporateInfo.CompanyAddress.DistrictId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.RuralRegionsId ?? null);
            ViewBag.NeigboorHoodsByCorporativeCompany = AddressInfo.NeighborhoodList(addCustomerViewModel.CorporateInfo.CompanyAddress.RuralRegionsId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.NeighborhoodId ?? null);
            ViewBag.StreetsByCorporativeCompany = AddressInfo.StreetList(addCustomerViewModel.CorporateInfo.CompanyAddress.NeighborhoodId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.StreetId ?? null);
            ViewBag.BuildingsByCorporativeCompany = AddressInfo.BuildingList(addCustomerViewModel.CorporateInfo.CompanyAddress.StreetId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.BuildingId ?? null);
            ViewBag.ApartmentsByCorporativeCompany = AddressInfo.ApartmentList(addCustomerViewModel.CorporateInfo.CompanyAddress.BuildingId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.ApartmentId ?? null);

            ViewBag.DayList = DayList(addCustomerViewModel.IDCard.SelectedBirthDay ?? null);
            ViewBag.MonthList = Monthlist(addCustomerViewModel.IDCard.SelectedBirthMonth ?? null);
            ViewBag.YearList = YearList(addCustomerViewModel.IDCard.SelectedBirthYear ?? null, false);

            ViewBag.HavePSTN = HavePSTN(addCustomerViewModel.ExtraInfo.HavePSTNId);


            ViewBag.DayListByIssueDate = DayList(addCustomerViewModel.IDCard.TCBirthCertificate?.DateOfIssueDay ?? null);
            ViewBag.MonthListByIssueDate = Monthlist(addCustomerViewModel.IDCard.TCBirthCertificate?.DateOfIssueMonth ?? null);
            ViewBag.YearListByIssueDate = YearList(addCustomerViewModel.IDCard.TCBirthCertificate?.DateOfIssueYear ?? null, false);

            ViewBag.DayListByExpiryDate = DayList(addCustomerViewModel.IDCard.TCIDCardWithChip?.ExpiryDay ?? null);
            ViewBag.MonthListByExpiryDate = Monthlist(addCustomerViewModel.IDCard.TCIDCardWithChip?.ExpiryMonth ?? null);
            ViewBag.YearListByExpiryDate = YearList(addCustomerViewModel.IDCard.TCIDCardWithChip?.ExpiryYear ?? null, true);

            return View(addCustomerViewModel);
        }

        public ActionResult SmsConfirmation()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SmsConfirmation(string inputCode)
        {
            var serviceCode = Session["SMSCode"]?.ToString();
            if (!string.IsNullOrEmpty(serviceCode))
            {
                if (Convert.ToInt32(Session["Counter"]) < 3)
                {
                    if (inputCode == serviceCode)
                    {
                        var wrapper = new WebServiceWrapper();

                        var customerApplicationInfo = Session["CustomerApplicationInfo"] as NewCustomerViewModel;

                        var response = wrapper.NewCustomerRegister(customerApplicationInfo);

                        if (response.ResponseMessage.ErrorCode == 0)
                        {
                            Session.Remove("CustomerApplicationInfo");
                            Session.Remove("Counter");
                            Session.Remove("SMSCode");

                            //LOG
                            Logger.Info("Added Customer: " + customerApplicationInfo.IDCard.FirstName + customerApplicationInfo.IDCard.LastName + ", by: " + AgentClaimInfo.UserEmail());
                            //LOG

                            return RedirectToAction("NewCustomer", "Customer");

                            //return Json(new { status = "Success", message = MasterISS_Agent_Website_Localization.View.Successful }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            Session.Remove("CustomerApplicationInfo");
                            Session.Remove("Counter");
                            Session.Remove("SMSCode");

                            //LOG
                            LoggerError.Fatal($"An error occurred while NewCustomerRegister, ErrorCode:  {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage}, NameValuePair :{string.Join(",", response.NewCustomerRegisterResponse)}  by: {AgentClaimInfo.UserEmail()}");
                            //LOG

                            ViewBag.ErrorMessage = new LocalizedList<ErrorCodes, MasterISS_Agent_Website_Localization.Generic.ErrorCodeList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);

                            return View();
                            //return Json(new { status = "Failed", ErrorMessage = new LocalizedList<ErrorCodes, MasterISS_Agent_Website_Localization.Generic.ErrorCodeList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture) }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        Session["Counter"] = Convert.ToInt32(Session["Counter"]) + 1;

                        ViewBag.ErrorMessage = MasterISS_Agent_Website_Localization.Customer.NewCustomerView.WrongPassword;

                        return View();

                        //return Json(new { status = "Failed", ErrorMessage = MasterISS_Agent_Website_Localization.Customer.NewCustomerView.WrongPassword }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    Session.Remove("Counter");
                    Session.Remove("CustomerApplicationInfo");
                    Session.Remove("SMSCode");

                    TempData["SMSConfirmationError"] = MasterISS_Agent_Website_Localization.Customer.NewCustomerView.SMSCode3TimesIncorrectlyError;

                    return RedirectToAction("NewCustomer", "Customer");

                    //return Json(new { status = "FailedAndRedirect", ErrorMessage = MasterISS_Agent_Website_Localization.Customer.NewCustomerView.SMSCode3TimesIncorrectlyError }, JsonRequestBehavior.AllowGet);

                }
            }
            Session.Remove("CustomerApplicationInfo");

            return RedirectToAction("Successful", "Customer");

            //return Json(new { status = "FailedAndRedirect", ErrorMessage = MasterISS_Agent_Website_Localization.View.Successful }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAgentSubscriptions(int page = 1, int pageSize = 20)
        {
            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetAgentSubscriptions();

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var list = response.AgentSubscriptionList.Select(asl => new ListAgentSubscriptionViewModel
                {
                    CustomerName = asl.DisplayName,
                    CustomerState = asl.CustomerState.Name,
                    MembershipDate = Convert.ToDateTime(asl.MembershipDate),
                    SubscriberNo = asl.SubscriberNo,
                    SubscriptionId = asl.ID
                });

                var totalCount = list.Count();

                var pagedList = new StaticPagedList<ListAgentSubscriptionViewModel>(list.Skip((page - 1) * pageSize).Take(pageSize), page, pageSize, totalCount);

                return View(pagedList);
            }
            return View(Enumerable.Empty<ListAgentSubscriptionViewModel>());
        }

        [HttpPost]
        public ActionResult PaymentDayList(long? id)
        {
            var wrapper = new WebServiceWrapper();

            var paymentDayListResponse = wrapper.GetPaymentDays((long)id);

            if (paymentDayListResponse.ResponseMessage.ErrorCode == 0)
            {
                var list = paymentDayListResponse.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }).ToArray();

                return Json(new { list = list }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { errorMessage = new LocalizedList<ErrorCodes, MasterISS_Agent_Website_Localization.Generic.ErrorCodeList>().GetDisplayText(paymentDayListResponse.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DateValidation(int? year, int? month, int? day)
        {
            if (year != null && month != null && day != null)
            {
                var validation = DateTimeValidation.TryParseDate(year.Value, month.Value, day.Value, null);
                if (validation == null)
                {
                    return Json(new { status = "Failed", ErrorMessage = MasterISS_Agent_Website_Localization.Customer.NewCustomerView.DateFormatIsNotCorrect }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = "Success" }, JsonRequestBehavior.AllowGet);
        }

        private SelectList PaymentDayListByViewBag(long? tariffId, long? selectedValue)
        {
            if (tariffId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var paymentDayListResponse = wrapper.GetPaymentDays((long)tariffId);

                if (paymentDayListResponse.ResponseMessage.ErrorCode == 0)
                {
                    var list = new SelectList(paymentDayListResponse.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }), "Value", "Name", selectedValue);
                    return list;
                }
            }
            return new SelectList("");
        }
        private string ConvertCultureDisplayName(string convertedCulture)
        {
            var localizedList = new LocalizedList<MasterISS_Agent_Website_Enums.Enums.CultureList, MasterISS_Agent_Website_Localization.Generic.CultureList>();
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
        private bool IsCustomerTypeIndividual(int customerTypeId)
        {
            if (customerTypeId == (int)CustomerType.Individual)
            {
                return true;
            }
            return false;
        }
        private void RemoveModel(string removedModel)
        {
            var removedModelKeys = ModelState.Keys.Where(k => k.Contains(removedModel)).ToArray();
            foreach (var key in removedModelKeys)
            {
                ModelState.Remove(key);
            }
        }
        private void ChangeAddressInfo(AddressInfoViewModel changingViewModel, AddressInfoViewModel changerViewModel)
        {
            changingViewModel.ApartmentId = changerViewModel.ApartmentId;
            changingViewModel.BuildingId = changerViewModel.BuildingId;
            changingViewModel.DistrictId = changerViewModel.DistrictId;
            changingViewModel.NeighborhoodId = changerViewModel.NeighborhoodId;
            changingViewModel.ProvinceId = changerViewModel.ProvinceId;
            changingViewModel.RuralRegionsId = changerViewModel.RuralRegionsId;
            changingViewModel.StreetId = changerViewModel.StreetId;
            changingViewModel.PostalCode = changerViewModel.PostalCode;
            changingViewModel.Floor = changerViewModel.Floor;
        }
        private void IdCardValidationAndRemoveModelState(int cardTypeId, IDCardViewModel IDCard)
        {
            if (cardTypeId == (int)CardType.TCBirthCertificate)
            {
                IDCard.TCIDCardWithChip = null;
                RemoveModel("IDCard.TCIDCardWithChip");
                DateTime issueDate = new DateTime(IDCard.TCBirthCertificate.DateOfIssueYear.Value, IDCard.TCBirthCertificate.DateOfIssueMonth.Value, IDCard.TCBirthCertificate.DateOfIssueDay.Value);
                IDCard.TCBirthCertificate.DateOfIssue = issueDate.ToString("dd.MM.yyyy");

                ViewBag.DayListByIssueDate = DayList(IDCard.TCBirthCertificate.DateOfIssueDay ?? null);
                ViewBag.MonthListByIssueDate = Monthlist(IDCard.TCBirthCertificate.DateOfIssueMonth ?? null);
                ViewBag.YearListByIssueDate = YearList(IDCard.TCBirthCertificate.DateOfIssueYear ?? null, false);

                ViewBag.DayListByExpiryDate = DayList(null);
                ViewBag.MonthListByExpiryDate = Monthlist(null);
                ViewBag.YearListByExpiryDate = YearList(null, true);
            }
            else
            {
                ViewBag.DayListByExpiryDate = DayList(IDCard.TCIDCardWithChip.ExpiryDay ?? null);
                ViewBag.MonthListByExpiryDate = Monthlist(IDCard.TCIDCardWithChip.ExpiryMonth ?? null);
                ViewBag.YearListByExpiryDate = YearList(IDCard.TCIDCardWithChip.ExpiryYear ?? null, true);

                ViewBag.DayListByIssueDate = DayList(null);
                ViewBag.MonthListByIssueDate = Monthlist(null);
                ViewBag.YearListByIssueDate = YearList(null, false);

                IDCard.TCBirthCertificate = null;
                RemoveModel("IDCard.TCBirthCertificate");
                DateTime expiryDate = new DateTime(IDCard.TCIDCardWithChip.ExpiryYear.Value, IDCard.TCIDCardWithChip.ExpiryMonth.Value, IDCard.TCIDCardWithChip.ExpiryDay.Value);
                IDCard.TCIDCardWithChip.ExpiryDate = expiryDate.ToString("dd.MM.yyyy");
            }
        }
        private string ReplacePhoneNo(string phoneNo)
        {
            if (!string.IsNullOrEmpty(phoneNo))
            {
                var replacedNumber = phoneNo.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

                return replacedNumber;
            }
            return phoneNo;
        }
        private NewCustomerAddressInfoRequest NewCustomerAddressInfoRequest(AddressDetailsResponse addressDetailsResponse)
        {
            var request = new NewCustomerAddressInfoRequest
            {
                AddressText = addressDetailsResponse.AddressText,
                AddressNo = addressDetailsResponse.AddressNo,
                ApartmentNo = addressDetailsResponse.ApartmentNo,
                ApartmentId = addressDetailsResponse.ApartmentID,
                DistrictId = addressDetailsResponse.DistrictID,
                DistrictName = addressDetailsResponse.DistrictName,
                DoorId = addressDetailsResponse.DoorID,
                DoorNo = addressDetailsResponse.DoorNo,
                NeighbourhoodID = addressDetailsResponse.NeighbourhoodID,
                NeighbourhoodName = addressDetailsResponse.NeighbourhoodName,
                ProvinceId = addressDetailsResponse.ProvinceID,
                ProvinceName = addressDetailsResponse.ProvinceName,
                RuralCode = addressDetailsResponse.RuralCode,
                StreetId = addressDetailsResponse.StreetID,
                StreetName = addressDetailsResponse.StreetName,
            };
            return request;
        }
        private SelectList HavePSTN(int? selectedValue)
        {
            var havePstnList = new LocalizedList<MasterISS_Agent_Website_Enums.Enums.HavePSTN, MasterISS_Agent_Website_Localization.Generic.HavePSTN>().GetList(CultureInfo.CurrentCulture);

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