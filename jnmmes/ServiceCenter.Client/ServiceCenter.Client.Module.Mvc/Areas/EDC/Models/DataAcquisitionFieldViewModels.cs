using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EDCResources = ServiceCenter.Client.Mvc.Resources.EDC;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using System.Web.Mvc;
using ServiceCenter.Common;

namespace ServiceCenter.Client.Mvc.Areas.EDC.Models
{
    public class DataAcquisitionFieldQueryViewModel
    {
        public DataAcquisitionFieldQueryViewModel()
        {
        }

        /// <summary>
        /// 采集项目代码
        /// </summary>
        [Display(Name = "DataAcquisitionFieldQueryViewModel_ItemCode", ResourceType = typeof(EDCResources.StringResource))]
        public string ItemCode { get; set; }

        /// <summary>
        /// 采集字段代码
        /// </summary>
        [Display(Name = "DataAcquisitionFieldQueryViewModel_FieldCode", ResourceType = typeof(EDCResources.StringResource))]
        public string FieldCode { get; set; }
    }

    public class DataAcquisitionFieldViewModel
    {
        public DataAcquisitionFieldViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.DataType = EnumDataType.String;
            this.IsKEY = false;
            this.SerialNumber = 1;
            this.IsControl = false;
            this.MinLine = 0;
            this.MinLine = 0;
        }

        /// <summary>
        /// 采集项目代码
        /// </summary>
        [Required]
        [Display(Name = "DataAcquisitionFieldQueryViewModel_ItemCode", ResourceType = typeof(EDCResources.StringResource))]
        public string ItemCode { get; set; }

        /// <summary>
        /// 采集字段代码
        /// </summary>
        [Required]
        [Display(Name = "DataAcquisitionFieldQueryViewModel_FieldCode", ResourceType = typeof(EDCResources.StringResource))]
        public string FieldCode { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        [Required]
        [Display(Name = "DataAcquisitionFieldViewModel_FieldName", ResourceType = typeof(EDCResources.StringResource))]
        [StringLength(50, MinimumLength = 0
                        , ErrorMessageResourceName = "DataAcquisitionFieldViewModel_ValidateStringLength"
                        , ErrorMessageResourceType = typeof(EDCResources.StringResource))]
        public string FieldName { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Required]
        [Range(1, 65536
                , ErrorMessageResourceName = "DataAcquisitionFieldViewModel_ValidateRange"
                , ErrorMessageResourceType = typeof(EDCResources.StringResource))]
        [RegularExpression("[0-9]+"
            , ErrorMessageResourceName = "DataAcquisitionFieldViewModel_ValidateInt"
            , ErrorMessageResourceType = typeof(EDCResources.StringResource))]

        [Display(Name = "DataAcquisitionFieldViewModel_SerialNumber", ResourceType = typeof(EDCResources.StringResource))]        
        public int SerialNumber { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        [Required]
        [Display(Name = "DataAcquisitionFieldViewModel_DataType", ResourceType = typeof(EDCResources.StringResource))]
        public EnumDataType DataType { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        [Required]
        [Display(Name = "DataAcquisitionFieldViewModel_IsKEY", ResourceType = typeof(EDCResources.StringResource))]
        public bool IsKEY { get; set; }

        /// <summary>
        /// 范围控制
        /// </summary>
        [Required]
        [Display(Name = "DataAcquisitionFieldViewModel_IsControl", ResourceType = typeof(EDCResources.StringResource))]
        public bool IsControl { get; set; }

        /// <summary>
        /// 控制上限
        /// </summary>
        [Required]
        [Display(Name = "DataAcquisitionFieldViewModel_MaxLine", ResourceType = typeof(EDCResources.StringResource))]
        public decimal MaxLine { get; set; }

        /// <summary>
        /// 控制下限
        /// </summary>
        [Required]
        [Display(Name = "DataAcquisitionFieldViewModel_MinLine", ResourceType = typeof(EDCResources.StringResource))]
        public decimal MinLine { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }

        /// <summary>
        /// 编辑日期
        /// </summary>
        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }
        
        /// <summary>
        /// 取得数据类型列表
        /// </summary>
        /// <returns></returns>
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