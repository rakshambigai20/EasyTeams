using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Services.IService;
using EasyTeams.Data.IDAO;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using EasyTeams.Data.DAO;
using EasyTeams.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyTeams.Services.Service
{
    // Staff Service
    public class StaffService: IStaffService
    {
        IStaffDAO staffDAO;
        IManagerDAO managerDAO;
        IPTaskDAO pTaskDAO;
        IProjectDAO projectDAO;

        // Constructor
        public StaffService()
        {
            staffDAO = new StaffDAO();
            managerDAO = new ManagerDAO();
            pTaskDAO = new PTaskDAO();
            projectDAO = new ProjectDAO();
            
        }

        // Get Staff List
        public IList<Staff> GetStaffs()
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return staffDAO.GetStaffs(context);
            }
        }

        // Get Staff details
        public Staff GetStaff(string id)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return staffDAO.GetStaff(id, context);
            }
        }

        // Edit Staff
        public bool EditStaff(Staff data, string id)
        {
            try
            {
                //prepare staff object with edited fields
                Staff staff = new Staff()
                {
                    StaffId = id,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    WorkEmail = data.WorkEmail,
                    PhoneNumber = data.PhoneNumber,
                    Address = data.Address,
                };
                using(EasyTeamsContext context = new EasyTeamsContext())
                {
                    staffDAO.EditStaff(staff, context);
                    context.SaveChanges();
                }
                return true; // return true if successful
            }
            catch
            {
                return false; // return false if unsuccessful
            }
        }

        
        //public Staff GetStaff(Leave leave, EasyTeamsContext context)
        //{
        //        return staffDAO.GetStaff(leave, context);
        //}

       //Method to get staff by leave 
        public Staff GetStaff(Leave leave)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return staffDAO.GetStaff(leave, context);
            }
        }

        // Method to add a staff in to manager's collection
        public void AddStaff(Staff staff, string managerId)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                staffDAO.AddStaff(staff, context);
                managerDAO.AddToCollection(staff, managerId, context);
                context.SaveChanges();
            }
        }

        // Get Staff by task
        public Staff GetStaff(PTask pTask)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                Staff staff = staffDAO.GetStaff(pTask, context);
                return staff;
            }
        }

        // Method to generate timesheet for staff based on tasks assigned for a week
        //Logic to display tasks for current week is handled in the Timesheet/ TasksWithProjects view
        public Timesheet GenerateTimesheet(string staffId)
        {
            Timesheet timesheetData = new Timesheet(); //Timesheet class is created in the service layer

            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                Staff staff = GetStaff(staffId);
                if (staff != null && staff.Tasks.Any())
                {
                    timesheetData.staff = staff;

                    foreach (var task in staff.Tasks)
                    {
                        //get project information for each task to display the project name as a column in the timesheet
                        var project = projectDAO.GetProject(task, context); 
                        if (project != null)
                        {
                            PTaskProjectStaff taskProject = new PTaskProjectStaff()
                            {
                                Id = task.Id,
                                Description = task.Description,
                                Date = task.Date,
                                Hours = task.Hours,
                                ProjectName = project.ProjectName 
                            };
                            timesheetData.PTaskProject.Add(taskProject);
                        }
                    }
                }
            }

            return timesheetData; // return timesheet data
        }




    }
}
