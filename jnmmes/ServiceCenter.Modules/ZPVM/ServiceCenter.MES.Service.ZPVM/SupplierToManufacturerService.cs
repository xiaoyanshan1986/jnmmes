using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.MES.Service.ZPVM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ServiceCenter.MES.Service.ZPVM
{
    /// <summary>
    /// 实现供应商转换生产厂商规则数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SupplierToManufacturerService : ISupplierToManufacturerContract
    {
        /// <summary>
        /// 供应商转换生产厂商规则数据数据访问读写。
        /// </summary>
        public ISupplierToManufacturerDataEngine SupplierToManufacturerDataEngine { get; set; }       

        /// <summary>
        /// 添加供应商转换生产厂商规则数据。
        /// </summary>
        /// <param name="obj">供应商转换生产厂商规则数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(SupplierToManufacturer obj)
        {
            MethodReturnResult result = new MethodReturnResult();

            PagingConfig cfg = new PagingConfig()
            {
                Where = string.Format(@"Key.MaterialCode ='{0}' 
                                    AND Key.OrderNumber = '{1}'
                                    AND Key.SupplierCode='{2}'"
                                    , obj.Key.MaterialCode
                                    , "*"
                                    , obj.Key.SupplierCode)
            };
            MethodReturnResult<IList<SupplierToManufacturer>> Datas = Gets(ref cfg);
            if (Datas.Data.Count > 0)
            {
                result.Code = 1001;
                result.Message = String.Format("新增规则({0})与规则（{1}:*:{2}）冲突！若要新增，请删除原规则！", obj.Key, obj.Key.MaterialCode, obj.Key.SupplierCode);
                return result;
            }

            if (this.SupplierToManufacturerDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.SupplierToManufacturerService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.SupplierToManufacturerDataEngine.Insert(obj);
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
        /// 修改供应商转换生产厂商规则数据。
        /// </summary>
        /// <param name="obj">供应商转换生产厂商规则数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(SupplierToManufacturer obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.SupplierToManufacturerDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SupplierToManufacturerService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                ISession session = this.SupplierToManufacturerDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.SupplierToManufacturerDataEngine.Update(obj, session);
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
        /// 删除供应商转换生产厂商规则数据。
        /// </summary>
        /// <param name="key">供应商转换生产厂商规则主键。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(SupplierToManufacturerKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.SupplierToManufacturerDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SupplierToManufacturerService_IsNotExists, key);
                return result;
            }
            try
            {
                ISession session = this.SupplierToManufacturerDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.SupplierToManufacturerDataEngine.Delete(key, session);                    
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
        /// 获取供应商转换生产厂商规则数据。
        /// </summary>
        /// <param name="key">供应商转换生产厂商规则主键.</param>
        /// <returns><see cref="MethodReturnResult&lt;SupplierToManufacturer&gt;" />,供应商转换生产厂商规则数据数据.</returns>
        public MethodReturnResult<SupplierToManufacturer> Get(SupplierToManufacturerKey key)
        {
            MethodReturnResult<SupplierToManufacturer> result = new MethodReturnResult<SupplierToManufacturer>();
            if (!this.SupplierToManufacturerDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SupplierToManufacturerService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.SupplierToManufacturerDataEngine.Get(key);
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
        /// 获取供应商转换生产厂商规则数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;SupplierToManufacturer&gt;" />,供应商转换生产厂商规则数据数据集合。</returns>
        public MethodReturnResult<IList<SupplierToManufacturer>> Gets(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<SupplierToManufacturer>> result = new MethodReturnResult<IList<SupplierToManufacturer>>();
            try
            {
                result.Data = this.SupplierToManufacturerDataEngine.Get(cfg);
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
