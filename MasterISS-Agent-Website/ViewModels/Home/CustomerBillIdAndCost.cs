using MasterISS_Agent_Website_Localization.Home;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Home
{
    public class CustomerBillIdAndCost
    {
        public long BillId { get; set; }
        [Display(Name = "Amount", ResourceType = typeof(HomeModel))]
        public decimal Cost { get; set; }

    }
}