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
    public class SPCJobService : ISPCJobContract
    {
        public ISPCJobDataEngine SPCJobDataEngine { set; get; }

        public ISPCJobParamsDataEngine SPCJobParamsDataEngine { set; get; }

        public MethodReturnResult Add(SPCJob obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.SPCJobDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.SPCJobService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.SPCJobDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {

                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult Modify(SPCJob obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.SPCJobDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SPCJobService_IsnNotExists, obj.Key);
                return result;
            }
            try
            {
                this.SPCJobDataEngine.Update(obj);
            }
            catch (Exception ex)
            {

                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.SPCJobDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SPCJobService_IsnNotExists, key);
                return result;
            }
            try
            {
                this.SPCJobDataEngine.Delete(key);
            }
            catch (Exception ex)
            {

                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult<SPCJob> Get(string key)
        {
            MethodReturnResult<SPCJob> result = new MethodReturnResult<SPCJob>();

            if (!this.SPCJobDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SPCJobService_IsnNotExists, key);
                return result;
            }
            try
            {
                result.Data= this.SPCJobDataEngine.Get(key);
            }
            catch (Exception ex)
            {

                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult<IList<SPCJob>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<SPCJob>> result = new MethodReturnResult<IList<SPCJob>>();
            try
            {
                result.Data = this.SPCJobDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }


        public MethodReturnResult<IList<SPCJobParam>> GetJobParams(string JobId)
        {
            MethodReturnResult<IList<SPCJobParam>> result = new MethodReturnResult<IList<SPCJobParam>>();
            try
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging=false,
                    Where=  string.Format("Key.JobId='{0}'",JobId)
                };
                result.Data = this.SPCJobParamsDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult AddJobParams(SPCJobParam obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.SPCJobParamsDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.SPCJobService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.SPCJobParamsDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {

                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult DeleteJobParams(SPCJobParamKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.SPCJobParamsDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SPCJobService_IsnNotExists, key);
                return result;
            }
            try
            {
                this.SPCJobParamsDataEngine.Delete(key);
            }
            catch (Exception ex)
            {

                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult ModifyJobParams(SPCJobParam obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.SPCJobParamsDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SPCJobService_IsnNotExists, obj.Key);
                return result;
            }
            try
            {
                this.SPCJobParamsDataEngine.Update(obj);
            }
            catch (Exception ex)
            {

                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult<SPCJobParam> GetSPCJobParam(SPCJobParamKey key)
        {
            MethodReturnResult<SPCJobParam> result = new MethodReturnResult<SPCJobParam>();

            if (!this.SPCJobDataEngine.IsExists(key.JobId))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SPCJobService_IsnNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.SPCJobParamsDataEngine.Get(key);
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
