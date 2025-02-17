﻿using MasterISS_Agent_Website_Localization.Home;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Home
{
    public class CustomerBillCollectionViewModel
    {
        [Display(Name = "SubscriberName", ResourceType = typeof(HomeModel))]
        public string SubscriberName { get; set; }
        public IEnumerable<BillsViewModel> Bills { get; set; }
        public IEnumerable<GenericBillInfoViewModel> PrePaidSubscriberInfos { get; set; }
    }

    public class GenericBillInfoViewModel
    {
        [Display(Name = "Amount", ResourceType = typeof(HomeModel))]
        public decimal Cost { get; set; }

        [Display(Name = "SubscriberNo", ResourceType = typeof(HomeModel))]
        public string SubscriptionNo { get; set; }

        [Display(Name = "BillName", ResourceType = typeof(HomeModel))]
        public string BillName { get; set; }

    }

    public class BillsViewModel
    {
        public long BillID { get; set; }

        [Display(Name = "IssueDate", ResourceType = typeof(HomeModel))]
        [UIHint("DateTimeFormat")]
        public string IssueDate { get; set; }

        [Display(Name = "DueDate", ResourceType = typeof(HomeModel))]
        [UIHint("DateTimeFormat")]
        public string DueDate { get; set; }
        public GenericBillInfoViewModel GenericBillInfoViewModel { get; set; }


    }
}