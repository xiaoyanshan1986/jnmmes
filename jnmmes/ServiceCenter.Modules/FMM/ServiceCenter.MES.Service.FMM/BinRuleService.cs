using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.FMM;
using ServiceCenter.MES.Service.FMM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.FMM
{
    /// <summary>
    /// 实现客户端配置属性管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class BinRuleService : IBinRuleContract
    {
        /// <summary>
        /// 客户端配置属性数据访问读写。
        /// </summary>
        public IBinRuleDataEngine BinRuleDataEngine { get; set; }

        /// <summary>
        /// 线别数据属性
        /// </summary>
        public IProductionLineDataEngine ProductionLineDataEngine { get; set; }

        /// <summary>
        /// 包装线BIN数据属性
        /// </summary>
        public IPackageBinDataEngine PackageBinDataEngine { get; set; }


        /// <summary>
        /// 添加客户端配置属性。
        /// </summary>
        /// <param name="obj">客户端配置属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(BinRule obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.BinRuleDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.BinRuleService_IsExists, obj.Key);
                return result;
            }
            PagingConfig cfg = new PagingConfig()//判断该BIN是否是异常BIN号，如果是的话，不能设置
            {
                Where = string.Format("Key='{0}' and Attr2='{1}'", obj.Key.PackageLine, obj.Key.BinNo),
                IsPaging=false
            };
            IList<ProductionLine> listProductionLine = this.ProductionLineDataEngine.Get(cfg);
            if (listProductionLine.Count > 0)
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.AbnormalBin_IsExists, obj.Key.PackageLine,obj.Key.BinNo);
                return result;
            }

            #region 判断该BIN是否已经设置过了
            cfg = new PagingConfig()
            {
                Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}'",obj.Key.BinNo , obj.Key.PackageLine),
                IsPaging = false
            };
            IList<BinRule> listBinRule = this.BinRuleDataEngine.Get(cfg);
            if (listBinRule.Count > 0)
            {
                result.Code = 1002;
                result.Message = String.Format("线别[{0}]已设置过[{1}]Bin", obj.Key.PackageLine, obj.Key.BinNo);
                return result;
            }
            #endregion

            #region 判断该BIN是否已经清Bin
            cfg = new PagingConfig()
            {
                Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}' and BinPackaged = 0", obj.Key.BinNo, obj.Key.PackageLine),
                IsPaging = false,
                PageSize=1,
                OrderBy = "EditTime desc"
            };
            IList<PackageBin> listPackageBin = this.PackageBinDataEngine.Get(cfg);
            if (listPackageBin.Count > 0)
            {
                result.Code = 1002;
                result.Message = String.Format("线别[{0}]新增[{1}]Bin需将该[{1}]清Bin", obj.Key.PackageLine, obj.Key.BinNo);
                return result;
            }
            #endregion

            try
            {
                this.BinRuleDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改客户端配置属性。
        /// </summary>
        /// <param name="obj">客户端配置属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(BinRule obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.BinRuleDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.BinRuleService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.BinRuleDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除客户端配置属性。
        /// </summary>
        /// <param name="key">客户端配置属性标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(BinRuleKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.BinRuleDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.BinRuleService_IsNotExists, key);
                return result;
            }

            #region 判断该BIN是否已经清Bin
            PagingConfig cfg = new PagingConfig()
            {
                Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}' and BinPackaged = 0", key.BinNo, key.PackageLine),
                IsPaging = false,
                PageSize = 1,
                OrderBy = "EditTime desc"
            };
            IList<PackageBin> listPackageBin = this.PackageBinDataEngine.Get(cfg);
            if (listPackageBin.Count > 0)
            {
                result.Code = 1002;
                result.Message = String.Format("线别[{0}]删除[{1}]Bin需将该[{1}]清Bin", key.PackageLine, key.BinNo);
                return result;
            }
            #endregion

            try
            {
                this.BinRuleDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取客户端配置属性数据。
        /// </summary>
        /// <param name="key">客户端配置属性标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;BinRule&gt;" />,客户端配置属性数据.</returns>
        public MethodReturnResult<BinRule> Get(BinRuleKey key)
        {
            MethodReturnResult<BinRule> result = new MethodReturnResult<BinRule>();
            if (!this.BinRuleDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.BinRuleService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.BinRuleDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取客户端配置属性数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;BinRule&gt;" />,客户端配置属性数据集合。</returns>
        public MethodReturnResult<IList<BinRule>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<BinRule>> result = new MethodReturnResult<IList<BinRule>>();
            try
            {
                result.Data = this.BinRuleDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
    }
}
