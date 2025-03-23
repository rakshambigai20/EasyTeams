
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using EasyTeams.Services.IService;
using EasyTeams.Data.IDAO;
using EasyTeams.Data.DAO;

namespace EasyTeams.Services.Service
{
    // Manager Service
    public class ManagerService : IManagerService
    {
        
        public IManagerDAO managerDAO;

        // Constructor
        public ManagerService()
        {
            managerDAO = new ManagerDAO();
        }

        // Get Manager
        public Manager GetManager(string id)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return managerDAO.GetManager(id, context);
            }
        }

        // Get Manager and their Staff+Leaves
        public Manager GetManagerAndStaffLeaves(string id, EasyTeamsContext context)
        {
                return managerDAO.GetManager(id, context);
        }

        // Get List of Managers
        public IList<Manager> GetManagers()
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return managerDAO.GetManagers(context);
            }
        }

        // Get Manager through Staff
        public Manager GetManager(Staff staff)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return managerDAO.GetManager(staff, context);
            }
        }

        // Get Manager through Leave
        public Manager GetManager(Leave leave, EasyTeamsContext context)
        {
            return managerDAO.GetManager(leave, context);
        }

        // Get Manager through Leave - separate context
        public Manager GetManager(Leave leave)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return managerDAO.GetManager(leave, context);
            }
        }

        //Add new Manager
        public void AddManager(Manager manager, string adminId)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                managerDAO.AddManager(manager, adminId, context);
                context.SaveChanges();
            }
        }

        public void AddAdmin(Manager manager)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                managerDAO.AddAdmin(manager, context);
                context.SaveChanges();
            }
        }

        //Calculate available staff from Manager's collection (Staff not on leave)
        public int AvailableStaff(string managerId)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return managerDAO.AvailableStaff(managerId, context);
            }
        }
    }
}
