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
    /// 表示产品编码成柜参数数据访问类。
    /// </summary>
    public class MaterialChestParameterDataEngine
        : DatabaseDataEngine<MaterialChestParameter, string>
        , IMaterialChestParameterDataEngine
    {
        public MaterialChestParameterDataEngine(ISessionFactory sf):base(sf){}
    }
}
