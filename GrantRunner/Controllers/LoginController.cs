using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;


namespace Grant.WebApi.Controllers
{
    public class LoginController : Controller
    {
        [Route("login")]
        public ActionResult Index()
        {
            return File(@"login.html", "text/html");
        }
    }
}
