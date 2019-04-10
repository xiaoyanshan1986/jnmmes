using ServiceCenter.Client.Mvc.Areas.WIP.Models;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using ServiceCenter.Service.Client;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using System.Speech.Synthesis;
using System.Threading;
using System.IO;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotMateSEModulesController : Controller
    {
        /// <summary> 显示优化器匹配作业界面 </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(new LotMateSeModel());
        }

        MethodReturnResult GetLot(string lotNumber)
        {
            bool IsMapChkLotState = false;
            //获取是否允许无限制条件(批次状态)匹配优化器
            using (BaseAttributeValueServiceClient ClientOfBASE = new BaseAttributeValueServiceClient())
            {
                IList<BaseAttributeValue> lstBaseAttributeValue = new List<BaseAttributeValue>();
                PagingConfig pg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='SystemParameters' and Key.AttributeName='MapChkLotState' ")
                };
                MethodReturnResult<IList<BaseAttributeValue>> r = ClientOfBASE.Get(ref pg);
                if (r.Code <= 0 && r.Data != null)
                {
                    lstBaseAttributeValue = r.Data;
                    IsMapChkLotState = Convert.ToBoolean(lstBaseAttributeValue[0].Value);
                }
            }
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<Lot> rst = null;
            Lot obj = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                rst = client.Get(lotNumber);
                if (rst.Code == 0 && rst.Data != null)
                {
                    obj = rst.Data;
                }
                else
                {
                    result.Code = rst.Code;
                    result.Message = rst.Message;
                    result.Detail = rst.Detail;
                    return result;
                }
            }

            if (obj == null || obj.Status == EnumObjectStatus.Disabled)
            {
                result.Code = 2001;
                result.Message = string.Format(WIPResources.StringResource.LotIsNotExists, lotNumber);
                return result;
            }
            if (IsMapChkLotState)
            {
                if (obj.StateFlag == EnumLotState.Finished)
                {
                    result.Code = 2002;
                    result.Message = string.Format("批次({0})已完成。", lotNumber);
                    return result;
                }
                else if (obj.Status == EnumObjectStatus.Disabled || obj.DeletedFlag == true)
                {
                    result.Code = 2003;
                    result.Message = string.Format("批次({0})已结束。", lotNumber);
                    return result;
                }
                else if (obj.HoldFlag == true)
                {
                    string res = null;
                    string res2 = null;
                    string sql = string.Format(@"select ATTR_4  from WIP_LOT where LOT_NUMBER='{0}'", lotNumber);
                    DataTable dt = new DataTable();
                    using (DBServiceClient client = new DBServiceClient())
                    {
                        MethodReturnResult<DataTable> dtResult = client.ExecuteQuery(sql);
                        if (result.Code == 0)
                        {
                            dt = dtResult.Data;
                            res = dt.Rows[0][0].ToString();
                        }
                    }

                    string sql2 = string.Format(@"select top 1 t2.HOLD_DESCRIPTION  from  WIP_TRANSACTION  t1
                                                   inner join [dbo].[WIP_TRANSACTION_HOLD_RELEASE]  t2 on  t1.TRANSACTION_KEY=t2.TRANSACTION_KEY
                                                   inner join WIP_LOT t3  on t3.LOT_NUMBER = t1.LOT_NUMBER  
                                                   where t1.LOT_NUMBER='{0}'
                                                   order by t2.HOLD_TIME  desc", lotNumber);
                    DataTable dt2 = new DataTable();
                    using (DBServiceClient client2 = new DBServiceClient())
                    {
                        MethodReturnResult<DataTable> dtResult2 = client2.ExecuteQuery(sql2);
                        if (result.Code == 0 && dtResult2.Data != null && dtResult2.Data.Rows.Count > 0)
                        {
                            dt2 = dtResult2.Data;
                            res2 = dt2.Rows[0][0].ToString();
                        }
                    }

                    if (dt != null && dt.Rows.Count > 0 && res != null && res != "")
                    {
                        result.Code = 2004;
                        result.Message = string.Format("批次（{0}）已暂停,原因为：{1}。", lotNumber, res);
                    }
                    else if (dt != null && dt.Rows.Count > 0 && res2 != null && res2 != "")
                    {
                        result.Code = 2004;
                        result.Message = string.Format("批次（{0}）已暂停。", lotNumber);
                    }
                    else
                    {
                        result.Code = 2004;
                        result.Message = string.Format("批次（{0}）已暂停。", lotNumber);
                    }
                    return result;
                }
            }            
            return rst;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(LotMateSeModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            int count = 0;
            try
            {
                string lotNumber = model.LotNumber.ToUpper();
                result = GetLot(lotNumber);
                if (result.Code > 0)
                {
                    return Json(result);
                }
                //取得批次信息
                MethodReturnResult<Lot> rst = result as MethodReturnResult<Lot>;
                Lot obj = rst.Data;

                MaterialAttributeServiceClient clientOfMattr = new MaterialAttributeServiceClient();
                MaterialAttributeKey materialAttributeKey = new MaterialAttributeKey()
                {
                    MaterialCode = obj.MaterialCode,
                    AttributeName = "MapBitCount"
                };
                MethodReturnResult<MaterialAttribute> materialAttributeOfMap = clientOfMattr.Get(materialAttributeKey);
                if (materialAttributeOfMap.Code == 0 && materialAttributeOfMap.Data != null)
                {
                    count = Convert.ToInt32(materialAttributeOfMap.Data.Value);
                }

                if (count != 0 && model.SEModulesNo.Length != count)
                {
                    result.Code = 1000;
                    result.Message = string.Format("组件批次{0}优化器序列号{1}长度不符合长度限制{2}", model.LotNumber, model.SEModulesNo, count);
                    return Json(result);
                }
                else if(count != 0 && model.SEModulesNo.Length == count)
                {
                    if (model.SEModulesNo.Length == 8)
                    {
                        int seModulesNo = 0;
                        bool isNum = int.TryParse(model.SEModulesNo, out seModulesNo);
                        if (isNum)
                        {
                            obj.Attr3 = model.SEModulesNo;
                        }
                        else
                        {
                            result.Code = 1000;
                            result.Message = string.Format("组件批次{0}优化器序列号{1}不合法", model.LotNumber, model.SEModulesNo);
                            return Json(result);
                        }
                    }
                    else
                    {
                        obj.Attr3 = model.SEModulesNo;
                    }
                } 
                else
                {
                    obj.Attr3 = model.SEModulesNo;
                }
                using (LotCreateServiceClient client = new LotCreateServiceClient())
                {
                    result=  client.UpdateLotSEModules(obj);
                    if (result.Code == 0)
                    {
                        result.Message =string.Format("组件批次{0}优化器序列号{1}匹配成功",model.LotNumber,model.SEModulesNo);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return Json(result);
        }
	}
}

