using EasyTeams.Data.IDAO;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTeams.Data.DAO
{
    // PTask Data Access Object
    public class PTaskDAO : IPTaskDAO
    {
        IStaffDAO staffDAO = new StaffDAO(); 

        // Add a task to the database
        public void CreatePTask(PTask pTask, EasyTeamsContext context)
        {
            context.Tasks.Add(pTask);
        }

        // Delete a task from the database
        public void DeletePTask(PTask pTask, EasyTeamsContext context)
        {
            context.Tasks.Remove(pTask);
        }

        // Edit a task in the database
        public void EditPTask(PTask task, EasyTeamsContext context)
        {
            PTask t = context.Tasks.Find(task.Id);
            context.Entry(t).CurrentValues.SetValues(task);
        }

        // Get a specific task from the database
        public PTask GetPTask(int id, EasyTeamsContext context)
        {
           return context.Tasks.Find(id);
        }

        // Get all tasks from the database
        public List<PTask> GetPTasks(EasyTeamsContext context)
        {
            return context.Tasks.ToList();
        }
        //Get all tasks for a specific staff member
        public List<PTask> GetTasks(string staffId, EasyTeamsContext context)
        {
            var tasks = context.Staffs
                .Where(s => s.StaffId == staffId)
                .Include(s => s.Tasks)
                .SelectMany(s => s.Tasks)
                .ToList();
            return tasks;
        }
        //Mark a task as finished
        public bool MarkPTaskFinished(int id, EasyTeamsContext context)
        {
            var pTask = context.Tasks.Find(id);
            if (pTask != null)
            {
                pTask.IsFinished = true;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        // Reassign a task to a different staff member
        public void ReassignPTask(PTask pTask, EasyTeamsContext context)
        {
            context.Tasks.Add(pTask);//edited records are created as a new task
        }

        //Method to get tasks collection of a staff
        public List<PTask> GetTask(string StaffId, EasyTeamsContext context)
        {
            List<PTask> tasks = new List<PTask>();
            Staff staff = staffDAO.GetStaff(StaffId,context);
            tasks = staff.Tasks.ToList();
            return tasks;
    
        }
        //Method to submit report to a task. 
        public bool submitReport(int taskId, string staffId, string report, EasyTeamsContext context)
        {
            Staff staff = staffDAO.GetStaff(staffId, context);

            if (staff == null)
            {
                return false; 
            }
            PTask pTask = GetPTask(taskId, context);

            if (pTask != null && staff.Tasks.Any(t => t.Id == taskId))
            {
                pTask.Report = report; //stores report in the database
                if (pTask.Report != null) //checks for content in report
                {
                    pTask.ReportSubmitted = true;
                }
                else
                {
                    pTask.ReportSubmitted = false;
                }
                context.SaveChanges();
                return true;
            }
            return false;
        }
        //List of not finished tasks for a specific staff member
        public List<PTask> GetPendingTasks(string id, EasyTeamsContext context)
        {
            //Staff staff = staffDAO.GetStaff(id, context);
            List<PTask> allTasks = GetTasks(id, context);
            List<PTask> pendingTasks = new List<PTask>();
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            
            foreach (PTask task in allTasks)
            {
                if (task.ReportSubmitted == false && task.IsFinished == false && task.Date < currentDate)
                pendingTasks.Add(task);
            }
           
            return pendingTasks;
        }
        //List of first 5 pending tasks 
        public List<PTask> Get5PendingTasks(string id, EasyTeamsContext context)
        {
            List<PTask> pendingTasks = GetPendingTasks(id, context);
            pendingTasks = pendingTasks.OrderBy(t => t.Date).ToList();
            List<PTask> first5pendingTasks = pendingTasks.Take(6).ToList();
            return first5pendingTasks;
        }
    }
}
