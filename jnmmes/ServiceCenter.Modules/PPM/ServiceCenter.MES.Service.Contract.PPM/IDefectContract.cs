using ServiceCenter.MES.Model.PPM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.PPM
{
    /// <summary>
    /// 定义日不良服务契约。
    /// </summary>
    [ServiceContract]
    public interface IDefectContract
    {
        /// <summary>
        /// 添加日不良。
        /// </summary>
        /// <param name="obj">日不良。</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult Add(Defect obj);

        /// <summary>
        /// 修改日不良。
        /// </summary>
        /// <param name="obj">日不良。</param>
        /// <returns><see cref="MethodReturnResult" /></returns>
        [OperationContract]
        MethodReturnResult Modify(Defect obj);

        /// <summary>
        /// 修改日不良。
        /// </summary>
        /// <param name="lst">日不良集合。</param>
        /// <returns><see cref="MethodReturnResult" /></returns>
        [OperationContract(Name = "ModifyList")]
        MethodReturnResult Modify(IList<Defect> lst);

        /// <summary>
        /// 删除日不良数据。
        /// </summary>
        /// <param name="key">日不良标识符.</param>
        /// <returns>MethodReturnResult.</returns>
        [OperationContract]
        MethodReturnResult Delete(DefectKey key);

        /// <summary>
        /// 获取日不良数据。
        /// </summary>
        /// <param name="key">日不良标识符.</param>
        /// <returns>MethodReturnResult&lt;Defect&gt;，日不良数据.</returns>
        [OperationContract]
        MethodReturnResult<Defect> Get(DefectKey key);

        /// <summary>
        /// 获取日不良数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;Defect&gt;&gt;，日不良数据集合。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<Defect>> Get(ref PagingConfig cfg);
    }
}
