using MasterISS_Agent_Website_Localization.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Report
{
    public class ListAgentAllowenceViewModel
    {
        [Display(Name = "AllowanceAmount", ResourceType = typeof(ReportModel))]
        public decimal AllowanceAmount { get; set; }
        
        public long CollectionId { get; set; }

        [Display(Name = "CompanyTitle", ResourceType = typeof(ReportModel))]
        public string CompanyTitle { get; set; }


        [Display(Name = "CreationDate", ResourceType = typeof(ReportModel))]
        [UIHint("DateTimeConverted")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "PaymentDate", ResourceType = typeof(ReportModel))]
        [UIHint("DateTimeConverted")]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "PaymentStatus", ResourceType = typeof(ReportModel))]
        [UIHint("PaymentStatusFormat")]
        public bool PaymentStatus { get; set; }
    }
}