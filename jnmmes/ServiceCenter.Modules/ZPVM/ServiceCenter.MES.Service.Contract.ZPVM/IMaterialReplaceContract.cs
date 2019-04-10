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
    /// 定义物料替换规则数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IMaterialReplaceContract 
    {
         /// <summary>
         /// 添加物料替换规则。
         /// </summary>
         /// <param name="obj">物料替换规则数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(MaterialReplace obj);
         /// <summary>
         /// 修改物料替换规则。
         /// </summary>
         /// <param name="obj">物料替换规则数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(MaterialReplace obj);
         /// <summary>
         /// 删除物料替换规则。
         /// </summary>
         /// <param name="key">物料替换规则主键.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(MaterialReplaceKey key);
         /// <summary>
         /// 获取物料替换规则。
         /// </summary>
         /// <param name="key">物料替换规则主键.</param>
         /// <returns>MethodReturnResult&lt;MaterialReplace&gt;，物料替换规则数据.</returns>
         [OperationContract]
         MethodReturnResult<MaterialReplace> Get(MaterialReplaceKey key);
         /// <summary>
         /// 获取物料替换规则。
         /// </summary>
         /// <param name="cfg">查询物料替换规则数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;MaterialReplace&gt;&gt;，物料替换规则数据集合。</returns>
         [OperationContract]
         MethodReturnResult<IList<MaterialReplace>> Gets(ref PagingConfig cfg);
    }
}
