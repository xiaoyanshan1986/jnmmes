using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.DataAccess;
using ServiceCenter.Model;

namespace ServiceCenter.MES.DataAccess.Interface.PPM
{
    /// <summary>
    /// 定义混工单组规则数据访问接口。
    /// </summary>
    public interface IWorkOrderGroupDetailDataEngine : IDatabaseDataEngine<WorkOrderGroupDetail, WorkOrderGroupDetailKey>
    {
    }
}
