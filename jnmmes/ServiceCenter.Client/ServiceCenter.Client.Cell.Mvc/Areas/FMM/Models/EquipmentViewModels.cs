
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
    public class EquipmentQueryViewModel
    {
        public EquipmentQueryViewModel()
        {

        }

        [Display(Name = "EquipmentQueryViewModel_GroupName", ResourceType = typeof(FMMResources.StringResource))]
        public string GroupName { get; set; }

        [Display(Name = "EquipmentQueryViewModel_Code", ResourceType = typeof(FMMResources.StringResource))]
        public string Code { get; set; }
        [Display(Name = "EquipmentQueryViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        public string Name { get; set; }
    }

    public class EquipmentViewModel
    {
        public EquipmentViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.IsBatch = true;
            this.IsChamber = false;
            this.IsMultiChamber = false;
            this.StateName = "INIT";
        }
        [Required]
        [Display(Name = "EquipmentViewModel_Code", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Code { get; set; }

        [Required]
        [Display(Name = "EquipmentViewModel_Name", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string Name { get; set; }


        [Display(Name = "EquipmentViewModel_No", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string No { get; set; }


        [Required]
        [Display(Name = "EquipmentViewModel_Type", ResourceType = typeof(FMMResources.StringResource))]
        public EnumEquipmentType Type { get; set; }

        [Display(Name = "EquipmentViewModel_WPH", ResourceType = typeof(FMMResources.StringResource))]
        [Range(0, 65536
               , ErrorMessageResourceName = "ValidateRange"
               , ErrorMessageResourceType = typeof(StringResource))]
        public double? WPH { get; set; }

        [Display(Name = "EquipmentViewModel_AvTime", ResourceType = typeof(FMMResources.StringResource))]
        [Range(0, 65536
               , ErrorMessageResourceName = "ValidateRange"
               , ErrorMessageResourceType = typeof(StringResource))]
        public double? AvTime { get; set; }

        [Display(Name = "EquipmentViewModel_TactTime", ResourceType = typeof(FMMResources.StringResource))]
        [Range(0, 65536
               , ErrorMessageResourceName = "ValidateRange"
               , ErrorMessageResourceType = typeof(StringResource))]
        public double? TactTime { get; set; }

        [Display(Name = "EquipmentViewModel_AssetsNo", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string AssetsNo { get; set; }

        /// <summary>
        /// 物理设备代码。
        /// </summary>
        [Display(Name = "EquipmentViewModel_RealEquipmentCode", ResourceType = typeof(FMMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public virtual string RealEquipmentCode { get; set; }
        /// <summary>
        /// 是否多腔体设备。 
        /// </summary>
        [Display(Name = "EquipmentViewModel_IsMultiChamber", ResourceType = typeof(FMMResources.StringResource))]
        public virtual bool IsMultiChamber { get; set; }
        /// <summary>
        /// 是否批处理设备。 
        /// </summary>
        [Display(Name = "EquipmentViewModel_IsBatch", ResourceType = typeof(FMMResources.StringResource))]
        public virtual bool IsBatch { get; set; }

        /// <summary>
        /// 是否是设备腔体。
        /// </summary>
        [Display(Name = "EquipmentViewModel_IsChamber", ResourceType = typeof(FMMResources.StringResource))]
        public virtual bool IsChamber { get; set; }
        /// <summary>
        /// 设备腔体数量。
        /// </summary>
        [Display(Name = "EquipmentViewModel_TotalChamber", ResourceType = typeof(FMMResources.StringResource))]
        [Range(0, 65536
               , ErrorMessageResourceName = "ValidateRange"
               , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public virtual int? TotalChamber { get; set; }
        /// <summary>
        /// 腔体索引序号。
        /// </summary>
        [Display(Name = "EquipmentViewModel_ChamberIndex", ResourceType = typeof(FMMResources.StringResource))]
        [Range(0, 65536
               , ErrorMessageResourceName = "ValidateRange"
               , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public virtual int? ChamberIndex { get; set; }
        /// <summary>
        /// 设备运行速率
        /// </summary>
        [Display(Name = "EquipmentViewModel_RunRate", ResourceType = typeof(FMMResources.StringResource))]
        [Range(0, 65536
               , ErrorMessageResourceName = "ValidateRange"
               , ErrorMessageResourceType = typeof(StringResource))]
        public virtual double? RunRate { get; set; }
        /// <summary>
        /// 最小加工数量。
        /// </summary>
        [Display(Name = "EquipmentViewModel_MinQuantity", ResourceType = typeof(FMMResources.StringResource))]
        [Range(0, 65536
               , ErrorMessageResourceName = "ValidateRange"
               , ErrorMessageResourceType = typeof(StringResource))]
        public virtual double? MinQuantity { get; set; }
        /// <summary>
        /// 最大加工数量。
        /// </summary>
        [Display(Name = "EquipmentViewModel_MaxQuantity", ResourceType = typeof(FMMResources.StringResource))]
        [Range(0, 65536
               , ErrorMessageResourceName = "ValidateRange"
               , ErrorMessageResourceType = typeof(StringResource))]
        public virtual double? MaxQuantity { get; set; }

        /// <summary>
        /// 设备状态名称。
        /// </summary>
        [Required]
        [Display(Name = "EquipmentViewModel_StateName", ResourceType = typeof(FMMResources.StringResource))]
        public virtual string StateName { get; set; }
        /// <summary>
        /// 设备组名称。
        /// </summary>
        [Required]
        [Display(Name = "EquipmentViewModel_GroupName", ResourceType = typeof(FMMResources.StringResource))]
        public virtual string GroupName { get; set; }
        /// <summary>
        /// 区域名称。
        /// </summary>
        [Required]
        [Display(Name = "EquipmentViewModel_LocationName", ResourceType = typeof(FMMResources.StringResource))]
        public virtual string LocationName { get; set; }
        /// <summary>
        /// 生产线代码。
        /// </summary>
        [Required]
        [Display(Name = "EquipmentViewModel_LineCode", ResourceType = typeof(FMMResources.StringResource))]
        public virtual string LineCode { get; set; }
        /// <summary>
        /// 父设备代码。
        /// </summary>
        [Display(Name = "EquipmentViewModel_ParentEquipmentCode", ResourceType = typeof(FMMResources.StringResource))]
        public virtual string ParentEquipmentCode { get; set; }
        

        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }


        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }


        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }


        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        public IEnumerable<SelectListItem> GetEquipmentTypeList()
        {
            IDictionary<EnumEquipmentType, string> dic = EnumExtensions.GetDisplayNameDictionary<EnumEquipmentType>();

            IEnumerable<SelectListItem> lst = from item in dic
                                              select new SelectListItem()
                                              {
                                                  Text = item.Value,
                                                  Value = item.Key.ToString()
                                              };
            return lst;
        }

        public IEnumerable<SelectListItem> GetEquipmentGroupList()
        {
            using (EquipmentGroupServiceClient client = new EquipmentGroupServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<EquipmentGroup>> result = client.Get(ref cfg);
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

        public IEnumerable<SelectListItem> GetEquipmentStateList()
        {
            using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<EquipmentState>> result = client.Get(ref cfg);
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

        public IEnumerable<SelectListItem> GetLocationList()
        {
            using (LocationServiceClient client = new LocationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where=string.Format("Level='{0}'",Convert.ToInt32(LocationLevel.Area))
                };

                MethodReturnResult<IList<Location>> result = client.Get(ref cfg);
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
        public IEnumerable<SelectListItem> GetProductionLineList()
        {
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };

                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
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
    }
}