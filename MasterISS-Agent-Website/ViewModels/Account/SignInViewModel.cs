﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MasterISS_Agent_Website_Localization;
using MasterISS_Agent_Website_Localization.Account;

namespace MasterISS_Agent_Website.ViewModels
{
    public class SignInViewModel
    {
        [Display(Name = "Username", ResourceType = typeof(AccountModel))]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        public string Username { get; set; }

        [Display(Name = "Password", ResourceType = typeof(AccountModel))]
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}