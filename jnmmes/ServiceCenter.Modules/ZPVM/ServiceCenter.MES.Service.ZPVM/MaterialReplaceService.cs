using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.MES.Service.ZPVM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ServiceCenter.MES.Service.ZPVM
{
    /// <summary>
    /// 实现物料替换规则数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MaterialReplaceService : IMaterialReplaceContract
    {
        /// <summary>
        /// 物料替换规则数据数据访问读写。
        /// </summary>
        public IMaterialReplaceDataEngine MaterialReplaceDataEngine { get; set; }       

        /// <summary>
        /// 添加物料替换规则数据。
        /// </summary>
        /// <param name="obj">物料替换规则数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialReplace obj)
        {
            MethodReturnResult result = new MethodReturnResult();

            PagingConfig cfg = new PagingConfig()
            {
                Where = string.Format(@"Key.ProductCode ='{0}' 
                                    AND Key.OrderNumber = '{1}'
                                    AND Key.OldMaterialCode='{2}'
                                    AND Key.OldMaterialSupplier='{3}'"
                                    , obj.Key.ProductCode
                                    , "*"
                                    , obj.Key.OldMaterialCode
                                    , obj.Key.OldMaterialSupplier)              
            };
            MethodReturnResult<IList<MaterialReplace>> materialData = Gets(ref cfg);

            PagingConfig cfgs = new PagingConfig()
            {
                Where = string.Format(@"Key.ProductCode ='{0}' 
                                    AND Key.OrderNumber = '{1}'
                                    AND Key.OldMaterialCode='{2}'
                                    AND Key.OldMaterialSupplier='{3}'"
                                    , obj.Key.ProductCode
                                    , obj.Key.OrderNumber
                                    , obj.Key.OldMaterialCode
                                    , "*")
            };
            MethodReturnResult<IList<MaterialReplace>> materialDatas = Gets(ref cfgs);

            if (materialDatas.Data.Count > 0)
            {
                result.Code = 1001;
                result.Message = String.Format("新增规则({0})与规则（{1}:{2}:{3}:*）冲突！若要新增，请删除原规则！", obj.Key, obj.Key.ProductCode, obj.Key.OrderNumber, obj.Key.OldMaterialCode);
                return result;
            }
            if (materialData.Data.Count > 0)
            {                
                result.Code = 1001;
                result.Message = String.Format("新增规则({0})与规则（{1}:*:{2}:{3}）冲突！若要新增，请删除原规则！",obj.Key,obj.Key.ProductCode,obj.Key.OldMaterialCode,obj.Key.OldMaterialSupplier);
                return result;
            }
            else
            {
                if (this.MaterialReplaceDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.MaterialReplaceService_IsExists, obj.Key);
                    return result;
                }
                try
                {
                    this.MaterialReplaceDataEngine.Insert(obj);
                }
                catch (Exception ex)
                {
                    result.Code = 1000;
                    result.Message = String.Format(StringResource.Error, ex.Message);
                    result.Detail = ex.ToString();
                }
                return result;
            }           
        }


        /// <summary>
        /// 修改物料替换规则数据。
        /// </summary>
        /// <param name="obj">物料替换规则数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(MaterialReplace obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialReplaceDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialReplaceService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                ISession session = this.MaterialReplaceDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.MaterialReplaceDataEngine.Update(obj, session);
                    transaction.Commit();
                    session.Close();
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
        /// 删除物料替换规则数据。
        /// </summary>
        /// <param name="key">物料替换规则主键。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(MaterialReplaceKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialReplaceDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialReplaceService_IsNotExists, key);
                return result;
            }
            try
            {
                ISession session = this.MaterialReplaceDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.MaterialReplaceDataEngine.Delete(key, session);                    
                    transaction.Commit();
                    session.Close();
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
        /// 获取物料替换规则数据。
        /// </summary>
        /// <param name="key">物料替换规则主键.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReplace&gt;" />,物料替换规则数据数据.</returns>
        public MethodReturnResult<MaterialReplace> Get(MaterialReplaceKey key)
        {
            MethodReturnResult<MaterialReplace> result = new MethodReturnResult<MaterialReplace>();
            if (!this.MaterialReplaceDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialReplaceService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialReplaceDataEngine.Get(key);
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
        /// 获取物料替换规则数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReplace&gt;" />,物料替换规则数据数据集合。</returns>
        public MethodReturnResult<IList<MaterialReplace>> Gets(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialReplace>> result = new MethodReturnResult<IList<MaterialReplace>>();
            try
            {
                result.Data = this.MaterialReplaceDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
    }
}
