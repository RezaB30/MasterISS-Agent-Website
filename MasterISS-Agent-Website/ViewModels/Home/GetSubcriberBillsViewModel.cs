using MasterISS_Agent_Website_Localization;
using MasterISS_Agent_Website_Localization.Home;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Home
{
    public class GetSubcriberBillsViewModel
    {
        [Display(Name = "GetBillsParameter", ResourceType = typeof(HomeModel))]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "OnlyNumeric")]
        public string GetBillsInputParameter { get; set; }
    }
}