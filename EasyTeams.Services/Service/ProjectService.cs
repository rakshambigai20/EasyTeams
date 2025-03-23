using EasyTeams.Data.DAO;
using EasyTeams.Data.IDAO;
//using EasyTeams.Data.Migrations;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using EasyTeams.Services.IService;
using EasyTeams.Services.Models;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTeams.Services.Service
{
    // Project Service
    public class ProjectService : IProjectService
    {
        private IProjectDAO projectDAO;
        private IManagerDAO managerDAO;
            public ProjectService()
        {
            projectDAO = new ProjectDAO();
            managerDAO = new ManagerDAO();
        }

        // Add Project
        public bool AddProject(Project data, string staffId)
        {
            try
            {
                #region(prepare Project Object)
                Project project = new Project()
                {
                    Id = data.Id,
                    ProjectName = data.ProjectName,
                    Deadline = data.Deadline,
                    Description = data.Description,
                };
                #endregion
                #region(Unit of work)
                using (EasyTeamsContext context = new EasyTeamsContext())
                {
                    projectDAO.AddProject(project, context);
                    Manager manager = managerDAO.GetManager(staffId, context);
                    managerDAO.AddToCollection(project, manager, context);
                    context.SaveChanges();
                }
                #endregion
                return true;
            }
            catch
            {
                return false;
            }
        }


        // Get Project
        public Project GetProject(int id)
        {
            using(EasyTeamsContext context = new EasyTeamsContext())
            {
                return projectDAO.GetProject(id, context);
            }
        }

        // Get Projects
        public List<Project> GetProjects()
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return projectDAO.GetProjects(context);
            }
        }

        // Get Projects by id
        public List<Project> GetProjects(string id)
        {
            using(EasyTeamsContext context = new EasyTeamsContext())
            {
                return projectDAO.GetProjects(id, context);
            }
        }

        // Get Project by Task
        public Project GetProject(PTask pTask)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                Project project = projectDAO.GetProject(pTask, context);
                return project;
            }
        }

        // Delete Project by id
        public bool DeleteProject(int id)
        {
            try
            {
                using (EasyTeamsContext context = new EasyTeamsContext())
                {
                    Project project = projectDAO.GetProject(id, context);
                    Manager manager = managerDAO.GetManager(project, context);
                    managerDAO.RemoveFromCollection(project, manager, context); //method to remove project from the manager's collection
                    context.Projects.Remove(project); //remove project from the database
                    context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Edit Project
        public bool EditProject(Project data, int id)
        {
            try
            {
                //prepare project object with edited data
                Project project = new Project()
                {
                    Id = id, 
                    ProjectName = data.ProjectName,
                    Deadline = data.Deadline,
                    Description = data.Description,
                };
                using (EasyTeamsContext context = new EasyTeamsContext())
                {
                    projectDAO.EditProject(project, context);
                    context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Get Ongoing Projects by Manager
        public int OngoingProjects(DateOnly date, string managerId)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return projectDAO.OngoingProjects(date, managerId, context);
            }
        }

        // Get Upcoming Projects by Manager
        public IList<Project> UpcomingProjects(string managerId)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return projectDAO.UpcomingProjects(managerId, context);
            }
        }
    }
}
