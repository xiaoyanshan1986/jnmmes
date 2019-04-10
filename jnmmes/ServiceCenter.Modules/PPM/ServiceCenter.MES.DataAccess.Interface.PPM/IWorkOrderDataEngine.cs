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
    /// 表示工单数据访问接口。
    /// </summary>
    public interface IWorkOrderDataEngine : IDatabaseDataEngine<WorkOrder, string>
    {
    }
}
