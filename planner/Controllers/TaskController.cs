using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using planner.Services;
using planner.ViewModel;
using System.Security.Claims;
using System.Threading.Tasks;

namespace planner.Controllers
{
    [Authorize]
    public class TaskController(UserManager<AppUser> userManager, Planner planner) : Controller
    {
        private readonly Planner _planner = planner;
        public async Task<IActionResult> Task()
        {
            var task = await GetTasksForCurrentUserAsync(User);
            return View(task);
        }
        public async Task<List<Task>> GetTasksForCurrentUserAsync(ClaimsPrincipal user)
        {
            var currentUser = await userManager.GetUserAsync(user);
            if (!string.IsNullOrEmpty(currentUser!.UserName))
            {
                return _planner.GetTaskByUser(currentUser!.UserName!);
            }
            return [];
        }
        public async Task<IActionResult> TaskCreate(int id)
        {
            var currentUser = await userManager.GetUserAsync(User);
            TaskVM viewModel = new()
            {
                ProjectId = id,
                CreatedByUserId = currentUser!.UserName!,
                AssignedUserId = currentUser.UserName!,
                Duration = 0,
                CreatedDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),
            };
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult TaskCreate(TaskVM model)
        {
            var duration = model.Duration == 0 ? (model.DueDate!.Value - DateTime.Today).Days : model.Duration;
            Task task = new()
            {
                Duration = duration,
                DueDate = model.DueDate == null ? DateTime.UtcNow.AddDays(duration ?? 0) : model.DueDate
            };

            if (model.PredecessorId == null || model.PredecessorId!.Count == 0)
            {
                model.PredecessorId = [];
                task.CreatedDate = DateTime.UtcNow;
            }
            else
            {
                task.PredecessorId = model.PredecessorId;
                task.DueDate = model.DueDate;
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "Task Name is required");
            }
            if (string.IsNullOrEmpty(model.Description))
            {
                ModelState.AddModelError("Description", "Task Description is required");
            }
            if (string.IsNullOrEmpty(model.AssignedUserId))
            {
                ModelState.AddModelError("AssignedUserId", "Task Assigned User is required");
            }
            ModelState.Remove("PredecessorId");
            if (ModelState.IsValid)
            {
                task.ProjectId = model.ProjectId;
                task.CreatedByUserId = model.CreatedByUserId;
                task.Name = model.Name;
                task.Description = model.Description;
                task.AssignedUserId = model.AssignedUserId;
                _planner.AddTask(task);
                return RedirectToAction("ProjectDetail", "Project",new { id = task.ProjectId });
            }
            return View(model);
        }
        public IActionResult TaskUpdate(int id)
        {
            var p = _planner.GetTaskById(id);
            var Assigned = userManager.Users.Where(u => u.UserName == p.AssignedUserId).FirstOrDefault();
            TaskVM viewModel = new()
            {
                TaskId = p.Id,
                ProjectId = p.ProjectId,
                ProjectName = _planner.GetProjectById(p.ProjectId ?? 0).Name,
                Name = p.Name,
                Description = p.Description,
                CreatedDate = p.CreatedDate,
                DueDate = p.DueDate,
                IsCompleted = p.IsCompleted,
                PredecessorId = p.PredecessorId,
                AssignedUserId = p.AssignedUserId,
                AssignedUserName = Assigned!.Name!
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> TaskUpdate(TaskVM model)
        {
            if (string.IsNullOrEmpty(model.Note))
            {
                ModelState.AddModelError("Note", "A Note is required");
                return View(model);
            }
            else
            {
                Task task = _planner.GetTaskById(model.TaskId);
                var pid = task.ProjectId ?? 0;
                var currentUser = await userManager.GetUserAsync(User);
                if (model.Mode == "Update")
                {
                    if(task.DueDate != model.DueDate)
                    {
                        task.DueDate = model.DueDate;
                    }
                    _planner.UpdateTask(task);
                }
                else
                {
                    task.IsCompleted = true;
                    _planner.UpdateTask(task);
                }
                Note note = new()
                {
                    TaskId = task.Id,
                    ProjectId = pid,
                    Comment = model.Note!,
                    CreatedBy = currentUser!.Name,
                    CreatedAt = DateTime.UtcNow,
                };
                _planner.AddNote(note);
                return RedirectToAction("ProjectDetail", "Project", new { id = model.ProjectId });
            }
        }
        public IActionResult GetTasks(int id)
        {
            var tasks = _planner.GetTaskByProjectId(id) ?? [];
            return Json(tasks);
        }
        public async Task<IActionResult> GetMemberTeam()
        {
            var currentUser = await userManager.GetUserAsync(User);
            var usersWithTeamId = userManager.Users.Where(u => u.TeamId == currentUser!.TeamId).ToList();

            return Json(usersWithTeamId.Select(x => new { Id = x.UserName, x.Name }));
        }
    }
}
