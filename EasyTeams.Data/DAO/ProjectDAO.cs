using EasyTeams.Data.IDAO;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyTeams.Data.DAO
{
    public class ProjectDAO : IProjectDAO
    {
        private IManagerDAO managerDAO;
        public ProjectDAO()
        {
            managerDAO = new ManagerDAO();
        }

        // Add a project to the database
        public void AddProject(Project project, EasyTeamsContext context)
        {
            context.Projects.Add(project);
        }

        //This method is used in create task
        public void AddToCollection(PTask pTask, Project project, EasyTeamsContext context)
        {
            if (context.Projects.Contains(project))
            {
                project.Tasks.Add(pTask); //add task to the project's collection
            }
        }
        //Get projects based on project id
        public Project GetProject(int id, EasyTeamsContext context)
        { 
            context.Projects.Include(project => project.Tasks).ToList(); //include the task collection of project
            return context.Projects.Find(id);
        }
        //Get the project that specific task belongs to
        public Project GetProject(PTask pTask, EasyTeamsContext context)
        {
            var project = context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefault(project => project.Tasks.Any(pT => pT.Id == pTask.Id));
            return project;
        }
        //Get the project that belongs to a specific manager
        public List<Project> GetProjects(string managerId, EasyTeamsContext context)
        {
            var manager = context.Managers
                         .Include(m => m.Projects) 
                         .FirstOrDefault(m => m.StaffId == managerId);
            return manager.Projects.ToList();
        }

        // Get all projects from the database
        public List<Project> GetProjects(EasyTeamsContext context)
        {
            return context.Projects.Include(project => project.Tasks).ToList();
        }

        // this method is used to delete task
        public void RemoveFromCollection(PTask pTask, Project project, EasyTeamsContext context)
        {
            if(context.Projects.Contains(project))
            {
                if(project.Tasks.Contains(pTask))
                {
                    project.Tasks.Remove(pTask); //remove task from the project's collection
                }
            }
        }

        // Edit a project
        public void EditProject(Project project, EasyTeamsContext context)
        {
            Project p = context.Projects.Find(project.Id);
            context.Entry(p).CurrentValues.SetValues(project);
        }
        //Calculate the number of ongoing projects for a specific manager based on a deadline and today's date
        public int OngoingProjects(DateOnly date, string managerId, EasyTeamsContext context)
        {
            Manager manager = managerDAO.GetManager(managerId, context);
            ICollection<Project> projects = manager.Projects;
            int ongoingProjects = projects.Where(p => p.Deadline >= date).Count();
            return ongoingProjects;
        }
        //List of projects with upcoming deadlines for a specific manager (for today and the next 7 days)
        public IList<Project> UpcomingProjects(string managerId, EasyTeamsContext context)
        {
            Manager manager = managerDAO.GetManager(managerId, context);
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            IList<Project> upcomingProjects = manager.Projects.Where(p => p.Deadline >= today && p.Deadline <= today.AddDays(7)).ToList();
            IList<Project> upcoming5Projects = upcomingProjects.Take(5).ToList();
            return upcoming5Projects;
        }
    }
}
