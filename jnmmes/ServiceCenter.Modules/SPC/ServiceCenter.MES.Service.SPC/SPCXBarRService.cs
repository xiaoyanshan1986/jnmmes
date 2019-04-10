using Microsoft.Practices.EnterpriseLibrary.Data;
using ServiceCenter.MES.Service.Contract.SPC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.SPC;

namespace ServiceCenter.MES.Service.SPC
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SPCXBarRService : ISPCXBarRContract
    {

        protected Database _db;
        public SPCXBarRService()
        {
            this._db = DatabaseFactory.CreateDatabase("JNCSPCDB");
        }

        public MethodReturnResult<DataSet> Get(SPCXBarRDataGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_SPC_RPT_XBARR_DATA";
                    this._db.AddInParameter(cmd, "p_routestepname", DbType.String, p.RouteStepName);
                    this._db.AddInParameter(cmd, "p_linecode", DbType.String, p.ProductionLineCode);
                    this._db.AddInParameter(cmd, "p_equipmentcode", DbType.String, p.EquipmentCode);
                    this._db.AddInParameter(cmd, "p_d_attr_1", DbType.String, p.DAttr1);
                    this._db.AddInParameter(cmd, "p_startTime", DbType.String, p.StartTime);
                    this._db.AddInParameter(cmd, "p_endTime", DbType.String, p.EndTime);
                    result.Data = this._db.ExecuteDataSet(cmd);
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


        public MethodReturnResult<DataSet> GetXBarData(SPCXBarRDataGetParameter p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_SPC_RPT_XBARR_DATA";
                    this._db.AddInParameter(cmd, "p_JobId", DbType.String, p.JobId);
                    this._db.AddInParameter(cmd, "p_paramName", DbType.String, p.ParamterName);
                    this._db.AddInParameter(cmd, "p_routestepname", DbType.String, p.RouteStepName);
                    this._db.AddInParameter(cmd, "p_linecode", DbType.String, p.ProductionLineCode);
                    this._db.AddInParameter(cmd, "p_equipmentcode", DbType.String, p.EquipmentCode);
                    this._db.AddInParameter(cmd, "p_slotcode", DbType.String, p.SLotCode);
                    this._db.AddInParameter(cmd, "p_d_attr_1", DbType.String, p.DAttr1);
                    this._db.AddInParameter(cmd, "p_startTime", DbType.String, p.StartTime);
                    this._db.AddInParameter(cmd, "p_endTime", DbType.String, p.EndTime);
                    result.Data = this._db.ExecuteDataSet(cmd);
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


        public MethodReturnResult<DataSet> GetEquipment(string stepname)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_SPC_RPT_EQUIPMENTCODE_DATA";
                    //this._db.AddInParameter(cmd, "p_JobId", DbType.String, JobId);
                    this._db.AddInParameter(cmd, "p_routestepname", DbType.String, stepname);
                    result.Data = this._db.ExecuteDataSet(cmd);
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

        public MethodReturnResult<DataSet> GetJobDataCode(string codeType, string jobId, string stepName)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_SPC_RPT_JOBDATA_CODE";
                    this._db.AddInParameter(cmd, "p_CodeType", DbType.String, codeType);
                    this._db.AddInParameter(cmd, "p_JobId", DbType.String, jobId);
                    this._db.AddInParameter(cmd, "p_RouteStepName", DbType.String, stepName);
                    result.Data = this._db.ExecuteDataSet(cmd);
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


        public MethodReturnResult<DataSet> GetChartMonitorList(SPCChartMonitorQuery p)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format(@"
                        select Max(JOB_ID) as JOB_ID ,JOB_TYPE,PARAM_NAME,ROUTE_STEP_NAME , CHART_TYPE ,
                        [LINK_ACTION],[LINK_CONTROLLER],[LINK_AREA]
                        from JNCMES.[dbo].[SPC_JOB] 
                        where ATTR_5='1' and ROUTE_STEP_NAME like '%{0}%'
                        and PARAM_NAME like '%{1}%' and EQUIPMENT_CODE like '%{2}%' 
                        group by JOB_TYPE,PARAM_NAME,ROUTE_STEP_NAME , CHART_TYPE,
                        [LINK_ACTION],[LINK_CONTROLLER],[LINK_AREA]
                        order by ROUTE_STEP_NAME,PARAM_NAME", p.RouteStepName, p.ParamterName, p.EquipmentCode);
                    result.Data = this._db.ExecuteDataSet(cmd);
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

        public MethodReturnResult<DataSet> GetOriginalDataForExport(string testtime, string linecode, string eqpcode, string ParamterName)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            MethodReturnResult<DataSet> result1 = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    StringBuilder sb = new StringBuilder();
                    if (!string.IsNullOrEmpty(testtime))
                    {

                        sb.Append(" and TEST_TIME= '" + testtime + "'");
                    }
                    if (!string.IsNullOrEmpty(linecode))
                    {
                        sb.Append(" and D_LINE_CODE LIKE '%" + linecode + "'");
                    }
                    if (!string.IsNullOrEmpty(eqpcode))
                    {
                        sb.Append(" and D_EQUIPMENT_CODE='" + eqpcode + "'");
                    }
                    if (!string.IsNullOrEmpty(ParamterName))
                    {
                        sb.Append(" and PARAM_NAME='" + ParamterName + "'");
                    }
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format(@"
                        select JOB_DATA_ID from SPC_JOB_DATA01 where 1=1 {0}", sb);
                    result.Data = this._db.ExecuteDataSet(cmd);
                }
                if (!string.IsNullOrEmpty(result.Data.Tables[0].Rows[0][0].ToString()))
                {
                    using (DbConnection con = this._db.CreateConnection())
                    {
                        DbCommand cmd = con.CreateCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SP_SPC_GET_RAWDATA_FROM_JOBID";
                        this._db.AddInParameter(cmd, "p_JOB_DATA_ID", DbType.String, result.Data.Tables[0].Rows[0][0].ToString());
                        result1.Data = this._db.ExecuteDataSet(cmd);
                    }
                }

            }
            catch (Exception ex)
            {
                result1.Code = 1000;
                result1.Message = ex.Message;
                result1.Detail = ex.ToString();
            }
            return result1;
        }

        public int UpdateDealNote(string testtime, string linecode, string eqpcode, string ParamterName, string Note)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            int flag = 0;
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    StringBuilder sb = new StringBuilder();
                    if (!string.IsNullOrEmpty(testtime))
                    {

                        sb.Append(" and TEST_TIME= '" + testtime + "'");
                    }
                    if (!string.IsNullOrEmpty(linecode))
                    {
                        sb.Append(" and D_LINE_CODE LIKE '%" + linecode + "'");
                    }
                    if (!string.IsNullOrEmpty(eqpcode))
                    {
                        sb.Append(" and D_EQUIPMENT_CODE='" + eqpcode + "'");
                    }
                    if (!string.IsNullOrEmpty(ParamterName))
                    {
                        sb.Append(" and PARAM_NAME='" + ParamterName + "'");
                    }
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format(@"
                        update SPC_JOB_DATA01 set VALID_REMARK={0}
                        where {1}", Note, sb);
                   flag = this._db.ExecuteNonQuery(cmd);
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return flag;
        }
    }
}
