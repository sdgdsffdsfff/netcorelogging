using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Logging.Client.Examples.Controllers
{
    public class HomeController : Controller
    {

        readonly static ILog logger = LogManager.GetLogger(typeof(HomeController));

        public IActionResult Index()
        {

            //for (int i = 0; i < 100; i++)
            //{
            //    logger.Debug("test");
            //    logger.Info("aabbbbbcc", "test");
            //    logger.Warn("test", "大大的打算打算大大", null);
            //    logger.Error("test", "大大的打算打算大大", null);
            //}

            for (int i = 0; i < 1000; i++)
            {
                logger.Metric("csharp_test", 100, new Tags { { "tag1", "val1" } });
            }

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
