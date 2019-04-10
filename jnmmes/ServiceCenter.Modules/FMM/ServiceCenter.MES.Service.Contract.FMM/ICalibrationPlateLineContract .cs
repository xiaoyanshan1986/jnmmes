using ServiceCenter.MES.Model.FMM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.FMM
{
    /// <summary>
    /// 定义校准板线别规则属性数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface ICalibrationPlateLineContract
    {
         /// <summary>
         /// 添加校准板线别规则属性数据。
         /// </summary>
         /// <param name="obj">校准板线别规则属性数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(CalibrationPlateLine obj);
         /// <summary>
         /// 修改校准板线别规则属性数据。
         /// </summary>
         /// <param name="obj">校准板线别规则属性数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(CalibrationPlateLine obj);
         /// <summary>
         /// 删除校准板线别规则属性数据。
         /// </summary>
         /// <param name="key">校准板线别规则属性数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(CalibrationPlateLineKey key);
         /// <summary>
         /// 获取校准板线别规则属性数据。
         /// </summary>
         /// <param name="key">校准板线别规则属性数据标识符.</param>
         /// <returns>MethodReturnResult&lt;CalibrationPlateLine&gt;，校准板线别规则属性数据.</returns>
         [OperationContract]
         MethodReturnResult<CalibrationPlateLine> Get(CalibrationPlateLineKey key);
         /// <summary>
         /// 获取校准板线别规则属性数据集合。
         /// </summary>
         /// <param name="cfg">查询校准板线别规则属性。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;CalibrationPlateLine&gt;&gt;，校准板线别规则属性数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<CalibrationPlateLine>> Get(ref PagingConfig cfg);
    }
}
