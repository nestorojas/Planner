using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using planner.Services;
using planner.ViewModel;


namespace planner.Controllers
{
    public class AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, Planner planner ) : Controller
    {
        private readonly Planner _planner = planner;
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                //login
                var result = await signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }
            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    Address = model.Address
                };

                var result = await userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User) ?? new AppUser();
            var team = _planner.GetTeam(user.TeamId) ?? new Team();
            var TeamRequested = _planner.GetRequestAccessByEmail(user.Email!);
            var model = new ProfileVM
            {
                Name = user.Name,
                Email = user.Email,
                TeamId = user.TeamId ?? 0,
                CreateTeam = user.TeamId != null && user.TeamId != 0,
                TeamName = team.Name,
                TeamOwnerEmail = team.OwnerEmail,
                IsTeamOwner = user.Email == team.OwnerEmail,
                TeamRequestAccess = TeamRequested
            };
            if(team.OwnerEmail == user.Email)
            {
                var usersWithTeamId = userManager.Users.Where(u => u.TeamId == model.TeamId).ToList();
                if(usersWithTeamId.Any())
                {
                    model.TeamMembers = usersWithTeamId.Where(x => x.Email != model.Email).Select(x => new TeamMembers
                    {
                        Email = x.Email,
                        Name = x.Name,
                    }).ToList();
                }

                model.TeamAccess = _planner.GetRequestAccess(user.TeamId);
            }
            ModelState.Clear();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RequestAccess(ProfileVM model)
        {          
            if (model.CreateTeam)
            {
                var user = await userManager.FindByEmailAsync(model.Email!) ?? new AppUser();
                var team = new Team
                {
                    Name = model.TeamName,
                    OwnerEmail = user!.Email!
                };
                _planner.AddTeam(team);
                user.TeamId = team.Id;
                await userManager.UpdateAsync(user);
                return RedirectToAction("Profile");
            }
            else
            {
                var user = await userManager.FindByEmailAsync(model.TeamOwnerEmail!) ?? new AppUser();
                if (user!.TeamId == null)
                {
                    model.CreateTeam = true;
                    ViewBag.ModelError = "Team not found review: Team Admin Email";
                    ModelState.Clear();
                    return View("Profile", model);
                }
                else
                {
                    var team = _planner.GetTeam(user!.TeamId) ?? new Team();
                    var requestor = new RequestTeamAccess
                    {
                        TeamId = team.Id,
                        EmailOwner = user!.Email!,
                        UserRequestorEmail = model.Email!,
                    };
                    _planner.RequestTeamAccess(requestor);
                    return RedirectToAction("Profile");
                }              
            }
        }
        [HttpPost]
        public async Task<IActionResult> ExitAccess(ProfileVM model)
        {
            var user = await userManager.FindByEmailAsync(model.Email!) ?? new AppUser();
            user.TeamId = 0;
            await userManager.UpdateAsync(user);
            return RedirectToAction("Profile");
        }
        [HttpPost]
        public async Task<IActionResult> AcceptTeamMemberRequest(int requestId)
        {
            var request = _planner.GetRequestAccessById(requestId);
            var user = await userManager.FindByEmailAsync(request.UserRequestorEmail!) ?? new AppUser();
            user.TeamId = request.TeamId;
            await userManager.UpdateAsync(user);
            _planner.UpdateRequestTeamAccess(request);

            return RedirectToAction("Profile");
        }
        [HttpPost]
        public IActionResult RejectTeamMemberRequest(int requestId)
        {
            var request = _planner.GetRequestAccessById(requestId);
            _planner.UpdateRequestTeamAccess(request);

            return RedirectToAction("Profile");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveTeamMemberRequest(string email)
        {
            var user = await userManager.FindByEmailAsync(email!) ?? new AppUser();
            user.TeamId = 0;
            await userManager.UpdateAsync(user);
            return RedirectToAction("Profile");
        }
        [HttpPost]
        public async Task<IActionResult> ExitTeam(ProfileVM model)
        {
            var user = await userManager.FindByEmailAsync(model.Email!) ?? new AppUser();
            user.TeamId = 0;
            await userManager.UpdateAsync(user);
            return RedirectToAction("Profile");
        }
    }
}
