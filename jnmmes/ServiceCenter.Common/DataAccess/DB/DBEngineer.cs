using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Xml;
using System.Data.Common;
using NHibernate;

namespace ServiceCenter.Common.DataAccess.DB
{
    public class DBEngineer:IDBEngineer
    {
        private static DBEngineer dbEngineer = null;

        private Database db = null;
        private ISessionFactory sessionFactory = null;
        private string strConnection = string.Empty;
        public DBEngineer(ISessionFactory f)
        {
            sessionFactory = f;
            db = DatabaseFactory.CreateDatabase("RPTDB");
        }

        public static DBEngineer CreateDbInstance(ISessionFactory f)
        {
            if (dbEngineer == null)
            {
                dbEngineer = new DBEngineer(f);
            }
            return dbEngineer;
        }

        public DataSet ExecuteDataSet(string strSql)
        {
            return db.ExecuteDataSet(CommandType.Text, strSql);
        }
    }

    public interface IDBEngineer
    {
        DataSet ExecuteDataSet(string strSql);

        //DataTable ExecuteDataTable(string strSql);

        //bool ChkExists(string strSql);

        //Database  DEngineer{get;}
    }
    
}
