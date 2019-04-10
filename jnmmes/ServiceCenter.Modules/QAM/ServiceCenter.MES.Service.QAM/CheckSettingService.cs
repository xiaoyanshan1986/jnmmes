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
    /// 实现检验设置管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CheckSettingService : ICheckSettingContract
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
        /// 添加检验设置。
        /// </summary>
        /// <param name="obj">检验设置数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CheckSetting obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.CheckSettingDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.CheckSettingService_IsExists, obj.Key);
                return result;
            }
            try
            {
               // using (TransactionScope ts = new TransactionScope())
                ISession session = this.CheckSettingDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    //新增检验设置。
                    this.CheckSettingDataEngine.Insert(obj,session);
                    //根据检验设置组名获取记录
                    PagingConfig cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format("GroupName='{0}'", obj.GroupName)
                    };
                    IList<CheckSetting> lst = this.CheckSettingDataEngine.Get(cfg,session);
                    IList<CheckSettingPoint> lstPoint = new List<CheckSettingPoint>();
                    if (lst.Count > 0)
                    {
                        CheckSetting p = lst[0];
                        obj.ActionName = p.ActionName;
                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format("Key.CheckSettingKey='{0}'", p.Key)
                        };
                        lstPoint = this.CheckSettingPointDataEngine.Get(cfg,session);
                    }
                    //曾经有设置过检验设置，则直接复制原有检验设置参数组。
                    if (lstPoint.Count > 0)
                    {
                        foreach (CheckSettingPoint item in lstPoint)
                        {
                            //复制检验参数组。
                            CheckSettingPoint itemNew = item.Clone() as CheckSettingPoint;
                            itemNew.Key = new CheckSettingPointKey()
                            {
                                CheckSettingKey = obj.Key,
                                ItemNo = item.Key.ItemNo
                            };
                            this.CheckSettingPointDataEngine.Modify(itemNew);
                            //复制检验参数组中的检验参数。
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.CheckSettingKey='{0}' AND Key.ItemNo='{1}'"
                                                       , item.Key.CheckSettingKey
                                                       , item.Key.ItemNo)
                            };
                            IList<CheckSettingPointDetail> lstPointDetail = this.CheckSettingPointDetailDataEngine.Get(cfg,session);
                            foreach(CheckSettingPointDetail itemDetail in lstPointDetail)
                            {
                                CheckSettingPointDetail itemDetailNew = itemDetail.Clone() as CheckSettingPointDetail;
                                itemDetailNew.Key = new CheckSettingPointDetailKey()
                                {
                                    CheckSettingKey = itemNew.Key.CheckSettingKey,
                                    ItemNo = itemNew.Key.ItemNo,
                                    ParameterName = itemDetail.Key.ParameterName
                                };
                                this.CheckSettingPointDetailDataEngine.Modify(itemDetailNew,session);
                            }
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
        /// 修改检验设置。
        /// </summary>
        /// <param name="obj">检验设置数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(CheckSetting obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckSettingDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckSettingService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.CheckSettingDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    //根据检验设置组名获取记录
                    PagingConfig cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format("GroupName='{0}'", obj.GroupName)
                    };
                    IList<CheckSetting> lst = this.CheckSettingDataEngine.Get(cfg,session);
                    if (lst.Count > 0)
                    {
                        CheckSetting p = lst[0];
                        obj.ActionName = p.ActionName;
                    }

                    this.CheckSettingDataEngine.Update(obj,session);

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
        /// 删除检验设置。
        /// </summary>
        /// <param name="key">检验设置标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckSettingDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckSettingService_IsNotExists, key);
                return result;
            }
            try
            {
                this.CheckSettingDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取检验设置数据。
        /// </summary>
        /// <param name="key">检验设置标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckSetting&gt;" />,检验设置数据.</returns>
        public MethodReturnResult<CheckSetting> Get(string key)
        {
            MethodReturnResult<CheckSetting> result = new MethodReturnResult<CheckSetting>();
            if (!this.CheckSettingDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckSettingService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.CheckSettingDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取检验设置数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckSetting&gt;" />,检验设置数据集合。</returns>
        public MethodReturnResult<IList<CheckSetting>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<CheckSetting>> result = new MethodReturnResult<IList<CheckSetting>>();
            try
            {
                result.Data = this.CheckSettingDataEngine.Get(cfg);
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
