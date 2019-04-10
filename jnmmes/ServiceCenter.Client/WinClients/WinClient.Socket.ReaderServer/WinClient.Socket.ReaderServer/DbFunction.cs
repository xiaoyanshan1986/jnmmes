using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using System.Configuration;
using System.Data.SqlClient;

namespace WinClient.Socket.ReaderServer
{
    public static class SqlHelper
    {
        private static string conStr = ConfigurationManager.ConnectionStrings["RPTDB"].ConnectionString;
        //private static string conStr = "Server=10.0.2.83;uid=sa;password=123456;initial catalog=JNMES;Pooling=true;Max Pool Size=40000;Min Pool Size=0;";
        //增删改
        public static int ExecuteNonQuery(string sqlStr, CommandType ct, params SqlParameter[] sq)
        {

            using (SqlConnection sqlCon = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand(sqlStr, sqlCon))
                {
                    cmd.CommandType = ct;
                    if (sq != null)
                    {
                        cmd.Parameters.AddRange(sq);
                    }
                    //打开连接
                    sqlCon.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        //scalar
        public static object ExecuteScalar(string sqlStr, CommandType ct, params SqlParameter[] sq)
        {
            using (SqlConnection sqlCon = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand(sqlStr, sqlCon))
                {
                    cmd.CommandType = ct;
                    if (sq != null)
                    {
                        cmd.Parameters.AddRange(sq);
                    }
                    //打开连接
                    sqlCon.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }
        //reader
        public static SqlDataReader ExecuteReader(string sqlStr, CommandType ct, params SqlParameter[] sq)
        {
            SqlConnection sqlCon = new SqlConnection(conStr);
            using (SqlCommand cmd = new SqlCommand(sqlStr, sqlCon))
            {
                cmd.CommandType = ct;
                if (sq != null)
                {
                    cmd.Parameters.AddRange(sq);
                }
                //打开连接
                try
                {
                    sqlCon.Open();
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception)
                {
                    sqlCon.Dispose();
                    throw;
                }
            }
        }
        //table
        public static DataTable ExecuteDataTable(string sqlStr, CommandType ct, params SqlParameter[] sq)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter sda = new SqlDataAdapter(sqlStr, conStr))
            {
                sda.SelectCommand.CommandType = ct;
                if (sq != null)
                {
                    sda.SelectCommand.Parameters.AddRange(sq);
                }
                sda.Fill(dt);
                return dt;
            }
        }
    }
}
