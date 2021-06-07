using MasterISS_Agent_Website_Localization;
using MasterISS_Agent_Website_Localization.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Setup
{
    public class GetTaskListViewModel
    {
        [RegularExpression("^(3[01]|[12][0-9]|0[1-9]).(1[0-2]|0[1-9]).[0-9]{4} (2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "DateFormatErrorReservationDate")]
        [Display(ResourceType = typeof(SetupModel), Name = "StartDate")]
        public string StartDate { get; set; }

        [RegularExpression("^(3[01]|[12][0-9]|0[1-9]).(1[0-2]|0[1-9]).[0-9]{4} (2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "DateFormatErrorReservationDate")]
        [Display(ResourceType = typeof(SetupModel), Name = "EndDate")]
        public string EndDate { get; set; }
    }
}