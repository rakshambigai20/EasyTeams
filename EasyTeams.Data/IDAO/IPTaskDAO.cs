using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTeams.Data.IDAO
{
    // Project Task Data Access Object Interface
    public interface IPTaskDAO
    {
        public PTask GetPTask(int id, EasyTeamsContext context);
        public List<PTask> GetPTasks(EasyTeamsContext context);
        void CreatePTask(PTask pTask, EasyTeamsContext context);
        void EditPTask(PTask task, EasyTeamsContext context);
        public bool submitReport(int taskId, string staffId,string report, EasyTeamsContext context);
        public bool MarkPTaskFinished(int id, EasyTeamsContext context);
        void DeletePTask(PTask pTask, EasyTeamsContext context);
        void ReassignPTask(PTask pTask, EasyTeamsContext context);
        List<PTask> GetTasks(String staffId, EasyTeamsContext context);
        List<PTask> GetPendingTasks(string id, EasyTeamsContext context);
        List<PTask> Get5PendingTasks(string id, EasyTeamsContext context);

    }
}
