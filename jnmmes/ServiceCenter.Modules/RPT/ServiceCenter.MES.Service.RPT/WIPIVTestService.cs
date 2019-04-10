using Microsoft.Practices.EnterpriseLibrary.Data;
using ServiceCenter.MES.Service.Contract.RPT;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.RPT
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    class WIPIVTestService:IWIPIVTestContract
    {
        //protected Database _db;
        protected Database _query_db;       //组件报表数据查询数据库连接

         public WIPIVTestService()
        {
            _query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
            //this._db = DatabaseFactory.CreateDatabase();

        }

         public MethodReturnResult<DataSet> GetBaseDataForIVTest(string type)
         {
             MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
             try
             {
                 using (DbConnection con = this._query_db.CreateConnection())
                 {
                     DbCommand cmd = con.CreateCommand();
                     cmd.CommandType = CommandType.StoredProcedure;
                     cmd.CommandText = "SP_QRY_WIP_IVTEST_BASEDATA_DATA";
                     this._query_db.AddInParameter(cmd, "p_type", DbType.String, type);
                     result.Data = this._query_db.ExecuteDataSet(cmd);
                 }
             }
             catch (Exception ex)
             {
                 result.Code = 1000;
                 result.Message = ex.Message;
                 result.Detail = ex.ToString();
             }
             return result;
         }
         /// <summary>
         /// IVTEST数据获取操作。
         /// </summary>
         /// <param name="p">参数。</param>
         /// <returns>方法执行结果。</returns>
         public MethodReturnResult<DataSet> Get(WIPIVTestGetParameter p)
         {
             MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
             try
             {
                 using (DbConnection con = this._query_db.CreateConnection())
                 {
                     DbCommand cmd = con.CreateCommand();
                     cmd.CommandType = CommandType.StoredProcedure;
                     cmd.CommandText = "SP_QRY_WIP_IVTEST_DATA";
                     this._query_db.AddInParameter(cmd, "p_startTime", DbType.String, p.StartDate);
                     this._query_db.AddInParameter(cmd, "p_endTime", DbType.String, p.EndDate);
                     this._query_db.AddInParameter(cmd, "p_eqpcode", DbType.String, p.EquipmentCode);
                     this._query_db.AddInParameter(cmd, "p_attr1", DbType.String, p.Attr_1);
                     this._query_db.AddInParameter(cmd, "p_lotnumber", DbType.String, p.Lot_Number);
                     result.Data = this._query_db.ExecuteDataSet(cmd);
                 }
             }
             catch (Exception ex)
             {
                 result.Code = 1000;
                 result.Message = ex.Message;
                 result.Detail = ex.ToString();
             }
             return result;
         }


         /// <summary>
         /// IVTEST数据获取操作。
         /// </summary>
         /// <param name="p">参数。</param>
         /// <returns>方法执行结果。</returns>
         public MethodReturnResult<DataTable> GetIVDataForJZ(WIPIVTestGetParameter p)
         {
             DataSet ds = null;
             DataTable dt = null;
             MethodReturnResult<DataTable> result = new MethodReturnResult<DataTable>();
             try
             {
                 using (DbConnection con = this._query_db.CreateConnection())
                 {
                     DbCommand cmd = con.CreateCommand();
                     cmd.CommandType = CommandType.StoredProcedure;
                     cmd.CommandText = "SP_QRY_WIP_IVTEST_CALIBRATION_DATA";
                     this._query_db.AddInParameter(cmd, "p_startTime", DbType.String, p.StartDate);
                     this._query_db.AddInParameter(cmd, "p_endTime", DbType.String, p.EndDate);
                     this._query_db.AddInParameter(cmd, "p_eqpcode", DbType.String, p.EquipmentCode);
                     //this._query_db.AddInParameter(cmd, "p_attr1", DbType.String, p.Attr_1);
                     this._query_db.AddInParameter(cmd, "p_lotnumber", DbType.String, p.Lot_Number);
                     this._query_db.AddInParameter(cmd, "p_linecode", DbType.String, p.LineCode);
                     this._query_db.AddInParameter(cmd, "p_calibrationId", DbType.String, p.CalibrationId);

                     ds = this._query_db.ExecuteDataSet(cmd);
                     if (ds != null && ds.Tables.Count > 0)
                     {
                         dt = ds.Tables[0].Clone();
                         System.DateTime dLastTestTime = System.DateTime.Now;
                         System.DateTime dTestTime;
                         System.Data.DataRow dRow = null;
                         //System.Data.DataRow dRowClone = null;
                         //bool blIsNewPoint =false;
                         Queue<DataRow> docQueue = new Queue<DataRow>();
                         for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                         {
                             dRow = ds.Tables[0].Rows[i];
                             if (DateTime.TryParse(ds.Tables[0].Rows[i]["TEST_TIME"].ToString(), out dTestTime) == false)
                             {
                                 continue;
                             }
                             if (i == 0)
                             {
                                 dLastTestTime = dTestTime;
                                 addDataRowToQueue(docQueue, dRow);
                             }
                             if (dLastTestTime.AddMinutes(50).CompareTo(dTestTime) < 0)
                             {
                                 //清空docQueue，新增点
                                 AddQueueToTable(dt, docQueue);
                             }
                             addDataRowToQueue(docQueue, dRow);
                             dLastTestTime = dTestTime;
                         }
                         AddQueueToTable(dt, docQueue);
                     }
                     result.Data = dt;
                 }
             }
             catch (Exception ex)
             {
                 result.Code = 1000;
                 result.Message = ex.Message;
                 result.Detail = ex.ToString();
             }
             return result;
         }

        private void addDataRowToQueue(Queue<DataRow> QueueRows, DataRow dRow)
        {
            if(QueueRows.Count>2)
            {
                QueueRows.Dequeue();
            }
            QueueRows.Enqueue(dRow);
        }

        private void AddQueueToTable(DataTable dt , Queue<DataRow> QueueRows)
        {
            while(QueueRows.Count>0)
            {
                DataRow sourceRow = QueueRows.Dequeue();
                DataRow row= dt.NewRow();
                for (int indexOfCol = 0; indexOfCol < dt.Columns.Count; indexOfCol++)
                {
                    row[indexOfCol] = sourceRow[indexOfCol];
                }
                dt.Rows.Add(row);   
            }
        }


        /// <summary>
        /// IVTEST FOR CTM数据获取操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        public MethodReturnResult<DataSet> GetIVDataForCTM(WIPIVTestGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_QRY_WIP_IVTEST_CTM_DATA";
                    this._query_db.AddInParameter(cmd, "p_startTime", DbType.String, p.StartDate);
                    this._query_db.AddInParameter(cmd, "p_endTime", DbType.String, p.EndDate);
                    this._query_db.AddInParameter(cmd, "p_eqpcode", DbType.String, p.EquipmentCode);
                    this._query_db.AddInParameter(cmd, "p_attr1", DbType.String, p.Attr_1);
                    this._query_db.AddInParameter(cmd, "p_lotnumber", DbType.String, p.Lot_Number);
                    result.Data = this._query_db.ExecuteDataSet(cmd);
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }
    }
}
