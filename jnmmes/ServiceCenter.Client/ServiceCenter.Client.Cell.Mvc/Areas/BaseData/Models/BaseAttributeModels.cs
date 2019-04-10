
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.Client.Mvc.Resources.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using System.Web.Mvc;
using ServiceCenter.Common;

namespace ServiceCenter.Client.Mvc.Areas.BaseData.Models
{
    public class BaseAttributeQueryViewModel
    {
        public BaseAttributeQueryViewModel()
        {
        }

        [Display(Name = "BaseAttributeQueryViewModel_CategoryName", ResourceType = typeof(StringResource))]
        public string CategoryName { get; set; }
        [Display(Name = "BaseAttributeQueryViewModel_AttributeName", ResourceType = typeof(StringResource))]
        public string AttributeName { get; set; }

    }

    public class BaseAttributeViewModel
    {
        public BaseAttributeViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.DataType = EnumDataType.String;
            this.IsPrimaryKey = false;
            this.Order = 1;
        }

        [Required]
        [Display(Name = "BaseAttributeViewModel_CategoryName", ResourceType = typeof(StringResource))]
        [StringLength(50, MinimumLength = 3
                        , ErrorMessageResourceName = "BaseAttributeViewModel_ValidateStringLength"
                        , ErrorMessageResourceType=typeof(StringResource))]
        public string CategoryName { get; set; }


        [Required]
        [Display(Name = "BaseAttributeViewModel_AttributeName", ResourceType = typeof(StringResource))]
        [StringLength(50, MinimumLength = 3
                        , ErrorMessageResourceName = "BaseAttributeViewModel_ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string AttributeName { get; set; }


        [Required]
        [Display(Name = "BaseAttributeViewModel_DataType", ResourceType = typeof(StringResource))]
        public EnumDataType DataType { get; set; }

        [Required]
        [Range(1,65536
                , ErrorMessageResourceName = "BaseAttributeViewModel_ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
            , ErrorMessageResourceName = "BaseAttributeViewModel_ValidateInt"
            , ErrorMessageResourceType = typeof(StringResource))]
        [Display(Name = "BaseAttributeViewModel_Order", ResourceType = typeof(StringResource))]
        public int Order { get; set; }

        [Required]
        [Display(Name = "BaseAttributeViewModel_IsPrimaryKey", ResourceType = typeof(StringResource))]
        public bool IsPrimaryKey { get; set; }

        [Display(Name = "BaseAttributeViewModel_Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "BaseAttributeViewModel_MaxValidateStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }


        [Display(Name = "BaseAttributeViewModel_Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "BaseAttributeViewModel_EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "BaseAttributeViewModel_Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "BaseAttributeViewModel_CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }


        public IEnumerable<SelectListItem> GetDataTypeList()
        {
            IDictionary<EnumDataType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumDataType>();

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