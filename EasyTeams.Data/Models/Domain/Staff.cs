using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EasyTeams.Data.Models.Domain
{
    // Staff class
    public class Staff
    {
        [Key]
        public string StaffId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string WorkEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public virtual ICollection<Leave> Leaves { get; set; } = new List<Leave>();
        public virtual ICollection<PTask> Tasks { get; set; } = new List<PTask>();
    }
}
