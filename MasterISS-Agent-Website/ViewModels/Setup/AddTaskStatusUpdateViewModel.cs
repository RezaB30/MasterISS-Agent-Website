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

        public string Description { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "FaultCodes")]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public FaultCodes FaultCode { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "ReservationDate")]
        public string ReservationDate { get; set; }

    }
}