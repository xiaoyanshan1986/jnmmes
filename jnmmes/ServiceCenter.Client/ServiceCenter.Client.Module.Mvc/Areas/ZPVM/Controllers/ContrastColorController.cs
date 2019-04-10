using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ServiceCenter.Model;
using ServiceCenter.Service.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class ContrastColorController : Controller
    {
        //
        // GET: /ZPVM/ContrastColor/
        public ActionResult Index()
        {
            return View("Index", new ContrastColorViewModel());
        }

        public ActionResult Query(ContrastColorViewModel mode)
        {
            String countTotal = string.Format(@"select count(*) from dbo.v_LotColor where create_time>'{0}' and create_time<'{1}'", mode.StartTestTime, mode.EndTestTime);
            String countSame = string.Format(@"select count(*)  from dbo.v_LotColor where  Color=InspctResult and create_time>'{0}' and create_time<'{1}'", mode.StartTestTime, mode.EndTestTime);

            DataTable dtCountTotal = new DataTable();
            DataTable dtCountSame = new DataTable();
            String Proportion;
            using (DBServiceClient client = new DBServiceClient())
            {

                MethodReturnResult<DataTable> resultCountTotal = client.ExecuteQuery(countTotal);
                MethodReturnResult<DataTable> resultCountSame = client.ExecuteQuery(countSame);
                if (resultCountSame.Code == 0 && resultCountTotal.Code==0)
                {                 
                    dtCountSame = resultCountSame.Data;
                    dtCountTotal = resultCountTotal.Data;
                    Proportion = (Convert.ToDouble(dtCountSame.Rows[0][0]) / Convert.ToDouble(dtCountTotal.Rows[0][0])*100).ToString().Substring(0,4)+"%";
                    ViewBag.Proportion = Proportion;
                }
            }
            return PartialView("_ListPartial");
        }
	}
}