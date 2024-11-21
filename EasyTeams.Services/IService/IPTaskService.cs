using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using EasyTeams.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTeams.Services.IService
{

    // Project Service Interface
    public interface IPTaskService
    {
        public PTask GetPTask(int id);
        public List<PTask> GetPTasks();
        public List<PTaskProjectStaff> GetPTasksList(int Id);
        bool CreatePTask(PTaskProjectStaff pTaskProjectStaff, string staffId);
        bool EditPTask(PTask task, int id);
        public bool MarkPTaskFinished(int id);
        bool DeletePTask(int id);
        bool ReassignPTask(PTaskProjectStaff collection, string staffId);
        public List<PTask> GetTasks(string id);
        bool submitReport(int taskId, string staffId, string report);
        List<PTask> GetPendingTasks(string  id);
        List<PTask> Get5PendingTasks(string id);

    }
}
