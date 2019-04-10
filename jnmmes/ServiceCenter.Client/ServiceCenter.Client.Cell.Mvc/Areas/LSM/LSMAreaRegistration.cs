using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.LSM
{
    public class LSMAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "LSM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "LSM_default",
                "LSM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}