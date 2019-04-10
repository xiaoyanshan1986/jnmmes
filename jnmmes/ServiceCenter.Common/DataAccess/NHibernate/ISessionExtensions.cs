using NHibernate;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.Common.DataAccess.NHibernate
{
    public static class ISessionExtensions
    {
        public static IList<T> GetList<T>(this ISession session, PagingConfig cfg)
        {
            string typeName = typeof(T).ToString();
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbSqlCount = new StringBuilder();
            sbSql.Append(string.Format("FROM {0} as self", typeName));
            sbSqlCount.Append(string.Format("SELECT COUNT(*) FROM {0} as self", typeName));
            if (!string.IsNullOrEmpty(cfg.Where))
            {
                sbSqlCount.AppendFormat(" WHERE {0} ", cfg.Where);
                sbSql.AppendFormat(" WHERE {0} ", cfg.Where);
            }
            if (!string.IsNullOrEmpty(cfg.OrderBy) )
            {
                if ( cfg.OrderBy != "#*#")
                {
                    sbSql.AppendFormat(" Order By {0}", cfg.OrderBy);
                }
            }
            else
            {
                sbSql.Append(" Order By Key");
            }

             IQuery qryCount = session.CreateQuery(sbSqlCount.ToString());
            cfg.Records = Convert.ToInt32(qryCount.UniqueResult());
            int pageNo = cfg.PageNo < 0 ? 0 : cfg.PageNo;
            IQuery qry = session.CreateQuery(sbSql.ToString());
            //需要分页。
            if(cfg.IsPaging)
            {
                qry=qry.SetFirstResult(pageNo * cfg.PageSize)
                       .SetMaxResults(cfg.PageSize);
            }
            return qry.List<T>();
        }
    }
}
