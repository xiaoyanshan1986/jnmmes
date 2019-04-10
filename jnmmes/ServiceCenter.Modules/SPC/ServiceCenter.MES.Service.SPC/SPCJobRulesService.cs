using ServiceCenter.MES.DataAccess.Interface.SPC;
using ServiceCenter.MES.Model.SPC;
using ServiceCenter.MES.Service.Contract.SPC;
using ServiceCenter.MES.Service.SPC.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.SPC
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SPCJobRulesService : ISPCJobRulesContract
    {
        public ISPCJobRulesDataEngine SPCJobRulesDataEngine { set; get; }

        public MethodReturnResult Add(SPCJobRules obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.SPCJobRulesDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.SPCJobService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.SPCJobRulesDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {

                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult Modify(SPCJobRules obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.SPCJobRulesDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SPCJobService_IsnNotExists, obj.Key);
                return result;
            }
            try
            {
                this.SPCJobRulesDataEngine.Update(obj);
            }
            catch (Exception ex)
            {

                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult Delete(SPCJobRulesKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.SPCJobRulesDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SPCJobService_IsnNotExists, key);
                return result;
            }
            try
            {
                this.SPCJobRulesDataEngine.Delete(key);
            }
            catch (Exception ex)
            {

                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult<SPCJobRules> Get(SPCJobRulesKey key)
        {
            MethodReturnResult<SPCJobRules> result = new MethodReturnResult<SPCJobRules>();

            if (!this.SPCJobRulesDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SPCJobService_IsnNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.SPCJobRulesDataEngine.Get(key);
            }
            catch (Exception ex)
            {

                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult<IList<SPCJobRules>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<SPCJobRules>> result = new MethodReturnResult<IList<SPCJobRules>>();
            try
            {
                result.Data = this.SPCJobRulesDataEngine.Get(cfg);
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
