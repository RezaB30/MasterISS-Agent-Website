﻿using MasterISS_Agent_Website_Localization;
using MasterISS_Agent_Website_Localization.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Customer
{
    public class AddClientAttachmentViewModel
    {
      
        public string SubscriberName { get; set; }

        [Display(ResourceType = typeof(CustomerModel), Name = "AttachmentType")]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public int? AttachmentId { get; set; }

        public byte[] FileContect { get; set; }

        public string FileExtention { get; set; }

        public long SubscriptionId { get; set; }

    }
}