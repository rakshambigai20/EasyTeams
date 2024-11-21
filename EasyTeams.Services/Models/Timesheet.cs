using EasyTeams.Data.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTeams.Services.Models
{

    // This class is used to represent a timesheet for a staff member
    public class Timesheet
    {
        public Staff staff { get; set; }
        public virtual ICollection<Project> Projects { get; set; }

        public virtual ICollection<PTaskProjectStaff> PTaskProject { get; set; }
        public Timesheet()
        {
            PTaskProject = new List<PTaskProjectStaff>();
        }

    }
}
