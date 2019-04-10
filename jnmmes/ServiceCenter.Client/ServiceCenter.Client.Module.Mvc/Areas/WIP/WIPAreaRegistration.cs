using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.WIP
{
    public class WIPAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WIP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "WIP_default",
                "WIP/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}