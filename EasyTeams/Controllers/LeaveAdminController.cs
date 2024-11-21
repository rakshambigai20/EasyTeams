using EasyTeams.Services.Models;
using EasyTeams.Services.Service;
using EasyTeams.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EasyTeams.Data.DAO;
using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Build.Construction;

namespace EasyTeams.Controllers
{
    // LeaveAdminController class
    public class LeaveAdminController : Controller
    {
        Helper helper = new Helper();
        ILeaveService leaveService = new LeaveService();
        IStaffService staffService = new StaffService();
        IManagerService managerService = new ManagerService();

        // GET: LeaveAdminController
        [Authorize(Roles = "Admin, Manager")]
        //List of all leaves in the system mainly used for partial view in staff's details
        public ActionResult LeavesList()
        {
            return View(leaveService.GetLeaves());
        }
        //List of leaves based on Staff in Manager's collection 
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Leaves()
        {
            using (EasyTeamsContext context = new EasyTeamsContext())
            {
                Manager manager = managerService.GetManagerAndStaffLeaves(HttpContext.Session.GetString("currentUserId"), context);
                foreach (Staff staff in manager.Staffs)
                {
                    context.Entry(staff).Collection(s => s.Leaves).Load(); //Load leaves for each staff
                }
                return View(manager.Staffs);
            }
        }
        //List of leaves based on logged in Staff/Manager/Admin
        [Authorize(Roles = "Admin, Manager, Staff")]
        public async Task<ActionResult> GetYourLeave()
        {
            Staff staff = staffService.GetStaff(HttpContext.Session.GetString("currentUserId"));
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            IList<Leave> leaves = leaveService.GetLeaves(staff);
            int approvedLeaveDays = await helper.CalculateApprovedLeaveDays(leaves, today);
            ViewBag.remainingLeaveDays = 28 - approvedLeaveDays;
            //return View(leaveService.GetLeaves(staff));
            return View(staff.Leaves);
        }

        // GET: LeaveAdminController/Details/5
        [Authorize(Roles = "Admin, Manager, Staff")]
        //Leave details for the author of the leave
        public ActionResult Details(int id)
        {
            Leave leave = leaveService.GetLeave(id);
            return View(leave);
        }
        [Authorize(Roles = "Admin, Manager")]
        //Leave details for Staff's Manager
        public ActionResult LeaveDetails(int id)
        {
            Leave leave = leaveService.GetLeave(id);
            return View(leave);
        }
        // GET: LeaveAdminController/Create
        [Authorize(Roles = "Admin, Manager, Staff")]
        //Every type of user can create leave request
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LeaveStaff collection)
        {
            collection.StaffId = HttpContext.Session.GetString("currentUserId");
            //collection.RequestDate = DateTime.Now;
            collection.DateCreated = DateOnly.FromDateTime(DateTime.Today); //I know, this is redundant date but all bool checks are based on DateOnly
            Staff staff = staffService.GetStaff(collection.StaffId);
            Manager manager = managerService.GetManager(staff);
            //IList<Leave> leaves = leaveService.GetLeaves(staff);
            try
            {
                collection.Authorised = false;
                collection.Rejected = false;
                //leave has certain conditions it has to meet before being submitted
                bool isToday = helper.IsToday(collection.DateCreated, collection.StartDate, collection.Sick);
                bool isTomorrow = await helper.IsTomorrow(collection.DateCreated, collection.StartDate, collection.Sick);
                bool isRequestOver28Days = await helper.CalculateWorkingDays(collection.StartDate, collection.EndDate) > 28;
                bool moreThan28DaysTaken = (await helper.CalculateApprovedLeaveDays(staff, collection.DateCreated) + await helper.CalculateWorkingDays(collection.StartDate, collection.EndDate)) > 28;
                //specific error messages for each condition
                if (isToday == true)
                {
                    ViewBag.Message = "Your Manager will not be able to accept or reject this request on time. Please choose different dates.";
                }
                else if (isTomorrow == true)
                {
                    ViewBag.Message = "Your Manager will not be able to accept or reject this request on time due to tomorrow's Bank Holiday. Please choose different dates.";
                }
                else if (isRequestOver28Days == true)
                {
                    ViewBag.Message = "Your requested leave exceeds 28 days of annual leave. Please choose different dates.";
                }
                else if (moreThan28DaysTaken == true)
                {
                    ViewBag.Message = "Your total of annual leaves taken with this request exceeds 28 days. Please change requested dates or contact your Manager to request unpaid leave.";
                }
                else
                {
                    //leaveService.AddLeave(collection, staff.StaffId, manager.StaffId);
                    leaveService.AddLeave(collection, staff.StaffId);
                    //await helper.SendMail(staff.WorkEmail, staff.WorkEmail, "Your Leave Request", "Your leave request has been submitted to " + manager.FirstName + " " + manager.LastName + ".");
                    //await helper.SendMail(staff.WorkEmail, manager.WorkEmail, "New Leave Request", "On " + collection.RequestDate + "new leave request has been submitted for " + staff.FirstName + " " + staff.LastName + ".");
                    return RedirectToAction("GetYourLeave", "LeaveAdmin");
                }
                return View(collection);
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAdminController/Edit/5
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult ApproveReject(int id)
        {
            Leave leave = leaveService.GetLeave(id);
            return View(leave);
        }

        // POST: LeaveAdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveReject(Leave leave, string decision)
        {
            try
            {
                Staff staff = staffService.GetStaff(leave);
                using (EasyTeamsContext context = new EasyTeamsContext())
                {
                    context.Entry(staff).Collection(s => s.Tasks).Load(); //Loads staff leaves
                }
                Manager manager = managerService.GetManager(staff); //gets staff's manager
                helper.ApproveReject(leave, decision, staff, manager); //calls helper method to make this action code light
                return RedirectToAction("Leaves", "LeaveAdmin");
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAdminController/Delete/5
        [Authorize(Roles = "Admin, Manager, Staff")]
        public ActionResult Cancel(int id)
        {
            Leave leave = leaveService.GetLeave(id);
            return View(leave);
        }

        // POST: LeaveAdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Cancel(int id, LeaveStaff collection)
        {
            Leave leave = leaveService.GetLeave(id);
            DateOnly requestDate = DateOnly.FromDateTime(DateTime.Today);
            bool days = await helper.CalculateWorkingDays(requestDate, leave.StartDate) > 3; //checks if leave is within 3 working days
            try
            {
                if (leave.Authorised == true)
                {
                    if (days == false && leave.Sick == false) //error message if leave is within 3 working days
                    {
                        ViewBag.Message = "You cannot cancel this leave request as it is within 3 working days of the start date.";
                        return View(leave);
                    }
                    else
                    {
                        Staff staff = staffService.GetStaff(leave);
                        Manager manager = managerService.GetManager(staff);
                        //helper.SendMail(staff.WorkEmail, staff.WorkEmail, "Leave Cancellation", "Your leave have been cancelled. Cancellation information has been sent to " + manager.FirstName + " " + manager.LastName + ".");
                        //helper.SendMail(staff.WorkEmail, manager.WorkEmail, "New Leave Cancellation", staff.FirstName + staff.LastName + "has cancelled their leave planned for " + collection.StartDate + "-" + collection.EndDate + ".");
                        leaveService.CancelLeave(leave, staff, manager);
                        return RedirectToAction("GetYourLeave", "LeaveAdmin");
                    }
                }
                else
                {
                    ViewBag.Message = "Something went wrong.";
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAdminController/RemainingLeaveDays
        [Authorize(Roles = "Admin, Manager, Staff")]
        public async Task<ActionResult> RemainingLeaveDays()
        {
            try
            {
                // Get the current staff's ID from the session
                string staffId = HttpContext.Session.GetString("currentUserId");

                // Get the number of approved days for the current staff
                int approvedDays = await helper.CalculateApprovedLeaveDays(leaveService.GetLeaves(staffService.GetStaff(staffId)), DateOnly.FromDateTime(DateTime.Today));
                // Calculate the remaining leave days for the current staff
                //int remainingLeaveDays = await helper.CalculateRemainingLeaveDays(staffId, approvedDays);
                int remainingLeaveDays = 28 - approvedDays;
                return View(remainingLeaveDays);
            }
            catch
            {
                return View("Error"); // Return the error view if an exception occurs
            }
        }

    }
}
