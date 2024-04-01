using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planner.Migrations;
using planner.Services;
using planner.ViewModel;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Diagrams;
using System.Security.Claims;

namespace planner.Controllers
{
    [Authorize]
    public class ProjectController(UserManager<AppUser> userManager, Planner planner) : Controller
    {
        private readonly Planner _planner = planner;
        public async Task<IActionResult> Project()
        {
            var project = await GetProjectsForCurrentUserAsync(User);
            var projectStatus = _planner.GetProjectStatus();
            var model = new List<ProjectVM>();
            if (project != null)
            {
                model = (from p in project select new ProjectVM
                {
                    ProjectId = p.Id,
                    ProjectName = p.Name,
                    ProjectDescription = p.Description,
                    ClientId = p.ClientId,
                    ClientName = p.ClientName,
                    ProjectStatusId = p.ProjectStatusId,
                    ProjectStatus = _planner.GetProjectStatusById(p.ProjectStatusId).ProjectStatusName,
                    WorkflowId = p.WorkflowId,
                    WorkflowName = _planner.GetWorkflowById(p.WorkflowId).Name
                }).ToList();
            }
            return View(model);
        }
        public async Task<List<Project>> GetProjectsForCurrentUserAsync(ClaimsPrincipal user)
        {
            var currentUser = await userManager.GetUserAsync(user);
            return _planner.GetProjects(currentUser!.Email, currentUser.TeamId);
        }
        public IActionResult ProjectEdit(int id)
        {
            var p = _planner.GetProjectById(id);
            var model = new ProjectVM
            {
                ProjectId = p.Id,
                ProjectName = p.Name,
                ProjectDescription = p.Description,
                ClientId = p.ClientId,
                ClientName = p.ClientName,
                ProjectStatusId = p.ProjectStatusId,
                ProjectStatus = _planner.GetProjectStatusById(p.ProjectStatusId).ProjectStatusName,
                WorkflowId = p.WorkflowId,
                WorkflowName = _planner.GetWorkflowById(p.WorkflowId).Name
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult ProjectEdit(ProjectVM model)
        {
            if (ModelState.IsValid)
            {
                Project project = _planner.GetProjectById(model.ProjectId);
                project.Name = model.ProjectName!;
                project.Description = model.ProjectDescription!;
                project.ProjectStatusId = model.IsStartProject ? 3 : 2;

                var result = _planner.UpdateProject(project);
                if (result)
                {
                    if (project.ProjectStatusId >= 3 && project.ProjectStatusId != 5)
                    {
                        _planner.ExecuteWorkFlow(project.Id);
                    }
                    return RedirectToAction("Project");
                }

                ModelState.AddModelError("ErrorModel", "System is busy, Project cannot be created, try again!");
                return View(model);
            }
            return View(model);
        }
        public IActionResult ProjectDetail(int id)
        {
            var p = _planner.GetProjectById(id);
            var model = new ProjectVM
            {
                ProjectId = p.Id,
                ProjectName = p.Name,
                ProjectDescription = p.Description,
                ClientId = p.ClientId,
                ClientName = p.ClientName,
                ProjectStatusId = p.ProjectStatusId,
                ProjectStatus = _planner.GetProjectStatusById(p.ProjectStatusId).ProjectStatusName,
                WorkflowId = p.WorkflowId,
                WorkflowName = _planner.GetWorkflowById(p.WorkflowId).Name,
                Tasks = _planner.GetTaskByProjectId(p.Id),
            };
            return View(model);
        }
        public async Task<IActionResult> ProjectCreate()
        {
            var model = new ProjectVM
            {
                Workflows = _planner.GetWorkflows(),
                TeamId = await GetTeamByUser(User)
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ProjectCreate(ProjectVM model)
        {
            if (ModelState.IsValid)
            {
                var client = _planner.GetClient(null,model.ClientId).FirstOrDefault();
                var currentUser = await userManager.GetUserAsync(User);
                Project project = new()
                {
                    ClientId = client!.ClientId,
                    ClientName = client.ClientName!,
                    CreatedOn = DateTime.UtcNow,
                    Description = model.ProjectDescription!,
                    Initiator = currentUser!.Name!,
                    Name = model.ProjectName!,
                    OwnerId = currentUser!.Email!,
                    ProjectStatusId = model.IsStartProject ? 3 : 2,
                    WorkflowId = model.WorkflowId,
                    TeamId = model.TeamId ?? 0
                };
                var result = _planner.AddProject(project);
                if (result)
                {
                    if(project.ProjectStatusId >= 3 && project.ProjectStatusId != 5)
                    {
                        _planner.ExecuteWorkFlow(project.Id);
                    }
                    return RedirectToAction("Project");
                }

                ModelState.AddModelError("ErrorModel", "System is busy, Project cannot be created, try again!");
                return View(model);
            }
            return View(model);
        }
        private async Task<int?> GetTeamByUser(ClaimsPrincipal user)
        {
            var currentUser = await userManager.GetUserAsync(user);
            return currentUser!.TeamId;
        }

        [HttpGet]
        public IActionResult GetClientNames(string term)
        {
            var clientNames = _planner.GetClient(term, 0);

            return Json(clientNames.Select(x => new { x.ClientId, x.ClientName }));
        }
    }
}
