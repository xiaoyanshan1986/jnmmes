using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using NHibernate;
using ServiceCenter.Common.DataAccess.NHibernate;


namespace ServiceCenter.MES.DataAccess.FMM
{
    /// <summary>
    /// 表示打印标签数据访问类。
    /// </summary>
    public class PrintLabelDataEngine
        : DatabaseDataEngine<PrintLabel, string>
        , IPrintLabelDataEngine
    {
        public PrintLabelDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }
    }
}
