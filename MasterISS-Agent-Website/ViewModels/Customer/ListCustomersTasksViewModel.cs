using MasterISS_Agent_Website_WebServices.AgentWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static MasterISS_Agent_Website_WebServices.AgentWebService.CustomerSetupTaskResponse;

namespace MasterISS_Agent_Website.ViewModels.Customer
{
    public class ListCustomersTasksViewModel
    {
        public long Id { get; set; }

        public string AllowenceState { get; set; }

        public DateTime CompletionDate { get; set; }

        public string Details { get; set; }

        public string SetupUserName { get; set; }

        public long SubscriptionID { get; set; }

        public DateTime TaskIssueDate { get; set; }

        public string TaskStatus { get; set; }

        public string TaskType { get; set; }
    }
}