using System.Web.Mvc;
using System.Web.Routing;

namespace Mood
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "SurveyRoute",
                url: "s/{id}",
                defaults: new { controller = "Survey", action = "Get", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ResultsRoute",
                url: "r/{id}",
                defaults: new { controller = "Answer", action = "Results" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
