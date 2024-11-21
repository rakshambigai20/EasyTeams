using Azure.Core;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Services.IService;
using EasyTeams.Services.Models;
using EasyTeams.Services.Service;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;


namespace EasyTeams.Controllers
{
    // Controller for Staff
    public class StaffController : Controller
    {
        IStaffService staffService;
        IPTaskService pTaskService;
        IProjectService projectService;
        IManagerService managerService;
        ILeaveService leaveService;
        Helper helper;
        BankHolidaysService _bankHolidaysService;

        // Constructor to initialize the services
        public StaffController()
        {
            staffService = new StaffService();
            pTaskService = new PTaskService();
            projectService = new ProjectService();
            managerService = new ManagerService();
            leaveService = new LeaveService();
            helper = new Helper();
            _bankHolidaysService = new BankHolidaysService();
        }
        [Authorize(Roles = "Admin")]
        //method to get all users for admin
        public ActionResult GetStaffs()
        {
            return View(staffService.GetStaffs());
        }
        [Authorize(Roles = "Manager, Admin")]
        //Method to get staffs reporting to logged in manager
        public ActionResult GetMyStaffs()
        {
            Manager manager = managerService.GetManager(HttpContext.Session.GetString("currentUserId"));
            return View(manager.Staffs);
        }
        [Authorize(Roles = "Manager, Admin")]
        //Only Managers and Admins can view the leave statistics for their Staff
        public ActionResult LeaveStatistics()
        {
            Manager manager = managerService.GetManager(HttpContext.Session.GetString("currentUserId"));
            return View(manager.Staffs);
        } 
        [Authorize(Roles = "Admin, Manager")]
        //return the staff details - view for Admin and Manager including options to edit Staff
        public ActionResult GetStaff(string id)
        {
            Staff staff = staffService.GetStaff(id);
            return View(staff);
        }
        [Authorize(Roles = "Admin, Manager, Staff")]
        //Profile view for staff, manager and admin - no extra options included
        public ActionResult Profile()
        {
            Staff staff = staffService.GetStaff(HttpContext.Session.GetString("currentUserId"));
            return View(staff);
        }
        // GET: StaffController/Create
        [Authorize(Roles = "Admin, Manager")] //Only Admin and Manager can create a new staff
        public IActionResult Create()
        {
            return RedirectToPage("/Account/Register", new { area = "Identity" }); //Redirects to the Identity Register page
        }
        // GET: StaffController/Edit/5
        [Authorize(Roles = "Admin, Manager")] //Only Admin and Manager can edit the staff details
        public ActionResult Edit(string id)
        {
            Staff staff = staffService.GetStaff(id);
            return View(staff);
        }

        // POST: StaffController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Staff collection)
        {
            try
            {
                staffService.EditStaff(collection, id);
                Staff staff = staffService.GetStaff(id);
                return RedirectToAction("GetStaff", "Staff", new { id = staff.StaffId });

            }
            catch
            {
                return View();
            }
        }

        // GET: StaffController/Delete/5
        [Authorize(Roles = "Admin, Manager, Staff")]
        public ActionResult GetTasks()
        {
            string id = HttpContext.Session.GetString("currentUserId");
            return View(pTaskService.GetTasks(id));
        }
        [Authorize(Roles = "Staff")]
        //Only staff is authorised to submit report for a task
        public ActionResult SubmitReport(int id)
        {
            PTask task = pTaskService.GetPTask(id);
            return View(task);
        }
        // POST: For processing the form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitReport(int id, string report)
        {
            try
            {
                string staffId = HttpContext.Session.GetString("currentUserId");
                bool result = pTaskService.submitReport(id, staffId, report);
                if (result)
                {
                    return RedirectToAction("ViewReport", "Staff", new { id = id });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to submit report.");
                    return View(pTaskService.GetPTask(id));  
                }
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while submitting the report.");
                return View(pTaskService.GetPTask(id));
            }
        }
        [Authorize(Roles = "Admin, Manager, Staff")]
        //Method to view report submitted by staff
        public ActionResult ViewReport(int id)
        {
            var task = pTaskService.GetPTask(id);
            if (task != null && task.ReportSubmitted)
            {
                Project project = projectService.GetProject(task);
                ViewBag.ProjectId = project.Id;
                return View(task);
            }
            return RedirectToAction("GetTasks"); //Redirects to the GetTasks page if the report is not submitted
        }
        


        [Authorize(Roles = "Admin, Manager, Staff")]
        //method to view timesheet of a staff. 
        public ActionResult ViewTimesheet(string id)
        {

            //String staffId = HttpContext.Session.GetString("currentUserId");
            var timesheet = staffService.GenerateTimesheet(id);
            if (timesheet.staff == null)
            {
                if (User.IsInRole("Manager, Admin"))
                    return RedirectToAction("Staff", "GetMyStaffs");
            }

            return View(timesheet); //Returns the timesheet view
        }

        [Authorize(Roles = "Admin, Manager, Staff")]
        public async Task<IActionResult> Dashboard()
        {
            if (User.Identity.IsAuthenticated) //If the user is authenticated
            {
                string userId = HttpContext.Session.GetString("currentUserId");                     //Gets the current user id
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);                               //Gets the current date
                Staff unknown = staffService.GetStaff(userId);                                      //Gets the staff - uknown since we don't know if Staff is also Manager
                IList<Leave> leaves = leaveService.GetLeaves(unknown);                              //Gets the leaves for the staff
                int approvedLeaveDays = await helper.CalculateApprovedLeaveDays(leaves, today);     //Calculates the approved leave days as a base for remaining leave
                ViewBag.remainingLeaveDays = 28 - approvedLeaveDays;                                //Calculates the remaining leave days
                Dictionary<DateTime, string> leaveDictionary = await helper.Calendar(userId);       //Gets the dictionary for the calendar
                ViewBag.LeaveDictionary = leaveDictionary;                                          //Passes the dictionary to the view

                if (User.IsInRole("Manager")) //If the user is a manager
                {
                    var manager = managerService.GetManager(userId);                                //Gets the manager
                    ViewBag.last5Requests = leaveService.Get5lastLeaves(userId);                    //Gets the 5 most recent leave requests from manager's staff
                    ViewBag.ongoingProjects = projectService.OngoingProjects(DateOnly.FromDateTime(DateTime.Today), userId); //Gets the number of ongoing projects for the manager
                    ViewBag.availableStaff = managerService.AvailableStaff(userId);                 //Gets the number of available staff for the manager
                    ViewBag.upcomingProjects = projectService.UpcomingProjects(userId);             //Gets the upcoming projects deadlines for the manager (initially also displayed in the dashboard view)

                    return View("Dashboard", manager);                                              //Returns the dashboard view for manager
                }
                else if (User.IsInRole("Staff")) // If the user is a staff
                {
                    var staff = staffService.GetStaff(userId);
                    IList<PTask> pendingTasks = pTaskService.GetPendingTasks(userId);               //Gets the pending tasks for the staff
                    IList<PTask> fivePendingTasks = pTaskService.Get5PendingTasks(userId);          //Gets the first 5 pending tasks for the staff to display as a list in the view
                    ViewBag.first5PendingTasks = fivePendingTasks;                                  //Passes the first 5 pending tasks to the view
                    ViewBag.pendingTasks = pendingTasks;                                            //Passes the pending tasks to the view
                    ViewBag.PendingCount = pendingTasks.Count;                                      //Passes the count of pending tasks to the view
                    return View("Dashboard", staff);                                                //Returns the dashboard view for staff
                }
                else if (User.IsInRole("Admin")) //If the user is an admin
                {
                    var manager = managerService.GetManager(userId);                                //Gets the Manager as role Admin is assigned to Manager object
                    ViewBag.last5Requests = leaveService.Get5lastLeaves(userId);                    //Since new Manager's are added to Admin's collection, they can view the leave requests
                    ViewBag.ongoingProjects = projectService.OngoingProjects(DateOnly.FromDateTime(DateTime.Today), userId); //Gets the ongoing projects for the manager
                    ViewBag.availableStaff = managerService.AvailableStaff(userId);                 //Gets the available Managers and Staff in admin's collection
                    ViewBag.upcomingProjects = projectService.UpcomingProjects(userId);             //Gets the upcoming projects for the manager

                    return View("Dashboard", manager); //Returns the dashboard view for admin
                }

                // If the user is authenticated but not in a recognized role
                ViewBag.ErrorMessage = "You are not authorized to access this dashboard.";
                return View("Error", ViewBag);
            }
            else
            {
                // If the user is not authenticated, redirect to the login page
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
        }
        [Authorize(Roles = "Admin, Manager, Staff")] //Only Admin, Manager and Staff can view the calendar
        public async Task<IActionResult> Calendar()
        {
                string userId = HttpContext.Session.GetString("currentUserId");
            //Dictionary with key: Dates, value: leave type    
            Dictionary<DateTime, string> leaveDictionary = await helper.Calendar(userId);
            //leaveDictionary is passed in the viewbag to access in the calendar view
                ViewBag.LeaveDictionary = leaveDictionary;
            //Dictionary with key: Deadline date, value: project name
            Dictionary<DateTime, string> upcomingProjects = new Dictionary<DateTime, string>();
            var projects = projectService.UpcomingProjects(userId);
            foreach (Project project in projects)
            {
                DateTime deadline = project.Deadline.ToDateTime(new TimeOnly(0, 0)); 
                string projectName = project.ProjectName;

                if (!upcomingProjects.ContainsKey(deadline))
                {
                    upcomingProjects.Add(deadline, projectName); //Adds the project to the dictionary
                }

            }
            //upcoming project dictionary is passed in the viewbag to access in the calendar view

            ViewBag.upcomingProjects = upcomingProjects; 
            return View();
        }

    }
}

