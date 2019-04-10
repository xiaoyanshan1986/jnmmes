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
    /// 定义产品控制对象设置数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IProductControlObjectContract
    {
         /// <summary>
         /// 添加产品控制对象设置数据。
         /// </summary>
         /// <param name="obj">产品控制对象设置数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(ProductControlObject obj);
         /// <summary>
         /// 修改产品控制对象设置数据。
         /// </summary>
         /// <param name="obj">产品控制对象设置数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(ProductControlObject obj);
         /// <summary>
         /// 删除产品控制对象设置数据。
         /// </summary>
         /// <param name="key">产品控制对象设置数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(ProductControlObjectKey key);
         /// <summary>
         /// 获取产品控制对象设置数据。
         /// </summary>
         /// <param name="key">产品控制对象设置数据标识符.</param>
         /// <returns>MethodReturnResult&lt;ProductControlObject&gt;，产品控制对象设置数据.</returns>
         [OperationContract]
         MethodReturnResult<ProductControlObject> Get(ProductControlObjectKey key);
         /// <summary>
         /// 获取产品控制对象设置数据集合。
         /// </summary>
         /// <param name="cfg">查询产品控制对象设置数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;ProductControlObject&gt;&gt;，产品控制对象设置数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<ProductControlObject>> Get(ref PagingConfig cfg);
    }
}
