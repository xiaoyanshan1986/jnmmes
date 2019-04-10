using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM
{
    public class ZPVMAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ZPVM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ZPVM_default",
                "ZPVM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}