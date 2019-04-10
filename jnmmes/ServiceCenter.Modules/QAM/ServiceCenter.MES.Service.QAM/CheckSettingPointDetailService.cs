using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.QAM;
using ServiceCenter.MES.Model.QAM;
using ServiceCenter.MES.Service.Contract.QAM;
using ServiceCenter.MES.Service.QAM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ServiceCenter.MES.Service.QAM
{
    /// <summary>
    /// 实现检验设置点明细管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CheckSettingPointDetailService : ICheckSettingPointDetailContract
    {
        /// <summary>
        /// 检验设置数据访问读写。
        /// </summary>
        public ICheckSettingDataEngine CheckSettingDataEngine { get; set; }
        /// <summary>
        /// 检验设置点数据访问读写。
        /// </summary>
        public ICheckSettingPointDataEngine CheckSettingPointDataEngine { get; set; }

        /// <summary>
        /// 检验设置点明细数据访问读写。
        /// </summary>
        public ICheckSettingPointDetailDataEngine CheckSettingPointDetailDataEngine { get; set; }


        /// <summary>
        /// 添加检验设置点明细。
        /// </summary>
        /// <param name="obj">检验设置点明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CheckSettingPointDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.CheckSettingPointDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.CheckSettingPointDetailService_IsExists, obj.Key);
                return result;
            }
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.CheckSettingPointDetailDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    //根据检验设置组名获取记录
                    CheckSetting setting = this.CheckSettingDataEngine.Get(obj.Key.CheckSettingKey,session);
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"EXISTS(From CheckSetting as p 
                                                       WHERE p.Key=self.Key.CheckSettingKey 
                                                       AND p.GroupName='{0}')
                                               AND Key.ItemNo='{1}'"
                                              , setting.GroupName
                                              , obj.Key.ItemNo)
                    };
                    IList<CheckSettingPoint> lst = this.CheckSettingPointDataEngine.Get(cfg,session);

                    foreach (CheckSettingPoint item in lst)
                    {
                        CheckSettingPointDetail itemdetailNew = obj.Clone() as CheckSettingPointDetail;
                        itemdetailNew.Key = new CheckSettingPointDetailKey()
                        {
                            CheckSettingKey=item.Key.CheckSettingKey,
                            ItemNo=item.Key.ItemNo,
                            ParameterName=obj.Key.ParameterName
                        };
                        this.CheckSettingPointDetailDataEngine.Insert(itemdetailNew,session);
                    }

                    this.CheckSettingPointDetailDataEngine.Insert(obj,session);

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
        /// 修改检验设置点明细。
        /// </summary>
        /// <param name="obj">检验设置点明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(CheckSettingPointDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckSettingPointDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckSettingPointDetailService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.CheckSettingPointDetailDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    //根据检验设置组名获取记录
                    CheckSetting setting = this.CheckSettingDataEngine.Get(obj.Key.CheckSettingKey,session);
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"EXISTS(From CheckSetting as p 
                                                       WHERE p.Key=self.Key.CheckSettingKey 
                                                       AND p.GroupName='{0}')
                                               AND Key.ItemNo='{1}'
                                               AND Key.ParameterName='{2}'"
                                              , setting.GroupName
                                              , obj.Key.ItemNo
                                              , obj.Key.ParameterName)
                    };
                    IList<CheckSettingPointDetail> lst = this.CheckSettingPointDetailDataEngine.Get(cfg,session);

                    foreach (CheckSettingPointDetail item in lst)
                    {
                        CheckSettingPointDetail itemUpdate = obj.Clone() as CheckSettingPointDetail;
                        itemUpdate.Key = new CheckSettingPointDetailKey()
                        {
                            CheckSettingKey=item.Key.CheckSettingKey,
                            ItemNo=item.Key.ItemNo,
                            ParameterName=item.Key.ParameterName
                        };
                        itemUpdate.CreateTime = item.CreateTime;
                        itemUpdate.Creator = item.Creator;
                        this.CheckSettingPointDetailDataEngine.Update(itemUpdate,session);
                    }

                    this.CheckSettingPointDetailDataEngine.Update(obj,session);

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
        /// 删除检验设置点明细。
        /// </summary>
        /// <param name="key">检验设置点明细标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(CheckSettingPointDetailKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckSettingPointDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckSettingPointDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.CheckSettingPointDetailDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    //根据检验设置组名获取记录
                    CheckSetting setting = this.CheckSettingDataEngine.Get(key.CheckSettingKey);
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"EXISTS(From CheckSetting as p 
                                                       WHERE p.Key=self.Key.CheckSettingKey 
                                                       AND p.GroupName='{0}')
                                               AND Key.ItemNo='{1}'
                                               AND Key.ParameterName='{2}'"
                                              , setting.GroupName
                                              , key.ItemNo
                                              , key.ParameterName)
                    };
                    IList<CheckSettingPointDetail> lst = this.CheckSettingPointDetailDataEngine.Get(cfg,session);

                    foreach (CheckSettingPointDetail item in lst)
                    {
                        this.CheckSettingPointDetailDataEngine.Delete(item.Key,session);
                    }

                    this.CheckSettingPointDetailDataEngine.Delete(key,session);

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
        /// 获取检验设置点明细数据。
        /// </summary>
        /// <param name="key">检验设置点明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckSettingPointDetail&gt;" />,检验设置点明细数据.</returns>
        public MethodReturnResult<CheckSettingPointDetail> Get(CheckSettingPointDetailKey key)
        {
            MethodReturnResult<CheckSettingPointDetail> result = new MethodReturnResult<CheckSettingPointDetail>();
            if (!this.CheckSettingPointDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckSettingPointDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.CheckSettingPointDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取检验设置点明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckSettingPointDetail&gt;" />,检验设置点明细数据集合。</returns>
        public MethodReturnResult<IList<CheckSettingPointDetail>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<CheckSettingPointDetail>> result = new MethodReturnResult<IList<CheckSettingPointDetail>>();
            try
            {
                result.Data = this.CheckSettingPointDetailDataEngine.Get(cfg);
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
