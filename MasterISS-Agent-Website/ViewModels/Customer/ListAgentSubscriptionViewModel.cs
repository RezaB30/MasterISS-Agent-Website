using MasterISS_Agent_Website_Localization.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Customer
{
    public class ListAgentSubscriptionViewModel
    {
        [Display(Name = "CustomerState", ResourceType = typeof(CustomerModel))]
        public string CustomerState { get; set; }

        [Display(Name = "CustomerName", ResourceType = typeof(CustomerModel))]
        public string CustomerName { get; set; }

        public long SubscriptionId { get; set; }

        [Display(Name = "MembershipDate", ResourceType = typeof(CustomerModel))]
        public DateTime MembershipDate { get; set; }

        [Display(Name = "SubscriberNo", ResourceType = typeof(CustomerModel))]
        public string SubscriberNo { get; set; }
    }
}