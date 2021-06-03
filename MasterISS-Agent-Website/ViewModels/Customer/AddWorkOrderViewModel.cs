using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Customer
{
    public class AddWorkOrderViewModel
    {
        public string Description { get; set; }

        public bool HasModem { get; set; }

        public RadiusR.DB.Enums.CustomerSetup.XDSLTypes XDSLTypes { get; set; }

        public RadiusR.DB.Enums.CustomerSetup.TaskTypes TaskTypes { get; set; }

        public string ModemName { get; set; }

        public int SetupUserId { get; set; }

        public long SubscriptionId { get; set; }

        public string  UserEmail { get; set; }
    }
}