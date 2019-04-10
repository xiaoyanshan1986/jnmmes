using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Contract.PPM;
using ServiceCenter.MES.Service.PPM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ServiceCenter.MES.Service.PPM
{
    /// <summary>
    /// 实现工单管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkOrderService : IWorkOrderContract
    {
        /// <summary>
        /// 工单数据访问读写。
        /// </summary>
        public IWorkOrderDataEngine WorkOrderDataEngine { get; set; }

        /// <summary>
        /// 物料数据访问读写。
        /// </summary>
        public IMaterialDataEngine MaterialDataEngine { get; set; }

        /// <summary>
        /// 物料类型工艺数据访问对象。
        /// </summary>
        public IMaterialRouteDataEngine MaterialRouteDataEngine { get; set; }

        /// <summary>
        /// 物料属性数据访问对象。
        /// </summary>
        public IMaterialAttributeDataEngine MaterialAttributeDataEngine { get; set; }

        /// <summary>
        /// 物料类型工艺数据访问对象。
        /// </summary>
        public IMaterialTypeRouteDataEngine MaterialTypeRouteDataEngine { get; set; }

        /// <summary>
        /// 工艺流程组明细访问对象。
        /// </summary>
        public IRouteEnterpriseDetailDataEngine RouteEnterpriseDetailDataEngine { get; set; }

        /// <summary>
        /// 工步数据访问对象。
        /// </summary>
        public IRouteStepDataEngine RouteStepDataEngine { get; set; }
        /// <summary>
        /// 工单产品数据访问对象。
        /// </summary>
        public IWorkOrderProductDataEngine WorkOrderProductDataEngine { get; set; }

        /// <summary>
        /// 工单工艺数据访问对象。
        /// </summary>
        public IWorkOrderRouteDataEngine WorkOrderRouteDataEngine { get; set; }

        /// <summary>
        /// 工单属性数据访问对象
        /// </summary>
        public IWorkOrderAttributeDataEngine WorkOrderAttributeDataEngine { get; set; }

        /// <summary>
        /// 物料BOM数据访问对象。
        /// </summary>
        public IMaterialBOMDataEngine MaterialBOMDataEngine { get; set; }

        /// <summary>
        /// 工单BOM数据访问对象。
        /// </summary>
        public IWorkOrderBOMDataEngine WorkOrderBOMDataEngine { get; set; }

        /// <summary>
        /// 工艺流程访问类
        /// </summary>
        public IRouteDataEngine RouteDataEngine { get; set; }

        /// <summary> 新增工单 </summary>
        /// <param name="obj">工单对象</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrder obj)
        {
            MethodReturnResult result = new MethodReturnResult();

            List<WorkOrderRoute> lstWorkOrderRouteForInsert = new List<WorkOrderRoute>();
            List<WorkOrderAttribute> lstWorkOrderAttributeForInsert = new List<WorkOrderAttribute>();
            List<WorkOrderBOM> lstWorkOrderBOMForInsert = new List<WorkOrderBOM>();
            ITransaction transaction = null;
            ISession session = null;
            PagingConfig cfg = null;

            try
            {
                //检验工单是否存在
                if (this.WorkOrderDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.WorkOrderService_IsExists, obj.Key);
                    return result;
                }

                //取得产品对象
                Material material = this.MaterialDataEngine.Get(obj.MaterialCode);

                if (material == null || material.IsProduct == false)
                {
                    result.Code = 1003;
                    result.Message = String.Format(StringResource.WorkOrderService_MaterialCodeIsNotExists, obj.MaterialCode);
                    return result;
                }

                #region 1.工单工艺流程设置(根据产品设置取得对应的工艺流程)
                if (obj.OrderType == "1")
                {
                    //1.取得产品对应工艺流程设置
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.MaterialCode = '{0}' AND Key.LocationName = '{1}'",
                                                obj.MaterialCode,
                                                obj.LocationName)
                    };

                    IList<MaterialRoute> lstMaterialRoute = this.MaterialRouteDataEngine.Get(cfg);

                    if (lstMaterialRoute.Count > 0)
                    {
                        int j = 1;      //工单工艺流程项目号计数器

                        foreach (MaterialRoute mRoute in lstMaterialRoute)
                        {
                            //取得工艺流程组对应的工艺流程
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format(@"Key.RouteEnterpriseName = '{0}'"
                                    , mRoute.Key.RouteEnterpriseName)
                            };

                            IList<RouteEnterpriseDetail> lstRouteEnterpriseDetail = this.RouteEnterpriseDetailDataEngine.Get(cfg);

                            if (lstRouteEnterpriseDetail.Count > 0)
                            {
                                //循环取得工艺流程
                                foreach (RouteEnterpriseDetail routeEnterpriseDetail in lstRouteEnterpriseDetail)
                                {
                                    //取得工艺流程第一个工步对象
                                    cfg = new PagingConfig()
                                    {
                                        IsPaging = false,
                                        Where = string.Format(@"Key.RouteName = '{0}'"
                                            , routeEnterpriseDetail.Key.RouteName),
                                        OrderBy = "SortSeq"
                                    };

                                    IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg);

                                    if (lstRouteStep.Count > 0)
                                    {
                                        //取得工艺流程对象,取得工艺流程类型（主流程、返修流程）
                                        Route route = this.RouteDataEngine.Get(routeEnterpriseDetail.Key.RouteName);

                                        if (route == null)
                                        {
                                            result.Code = 2001;
                                            result.Message = string.Format("工艺流程（{0}）不存在！"
                                                                            , lstRouteStep[0].Key.RouteName);
                                            return result;
                                        }

                                        bool isRework = false;

                                        if (route.RouteType == EnumRouteType.Repair)
                                        {
                                            isRework = true;
                                        }

                                        //创建工单工艺流程
                                        WorkOrderRoute woRoute = new WorkOrderRoute()
                                        {
                                            Key = new WorkOrderRouteKey()
                                            {
                                                OrderNumber = obj.Key,
                                                ItemNo = j
                                            },
                                            RouteEnterpriseName = routeEnterpriseDetail.Key.RouteEnterpriseName,
                                            RouteName = lstRouteStep[0].Key.RouteName,
                                            RouteStepName = lstRouteStep[0].Key.RouteStepName,
                                            CreateTime = obj.CreateTime,
                                            Creator = obj.Creator,
                                            Editor = obj.Editor,
                                            EditTime = obj.EditTime,
                                            IsRework = isRework
                                        };

                                        lstWorkOrderRouteForInsert.Add(woRoute);

                                        j++;
                                    }
                                }
                            }
                        }
                    }
                }



                //int i = 1;
                
                //foreach (MaterialRoute mr in lstMaterialRoute)
                //{
                //    //根据工艺流程获取工步。
                //    WorkOrderRoute wor = new WorkOrderRoute()
                //    {
                //        Key = new WorkOrderRouteKey()
                //        {
                //            OrderNumber = obj.Key,
                //            ItemNo = i
                //        },
                //        RouteEnterpriseName = mr.Key.RouteEnterpriseName,                        
                //        CreateTime = obj.CreateTime,
                //        Creator = obj.Creator,
                //        Editor = obj.Editor,
                //        EditTime = obj.EditTime,

                //        RouteName = "",
                //        RouteStepName = "",
                //        IsRework = false
                //    };

                //    //this.WorkOrderRouteDataEngine.Insert(wor);
                //    lstWorkOrderRouteForInsert.Add(wor);

                //    i++;
                //}












                #endregion

                #region 2.设置工单产品
                WorkOrderProduct workOrderProduct = new WorkOrderProduct()
                {
                    Key = new WorkOrderProductKey()
                    {
                        OrderNumber = obj.Key,
                        MaterialCode = obj.MaterialCode
                    },
                    IsMain = true,
                    ItemNo = 0,
                    CreateTime = obj.CreateTime,
                    Creator = obj.Creator,
                    Editor = obj.Editor,
                    EditTime = obj.EditTime
                };
                #endregion

                #region 3.工单属性设置(根据产品设置取得对应属性)
                //1.取得产品对应属性设置
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.MaterialCode = '{0}'",
                                            obj.MaterialCode)
                };

                IList<MaterialAttribute> lstMaterialAttribute = this.MaterialAttributeDataEngine.Get(cfg);

                foreach (MaterialAttribute ma in lstMaterialAttribute)
                {
                    //创建工单属性列表
                    WorkOrderAttribute wora = new WorkOrderAttribute()
                    {
                        Key = new WorkOrderAttributeKey()
                        {
                            OrderNumber = obj.Key,
                            AttributeName = ma.Key.AttributeName
                        },
                        AttributeValue = ma.Value,                        
                        Editor = obj.Editor,
                        EditTime = obj.EditTime
                    };

                    lstWorkOrderAttributeForInsert.Add(wora);
                }
                #endregion

                #region 4.设置工单BOM
                cfg.IsPaging = false;
                cfg.Where = string.Format("Key.MaterialCode = '{0}'", obj.MaterialCode);
                cfg.OrderBy = "Key.ItemNo";

                IList<MaterialBOM> lstMaterialBOM = this.MaterialBOMDataEngine.Get(cfg);

                foreach (MaterialBOM item in lstMaterialBOM)
                {
                    WorkOrderBOM woBOM = new WorkOrderBOM()
                    {
                        Key = new WorkOrderBOMKey()
                        {
                            OrderNumber = obj.Key,
                            ItemNo = item.Key.ItemNo
                        },
                        MaterialCode = item.RawMaterialCode,
                        MaterialUnit = item.MaterialUnit,
                        Qty = Convert.ToDecimal(item.Qty),
                        StoreLocation = item.StoreLocation,
                        WorkCenter = item.WorkCenter,
                        CreateTime = obj.CreateTime,
                        Creator = obj.Creator,
                        Description = item.Description,
                        Editor = obj.Editor,
                        EditTime = obj.EditTime
                    };

                    lstWorkOrderBOMForInsert.Add(woBOM);
                }
                #endregion

                #region 5.分档规程（待定）

                #endregion

                #region 6.等级设置（待定）

                #endregion

                #region 7.设置工单规则（根据产品）（待定）

                #endregion

                #region 开始事物处理
                session = this.WorkOrderDataEngine.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                //1. 新增工单
                this.WorkOrderDataEngine.Insert(obj, session);

                //2.工单工艺流程
                foreach (WorkOrderRoute wor in lstWorkOrderRouteForInsert)
                {
                    this.WorkOrderRouteDataEngine.Insert(wor, session);
                }

                //3.设置工单产品
                this.WorkOrderProductDataEngine.Insert(workOrderProduct, session);

                //4.工单属性
                foreach (WorkOrderAttribute wora in lstWorkOrderAttributeForInsert)
                {
                    this.WorkOrderAttributeDataEngine.Insert(wora, session);
                }

                //5.工单BOM
                foreach (WorkOrderBOM wBOM in lstWorkOrderBOMForInsert)
                {
                    this.WorkOrderBOMDataEngine.Insert(wBOM, session);
                }

                transaction.Commit();
                session.Close();

                #endregion


                ////using (TransactionScope ts = new TransactionScope())
                //ISession session = this.WorkOrderDataEngine.SessionFactory.OpenSession();
                //ITransaction transaction = session.BeginTransaction();
                //{
                //    //新增工单。
                //    this.WorkOrderDataEngine.Insert(obj,session);

                //    //设置工单工艺。
                //    //获取物料类型工艺。
                //    PagingConfig cfg = new PagingConfig()
                //    {
                //        IsPaging = false,
                //        Where = string.Format("Key.MaterialType='{0}' AND Key.LocationName='{1}'", material.Type, obj.LocationName)
                //    };
                //    IList<MaterialTypeRoute> lstRouteEnterprise = this.MaterialTypeRouteDataEngine.Get(cfg,session);
                //    int i = 1;
                //    foreach (MaterialTypeRoute mtr in lstRouteEnterprise)
                //    {
                //        //根据工艺流程组获取工艺流程名称。
                //        cfg.PageNo = 0;
                //        cfg.PageSize = 1;
                //        cfg.Where = string.Format("Key.RouteEnterpriseName='{0}'", mtr.Key.RouteEnterpriseName);
                //        cfg.OrderBy = "ItemNo";

                //        IList<RouteEnterpriseDetail> lstRoute = this.RouteEnterpriseDetailDataEngine.Get(cfg,session);
                //        string routeName = lstRoute.Count > 0 ? lstRoute[0].Key.RouteName : string.Empty;
                //        //根据工艺流程名称获取第一个工步名称。
                //        cfg.Where = string.Format("Key.RouteName='{0}'", routeName);
                //        cfg.OrderBy = "SortSeq";
                //        IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg,session);
                //        string routeStepName = lstRouteStep.Count > 0 ? lstRouteStep[0].Key.RouteStepName : string.Empty;
                //        //根据工艺流程获取工步。
                //        WorkOrderRoute wor = new WorkOrderRoute()
                //        {
                //            Key = new WorkOrderRouteKey()
                //            {
                //                OrderNumber = obj.Key,
                //                ItemNo = i
                //            },
                //            RouteEnterpriseName = mtr.Key.RouteEnterpriseName,
                //            RouteName = routeName,
                //            RouteStepName = routeStepName,
                //            CreateTime = obj.CreateTime,
                //            Creator = obj.Creator,
                //            Editor = obj.Editor,
                //            EditTime = obj.EditTime,
                //            IsRework = mtr.Key.IsRework
                //        };
                //        this.WorkOrderRouteDataEngine.Insert(wor,session);
                //        i++;
                //    }
                //    //设置工单产品。
                //    WorkOrderProduct wop = new WorkOrderProduct()
                //    {
                //        Key = new WorkOrderProductKey()
                //        {
                //            OrderNumber = obj.Key,
                //            MaterialCode = obj.MaterialCode
                //        },
                //        IsMain = true,
                //        ItemNo = 0,
                //        CreateTime = obj.CreateTime,
                //        Creator = obj.Creator,
                //        Editor = obj.Editor,
                //        EditTime = obj.EditTime
                //    };
                //    this.WorkOrderProductDataEngine.Insert(wop,session);
                //    //设置工单BOM
                //    cfg.IsPaging = false;
                //    cfg.Where = string.Format("Key.MaterialCode = '{0}'",obj.MaterialCode);
                //    cfg.OrderBy = "Key.ItemNo";
                //    IList<MaterialBOM> lstMaterialBOM = this.MaterialBOMDataEngine.Get(cfg,session);
                //    foreach (MaterialBOM item in lstMaterialBOM)
                //    {
                //        WorkOrderBOM woBOM = new WorkOrderBOM()
                //        {
                //            Key = new WorkOrderBOMKey()
                //            {
                //                OrderNumber = obj.Key,
                //                ItemNo = item.Key.ItemNo
                //            },
                //            MaterialCode=item.RawMaterialCode,
                //            MaterialUnit=item.MaterialUnit,
                //            Qty= Convert.ToDecimal( item.Qty),
                //            StoreLocation=item.StoreLocation,
                //            WorkCenter=item.WorkCenter,
                //            CreateTime=obj.CreateTime,
                //            Creator=obj.Creator,
                //            Description=item.Description,
                //            Editor=obj.Editor,
                //            EditTime=obj.EditTime
                //        };
                //        this.WorkOrderBOMDataEngine.Insert(woBOM,session);
                //    }

                //    //ts.Complete();
                //    transaction.Commit();
                //    session.Close();
                //}
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);

                if (transaction != null)
                {
                    transaction.Rollback();
                    session.Close();
                }      
            }
            return result;
        }
        
        /// <summary>
        /// 修改工单。
        /// </summary>
        /// <param name="obj">工单数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(WorkOrder obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (!this.WorkOrderDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1002;
                    result.Message = String.Format(StringResource.WorkOrderService_IsNotExists, obj.Key);
                    return result;
                }

                if (!this.MaterialDataEngine.IsExists(obj.MaterialCode))
                {
                    result.Code = 1003;
                    result.Message = String.Format(StringResource.WorkOrderService_MaterialCodeIsNotExists, obj.MaterialCode);
                    return result;
                }

                this.WorkOrderDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 删除工单。
        /// </summary>
        /// <param name="key">工单标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (!this.WorkOrderDataEngine.IsExists(key))
                {
                    result.Code = 1002;
                    result.Message = String.Format(StringResource.WorkOrderService_IsNotExists, key);
                    return result;
                }

                //判断工单是否已经投产


                this.WorkOrderDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工单数据。
        /// </summary>
        /// <param name="key">工单标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrder&gt;" />,工单数据.</returns>
        public MethodReturnResult<WorkOrder> Get(string key)
        {
            MethodReturnResult<WorkOrder> result = new MethodReturnResult<WorkOrder>();
            try
            {
                result.Data = this.WorkOrderDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工单数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrder&gt;" />,工单数据集合。</returns>
        public MethodReturnResult<IList<WorkOrder>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WorkOrder>> result = new MethodReturnResult<IList<WorkOrder>>();
            try
            {
                result.Data = this.WorkOrderDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
    }
}
