namespace Grant.WebApi
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.IgnoreRoute("{file}.json");
            routes.IgnoreRoute("{file}.html");
            routes.IgnoreRoute("{file}.js");
            routes.IgnoreRoute("{file}.png");
            routes.IgnoreRoute("{file}.jpg");
            routes.IgnoreRoute("{file}.jpeg");
            routes.IgnoreRoute("{file}.pdf");
            routes.IgnoreRoute("{file}.eot");
            routes.IgnoreRoute("{file}.svg");
            routes.IgnoreRoute("{file}.ttf");
            routes.IgnoreRoute("{file}.woff");
            routes.IgnoreRoute("{file}.woff2");
            routes.IgnoreRoute("{file}.otf");
            routes.IgnoreRoute("{file}.css");
            routes.IgnoreRoute("{file}.dbf");
            routes.IgnoreRoute("{file}.zip");
            

        }
    }
}