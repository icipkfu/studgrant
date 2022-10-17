using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace Grant.WebApi.Controllers
{
    public class JoinController : Controller
    {
        [Route("join")]
        public ActionResult Index()
        {
            return File(@"join.html", "text/html");
        }
    }
}
