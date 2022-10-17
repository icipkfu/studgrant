namespace Grant.WebApi.Controllers
{
    using System.Web.Http;
    using Grant.Core.Context;
    using System.Web;
    using Microsoft.Owin.Security;

    using LightInject;

    public class BaseController : ApiController
    {
        public IServiceContainer Container
        {
            get
            {
                return ApplicationContext.Current.Container;
            }
        }

        protected IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }
    }
}