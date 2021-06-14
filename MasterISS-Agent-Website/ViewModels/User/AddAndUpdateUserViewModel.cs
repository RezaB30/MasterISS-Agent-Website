using MasterISS_Agent_Website_Localization;
using MasterISS_Agent_Website_Localization.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.User
{
    public class AddAndUpdateUserViewModel
    {
        public long UserId { get; set; }

        [Display(ResourceType = typeof(UserModel), Name = "IsEnabled")]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public bool IsEnabled { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(UserModel), Name = "RoleName")]
        public int RoleId { get; set; }

        [MaxLength(300, ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "EmailMaxLenght")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((([a-zA-Z\-]+\.)+))([a-zA-Z]{2,4})(\]?)$", ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "EmailFormat")]
        [Display(ResourceType = typeof(UserModel), Name = "UserEmail")]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public string UserEmail { get; set; }


        [MinLength(6, ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "PasswordMinLenght")]
        [Display(ResourceType = typeof(UserModel), Name = "Password")]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(UserModel), Name = "UserNameSurname")]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public string UserNameSurname { get; set; }

        [Display(ResourceType = typeof(UserModel), Name = "PhoneNo")]
        [RegularExpression(@"^((\d{3})(\d{3})(\d{2})(\d{2}))$", ErrorMessageResourceName = "ValidPhoneNumber", ErrorMessageResourceType = typeof(Validation))]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public string PhoneNumber { get; set; }
    }
}