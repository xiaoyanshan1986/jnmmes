using ServiceCenter.MES.DataAccess.Interface.EDC;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.MES.Service.Contract.EDC;
using ServiceCenter.MES.Service.EDC.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ServiceCenter.MES.Service.EDC
{
    /// <summary>
    /// 实现采集点设置明细管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PointDetailService : IPointDetailContract
    {
        /// <summary>
        /// 采集点设置数据访问读写。
        /// </summary>
        public IPointDataEngine PointDataEngine { get; set; }
        /// <summary>
        /// 采集点设置明细数据访问读写。
        /// </summary>
        public IPointDetailDataEngine PointDetailDataEngine { get; set; }


        /// <summary>
        /// 添加采集点设置明细。
        /// </summary>
        /// <param name="obj">采集点设置明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(PointDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.PointDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.PointDetailService_IsExists, obj.Key);
                return result;
            }
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    Point p = this.PointDataEngine.Get(obj.Key.PointKey);

                    //根据采集设置组名获取记录
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging=false,
                        Where = string.Format("GroupName='{0}'", p.GroupName)
                    };
                    IList<Point> lstPoint = this.PointDataEngine.Get(cfg);

                    foreach(Point item in lstPoint)
                    {
                        PointDetail itemNew = obj.Clone() as PointDetail;
                        itemNew.Key = new PointDetailKey()
                        {
                            PointKey=item.Key,
                            ParameterName=obj.Key.ParameterName
                        };
                        this.PointDetailDataEngine.Insert(itemNew);
                    }
                    this.PointDetailDataEngine.Insert(obj);

                    ts.Complete();
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
        /// 修改采集点设置明细。
        /// </summary>
        /// <param name="obj">采集点设置明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(PointDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PointDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PointDetailService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    Point p = this.PointDataEngine.Get(obj.Key.PointKey);
                    //根据采集设置组名获取记录
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"EXISTS(From Point as p 
                                                       WHERE p.Key=self.Key.PointKey 
                                                       AND p.GroupName='{0}')
                                              AND Key.ParameterName='{1}'"
                                              , p.GroupName
                                              , obj.Key.ParameterName)
                    };
                    IList<PointDetail> lstPointDetail = this.PointDetailDataEngine.Get(cfg);
                    foreach (PointDetail item in lstPointDetail)
                    {
                        PointDetail itemUpdate = obj.Clone() as PointDetail;
                        itemUpdate.Key = new PointDetailKey()
                        {
                            PointKey = item.Key.PointKey,
                            ParameterName = item.Key.ParameterName
                        };
                        itemUpdate.Creator = item.Creator;
                        itemUpdate.CreateTime = item.CreateTime;
                        this.PointDetailDataEngine.Update(itemUpdate);
                    }
                    this.PointDetailDataEngine.Update(obj);
                    ts.Complete();
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
        /// 删除采集点设置明细。
        /// </summary>
        /// <param name="key">采集点设置明细标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(PointDetailKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PointDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PointDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                
                using (TransactionScope ts = new TransactionScope())
                {
                    Point p = this.PointDataEngine.Get(key.PointKey);
                    //根据采集设置组名获取记录
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"EXISTS(From Point as p 
                                                       WHERE p.Key=self.Key.PointKey 
                                                       AND p.GroupName='{0}')
                                                AND Key.ParameterName='{1}'"
                                              , p.GroupName
                                              , key.ParameterName)
                    };
                    IList<PointDetail> lstPointDetail = this.PointDetailDataEngine.Get(cfg);
                    foreach (PointDetail item in lstPointDetail)
                    {
                        this.PointDetailDataEngine.Delete(item.Key);
                    }

                    this.PointDetailDataEngine.Delete(key);

                    ts.Complete();
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
        /// 获取采集点设置明细数据。
        /// </summary>
        /// <param name="key">采集点设置明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;PointDetail&gt;" />,采集点设置明细数据.</returns>
        public MethodReturnResult<PointDetail> Get(PointDetailKey key)
        {
            MethodReturnResult<PointDetail> result = new MethodReturnResult<PointDetail>();
            if (!this.PointDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PointDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.PointDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集点设置明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;PointDetail&gt;" />,采集点设置明细数据集合。</returns>
        public MethodReturnResult<IList<PointDetail>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<PointDetail>> result = new MethodReturnResult<IList<PointDetail>>();
            try
            {
                result.Data = this.PointDetailDataEngine.Get(cfg);
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
