﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DemoShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Cart", "Cart/{action}/{id}", new { controller = "Cart", action = "Index", id = UrlParameter.Optional }, new[] { "DemoShop.Controllers" });


            routes.MapRoute("Shop", "Shop/{action}/{name}",new { controller = "Shop", action = "Index", name = UrlParameter.Optional }, new[] { "DemoShop.Controllers" });


            routes.MapRoute("PagesMenuPartial", "Pages/PagesMenuPartial", new { controller = "Pages", action = "PagesMenuPartial" }, new[] { "DemoShop.Controllers" });


            routes.MapRoute("SidebarPartial", "Pages/SidebarPartial", new { controller = "Pages", action = "SidebarPartial" }, new[] { "DemoShop.Controllers" });


            routes.MapRoute("Pages", "{page}", new { controller = "Pages", action = "Index" }, new[] { "DemoShop.Controllers" });


            routes.MapRoute("Default", "", new { controller = "Pages", action = "Index" }, new[] { "DemoShop.Controllers" });


            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
