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
    /// 实现班别管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ShiftService : IShiftContract
    {
        /// <summary>
        /// 班别数据访问读写。
        /// </summary>
        public IShiftDataEngine ShiftDataEngine { get; set; }


        /// <summary>
        /// 添加班别。
        /// </summary>
        /// <param name="obj">班别数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Shift obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ShiftDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ShiftService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ShiftDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改班别。
        /// </summary>
        /// <param name="obj">班别数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Shift obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ShiftDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ShiftService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.ShiftDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除班别。
        /// </summary>
        /// <param name="name">班别名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string name)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ShiftDataEngine.IsExists(name))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ShiftService_IsNotExists, name);
                return result;
            }
            try
            {
                this.ShiftDataEngine.Delete(name);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取班别数据。
        /// </summary>
        /// <param name="name">班别名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;Shift&gt;" />,班别数据.</returns>
        public MethodReturnResult<Shift> Get(string name)
        {
            MethodReturnResult<Shift> result = new MethodReturnResult<Shift>();
            if (!this.ShiftDataEngine.IsExists(name))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ShiftService_IsNotExists, name);
                return result;
            }
            try
            {
                result.Data = this.ShiftDataEngine.Get(name);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取班别数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Shift&gt;" />,班别数据集合。</returns>
        public MethodReturnResult<IList<Shift>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Shift>> result = new MethodReturnResult<IList<Shift>>();
            try
            {
                result.Data = this.ShiftDataEngine.Get(cfg);
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
