using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Contract.FMM;
using ServiceCenter.MES.Service.FMM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ServiceCenter.MES.Service.FMM
{
    /// <summary>
    /// 实现工步管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RouteStepService : IRouteStepContract
    {
        /// <summary>
        /// 工步数据访问读写。
        /// </summary>
        public IRouteStepDataEngine RouteStepDataEngine { get; set; }
        public IRouteOperationParameterDataEngine RouteOperationParameterDataEngine { get; set; }
        public IRouteStepParameterDataEngine RouteStepParameterDataEngine { get; set; }

        /// <summary>
        /// 添加工步。
        /// </summary>
        /// <param name="obj">工步数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RouteStep obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RouteStepDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RouteStepService_IsExists, obj.Key);
                return result;
            }
            try
            {

                //获取序号大于当前新增工步序号的工步对象。
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "SortSeq",
                    Where = string.Format("Key.RouteName='{0}' AND SortSeq>='{1}'"
                                            , obj.Key.RouteName, obj.SortSeq)
                };
                IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg);

                //using(TransactionScope ts=new TransactionScope())
                ISession session = this.RouteStepDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    for(int i=0;i<lstRouteStep.Count;i++)
                    {
                        RouteStep step = lstRouteStep[i];
                        step.SortSeq = obj.SortSeq + i+1;
                        step.Editor = obj.Editor;
                        step.EditTime=obj.EditTime;
                        //更新原有工步对象。
                        this.RouteStepDataEngine.Update(step,session);
                    }
                    //新增工步对象。
                    this.RouteStepDataEngine.Insert(obj,session);
                    //根据工序参数新增工步参数。
                    if (this.RouteOperationParameterDataEngine != null && this.RouteStepParameterDataEngine!=null)
                    {
                        cfg = new PagingConfig() { 
                            IsPaging=false,
                            Where = string.Format("Key.RouteOperationName='{0}' AND IsDeleted=false", obj.RouteOperationName)
                        };
                        IList<RouteOperationParameter> lstRouteOperationParameter = this.RouteOperationParameterDataEngine.Get(cfg);
                        foreach (RouteOperationParameter p in lstRouteOperationParameter)
                        {
                            RouteStepParameter rsParameter = new RouteStepParameter()
                            {
                                Key = new RouteStepParameterKey()
                                {
                                    RouteName=obj.Key.RouteName,
                                    RouteStepName=obj.Key.RouteStepName,
                                    ParameterName=p.Key.ParameterName
                                },
                                DataFrom=p.DataFrom,
                                DataType=p.DataType,
                                DCType=p.DCType,
                                MaterialType=p.MaterialType,
                                IsDeleted=p.IsDeleted,
                                IsMustInput=p.IsMustInput,
                                IsReadOnly=p.IsReadOnly,
                                Editor=obj.Editor,
                                EditTime=obj.EditTime,
                                IsUsePreValue = p.IsUsePreValue,
                                ParamIndex=p.ParamIndex,
                                ValidateFailedMessage=p.ValidateFailedMessage,
                                ValidateFailedRule=p.ValidateFailedRule,
                                ValidateRule=p.ValidateRule
                            };
                            this.RouteStepParameterDataEngine.Insert(rsParameter,session);
                        }
                    }
                    //ts.Complete();
                    transaction.Commit();
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工步。
        /// </summary>
        /// <param name="obj">工步数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(RouteStep obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            
            try
            {
                //校验工步是否已经设置
                if (!this.RouteStepDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1002;
                    result.Message = String.Format(StringResource.RouteStepService_IsNotExists, obj.Key);
                    return result;
                }

                //判断序号是否重复
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "SortSeq",
                    Where = string.Format("Key.RouteName='{0}' AND Key.RouteStepName!='{1}' and SortSeq = {2} "
                                            , obj.Key.RouteName
                                            , obj.Key.RouteStepName
                                            , obj.SortSeq)
                };

                IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg);

                if (lstRouteStep.Count > 0)
                {
                    result.Code = 1000;
                    result.Message = String.Format("序号{0}重复！请重新设置。", obj.SortSeq);
                    return result;
                }

                //数据更新
                ISession session = this.RouteStepDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                
                this.RouteStepDataEngine.Update(obj, session);
               
                transaction.Commit();
                session.Close();
                





                ////获取序号大于当前新增工步序号的工步对象。
                //PagingConfig cfg = new PagingConfig()
                //{
                //    IsPaging = false,
                //    OrderBy = "SortSeq",
                //    Where = string.Format("Key.RouteName='{0}' AND Key.RouteStepName!='{1}'"
                //                            , obj.Key.RouteName
                //                            , obj.Key.RouteStepName)
                //};
                //IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg);

                ////using (TransactionScope ts = new TransactionScope())
                //ISession session = this.RouteStepDataEngine.SessionFactory.OpenSession();
                //ITransaction transaction = session.BeginTransaction();
                //{
                //    var lnqRouteStep = from item in lstRouteStep
                //              where item.SortSeq < obj.SortSeq
                //              select item;
                //    if (lnqRouteStep.Count() == 0)
                //    {
                //        obj.SortSeq=1;
                //    }
                //    lnqRouteStep = from item in lstRouteStep
                //                   where item.SortSeq >= obj.SortSeq
                //                   select item;
                //    int i = 1;
                //    foreach(RouteStep step in lnqRouteStep)
                //    {
                //        step.SortSeq = obj.SortSeq + i;
                //        step.Editor = obj.Editor;
                //        step.EditTime = obj.EditTime;
                //        //更新原有工步对象。
                //        this.RouteStepDataEngine.Update(step,session);
                //        i++;
                //    }
                //    this.RouteStepDataEngine.Update(obj,session);
                //    //ts.Complete();
                //    transaction.Commit();
                //    session.Close();
                //}
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工步。
        /// </summary>
        /// <param name="key">工步名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(RouteStepKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteStepDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteStepService_IsNotExists, key);
                return result;
            }
            try
            {
                RouteStep obj = this.RouteStepDataEngine.Get(key);
                 //获取序号大于当前删除工步序号的工步对象。
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "SortSeq",
                    Where = string.Format("Key.RouteName='{0}' AND SortSeq>'{1}'"
                                            , obj.Key.RouteName, obj.SortSeq)
                };
                IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg);

                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.RouteStepDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    for (int i = 0; i < lstRouteStep.Count; i++)
                    {
                        RouteStep step = lstRouteStep[i];
                        step.SortSeq = obj.SortSeq + i;
                        step.Editor = obj.Editor;
                        step.EditTime = obj.EditTime;
                        //更新原有工步对象。
                        this.RouteStepDataEngine.Update(step,session);
                    }
                    this.RouteStepDataEngine.Delete(key,session);
                    //ts.Complete();
                    transaction.Commit();
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工步数据。
        /// </summary>
        /// <param name="key">工步名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteStep&gt;" />,工步数据.</returns>
        public MethodReturnResult<RouteStep> Get(RouteStepKey key)
        {
            MethodReturnResult<RouteStep> result = new MethodReturnResult<RouteStep>();
            if (!this.RouteStepDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteStepService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RouteStepDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工步数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteStep&gt;" />,工步数据集合。</returns>
        public MethodReturnResult<IList<RouteStep>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<RouteStep>> result = new MethodReturnResult<IList<RouteStep>>();
            try
            {
                result.Data = this.RouteStepDataEngine.Get(cfg);
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
