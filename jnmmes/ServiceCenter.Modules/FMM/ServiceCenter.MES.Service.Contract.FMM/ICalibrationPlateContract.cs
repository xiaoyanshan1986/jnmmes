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
    /// 定义CalibrationPlate规则属性数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface ICalibrationPlateContract
    {
         /// <summary>
         /// 添加CalibrationPlate规则属性数据。
         /// </summary>
         /// <param name="obj">CalibrationPlate规则属性数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(CalibrationPlate obj);
         /// <summary>
         /// 修改CalibrationPlate规则属性数据。
         /// </summary>
         /// <param name="obj">CalibrationPlate规则属性数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(CalibrationPlate obj);
         /// <summary>
         /// 删除CalibrationPlate规则属性数据。
         /// </summary>
         /// <param name="key">CalibrationPlate规则属性数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取CalibrationPlate规则属性数据。
         /// </summary>
         /// <param name="key">CalibrationPlate规则属性数据标识符.</param>
         /// <returns>MethodReturnResult&lt;CalibrationPlateRule&gt;，CalibrationPlate规则属性数据.</returns>
         [OperationContract]
         MethodReturnResult<CalibrationPlate> Get(string key);
         /// <summary>
         /// 获取CalibrationPlate规则属性数据集合。
         /// </summary>
         /// <param name="cfg">查询CalibrationPlate规则属性。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;CalibrationPlateRule&gt;&gt;，CalibrationPlate规则属性数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<CalibrationPlate>> Get(ref PagingConfig cfg);
    }
}
