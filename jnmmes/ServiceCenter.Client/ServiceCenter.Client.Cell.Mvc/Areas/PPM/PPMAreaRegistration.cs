using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.PPM
{
    public class PPMAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PPM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PPM_default",
                "PPM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}