using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Service.Contract.WIP
{
     /// <summary>
    /// 批次暂停操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface IWipEngineerContract
    {
        [OperationContract]
        MethodReturnResult TrackOutLot(TrackOutParameter p);

        [OperationContract]
        MethodReturnResult TrackInLot(TrackInParameter p);

        [OperationContract]
        MethodReturnResult ExecuteInPackageDetail(Lot lot,string packageLine);

        [OperationContract]
        MethodReturnResult<IList<PrintLabelLog>> Get(ref PagingConfig cfg);

        [OperationContract]
        MethodReturnResult UpdatePrintLabelLog(PrintLabelLog printLabelLog);

        [OperationContract]
        MethodReturnResult UnDoPackageCorner(string lotNumber);
        [OperationContract]
        MethodReturnResult ExecuteInAbnormalBIN(Lot lot, string PackageLine);
        //[OperationContract]
        //MethodReturnResult GetTransactionParameter();
    }
}
