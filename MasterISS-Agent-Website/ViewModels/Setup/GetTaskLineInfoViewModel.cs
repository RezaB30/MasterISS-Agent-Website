using MasterISS_Agent_Website_Localization.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Setup
{
    public class GetTaskLineInfoViewModel
    {
        [Display(ResourceType = typeof(SetupModel), Name = "XDSLNo")]
        public string XDSLNo { get; set; }
        public string CustomerName { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "IsActive")]
        [UIHint("IsBoolFormat")]
        public bool IsActive { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "DowloadNoiseMargin")]
        public string DownloadNoiseMargin { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "UploadNoiseMargin")]
        public string UploadNoiseMargin { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "CurrentDowloadSpeed")]
        public string CurrentDownloadSpeed { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "CurrentUploadSpeed")]
        public string CurrentUploadSpeed { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "DowloadSpeedCapasity")]
        public string DownloadSpeedCapasity { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "UploadSpeedCapasity")]
        public string UploadSpeedCapasity { get; set; }

        [Display(ResourceType = typeof(SetupModel), Name = "ShelfCardPort")]
        public string ShelfCardPort { get; set; }
    }
}