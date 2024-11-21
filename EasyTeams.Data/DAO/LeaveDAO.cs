
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Data.IDAO;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using Microsoft.IdentityModel.Tokens;

namespace EasyTeams.Data.DAO
{
    // Leave Data Access Object
    public class LeaveDAO : ILeaveDAO
    {
        IStaffDAO staffDAO;
        IManagerDAO managerDAO;

        // Constructor
        public LeaveDAO()
        {
            staffDAO = new StaffDAO();
            managerDAO = new ManagerDAO();
        }

        // Get all leaves from the database
        public IList<Leave> GetLeaves(EasyTeamsContext context)
        {
            return context.Leaves.ToList();
        }

        // Get a specific leave from the database
        public Leave GetLeave(int id, EasyTeamsContext context)
        {
            return context.Leaves.Find(id);
        }

        // Add a leave to the database
        public void AddLeave(Leave leave, EasyTeamsContext context)
        {
            context.Leaves.Add(leave);
            context.SaveChanges();
        }
        //Method gets all leaves for a specific staff member (written before programmer discovered
        //you can actually pass staff.Leaves into a view) - Method not really in use
        public IList<Leave> GetLeaves(Staff staff, EasyTeamsContext context)
        {
            IList<Leave> allLeaves = GetLeaves(context); //Gets all leaves from the database
            Staff s = staffDAO.GetStaff(staff.StaffId, context);
            IList<Leave> staffLeaves = new List<Leave>(); //List to store staff leaves
            for (int i = 0; i < allLeaves.Count; i++)
            {
                if (s.Leaves.Contains(allLeaves[i])) //If the staff member is in the list of leaves, add the leave to the staffLeaves list
                    staffLeaves.Add(allLeaves[i]);  //Massive redundancy as you can just pass staff.Leaves into the view
            }
            return staffLeaves;
        }
        //Cancel leave removes the leave from the database
        public void CancelLeave(Leave leave, EasyTeamsContext context)
        {
            context.Leaves.Remove(leave);
            context.SaveChanges();
        }
        public void UpdateLeave(Leave leave, EasyTeamsContext context)
        {
            Leave l = context.Leaves.Find(leave.Id);
            context.Entry(l).CurrentValues.SetValues(leave);
        }
        //Method gets 5 last leave requests awaiting approval depending on the logged in manager
        public IList<Leave> Get5lastLeaves(string managerId, EasyTeamsContext context)
        {
            Manager manager = managerDAO.GetManager(managerId, context);
            IList<Leave> unapprovedLeaves = new List<Leave>();
            foreach (Staff staff in manager.Staffs)
            {
                context.Entry(staff).Collection(s => s.Leaves).Load(); //Load leaves for each staff
                foreach (Leave leave in staff.Leaves)
                {
                    if (leave.Authorised == false && leave.Rejected == false)
                        unapprovedLeaves.Add(leave);    //Add unapproved leaves to the list if condition is met
                }
            }
            unapprovedLeaves = unapprovedLeaves.OrderByDescending(l => l.RequestDate).ToList(); //Order the list by request date
            IList<Leave> last5Requests = unapprovedLeaves.Take(5).ToList(); //Take the first 5 requests
            return last5Requests;
        }
    }
}
