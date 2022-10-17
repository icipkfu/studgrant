namespace Grant.Core.Context
{
    using System.IO;
    using System.Web;
    using System.Web.Hosting;
    using LightInject;


    public class WebAppContext : AppContext
    {
        public WebAppContext(IServiceContainer container) : base(container)
        {
        }

        public override string MapPath(string virtualPath)
        {
            return Path.Combine(HostingEnvironment.ApplicationPhysicalPath, virtualPath);
        }

        public override string ApplicationPhysicalPath()
        {
            return HostingEnvironment.ApplicationPhysicalPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetUrl()
        {
            var url = HttpContext.Current.Request.Url;

            return string.Format("{0}://{1}:{2}/", url.Scheme, url.Host, url.Port);
        }
    }
}