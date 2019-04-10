using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Model;
using System.Data;

namespace ServiceCenter.MES.Service.Contract.WIP
{
    /// <summary>
    /// 撤销批次上一步操作方法的参数类。
    /// </summary>
    [DataContract]
    public class UndoParameter : MethodParameter
    {
        /// <summary>
        /// 被撤销操作主键的集合。
        /// </summary>
        [DataMember]
        public IDictionary<string, IList<string>> UndoTransactionKeys { get; set; }

        /// <summary>
        /// 组件序列号。
        /// </summary>
        [DataMember]
        public string LotNumber { get; set; }
    }
    
  
    /// <summary>
    /// 撤销批次上一步操作的契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotUndoContract
    {
        /// <summary>
        /// 撤销批次上一步操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult Undo(UndoParameter p);
      
        [OperationContract]
        MethodReturnResult<DataSet> GetLotProcessing(ref LotProcessingParameter p);
    }

    /// <summary>
    /// 用于扩展批次操作撤销检查的接口。
    /// </summary>
    public interface ILotUndoCheck
    {
        /// <summary>
        /// 进行批次操作撤销前检查。
        /// </summary>
        /// <param name="p">操作撤销参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(UndoParameter p);
    }

    /// <summary>
    /// 用于扩展批次操作撤销执行的接口。
    /// </summary>
    public interface ILotUndo
    {
        /// <summary>
        /// 进行批次操作撤销。
        /// </summary>
        /// <param name="p">操作撤销参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(UndoParameter p);

       
    }
}
