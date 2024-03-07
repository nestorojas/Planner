using Microsoft.EntityFrameworkCore;
using planner.Data;
using planner.Migrations;
using System.Threading.Tasks;

namespace planner.Services
{
    public class Planner(ApplicationDbContext context)
    {
        private readonly ApplicationDbContext _context = context;

        public Workflow AddWorkflow(Workflow workflow)
        {
            _context.Workflows.Add(workflow);
            _context.SaveChanges();
            return workflow;
        }

        public List<Workflow> GetWorkflows()
        {
            return [.. _context.Workflows];
        }

        public Workflow GetWorkflowById(int id)
        {
            return _context.Workflows.Find(id)!;
        }

        public void UpdateWorkflow(Workflow workflow)
        {
            _context.Entry(workflow).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteWorkflow(int id)
        {
            var workflow = _context.Workflows.Find(id);
            if (workflow != null)
            {
                _context.Workflows.Remove(workflow);
                _context.SaveChanges();
            }
        }

        public void AddTask(Task task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        public List<Task> GetTasks()
        {
            return _context.Tasks.ToList();
        }

        public Task GetTaskById(int id)
        {
            return _context.Tasks.FirstOrDefault(t => t.Id == id)!;
        }

        public Task GatTaskPredecessor(int id)
        {
            return _context.Tasks.FirstOrDefault(t => t.IdTemplate == id)!;
        }

        public List<Task> GetTaskByWorkflowId(int workflowId)
        {
            return [.. _context.Tasks.Where(t => t.WorkflowId == workflowId)];
        }

        public void UpdateTask(Task task)
        {
            _context.Entry(task).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteTask(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
        }

        public Team GetTeam(int? id)
        {
            return _context.Teams.Where(x=> x.Id == id).FirstOrDefault()!;
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

        internal List<Client> GetClient(string? ClientName, int? ClientId)
        {
            if(!string.IsNullOrEmpty(ClientName))
            {
                return [.. _context.Clients.Where(x => x.ClientName!.ToLower().Contains(ClientName.ToLower()))];
            }
            if(ClientId != null && ClientId > 0) 
            {
                return [.. _context.Clients.Where(x => x.ClientId! == ClientId)];
            }
            else
            {
                return [.._context.Clients];
            }
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

        public void ExecuteWorkFlow(int id)
        {
            var project = GetProjectById(id);
            var workflowTasks = _context.Tasks
                .Where(wt => wt.WorkflowId == project.WorkflowId && !wt.IsCompleted)
                .OrderBy(wt => wt.Id)
                .ToList();

            foreach (var workflowTask in workflowTasks)
            {
                if ((workflowTask.PredecessorId == null || workflowTask.PredecessorId.Count == 0))
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
                else if(!(workflowTask.PredecessorId != null && workflowTask.PredecessorId.Count > 0))
                {
                    var isPredecessorCompleted = true;
                    Task Predecessor = new();
                    var startDate = DateTime.Today;
                    foreach (int predecessor in workflowTask.PredecessorId!)
                    {
                        Predecessor = GatTaskPredecessor(predecessor);
                        isPredecessorCompleted = Predecessor.IsCompleted;
                        startDate = (DateTime)((Predecessor.CompletedDate >= startDate) ? Predecessor.CompletedDate : startDate);
                    }
                    if(isPredecessorCompleted)
                    {
                        var newTask = new Task
                        {
                            ProjectId = project.Id,
                            Name = workflowTask.Name,
                            Description = workflowTask.Description,
                            CreatedDate = startDate,
                            DueDate = startDate.AddDays(7),
                            CreatedByUserId = project.OwnerId,
                            AssignedUserId = project.OwnerId,
                            IsWorkflow = false,
                            IdTemplate = workflowTask.Id
                        };
                        _context.Tasks.Add(newTask);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
                _context.SaveChanges();
            }
        }
    }
}
