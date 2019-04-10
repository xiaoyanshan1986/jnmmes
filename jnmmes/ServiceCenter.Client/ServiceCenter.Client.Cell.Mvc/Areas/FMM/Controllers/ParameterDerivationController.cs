using ServiceCenter.Client.Mvc.Areas.FMM.Models;
using FMMResources = ServiceCenter.Client.Mvc.Resources.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Controllers
{
    public class ParameterDerivationController : Controller
    {

        //
        // GET: /FMM/ParameterDerivation/
        public async Task<ActionResult> Index(string derivedParameterName)
        {
            using (ParameterServiceClient client = new ParameterServiceClient())
            {
                MethodReturnResult<Parameter> result = await client.GetAsync(derivedParameterName ?? string.Empty);
                if (result.Code > 0 || result.Data == null || result.Data.IsDerived==false)
                {
                    return RedirectToAction("Index", "Parameter");
                }
                ViewBag.DerivedParamter = result.Data;
            }

            using (ParameterDerivationServiceClient client = new ParameterDerivationServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig() { 
                        IsPaging=false,
                        OrderBy = "ItemNo",
                        Where = string.Format(" Key.DerivedParameterName = '{0}'"
                                                    , derivedParameterName)
                    };
                    MethodReturnResult<IList<ParameterDerivation>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new ParameterDerivationQueryViewModel() { DerivedParameterName = derivedParameterName });
        }

        //
        //POST: /FMM/ParameterDerivation/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(ParameterDerivationQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (ParameterDerivationServiceClient client = new ParameterDerivationServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.DerivedParameterName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.DerivedParameterName);

                            if (!string.IsNullOrEmpty(model.ParameterName))
                            {
                                where.AppendFormat(" {0} Key.ParameterName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ParameterName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging=false,
                            OrderBy="ItemNo",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<ParameterDerivation>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial");
        }
        
        //
        // POST: /FMM/ParameterDerivation/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(ParameterDerivationViewModel model)
        {
            using (ParameterDerivationServiceClient client = new ParameterDerivationServiceClient())
            {
                ParameterDerivation obj = new ParameterDerivation()
                {
                    Key = new ParameterDerivationKey() {
                        DerivedParameterName = model.DerivedParameterName,
                        ParameterName = model.ParameterName
                    },
                    ItemNo=model.ItemNo,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime=DateTime.Now,
                    Creator=User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.ParameterDerivation_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // POST: /FMM/ParameterDerivation/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string derivedParameterName, string parameterName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (ParameterDerivationServiceClient client = new ParameterDerivationServiceClient())
            {
                result = await client.DeleteAsync(new ParameterDerivationKey()
                {
                    DerivedParameterName = derivedParameterName,
                    ParameterName = parameterName
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.ParameterDerivation_Delete_Success
                                                    , parameterName);
                }
                return Json(result);
            }
        }
    }
}