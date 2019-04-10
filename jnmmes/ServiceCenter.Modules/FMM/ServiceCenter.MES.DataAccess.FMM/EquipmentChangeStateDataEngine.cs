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
    /// 表示设备切换状态数据访问类。
    /// </summary>
    public class EquipmentChangeStateDataEngine
        : DatabaseDataEngine<EquipmentChangeState, string>
        , IEquipmentChangeStateDataEngine
    {
        public EquipmentChangeStateDataEngine(ISessionFactory sf) : base(sf) { }

        public EquipmentChangeState Get(string from, string to)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                IQuery qry = session.CreateQuery(@"FROM EquipmentChangeState as a
                                                 WHERE a.FromStateName=:from
                                                 AND a.ToStateName=:to");
                qry = qry.SetString("from", from).SetString("to",to);
                return qry.UniqueResult<EquipmentChangeState>();
            }
        }

        public EquipmentChangeState Get(string from, string to, ISession session)
        {
            if(session==null)
            {
                session = this.SessionFactory.OpenSession();
            }
        
            IQuery qry = session.CreateQuery(@"FROM EquipmentChangeState as a
                                                WHERE a.FromStateName=:from
                                                AND a.ToStateName=:to");
            qry = qry.SetString("from", from).SetString("to", to);
            return qry.UniqueResult<EquipmentChangeState>();
           
        }

    }
}
