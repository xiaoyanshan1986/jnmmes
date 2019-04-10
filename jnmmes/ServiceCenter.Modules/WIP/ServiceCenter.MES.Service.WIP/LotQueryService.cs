using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using NHibernate;
using System.Data.SqlClient;

namespace ServiceCenter.MES.Service.WIP
{  
    /// <summary>
    /// 实现批次查询服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotQueryService : ILotQueryContract
    {
        protected Database _db;
        protected Database _query_db;       //组件报表数据查询数据库连接
        public ISessionFactory SessionFactory
        {
            get;
            set;
        }
        public LotQueryService(ISessionFactory sf)
        {
            this._db = DatabaseFactory.CreateDatabase();
            this.SessionFactory = sf;
            _query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
        }

        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public ILotDataEngine LotDataEngine { get; set; }

        /// <summary>
        /// 批次自定义特性数据访问类。
        /// </summary>
        public ILotAttributeDataEngine LotAttributeDataEngine { get; set; }

        /// <summary>
        /// 批次操作数据访问类。
        /// </summary>
        public ILotTransactionDataEngine LotTransactionDataEngine { get; set; }

        /// <summary>
        /// 批次历史数据访问类。
        /// </summary>
        public ILotTransactionHistoryDataEngine LotTransactionHistoryDataEngine { get; set; }

        /// <summary>
        /// 批次参数数据访问类。
        /// </summary>
        public ILotTransactionParameterDataEngine LotTransactionParameterDataEngine { get; set; }

        /// <summary>
        /// 批次不良数据访问类。
        /// </summary>
        public ILotTransactionDefectDataEngine LotTransactionDefectDataEngine { get; set; }

        /// <summary>
        /// 批次报废数据访问类。
        /// </summary>
        public ILotTransactionScrapDataEngine LotTransactionScrapDataEngine { get; set; }

        /// <summary>
        /// 批次补料数据访问类。
        /// </summary>
        public ILotTransactionPatchDataEngine LotTransactionPatchDataEngine { get; set; }

        /// <summary>
        /// 批次用料数据访问类。
        /// </summary>
        public ILotBOMDataEngine LotBOMDataEngine { get; set; }

        /// <summary>
        /// 批次设备加工数据访问类。
        /// </summary>
        public ILotTransactionEquipmentDataEngine LotTransactionEquipmentDataEngine { get; set; }

        /// <summary>
        /// 批次检验数据访问类。
        /// </summary>
        public ILotTransactionCheckDataEngine LotTransactionCheckDataEngine { get; set; }

        /// <summary>
        /// 批次定时作业数据访问类。
        /// </summary>
        public ILotJobDataEngine LotJobDataEngine { get; set; }

        /// <summary>
        /// 获取批次数据。
        /// </summary>
        /// <param name="key">批次标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Lot&gt;" />,批次数据.</returns>
        public MethodReturnResult<Lot> Get(string key)
        {
            MethodReturnResult<Lot> result = new MethodReturnResult<Lot>();
            if (!this.LotDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LotQueryService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LotDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Lot&gt;" />,批次数据集合。</returns>
        public MethodReturnResult<IList<Lot>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Lot>> result = new MethodReturnResult<IList<Lot>>();
            try
            {
                result.Data = this.LotDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次数据集合（包含归档数据）。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Lot&gt;" />,批次数据集合。</returns>
        public MethodReturnResult<DataSet> GetEx(RPTLotQueryDetailParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_RPT_LotAttributeList";
                    this._db.AddInParameter(cmd, "@LotList", DbType.String, p.LotNumber);
                    this._db.AddInParameter(cmd, "@AttributeType", DbType.String, p.AttributeType);
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;
                    SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);
                    result.Data = this._db.ExecuteDataSet(cmd);
                    int i = (int)cmd.Parameters["@return"].Value;
                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
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
        /// 获取批次自定义特性数据。
        /// </summary>
        /// <param name="key">批次自定义特性标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotAttribute&gt;" />,批次自定义特性数据.</returns>
        public MethodReturnResult<LotAttribute> GetAttribute(LotAttributeKey key)
        {
            MethodReturnResult<LotAttribute> result = new MethodReturnResult<LotAttribute>();
            if (!this.LotAttributeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LotQueryService_AttributeIsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LotAttributeDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次自定义特性数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotAttribute&gt;" />,批次自定义特性数据集合。</returns>
        public MethodReturnResult<IList<LotAttribute>> GetAttribute(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LotAttribute>> result = new MethodReturnResult<IList<LotAttribute>>();
            try
            {
                result.Data = this.LotAttributeDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        public MethodReturnResult<DataSet> GetAttributeEx(RPTLotQueryDetailParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_RPT_LotAttributeList";
                    this._db.AddInParameter(cmd, "@LotList", DbType.String, p.LotNumber);
                    this._db.AddInParameter(cmd, "@AttributeType", DbType.String, p.AttributeType);
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;
                    SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);
                    result.Data = this._db.ExecuteDataSet(cmd);
                    int i = (int)cmd.Parameters["@return"].Value;
                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
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

        public MethodReturnResult<DataSet> GetLotList(RPTLotQueryDetailParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_RPT_LotList";
                    this._db.AddInParameter(cmd, "@LotNumber", DbType.String, p.LotNumber);
                    this._db.AddInParameter(cmd, "@EndLotNumber", DbType.String, p.LotNumber1);
                    this._db.AddInParameter(cmd, "@LocationName", DbType.String, p.LocationName);
                    this._db.AddInParameter(cmd, "@OrderNumber", DbType.String, p.OrderNumber);
                    this._db.AddInParameter(cmd, "@MaterialCode", DbType.String, p.MaterialCode);
                    this._db.AddInParameter(cmd, "@LineCode", DbType.String, p.LineCode);
                    this._db.AddInParameter(cmd, "@RouteStepName", DbType.String, p.RouteStepName);
                    this._db.AddInParameter(cmd, "@LotState", DbType.String, p.StateFlag);
                    this._db.AddInParameter(cmd, "@PackageNo ", DbType.String, p.PackageNo);
                    this._db.AddInParameter(cmd, "@HoldFlag", DbType.String, p.HoldFlag);
                    this._db.AddInParameter(cmd, "@EndStatus", DbType.String, p.DeletedFlag);
                    this._db.AddInParameter(cmd, "@StartDateTime", DbType.String, p.StartCreateTime);
                    this._db.AddInParameter(cmd, "@EndDateTime", DbType.String, p.EndCreateTime);
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;
                    SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);
                    result.Data = this._db.ExecuteDataSet(cmd);
                    int i = (int)cmd.Parameters["@return"].Value;
                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
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
        /// 获取批次历史数据。
        /// </summary>
        /// <param name="key">批次操作标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionHistory&gt;" />,批次操作数据.</returns>
        public MethodReturnResult<LotTransactionHistory> GetLotTransactionHistory(string key)
        {
            MethodReturnResult<LotTransactionHistory> result = new MethodReturnResult<LotTransactionHistory>();
            try
            {
                result.Data = this.LotTransactionHistoryDataEngine.Get(key??string.Empty);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次历史数据。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionHistory&gt;" />,批次操作数据集合。</returns>
        public MethodReturnResult<IList<LotTransactionHistory>> GetLotTransactionHistory(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LotTransactionHistory>> result = new MethodReturnResult<IList<LotTransactionHistory>>();
            try
            {
                result.Data = this.LotTransactionHistoryDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次操作数据。
        /// </summary>
        /// <param name="key">批次操作标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransaction&gt;" />,批次操作数据.</returns>
        public MethodReturnResult<LotTransaction> GetTransaction(string key)
        {
            MethodReturnResult<LotTransaction> result = new MethodReturnResult<LotTransaction>();

            if (!this.LotTransactionDataEngine.IsExists(key))
            {
                result.Code = 1003;
                result.Message = String.Format(StringResource.LotQueryService_TransactionIsNotExists, key);
                return result;
            }
            try
            {                
                result.Data = this.LotTransactionDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }

            return result;
        }

        /// <summary>
        /// 获取批次操作数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransaction&gt;" />,批次操作数据集合。</returns>
        public MethodReturnResult<IList<LotTransaction>> GetTransaction(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LotTransaction>> result = new MethodReturnResult<IList<LotTransaction>>();

            try
            {
                ////查询使用报表数据库进行
                //using (DbConnection con = this._query_db.CreateConnection())
                //{
                //    //打开报表数据库连接
                //    con.Open();

                //    //创建链接事物期间
                //    ISession session = this.SessionFactory.OpenSession(con);

                //    //取数
                //    result.Data = this.LotTransactionDataEngine.Get(cfg, session);
                //}

                //取数
                result.Data = this.LotTransactionDataEngine.Get(cfg);                
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }

            return result;
        }
        /// <summary>
        /// 获取批次参数数据。
        /// </summary>
        /// <param name="key">批次参数标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionParameter&gt;" />,批次参数数据.</returns>
        public MethodReturnResult<LotTransactionParameter> GetTransactionParameter(LotTransactionParameterKey key)
        {
            MethodReturnResult<LotTransactionParameter> result = new MethodReturnResult<LotTransactionParameter>();
            if (!this.LotTransactionParameterDataEngine.IsExists(key))
            {
                result.Code = 1003;
                result.Message = String.Format(StringResource.LotQueryService_TransactionParameterIsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LotTransactionParameterDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次参数数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionParameter&gt;" />,批次参数数据集合。</returns>
        public MethodReturnResult<IList<LotTransactionParameter>> GetTransactionParameter(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LotTransactionParameter>> result = new MethodReturnResult<IList<LotTransactionParameter>>();
            try
            {
                result.Data = this.LotTransactionParameterDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次不良数据。
        /// </summary>
        /// <param name="key">批次不良标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionDefect&gt;" />,批次不良数据.</returns>
        public MethodReturnResult<LotTransactionDefect> GetLotTransactionDefect(LotTransactionDefectKey key)
        {
            MethodReturnResult<LotTransactionDefect> result = new MethodReturnResult<LotTransactionDefect>();
            if (!this.LotTransactionDefectDataEngine.IsExists(key))
            {
                result.Code = 1003;
                result.Message = String.Format(StringResource.LotQueryService_TransactionDefectIsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LotTransactionDefectDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次不良数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionDefect&gt;" />,批次不良数据集合。</returns>
        public MethodReturnResult<IList<LotTransactionDefect>> GetLotTransactionDefect(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LotTransactionDefect>> result = new MethodReturnResult<IList<LotTransactionDefect>>();

            try
            {
                //查询使用报表数据库进行
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    //打开报表数据库连接
                    con.Open();

                    //创建链接事物期间
                    ISession session = this.SessionFactory.OpenSession(con);

                    //取数
                    result.Data = this.LotTransactionDefectDataEngine.Get(cfg, session);
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次报废数据。
        /// </summary>
        /// <param name="key">批次报废标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionScrap&gt;" />,批次报废数据.</returns>
        public MethodReturnResult<LotTransactionScrap> GetLotTransactionScrap(LotTransactionScrapKey key)
        {
            MethodReturnResult<LotTransactionScrap> result = new MethodReturnResult<LotTransactionScrap>();
            if (!this.LotTransactionScrapDataEngine.IsExists(key))
            {
                result.Code = 1003;
                result.Message = String.Format(StringResource.LotQueryService_TransactionScrapIsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LotTransactionScrapDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次报废数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionScrap&gt;" />,批次报废数据集合。</returns>
        public MethodReturnResult<IList<LotTransactionScrap>> GetLotTransactionScrap(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LotTransactionScrap>> result = new MethodReturnResult<IList<LotTransactionScrap>>();
            try
            {
                //查询使用报表数据库进行
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    //打开报表数据库连接
                    con.Open();

                    //创建链接事物期间
                    ISession session = this.SessionFactory.OpenSession(con);

                    //取数
                    result.Data = this.LotTransactionScrapDataEngine.Get(cfg, session);
                }
                //result.Data = this.LotTransactionScrapDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次补料数据。
        /// </summary>
        /// <param name="key">批次补料标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionPatch&gt;" />,批次补料数据.</returns>
        public MethodReturnResult<LotTransactionPatch> GetLotTransactionPatch(LotTransactionPatchKey key)
        {
            MethodReturnResult<LotTransactionPatch> result = new MethodReturnResult<LotTransactionPatch>();
            if (!this.LotTransactionPatchDataEngine.IsExists(key))
            {
                result.Code = 1003;
                result.Message = String.Format(StringResource.LotQueryService_TransactionPatchIsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LotTransactionPatchDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次补料数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionPatch&gt;" />,批次补料数据集合。</returns>
        public MethodReturnResult<IList<LotTransactionPatch>> GetLotTransactionPatch(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LotTransactionPatch>> result = new MethodReturnResult<IList<LotTransactionPatch>>();

            try
            {
                //查询使用报表数据库进行
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    //打开报表数据库连接
                    con.Open();

                    //创建链接事物期间
                    ISession session = this.SessionFactory.OpenSession(con);

                    //取数
                    result.Data = this.LotTransactionPatchDataEngine.Get(cfg, session);
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次用料数据。
        /// </summary>
        /// <param name="key">批次用料标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotBOM&gt;" />,批次用料数据.</returns>
        public MethodReturnResult<LotBOM> GetLotBOM(LotBOMKey key)
        {
            MethodReturnResult<LotBOM> result = new MethodReturnResult<LotBOM>();
            if (!this.LotBOMDataEngine.IsExists(key))
            {
                result.Code = 1003;
                result.Message = String.Format(StringResource.LotQueryService_LotBOMIsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LotBOMDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次用料数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotBOM&gt;" />,批次用料数据集合。</returns>
        public MethodReturnResult<IList<LotBOM>> GetLotBOM(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LotBOM>> result = new MethodReturnResult<IList<LotBOM>>();
            try
            {
                result.Data = this.LotBOMDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次设备加工数据。
        /// </summary>
        /// <param name="key">批次设备加工数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionEquipment&gt;" />,批次设备加工数据.</returns>
        public MethodReturnResult<LotTransactionEquipment> GetLotTransactionEquipment(string key)
        {
            MethodReturnResult<LotTransactionEquipment> result = new MethodReturnResult<LotTransactionEquipment>();
            if (!this.LotTransactionEquipmentDataEngine.IsExists(key))
            {
                result.Code = 1003;
                result.Message = String.Format(StringResource.LotQueryService_LotTransactionEquipmentIsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LotTransactionEquipmentDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次设备加工数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionEquipment&gt;" />,批次设备加工数据集合。</returns>
        public MethodReturnResult<IList<LotTransactionEquipment>> GetLotTransactionEquipment(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LotTransactionEquipment>> result = new MethodReturnResult<IList<LotTransactionEquipment>>();
            try
            {
                result.Data = this.LotTransactionEquipmentDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次检验数据。
        /// </summary>
        /// <param name="key">批次检验数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionCheck&gt;" />,批次检验数据.</returns>
        public MethodReturnResult<LotTransactionCheck> GetLotTransactionCheck(string key)
        {
            MethodReturnResult<LotTransactionCheck> result = new MethodReturnResult<LotTransactionCheck>();
            try
            {
                result.Data = this.LotTransactionCheckDataEngine.Get(key??string.Empty);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次检验数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionCheck&gt;" />,批次检验数据集合。</returns>
        public MethodReturnResult<IList<LotTransactionCheck>> GetLotTransactionCheck(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LotTransactionCheck>> result = new MethodReturnResult<IList<LotTransactionCheck>>();
            try
            {
                result.Data = this.LotTransactionCheckDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次定时作业数据。
        /// </summary>
        /// <param name="key">批次定时作业数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotJob&gt;" />,批次定时作业数据.</returns>
        public MethodReturnResult<LotJob> GetLotJob(string key)
        {
            MethodReturnResult<LotJob> result = new MethodReturnResult<LotJob>();
            if (!this.LotJobDataEngine.IsExists(key))
            {
                result.Code = 1003;
                result.Message = String.Format(StringResource.LotQueryService_LotJobIsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LotJobDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次定时作业数据。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotJob&gt;" />,批次定时作业数据集合。</returns>
        public MethodReturnResult<IList<LotJob>> GetLotJob(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LotJob>> result = new MethodReturnResult<IList<LotJob>>();
            try
            {
                result.Data = this.LotJobDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        public MethodReturnResult<DataSet> GetLotCount(string where)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();

                    cmd.CommandText = string.Format(@" select count(*) as count 
                                                         from WIP_TRANSACTION  w 
                                                         where {0}", where);

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

        public MethodReturnResult<DataSet> GetRPTLotMaterialList( ref RPTLotMateriallistParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_RPT_LotMaterialList";

                    this._query_db.AddInParameter(cmd, "@LotList", DbType.String, p.LotNumber);
                    this._query_db.AddInParameter(cmd, "@PageNo", DbType.Int32, p.PageNo + 1);
                    this._query_db.AddInParameter(cmd, "@PageSize", DbType.Int32, p.PageSize);

                    //返回总记录数
                    this._query_db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);
                    cmd.Parameters["@Records"].Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);
                    result.Data = this._query_db.ExecuteDataSet(cmd);

                    //返回总记录数
                    p.TotalRecords = Convert.ToInt32(cmd.Parameters["@Records"].Value);

                    //取得存储过程返回值
                    int i = (int)cmd.Parameters["@return"].Value;

                    if (i == -1)    //-1时计算失败
                    {
                        result.Code = 1000;

                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString(); //取得错误信息
                        
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
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
        
        public MethodReturnResult<DataSet> GetRPTLotProcessingHistory( ref RPTLotMateriallistParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    //调用存储过程
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_RPT_LotProcessingHistory";
                    
                    //存储过程传递的参数
                    this._query_db.AddInParameter(cmd, "@LotList", DbType.String, p.LotNumber);
                    this._query_db.AddInParameter(cmd, "@PageNo", DbType.Int32, p.PageNo + 1);
                    this._query_db.AddInParameter(cmd, "@PageSize", DbType.Int32, p.PageSize);
                    //返回总记录数
                    this._query_db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);
                    cmd.Parameters["@Records"].Direction = ParameterDirection.Output;
                    //设置返回错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                   //设置返回值
                    SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);
                    cmd.CommandTimeout = 960;

                    //执行存储过程
                    result.Data = this._query_db.ExecuteDataSet(cmd);

                    //返回总记录数
                    p.TotalRecords = Convert.ToInt32(cmd.Parameters["@Records"].Value);

                    //取得返回值
                    int i = (int)cmd.Parameters["@return"].Value;

                    //调用失败返回错误信息
                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
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

        #region //查询组件颜色功率档及返修流程
        public MethodReturnResult<DataSet> GetLotColor(string lot)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"select SUBSTRING(ATTR_2, 0, 2) AS LOT_NUMBER from WIP_LOT
                                                        where LOT_NUMBER='{0}'
                                                        union
                                                        select 
                                                          CASE WHEN b.ROUTE_NAME like'%扒皮返修%' then '扒皮返修流程'
                                                          else '' end RouteName 
                                                         from WIP_LOT a
                                                         left join WIP_TRANSACTION b on a.LOT_NUMBER=b.LOT_NUMBER
                                                         where a.LOT_NUMBER='{0}'
                                                          group by b.ROUTE_NAME
                                                        union
                                                        select t3.PM_NAME from ZWIP_IV_TEST t1
                                                        inner join WIP_LOT t2
                                                        on t1.lot_number=t2.lot_number  and t2.ORDER_NUMBER=t2.ORDER_NUMBER 
                                                        and t1.LOT_NUMBER ='{0}'
                                                        inner join ZPPM_WORK_ORDER_PRD_POWERSET t3
                                                        on t1.PS_CODE=t3.PS_CODE and t1.PS_ITEM_NO=t3.ITEM_NO and t2.order_number=t3.order_number
                                                        where t1.IS_DEFAULT=1
                                                        union
                                                        select PS_SUBCODE from ZWIP_IV_TEST where LOT_NUMBER='{0}'
                                                       ", lot);
                    result.Data = _db.ExecuteDataSet(cmd);
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
        #endregion

        /// <summary>
        /// 查询组件匹配信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetMapDataQueryDb(ref RPTLotQueryDetailParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this._query_db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandText = "sp_LotMapDataList";
                    this._query_db.AddInParameter(cmd, "@LotNumberList", DbType.String, p.LotNumber);
                    this._query_db.AddInParameter(cmd, "@PackageNoList", DbType.String, p.PackageNo);
                    this._query_db.AddInParameter(cmd, "@OrderNumberList", DbType.String, p.OrderNumber);
                    this._query_db.AddInParameter(cmd, "@MaterialCodeList", DbType.String, p.MaterialCode);
                    this._query_db.AddInParameter(cmd, "@RouteStepName", DbType.String, p.RouteStepName);
                    this._query_db.AddInParameter(cmd, "@IsMapOk", DbType.String, p.MapType);
                    this._query_db.AddInParameter(cmd, "@PageNo", DbType.Int32, p.PageNo + 1);
                    this._query_db.AddInParameter(cmd, "@PageSize", DbType.Int32, p.PageSize);

                    //返回总记录数
                    this._query_db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);
                    cmd.Parameters["@Records"].Direction = ParameterDirection.Output;

                    //错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    //返回参数
                    SqlParameter parReturn = new SqlParameter("@return_value", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);

                    cmd.CommandTimeout = 120;

                    //执行
                    result.Data = this._query_db.ExecuteDataSet(cmd);

                    //返回总记录数
                    p.TotalRecords = Convert.ToInt32(cmd.Parameters["@Records"].Value);
                    int i = (int)cmd.Parameters["@return_value"].Value;

                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
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
