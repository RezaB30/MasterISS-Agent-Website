using MasterISS_Agent_Website_Localization;
using MasterISS_Agent_Website_Localization.Setup;
using RadiusR.DB.Enums.CustomerSetup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Setup
{
    public class AddTaskStatusUpdateViewModel
    {
        [Required]
        public long TaskNo { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "Description")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "FaultCodes")]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public FaultCodes FaultCode { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "ReservationDate")]
        [RegularExpression("^(3[01]|[12][0-9]|0[1-9]).(1[0-2]|0[1-9]).[0-9]{4} (2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "DateFormatErrorReservationDate")]
        public string ReservationDate { get; set; }

    }
}