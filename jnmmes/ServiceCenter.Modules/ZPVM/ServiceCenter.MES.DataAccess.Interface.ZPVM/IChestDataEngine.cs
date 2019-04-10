using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.DataAccess;
using ServiceCenter.Model;

namespace ServiceCenter.MES.DataAccess.Interface.ZPVM
{
    /// <summary>
    /// 入柜数据访问接口。
    /// </summary>
    public interface IChestDataEngine
        : IDatabaseDataEngine<Chest, string>
    {
    }
}
