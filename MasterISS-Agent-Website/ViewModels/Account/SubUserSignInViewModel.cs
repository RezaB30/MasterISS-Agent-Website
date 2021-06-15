using MasterISS_Agent_Website_Localization;
using MasterISS_Agent_Website_Localization.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Account
{
    public class SubUserSignInViewModel
    {
        [Display(Name = "Username", ResourceType = typeof(AccountModel))]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public string Username { get; set; }

         [Display(Name = "AdminUsername", ResourceType = typeof(AccountModel))]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public string AdminUsername { get; set; }



        [Display(Name = "Password", ResourceType = typeof(AccountModel))]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}