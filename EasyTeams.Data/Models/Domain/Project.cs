using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EasyTeams.Data.Models.Domain
{
    // Project class
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string? ProjectName { get; set; }
        public DateOnly Deadline { get; set;}
        public string? Description {  get; set; }
        public virtual ICollection<PTask>? Tasks { get; set; } = new List<PTask>();
    }
}