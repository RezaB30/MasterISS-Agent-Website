using MasterISS_Agent_Website.ViewModels.Customer;
using MasterISS_Agent_Website_Localization.Customer;
using MasterISS_Agent_Website_Localization.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Setup
{
    public class GetTaskDetailsViewModel
    {
        [Display(ResourceType = typeof(SetupModel), Name = "TaskNo")]
        public long TaskNo { get; set; }
        [Display(ResourceType = typeof(SetupModel), Name = "XDSLNo")]
        public string XDSLNo { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "SetupAddress")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "Username")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "Password")]
        public string Password { get; set; }

        public string BBK { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "CustomerPhoneNo")]
        public string CustomerPhoneNo { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "CustomerType")]
        public string CustomerType { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "Details")]
        public string Details { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "HasModem")]
        [UIHint("BoolFormat")]
        public bool HasModem { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "ModemName")]
        public string ModemName { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "XDSLType")]
        public string XDSLType { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "PSTN")]
        public string PSTN { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "TaskUpdates")]
        public IEnumerable<TaskUpdatesDetailListViewModel> TaskUpdates { get; set; }


    }
}