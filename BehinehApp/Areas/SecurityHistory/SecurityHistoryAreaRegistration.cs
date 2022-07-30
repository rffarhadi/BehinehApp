using System.Web.Mvc;

namespace BehinehApp.Areas.SecurityHistory
{
    public class SecurityHistoryAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SecurityHistory";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SecurityHistory_default",
                "SecurityHistory/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}