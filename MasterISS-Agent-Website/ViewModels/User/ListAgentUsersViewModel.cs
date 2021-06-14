using MasterISS_Agent_Website_Localization.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.User
{
    public class ListAgentUsersViewModel
    {
        public long UserId { get; set; }

        [Display(ResourceType = typeof(UserModel), Name = "UserEmail")]
        public string UserEmail { get; set; }

        [Display(ResourceType = typeof(UserModel), Name = "UserNameSurname")]
        public string NameSurname { get; set; }

        [Display(ResourceType = typeof(UserModel), Name = "IsEnabled")]
        [UIHint("BoolFormat")]
        public bool IsEnabled { get; set; }

        [Display(ResourceType = typeof(UserModel), Name = "RoleName")]
        public string RoleName { get; set; }

        [Display(ResourceType = typeof(UserModel), Name = "PhoneNo")]
        public string PhoneNumber { get; set; }
    }
}