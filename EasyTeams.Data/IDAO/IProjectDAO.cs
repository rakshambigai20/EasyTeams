using EasyTeams.Data.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Data.Models.Repository;

namespace EasyTeams.Data.IDAO
{
    // Project Data Access Object Interface
    public interface IProjectDAO
    {
        public Project GetProject(int id, EasyTeamsContext context);
        public List<Project> GetProjects(string managerId, EasyTeamsContext context);
        public List<Project> GetProjects(EasyTeamsContext context);
        void AddProject(Project project, EasyTeamsContext context);
        void AddToCollection(PTask pTask, Project project, EasyTeamsContext context);
        Project GetProject(PTask pTask, EasyTeamsContext context);
        void RemoveFromCollection(PTask pTask, Project project, EasyTeamsContext context);
        void EditProject(Project project, EasyTeamsContext context);
        int OngoingProjects(DateOnly date, string managerId, EasyTeamsContext context);
        IList<Project> UpcomingProjects(string managerId, EasyTeamsContext context);
    }
}
