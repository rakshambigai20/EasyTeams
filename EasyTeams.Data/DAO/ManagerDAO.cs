using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Data.IDAO;
using EasyTeams.Data.Models.Repository;
using EasyTeams.Data.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace EasyTeams.Data.DAO
{
    // Manager Data Access Object
    public class ManagerDAO : IManagerDAO
    {
        IStaffDAO staffDAO;
        EasyTeamsContext context;

        // Constructor
        public ManagerDAO()
        {
            staffDAO = new StaffDAO();
            context = new EasyTeamsContext();
        }

        // Add a manager to the database
        public void AddToCollection(Project project, Manager manager, EasyTeamsContext context)
        {
            if (context.Managers.Contains(manager))
            {
                manager.Projects.Add(project);
            }
        }

        // Get a specific manager from the database
        public Manager GetManager(string id, EasyTeamsContext context)
        {
            context.Managers.Include(manager => manager.Leaves).Include(manager => manager.AuthorisedLeaves)
                .Include(manager => manager.Projects).Include(manager => manager.Staffs)
                .Include(manager => manager.Tasks).ToList(); //Eager loading of all collections that manager has
            return context.Managers.Find(id);
        }

        // Get all managers from the database
        public IList<Manager> GetManagers(EasyTeamsContext context) //Method to get all managers from the database and their collections
        {
            return context.Managers.Include(manager => manager.Staffs).Include(manager=>manager.AuthorisedLeaves).
                Include(manager=>manager.Projects).Include(manager=>manager.Tasks).Include(manager=>manager.Leaves).ToList();
        }
        //Method to get the manager of a specific staff member
        public Manager GetManager(Staff staff, EasyTeamsContext context)
        {
            IList<Manager> managers = GetManagers(context);
            Staff s = staffDAO.GetStaff(staff.StaffId, context);
            for (int i = 0; i < managers.Count; i++)
            {                 
                if (managers[i].Staffs.Contains(s))
                    return managers[i];
            }
            return null;
        }

        public void AddToCollection(Leave leave, string managerId, EasyTeamsContext context)
        {
            Manager manager = GetManager(managerId, context);
            manager.AuthorisedLeaves.Add(leave);
            context.SaveChanges();
        }
        //Get Manager based on approved leave
        public Manager GetManager(Leave leave, EasyTeamsContext context)
        {
            IList<Manager> managers = GetManagers(context);
            for (int i = 0; i < managers.Count; i++)
            {
                if (managers[i].AuthorisedLeaves.Contains(leave))
                    return managers[i];
            }
            return null;
        }

        public void RemoveFromCollection(Leave leave, Manager manager, EasyTeamsContext context)
        {
            manager.AuthorisedLeaves.Remove(leave);
            context.SaveChanges();
        }

        // Add a manager to the database
        public void AddManager(Manager manager, string adminId, EasyTeamsContext context)
        {
            context.Managers.Add(manager);
            Manager m = GetManager(adminId, context);
            m.Staffs.Add(manager);
            context.SaveChanges();
        }

        public void AddAdmin(Manager manager, EasyTeamsContext context)
        {
            context.Managers.Add(manager);
            context.SaveChanges();
        }
        public void AddToCollection(Staff staff, string managerId, EasyTeamsContext context)
        {
            Manager manager = GetManager(managerId, context);
            manager.Staffs.Add(staff);
            context.SaveChanges();
        }

        public void RemoveFromCollection(Project project, Manager manager, EasyTeamsContext context)
        {
            if (context.Managers.Contains(manager))
            {
                if (manager.Projects.Contains(project))
                {
                    manager.Projects.Remove(project);
                }
            }
        }

        // Get the manager of a specific task
        public Manager GetManager(PTask pTask, EasyTeamsContext context)
        {
            var manager = context.Managers
                                .Include(m => m.Tasks)
                                .FirstOrDefault(m => m.Tasks.Any(t => t.Id == pTask.Id));
            return manager;
        }

        // Get the manager of a specific project
        public Manager GetManager(Project project, EasyTeamsContext context)
        {
            var manager = context.Managers
                                .Include(m => m.Projects)
                                .FirstOrDefault(m => m.Projects.Any(p => p.Id == project.Id));
            return manager;
        }
        //Method to get the number of available staff for a manager for "today"
        public int AvailableStaff(string managerId, EasyTeamsContext context)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            Manager manager = GetManager(managerId, context);
            int staffOnLeave = 0;
            foreach (Staff staff in manager.Staffs)
            {
                context.Entry(staff).Collection(s => s.Leaves).Load(); //Load leaves for each staff
                foreach (Leave leave in staff.Leaves)
                {
                    if (leave.StartDate <= today && leave.EndDate >= today && leave.Authorised == true && leave.StartDate.Year == today.Year)
                    {
                        staffOnLeave++; //Increment staff on leave if the leave is authorised and today is within the leave period
                    }
                }
            }
            int availableStaff = manager.Staffs.Count - staffOnLeave; //Calculate available staff
            return availableStaff;
        }
    }
}
