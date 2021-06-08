using MasterISS_Agent_Website_Localization.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Setup
{
    public class ListCustomerSessionInfo
    {
        public SessionInfo FirstSessionInfo { get; set; }
        public SessionInfo LastSessionInfo { get; set; }
    }

    public class SessionInfo
    {
        [Display(ResourceType = typeof(SetupModel), Name = "IsOnline")]
        [UIHint("BoolFormat")]
        public bool IsOnline { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "SessionId")]
        public string SessionId { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "NASIPAddress")]
        public string NASIPAddress { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "IPAddress")]
        public string IPAddress { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "SessionTime")]
        public TimeSpan? SessionTime { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "SessionStart")]
        [UIHint("DateTimeConverted")]
        public DateTime SessionStart { get; set; }
    }
}