
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Data.Models.Repository;
using EasyTeams.Data.Models.Domain;

namespace EasyTeams.Data.IDAO
{
    // Manager Data Access Object Interface
    public interface IManagerDAO
    {
        void AddToCollection(Project project, Manager manager, EasyTeamsContext context);
        Manager GetManager(string id, EasyTeamsContext context);
        IList<Manager> GetManagers(EasyTeamsContext context);
        Manager GetManager(Staff staff, EasyTeamsContext context);
        Manager GetManager(PTask pTask, EasyTeamsContext context);
        void AddToCollection(Leave leave, string managerId, EasyTeamsContext context);
        Manager GetManager(Leave leave, EasyTeamsContext context);
        void RemoveFromCollection(Leave leave, Manager manager, EasyTeamsContext context);
        void RemoveFromCollection(Project project, Manager manager, EasyTeamsContext context);
        void AddManager(Manager manager, string adminId, EasyTeamsContext context);
        void AddToCollection(Staff staff, string managerId, EasyTeamsContext context);
        Manager GetManager(Project project, EasyTeamsContext context);
        int AvailableStaff(string managerId, EasyTeamsContext context);
    }
}
