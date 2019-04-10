using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.QAM;
using ServiceCenter.MES.Model.FMM;
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
    /// 实现检验设置点管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CheckSettingPointService : ICheckSettingPointContract
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
        /// 检验参数数据访问读写。
        /// </summary>
        public ICheckCategoryDetailDataEngine CheckCategoryDetailDataEngine { get; set; }

        /// <summary>
        /// 参数数据访问读写。
        /// </summary>
        public IParameterDataEngine ParameterDataEngine { get; set; }

        /// <summary>
        /// 添加检验设置点。
        /// </summary>
        /// <param name="obj">检验设置点数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CheckSettingPoint obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.CheckSettingPointDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.CheckSettingPointService_IsExists, obj.Key);
                return result;
            }
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.CheckSettingPointDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.CheckSettingPointDataEngine.Insert(obj);
                    //获取检验参数组参数明细。
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.CategoryName='{0}'", obj.CategoryName)
                    };
                    IList<CheckCategoryDetail> lstDetail = this.CheckCategoryDetailDataEngine.Get(cfg,session);
                    //设置检验设置点明细数据。
                    IList<CheckSettingPointDetail> lstPointDetail = new List<CheckSettingPointDetail>();
                    foreach (CheckCategoryDetail item in lstDetail)
                    {
                        Parameter p = this.ParameterDataEngine.Get(item.Key.ParameterName,session);

                        CheckSettingPointDetail pointdetailItem = new CheckSettingPointDetail()
                        {
                            Key = new CheckSettingPointDetailKey()
                            {
                                CheckSettingKey=obj.Key.CheckSettingKey,
                                ItemNo=obj.Key.ItemNo,
                                ParameterName = item.Key.ParameterName
                            },
                            CreateTime = obj.CreateTime,
                            Creator = obj.Creator,
                            DataType = p != null ? p.DataType : EnumDataType.String,
                            DerivedFormula = p != null ? p.DerivedFormula : string.Empty,
                            DeviceType = p != null ? p.DeviceType : EnumDeviceType.None,
                            Editor = obj.Editor,
                            EditTime = obj.EditTime,
                            IsDerived = p != null ? p.IsDerived : false,
                            Mandatory = p != null ? p.Mandatory : true,
                            ParameterCount = 1,
                            ParameterType = EnumParameterType.Check,
                            ParameterItemNo=item.ItemNo
                        };
                        lstPointDetail.Add(pointdetailItem);
                        this.CheckSettingPointDetailDataEngine.Insert(pointdetailItem,session);
                    }
                    //根据检验设置组名获取记录
                    CheckSetting setting = this.CheckSettingDataEngine.Get(obj.Key.CheckSettingKey,session);
                    cfg = new PagingConfig()
                    {
                        IsPaging=false,
                        Where = string.Format("GroupName='{0}' AND Key!='{1}'", setting.GroupName,setting.Key)
                    };
                    IList<CheckSetting> lst = this.CheckSettingDataEngine.Get(cfg,session);
                    foreach(CheckSetting item in lst)
                    {
                        CheckSettingPoint point = obj.Clone() as CheckSettingPoint;
                        point.Key = new CheckSettingPointKey()
                        {
                            CheckSettingKey=item.Key,
                            ItemNo=obj.Key.ItemNo
                        };
                        this.CheckSettingPointDataEngine.Insert(point,session);

                        foreach(CheckSettingPointDetail detailItem in lstPointDetail)
                        {
                            CheckSettingPointDetail detailItemNew = detailItem.Clone() as CheckSettingPointDetail;
                            detailItemNew.Key = new CheckSettingPointDetailKey()
                            {
                                CheckSettingKey=point.Key.CheckSettingKey,
                                ItemNo=point.Key.ItemNo,
                                ParameterName=detailItem.Key.ParameterName
                            };
                            this.CheckSettingPointDetailDataEngine.Insert(detailItemNew,session);
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
        /// 修改检验设置点。
        /// </summary>
        /// <param name="obj">检验设置点数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(CheckSettingPoint obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckSettingPointDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckSettingPointService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.CheckSettingPointDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.CheckSettingPointDataEngine.Update(obj,session);
                    
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
                        CheckSettingPoint itemUpdate = item.Clone() as CheckSettingPoint;
                        itemUpdate.CategoryName = obj.CategoryName;
                        itemUpdate.CheckPlanName = obj.CheckPlanName;
                        itemUpdate.Editor = obj.Editor;
                        itemUpdate.EditTime = obj.EditTime;
                        itemUpdate.Status = obj.Status;
                        this.CheckSettingPointDataEngine.Update(itemUpdate,session);
                    }
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
        /// 删除检验设置点。
        /// </summary>
        /// <param name="key">检验设置点标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(CheckSettingPointKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckSettingPointDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckSettingPointService_IsNotExists, key);
                return result;
            }
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.CheckSettingPointDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.CheckSettingPointDataEngine.Delete(key,session);

                    //根据检验设置组名获取记录
                    CheckSetting setting = this.CheckSettingDataEngine.Get(key.CheckSettingKey,session);
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"EXISTS(From CheckSetting as p 
                                                       WHERE p.Key=self.Key.CheckSettingKey 
                                                       AND p.GroupName='{0}')
                                               AND Key.ItemNo='{1}'"
                                              , setting.GroupName
                                              , key.ItemNo)
                    };
                    IList<CheckSettingPoint> lst = this.CheckSettingPointDataEngine.Get(cfg,session);
                    foreach (CheckSettingPoint item in lst)
                    {
                        this.CheckSettingPointDataEngine.Delete(item.Key,session);
                    }
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
        /// 获取检验设置点数据。
        /// </summary>
        /// <param name="key">检验设置点标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckSettingPoint&gt;" />,检验设置点数据.</returns>
        public MethodReturnResult<CheckSettingPoint> Get(CheckSettingPointKey key)
        {
            MethodReturnResult<CheckSettingPoint> result = new MethodReturnResult<CheckSettingPoint>();
            if (!this.CheckSettingPointDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckSettingPointService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.CheckSettingPointDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取检验设置点数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckSettingPoint&gt;" />,检验设置点数据集合。</returns>
        public MethodReturnResult<IList<CheckSettingPoint>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<CheckSettingPoint>> result = new MethodReturnResult<IList<CheckSettingPoint>>();
            try
            {
                result.Data = this.CheckSettingPointDataEngine.Get(cfg);
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
