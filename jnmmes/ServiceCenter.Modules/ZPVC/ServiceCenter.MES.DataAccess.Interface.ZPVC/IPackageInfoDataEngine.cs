using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.ZPVC;
using ServiceCenter.DataAccess;
using ServiceCenter.Model;

namespace ServiceCenter.MES.DataAccess.Interface.ZPVC
{
    /// <summary>
    /// 定义包装信息数据访问接口。
    /// </summary>
    public interface IPackageInfoDataEngine 
        : IDatabaseDataEngine<PackageInfo, string>
    {
    }
}
