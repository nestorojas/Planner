using Microsoft.AspNetCore.Mvc;

namespace planner.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Task()
        {
            return View();
        }
    }
}
