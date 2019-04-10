using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
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
    /// 实现生产线管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ProductionLineService : IProductionLineContract
    {
        /// <summary>
        /// 生产线数据访问读写。
        /// </summary>
        public IProductionLineDataEngine ProductionLineDataEngine { get; set; }

        /// <summary>
        /// 生产线数据访问读写。
        /// </summary>
        public IBinRuleDataEngine BinRuleDataEngine { get; set; }

        /// <summary>
        /// 添加生产线。
        /// </summary>
        /// <param name="obj">生产线数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(ProductionLine obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ProductionLineDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ProductionLineService_IsExists, obj.Key);
                return result;
            }
            if (obj.Attr2 != "")
            {
                PagingConfig cfg = new PagingConfig()
                {
                    Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}'", obj.Attr2, obj.Key),
                    IsPaging=false
                };
                IList<BinRule> listBinRule = this.BinRuleDataEngine.Get(cfg);
                if (listBinRule.Count > 0)
                {
                    result.Code = 1002;
                    result.Message = string.Format("线别：{0}BIN：{1}的规则已经设置，请先删除", obj.Key, obj.Attr2);
                    return result;
                }
            }
        
            if (this.ProductionLineDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ProductionLineService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ProductionLineDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改生产线。
        /// </summary>
        /// <param name="obj">生产线数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(ProductionLine obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ProductionLineDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ProductionLineService_IsNotExists, obj.Key);
                return result;
            }
            if (obj.Attr2 != "")
            {
                PagingConfig cfg = new PagingConfig()
                {
                    Where = string.Format("Key.BinNo='{0}' and Key.PackageLine='{1}'", obj.Attr2, obj.Key),
                    IsPaging = false
                };
                IList<BinRule> listBinRule = this.BinRuleDataEngine.Get(cfg);
                if (listBinRule.Count > 0)
                {
                    result.Code = 1002;
                    result.Message = string.Format("线别：{0}BIN：{1}的规则已经设置，请先删除", obj.Key, obj.Attr2);
                    return result;
                }
            }
            try
            {
                this.ProductionLineDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除生产线。
        /// </summary>
        /// <param name="key">生产线代码。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ProductionLineDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ProductionLineService_IsNotExists, key);
                return result;
            }
            try
            {
                this.ProductionLineDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取生产线数据。
        /// </summary>
        /// <param name="key">生产线代码.</param>
        /// <returns><see cref="MethodReturnResult&lt;ProductionLine&gt;" />,生产线数据.</returns>
        public MethodReturnResult<ProductionLine> Get(string key)
        {
            MethodReturnResult<ProductionLine> result = new MethodReturnResult<ProductionLine>();
            if (!this.ProductionLineDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ProductionLineService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.ProductionLineDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取生产线数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;ProductionLine&gt;" />,生产线数据集合。</returns>
        public MethodReturnResult<IList<ProductionLine>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<ProductionLine>> result = new MethodReturnResult<IList<ProductionLine>>();
            try
            {
                result.Data = this.ProductionLineDataEngine.Get(cfg);
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
