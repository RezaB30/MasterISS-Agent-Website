using MasterISS_Agent_Website_Localization.Customer;
using MasterISS_Agent_Website_WebServices.AgentWebService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static MasterISS_Agent_Website_WebServices.AgentWebService.CustomerSetupTaskResponse;

namespace MasterISS_Agent_Website.ViewModels.Customer
{
    public class ListCustomersTaskDetailsViewModel
    {
        [Display(ResourceType = typeof(CustomerModel), Name = "Allowence")]
        public decimal? Allowence { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "TaskUpdates")]
        public IEnumerable<TaskUpdatesDetailListViewModel> TaskUpdates { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "HasModem")]
        [UIHint("BoolFormat")]
        public bool HasModem { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "ModemName")]
        public string ModemName { get; set; }

        public long SubscriptionID { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "CustomerName")]
        public string CustomerName { get; set; }
    }

    public class TaskUpdatesDetailListViewModel
    {
        [Display(ResourceType = typeof(CustomerModel), Name = "FaultCodes")]
        public string FaultCodes { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "CreationDate")]
        [UIHint("DateTimeConverted")]
        public DateTime CreationDate { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "Description")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "ReservationDate")]
        [UIHint("DateTimeConverted")]
        public DateTime ReservationDate { get; set; }
    }
}