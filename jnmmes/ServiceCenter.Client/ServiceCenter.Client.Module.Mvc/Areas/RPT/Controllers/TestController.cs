using ServiceCenter.MES.Service.Client.RPT;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;

namespace ServiceCenter.Client.Mvc.Areas.RPT.Controllers
{
    public class TestController : Controller
    {

        //
        // GET: /RPT/Test/
        public ActionResult Index()
        {
            using (WIPMoveServiceClient client = new WIPMoveServiceClient())
            {
                MethodReturnResult<DataSet> result = client.GetPackageYield();
                ViewBag.Key = Guid.NewGuid().ToString();
                this.HttpContext.Cache[ViewBag.Key] = result.Data;
            }
            return View();
        }


        public ActionResult ShowChartImage(string key){

            ViewBag.ChartData = this.HttpContext.Cache[key];

            //System.Web.UI.DataVisualization.Charting.Chart c = new System.Web.UI.DataVisualization.Charting.Chart();
            //c.Series[0].YAxisType = AxisType.Secondary;
            return View("ShowChartImage");
        }
	}
}