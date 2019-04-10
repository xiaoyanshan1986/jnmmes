using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.FMM
{
    public class FMMAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FMM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FMM_default",
                "FMM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}