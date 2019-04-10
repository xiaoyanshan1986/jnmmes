using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.DataAccess.Interface.ZPVC;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.ZPVC;
using ServiceCenter.MES.Service.Contract.ZPVC;
using ServiceCenter.MES.Service.ZPVC.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ServiceCenter.MES.Service.ZPVC
{
    /// <summary>
    /// 实现包装信息数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PackageInfoService : IPackageInfoContract
    {
        /// <summary>
        /// 包装数据数据访问读写。
        /// </summary>
        public IPackageDataEngine PackageDataEngine { get; set; }
        /// <summary>
        /// 包装信息数据数据访问读写。
        /// </summary>
        public IPackageInfoDataEngine PackageInfoDataEngine { get; set; }

        /// <summary>
        /// 添加包装信息数据。
        /// </summary>
        /// <param name="obj">包装信息数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Package p, PackageInfo obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(p.Key))
                {
                    p.Key = p.Key.ToUpper();
                }

                if (!string.IsNullOrEmpty(obj.Key))
                {
                    obj.Key = obj.Key.ToUpper();
                }

                Package packageObj = this.PackageDataEngine.Get(p.Key??string.Empty);
                if (packageObj!=null && packageObj.PackageState == EnumPackageState.Packaged)
                {
                    result.Message = string.Format("包 {0} 已成箱，不能修改。",p.Key);
                    result.Code = 1001;
                    return result;
                }

                //using(TransactionScope ts=new TransactionScope())
                ISession session = this.PackageDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {   
                    this.PackageDataEngine.Modify(p,session);
                    this.PackageInfoDataEngine.Modify(obj,session);
                    //ts.Complete();
                    transaction.Commit();
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error,ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// 修改包装信息数据。
        /// </summary>
        /// <param name="obj">包装信息数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(PackageInfo obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PackageInfoDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PackageInfoService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                if (!string.IsNullOrEmpty(obj.Key))
                {
                    obj.Key = obj.Key.ToUpper();
                }

                this.PackageInfoDataEngine.Update(obj);
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
        /// 删除包装信息数据。
        /// </summary>
        /// <param name="key">包装信息数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PackageInfoDataEngine.IsExists(key??string.Empty))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PackageInfoService_IsNotExists, key);
                return result;
            }
            try
            {
                Package packageObj = this.PackageDataEngine.Get(key??string.Empty);
                if (packageObj != null && packageObj.PackageState == EnumPackageState.Packaged)
                {
                    result.Message = string.Format("包 {0} 已成箱，不能删除。", key);
                    result.Code = 1001;
                    return result;
                }

                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.PackageDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.PackageDataEngine.Delete(key??string.Empty);
                    this.PackageInfoDataEngine.Delete(key ?? string.Empty);
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
        /// 获取包装信息数据数据。
        /// </summary>
        /// <param name="key">包装信息数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;PackageInfo&gt;" />,包装信息数据数据.</returns>
        public MethodReturnResult<PackageInfo> Get(string key)
        {
            MethodReturnResult<PackageInfo> result = new MethodReturnResult<PackageInfo>();
            if (!this.PackageInfoDataEngine.IsExists(key??string.Empty))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PackageInfoService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.PackageInfoDataEngine.Get(key??string.Empty);
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
        /// 获取包装信息数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;PackageInfo&gt;" />,包装信息数据数据集合。</returns>
        public MethodReturnResult<IList<PackageInfo>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<PackageInfo>> result = new MethodReturnResult<IList<PackageInfo>>();
            try
            {
                result.Data = this.PackageInfoDataEngine.Get(cfg);
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
