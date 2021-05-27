using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MasterISS_Agent_Website_Localization;
using MasterISS_Agent_Website_Localization.Customer;
using MasterISS_Agent_Website_Localization.AddressInfo;

namespace MasterISS_Agent_Website.ViewModels.Customer
{
    public class NewCustomerViewModel
    {
        public CorporateInfoViewModel CorporateInfo { get; set; }
        public GeneralInfoViewModel GeneralInfo { get; set; }
        public IDCardViewModel IDCard { get; set; }
        public IndividualViewModel Individual { get; set; }
        public SubscriptionInfoViewModel SubscriptionInfo { get; set; }
        public ExtraInfoViewModel ExtraInfo { get; set; }

    }

    public class ExtraInfoViewModel
    {
        [Display(Name = "RegistrationType", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public int? SubscriptionRegistrationTypeId { get; set; }

        [Display(Name = "PSTNNo", ResourceType = typeof(NewCustomerModel))]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "OnlyNumeric")]
        public string PSTN { get; set; }

        [Display(Name = "HavePSTNId", ResourceType = typeof(NewCustomerModel))]
        public int? HavePSTNId { get; set; }


        [Display(Name = "XDSLNo", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [MaxLength(10, ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "XDSLNoValid")]
        [MinLength(10, ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "XDSLNoValid")]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "OnlyNumeric")]
        public string XDSLNo { get; set; }
    }

    public class SubscriptionInfoViewModel
    {
        [Display(Name = "BillingPeriodId", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public int? BillingPeriodId { get; set; }

        [Display(Name = "PartnerTariffID", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public int? PartnerTariffID { get; set; }

        public AddressInfoViewModel SetupAddress { get; set; }
    }

    public class AddressInfoViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "ProvinceId", ResourceType = typeof(AddressInfoModel))]
        public long? ProvinceId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "DistrictId", ResourceType = typeof(AddressInfoModel))]
        public long? DistrictId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "RuralRegionsId", ResourceType = typeof(AddressInfoModel))]
        public long? RuralRegionsId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "NeighborhoodId", ResourceType = typeof(AddressInfoModel))]
        public long? NeighborhoodId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "StreetId", ResourceType = typeof(AddressInfoModel))]
        public long? StreetId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "BuildingId", ResourceType = typeof(AddressInfoModel))]
        public long? BuildingId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "ApartmentId", ResourceType = typeof(AddressInfoModel))]
        public long? ApartmentId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "PostalCode", ResourceType = typeof(AddressInfoModel))]
        public int? PostalCode { get; set; }

        [Display(Name = "Floor", ResourceType = typeof(AddressInfoModel))]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public string Floor { get; set; }
        public NewCustomerAddressInfoRequest NewCustomerAddressInfoRequest { get; set; }

    }

    public class NewCustomerAddressInfoRequest
    {
        public long AddressNo { get; set; }

        public string AddressText { get; set; }

        public long ApartmentId { get; set; }

        public string ApartmentNo { get; set; }

        public long DistrictId { get; set; }

        public string DistrictName { get; set; }

        public long DoorId { get; set; }

        public string DoorNo { get; set; }

        public long NeighbourhoodID { get; set; }

        public string NeighbourhoodName { get; set; }

        public long ProvinceId { get; set; }

        public string ProvinceName { get; set; }

        public long RuralCode { get; set; }

        public long StreetId { get; set; }

        public string StreetName { get; set; }
    }

    public class IndividualViewModel
    {
        [Display(Name = "BirthPlace", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string BirthPlace { get; set; }

        [Display(Name = "FathersName", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string FathersName { get; set; }

        [Display(Name = "MothersMaidenName", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string MothersMaidenName { get; set; }

        [Display(Name = "MothersName", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string MothersName { get; set; }

        [Display(Name = "NationalityId", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public int NationalityId { get; set; }

        [Display(Name = "ProfessionId", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public int? ProfessionId { get; set; }

        public AddressInfoViewModel ResidencyAddress { get; set; }

        [Display(Name = "SexId", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public int? SexId { get; set; }

        [Display(Name = "CustomerResidanceAddressSameSetupAddress", ResourceType = typeof(NewCustomerModel))]
        public bool SameSetupAddressByIndividual { get; set; }
    }

    public class IDCardViewModel
    {
        public TCIDCardWithChip TCIDCardWithChip { get; set; }
        public TCBirthCertificate TCBirthCertificate { get; set; }

        public string BirthDate { get; set; }

        [Display(Name = "CardTypeId", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public int? CardTypeId { get; set; }

        [Display(Name = "FirstName", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "LastName", ResourceType = typeof(NewCustomerModel))]
        public string LastName { get; set; }

        [Display(Name = "PassportNo", ResourceType = typeof(NewCustomerModel))]
        public string PassportNo { get; set; }

        [Display(Name = "SerialNo", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string SerialNo { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [MaxLength(11, ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "MaxTCKValid")]
        [MinLength(11, ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "MinTCKValid")]
        [Display(Name = "TCKNo", ResourceType = typeof(NewCustomerModel))]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "OnlyNumeric")]
        public string TCKNo { get; set; }

        [Display(Name = "SelectedBirthDay", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public int? SelectedBirthDay { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "SelectedBirthMonth", ResourceType = typeof(NewCustomerModel))]
        public int? SelectedBirthMonth { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "SelectedBirthYear", ResourceType = typeof(NewCustomerModel))]
        public int? SelectedBirthYear { get; set; }
    }

    public class TCBirthCertificate
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "VolumeNo", ResourceType = typeof(NewCustomerModel))]
        public string VolumeNo { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "Neighbourhood", ResourceType = typeof(NewCustomerModel))]
        public string Neighbourhood { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "PageNo", ResourceType = typeof(NewCustomerModel))]
        public string PageNo { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "DistrictId", ResourceType = typeof(NewCustomerModel))]
        public string District { get; set; }

        public string DateOfIssue { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "DateOfIssueDay", ResourceType = typeof(NewCustomerModel))]
        public int? DateOfIssueDay { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "DateOfIssueMonth", ResourceType = typeof(NewCustomerModel))]
        public int? DateOfIssueMonth { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "DateOfIssueYear", ResourceType = typeof(NewCustomerModel))]
        public int? DateOfIssueYear { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "PlaceOfIssue", ResourceType = typeof(NewCustomerModel))]
        public string PlaceOfIssue { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "ProvinceId", ResourceType = typeof(NewCustomerModel))]
        public string Province { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "RowNo", ResourceType = typeof(NewCustomerModel))]
        public string RowNo { get; set; }
    }

    public class TCIDCardWithChip
    {
        public string ExpiryDate { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "ExpiryDay", ResourceType = typeof(NewCustomerModel))]
        public int? ExpiryDay { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "ExpiryMonth", ResourceType = typeof(NewCustomerModel))]
        public int? ExpiryMonth { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "ExpiryYear", ResourceType = typeof(NewCustomerModel))]
        public int? ExpiryYear { get; set; }
    }

    public class GeneralInfoViewModel
    {
        public GeneralInfoViewModel()
        {
            OtherPhoneNos = new List<string>();
        }

        public AddressInfoViewModel BillingAddress { get; set; }

        [Display(Name = "ContactPhoneNo", ResourceType = typeof(NewCustomerModel))]//Burada Kaldın
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        //[RegularExpression(@"^((\d{3})(\d{3})(\d{2})(\d{2}))$", ErrorMessageResourceName = "ValidPhoneNumber", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string ContactPhoneNo { get; set; }

        [Display(Name = "CustomerTypeId", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public int? CustomerTypeId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [MaxLength(300, ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "EmailMaxLenght")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "EmailFormat")]
        [Display(Name = "Email", ResourceType = typeof(NewCustomerModel))]
        public string Email { get; set; }

        [Display(Name = "OtherPhoneNos", ResourceType = typeof(NewCustomerModel))]
        [OtherPhoneNoValidation(ErrorMessageResourceName = "ValidPhoneNumber", ErrorMessageResourceType = typeof(Validation))]
        public List<string> OtherPhoneNos { get; set; }

        [Display(Name = "CultureCustomer", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string Culture { get; set; }

        [Display(Name = "BillingAddressSameSetupAddress", ResourceType = typeof(NewCustomerModel))]
        public bool SameSetupAddressByBilling { get; set; }
    }

    public class CorporateInfoViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "CentralSystemNo", ResourceType = typeof(NewCustomerModel))]
        [MaxLength(16, ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "CentralSystemNoValid")]
        [MinLength(16, ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "CentralSystemNoValid")]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "OnlyNumeric")]
        public string CentralSystemNo { get; set; }

        public AddressInfoViewModel CompanyAddress { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "ExecutiveBirthPlace", ResourceType = typeof(NewCustomerModel))]
        public string ExecutiveBirthPlace { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "TaxNo", ResourceType = typeof(NewCustomerModel))]
        [MaxLength(10, ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "TaxNoValid")]
        [MinLength(10, ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "TaxNoValid")]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "OnlyNumeric")]
        public string TaxNo { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "TaxOffice", ResourceType = typeof(NewCustomerModel))]
        public string TaxOffice { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "ExecutiveFathersName", ResourceType = typeof(NewCustomerModel))]
        public string ExecutiveFathersName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "ExecutiveMothersMaidenName", ResourceType = typeof(NewCustomerModel))]
        public string ExecutiveMothersMaidenName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "ExecutiveMothersName", ResourceType = typeof(NewCustomerModel))]
        public string ExecutiveMothersName { get; set; }

        [Display(Name = "ExecutiveNationalityId", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public int? ExecutiveNationalityId { get; set; }

        [Display(Name = "ExecutiveProfessionId", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public int? ExecutiveProfessionId { get; set; }

        public AddressInfoViewModel ExecutiveResidencyAddress { get; set; }

        [Display(Name = "ExecutiveSexId", ResourceType = typeof(NewCustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public short? ExecutiveSexId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "Title", ResourceType = typeof(NewCustomerModel))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        [Display(Name = "TradeRegistrationNo", ResourceType = typeof(NewCustomerModel))]
        [MaxLength(6, ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "TradeRegistrationNoValid")]
        [MinLength(6, ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "TradeRegistrationNoValid")]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "OnlyNumeric")]
        public string TradeRegistrationNo { get; set; }

        [Display(Name = "CorporativeCompanyAddressSameSetupAddress", ResourceType = typeof(NewCustomerModel))]
        public bool SameSetupAddressByCorporativeCompanyAddress { get; set; }

        [Display(Name = "CorporativeResidencyAddressSameSetupAddress", ResourceType = typeof(NewCustomerModel))]
        public bool SameSetupAddressByCorporativeResidencyAddress { get; set; }
    }
}