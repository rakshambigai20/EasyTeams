using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTeams.Data.Models.Domain
{
    // Manager class inherits from Staff
    public class Manager : Staff
    {
        public virtual ICollection<Staff> Staffs { get; set; } = new List<Staff>();
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<Leave> AuthorisedLeaves { get; set; } = new List<Leave>();
    }
}
