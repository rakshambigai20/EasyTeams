using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTeams.Data.Models.Domain
{
    // PTask class
    public class PTask
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public DateOnly Date {  get; set; } 
        public int Hours { get; set; }
        public string? Report {  get; set; }
        public bool ReportSubmitted { get; set; }
        public bool IsFinished { get; set; } = false; // Set to false by default
    }
}
