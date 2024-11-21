using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTeams.Data.Models.Domain
{

    // Leave class
    public class Leave
    {
        [Key]
        public int Id { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool Authorised { get; set; }
        public DateTime AuthorisedDate { get; set; }
        public bool Sick {  get; set; }
        public bool Rejected { get; set; }
    }
}
