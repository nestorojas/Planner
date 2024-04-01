using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using planner.Services;
using planner.ViewModel;

namespace planner.Controllers
{
    [Authorize]
    public class ClientController(UserManager<AppUser> userManager, Planner planner) : Controller
    {
        private readonly Planner _planner = planner;
        public IActionResult Client()
        {
            var model = _planner.GetClients();
            return View(model);
        }
    }
}
