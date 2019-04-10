
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.FMM;
using FMMResources = ServiceCenter.Client.Mvc.Resources.FMM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.FMM;
using System.Web.Mvc;
using ServiceCenter.Common;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Models
{
    public class MaterialQueryViewModel
    {
        public MaterialQueryViewModel()
        {
            Status = EnumObjectStatus.Available.GetHashCode().ToString();
        }

        /// <summary>
        /// 类型
        /// </summary>
        [Display(Name = "MaterialQueryViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        public string Type { get; set; }
        
        /// <summary>
        /// 代码
        /// </summary>
        [Display(Name = "MaterialQueryViewModel_Code", ResourceType = typeof(FMMResources.StringResource))]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "MaterialQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "Status", ResourceType = typeof(StringResource))]
        public string Status { get; set; }

        /// <summary>
        /// 物料归属类型（物料、产品）
        /// </summary>
        [Display(Name = "MaterialQueryViewModel_Ascription", ResourceType = typeof(FMMResources.StringResource))]
        public string Ascription { get; set; }

        /// <summary>
        /// 取得状态类型列表，包含空类型用于表示不选择
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> GetObjectStatusList()
        {
            IEnumerable<SelectListItem> Enumlst = new List<SelectListItem>();
            IList<SelectListItem> lst = new List<SelectListItem>();

            IDictionary<EnumObjectStatus, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumObjectStatus>();

            Enumlst = from item in dic
                      select new SelectListItem()
                      {
                          Text = item.Value,
                          Value = Convert.ToString(item.Key.GetHashCode())
                      };

            SelectListItem NewItem = new SelectListItem()
            {
                Text = "",
                Value = ""
            };

            lst.Add(NewItem);

            foreach (SelectListItem item in Enumlst)
            {
                NewItem = new SelectListItem()
                {
                    Text = item.Text,
                    Value = item.Value
                };

                lst.Add(NewItem);
            }

            return lst;
        }

        /// <summary>
        /// 取得物料归属类型列表，包含空类型用于表示不选择
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> GetMaterialAscriptionList()
        {
            IList<SelectListItem> lst = new List<SelectListItem>();

            SelectListItem NewItem = new SelectListItem()
            {
                Text = "",
                Value = ""
            };

            lst.Add(NewItem);

            //产品
            NewItem = new SelectListItem()
            {
                Text = "产品",
                Value = "P"
            };

            lst.Add(NewItem);

            //物料
            NewItem = new SelectListItem()
            {
                Text = "物料",
                Value = "M"
            };

            lst.Add(NewItem);

            return lst;
        }
    }

    public class MaterialViewModel
    {
        public MaterialViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.Status = EnumObjectStatus.Available;
        }

        /// <summary>
        /// 物料代码
        /// </summary>
        [Required]
        [Display(Name = "MaterialViewModel_Code", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        [Required]
        [Display(Name = "MaterialViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        [Display(Name = "MaterialViewModel_ModelName", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(64, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string ModelName { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        [Display(Name = "MaterialViewModel_Spec", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                   , ErrorMessageResourceName = "ValidateMaxStringLength"
                   , ErrorMessageResourceType = typeof(StringResource))]
        public string Spec { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        [Display(Name = "MaterialViewModel_Unit", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(10, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string Unit { get; set; }

        /// <summary>
        /// 条码？？？
        /// </summary>
        [Display(Name = "MaterialViewModel_BarCode", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string BarCode { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        [Display(Name = "MaterialViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        public string Type { get; set; }

        /// <summary>
        /// 物料分类
        /// </summary>
        [Display(Name = "MaterialViewModel_Class", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string Class { get; set; }

        /// <summary>
        /// 物料标识
        /// </summary>
        [Display(Name = "MaterialViewModel_IsRaw", ResourceType = typeof(FMMResources.StringResource))]
        public bool IsRaw { get; set; }

        /// <summary>
        /// 产品标识
        /// </summary>
        [Display(Name = "MaterialViewModel_IsProduct", ResourceType = typeof(FMMResources.StringResource))]
        public bool IsProduct { get; set; }

        /// <summary>
        /// 每批主材料数量。
        /// </summary>
        [Required]
        [Display(Name = "MaterialTypeViewModel_MainRawQtyPerLot", ResourceType = typeof(FMMResources.StringResource))]
        public double MainRawQtyPerLot { get; set; }

        /// <summary>
        /// 每批产品数量。
        /// </summary>
        [Required]
        [Display(Name = "MaterialTypeViewModel_MainProductQtyPerLot", ResourceType = typeof(FMMResources.StringResource))]
        public double MainProductQtyPerLot { get; set; }

        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }


        [Display(Name = "Status", ResourceType = typeof(StringResource))]
        public EnumObjectStatus Status { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 取得物料类型列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetMaterialTypeList()
        {
            using (MaterialTypeServiceClient client = new MaterialTypeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<MaterialType>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    return from item in result.Data
                           select new SelectListItem()
                           {
                               Text = item.Key,
                               Value = item.Key
                           };
                }
            }
            return new List<SelectListItem>();
        }

        /// <summary>
        /// 取得状态列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetObjectStatusList()
        {
            IDictionary<EnumObjectStatus, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumObjectStatus>();

            return  from item in dic
                    select new SelectListItem()
                    {
                        Text = item.Value,
                        Value = Convert.ToString(item.Key)
                    };;
        }


    }
}