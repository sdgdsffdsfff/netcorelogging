using Microsoft.AspNetCore.Mvc;

namespace Logging.Server.Site.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("this is netcorelogging server !");
        }
    }
}