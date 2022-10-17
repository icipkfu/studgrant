namespace Grant.WebApi.Controllers
{
    using System.Web.Mvc;
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return File("main.html", "text/html");
        }
    }
}
