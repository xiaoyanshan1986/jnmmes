using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.ZPVM
{
    /// <summary>
    /// 定义供应商转换生产厂商规则数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface ISupplierToManufacturerContract 
    {
         /// <summary>
         /// 添加供应商转换生产厂商规则。
         /// </summary>
         /// <param name="obj">供应商转换生产厂商规则数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(SupplierToManufacturer obj);
         /// <summary>
         /// 修改供应商转换生产厂商规则。
         /// </summary>
         /// <param name="obj">供应商转换生产厂商规则数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(SupplierToManufacturer obj);
         /// <summary>
         /// 删除供应商转换生产厂商规则。
         /// </summary>
         /// <param name="key">供应商转换生产厂商规则主键.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(SupplierToManufacturerKey key);
         /// <summary>
         /// 获取供应商转换生产厂商规则。
         /// </summary>
         /// <param name="key">供应商转换生产厂商规则主键.</param>
         /// <returns>MethodReturnResult&lt;SupplierToManufacturer&gt;，供应商转换生产厂商规则数据.</returns>
         [OperationContract]
         MethodReturnResult<SupplierToManufacturer> Get(SupplierToManufacturerKey key);
         /// <summary>
         /// 获取供应商转换生产厂商规则。
         /// </summary>
         /// <param name="cfg">查询供应商转换生产厂商规则数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;SupplierToManufacturer&gt;&gt;，供应商转换生产厂商规则数据集合。</returns>
         [OperationContract]
         MethodReturnResult<IList<SupplierToManufacturer>> Gets(ref PagingConfig cfg);
    }
}
