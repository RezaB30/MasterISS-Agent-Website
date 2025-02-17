﻿using MasterISS_Agent_Website_Localization;
using MasterISS_Agent_Website_Localization.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.User
{
    public class AddRoleViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "RoleName", ResourceType = typeof(UserModel))]
        public string RoleName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "Permissions", ResourceType = typeof(UserModel))]
        public int[] Permissions { get; set; }
    }

    public class AvailablePermissionList
    {
        public string PermissionName { get; set; }
        public int PermissionId { get; set; }
        public string IsSelected { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "AvailablePermission", ResourceType = typeof(UserModel))]
        public int[] SelectedPermissions { get; set; }

    }
}