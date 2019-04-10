using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.MES.Service.ZPVM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ServiceCenter.MES.Service.ZPVM
{
    /// <summary>
    /// 实现工单规则设置数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkOrderRuleService : IWorkOrderRuleContract
    {
        public ISessionFactory SessionFactory
        {
            get;
            set;
        }

        public WorkOrderRuleService(ISessionFactory sf)
        {
            this.SessionFactory = sf;
        }

        /// <summary>
        /// 工单规则设置数据数据访问读写。
        /// </summary>
        public IWorkOrderRuleDataEngine WorkOrderRuleDataEngine { get; set; }
        /// <summary>
        /// 规则数据数据访问读写。
        /// </summary>
        public IRuleDataEngine RuleDataEngine { get; set; }
        /// <summary>
        /// 分档数据数据访问读写。
        /// </summary>
        public IPowersetDataEngine PowersetDataEngine { get; set; }
        /// <summary>
        /// 子分档数据数据访问读写。
        /// </summary>
        public IPowersetDetailDataEngine PowersetDetailDataEngine { get; set; }

        /// <summary>
        /// 工单分档设置数据数据访问读写。
        /// </summary>
        public IWorkOrderPowersetDataEngine WorkOrderPowersetDataEngine { get; set; }
        /// <summary>
        /// 工单子分档设置数据数据访问读写。
        /// </summary>
        public IWorkOrderPowersetDetailDataEngine WorkOrderPowersetDetailDataEngine { get; set; }


        /// <summary>
        /// 规则-控制参数对象设置数据数据访问读写。
        /// </summary>
        public IRuleControlObjectDataEngine RuleControlObjectDataEngine { get; set; }

        /// <summary>
        /// 工单控制参数设置数据数据访问读写。
        /// </summary>
        public IWorkOrderControlObjectDataEngine WorkOrderControlObjectDataEngine { get; set; }


        /// <summary>
        /// 规则-衰减设置数据数据访问读写。
        /// </summary>
        public IRuleDecayDataEngine RuleDecayDataEngine { get; set; }
        /// <summary>
        /// 工单衰减设置数据数据访问读写。
        /// </summary>
        public IWorkOrderDecayDataEngine WorkOrderDecayDataEngine { get; set; }

        /// <summary>
        /// 规则-等级设置数据数据访问读写。
        /// </summary>
        public IRuleGradeDataEngine RuleGradeDataEngine { get; set; }
        /// <summary>
        /// 工单等级设置数据数据访问读写。
        /// </summary>
        public IWorkOrderGradeDataEngine WorkOrderGradeDataEngine { get; set; }

        /// <summary>
        /// 规则-标签打印设置数据数据访问读写。
        /// </summary>
        public IRulePrintSetDataEngine RulePrintSetDataEngine { get; set; }

        /// <summary>
        /// 工单打印标签设置数据数据访问读写。
        /// </summary>
        public IWorkOrderPrintSetDataEngine WorkOrderPrintSetDataEngine { get; set; }

        public MethodReturnResult Add(WorkOrderRule obj)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.WorkOrderRuleDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.WorkOrderRuleService_IsExists, obj.Key);
                return result;
            }

            Rule rule = this.RuleDataEngine.Get(obj.RuleCode);
            if (rule == null)
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RuleService_IsNotExists, obj.RuleCode);
                return result;
            }
            #region //新增工单分档规则。
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format("Key.Code='{0}' AND IsUsed=1", rule.PowersetCode)
            };


            IList<Powerset> lstPowerset = this.PowersetDataEngine.Get(cfg);
            List<WorkOrderPowerset> lstWorkOrderPowerset = new List<WorkOrderPowerset>();
            List<WorkOrderPowersetDetail> lstWorkOrderPowersetDetail = new List<WorkOrderPowersetDetail>();

            foreach (Powerset item in lstPowerset)
            {
                WorkOrderPowerset wps = new WorkOrderPowerset()
                {
                    Key = new WorkOrderPowersetKey()
                    {
                        OrderNumber = obj.Key.OrderNumber,
                        MaterialCode = obj.Key.MaterialCode,
                        Code = item.Key.Code,
                        ItemNo = item.Key.ItemNo
                    },
                    EditTime = obj.EditTime,
                    Editor = obj.Editor,
                    Creator = obj.Creator,
                    CreateTime = obj.CreateTime,
                    SubWay = item.SubWay,
                    ArticleNo = item.ArticleNo,
                    Description = item.Description,
                    IsUsed = item.IsUsed,
                    MaxValue = item.MaxValue,
                    MinValue = item.MinValue,
                    StandardIPM = item.StandardIPM,
                    Name = item.Name,
                    PowerDifference = item.PowerDifference,
                    PowerName = item.PowerName,
                    StandardFuse = item.StandardFuse,
                    StandardIsc = item.StandardIsc,
                    StandardPower = item.StandardPower,
                    StandardVoc = item.StandardVoc,
                    StandardVPM = item.StandardVPM
                };
                lstWorkOrderPowerset.Add(wps);

                if (item.SubWay != EnumPowersetSubWay.None)
                {
                    PagingConfig cfg1 = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.Code='{0}' AND Key.ItemNo='{1}' AND IsUsed=1"
                                                , item.Key.Code
                                                , item.Key.ItemNo)
                    };
                    IList<PowersetDetail> lstPowersetDetail = this.PowersetDetailDataEngine.Get(cfg1);
                    foreach (PowersetDetail itemDetail in lstPowersetDetail)
                    {
                        WorkOrderPowersetDetail wpd = new WorkOrderPowersetDetail()
                        {
                            Key = new WorkOrderPowersetDetailKey()
                            {
                                OrderNumber = obj.Key.OrderNumber,
                                MaterialCode = obj.Key.MaterialCode,
                                Code = item.Key.Code,
                                ItemNo = item.Key.ItemNo,
                                SubCode = itemDetail.Key.SubCode
                            },
                            CreateTime = obj.CreateTime,
                            Creator = obj.Creator,
                            Editor = obj.Editor,
                            EditTime = obj.EditTime,
                            IsUsed = true,
                            MaxValue = itemDetail.MaxValue,
                            MinValue = itemDetail.MinValue,
                            SubName = itemDetail.SubName,
                            Picture = itemDetail.Picture
                        };

                        lstWorkOrderPowersetDetail.Add(wpd);
                    }
                }
            }
            #endregion

            #region //新增工单控制参数
            cfg.Where = string.Format("Key.Code='{0}' AND IsUsed=1", rule.Key);
            IList<RuleControlObject> lstRuleControlObject = this.RuleControlObjectDataEngine.Get(cfg);

            List<WorkOrderControlObject> lstWorkOrderControlObject = new List<WorkOrderControlObject>();
            foreach (RuleControlObject item in lstRuleControlObject)
            {
                WorkOrderControlObject wco = new WorkOrderControlObject()
                {
                    Key = new WorkOrderControlObjectKey()
                    {
                        OrderNumber = obj.Key.OrderNumber,
                        MaterialCode = obj.Key.MaterialCode,
                        Object = item.Key.Object,
                        Type = item.Key.Type
                    },
                    EditTime = obj.EditTime,
                    Editor = obj.Editor,
                    Creator = obj.Creator,
                    CreateTime = obj.CreateTime,
                    IsUsed = item.IsUsed,
                    Value = item.Value
                };
                lstWorkOrderControlObject.Add(wco);
            }
            #endregion

            #region //新增工单衰减设置
            cfg.Where = string.Format("Key.Code='{0}' AND IsUsed=1", rule.Key);
            IList<RuleDecay> lstRuleDecay = this.RuleDecayDataEngine.Get(cfg);

            List<WorkOrderDecay> lstWorkOrderDecay = new List<WorkOrderDecay>();
            foreach (RuleDecay item in lstRuleDecay)
            {
                WorkOrderDecay wd = new WorkOrderDecay()
                {
                    Key = new WorkOrderDecayKey()
                    {
                        OrderNumber = obj.Key.OrderNumber,
                        MaterialCode = obj.Key.MaterialCode,
                        MaxPower = item.Key.MaxPower,
                        MinPower = item.Key.MinPower
                    },
                    EditTime = obj.EditTime,
                    Editor = obj.Editor,
                    Creator = obj.Creator,
                    CreateTime = obj.CreateTime,
                    IsUsed = item.IsUsed,
                    DecayCode = item.DecayCode
                };
                lstWorkOrderDecay.Add(wd);
            }
            #endregion

            #region //新增工单等级设置
            cfg.Where = string.Format("Key.Code='{0}' AND IsUsed=1", rule.Key);
            IList<RuleGrade> lstRuleGrade = this.RuleGradeDataEngine.Get(cfg);

            List<WorkOrderGrade> lstWorkOrderGrade = new List<WorkOrderGrade>();
            foreach (RuleGrade item in lstRuleGrade)
            {
                WorkOrderGrade wg = new WorkOrderGrade()
                {
                    Key = new WorkOrderGradeKey()
                    {
                        OrderNumber = obj.Key.OrderNumber,
                        MaterialCode = obj.Key.MaterialCode,
                        Grade = item.Key.Grade
                    },
                    EditTime = obj.EditTime,
                    Editor = obj.Editor,
                    Creator = obj.Creator,
                    CreateTime = obj.CreateTime,
                    IsUsed = item.IsUsed,
                    ItemNo = item.ItemNo,
                    MixColor = item.MixColor,
                    MixPowerset = item.MixPowerset,
                    MixSubPowerset = item.MixSubPowerset,
                    PackageGroup = item.PackageGroup
                };
                lstWorkOrderGrade.Add(wg);
            }
            #endregion

            #region //新增工单标签设置
            cfg.Where = string.Format("Key.Code='{0}' AND IsUsed=1", rule.Key);
            IList<RulePrintSet> lstRulePrintSet = this.RulePrintSetDataEngine.Get(cfg);

            List<WorkOrderPrintSet> lstWorkOrderPrintSet = new List<WorkOrderPrintSet>();
            foreach (RulePrintSet item in lstRulePrintSet)
            {
                WorkOrderPrintSet wp = new WorkOrderPrintSet()
                {
                    Key = new WorkOrderPrintSetKey()
                    {
                        OrderNumber = obj.Key.OrderNumber,
                        MaterialCode = obj.Key.MaterialCode,
                        LabelCode = item.Key.LabelCode
                    },
                    EditTime = obj.EditTime,
                    Editor = obj.Editor,
                    Creator = obj.Creator,
                    CreateTime = obj.CreateTime,
                    IsUsed = item.IsUsed,
                    ItemNo = item.ItemNo,
                    Qty = item.Qty
                };
                lstWorkOrderPrintSet.Add(wp);
            }
            #endregion

            #region 事务
            ISession db = this.SessionFactory.OpenSession();
            ITransaction transaction = db.BeginTransaction();
            try
            {
                this.WorkOrderRuleDataEngine.Insert(obj, db);

                if (lstWorkOrderPowerset.Count > 0)
                {
                    foreach (WorkOrderPowerset item in lstWorkOrderPowerset)
                    {
                        this.WorkOrderPowersetDataEngine.Insert(item, db);
                    }
                }

                if (lstWorkOrderPowersetDetail.Count > 0)
                {
                    foreach (WorkOrderPowersetDetail item in lstWorkOrderPowersetDetail)
                    {
                        this.WorkOrderPowersetDetailDataEngine.Insert(item, db);
                    }
                }

                if (lstWorkOrderControlObject.Count > 0)
                {
                    foreach (WorkOrderControlObject item in lstWorkOrderControlObject)
                    {
                        this.WorkOrderControlObjectDataEngine.Insert(item, db);
                    }
                }

                if (lstWorkOrderDecay.Count > 0)
                {
                    foreach (WorkOrderDecay item in lstWorkOrderDecay)
                    {
                        this.WorkOrderDecayDataEngine.Insert(item, db);
                    }
                }

                if (lstWorkOrderGrade.Count > 0)
                {
                    foreach (WorkOrderGrade item in lstWorkOrderGrade)
                    {
                        this.WorkOrderGradeDataEngine.Insert(item, db);
                    }
                }

                if (lstWorkOrderPrintSet.Count > 0)
                {
                    foreach (WorkOrderPrintSet item in lstWorkOrderPrintSet)
                    {
                        this.WorkOrderPrintSetDataEngine.Insert(item, db);
                    }
                }

                transaction.Commit();
                db.Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message += String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 添加工单规则设置数据。
        /// </summary>
        /// <param name="obj">工单规则设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        //public MethodReturnResult Add(WorkOrderRule obj)
        //{
        //    MethodReturnResult result = new MethodReturnResult();
        //    if (this.WorkOrderRuleDataEngine.IsExists(obj.Key))
        //    {
        //        result.Code = 1001;
        //        result.Message = String.Format(StringResource.WorkOrderRuleService_IsExists, obj.Key);
        //        return result;
        //    }
        //    try
        //    {
        //        //判断规则代码是否存在。
        //        Rule rule = this.RuleDataEngine.Get(obj.RuleCode);
        //        if (rule == null)
        //        {
        //            result.Code = 1001;
        //            result.Message = String.Format(StringResource.RuleService_IsNotExists, obj.RuleCode);
        //            return result;
        //        }

        //        using(TransactionScope ts=new TransactionScope())
        //        {
        //            this.WorkOrderRuleDataEngine.Insert(obj);

        //            PagingConfig cfg = new PagingConfig()
        //            {
        //                IsPaging = false,
        //                Where = string.Format("Key.Code='{0}' AND IsUsed=1", rule.PowersetCode)
        //            };

        //            #region //新增工单分档规则。
        //            IList<Powerset> lstPowerset = this.PowersetDataEngine.Get(cfg);

        //            foreach (Powerset item in lstPowerset)
        //            {
        //                WorkOrderPowerset wps = new WorkOrderPowerset()
        //                {
        //                    Key = new WorkOrderPowersetKey()
        //                    {
        //                        OrderNumber=obj.Key.OrderNumber,
        //                        MaterialCode=obj.Key.MaterialCode,
        //                        Code=item.Key.Code,
        //                        ItemNo=item.Key.ItemNo
        //                    },
        //                    EditTime=obj.EditTime,
        //                    Editor=obj.Editor,
        //                    Creator=obj.Creator,
        //                    CreateTime=obj.CreateTime,
        //                    SubWay=item.SubWay,
        //                    ArticleNo=item.ArticleNo,
        //                    Description=item.Description,
        //                    MixColor =item.MixColor,
        //                    IsUsed=item.IsUsed,
        //                    MaxValue=item.MaxValue,
        //                    MinValue=item.MinValue,
        //                    StandardIPM=item.StandardIPM,
        //                    Name=item.Name,
        //                    PowerDifference=item.PowerDifference,
        //                    PowerName=item.PowerName,
        //                    StandardFuse=item.StandardFuse,
        //                    StandardIsc=item.StandardIsc,
        //                    StandardPower=item.StandardPower,
        //                    StandardVoc=item.StandardVoc,
        //                    StandardVPM=item.StandardVPM
        //                };
        //                this.WorkOrderPowersetDataEngine.Insert(wps);

        //                #region //新增子分档数据
        //                if (item.SubWay != EnumPowersetSubWay.None)
        //                {
        //                    PagingConfig cfg1 = new PagingConfig()
        //                    {
        //                        IsPaging = false,
        //                        Where = string.Format("Key.Code='{0}' AND Key.ItemNo='{1}' AND IsUsed=1"
        //                                                , item.Key.Code
        //                                                , item.Key.ItemNo)
        //                    };
        //                    IList<PowersetDetail> lstPowersetDetail = this.PowersetDetailDataEngine.Get(cfg1);
        //                    foreach (PowersetDetail itemDetail in lstPowersetDetail)
        //                    {
        //                        WorkOrderPowersetDetail wpd = new WorkOrderPowersetDetail()
        //                        {
        //                            Key = new WorkOrderPowersetDetailKey()
        //                            {
        //                                OrderNumber = obj.Key.OrderNumber,
        //                                MaterialCode = obj.Key.MaterialCode,
        //                                Code = item.Key.Code,
        //                                ItemNo = item.Key.ItemNo,
        //                                SubCode = itemDetail.Key.SubCode
        //                            },
        //                            CreateTime = obj.CreateTime,
        //                            Creator = obj.Creator,
        //                            Editor = obj.Editor,
        //                            EditTime = obj.EditTime,
        //                            IsUsed = true,
        //                            MaxValue = itemDetail.MaxValue,
        //                            MinValue = itemDetail.MinValue,
        //                            SubName = itemDetail.SubName,
        //                            Picture=itemDetail.Picture
        //                        };

        //                        this.WorkOrderPowersetDetailDataEngine.Insert(wpd);
        //                    }
        //                }
        //                #endregion
        //            }
        //            #endregion

        //            #region //新增工单控制参数
        //            cfg.Where = string.Format("Key.Code='{0}' AND IsUsed=1", rule.Key);
        //            IList<RuleControlObject> lstRuleControlObject = this.RuleControlObjectDataEngine.Get(cfg);
        //            foreach (RuleControlObject item in lstRuleControlObject)
        //            {
        //                WorkOrderControlObject wco = new WorkOrderControlObject()
        //                {
        //                    Key = new WorkOrderControlObjectKey()
        //                    {
        //                        OrderNumber = obj.Key.OrderNumber,
        //                        MaterialCode = obj.Key.MaterialCode,
        //                        Object=item.Key.Object,
        //                        Type=item.Key.Type
        //                    },
        //                    EditTime = obj.EditTime,
        //                    Editor = obj.Editor,
        //                    Creator = obj.Creator,
        //                    CreateTime = obj.CreateTime,
        //                    IsUsed=item.IsUsed,
        //                    Value=item.Value
        //                };
        //                this.WorkOrderControlObjectDataEngine.Insert(wco);
        //            }
        //            #endregion

        //            #region //新增工单衰减设置
        //            cfg.Where = string.Format("Key.Code='{0}' AND IsUsed=1", rule.Key);
        //            IList<RuleDecay> lstRuleDecay = this.RuleDecayDataEngine.Get(cfg);
        //            foreach (RuleDecay item in lstRuleDecay)
        //            {
        //                WorkOrderDecay wd = new WorkOrderDecay()
        //                {
        //                    Key = new WorkOrderDecayKey()
        //                    {
        //                        OrderNumber = obj.Key.OrderNumber,
        //                        MaterialCode = obj.Key.MaterialCode,
        //                        MaxPower=item.Key.MaxPower,
        //                        MinPower=item.Key.MinPower
        //                    },
        //                    EditTime = obj.EditTime,
        //                    Editor = obj.Editor,
        //                    Creator = obj.Creator,
        //                    CreateTime = obj.CreateTime,
        //                    IsUsed = item.IsUsed,
        //                    DecayCode=item.DecayCode
        //                };
        //                this.WorkOrderDecayDataEngine.Insert(wd);
        //            }
        //            #endregion

        //            #region //新增工单等级设置
        //            cfg.Where = string.Format("Key.Code='{0}' AND IsUsed=1", rule.Key);
        //            IList<RuleGrade> lstRuleGrade = this.RuleGradeDataEngine.Get(cfg);
        //            foreach (RuleGrade item in lstRuleGrade)
        //            {
        //                WorkOrderGrade wg = new WorkOrderGrade()
        //                {
        //                    Key = new WorkOrderGradeKey()
        //                    {
        //                        OrderNumber = obj.Key.OrderNumber,
        //                        MaterialCode = obj.Key.MaterialCode,
        //                        Grade=item.Key.Grade
        //                    },
        //                    EditTime = obj.EditTime,
        //                    Editor = obj.Editor,
        //                    Creator = obj.Creator,
        //                    CreateTime = obj.CreateTime,
        //                    IsUsed = item.IsUsed,
        //                    ItemNo=item.ItemNo,
        //                    MixColor=item.MixColor,
        //                    MixPowerset=item.MixPowerset,
        //                    MixSubPowerset=item.MixSubPowerset,
        //                    PackageGroup=item.PackageGroup
        //                };
        //                this.WorkOrderGradeDataEngine.Insert(wg);
        //            }
        //            #endregion

        //            #region //新增工单标签设置
        //            cfg.Where = string.Format("Key.Code='{0}' AND IsUsed=1", rule.Key);
        //            IList<RulePrintSet> lstRulePrintSet = this.RulePrintSetDataEngine.Get(cfg);
        //            foreach (RulePrintSet item in lstRulePrintSet)
        //            {
        //                WorkOrderPrintSet wp = new WorkOrderPrintSet()
        //                {
        //                    Key = new WorkOrderPrintSetKey()
        //                    {
        //                        OrderNumber = obj.Key.OrderNumber,
        //                        MaterialCode = obj.Key.MaterialCode,
        //                        LabelCode=item.Key.LabelCode
        //                    },
        //                    EditTime = obj.EditTime,
        //                    Editor = obj.Editor,
        //                    Creator = obj.Creator,
        //                    CreateTime = obj.CreateTime,
        //                    IsUsed = item.IsUsed,
        //                    ItemNo = item.ItemNo,
        //                    Qty=item.Qty
        //                };
        //                this.WorkOrderPrintSetDataEngine.Insert(wp);
        //            }
        //            #endregion

        //            ts.Complete();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Code = 1000;
        //        result.Message = String.Format(StringResource.Error,ex.Message);
        //        result.Detail = ex.ToString();
        //    }
        //    return result;
        //}


        /// <summary>
        /// 修改工单规则设置数据。
        /// </summary>
        /// <param name="obj">工单规则设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(WorkOrderRule obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderRuleDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderRuleService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderRuleDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// 删除工单规则设置数据。
        /// </summary>
        /// <param name="key">工单规则设置数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderRuleKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderRuleDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderRuleService_IsNotExists, key);
                return result;
            }
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.WorkOrderRuleDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.WorkOrderRuleDataEngine.Delete(key,session);

                    string condition = string.Format("Key.OrderNumber='{0}' AND Key.MaterialCode='{1}'"
                                                   , key.OrderNumber, key.MaterialCode);

                    this.WorkOrderControlObjectDataEngine.DeleteByCondition(condition,session);
                    this.WorkOrderDecayDataEngine.DeleteByCondition(condition,session);
                    this.WorkOrderGradeDataEngine.DeleteByCondition(condition,session);
                    this.WorkOrderPowersetDetailDataEngine.DeleteByCondition(condition,session);
                    this.WorkOrderPowersetDataEngine.DeleteByCondition(condition,session);
                    this.WorkOrderPrintSetDataEngine.DeleteByCondition(condition,session);
                    //ts.Complete();
                    transaction.Commit();
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取工单规则设置数据数据。
        /// </summary>
        /// <param name="key">工单规则设置数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderRule&gt;" />,工单规则设置数据数据.</returns>
        public MethodReturnResult<WorkOrderRule> Get(WorkOrderRuleKey key)
        {
            MethodReturnResult<WorkOrderRule> result = new MethodReturnResult<WorkOrderRule>();
            if (!this.WorkOrderRuleDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderRuleService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.WorkOrderRuleDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取工单规则设置数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderRule&gt;" />,工单规则设置数据数据集合。</returns>
        public MethodReturnResult<IList<WorkOrderRule>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WorkOrderRule>> result = new MethodReturnResult<IList<WorkOrderRule>>();
            try
            {
                result.Data = this.WorkOrderRuleDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
    }
}
