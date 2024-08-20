using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using planner.Models;
using System.Diagnostics;

namespace planner.Controllers
{
    [Authorize]
    public class LayoutData
    {
        public string? Period;
        public double? OnlinePercentage;
        public double? RetailPercentage;
    }
    public class LayoutPieData
    {
        public string? Product;
        public double? Percentage;
        public string? text;
    }
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Home()
        {
            return View();
        }
        [Authorize]
        public IActionResult Index()
        {
            List<LayoutData> ColumnChartData = new List<LayoutData>
            {
                new LayoutData {Period = "2017", OnlinePercentage = 60, RetailPercentage = 40 },
                new LayoutData {Period = "2018", OnlinePercentage = 56, RetailPercentage = 44 },
                new LayoutData {Period = "2019", OnlinePercentage = 71, RetailPercentage = 29 },
                new LayoutData {Period = "2020", OnlinePercentage = 85, RetailPercentage = 15 },
                new LayoutData {Period = "2021", OnlinePercentage = 73, RetailPercentage = 27 }
            };
            ViewBag.columnSource = ColumnChartData;

            List<LayoutPieData> PieData = new List<LayoutPieData>
            {
                 new LayoutPieData{ Product = "TV : 30 (12%)",     Percentage = 12, text = "TV, 30<br>12%" },
                 new LayoutPieData{ Product = "PC : 20 (8%)",      Percentage = 8,  text = "PC, 20<br>8%" },
                 new LayoutPieData{ Product = "Laptop : 40 (16%)", Percentage = 16, text = "Laptop, 40<br>16%" },
                 new LayoutPieData{ Product = "Mobile : 90 (36%)", Percentage = 36, text = "Mobile, 90<br>36%" },
                 new LayoutPieData{ Product = "Camera : 27 (11%)", Percentage = 11, text = "Camera, 27<br>11%" },

            };
            ViewBag.pieSource = PieData;

            List<LayoutData> SplineChartData = new List<LayoutData>
            {
                    new LayoutData{ Period = "Jan", OnlinePercentage = 3600, RetailPercentage = 6400 },
                    new LayoutData{ Period = "Feb", OnlinePercentage = 6200, RetailPercentage = 5300 },
                    new LayoutData{ Period = "Mar", OnlinePercentage = 8100, RetailPercentage = 4900 },
                    new LayoutData{ Period = "Apr", OnlinePercentage = 5900, RetailPercentage = 5300 },
                    new LayoutData{ Period = "May", OnlinePercentage = 8900, RetailPercentage = 4200 },
                    new LayoutData{ Period = "Jun", OnlinePercentage = 7200, RetailPercentage = 6500 },
                    new LayoutData{ Period = "Jul", OnlinePercentage = 4300, RetailPercentage = 7900 },
                    new LayoutData{ Period = "Aug", OnlinePercentage = 4600, RetailPercentage = 3800 },
                    new LayoutData{ Period = "Sep", OnlinePercentage = 5500, RetailPercentage = 6800 },
                    new LayoutData{ Period = "Oct", OnlinePercentage = 6350, RetailPercentage = 3400 },
                    new LayoutData{ Period = "Nov", OnlinePercentage = 5700, RetailPercentage = 6400 },
                    new LayoutData{ Period = "Dec", OnlinePercentage = 8000, RetailPercentage = 6800 }
            };
            ViewBag.splineSource = SplineChartData;
            ViewBag.palettes = new string[] { "#61EFCD", "#CDDE1F", "#FEC200", "#CA765A", "#2485FA", "#F57D7D", "#C152D2",
                "#8854D9", "#3D4EB8", "#00BCD7", "#4472c4", "#ed7d31", "#ffc000", "#70ad47", "#5b9bd5", "#c1c1c1", "#6f6fe2", "#e269ae", "#9e480e", "#997300" };
            ViewBag.cellSpacing = new double[] { 10, 10 };
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
