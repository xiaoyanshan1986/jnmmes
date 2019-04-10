
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.LSM;
using LSMResources = ServiceCenter.Client.Mvc.Resources.LSM;
using ServiceCenter.Client.Mvc.Resources;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.PPM;

namespace ServiceCenter.Client.Mvc.Areas.LSM.Models
{
    public class MaterialReceiptExQueryViewModel
    {
        public MaterialReceiptExQueryViewModel()
        {

        }
        [Display(Name = "MaterialReceiptQueryViewModel_ReceiptNo", ResourceType = typeof(LSMResources.StringResource))]
        public string ReceiptNo { get; set; }
    }

    public class MaterialReceiptExViewModel
    {
        public MaterialReceiptExViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.ReceiptDate = DateTime.Now.Date;
            this.Type = EnumReceiptType.Normal;
        }


        [Display(Name = "MaterialReceiptViewModel_ReceiptNo", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        //[RegularExpression("LMK[0-9]{2}(0[1-9]|1[0-2])[0-9]{4}"
        //          , ErrorMessage = "格式为：LMK+YYMM(年月)+4位流水号")]
        public string ReceiptNo { get; set; }

        [Required]
        [Display(Name = "MaterialReceiptViewModel_Type", ResourceType = typeof(LSMResources.StringResource))]
        public EnumReceiptType Type { get; set; }

        [Required]
        [Display(Name = "MaterialReceiptViewModel_ReceiptDate", ResourceType = typeof(LSMResources.StringResource))]
        public DateTime ReceiptDate { get; set; }

        [Required]
        [Display(Name = "MaterialReceiptViewModel_OrderNumber", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                       , ErrorMessageResourceName = "ValidateStringLength"
                       , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }

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

        [Required]
        [Display(Name = "MaterialReceiptDetailViewModel_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
                , ErrorMessageResourceName = "ValidateString"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string LineStore { get; set; }

        public Material GetMaterial(string key)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }

        public IEnumerable<SelectListItem> GetLineStoreList()
        {


            using (LineStoreServiceClient client = new LineStoreServiceClient())
            {

                PagingConfig cfg = new PagingConfig()
                {

                    IsPaging = false,
                    Where = string.Format(" Type='{0}' AND Status='{1}'"
                       
                        , Convert.ToInt32(EnumLineStoreType.Material)
                        , Convert.ToInt32(EnumObjectStatus.Available))
                };
                MethodReturnResult<IList<LineStore>> result = client.Get(ref cfg);

                if (result.Code == 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
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
        public IEnumerable<SelectListItem> GetOrderNumbers()
        {

            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"CloseType='{0}' "
                                           , Convert.ToInt32(EnumCloseType.None))
                };

                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
                if (result.Code == 0)
                {
                    IEnumerable<SelectListItem> lst = from item in result.Data
                                                      select new SelectListItem()
                                                      {
                                                          Text = item.Key,
                                                          Value = item.Key
                                                      };
                    return lst;
                }
                return new List<SelectListItem>();
            }

        }

    }

    public class MaterialReceiptExDetailQueryViewModel
    {
        public MaterialReceiptExDetailQueryViewModel()
        {
            this.ReceiptDate = DateTime.Now.ToString("yyyy-MM-dd");
        }

        [Display(Name = "MaterialReceiptDetailQueryViewModel_ReceiptNo", ResourceType = typeof(LSMResources.StringResource))]
        public string ReceiptNo { get; set; }

        [Display(Name = "MaterialReceiptDetailQueryViewModel_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        public string LineStoreName { get; set; }

        [Display(Name = "MaterialReceiptViewModel_OrderNumber", ResourceType = typeof(LSMResources.StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "MaterialReceiptDetailQueryViewModel_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialCode { get; set; }

        [Display(Name = "MaterialReceiptDetailQueryViewModel_MaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        public string MaterialLot { get; set; }

        [Display(Name = "MaterialReceiptViewModel_ReceiptDate", ResourceType = typeof(LSMResources.StringResource))]
        public string ReceiptDate { get; set; }

        public Material GetMaterial(string key)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }

        public MaterialReceipt GetMaterialReceipt(string key)
        {
            using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
            {
                MethodReturnResult<MaterialReceipt> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }

        public Supplier GetSupplier(string key)
        {
            using (SupplierServiceClient client = new SupplierServiceClient())
            {
                MethodReturnResult<Supplier> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }



    }

    public class MaterialReceiptExDetailViewModel
    {
        public MaterialReceiptExDetailViewModel()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.ItemNo = 1;
        }


        [Required]
        [Display(Name = "MaterialReceiptViewModel_OrderNumber", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                       , ErrorMessageResourceName = "ValidateStringLength"
                       , ErrorMessageResourceType = typeof(StringResource))]
        public string OrderNumber { get; set; }

        [Display(Name = "MaterialReceiptDetailViewModel_ReceiptNo", ResourceType = typeof(LSMResources.StringResource))]
        public string ReceiptNo { get; set; }

        [Display(Name = "MaterialReceiptDetailViewModel_ItemNo", ResourceType = typeof(LSMResources.StringResource))]
        public int ItemNo { get; set; }

        [Required]
        [Display(Name = "MaterialReceiptDetailViewModel_LineStoreName", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
                , ErrorMessageResourceName = "ValidateString"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string LineStoreName { get; set; }

        [Required]
        [Display(Name = "MaterialReceiptDetailViewModel_MaterialCode", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
               , ErrorMessageResourceName = "ValidateStringLength"
               , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
               , ErrorMessageResourceName = "ValidateString"
               , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialCode { get; set; }

        [Required]
        [Display(Name = "MaterialReceiptDetailViewModel_MaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
               , ErrorMessageResourceName = "ValidateStringLength"
               , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
               , ErrorMessageResourceName = "ValidateString"
               , ErrorMessageResourceType = typeof(StringResource))]
        public string MaterialLot { get; set; }

        [Required]
        [Display(Name = "MaterialReceiptDetailViewModel_Qty", ResourceType = typeof(LSMResources.StringResource))]
        [Range(0, 2147483648
            , ErrorMessageResourceName = "ValidateRange"
            , ErrorMessageResourceType = typeof(StringResource))]
        public double? Qty { get; set; }

        [Required]
        [Display(Name = "MaterialReceiptDetailViewModel_SupplierCode", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
       , ErrorMessageResourceName = "ValidateStringLength"
       , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
               , ErrorMessageResourceName = "ValidateString"
               , ErrorMessageResourceType = typeof(StringResource))]
        public string SupplierCode { get; set; }
        [Required]
        [Display(Name = "MaterialReceiptDetailViewModel_SupplierName", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
               , ErrorMessageResourceName = "ValidateStringLength"
               , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
               , ErrorMessageResourceName = "ValidateString"
               , ErrorMessageResourceType = typeof(StringResource))]
        public string SupplierName { get; set; }

        [Display(Name = "MaterialReceiptDetailViewModel_SupplierMaterialLot", ResourceType = typeof(LSMResources.StringResource))]
        [StringLength(50, MinimumLength = 1
               , ErrorMessageResourceName = "ValidateStringLength"
               , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
                , ErrorMessageResourceName = "ValidateString"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string SupplierMaterialLot { get; set; }

        [Display(Name = "Description", ResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
               , ErrorMessageResourceName = "ValidateString"
               , ErrorMessageResourceType = typeof(StringResource))]
        [StringLength(255, MinimumLength = 0, ErrorMessage = null
                , ErrorMessageResourceName = "ValidateMaxStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string DetailDescription { get; set; }

        [StringLength(50, MinimumLength = 1
                        , ErrorMessageResourceName = "ValidateStringLength"
                        , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
                , ErrorMessageResourceName = "ValidateString"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string Attr1 { get; set; }
        [StringLength(50, MinimumLength = 1
                       , ErrorMessageResourceName = "ValidateStringLength"
                       , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
                , ErrorMessageResourceName = "ValidateString"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string Attr2 { get; set; }
        [StringLength(50, MinimumLength = 1
                       , ErrorMessageResourceName = "ValidateStringLength"
                       , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
                , ErrorMessageResourceName = "ValidateString"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string Attr3 { get; set; }
        [StringLength(50, MinimumLength = 1
                       , ErrorMessageResourceName = "ValidateStringLength"
                       , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
                , ErrorMessageResourceName = "ValidateString"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string Attr4 { get; set; }
        [StringLength(50, MinimumLength = 1
                       , ErrorMessageResourceName = "ValidateStringLength"
                       , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
                , ErrorMessageResourceName = "ValidateString"
                , ErrorMessageResourceType = typeof(StringResource))]
        public string Attr5 { get; set; }

        [Display(Name = "Editor", ResourceType = typeof(StringResource))]
        public string Editor { get; set; }

        [Display(Name = "EditTime", ResourceType = typeof(StringResource))]
        public DateTime? EditTime { get; set; }

        [Display(Name = "Creator", ResourceType = typeof(StringResource))]
        public string Creator { get; set; }

        [Display(Name = "CreateTime", ResourceType = typeof(StringResource))]
        public DateTime? CreateTime { get; set; }

        public Material GetMaterial(string key)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }

        public MaterialReceipt GetMaterialReceipt(string key)
        {
            using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
            {
                MethodReturnResult<MaterialReceipt> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }

        public Supplier GetSupplier(string key)
        {
            using (SupplierServiceClient client = new SupplierServiceClient())
            {
                MethodReturnResult<Supplier> rst = client.Get(key);
                if (rst.Code <= 0)
                {
                    return rst.Data;
                }
            }
            return null;
        }


        public IEnumerable<SelectListItem> GetColorList()
        {
            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            using (BaseAttributeValueServiceClient client1 = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg1 = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='Color' AND Key.AttributeName='VALUE'"),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result1 = client1.Get(ref cfg1);
                if (result1.Code <= 0 && result1.Data != null)
                {
                    lstValues = result1.Data;
                }
            }
            return from item in lstValues
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = item.Value
                   };
        }


        public IEnumerable<SelectListItem> GetColorOfCell()
        {
            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='Color_Cell' AND Key.AttributeName='CName'"),
                    OrderBy = "Key.ItemOrder"
                };

                MethodReturnResult<IList<BaseAttributeValue>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstValues = result.Data;
                }
            }

            return from item in lstValues
                   select new SelectListItem()
                   {
                       Text = item.Value,
                       Value = item.Value
                   };

            //var lnqValue = from item in lstValues
            //               where item.Key.AttributeName == "VALUE"
            //               select item;

            //var lnqName = from item in lstValues
            //              where item.Key.AttributeName == "NAME"
            //              select item;

            //return from item in lnqValue
            //       join itemName in lnqName on item.Key.ItemOrder equals itemName.Key.ItemOrder
            //       select new SelectListItem()
            //       {
            //           Text = string.Format("{0}-{1}", item.Value, itemName.Value),
            //           Value = item.Value
            //       };
        }

    }

}