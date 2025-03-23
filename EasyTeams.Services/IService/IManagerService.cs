
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using EasyTeams.Services.Service;

namespace EasyTeams.Services.IService
{
    // Manager Service Interface
    public interface IManagerService
    {
        Manager GetManager(string id);
        Manager GetManagerAndStaffLeaves(string id, EasyTeamsContext context);
        IList<Manager> GetManagers();
        Manager GetManager(Staff staff);
        Manager GetManager(Leave leave, EasyTeamsContext context);
        Manager GetManager(Leave leave);
        void AddManager(Manager manager, string adminId);
        int AvailableStaff(string managerId);
        void AddAdmin(Manager manager);
    }
}
