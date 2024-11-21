using EasyTeams.Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTeams.Services.IService;



namespace EasyTeams.Services.Models
{
    // Class to hold the LeaveStaff model
    public class LeaveStaff
    {
        public int Id { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public DateOnly DateCreated { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool Authorised { get; set; }
        public DateTime AuthorisedDate { get; set; }
        public bool Sick { get; set; }
        public bool Rejected { get; set; }
        public string StaffId { get; set; }
        public string ManagerId { get; set; }
    }
}
