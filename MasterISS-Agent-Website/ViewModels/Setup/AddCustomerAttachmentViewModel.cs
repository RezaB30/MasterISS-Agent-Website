using MasterISS_Agent_Website_Enums.Enums;
using MasterISS_Agent_Website_Localization;
using MasterISS_Agent_Website_Localization.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Setup
{
    public class AddCustomerAttachmentViewModel
    {
        [Required]
        public long TaskNo { get; set; }
        public string CustomerName { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "AttachmentType")]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public AttachmentType AttachmentType { get; set; }
        public string FileData { get; set; }
        public string Extension { get; set; }
    }
}