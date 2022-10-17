using Microsoft.Owin;
using Microsoft.Owin.Cors;
[assembly: OwinStartup(typeof(Grant.WebApi.Startup))]

namespace Grant.WebApi
{
    using Grant.Core.Context;
    using Owin;
    using System.Web.Mvc;
    using System.Web.Http;
    using System.Web.Routing;
    using System.Web.Optimization;
    using OwinConfiguration;
    using Core.DbContext;
    using Core.Migrations;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use<Logger>();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

           // System.Data.Entity.Database.SetInitializer<GrantDbContext>(new System.Data.Entity.MigrateDatabaseToLatestVersion<GrantDbContext, Configuration>());

            ApplicationContext.Initialize<WebAppContext>();

            app.UseCors(CorsOptions.AllowAll);

            this.ConfigureAuth(app);
        }
    }
}
