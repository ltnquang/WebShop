using System.Web.Mvc;

namespace DemoWeb.Areas.DOTNETGROUP
{
    public class DOTNETGROUPAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DOTNETGROUP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DOTNETGROUP_default",
                "DOTNETGROUP/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}