using MasterISS_Agent_Website_Localization.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Customer
{
    public class FilterAgentSubscriptionsViewModel
    {
        [Display(Name = "CustomerNameAndSubsNo", ResourceType = typeof(CustomerModel))]
        public string CustomerName { get; set; }
    }
}