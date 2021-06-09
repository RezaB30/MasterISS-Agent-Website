using MasterISS_Agent_Website_Localization.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Setup
{
    public class ListTasksViewModel
    {
        [Display(ResourceType = typeof(SetupModel), Name = "CustomerName")]
        public string CustomerName { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "TaskIssueDate")]
        [UIHint("DateTimeConverted")]
        public DateTime TaskIssueDate { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "TaskType")]
        public string TaskType { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "TaskStatus")]
        public string TaskStatus { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        [UIHint("DateTimeConverted")]
        [Display(ResourceType = typeof(SetupModel), Name = "ReservationDate")]
        public DateTime ReservationDate { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "City")]
        public string City { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "Province")]
        public string Province { get; set; }

        public long TaskNo { get; set; }
    }
}