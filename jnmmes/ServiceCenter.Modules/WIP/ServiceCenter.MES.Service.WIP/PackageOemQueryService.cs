using Microsoft.Practices.EnterpriseLibrary.Data;
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


namespace ServiceCenter.MES.Service.WIP
{

    /// <summary>
    /// 实现包装查询服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PackageOemQueryService : IPackageOemQueryContract
    {
        protected Database _db;
        public PackageOemQueryService()
        {
            this._db = DatabaseFactory.CreateDatabase();
        }
        /// <summary>
        /// 包装数据访问类。
        /// </summary>

        public IPackageDataEngine PackageDataEngine { get; set; }
        /// <summary>
        /// 包装明细数据访问类。
        /// </summary>
        public IPackageOemDetailDataEngine PackageOemDetailDataEngine { get; set; }

        /// <summary>
        /// 获取包装数据。
        /// </summary>
        /// <param name="key">包装标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Package&gt;" />,包装数据.</returns>
        public MethodReturnResult<PackageOemDetail> Get(PackageOemDetailKey key)
        {
            MethodReturnResult<PackageOemDetail> result = new MethodReturnResult<PackageOemDetail>();
            if (!this.PackageOemDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PackageQueryService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.PackageOemDetailDataEngine.Get(key);
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
        /// 获取包装数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Package&gt;" />,包装数据集合。</returns>
        public MethodReturnResult<IList<PackageOemDetail>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<PackageOemDetail>> result = new MethodReturnResult<IList<PackageOemDetail>>();
            try
            {
                result.Data = this.PackageOemDetailDataEngine.Get(cfg);
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
        /// 获取包装明细数据。
        /// </summary>
        /// <param name="key">包装明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;PackageDetail&gt;" />,包装明细数据.</returns>
        public MethodReturnResult<PackageOemDetail> GetDetail(PackageOemDetailKey key)
        {
            MethodReturnResult<PackageOemDetail> result = new MethodReturnResult<PackageOemDetail>();
            if (!this.PackageOemDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PackageQueryService_DetailIsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.PackageOemDetailDataEngine.Get(key);
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
        /// 获取包装明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;PackageDetail&gt;" />,包装明细数据集合。</returns>
        public MethodReturnResult<IList<PackageOemDetail>> GetDetail(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<PackageOemDetail>> result = new MethodReturnResult<IList<PackageOemDetail>>();
            try
            {
                result.Data = this.PackageOemDetailDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }


        public MethodReturnResult<DataSet> GetPackageTransaction(string key)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"select  PALLET_NO,TYPE,SN,PMP,ISC,VOC,IMP,VMP,FF,PNOM,DL,EDIT_TIME
			                                           from WIP_OEM_PACKAGE_DETAIL  where   SN  Like '{0}%'
				                                            order by Time", key);
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


        public MethodReturnResult CleanBin(string lineCode,string binNo)
        {
            MethodReturnResult result = new MethodReturnResult();
            int i = 0;
            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    string sql = null;

                    if (lineCode!=null&&binNo != null)
                    {
                        sql= string.Format("update [dbo].[WIP_PACKAGE_BIN]  set BIN_PACKAGED=1   where PACKAGE_LINE='{0}'  and BIN_PACKAGED=0 and BIN_NO='{1}'", lineCode,binNo);
                    }
                    else if (lineCode!=null&&binNo == null)
                    {
                        sql=string.Format("update [dbo].[WIP_PACKAGE_BIN]  set BIN_PACKAGED=1   where PACKAGE_LINE='{0}'    and BIN_PACKAGED=0", lineCode);
                    }
                    cmd.CommandText =sql;
                     i =_db.ExecuteNonQuery(cmd);
                    if (i<=0)
                    {
                        result.Code = 100;
                        result.Message = "您输入的数据有误！请选择正确的线别和Bin号";
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
