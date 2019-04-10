using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.BaseData;
using ServiceCenter.MES.Model.BaseData;
using NHibernate;
using ServiceCenter.DataAccess;
using ServiceCenter.Common.DataAccess.NHibernate;

namespace ServiceCenter.MES.DataAccess.BaseData
{
    /// <summary>
    /// 表示基础属性值数据访问类。
    /// </summary>
    public class BaseAttributeValueDataEngine
        : DatabaseDataEngine<BaseAttributeValue, BaseAttributeValueKey>
        , IBaseAttributeValueDataEngine
    {
        public BaseAttributeValueDataEngine(ISessionFactory sf):base(sf)
        {
            this.SessionFactory = sf;
        }


        #region IBaseAttributeValueDataEngine 成员

        public IList<BaseAttributeValue> GetList(string categoryName,string attributeName)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                IQuery qry = session.CreateQuery(@"FROM BaseAttributeValue as a
                                                WHERE a.Key.CategoryName=:categoryName
                                                AND a.Key.AttributeName=:attrName");
                qry = qry.SetParameter<string>("categoryName", categoryName)
                         .SetParameter<string>("attrName", attributeName);
                return qry.List<BaseAttributeValue>();
            }
        }


        public IList<BaseAttributeValue> GetList(string categoryName)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                IQuery qry = session.CreateQuery(@"FROM BaseAttributeValue as a
                                                WHERE a.Key.CategoryName=:categoryName");
                qry = qry.SetParameter<string>("categoryName", categoryName);
                return qry.List<BaseAttributeValue>();
            }
        }
        
        #endregion

      

    }
}
