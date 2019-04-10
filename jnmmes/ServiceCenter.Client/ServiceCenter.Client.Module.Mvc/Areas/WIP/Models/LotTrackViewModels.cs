
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceCenter.MES.Service.Client;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.WIP;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Service.Client.WIP;
using System.Web.Mvc;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.MES.Service.Contract.WIP;
using System.Data;
using System.Text;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Models
{
    public class LotTrackViewModel
    {
        //string localName = System.Configuration.ConfigurationSettings.AppSettings["LocalName"];
        public LotTrackViewModel()
        {
            //if (localName == "K01")
            //{
                
            //}
            //if (localName == "G01")
            //{
                
            //}
        }

        [Required]
        [Display(Name = "LotNumber", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string LotNumber { get; set; }

        /// <summary>
        /// 工单号
        /// </summary>
        [Display(Name = "LotTrackMaterialViewModel_OrderNumber", ResourceType = typeof(WIPResources.StringResource))]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 工艺流程
        /// </summary>
        [Display(Name = "LotTrackViewModel_RouteName", ResourceType = typeof(WIPResources.StringResource))]
        public string RouteName { get; set; }

        /// <summary>
        /// 工序
        /// </summary>
        [Required]
        [Display(Name = "LotTrackViewModel_RouteOperationName", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string RouteOperationName { get; set; }

        [Required]
        [Display(Name = "LotTrackViewModel_LineCode", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string LineCode { get; set; }

        [Display(Name = "LotTrackViewModel_EquipmentCode", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string EquipmentCode { get; set; }

        [Display(Name = "LotTrackViewModel_EquipmentState", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
                , ErrorMessageResourceName = "ValidateStringLength"
                , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string EquipmentState { get; set; }

        /// <summary>
        /// 等级。
        /// </summary>
        [Required]
        [Display(Name = "LotViewModel_Grade", ResourceType = typeof(WIPResources.StringResource))]
        public string Grade { get; set; }

        /// <summary>
        /// 花色。
        /// </summary>
        [Required]
        [Display(Name = "LotViewModel_Color", ResourceType = typeof(WIPResources.StringResource))]
        public string Color { get; set; }

        /// 花色。
        /// </summary>
        [Required]
        [Display(Name = "LotViewModel_Color", ResourceType = typeof(WIPResources.StringResource))]
        public string LotPath { get; set; }
        
        /// 层压机的设备号
        /// </summary>
        [Required]
        [Display(Name = "LotViewModel_LayerEquipmentNo", ResourceType = typeof(WIPResources.StringResource))]
        public string LotLayerEquipmentNo { get; set; }

        /// <summary>
        /// 检验条码1。
        /// </summary>
        [Required]
        [Display(Name = "LotViewModel_Barcode1", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
               , ErrorMessageResourceName = "ValidateStringLength"
               , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string Barcode1 { get; set; }

        /// <summary>
        /// 检验条码2。
        /// </summary>
        [Required]
        [Display(Name = "LotViewModel_Barcode2", ResourceType = typeof(WIPResources.StringResource))]
        [StringLength(50, MinimumLength = 1
               , ErrorMessageResourceName = "ValidateStringLength"
               , ErrorMessageResourceType = typeof(StringResource))]
        [RegularExpression("[^,]+"
              , ErrorMessageResourceName = "ValidateString"
              , ErrorMessageResourceType = typeof(StringResource))]
        public string Barcode2 { get; set; }
        

        [Display(Name = "LotViewModel_Description", ResourceType = typeof(WIPResources.StringResource))]
        public string Description { get; set; }

        [Display(Name = "LotViewModel_ShowDialog", ResourceType = typeof(WIPResources.StringResource))]
        public bool IsShowDialog { get; set; }

        [Display(Name = "自动分档")]
        public bool IsAutoToBin { get; set; }

        /// <summary>
        /// 取得工步清单
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetRouteOperationList()
        {
            //获取用户拥有权限的工序。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.RouteOperation);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }

            IList<string> lstPackageOperation = new List<string>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = "Key.AttributeName='IsPackageOperation'"
            };
            using (RouteOperationAttributeServiceClient client = new RouteOperationAttributeServiceClient())
            {
                MethodReturnResult<IList<RouteOperationAttribute>> result = client.Get(ref cfg);

                if (result.Code <= 0 && result.Data != null)
                {
                    bool isPackageOperation = false;
                    lstPackageOperation = (from item in result.Data
                                           where bool.TryParse(item.Value, out isPackageOperation) == true
                                                 && isPackageOperation == true
                                           select item.Key.RouteOperationName).ToList();
                }
            }

            IList<RouteOperation> lst = new List<RouteOperation>();
            cfg.Where = "Status=1";
            cfg.OrderBy = "SortSeq";

            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                MethodReturnResult<IList<RouteOperation>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }


            return from item in lst
                   where lstResource.Any(m => m.Data.ToUpper() == item.Key.ToUpper())
                   select new SelectListItem()
                   {
                       Text = item.Key,
                       Value = item.Key
                   };

            //return from item in lst
            //       where lstPackageOperation.Contains(item.Key.ToUpper())==false
            //             && lstResource.Any(m => m.Data.ToUpper() == item.Key.ToUpper())
            //       select new SelectListItem()
            //       {
            //           Text = item.Key,
            //           Value = item.Key
            //       };
        }

        public IEnumerable<SelectListItem> GetProductionLineList()
        {
            //获取用户拥有权限的生产线。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(HttpContext.Current.User.Identity.Name, ResourceType.ProductionLine);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }
            IList<ProductionLine> lst = new List<ProductionLine>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false
            };
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }

            return from item in lst
                   where lstResource.Any(m => m.Data.ToUpper() == item.Key.ToUpper())
                   select new SelectListItem()
                   {
                       Text = string.Format("{0}[{1}]",item.Name,item.Key),
                       Value = item.Key
                   };
        }

        public IEnumerable<SelectListItem> GetGradeList(string lotGrade)
        {
            if(string.IsNullOrEmpty(lotGrade) || lotGrade.Length==0)
            {
                lotGrade = "A";
            }
            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            using (BaseAttributeValueServiceClient client = new BaseAttributeValueServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.CategoryName='Grade' AND Key.AttributeName='VALUE'
                            AND Exists( from BaseAttributeValue as p 
                                        where p.Key.CategoryName='Grade' 
                                           AND p.Key.AttributeName='VALUE' AND p.Value='{0}'
                                        AND self.Key.ItemOrder >= p.Key.ItemOrder
                                   )
                    ", lotGrade),
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
        }
        /// <summary>
        /// 获取颜色_通过抓取设备获取颜色_如果设备没有抓取则人工判断
        /// </summary>
        /// <param name="lotNumber"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetColorList(string lotNumber,string colorOfCell)
        {

            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            string strColorOfModule = colorOfCell;
            if (string.IsNullOrEmpty(colorOfCell) == false)
            {
                IList<BaseAttributeValue> lstValuesColor = new List<BaseAttributeValue>();
                using (BaseAttributeValueServiceClient client1 = new BaseAttributeValueServiceClient())
                {
                    PagingConfig cfg1 = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" Key.AttributeName='MDesc'
                                   and EXISTS (from BaseAttributeValue as p where p.Key.CategoryName='Color_Cell' AND p.Key.AttributeName='CName' and p.Value='{0}'
                                        and self.Key.ItemOrder = p.Key.ItemOrder
                                        and self.Key.CategoryName = p.Key.CategoryName
                                    )", colorOfCell),
                        OrderBy = "Key.ItemOrder"
                    };

                    MethodReturnResult<IList<BaseAttributeValue>> result1 = client1.Get(ref cfg1);
                    if (result1.Code <= 0 && result1.Data != null && result1.Data.Count > 0)
                    {
                        lstValuesColor = result1.Data;
                        strColorOfModule = lstValuesColor.FirstOrDefault().Value;
                    }
                }
            }
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
            List<SelectListItem> listItemValue = new List<SelectListItem>();

            if (lstValues != null && lstValues.Count > 0)
            {
                foreach (BaseAttributeValue item in lstValues)
                {
                   
                    SelectListItem listItem = new SelectListItem();
                    if (strColorOfModule == item.Value)
                    {
                        listItem.Selected = true;
                    }
                    listItem.Text = item.Value;
                    listItem.Value = item.Value; 
                    listItemValue.Add(listItem);
                }
            }
            return listItemValue;
   
            
        }

        /// <summary>
        /// 根据电池片颜色来获取组件的颜色
        /// </summary>
        /// <param name="colorOfCell"></param>
        /// <returns></returns>
        public string GetColorOfModule(string colorOfCell)
        {
            string strColorOfModule = colorOfCell;
            if(string.IsNullOrEmpty(colorOfCell)==false)
            { 
                IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();      
                using (BaseAttributeValueServiceClient client1 = new BaseAttributeValueServiceClient())
                {
                    PagingConfig cfg1 = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" Key.AttributeName='MDesc'
                                   and EXISTS (from BaseAttributeValue as p where p.Key.CategoryName='Color_Cell' AND p.Key.AttributeName='CName' and p.Value='{0}'
                                        and self.Key.ItemOrder = p.Key.ItemOrder
                                        and self.Key.CategoryName = p.Key.CategoryName
                                    )",colorOfCell),
                        OrderBy = "Key.ItemOrder"
                    };

                    MethodReturnResult<IList<BaseAttributeValue>> result1 = client1.Get(ref cfg1);
                    if (result1.Code <= 0 && result1.Data != null && result1.Data.Count>0)
                    {
                        lstValues = result1.Data;
                        strColorOfModule = lstValues.FirstOrDefault().Value;
                    }
                }
            }
            return strColorOfModule;
        }


        public bool RerurnIsExecutePowerset(string lotNumber)
        {
            IList<RouteStepAttribute> lstRouteStepAttribute = new List<RouteStepAttribute>();   //工序属性
            MethodReturnResult<Lot> rst=new MethodReturnResult<Lot>();
            MethodReturnResult result=new MethodReturnResult();
            Lot obj=null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                rst = client.Get(lotNumber);
                if (rst.Code == 0 && rst.Data != null)
                {                    
                    obj = rst.Data;
             
                }
                else
                {
                    result.Code = rst.Code;
                    result.Message = rst.Message;
                    result.Detail = rst.Detail;
                    return false;
                }
            }
            
            
            //获取工序控制属性列表                
            using (RouteStepAttributeServiceClient client = new RouteStepAttributeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.RouteName='{0}' AND Key.RouteStepName='{1}'"
                                            , obj.RouteName
                                            , obj.RouteStepName)
                };
                MethodReturnResult<IList<RouteStepAttribute>> r = client.Get(ref cfg);
                if (r.Code <= 0 && r.Data != null)
                {
                    lstRouteStepAttribute = r.Data;
                }
            }
            //是否显示电流档（利用功率分档显示电流档）yanshan.xiao
            bool IsExecutePowerset = false;
            var lnq = from item in lstRouteStepAttribute
                      where item.Key.AttributeName == "IsExecutePowerset"
                      select item;
            RouteStepAttribute rsaTmp = lnq.FirstOrDefault();
            if (rsaTmp != null)
            {
                bool.TryParse(rsaTmp.Value, out IsExecutePowerset);
            }


            return IsExecutePowerset;
        }

        /// <summary>
        /// 获取IV测试数据
        /// </summary>
        /// <param name="lotNumber"></param>
        /// <returns></returns>
        public IVTestData GetIVTestData(string lotNumber)
        {
            using (IVTestDataServiceClient client = new IVTestDataServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber)
                };
                MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);

                if (result.Code == 0 && result.Data != null & result.Data.Count > 0)
                {
                    return result.Data[0];
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lotNumber"></param>
        /// <returns></returns>
        public string GetLotPath(string lotNumber)
        {
            MethodReturnResult result1 = new MethodReturnResult();
            IList<ColorTestData> corolValues = new List<ColorTestData>();

            IList<BaseAttributeValue> lstValues = new List<BaseAttributeValue>();
            using (ColorTestDataServiceClient client = new ColorTestDataServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}'", lotNumber)
                };
                MethodReturnResult<IList<ColorTestData>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    corolValues = result.Data;
                }
            }
            if (corolValues.Count > 0)
            {
                foreach (ColorTestData item in corolValues)
                {
                     if (item.InspctResult.Equals("NG品"))
                     {
                         //返回不良品图片地址名称
                         return "10.png";
                     }
                     else {
                             try
                             {
                                   InBinParameter p = new InBinParameter()
                                   {
                                       ScanLotNumber = lotNumber
                                   };
                                   using (LotBinServiceClient client = new LotBinServiceClient())
                                   {
                                       result1 = client.PathCheck(p);
                                   }
                                   if (result1.Code < 1000)
                                   {
                                       //返回通过品图片地址名称
                                       return "20.png";
                                   }
                                   else {
                                       //返回档外品图片地址名称
                                       return "30.png";
                                   }                               
                             }
                             catch (Exception ex)
                             {

                                 result1.Code = 1000;
                                 result1.Message = ex.Message;
                                 result1.Detail = ex.ToString();
                                 //返回档外品图片地址名称
                                 return "30.png";
                             }                       
                     }                 
                 }
            }
            else
            {
                //返回不良品图片地址名称
                return "10.png";
            }
            //返回档外品图片地址名称
            return "30.png";
        }
        public string GetPowersetName(string lotNumber, string powersetCode, int itemNo)
        {
            string powerName = string.Empty;
            string cacheKey = string.Format("{0}_{1}", powersetCode, itemNo);
            if (HttpContext.Current.Cache[cacheKey] != null)
            {
                powerName = HttpContext.Current.Cache[cacheKey] as string;
            }
            else
            {
                using (PowersetServiceClient client = new PowersetServiceClient())
                {
                    MethodReturnResult<Powerset> result = client.Get(new PowersetKey()
                    {
                        Code = powersetCode,
                        ItemNo = itemNo
                    });
                    if (result.Code == 0)
                    {
                        powerName = result.Data.PowerName;
                        HttpContext.Current.Cache[cacheKey] = powerName;
                    }
                }
            }
            return powerName;
        }

        public List<string> GetLotDefect(string lotNumber)
        {
            string where = null;
            List<string> XY = new List<string>();
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {


                if (!string.IsNullOrEmpty(lotNumber))
                {
                    where=string.Format(@"  EXISTS( From LotTransaction as p 
                                                      WHERE p.Key=self.Key.TransactionKey 
                                                      AND p.LotNumber='{0}')"
                                        , lotNumber);
                }
                PagingConfig cfg = new PagingConfig()
                {
                    OrderBy = "EditTime",
                    Where = where
                };
                MethodReturnResult<IList<LotTransactionDefect>> result = client.GetLotTransactionDefect(ref cfg);
                if (result.Code == 0)
                {

                    
                    List<LotTransactionDefect> list = result.Data.ToList<LotTransactionDefect>();
                    foreach (var item in list)
                    {
                        StringBuilder lie = new  StringBuilder();
                        using (LotDefectServiceClient LotQueryServiceClient = new LotDefectServiceClient())
                        {
                            MethodReturnResult<DataSet> rt = LotQueryServiceClient.GetXY(item.Key.TransactionKey);
                            if (rt.Code == 0 && rt.Data.Tables[0] != null && rt.Data.Tables[0].Rows.Count>0)
                            {
                                foreach (DataRow i in rt.Data.Tables[0].Rows)
                                {
                                     
                                     lie.Append("  "+i[0].ToString()+";  ");
                                }
                            }
                            XY.Add(item.Key.ReasonCodeName + "   " + lie.ToString());
                            lie = null;
                        }
                    }
                }
            }
            return XY;
        }
    }

    public class LotTrackMaterialViewModel
    {
        public LotTrackMaterialViewModel()
        {
        }

        /// <summary>
        /// 工单号
        /// </summary>
        [Display(Name = "LotTrackMaterialViewModel_OrderNumber", ResourceType = typeof(WIPResources.StringResource))]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 工艺流程
        /// </summary>
        [Display(Name = "LotTrackMaterialViewModel_RouteName", ResourceType = typeof(WIPResources.StringResource))]
        public string RouteName { get; set; }

        /// <summary>
        /// 工序
        /// </summary>        
        [Display(Name = "LotTrackViewModel_RouteOperationName", ResourceType = typeof(WIPResources.StringResource))]        
        public string RouteOperationName { get; set; }
                
        /// <summary>
        /// 设备代码
        /// </summary>
        [Display(Name = "LotTrackViewModel_EquipmentCode", ResourceType = typeof(WIPResources.StringResource))]
        public string EquipmentCode { get; set; }

        /// <summary>
        /// 物料代码
        /// </summary>
        [Display(Name = "LotTrackMaterialViewModel_MaterialCode", ResourceType = typeof(WIPResources.StringResource))]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        [Display(Name = "LotTrackMaterialViewModel_MaterialName", ResourceType = typeof(WIPResources.StringResource))]
        public string MaterialName { get; set; }

        /// <summary>
        /// 批次代码
        /// </summary>
        [Display(Name = "LotTrackMaterialViewModel_MaterialLot", ResourceType = typeof(WIPResources.StringResource))]
        public string MaterialLot { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Display(Name = "LotTrackMaterialViewModel_LoadingQty", ResourceType = typeof(WIPResources.StringResource))]
        public decimal LoadingQty { get; set; }

        /// <summary> 取得工序需扣料清单 </summary>
        /// <param name="routeName">工艺流程</param>
        /// <param name="routeStepName">工序</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetMaterialName(string routeName, string routeStepName)
        {
            IList<RouteStepParameter> lstRouteStepParameter = new List<RouteStepParameter>();

            using (RouteStepParameterServiceClient client = new RouteStepParameterServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "ParamIndex",
                    Where = string.Format(@"DataFrom='{0}' AND IsDeleted=0
                                           AND Key.RouteName='{1}'
                                           AND Key.RouteStepName='{2}'"
                                           , Convert.ToInt32(EnumDataFrom.Manual)
                                           , routeName
                                           , routeStepName)
                };

                MethodReturnResult<IList<RouteStepParameter>> result = client.Get(ref cfg);

                if (result.Code <= 0 && result.Data != null)
                {
                    lstRouteStepParameter = result.Data;
                }
            }

            return from item in lstRouteStepParameter
                   select new SelectListItem()
                   {
                       Text = item.Key.ParameterName,
                       //Value = item.Key.ParameterName
                       Value = item.MaterialType
                   };
        }

        /// <summary>
        /// 取得物料消耗量
        /// </summary>
        /// <param name="materialcode"></param>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public decimal GetMaterialBOMUseLevel(string materialcode, string orderNumber)
        {
            //IList<WorkOrderBOM> lstWorkOrderBOM = new List<WorkOrderBOM>();

            using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.OrderNumber='{0}' 
                                        AND MaterialCode LIKE '{1}%'",
                                        orderNumber,
                                        materialcode)
                };

                MethodReturnResult<IList<WorkOrderBOM>> result = client.Get(ref cfg);

                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    return (decimal)result.Data[0].Qty;       //BOM消耗量                    
                }
                else
                {
                    return 0;
                }
            }              
        }       
    }
}