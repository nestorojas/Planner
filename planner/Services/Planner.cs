using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using planner.Data;
using planner.Migrations;
using System.Threading.Tasks;

namespace planner.Services
{
    public class Planner(ApplicationDbContext context)
    {
        private readonly ApplicationDbContext _context = context;

        #region Workflow
        internal Workflow AddWorkflow(Workflow workflow)
        {
            _context.Workflows.Add(workflow);
            _context.SaveChanges();
            return workflow;
        }
        internal List<Workflow> GetWorkflows()
        {
            return [.. _context.Workflows];
        }
        internal Workflow GetWorkflowById(int id)
        {
            return _context.Workflows.Find(id)!;
        }
        internal void UpdateWorkflow(Workflow workflow)
        {
            _context.Entry(workflow).State = EntityState.Modified;
            _context.SaveChanges();
        }
        internal void DeleteWorkflow(int id)
        {
            var workflow = _context.Workflows.Find(id);
            if (workflow != null)
            {
                _context.Workflows.Remove(workflow);
                _context.SaveChanges();
            }
        }
        internal void ExecuteWorkFlow(int id)
        {
            var project = GetProjectById(id);
            var workflowTasks = _context.Tasks
                .Where(wt => wt.WorkflowId == project.WorkflowId)
                .OrderBy(wt => wt.Id)
                .ToList();

            var workflowTaskWOPredecessor = workflowTasks.Where(x => x.PredecessorId == null || x.PredecessorId.Count == 0).ToList();
            var workflowTaskWithPredecessor = workflowTasks.Except(workflowTaskWOPredecessor).ToList();

            foreach (var workflowTask in workflowTaskWOPredecessor)
            {
                var newTask = new Task
                {
                    ProjectId = project.Id,
                    Name = workflowTask.Name,
                    Description = workflowTask.Description,
                    CreatedDate = DateTime.Today,
                    DueDate = DateTime.Today.AddDays(7),
                    CreatedByUserId = project.OwnerId,
                    AssignedUserId = project.OwnerId,
                    IsWorkflow = false,
                    IdTemplate = workflowTask.Id
                };
                _context.Tasks.Add(newTask);
            }
            _context.SaveChanges();

            foreach (var workflowTask in workflowTaskWithPredecessor)
            {
                var predecessorTask = _context.Tasks.Where(x => x.ProjectId == id && workflowTask.PredecessorId!.Contains(x.IdTemplate ?? 0)).Select(x => x.Id).ToList();
                var newTask = new Task
                {
                    ProjectId = project.Id,
                    Name = workflowTask.Name,
                    Description = workflowTask.Description,
                    CreatedDate = new DateTime(1900, 1, 1),
                    DueDate = new DateTime(1900, 1, 1),
                    CreatedByUserId = project.OwnerId,
                    AssignedUserId = project.OwnerId,
                    IsWorkflow = false,
                    IdTemplate = workflowTask.Id,
                    PredecessorId = predecessorTask
                };
                _context.Tasks.Add(newTask);
                _context.SaveChanges();
            }
        }
        internal void ExecuteTaskWorkFlow(int? pjId, int tskId)
        {
            var tasks = GetTaskByProjectId(pjId ?? 0);
            var tasksWithPredecessors = tasks.Where(x => x.PredecessorId != null && x.PredecessorId.Count > 0 && x.PredecessorId!.Contains(tskId)).ToList();
            var startDate = DateTime.Today;
            foreach (var taskWith in tasksWithPredecessors)
            {
                bool IsReady = true;
                foreach (var PredId in taskWith.PredecessorId!)
                {
                    var taskPredecessor = GetTaskById(PredId);
                    if (!taskPredecessor.IsCompleted)
                    {
                        IsReady = false; break;
                    }
                }
                if (IsReady)
                {
                    taskWith.CreatedDate = DateTime.UtcNow;
                    taskWith.DueDate = DateTime.UtcNow.AddDays(7);
                    _context.Entry(taskWith).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
        }
        #endregion

        #region Project
        internal List<Project> GetProjects(string? email, int? teamId)
        {
            var projectTeams = _context.Projects.Where(x => x.TeamId == teamId).ToList();
            var projectUser = _context.Projects.Where(x => x.OwnerId == email).ToList();
            return projectTeams.Union(projectUser).Distinct().ToList();
        }
        internal Project GetProjectById(int id)
        {
            return _context.Projects.Where(x => x.Id == id).FirstOrDefault()!;
        }
        internal List<ProjectStatus> GetProjectStatus()
        {
            return [.. _context.ProjectStatuses];
        }
        internal ProjectStatus GetProjectStatusById(int statusId)
        {
            return _context.ProjectStatuses.Where(x => x.ProjectStatusId == statusId).FirstOrDefault()!;
        }
        internal bool AddProject(Project project)
        {
            try
            {
                _context.Projects.Add(project);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        internal bool UpdateProject(Project project)
        {
            try
            {
                _context.Entry(project).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Task
        internal List<Task> GetTaskByUser(string username)
        {
            return [.. _context.Tasks.Where(x => x.AssignedUserId == username)];
        }
        internal void AddTask(Task task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }
        internal List<Task> GetTasks()
        {
            return [.. _context.Tasks];
        }
        internal Task GetTaskById(int id)
        {
            return _context.Tasks.FirstOrDefault(t => t.Id == id)!;
        }
        internal List<Task> GetTaskByProjectId(int id)
        {
            return [.. _context.Tasks.Where(t => t.ProjectId == id)!];
        }
        internal Task GetTaskPredecessor(int id)
        {
            return _context.Tasks.FirstOrDefault(t => t.IdTemplate == id)!;
        }
        internal List<Task> GetTaskByWorkflowId(int workflowId)
        {
            return [.. _context.Tasks.Where(t => t.WorkflowId == workflowId)];
        }
        internal void UpdateTask(Task task)
        {
            _context.Entry(task).State = EntityState.Modified;
            _context.SaveChanges();
            if (task.IsCompleted)
            {
                ExecuteTaskWorkFlow(task.ProjectId, task.Id);
            }
            int ProjectId = task.ProjectId ?? 0;
            if (ProjectId > 0)
            {
                var tasks = GetTaskByProjectId(ProjectId).Where(x => x.IsCompleted).ToList();
                if (tasks != null && tasks.Count == 0)
                {
                    Project project = GetProjectById(ProjectId);
                    project.ProjectStatusId = 4;
                    _context.Entry(project).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
        }
        internal void DeleteTask(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
        }
        internal void AddNote(Note note)
        {
            try
            {
                _context.Notes.Add(note);
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region User Profile
        internal Team GetTeam(int? id)
        {
            return _context.Teams.Where(x => x.Id == id).FirstOrDefault()!;
        }
        internal void AddTeam(Team team)
        {
            _context.Teams.Add(team);
            _context.SaveChanges();
        }
        internal void UpdateTeam(Team team)
        {
            _context.Entry(team).State = EntityState.Modified;
            _context.SaveChanges();
        }
        internal void RequestTeamAccess(RequestTeamAccess requestor)
        {
            _context.RequestTeamAccesses.Add(requestor);
            _context.SaveChanges();
        }
        internal List<RequestTeamAccess> GetRequestAccess(int? teamId)
        {
            return [.. _context.RequestTeamAccesses.Where(x => x.TeamId == teamId)];
        }
        internal RequestTeamAccess GetRequestAccessById(int requestId)
        {
            return _context.RequestTeamAccesses.Where(x => x.Id == requestId).FirstOrDefault()!;
        }
        internal string GetRequestAccessByEmail(string email)
        {
            var teamId = _context.RequestTeamAccesses.Where(x => x.UserRequestorEmail == email).Select(x => x.TeamId).FirstOrDefault()!;
            return _context.Teams.Where(x => x.Id == teamId).Select(x => x.Name).FirstOrDefault()!;
        }
        internal void UpdateRequestTeamAccess(RequestTeamAccess request)
        {
            _context.RequestTeamAccesses.Remove(request);
            _context.SaveChanges();
        }
        #endregion

        #region Client
        internal List<Client> GetClient(string? ClientName, int? ClientId)
        {
            if (!string.IsNullOrEmpty(ClientName))
            {
                return [.. _context.Clients.Where(x => x.ClientName!.ToLower().Contains(ClientName.ToLower()))];
            }
            if (ClientId != null && ClientId > 0)
            {
                return [.. _context.Clients.Where(x => x.ClientId! == ClientId)];
            }
            else
            {
                return [.. _context.Clients];
            }
        }
        internal List<Client> GetClients()
        {
            return [.. _context.Clients];
        }
        #endregion
    }
}
