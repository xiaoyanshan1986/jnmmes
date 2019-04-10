
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using LSMResources = ServiceCenter.Client.Mvc.Resources.LSM;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.LSM;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;


namespace ServiceCenter.Client.Mvc.Areas.LSM.Models
{
    public class MaterialScrapTypeQueryViewModel
    {
        public MaterialScrapTypeQueryViewModel() { }

        [Display(Name = "MaterialScrapTypeViewModels_BillCode", ResourceType = typeof(LSMResources.StringResource))]
        public string BillCode { get; set; }
    }

    public class MaterialScrapTypeViewModels
    {
        public MaterialScrapTypeViewModels()
        {
            ScrapDate = DateTime.Now;
        }

        [Display(Name = "MaterialScrapTypeViewModels_BillCode", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string BillCode { get; set; }

        [Display(Name = "MaterialScrapTypeViewModels_ScrapType", ResourceType = typeof(LSMResources.StringResource))]
        public EnumScrapType ScrapType { get; set; }

        [Display(Name = "MaterialScrapTypeViewModels_BillDate", ResourceType = typeof(LSMResources.StringResource))]
        public DateTime ScrapDate { get; set; }

        [Display(Name = "MaterialScrapTypeViewModels_BillState", ResourceType = typeof(LSMResources.StringResource))]
        public int State { get; set; }

        [Display(Name = "MaterialScrapTypeViewModels_OrderNumber", ResourceType = typeof(LSMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "MaterialScrapTypeViewModels_Note", ResourceType = typeof(LSMResources.StringResource))]
        public string Description { get; set; }

        [Display(Name = "MaterialScrapTypeViewModels_ScrapState", ResourceType = typeof(LSMResources.StringResource))]
        public int ScrapState { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }

        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }

        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "MaterialScrapTypeDetailViewModels_ItemNo", ResourceType = typeof(LSMResources.StringResource))]
        public int ItemNo { get; set; }

        [Display(Name = "MaterialScrapTypeDetailViewModels_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        public string LineStoreName { get; set; }

        [Display(Name = "MaterialScrapTypeDetailViewModels_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialCode { get; set; }
        [Display(Name = "MaterialScrapTypeDetailViewModels_MaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialLot { get; set; }
        [Display(Name = "MaterialScrapTypeDetailViewModels_Qty", ResourceType = typeof(LSMResources.StringResource))]
        public double Qty { get; set; }

        public IEnumerable<SelectListItem> GetLineStore()
        {
            //根据物料类型获取物料。
            //IList<WorkOrder> lst = new List<WorkOrder>();
            IList<LineStore> lstMaterial = null;

            using (LineStoreServiceClient client = new LineStoreServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                };
                MethodReturnResult<IList<LineStore>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstMaterial = result.Data;
                    IEnumerable<SelectListItem> lst = from item in lstMaterial
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Key,
                                                          Value = item.Key
                                                      };
                    return lst;
                }
            }
            return new List<SelectListItem>();
        }
    }

    public class MaterialScrapTypeDetailViewModels
    {
        public MaterialScrapTypeDetailViewModels()
        {
        }

        [Display(Name = "MaterialScrapTypeViewModels_BillCode", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 2
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        public string ScrapNo { get; set; }

        [Display(Name = "MaterialScrapTypeDetailViewModels_ItemNo", ResourceType = typeof(LSMResources.StringResource))]
        public int ItemNo { get; set; }

        [Display(Name = "MaterialScrapTypeDetailViewModels_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        public string LineStoreName { get; set; }

        [Display(Name = "MaterialScrapTypeViewModels_OrderNumber", ResourceType = typeof(LSMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "MaterialScrapTypeDetailViewModels_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialCode { get; set; }
        [Display(Name = "MaterialScrapTypeDetailViewModels_MaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialLot { get; set; }
        [Display(Name = "MaterialScrapTypeDetailViewModels_Qty", ResourceType = typeof(LSMResources.StringResource))]
        public string Qty { get; set; }

        [Display(Name = "MaterialScrapTypeDetailViewModels_Note", ResourceType = typeof(LSMResources.StringResource))]
        public string Description { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }

        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }

        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }
    }

}