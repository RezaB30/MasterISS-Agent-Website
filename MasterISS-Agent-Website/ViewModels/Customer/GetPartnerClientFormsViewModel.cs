using MasterISS_Agent_Website_Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Customer
{
    public class GetPartnerClientFormsViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public int FormTypeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public long SubscriptionId { get; set; }

        public List<KeyValuePair<int, string>> FormTypes { get; set; }
    }
}