// ***********************************************************************
// Assembly         : ServiceCenter.Client.Mvc
// Author           : peter
// Created          : 08-06-2014
//
// Last Modified By : peter
// Last Modified On : 08-06-2014
// ***********************************************************************
// <copyright file="RoleViewModels.cs" company="">
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
    /// Class RoleQueryViewModel.
    /// </summary>
    public class RoleQueryViewModel
    {
        public RoleQueryViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>The name of the role.</value>
        [Display(Name = "Role_RoleQueryViewModel_RoleName", ResourceType = typeof(StringResource))]
        public string RoleName { get; set; }

    }

    /// <summary>
    /// Class RoleViewModel.
    /// </summary>
    public class RoleViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleViewModel" /> class.
        /// </summary>
        public RoleViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            
        }

        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>The name of the role.</value>
        [Required]
        [Display(Name = "Role_RoleViewModel_RoleName", ResourceType = typeof(StringResource))]
        [Editable(false)]
        [StringLength(20, MinimumLength = 3
                        , ErrorMessageResourceName = "Role_RoleViewModel_RoleNameValidateStringLength"
                        , ErrorMessageResourceType=typeof(StringResource))]
        public string RoleName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Display(Name = "Role_RoleViewModel_Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "Role_RoleViewModel_DescriptionValidateStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the editor.
        /// </summary>
        /// <value>The editor.</value>
        [Display(Name = "Role_RoleViewModel_Editor", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public string Editor { get; set; }

        /// <summary>
        /// Gets or sets the edit time.
        /// </summary>
        /// <value>The edit time.</value>
        [Display(Name = "Role_RoleViewModel_EditTime", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public DateTime? EditTime { get; set; }

        /// <summary>
        /// Gets or sets the creator.
        /// </summary>
        /// <value>The creator.</value>
        [Display(Name = "Role_RoleViewModel_Creator", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public string Creator { get; set; }

        /// <summary>
        /// Gets or sets the create time.
        /// </summary>
        /// <value>The create time.</value>
        [Display(Name = "Role_RoleViewModel_CreateTime", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public DateTime? CreateTime { get; set; }
    }

    public class SetRoleUserViewModel
    {
        public SetRoleUserViewModel()
        {
        }

        [Display(Name = "Role_RoleViewModel_RoleName", ResourceType = typeof(StringResource))]
        public string RoleName { get; set; }

        public IList<string> Users { get; set; }

        public IList<User> GetUsers()
        {
            using (UserServiceClient client = new UserServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("IsActive='{0}'", true)
                };
                MethodReturnResult<IList<User>> result = client.Get(ref cfg);
                if (result.Code == 0)
                {
                    return result.Data;
                }
            }
            return new List<User>();
        }

        public IList<UserInRole> GetUserInRoles(string roleName)
        {
            using (UserInRoleServiceClient client = new UserInRoleServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.RoleName='{0}'", roleName)
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

    public class SetRoleResourceViewModel
    {
        public SetRoleResourceViewModel()
        {
        }

        [Display(Name = "Role_RoleViewModel_RoleName", ResourceType = typeof(StringResource))]
        public string RoleName { get; set; }

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

        public IList<RoleOwnResource> GetRoleOwnResources(string roleName)
        {
            using (RoleOwnResourceServiceClient client = new RoleOwnResourceServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.RoleName='{0}'", roleName)
                };
                MethodReturnResult<IList<RoleOwnResource>> result = client.Get(ref cfg);
                if (result.Code == 0)
                {
                    return result.Data;
                }
            }
            return new List<RoleOwnResource>();
        }
    }
}