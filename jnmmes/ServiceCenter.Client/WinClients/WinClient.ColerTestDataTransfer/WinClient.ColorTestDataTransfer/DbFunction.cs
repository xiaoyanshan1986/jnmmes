using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;



namespace WinClient.ColorTestDataTransfer
{
  
    public class DatabaseEx
    {
        private static DatabaseEx dbDatabaseEx = null;

        private Database db = null;
        public DatabaseEx()
        {
            db = DatabaseFactory.CreateDatabase();
        }

        public DataTable getDataTable(string strSql)
        {

            DataSet oDataSet = new DataSet();
            DataTable oDataTable = null;
            try
            {
                oDataSet = db.ExecuteDataSet(CommandType.Text,strSql);

                if(oDataSet.Tables.Count>0)
                {
                    oDataTable=oDataSet.Tables[0];
                }           
            }catch(Exception er)
            {

            }
            return oDataTable;
        }

        public int ExecuteNonQuery(string strSql)
        {
            return db.ExecuteNonQuery(CommandType.Text, strSql);
        }

        public string ExecuteNonQuery(string[] arrSql)
        {
            string strReturn = "";
            DbConnection conn = null;
            DbTransaction trans = null;
            try { 
                conn =db.CreateConnection();
                conn.Open();
                trans = conn.BeginTransaction();
                DbCommand command = null;
                for (int i = 0; i < arrSql.Length;i++ )
                {
                    command = conn.CreateCommand();
                    command.CommandText = arrSql[i];
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                trans.Commit();
            }catch(Exception er)
            {
                strReturn = er.Message;
                trans.Rollback();
            }
            finally
            {
                conn.Close();
            }
            return strReturn;
        }

        public string ExecuteNonQuery(List<string> lstSql)
        {
            string strReturn = "";
            if(lstSql!=null && lstSql.Count>0)
            { 
                DbConnection conn = null;
                DbTransaction trans = null;
                try
                {
                    conn = db.CreateConnection();
                    conn.Open();
                    trans = conn.BeginTransaction();
                    DbCommand command = null;
                    foreach(string strSql in lstSql)
                    {
                        command = conn.CreateCommand();
                        command.Transaction = trans;
                        command.CommandText = strSql;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                    trans.Commit();
                }
                catch (Exception er)
                {
                    strReturn = er.Message;
                    trans.Rollback();
                }
                finally
                {
                    conn.Close();
                }
            }
            return strReturn;
        }


        public static DatabaseEx getDbInstance()
        {
            if (dbDatabaseEx == null)
            {
                dbDatabaseEx = new DatabaseEx();
            }
            return dbDatabaseEx;
        }
    }
}


