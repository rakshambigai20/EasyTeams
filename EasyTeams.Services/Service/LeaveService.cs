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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EasyTeams.Services.Service
{
    // Service class to manage the leave requests
    public class LeaveService : ILeaveService
    {
        ILeaveDAO leaveDAO;
        IStaffDAO staffDAO;
        IManagerDAO managerDAO;

        // Constructor
        public LeaveService()
        {
            leaveDAO = new LeaveDAO();
            staffDAO = new StaffDAO();
            managerDAO = new ManagerDAO();
        }

        // Method to get all the leaves - used for partial view
        public IList<Leave> GetLeaves()
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return leaveDAO.GetLeaves(context);
            }
        }

        // Method to get the leaves of a staff
        public IList<Leave> GetLeaves(Staff staff)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return leaveDAO.GetLeaves(staff, context);
            }
        }

        // Method to get the leaves by id
        public Leave GetLeave(int id)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return leaveDAO.GetLeave(id, context);
            }
        }

        // Add approved leave to collections
        public bool AddLeave(LeaveStaff data, string staffId, string managerId)
        {
            // Try to add the leave
            try
            {
                Leave leave = new Leave()
                {
                    Id = data.Id,
                    RequestDate = data.RequestDate,
                    StartDate = data.StartDate,
                    EndDate = data.EndDate,
                    Authorised = data.Authorised,
                    AuthorisedDate = data.AuthorisedDate, //date only changed if leave is approved
                    Sick = data.Sick,
                };
                using (EasyTeamsContext context = new EasyTeamsContext())
                {
                    leaveDAO.AddLeave(leave, context);
                    staffDAO.AddToCollection(leave, staffId, context);
                    managerDAO.AddToCollection(leave, managerId, context);
                    context.SaveChanges();
                }
                return true; // Return true if the leave was added
            }
            catch // Catch the exception
            {
                return false; // Return false if the leave was not added
            }
        }

        // Method to add a leave after a request
        public bool AddLeave(LeaveStaff data, string staffId)
        {
            // Try to add the leave
            try
            {
                Leave leave = new Leave()
                {
                    Id = data.Id,
                    StartDate = data.StartDate,
                    EndDate = data.EndDate,
                    Authorised = data.Authorised,
                    Sick = data.Sick,
                };
                using (EasyTeamsContext context = new EasyTeamsContext())
                {
                    leaveDAO.AddLeave(leave, context);
                    staffDAO.AddToCollection(leave, staffId, context);
                    context.SaveChanges();
                }
                return true; // Return true if the leave was added
            }
            catch
            {
                return false; // Return false if the leave was not added
            }
        }
        
        // Method to cancel a leave
        public bool CancelLeave(Leave leave, Staff staff, Manager manager)
        {
            // Try to cancel the leave
            try
            {
                using (EasyTeamsContext context = new EasyTeamsContext())
                {
                    staffDAO.RemoveFromCollection(leave, staff, context);
                    managerDAO.RemoveFromCollection(leave, manager, context);
                    leaveDAO.CancelLeave(leave, context);
                    context.SaveChanges();
                    return true; // Return true if the leave was cancelled
                }
            }
            catch
            {
                return false; // Return false if the leave was not cancelled
            }
        }

        //Method to remove unapproved leave
        public bool CancelLeave(Leave leave, Staff staff)
        {

            // Try to cancel the leave
            try
            {
                using (EasyTeamsContext context = new EasyTeamsContext())
                {
                    staffDAO.RemoveFromCollection(leave, staff, context);
                    leaveDAO.CancelLeave(leave, context);
                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false; // Return false if the leave was not cancelled
            }
        }

        // Method to update a leave
        public void UpdateLeave(Leave leave)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                leaveDAO.UpdateLeave(leave, context);
                context.SaveChanges();
            }
        }

        // Method to get 5 newest leave requests from manager's staff collection
        public IList<Leave> Get5lastLeaves(string managerId)
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                return leaveDAO.Get5lastLeaves(managerId, context);
            }
        }
    }
}
