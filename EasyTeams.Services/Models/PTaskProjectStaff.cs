using EasyTeams.Data.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTeams.Services.Models
{
    // This class is used to represent a project task for a staff member
    public class PTaskProjectStaff
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string? Description { get; set; }
        public int Hours { get; set; }
        public string? Report { get; set; }
        public bool IsFinished { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string StaffId { get; set; }

        public string StaffName{ get; set; }
    }
}
