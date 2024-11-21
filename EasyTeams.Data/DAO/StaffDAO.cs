using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Data.IDAO;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace EasyTeams.Data.DAO
{

    // Staff Data Access Object
    public class StaffDAO : IStaffDAO
    {

        // Get all staff from the database
        public IList<Staff> GetStaffs(EasyTeamsContext context)
        {
            return context.Staffs.Include(staff => staff.Leaves).Include(staff=>staff.Tasks).ToList();
        }
        //Get Staff based on staff id
        public Staff GetStaff(string id, EasyTeamsContext context)
        {
            context.Staffs.Include(staff => staff.Leaves)
                .Include(staff => staff.Tasks).ToList(); //Eager loading of all collections that staff has
            return context.Staffs.Find(id);
        }

        //This method is used to create task. 
        public void AddToCollection(PTask pTask, Staff staff, EasyTeamsContext context)
        {
            if (context.Staffs.Contains(staff))
            {
                staff.Tasks.Add(pTask); //Add task to the staff's collection
            }
        }
        //This method is used to create leave
        public void AddToCollection(Leave leave, string staffId, EasyTeamsContext context)
        {
            Staff staff = GetStaff(staffId, context);
            staff.Leaves.Add(leave); //Add leave to the staff's collection
            context.SaveChanges();
        }

        // Edit a staff member in the database
        public void EditStaff(Staff staff, EasyTeamsContext context)
        {
            Staff s = context.Staffs.Find(staff.StaffId);
            context.Entry(s).CurrentValues.SetValues(staff);
        }
        //Get staff based on leave
        public Staff GetStaff(Leave leave, EasyTeamsContext context)
        {
            IList<Staff> staffs = GetStaffs(context);
            Leave l = context.Leaves.Find(leave.Id);
            for (int i = 0; i < staffs.Count; i++)
            {
                if (staffs[i].Leaves.Contains(l))
                    return staffs[i];
            }
            return null;
        }

        // This method is used to cancel leave
        public void RemoveFromCollection(Leave leave, Staff staff, EasyTeamsContext context)
        {
            staff.Leaves.Remove(leave); //remove leave from the staff's collection
            context.SaveChanges();
        }

        // Add a staff member to the database
        public void AddStaff(Staff staff, EasyTeamsContext context)
        {
            context.Staffs.Add(staff);
            context.SaveChanges();
        }

        //This method is used to delete task
        public void RemoveFromCollection(PTask pTask, Staff staff, EasyTeamsContext context)
        {
            if (context.Staffs.Contains(staff))
            {
                if (staff.Tasks.Contains(pTask))
                {
                    staff.Tasks.Remove(pTask); //remove task from the staff's collection
                }
            }
        }
        //Get staff based on task
        public Staff GetStaff(PTask pTask, EasyTeamsContext context)
        {
            var staff = context.Staffs
                                .Include(s => s.Tasks)
                                .FirstOrDefault(s => s.Tasks.Any(t => t.Id == pTask.Id));
            return staff; // Return staff member that has the task
        }
    }
}