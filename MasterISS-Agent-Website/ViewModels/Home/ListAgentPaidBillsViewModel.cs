using MasterISS_Agent_Website_Localization.Home;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Home
{
    public class ListAgentPaidBillsViewModel
    {
        [Display(Name = "SubscriberNo", ResourceType = typeof(HomeModel))]
        public string SubscriberNo { get; set; }

        [Display(Name = "SubscriberName", ResourceType = typeof(HomeModel))]
        public string SubscriberName { get; set; }
        public CustomerBillIdAndCost[] CustomerBillIdsAndCosts { get; set; }
    }
}