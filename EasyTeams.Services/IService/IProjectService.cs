using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using EasyTeams.Services.Models;

namespace EasyTeams.Services.IService
{

    // Project Service Interface
    public interface IProjectService
    {
        public Project GetProject(int id);
        public List<Project> GetProjects(string id);
        public List<Project> GetProjects();
        bool AddProject(Project project, string staffId);
        Project GetProject(PTask pTask);
        bool DeleteProject(int id);
        bool EditProject(Project collection, int id);
        int OngoingProjects(DateOnly date, string managerId);
        IList<Project> UpcomingProjects(string managerId);
    }
}
