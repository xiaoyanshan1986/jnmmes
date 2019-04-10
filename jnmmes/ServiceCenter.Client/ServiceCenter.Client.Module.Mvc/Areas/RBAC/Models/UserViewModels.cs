// ***********************************************************************
// Assembly         : ServiceCenter.Client.Mvc
// Author           : peter
// Created          : 08-01-2014
//
// Last Modified By : peter
// Last Modified On : 08-06-2014
// ***********************************************************************
// <copyright file="UserViewModels.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ServiceCenter.Client.Mvc.Resources.RBAC;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

/// <summary>
/// The Models namespace.
/// </summary>
namespace ServiceCenter.Client.Mvc.Areas.RBAC.Models
{
    /// <summary>
    /// Class UserQueryViewModel.
    /// </summary>
    public class UserQueryViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserQueryViewModel" /> class.
        /// </summary>
        public UserQueryViewModel()
        {
        }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        [Display(Name = "User_UserQueryViewModel_UserName",ResourceType=typeof(StringResource))]
        public string UserName { get; set; }


        /// <summary>
        /// Gets or sets the name of the login.
        /// </summary>
        /// <value>The name of the login.</value>
        [Display(Name = "User_UserQueryViewModel_LoginName", ResourceType = typeof(StringResource))]
        public string LoginName { get; set; }
    }

    /// <summary>
    /// Class UserViewModel.
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserViewModel"/> class.
        /// </summary>
        public UserViewModel()
        {
            this.IsActive = true;
            this.IsApproved = true;
            this.IsLocked = false;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the name of the login.
        /// </summary>
        /// <value>The name of the login.</value>
        [Required]
        [Display(Name = "User_UserViewModel_LoginName",ResourceType = typeof(StringResource))]
        [Editable(false)]
        [StringLength(20, MinimumLength = 1
                        , ErrorMessageResourceName="User_UserViewModel_LoginNameValidateStringLength"
                        , ErrorMessageResourceType=typeof(StringResource))]
        public string LoginName { get; set; }


        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        [Required]
        [Display(Name = "User_UserViewModel_UserName", ResourceType = typeof(StringResource))]
        [StringLength(20, MinimumLength = 1
                , ErrorMessageResourceName = "User_UserViewModel_UserNameValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [Required]
        [Display(Name = "User_UserViewModel_Password", ResourceType = typeof(StringResource))]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 3
                , ErrorMessageResourceName = "User_UserViewModel_PasswordValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [Display(Name = "User_UserViewModel_Email", ResourceType = typeof(StringResource))]
        [EmailAddress(ErrorMessage=null
            ,ErrorMessageResourceName = "User_UserViewModel_EmalValidateFormat"
            ,ErrorMessageResourceType = typeof(StringResource))]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the office phone.
        /// </summary>
        /// <value>The office phone.</value>
        [Display(Name = "User_UserViewModel_OfficePhone", ResourceType = typeof(StringResource))]
        [Phone(ErrorMessage = null
                , ErrorMessageResourceName = "User_UserViewModel_OfficePhoneValidateFormat"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string OfficePhone { get; set; }

        /// <summary>
        /// Gets or sets the mobile phone.
        /// </summary>
        /// <value>The mobile phone.</value>
        [Display(Name = "User_UserViewModel_MobilePhone", ResourceType = typeof(StringResource))]
        [Phone(ErrorMessage = null
                , ErrorMessageResourceName = "User_UserViewModel_MobilePhoneValidateFormat"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string MobilePhone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is locked.
        /// </summary>
        /// <value><c>true</c> if this instance is locked; otherwise, <c>false</c>.</value>
        [Display(Name = "User_UserViewModel_IsLocked", ResourceType = typeof(StringResource))]
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [Display(Name = "User_UserViewModel_IsActive", ResourceType = typeof(StringResource))]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is approved.
        /// </summary>
        /// <value><c>true</c> if this instance is approved; otherwise, <c>false</c>.</value>
        [Display(Name = "User_UserViewModel_IsApproved", ResourceType = typeof(StringResource))]
        public bool IsApproved { get; set; }

        /// <summary>
        /// Gets or sets the last login ip.
        /// </summary>
        /// <value>The last login ip.</value>
        [Display(Name = "User_UserViewModel_LastLoginIP", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public string LastLoginIP { get; set; }

        /// <summary>
        /// Gets or sets the last login time.
        /// </summary>
        /// <value>The last login time.</value>
        [Display(Name = "User_UserViewModel_LastLoginTime", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Display(Name = "User_UserViewModel_Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "User_UserViewModel_DescriptionValidateStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the editor.
        /// </summary>
        /// <value>The editor.</value>
        [Display(Name = "User_UserViewModel_Editor", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public string Editor { get; set; }

        /// <summary>
        /// Gets or sets the edit time.
        /// </summary>
        /// <value>The edit time.</value>
        [Display(Name = "User_UserViewModel_EditTime", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public DateTime? EditTime { get; set; }

        /// <summary>
        /// Gets or sets the creator.
        /// </summary>
        /// <value>The creator.</value>
        [Display(Name = "User_UserViewModel_Creator", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public string Creator { get; set; }

        /// <summary>
        /// Gets or sets the create time.
        /// </summary>
        /// <value>The create time.</value>
        [Display(Name = "User_UserViewModel_CreateTime", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public DateTime? CreateTime { get; set; }
    }

    public class SetUserRoleViewModel
    {
        public SetUserRoleViewModel()
        {
        }

        [Display(Name = "User_UserQueryViewModel_LoginName", ResourceType = typeof(StringResource))]
        public string LoginName { get; set; }

        public IList<string> Roles { get; set; }

        public IList<Role> GetRoles()
        {
            using (RoleServiceClient client = new RoleServiceClient())
            {
                PagingConfig cfg=new PagingConfig() { 
                    IsPaging=false
                };
               MethodReturnResult<IList<Role>> result = client.Get(ref cfg);
               if (result.Code == 0)
               {
                   return result.Data;
               }
            }
            return new List<Role>();
        }

        public IList<UserInRole> GetUserInRoles(string loginName)
        {
            using (UserInRoleServiceClient client = new UserInRoleServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where=string.Format("Key.LoginName='{0}'",loginName)
                };
                MethodReturnResult<IList<UserInRole>> result = client.Get(ref cfg);
                if (result.Code == 0)
                {
                    return result.Data;
                }
            }
            return new List<UserInRole>();
        }
    }

    public class SetUserResourceViewModel
    {
        public SetUserResourceViewModel()
        {
        }

        [Display(Name = "User_UserQueryViewModel_LoginName", ResourceType = typeof(StringResource))]
        public string LoginName { get; set; }

        public IList<string> Resources { get; set; }

        public IList<Resource> GetResources()
        {
            using (ResourceServiceClient client = new ResourceServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };
                MethodReturnResult<IList<Resource>> result = client.Get(ref cfg);
                if (result.Code == 0)
                {
                    return result.Data;
                }
            }
            return new List<Resource>();
        }

        public IList<UserOwnResource> GetUserOwnResource(string loginName)
        {
            using (UserOwnResourceServiceClient client = new UserOwnResourceServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.LoginName='{0}'", loginName)
                };
                MethodReturnResult<IList<UserOwnResource>> result = client.Get(ref cfg);
                if (result.Code == 0)
                {
                    return result.Data;
                }
            }
            return new List<UserOwnResource>();
        }
    }
}