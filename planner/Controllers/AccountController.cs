using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using planner.Services;
using planner.ViewModel;
using System.Xml.Linq;


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
                    var user = await userManager.GetUserAsync(User) ?? new AppUser();
                    if (user.TeamId > 0)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Profile", "Account");
                    }
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
                    Address = model.Address,
                    IsTeamManager = true
                };

                var result = await userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    var team = new Team
                    {
                        Name = user.Name!,
                        OwnerEmail = user!.Email!
                    };
                    _planner.AddTeam(team);
                    user.TeamId = team.Id;

                    await userManager.UpdateAsync(user);

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
                TeamId = user.TeamId,
                CreateTeam = user.TeamId != 0,
                TeamName = team.Name,
                TeamOwnerEmail = team.OwnerEmail,
                IsTeamOwner = user.Email == team.OwnerEmail || user.IsTeamManager,
                TeamRequestAccess = TeamRequested
            };
            if(team.OwnerEmail == user.Email || user.IsTeamManager)
            {
                var usersWithTeamId = userManager.Users.Where(u => u.TeamId == model.TeamId).ToList();
                if(usersWithTeamId.Any())
                {
                    model.TeamMembers = usersWithTeamId.Where(x => x.Email != model.Email).Select(x => new TeamMembers
                    {
                        Email = x.Email,
                        Name = x.Name,
                        IsTeamOwner = x.Email == team.OwnerEmail,
                        IsTeamManager = x.IsTeamManager
                    }).ToList();
                }

                model.TeamAccess = _planner.GetRequestAccess(user.TeamId);
            }
            ModelState.Clear();
            return View(model);
        }

        public async Task<IActionResult> TeamAdmin()
        {
            List<AppUser> TeamMembers = [];
            List<TeamsVM> model = [];
            var user = await userManager.GetUserAsync(User) ?? new AppUser();
            var team = _planner.GetTeam(user.TeamId) ?? new Team();
            if (user.TeamId > 0) 
            {
                TeamMembers = [.. userManager.Users.Where(u => u.TeamId == user.TeamId)];
            }
            if (TeamMembers.Count > 0)
            {
                model = (from u in TeamMembers
                         select new TeamsVM
                         {
                             Name = u.Name!,
                             Email = u.Email!,
                             TeamId = team.Id,
                             TeamName = team.Name,
                             IsTeamManager = u.IsTeamManager,
                             IsTeamOwner = u.Email == team.OwnerEmail,
                         }).ToList();
            }
            ViewBag.IsTeamManager = user.IsTeamManager;
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
                if (user!.TeamId == 0)
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
                    user.IsTeamManager = false;
                    await userManager.UpdateAsync(user);
                    return RedirectToAction("Profile");
                }              
            }
        }
        [HttpPost]
        public async Task<IActionResult> AcceptTeamMemberRequest(int requestId, bool isManager)
        {
            var request = _planner.GetRequestAccessById(requestId);
            var user = await userManager.FindByEmailAsync(request.UserRequestorEmail!) ?? new AppUser();
            user.TeamId = request.TeamId;
            user.IsTeamManager = isManager;
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
            try
            {
                await RemoveUserFromTeam(email);
            }
            catch
            {
            }
            return RedirectToAction("Profile");
        }
        [HttpPost]
        public async Task<IActionResult> SwitchTeamOwner(string email)
        {
            bool result = true;
            try
            {
                var user = await userManager.FindByEmailAsync(email!) ?? new AppUser();
                var team = _planner.GetTeam(user.TeamId) ?? new Team();
                user.IsTeamManager = true;
                team.OwnerEmail = email;
                _planner.UpdateTeam(team);
                await userManager.UpdateAsync(user);
            }
            catch
            {
                result = false;
            }
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> ToggleManagerStatus(string email, bool isManager)
        {
            bool result = true;
            try
            {
                var user = await userManager.FindByEmailAsync(email!) ?? new AppUser();
                user.IsTeamManager = isManager;
                await userManager.UpdateAsync(user);
            }
            catch
            {
                result = false;
            }
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveTeamUser(string Email)
        {
            return Json(await RemoveUserFromTeam(Email));
        }
        private async Task<bool> RemoveUserFromTeam(string Email)
        {
            bool result = true;
            try
            {
                var user = await userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    var TeamAction = _planner.GetTeam(user.TeamId);
                    if (TeamAction != null)
                    {
                        if (TeamAction.OwnerEmail == user.Email)
                        {
                            result = false;
                        }
                        else
                        {
                            var UserTeam = _planner.GetTeamByOwner(user.Name!);
                            user.TeamId = 0;
                            if(UserTeam != null)
                                user.TeamId = UserTeam.Id;
                            await userManager.UpdateAsync(user);
                        }
                    }
                    else
                    {
                        result = false;
                    }

                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}
