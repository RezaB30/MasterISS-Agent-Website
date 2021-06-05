using MasterISS_Agent_Website_WebServices.AgentWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Customer
{
    public class ListCustomersTaskDetailsViewModel
    {
        public decimal? Allowence { get; set; }

        public NameValuePair TaskUpdates { get; set; }

        public bool HasModem { get; set; }

        public string ModemName { get; set; }

        public long SubscriptionID { get; set; }

        public string CustomerName { get; set; }
    }
}