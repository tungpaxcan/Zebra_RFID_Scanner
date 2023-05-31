using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Zebra_RFID_Scanner
{
    public class RouteConfig
    {
       
        public static void RegisterRoutes(RouteCollection routes)
        {
            ResourceManager rm = new ResourceManager("Zebra_RFID_Scanner.Resources.Resource", typeof(Resources.Resource).Assembly);
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
              name: "FunctionOrder",
              url: "123-rfid-scanner/{meta}/{id}",
              defaults: new { controller = "FunctionOrder", action = "Index", id = UrlParameter.Optional }
          );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
