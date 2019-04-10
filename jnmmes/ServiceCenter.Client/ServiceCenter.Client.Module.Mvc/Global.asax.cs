using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ServiceCenter.Common;
using ServiceCenter.Common.Print;

namespace ServiceCenter.Client.Mvc
{
    
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {            
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ServiceCenter.Common.LogHelper helper = new Common.LogHelper();
            LogHelper.WriteLogInfo("START>Application");
        }

        protected void Application_AcquireRequestState()
        {
            
        }


        protected void Application_End()
        {
            //if (PrintHelperFactory.DicPrintDocuments != null)
            //{
            //    try
            //    {
            //        foreach (string strKey in PrintHelperFactory.DicPrintDocuments.Keys)
            //        {
            //            PrintHelperFactory.DicPrintDocuments[strKey].Dispose();
            //        }
            //    }
            //    catch
            //    {

            //    }

            //}
        }

        
    }
}
