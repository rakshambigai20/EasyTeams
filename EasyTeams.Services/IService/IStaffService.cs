using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using EasyTeams.Services.Models;

namespace EasyTeams.Services.IService
{
    // Staff Service Interface
    public interface IStaffService
    {
        public IList<Staff> GetStaffs();
        public Staff GetStaff(string id);
        bool EditStaff(Staff staff, string id);
        //Staff GetStaff(Leave leave, EasyTeamsContext context);
        Staff GetStaff(Leave leave);
        void AddStaff(Staff staff, string managerId);
        Staff GetStaff(PTask pTask);

       
        Timesheet GenerateTimesheet(string staffId);
    }
}
