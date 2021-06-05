using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Customer
{
    public class AddClientAttachmentViewModel
    {
        public string SubscriberName { get; set; }

        public int? AttachmentId { get; set; }

        public byte[] FileContect { get; set; }

        public string FileExtention { get; set; }

        public long SubscriptionId { get; set; }

    }
}