using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using planner.Services;

namespace planner.Controllers
{
    [Authorize]
    public class WorkflowController(UserManager<AppUser> userManager, Planner planner) : Controller
    {
        private readonly Planner _planner = planner;

        public IActionResult Index()
        {
            var userId = User.Claims.FirstOrDefault()!.Value;
            List<Workflow> workflows = _planner.GetWorkflows();
            return View(workflows.Where(x => x.OwnerId == userId));
        }
        public IActionResult Details(int id)
        {
            Workflow workflow = _planner.GetWorkflowById(id);
            if (workflow == null)
            {
                return RedirectToAction("Index");
            }

            return View("Create",workflow);
        }
        public IActionResult Create()
        {
            return View(new Workflow());
        }
        [HttpPost]
        public IActionResult Create(int Id, string Name, string Description)
        {
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Description))
            {
                var workflow = new Workflow { Id = Id, Name = Name, Description = Description};
                if (workflow.Id == 0)
                {
                    var createdWorkflow = _planner.AddWorkflow(workflow);
                    return Json(new { id = createdWorkflow.Id });
                }
                else
                {
                    _planner.UpdateWorkflow(workflow);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(new Workflow());
        }
        public IActionResult Delete(int id)
        {
            Workflow workflow = _planner.GetWorkflowById(id);
            if (workflow == null)
            {
                return NotFound();
            }

            return View(workflow);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _planner.DeleteWorkflow(id);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult GetTasks(int id)
        {
            var tasks = _planner.GetTaskByWorkflowId(id) ?? [];
            return Json(tasks);
        }
        [HttpPost]
        public async Task<IActionResult> CreateTaskForWorkflow(string Name, string predecessors, int workflowId)
        {
            if(!string.IsNullOrEmpty(Name))
            {
                var prdId = string.IsNullOrEmpty(predecessors) ? [] : predecessors.Split(',').Select(s => int.Parse(s.Trim())).ToList();
                var currentUser = await userManager.GetUserAsync(User);
                var newTask = new Task
                {
                    Name = Name,
                    Description = Name,
                    PredecessorId = prdId,
                    IsWorkflow = true,
                    WorkflowId = workflowId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedByUserId = currentUser!.Id,
                };
                try
                {
                    _planner.AddTask(newTask);
                    return Json(true);
                }
                catch
                {
                    return Json(false);
                }
            }
            return Json(false);
        }
        private bool WorkflowExists(int id)
        {
            return _planner.GetWorkflowById(id) != null;
        }
    }
}
