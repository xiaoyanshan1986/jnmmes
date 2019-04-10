using Microsoft.Practices.EnterpriseLibrary.Data;
using ServiceCenter.MES.Service.Contract.RPT;
using ServiceCenter.Model;
using System;
using System.Data;
using System.Data.Common;
using System.ServiceModel.Activation;
using System.Text;

namespace ServiceCenter.MES.Service.RPT
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    class QMSemiProductionService : IQMSemiProductionContract
    {
        //protected Database _db;
        protected Database _query_db;       //组件报表数据查询数据库连接

        public QMSemiProductionService()
        {
            //this._db = DatabaseFactory.CreateDatabase();

            _query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
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
        public MethodReturnResult<DataSet> GetSemiProdQtyForLine(QMSemiProductionGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_QUALITY_DAILY_FOR_LINE";
                    this._query_db.AddInParameter(cmd, "p_curDate", DbType.String, p.StartDate);
                    this._query_db.AddInParameter(cmd, "p_locationName", DbType.String, p.LocationName);
                    this._query_db.AddInParameter(cmd, "p_IsProdReport", DbType.Int32, p.IsProdReport);
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

        public MethodReturnResult<DataSet> GetSemiProdQtyForLocation(QMSemiProductionGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_QUALITY_DAILY_FOR_LOCATION";
                    this._query_db.AddInParameter(cmd, "p_curDate", DbType.String, p.StartDate);
                    this._query_db.AddInParameter(cmd, "p_IsProdReport", DbType.Int32, p.IsProdReport);
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

        public MethodReturnResult<DataSet> GetQtyForDefective(QMSemiProductionGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();

                    cmd.CommandText = string.Format(@" select t.LOCATION_NAME,t1.REASON_CODE_CATEGORY_NAME,sum(t1.DEFECT_QUANTITY) QUANTITY from WIP_TRANSACTION t 
                                                        INNER join WIP_TRANSACTION_DEFECT t1 on t.TRANSACTION_KEY=t1.TRANSACTION_KEY
                                                        where t.LOCATION_NAME='{0}' and t.CREATE_TIME>=CONVERT(DATETIME,'{1}',121)+' 08:00:00'  
                                                        AND t.CREATE_TIME<CONVERT(DATETIME,'{1}',121)+1+'08:00:00'
                                                         group by t1.REASON_CODE_CATEGORY_NAME,t.LOCATION_NAME,t.CREATE_TIME
                                                         order by t1.REASON_CODE_CATEGORY_NAME,t.LOCATION_NAME,t.CREATE_TIME", p.LocationName, p.StartDate);

                    result.Data = _query_db.ExecuteDataSet(cmd);
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

        public MethodReturnResult<DataSet> GetQtyForDefectPOS(DefectPOSGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    StringBuilder where = new StringBuilder();
                    if (!string.IsNullOrEmpty(p.PosX))
                    {
                        where.Append(" and t1.POS_X='" + p.PosX + "'");
                    }
                    if (!string.IsNullOrEmpty(p.PosY))
                    {
                        where.Append(" and t1.POS_Y='" + p.PosY + "'");
                    }
                    if (!string.IsNullOrEmpty(p.LineCode))
                    {
                        where.Append(" and t.LINE_CODE='" + p.LineCode + "'");
                    }

                    if (!string.IsNullOrEmpty(p.StepName))
                    {
                        where.Append(" and t.ROUTE_STEP_NAME='" + p.StepName + "'");
                    }
                    if (!string.IsNullOrEmpty(p.StartDate))
                    {
                        where.Append(" and t1.EDIT_TIME>='" + p.StartDate + "'");
                    }
                    if (!string.IsNullOrEmpty(p.EndDate))
                    {
                        where.Append(" and t1.EDIT_TIME<='" + p.EndDate + "'");
                    }
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select t.LOT_NUMBER,t.ROUTE_STEP_NAME,t.EDIT_TIME,count(*) qty,t.LINE_CODE from WIP_TRANSACTION t
                                                        inner join WIP_TRANSACTION_DEFECT_POS t1 on t1.TRANSACTION_KEY=t.TRANSACTION_KEY
                                                        where 1=1 {0}
                                                        group by t.LOT_NUMBER,t.ROUTE_STEP_NAME,t.EDIT_TIME,t.LINE_CODE", where);
                    result.Data = _query_db.ExecuteDataSet(cmd);
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

        public MethodReturnResult<DataSet> GetQtyForDefectReason(DefectPOSGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    StringBuilder where = new StringBuilder();
                    if(p.IsProdReport=="1")
                    {
                        where.Append(" AND t.ROUTE_STEP_NAME ='终检'");
                    }
                    else
                    {
                        where.Append(" AND t.ROUTE_STEP_NAME ='层后检测' ");
                    }
                    if (!string.IsNullOrEmpty(p.LineCode))
                    {
                        where.Append(" and t.LINE_CODE='" + p.LineCode + "'");
                    }
                    if (!string.IsNullOrEmpty(p.StartDate))
                    {
                        where.Append(" and t1.EDIT_TIME>='" + p.StartDate + "'");
                    }
                    if (!string.IsNullOrEmpty(p.EndDate))
                    {
                        where.Append(" and t1.EDIT_TIME<='" + p.EndDate + "'");
                    }
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@" select t.LINE_CODE,t1.REASON_CODE_NAME,COUNT(*) qty from WIP_TRANSACTION t
                                                        inner join WIP_TRANSACTION_DEFECT t1 on t.TRANSACTION_KEY=t1.TRANSACTION_KEY
                                                        where t.ACTIVITY=6 and t.LINE_CODE<>'' {0}
                                                        group by t.LINE_CODE,t1.REASON_CODE_NAME", where);
                    result.Data = _query_db.ExecuteDataSet(cmd);
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

        public MethodReturnResult<DataSet> GetEquipmentDailyMoveForOEE(string EquipmentNo,string curDate)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_QRY_EQP_DAILY_MOVE_TIME";
                    this._query_db.AddInParameter(cmd, "p_EquipmentNo", DbType.String, EquipmentNo);
                    this._query_db.AddInParameter(cmd, "p_curDate", DbType.String, curDate);
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
