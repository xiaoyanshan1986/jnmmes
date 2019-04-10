using ServiceCenter.MES.DataAccess.Interface.EDC;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.MES.Model.FMM;
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
    /// 实现采集点设置管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PointService : IPointContract
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
        /// 采集参数数据访问读写。
        /// </summary>
        public ICategoryDetailDataEngine CategoryDetailDataEngine { get; set; }

        /// <summary>
        /// 参数数据访问读写。
        /// </summary>
        public IParameterDataEngine ParameterDataEngine { get; set; }

        /// <summary>
        /// 添加采集点设置。
        /// </summary>
        /// <param name="obj">采集点设置数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Point obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.PointDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.PointService_IsExists, obj.Key);
                return result;
            }
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //根据采集设置组名获取记录
                    PagingConfig cfg = new PagingConfig()
                    {
                        PageNo=0,
                        PageSize=1,
                        Where = string.Format("GroupName='{0}'", obj.GroupName)
                    };
                    IList<Point> lstPoint = this.PointDataEngine.Get(cfg);
                    IList<PointDetail> lstPointDetail = new List<PointDetail>();
                    if(lstPoint.Count>0)
                    {
                        Point p = lstPoint[0];
                        obj.SamplingPlanName = p.SamplingPlanName;
                        obj.CategoryName = p.CategoryName;
                        obj.ActionName = p.ActionName;
                        cfg = new PagingConfig()
                        {
                            IsPaging=false,
                            Where = string.Format("Key.PointKey='{0}'", p.Key)
                        };
                        lstPointDetail = this.PointDetailDataEngine.Get(cfg);
                    }
                    //新增采集点设置。
                    this.PointDataEngine.Insert(obj);

                    //曾经有设置过该组采集点，则直接复制原有采集参数点设置。
                    if (lstPointDetail.Count > 0)
                    {
                        foreach (PointDetail item in lstPointDetail)
                        {
                            PointDetail itemNew = item.Clone() as PointDetail;
                            itemNew.Key = new PointDetailKey()
                            {
                                PointKey=obj.Key,
                                ParameterName=item.Key.ParameterName
                            };
                            this.PointDetailDataEngine.Insert(itemNew);
                        }
                    }
                    else
                    {//之前没有设置采集参数明细，则插入参数。
                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format("Key.CategoryName='{0}'", obj.CategoryName)
                        };
                        IList<CategoryDetail> lstDetail = this.CategoryDetailDataEngine.Get(cfg);
                        foreach (CategoryDetail item in lstDetail)
                        {
                            Parameter p = this.ParameterDataEngine.Get(item.Key.ParameterName);

                            PointDetail pointdetailItem = new PointDetail()
                            {
                                Key = new PointDetailKey()
                                {
                                    ParameterName = item.Key.ParameterName,
                                    PointKey = obj.Key
                                },
                                CreateTime = obj.CreateTime,
                                Creator = obj.Creator,
                                DataType = p != null ? p.DataType : EnumDataType.String,
                                DerivedFormula = p != null ? p.DerivedFormula : string.Empty,
                                DeviceType = p != null ? p.DeviceType : EnumDeviceType.None,
                                Editor = obj.Editor,
                                EditTime = obj.EditTime,
                                IsDerived = p != null ? p.IsDerived : false,
                                ItemNo = item.ItemNo,
                                Mandatory = p != null ? p.Mandatory : true,
                                ParameterCount = 1,
                                ParameterType = EnumParameterType.EDC
                            };
                            this.PointDetailDataEngine.Insert(pointdetailItem);
                        }
                    }
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
        /// 修改采集点设置。
        /// </summary>
        /// <param name="obj">采集点设置数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Point obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PointDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PointService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //根据采集设置组名获取记录
                    PagingConfig cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format("GroupName='{0}' AND Key!='{1}'"
                                              , obj.GroupName
                                              , obj.Key)
                    };
                    IList<Point> lstPoint = this.PointDataEngine.Get(cfg);
                    if (lstPoint.Count > 0)
                    {
                        Point p = lstPoint[0];
                        obj.SamplingPlanName = p.SamplingPlanName;
                        obj.CategoryName = p.CategoryName;
                        obj.ActionName = p.ActionName;
                    }

                    this.PointDataEngine.Update(obj);

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
        /// 删除采集点设置。
        /// </summary>
        /// <param name="key">采集点设置标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PointDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PointService_IsNotExists, key);
                return result;
            }
            try
            {
                this.PointDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集点设置数据。
        /// </summary>
        /// <param name="key">采集点设置标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Point&gt;" />,采集点设置数据.</returns>
        public MethodReturnResult<Point> Get(string key)
        {
            MethodReturnResult<Point> result = new MethodReturnResult<Point>();
            if (!this.PointDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PointService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.PointDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集点设置数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Point&gt;" />,采集点设置数据集合。</returns>
        public MethodReturnResult<IList<Point>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Point>> result = new MethodReturnResult<IList<Point>>();
            try
            {
                result.Data = this.PointDataEngine.Get(cfg);
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
