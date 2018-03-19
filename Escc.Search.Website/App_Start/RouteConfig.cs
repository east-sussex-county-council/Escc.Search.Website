using System.Web.Mvc;
using System.Web.Routing;

namespace Escc.Search.Website.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "PreserveWebFormsPath",
                url: "{controller}.aspx",
                defaults: new { controller = "Search", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}",
                defaults: new { controller = "Search", action = "Index" }
            );

        }
    }
}