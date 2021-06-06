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
    public class ListCustomersTasksViewModel
    {
        [Display(ResourceType = typeof(CustomerModel), Name = "CustomerName")]
        public string CustomerName { get; set; }

        public long Id { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "AllowenceState")]
        public string AllowenceState { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "CompletionDate")]
        [UIHint("DateTimeConverted")]
        public DateTime CompletionDate { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "Description")]
        public string Details { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "SetupUserId")]
        public string SetupUserName { get; set; }

        public long SubscriptionID { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "TaskIssueDate")]
        [UIHint("DateTimeConverted")]
        public DateTime TaskIssueDate { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "TaskStatus")]
        public string TaskStatus { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "TaskType")]
        public string TaskType { get; set; }
    }
}