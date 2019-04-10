using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.DataAccess;

namespace ServiceCenter.MES.DataAccess.Interface.PPM
{
    /// <summary>
    /// 表示工单产品的数据访问接口。
    /// </summary>
    public interface IWorkOrderProductDataEngine
        : IDatabaseDataEngine<WorkOrderProduct, WorkOrderProductKey>
    {
    }
}
