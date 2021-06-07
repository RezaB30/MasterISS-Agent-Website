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
        public long BillId { get; set; }
        [Display(Name = "Amount", ResourceType = typeof(HomeModel))]
        public decimal Cost { get; set; }

        //[Display(Name = "BillName", ResourceType = typeof(HomeModel))]
        //public string BillName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(HomeModel))]
        public string Description { get; set; }

        [Display(Name = "IssueDate", ResourceType = typeof(HomeModel))]
        [UIHint("DateTimeConverted")]
        public DateTime IssueDate { get; set; }

        [Display(Name = "PayDate", ResourceType = typeof(HomeModel))]
        [UIHint("DateTimeConverted")]
        public DateTime PayDate { get; set; }
    }
}