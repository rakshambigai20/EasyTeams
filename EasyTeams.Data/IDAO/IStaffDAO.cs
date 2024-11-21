using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;

namespace EasyTeams.Data.IDAO
{
    // Staff Data Access Object Interface
    public interface IStaffDAO
    {
        void AddToCollection(PTask pTask, Staff staff, EasyTeamsContext context);
        IList<Staff> GetStaffs(EasyTeamsContext context);
        Staff GetStaff(PTask pTask, EasyTeamsContext context);
        Staff GetStaff(string id, EasyTeamsContext context);
        void AddToCollection(Leave leave, string staffId, EasyTeamsContext context);
        void EditStaff(Staff staff, EasyTeamsContext context);
        Staff GetStaff(Leave leave, EasyTeamsContext context);
        void RemoveFromCollection(Leave leave, Staff staff, EasyTeamsContext context);
        void RemoveFromCollection(PTask pTask, Staff staff, EasyTeamsContext context);
        void AddStaff(Staff staff, EasyTeamsContext context);
    }
}
