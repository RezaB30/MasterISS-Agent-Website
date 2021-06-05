using MasterISS_Agent_Website_Localization;
using MasterISS_Agent_Website_Localization.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Customer
{
    public class AddWorkOrderViewModel
    {
        [Display(Name = "Description", ResourceType = typeof(CustomerModel))]
        public string Description { get; set; }

        [Display(Name = "HasModem", ResourceType = typeof(CustomerModel))]
        public bool HasModem { get; set; }

        [Display(Name = "XDSLTypes", ResourceType = typeof(CustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public RadiusR.DB.Enums.CustomerSetup.XDSLTypes XDSLTypes { get; set; }

        [Display(Name = "TaskTypes", ResourceType = typeof(CustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public RadiusR.DB.Enums.CustomerSetup.TaskTypes TaskTypes { get; set; }

        [Display(Name = "ModemName", ResourceType = typeof(CustomerModel))]
        public string ModemName { get; set; }

        [Display(Name = "SetupUserId", ResourceType = typeof(CustomerModel))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public int SetupUserId { get; set; }

        public string SubscriberName { get; set; }

        [Required]
        public long SubscriptionId { get; set; }

    }
}