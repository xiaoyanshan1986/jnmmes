
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.PPM;
using PPMResources = ServiceCenter.Client.Mvc.Resources.PPM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.PPM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Models
{
    public class WorkOrderBOMQueryViewModel
    {
        public WorkOrderBOMQueryViewModel()
        {

        }
        [Display(Name = "WorkOrderBOMQueryViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "WorkOrderBOMQueryViewModel_MaterialCode", ResourceType = typeof(PPMResources.StringResource))]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 取得物料名称
        /// </summary>
        /// <param name="materialCode">物料代码</param>
        /// <returns>物料名称</returns>
        public string GetMaterialName(string materialCode)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = client.Get(materialCode);
                if (result.Code <= 0 && result.Data != null)
                {
                    return result.Data.Name;
                }
            }
            return string.Empty;
        }
    }

    public class WorkOrderBOMViewModel
    {
        public WorkOrderBOMViewModel()
        {
            this.EditTime = DateTime.Now;
            this.CreateTime = DateTime.Now;
            this.Qty = 1;
            this.MinUnit = 0;                   //默认不进行最小扣料数量控制
        }

        /// <summary> 工单号 </summary>
        [Required]
        [Display(Name = "WorkOrderBOMViewModel_OrderNumber", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }

        /// <summary> 项目号 </summary>
        [Required]
        [Display(Name = "WorkOrderBOMViewModel_ItemNo", ResourceType = typeof(PPMResources.StringResource))]
        [Range(1, 65536
            , ErrorMessageResourceName = "ValidateRange"
            , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[0-9]+"
                , ErrorMessageResourceName = "ValidateInt"
                , ErrorMessageResourceType = typeof(StringResource))]
        public int ItemNo { get; set; }

        /// <summary> 物料代码 </summary>
        [Required]
        [Display(Name = "WorkOrderBOMViewModel_MaterialCode", ResourceType = typeof(PPMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }

        /// <summary> 物料名称 </summary>
        [Display(Name = "WorkOrderBOMViewModel_MaterialName", ResourceType = typeof(PPMResources.StringResource))]        
        public string MaterialName { get; set; }

        /// <summary> 物料规格 </summary>
        [Display(Name = "WorkOrderBOMViewModel_MaterialSpec", ResourceType = typeof(PPMResources.StringResource))]
        public string MaterialSpec { get; set; }

        /// <summary> 物料型号 </summary>
        [Display(Name = "WorkOrderBOMViewModel_MaterialModel", ResourceType = typeof(PPMResources.StringResource))]
        public string MaterialModel { get; set; }

        /// <summary> 物料描述 </summary>
        [Display(Name = "WorkOrderBOMViewModel_MaterialMemo", ResourceType = typeof(PPMResources.StringResource))]
        public string MaterialMemo { get; set; }

        /// <summary> BOM需求量 </summary>
        [Required]
        [Display(Name = "WorkOrderBOMViewModel_Qty", ResourceType = typeof(PPMResources.StringResource))]
        [Range(0, 2147483648
                , ErrorMessageResourceName = "ValidateRange"
                , ErrorMessageResourceType = typeof(StringResource))]
        public double Qty { get; set; }

        /// <summary> 计量单位 </summary>
        [Display(Name = "WorkOrderBOMViewModel_MaterialUnit", ResourceType = typeof(PPMResources.StringResource))]
        public string MaterialUnit { get; set; }

        /// <summary> 最小扣料单位 </summary>
        [Display(Name = "WorkOrderBOMViewModel_MinUnit", ResourceType = typeof(PPMResources.StringResource))]
        public double MinUnit { get; set; }

        /// <summary> 可替换物料 </summary>
        [Display(Name = "WorkOrderBOMViewModel_ReplaceMaterial", ResourceType = typeof(PPMResources.StringResource))]        
        public string ReplaceMaterial { get; set; }

        /// <summary> 工作中心 </summary>
        [Display(Name = "WorkOrderBOMViewModel_WorkCenter", ResourceType = typeof(PPMResources.StringResource))]
        public string WorkCenter { get; set; }

        /// <summary> 存储位置 </summary>
        [Display(Name = "WorkOrderBOMViewModel_StoreLocation", ResourceType = typeof(PPMResources.StringResource))]
        public string StoreLocation { get; set; }

        /// <summary> 描述 </summary>
        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                     , ErrorMessageResourceName = "ValidateMaxStringLength"
                     , ErrorMessageResourceType = typeof(StringResource))]
        public string Description { get; set; }

        /// <summary> 编辑人 </summary>
        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }

        /// <summary> 编辑日期 </summary>
        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        /// <summary> 创建人 </summary>
        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }

        /// <summary> 创建日期 </summary>
        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        /// <summary> 取得物料名称 </summary>
        /// <param name="materialCode">物料代码</param>
        /// <returns>物料名称</returns>
        public string GetMaterialName(string materialCode)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = client.Get(materialCode);
                if (result.Code <= 0 && result.Data != null)
                {
                    return result.Data.Name;
                }
            }
            return string.Empty;
        }

        /// <summary> 取得物料规格 </summary>
        /// <param name="materialCode">物料代码</param>
        /// <returns>物料规格</returns>
        public string GetMaterialSpec(string materialCode)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = client.Get(materialCode);
                if (result.Code <= 0 && result.Data != null)
                {
                    return result.Data.Spec;
                }
            }
            return string.Empty;
        }

        /// <summary> 取得物料型号 </summary>
        /// <param name="materialCode">物料代码</param>
        /// <returns>物料型号</returns>
        public string GetMaterialModel(string materialCode)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = client.Get(materialCode);
                if (result.Code <= 0 && result.Data != null)
                {
                    return result.Data.ModelName;
                }
            }
            return string.Empty;
        }

        /// <summary> 取得物料描述 </summary>
        /// <param name="materialCode">物料代码</param>
        /// <returns>物料描述</returns>
        public string GetMaterialDescription(string materialCode)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = client.Get(materialCode);
                if (result.Code <= 0 && result.Data != null)
                {
                    return result.Data.Description;
                }
            }
            return string.Empty;
        }
    }

}