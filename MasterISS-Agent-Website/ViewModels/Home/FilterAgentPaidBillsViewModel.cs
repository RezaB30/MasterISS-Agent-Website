using MasterISS_Agent_Website_Localization;
using MasterISS_Agent_Website_Localization.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Home
{
    public class FilterAgentPaidBillsViewModel
    {
        [Display(Name = "CustomerNameAndSubsNo", ResourceType = typeof(CustomerModel))]
        public string CustomerName { get; set; }

        [RegularExpression("^(3[01]|[12][0-9]|0[1-9]).(1[0-2]|0[1-9]).[0-9]{4} (2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "DateFormatErrorReservationDate")]
        [Display(Name = "PaymentDayStartDate", ResourceType = typeof(CustomerModel))]
        public string PaymentDayStartDate { get; set; }

        [Display(Name = "PaymentDayEndDate", ResourceType = typeof(CustomerModel))]
        [RegularExpression("^(3[01]|[12][0-9]|0[1-9]).(1[0-2]|0[1-9]).[0-9]{4} (2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "DateFormatErrorReservationDate")]
        public string PaymentDayEndDate { get; set; }
    }
}