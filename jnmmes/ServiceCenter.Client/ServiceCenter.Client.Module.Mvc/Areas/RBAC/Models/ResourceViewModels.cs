// ***********************************************************************
// Assembly         : ServiceCenter.Client.Mvc
// Author           : peter
// Created          : 08-06-2014
//
// Last Modified By : peter
// Last Modified On : 08-06-2014
// ***********************************************************************
// <copyright file="ResourceViewModels.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ServiceCenter.Client.Mvc.Resources.RBAC;
using ServiceCenter.Common;
using ServiceCenter.MES.Model.RBAC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

/// <summary>
/// The Models namespace.
/// </summary>
namespace ServiceCenter.Client.Mvc.Areas.RBAC.Models
{
    /// <summary>
    /// Class ResourceQueryViewModel.
    /// </summary>
    public class ResourceQueryViewModel
    {
        public ResourceQueryViewModel()
        {
            this.Type = null;
        }

        [Display(Name = "Resource_ResourceQueryViewModel_ResourceType", ResourceType = typeof(StringResource))]
        public ResourceType? Type { get; set; }

        [Display(Name = "Resource_ResourceQueryViewModel_ResourceCode", ResourceType = typeof(StringResource))]
        public string Code { get; set; }

        [Display(Name = "Resource_ResourceQueryViewModel_ResourceName", ResourceType = typeof(StringResource))]
        public string Name { get; set; }

        public IEnumerable<SelectListItem> GetResourceTypeList()
        {
            IList<string> nullValues = new List<string>();
            nullValues.Add("");
            IDictionary<ResourceType, string> dic = EnumExtensions.GetDisplayNameDictionary<ResourceType>();
            

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text=item.Value,
                                                  Value=item.Key.ToString()
                                              };
            return   (from item in nullValues
                      select new SelectListItem()
                      {
                        Text=item,
                        Value=null,
                        Selected=true
                      }).Union(lst);
        }
    }
    /// <summary>
    /// Class ResourceViewModel.
    /// </summary>
    public class ResourceViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceViewModel" /> class.
        /// </summary>
        public ResourceViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        [Required]
        [Display(Name = "Resource_ResourceViewModel_ResourceType", ResourceType = typeof(StringResource))]
        public ResourceType Type { get; set; }

        [Required]
        [Display(Name = "Resource_ResourceViewModel_ResourceCode", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 1
                        , ErrorMessageResourceName = "Resource_ResourceViewModel_ResourceCodeValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>The name of the role.</value>
        [Required]
        [Display(Name = "Resource_ResourceViewModel_ResourceName", ResourceType = typeof(StringResource))]
        [StringLength(128, MinimumLength = 1
                        , ErrorMessageResourceName = "Resource_ResourceViewModel_ResourceNameValidateStringLength"
                        , ErrorMessageResourceType=typeof(StringResource))]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Resource_ResourceViewModel_ResourceData", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 1
                        , ErrorMessageResourceName = "Resource_ResourceViewModel_ResourceDataValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Display(Name = "Resource_ResourceViewModel_Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "Resource_ResourceViewModel_DescriptionValidateStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the editor.
        /// </summary>
        /// <value>The editor.</value>
        [Display(Name = "Resource_ResourceViewModel_Editor", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public string Editor { get; set; }

        /// <summary>
        /// Gets or sets the edit time.
        /// </summary>
        /// <value>The edit time.</value>
        [Display(Name = "Resource_ResourceViewModel_EditTime", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public DateTime? EditTime { get; set; }

        /// <summary>
        /// Gets or sets the creator.
        /// </summary>
        /// <value>The creator.</value>
        [Display(Name = "Resource_ResourceViewModel_Creator", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public string Creator { get; set; }

        /// <summary>
        /// Gets or sets the create time.
        /// </summary>
        /// <value>The create time.</value>
        [Display(Name = "Resource_ResourceViewModel_CreateTime", ResourceType = typeof(StringResource))]
        [Editable(false)]
        public DateTime? CreateTime { get; set; }

        public IEnumerable<SelectListItem> GetResourceTypeList()
        {
            IDictionary<ResourceType, string> dic = EnumExtensions.GetDisplayNameDictionary<ResourceType>();


            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }
    }
}