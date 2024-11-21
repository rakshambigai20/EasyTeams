
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;

namespace EasyTeams.Data.IDAO
{
    // Leave Data Access Object Interface
    public interface ILeaveDAO
    {
        IList<Leave> GetLeaves(EasyTeamsContext context);
        Leave GetLeave(int id, EasyTeamsContext context);
        void AddLeave(Leave leave, EasyTeamsContext context);
        IList<Leave> GetLeaves(Staff staff, EasyTeamsContext context);
        void CancelLeave(Leave leave, EasyTeamsContext context);
        void UpdateLeave(Leave leave, EasyTeamsContext context);
        IList<Leave> Get5lastLeaves(string managerId, EasyTeamsContext context);
    }
}

