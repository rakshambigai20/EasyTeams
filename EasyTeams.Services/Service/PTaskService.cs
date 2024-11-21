using EasyTeams.Data.DAO;
using EasyTeams.Data.IDAO;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using EasyTeams.Services.IService;
using EasyTeams.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EasyTeams.Services.Service
{
    // PTask Service
    public class PTaskService : IPTaskService
    {
        private IPTaskDAO pTaskDAO;
        private IProjectDAO projectDAO;
        private IStaffDAO staffDAO;
        private IManagerDAO managerDAO;

        // Constructor
        public PTaskService()
        {
            pTaskDAO = new PTaskDAO();
            projectDAO = new ProjectDAO();
            staffDAO = new StaffDAO();
            managerDAO = new ManagerDAO();
        }

        // Create PTask
        public bool CreatePTask(PTaskProjectStaff data, string staffId)
        {
            try
            {
                #region(prepare PTask Object)
                PTask pTask = new PTask()
                {
                    Id = data.Id,
                    Date = data.Date,
                    Description = data.Description,
                    Hours = data.Hours,
                };
                #endregion
                #region(Unit of work)
                using (EasyTeamsContext context = new EasyTeamsContext())
                {
                    pTaskDAO.CreatePTask(pTask, context); //calls method to add task to the database
                    Project project = projectDAO.GetProject(data.ProjectId, context);
                    projectDAO.AddToCollection(pTask, project, context); //calls method to add task to the project's collection
                    Staff staff = staffDAO.GetStaff(data.StaffId, context);
                    staffDAO.AddToCollection(pTask, staff, context); //calls method to add task to the staff's collection
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

        // Delete PTask
        public bool DeletePTask(int id)
        {
            try
            {
                using(EasyTeamsContext context = new EasyTeamsContext())
                {
                    PTask pTask = pTaskDAO.GetPTask(id, context);
                    Project project = projectDAO.GetProject(pTask, context);
                    projectDAO.RemoveFromCollection(pTask, project, context); //calls method to remove task from project's collection
                    Staff staff = staffDAO.GetStaff(pTask, context);
                    staffDAO.RemoveFromCollection(pTask, staff, context); //calls method to remove task from staff's collection
                    context.Tasks.Remove(pTask); ////calls method to remove task from database
                    context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Get PTask by Id
        public PTask GetPTask(int id)
        {
            
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return pTaskDAO.GetPTask(id, context);
            }
        }

        // Get PTasks List
        public List<PTask> GetPTasks()
        {

            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return pTaskDAO.GetPTasks(context);
            }
        }

        // Get PTasks List by StaffId
        public List<PTask> GetTasks(string id)
        {
            using(EasyTeamsContext context = new EasyTeamsContext())
            {
                return pTaskDAO.GetTasks(id, context);
            }
        }

        // Get PTasks List by Project Id. This method is used to view tasks under a project
        public List<PTaskProjectStaff> GetPTasksList(int ProjectId)
        {
            // Retrieve combined data from the tasks and staff models using the service layer method.
            var tasksWithStaff = new List<PTaskProjectStaff>();

            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                Project project = projectDAO.GetProject(ProjectId, context);
                var tasks = project.Tasks.ToList();

                    // Get Staff for each task  
                    foreach (var task in tasks) 
                    {
                        var staff = staffDAO.GetStaff(task, context); 
                        if (staff != null) 
                        {
                            tasksWithStaff.Add(new PTaskProjectStaff
                            {
                                Id = task.Id,
                                Description = task.Description,
                                Hours = task.Hours,
                                Date = task.Date,
                                Report = task.Report,
                                IsFinished = task.IsFinished,
                                StaffName = staff.FirstName + " " + staff.LastName 
                            });
                        }
                    }
            }

            return tasksWithStaff; // Return List
        }

        // Edit PTask
        public bool EditPTask(PTask data, int id)
        {
            try
            {
                //prepare pTask object with edited fields 
                PTask task = new PTask()
                {
                    Id = id,
                    Description = data.Description,
                    Date = data.Date,
                    Hours = data.Hours,
                };
                using (EasyTeamsContext context = new EasyTeamsContext())
                {
                    pTaskDAO.EditPTask(task, context);
                    context.SaveChanges();
                }
                return true; // Success
            }
            catch
            {
                return false; // Database error
            }
        }

        // Mark PTask as Finished
        public bool MarkPTaskFinished(int id)
        {
            try
            {
                using (EasyTeamsContext context = new EasyTeamsContext())
                {
                    return pTaskDAO.MarkPTaskFinished(id, context);
                }
            }
            catch
            {
                return false; // Database error
            }
        }

        // Reassign PTask to another Staff
        public bool ReassignPTask(PTaskProjectStaff collection, string staffId)
        {
            try
            {
                #region(prepare PTask Object)
                // Since reassigned tasks are created as new tasks with new IDs, we omit setting the ID here.
                PTask pTask = new PTask()
                {
                    Date = collection.Date,
                    Description = collection.Description,
                    Hours = collection.Hours,
                };
                #endregion
                #region(Unit of work)
                using (EasyTeamsContext context = new EasyTeamsContext())
                {
                    pTaskDAO.CreatePTask(pTask, context); //calls method to create a new task
                    Project project = projectDAO.GetProject(collection.ProjectId, context);
                    projectDAO.AddToCollection(pTask, project, context); //calls method to add task to the project's collection
                    Staff staff = staffDAO.GetStaff(staffId, context);
                    staffDAO.AddToCollection(pTask, staff, context);  //calls method to add task to the staff's collection
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

        // Submit Report for PTask
        public bool submitReport(int taskId, string staffId, string report)
        {
            try
            {
                using(EasyTeamsContext context = new EasyTeamsContext())
                {
                    return pTaskDAO.submitReport(taskId, staffId, report, context);
                }
            }
            catch
            {
                return false;
            }
        }

        
        // Get Pending Tasks for Staff 
        public List<PTask> GetPendingTasks(string id)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return pTaskDAO.GetPendingTasks(id, context);
            }
        }

        //Method to get first 5 pending tasks from staff's collection
        public List<PTask> Get5PendingTasks(string id)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return pTaskDAO.Get5PendingTasks(id, context);
            }
        }
    }
}

