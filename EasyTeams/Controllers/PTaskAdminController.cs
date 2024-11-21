using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using EasyTeams.Services.IService;
using EasyTeams.Services.Models;
using EasyTeams.Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Identity;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EasyTeams.Controllers
{
    //[Authorize(Roles = "Manager")]
    public class PTaskAdminController : Controller
    {
        Helper helper;
        IPTaskService pTaskService;
        IProjectService projectService;
        IStaffService staffService;
        EasyTeamsContext context;
        IManagerService managerService;
        private readonly BankHolidaysService _bankHolidaysService;

        // PTaskAdminController constructor
        public PTaskAdminController()
        {
            helper = new Helper();
            pTaskService = new PTaskService();
            projectService = new ProjectService();
            staffService = new StaffService();
            context = new EasyTeamsContext();
            managerService = new ManagerService();
            _bankHolidaysService = new BankHolidaysService();
        }
        


        // GET: PTaskAdminController/ViewTask/
        //Get details of a task
        [Authorize(Roles = "Admin, Manager, Staff")]
        public ActionResult GetPTask(int id)
        {
            try
            {
                PTask pTask = pTaskService.GetPTask(id);
                Project project = projectService.GetProject(pTask);
                ViewBag.ProjectId = project.Id;
                return View(pTask);
            }
            catch
            {
                return View();
            }
        }
        //commenting this out because we are not using it 
        //public ActionResult GetPtasks()
        //{
        //    return View(pTaskService.GetPTasks());
        //}

        // GET: PTaskAdminController/GetTasksList
        //Get tasks under a project assigned by manager with associated staff names and completion status
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult GetPTasksList(int id)
        {
            try
            {
                List<PTaskProjectStaff> tasks = pTaskService.GetPTasksList(id);
                return View(tasks);
            }
            catch
            {
                return View(); // Error message
            }
        }
       
       // GET: PTaskAdminController/Create
       //Only manager/admin is authorised to create a task
       [Authorize(Roles = "Admin, Manager")]
        public ActionResult Create()
        {
            Manager manager = managerService.GetManager(HttpContext.Session.GetString("currentUserId"));
            ViewBag.projectList = helper.GetProjectDropdown(manager);
            ViewBag.staffList = helper.GetStaffDropdown(manager);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Create task - creates a new task in the database
        public async Task<ActionResult> Create(PTaskProjectStaff collection)
        {
            Staff staff = staffService.GetStaff(collection.StaffId);
            Manager manager = managerService.GetManager(HttpContext.Session.GetString("currentUserId"));
            //task has date and hour restrictions
            bool isWeekend = collection.Date.DayOfWeek == DayOfWeek.Saturday || collection.Date.DayOfWeek == DayOfWeek.Sunday;
            bool isBankHoliday = false;
            var bankHolidays = await _bankHolidaysService.GetBankHolidays(collection.Date.Year);
            if (bankHolidays != null) 
            {
                isBankHoliday = bankHolidays.Any(holiday =>
                    holiday.Year == collection.Date.Year &&
                    holiday.Month == collection.Date.Month &&
                    holiday.Day == collection.Date.Day);
            }
            int dailyHours = helper.CalculateDailyHours(collection.StaffId, collection.Date);
            if (isWeekend)
            {
                ViewBag.Message = "Task cannot be set on weekends.";
            }
            else if (isBankHoliday)
            {
                ViewBag.Message = "Task cannot be set on a bank holiday.";
            }
            else if (collection.Hours > 8)
            {
                ViewBag.Message = "Task cannot be set for more than 8 hours.";
            }
            else if (dailyHours + collection.Hours > 8)
            {
                ViewBag.Message = "Assigning this task would exceed the 8-hour daily limit.";
            }
            // In case of any error, throw an error message and redisplay the form with the details the user has entered.
            if (!string.IsNullOrEmpty(ViewBag.Message))
            {
                ViewBag.projectList = helper.GetProjectDropdown(manager);
                ViewBag.staffList = helper.GetStaffDropdown(manager);
                return View(collection); 
            }
            //if there is no error, create the task
            try
            {
                pTaskService.CreatePTask(collection, collection.StaffId); 
                return RedirectToAction("GetProjects", "ProjectAdmin", new {id = collection.ProjectId});
            }
            catch 
            {
                ViewBag.Message = "An error occurred while creating the task. Please try again.";
                ViewBag.projectList = helper.GetProjectDropdown(manager);
                ViewBag.staffList = helper.GetStaffDropdown(manager);
                return View(collection); 
            }
        }


        [Authorize(Roles = "Admin, Manager")]
        //Only manager/admin is authorised to edit a task
        public ActionResult Edit(int id)
        {
            PTask task = pTaskService.GetPTask(id);
            Project project = projectService.GetProject(task);
            ViewBag.ProjectId = project.Id;
            return View(task);
        }

        // POST: StaffController/Edit/5
        //Edit task - edits the existing records in the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, PTask collection)
        {
            Staff staff = staffService.GetStaff(collection);
            Manager manager = managerService.GetManager(HttpContext.Session.GetString("currentUserId"));
            PTask task = pTaskService.GetPTask(id);
            //Edit task has the same date and hour restrictions as create task
            bool isWeekend = collection.Date.DayOfWeek == DayOfWeek.Saturday ||
                 collection.Date.DayOfWeek == DayOfWeek.Sunday;
            bool isBankHoliday = false;
            var bankHolidays = await _bankHolidaysService.GetBankHolidays(collection.Date.Year);
            if (collection.Date != task.Date || collection.Hours > task.Hours)
            {
                if (bankHolidays != null)
                {
                    isBankHoliday = bankHolidays.Any(holiday =>
                        holiday.Year == collection.Date.Year &&
                        holiday.Month == collection.Date.Month &&
                        holiday.Day == collection.Date.Day);
                }

                int dailyHours = helper.CalculateDailyHours(staff.StaffId, collection.Date);

                if (isWeekend)
                {
                    ViewBag.Message = "Task cannot be set on weekends.";
                }
                else if (isBankHoliday)
                {
                    ViewBag.Message = "Task cannot be set on a bank holiday.";
                }
                else if (collection.Hours > 8)
                {
                    ViewBag.Message = "Task cannot be set for more than 8 hours.";
                }
                // If the date is unchanged and hours are modified,
                // subtract the existing task hours from daily hours and add the edited hour to ensure 8-hour daily limit check is done correctly
                else if ( (task.Date == collection.Date) && ((dailyHours - task.Hours) + collection.Hours > 8) )
                {
                  
                        ViewBag.Message = "Assigning this task would exceed the 8-hour daily limit.";

                }
                // If both date and hours are edited, ensure the total hours for the new date, including edited hours, do not exceed the 8-hour limit.
                else if ((task.Date != collection.Date) && (dailyHours + collection.Hours > 8))
                {
                    ViewBag.Message = "Assigning this task would exceed the 8-hour daily limit.";
                }

                // In case of any error, throw an error message and redisplay the form with the details the user has entered.
                if (!string.IsNullOrEmpty(ViewBag.Message))
                {
                    return View(collection);
                }
            }
            //if there is no error, edit the task
            try
            {
                pTaskService.EditPTask(collection, id);
                return RedirectToAction("GetPTask", "PTaskAdmin", new { id = collection.Id });
            }
            catch 
            {
                ViewBag.Message = "An error occurred while editing the task.";
                return View(collection);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Manager")]
        // This code extends the deadline for a task and reassigns it to another staff member. Note that this operation creates a new task rather than modifying the existing one.
        public ActionResult ReassignTask(int id)
        {
            PTask pTask = pTaskService.GetPTask(id);
            Manager manager = managerService.GetManager(HttpContext.Session.GetString("currentUserId"));
            ViewBag.StaffList = helper.GetStaffDropdown(manager);

            Project project = projectService.GetProject(pTask);
            Staff staff = staffService.GetStaff(pTask);
            PTaskProjectStaff taskCollection= new PTaskProjectStaff()
            {
                //Id = pTask.Id,  
                Description = pTask.Description,
                Date = pTask.Date,
                Hours = pTask.Hours,
                Report = pTask.Report,
                ProjectId = project.Id,
                ProjectName = project.ProjectName,
                StaffId = staff.StaffId,
            };
            return View(taskCollection);
        }


        // POST: PTaskAdminController/ReassignTask
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<ActionResult> ReassignTask(PTaskProjectStaff data)
        {
            Staff staff = staffService.GetStaff(data.StaffId);
            Manager manager = managerService.GetManager(HttpContext.Session.GetString("currentUserId"));

            // reassign tasks has same date and hour restrictions as create task
            bool isWeekend = data.Date.DayOfWeek == DayOfWeek.Saturday ||
                 data.Date.DayOfWeek == DayOfWeek.Sunday;
            bool isBankHoliday = false;
            var bankHolidays = await _bankHolidaysService.GetBankHolidays(data.Date.Year);

            if (bankHolidays != null)
            {
                isBankHoliday = bankHolidays.Any(holiday =>
                    holiday.Year == data.Date.Year &&
                    holiday.Month == data.Date.Month &&
                    holiday.Day == data.Date.Day);
            }

            int dailyHours = helper.CalculateDailyHours(data.StaffId, data.Date);

            if (isWeekend)
            {
                ViewBag.Message = "Task cannot be set on weekends.";
            }
            else if (isBankHoliday)
            {
                ViewBag.Message = "Task cannot be set on a bank holiday.";
            }
            else if (data.Hours > 8)
            {
                ViewBag.Message = "Task cannot be set for more than 8 hours.";
            }
            else if (dailyHours + data.Hours > 8)
            {
                ViewBag.Message = "Assigning this task would exceed the 8-hour daily limit.";
            }

            // In case of any error, throw an error message and redisplay the form with the details the user has entered.
            if (!string.IsNullOrEmpty(ViewBag.Message)) 
            {
                ViewBag.StaffList = helper.GetStaffDropdown(manager);
                return View(data); 
            }

            try
            {
                pTaskService.ReassignPTask(data, data.StaffId);
                return RedirectToAction("GetPTasksList", "PTaskAdmin", new { id = data.ProjectId });
            }
            catch 
            {
                ViewBag.Message = "An error occurred while reassigning the task.";
                ViewBag.StaffList = helper.GetStaffDropdown(manager);
                return View(data);
            }
        }




        // GET: PTaskAdminController/Delete/5
        //Only manager/admin can delete a task
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Delete(int id)
        {
            PTask pTask = pTaskService.GetPTask(id);
            Project project = projectService.GetProject(pTask);
            ViewBag.ProjectId = project.Id;
            return View(pTask);
        }

        // POST: PTaskAdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                PTask pTask = pTaskService.GetPTask(id);
                Project project = projectService.GetProject(pTask);
                
                pTaskService.DeletePTask(id);
                return RedirectToAction("GetPTasksList","PtaskAdmin", new {id = project.Id});
            }
            catch
            {
                return View();
            }
        }



        // Mark Task as Complete
        //only manager/admin can mark the task as finished.
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult MarkFinished(int id)
        {
            if (pTaskService.MarkPTaskFinished(id))
            {
                PTask task = pTaskService.GetPTask(id);
                Project project = projectService.GetProject(task);
                return RedirectToAction("GetPTaskslist","PTaskAdmin", new {id = project.Id});
            }
            else
            {
                // Error message
                return View();
            }
        }
        [Authorize(Roles = "Staff")]
        //Get pending tasks based on logged in staff 
        public ActionResult GetPendingTasks()
        {
            string id = HttpContext.Session.GetString("currentUserId");
            return View(pTaskService.GetPendingTasks(id));
        }
        
    }
}
