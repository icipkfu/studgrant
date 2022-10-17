using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;


namespace Grant.WebApi.Controllers
{
    public class MyController : Controller
    {
        [Route("my")]
        public ActionResult Index()
        {
            return File(@"my.html", "text/html");
        }
    }
}
