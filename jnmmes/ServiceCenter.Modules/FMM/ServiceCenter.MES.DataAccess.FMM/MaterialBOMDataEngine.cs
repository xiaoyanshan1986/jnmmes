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
    /// 表示物料BOM数据访问类。
    /// </summary>
    public class MaterialBOMDataEngine
        : DatabaseDataEngine<MaterialBOM, MaterialBOMKey>
        ,IMaterialBOMDataEngine
    {
        public MaterialBOMDataEngine(ISessionFactory sf):base(sf)
        {
        }
    }
}
