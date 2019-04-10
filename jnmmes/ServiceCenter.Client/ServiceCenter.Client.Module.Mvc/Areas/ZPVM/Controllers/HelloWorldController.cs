using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ServiceCenter.MES.Service.Client.ZPVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class HelloWorldController : Controller
    {
        //
        // GET: /ZPVM/HelloWorld/
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult Show(HelloWorldViewModel model)
        {
            string result = string.Empty;

            using (HelloWorldServiceClient client = new HelloWorldServiceClient())
            {
                result = client.Hello(model.Name);
            }

            return Content(result);
        }
	}
}