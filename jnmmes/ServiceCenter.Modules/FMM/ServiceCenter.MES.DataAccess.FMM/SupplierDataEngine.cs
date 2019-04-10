using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using NHibernate;
using ServiceCenter.Model;
using ServiceCenter.Common.DataAccess.NHibernate;


namespace ServiceCenter.MES.DataAccess.FMM
{
    /// <summary>
    /// 表示供应商数据访问类。
    /// </summary>
    public class SupplierDataEngine
        : DatabaseDataEngine<Supplier, string>
        , ISupplierDataEngine
    {
        public SupplierDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }
    }
}
