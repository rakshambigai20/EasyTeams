
﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using EasyTeams.Services.Models;

namespace EasyTeams.Services.IService
{
    // Leave Service Interface
    public interface ILeaveService
    {
        IList<Leave> GetLeaves();
        Leave GetLeave(int id);
        IList<Leave> GetLeaves(Staff staff);
        bool AddLeave(LeaveStaff data, string staffId, string managerId);
        bool AddLeave(LeaveStaff data, string staffId);
        bool CancelLeave(Leave leave, Staff staff, Manager manager);
        bool CancelLeave(Leave leave, Staff staff);
        void UpdateLeave(Leave leave);
        IList<Leave> Get5lastLeaves(string managerId);
    }
}

