using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace newswebsite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
    name: "admin",
    url: "admin/{action}/{id}",
    defaults: new { controller = "admin", habername = "", action = "Index", id = UrlParameter.Optional }
);
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "admin", habername = "", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
