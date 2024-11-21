using EasyTeams.Data.Models.Domain;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net;
using System.Net.Mail;
using EasyTeams.Data.DAO;
using EasyTeams.Services.Service;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using EasyTeams.Data.Models.Repository;
using EasyTeams.Services.IService;
using EasyTeams.Services.Models;
using EasyTeams.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using NuGet.ProjectModel;


namespace EasyTeams.Controllers
{

    // Helper class to provide common methods for the controllers
    public class Helper : Controller
    {
        EasyTeamsContext context;
        IManagerService managerService;
        IProjectService projectService;
        IStaffService staffService;
        IPTaskService pTaskService;

        // BankHolidaysService is a service that provides bank holidays for the UK - already filtered to England and Wales
        private readonly BankHolidaysService _bankHolidaysService;
        ILeaveService leaveService;

        // Constructor
        public Helper()
        {
            _bankHolidaysService = new BankHolidaysService();
            leaveService = new LeaveService();
            managerService = new ManagerService();
            projectService = new ProjectService();
            staffService = new StaffService();
            pTaskService = new PTaskService();
        }
        //Send Email method unfortunately not working. Leaving it here if in the future client wants to implement it and has SMTP/Mail Server configured.
        public async Task SendMail(string author, string recipient, string subject, string body)
        {
            MailAddress from = new MailAddress(author);
            MailAddress to = new MailAddress(recipient);
            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = body;
            SmtpClient client = new SmtpClient();
            await client.SendMailAsync(message);
        }
        #region(Days Calculations)
        //Calculate days between two dates excluding weekends (does not take into consideration Bank Holidays)
        public int CalculateDays(DateOnly startDate, DateOnly endDate)
        {
            int workingDays = 0;
            for (DateOnly date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }
            return workingDays;
        }

        // Calculate the number of approved leave days for a staff member based on list of leaves
        public async Task<int> CalculateApprovedLeaveDays(IList<Leave> leaves, DateOnly dateCreated)
        {
            int currentYear = dateCreated.Year;
            int workingDays = 0;
            foreach (var leave in leaves) //Takes into consideration only current year, approved and non-sick leaves
            {
                if (leave.StartDate.Year == currentYear && leave.EndDate.Year == currentYear && leave.Authorised == true && leave.Sick == false)
                {
                    workingDays += await CalculateWorkingDays(leave.StartDate, leave.EndDate);
                }
            }
            return workingDays;
        }

        //Calculate the number of approved leave days for a staff member based on staff.Leaves
        //Async method since we need to wait for the result of CalculateWorkingDays
        public async Task<int> CalculateApprovedLeaveDays(Staff staff, DateOnly dateCreated)
        {
            int currentYear = dateCreated.Year;
            int workingDays = 0;
            foreach (var leave in staff.Leaves) //calculates only current year, approved and non-sick leaves
            {
                if (leave.StartDate.Year == currentYear && leave.EndDate.Year == currentYear && leave.Authorised == true && leave.Sick == false)
                {
                    workingDays += await CalculateWorkingDays(leave.StartDate, leave.EndDate); //calls the method to calculate working days
                }
            }
            return workingDays;
        }

        // Checks conditions of start date and request date
        public bool IsToday(DateOnly dateCreated, DateOnly startDate, bool sick)
        {
            //Is the Staff requesting annual leave on Friday for the following Monday?
            if (dateCreated.DayOfWeek == DayOfWeek.Friday && startDate == dateCreated.AddDays(3))
            {
                return true;
            }
            //Is the Staff requesting annual leave for the next day?
            if (startDate == dateCreated.AddDays(1) && sick == false)
            {
                return true;
            }
            //Is the Staff requesting annual leave for the same day?
            if (startDate == dateCreated && sick == false)
            {
                return true;
            }
            return false;
        }

        // Check if the following date is a bank holiday
        public async Task<bool> IsTomorrow(DateOnly dateCreated, DateOnly startDate, bool sick)
        {
            if (sick == false)
            {
                var _bankHolidays = await _bankHolidaysService.GetBankHolidays(startDate.Year);
                DateOnly tomorrow = dateCreated.AddDays(1);
                if (startDate.Equals(tomorrow) && _bankHolidays.Any(holiday => holiday.Year == tomorrow.Year && holiday.Month == tomorrow.Month && holiday.Day == tomorrow.Day))
                {
                    return true;
                }
            }
            return false;
        }

        // Calculate the number of working days between two dates
        public async Task<int> CalculateWorkingDays(DateOnly startDate, DateOnly endDate)
        {
            int workingDays = 0;
            for (DateOnly date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday) //Excludes weekends
                {
                    var bankHolidays = await _bankHolidaysService.GetBankHolidays(date.Year); //Get bank holidays for the year
                    if (!bankHolidays.Any(holiday => holiday.Year == date.Year && holiday.Month == date.Month && holiday.Day == date.Day))
                    {
                        workingDays++; //If date is not a bank holiday, increment working days
                    }
                }
            }
            return workingDays;
        }

        // Calculate the remaining leave days for a staff member
        //Longer, a bit redundant method
        public async Task<int> CalculateRemainingLeaveDays(string staffId)
        {
            int totalAnnualLeaveEntitlement = 28; // Example: Assume 28 days is the total annual leave entitlement

            // Get the list of leaves for the staff
            Staff staff = staffService.GetStaff(staffId);
            IList<Leave> leaves = leaveService.GetLeaves(staff);

            // Filter the leaves to get only approved and non-sick leaves for the current year
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Today);
            IList<Leave> approvedLeaves = leaves.Where(leave =>
                leave.StartDate.Year == currentDate.Year &&
                leave.EndDate.Year == currentDate.Year &&
                leave.Authorised &&
                !leave.Sick
            ).ToList();

            // Calculate the total approved leave days using the existing method
            int totalApprovedLeaveDays = await CalculateApprovedLeaveDays(approvedLeaves, currentDate);

            // Calculate the remaining leave days
            int remainingLeaveDays = totalAnnualLeaveEntitlement - totalApprovedLeaveDays;

            return remainingLeaveDays;
        }

        // Calculate the number of sick days for a staff member
        public async Task<int> CalculateSickDays(Staff staff, DateOnly dateCreated)
        {
            int currentYear = dateCreated.Year;
            int workingDays = 0;
            foreach (var leave in staff.Leaves)
            {
                if (leave.StartDate.Year == currentYear && leave.EndDate.Year == currentYear && leave.Authorised == true && leave.Sick != false)
                {
                    workingDays += await CalculateWorkingDays(leave.StartDate, leave.EndDate);
                }
            }
            return workingDays;
        }

        //method to calculate daily working hours for a staff
        public int CalculateDailyHours(string staffId, DateOnly taskDate)
        {

            // Retrieve tasks for the same day associated with the given staff
            var staffTasks = pTaskService.GetTasks(staffId);
            int totalDailyHours = 0;

            // Sum of all task hours for the day for this specific staff
            foreach (var task in staffTasks)
            {
                if (task.Date == taskDate)
                {
                    totalDailyHours += task.Hours;
                }
            }

            return totalDailyHours; // Return the total hours for the day

        }
        #endregion
        #region(Dropdowns)

        // Get the list of managers for the dropdown
        public List<SelectListItem> GetManagerDropdown()
        {
            List<SelectListItem> managerList = new List<SelectListItem>();
            IList<Manager> managers = managerService.GetManagers();
            foreach (Manager manager in managers)
            {
                managerList.Add
                    (
                    new SelectListItem()
                    {
                        Text = manager.FirstName + " " + manager.LastName,
                        Value = manager.StaffId.ToString(),
                        Selected = (manager.FirstName == (managers[0].FirstName) ? true : false)
                    }
                    );
            }
            return managerList;
        }

        // Get the list of managers for the dropdown
        public List<SelectListItem> GetProjectDropdown()
        {
            List<SelectListItem> projectList = new List<SelectListItem>();
            IList<Project> projects = projectService.GetProjects();
            foreach (Project project in projects)
            {
                projectList.Add
                    (
                    new SelectListItem()
                    {
                        Text = project.ProjectName,
                        Value = project.Id.ToString(),
                        Selected = (project.ProjectName == (projects[0].ProjectName) ? true : false)
                    }
                    );
            }
            return projectList;
        }

        // Get the list of projects for the dropdown for specific manager
        public List<SelectListItem> GetProjectDropdown(Manager manager)
        {
            List<SelectListItem> projectList = new List<SelectListItem>();
            foreach (Project project in manager.Projects)
            {
                projectList.Add
                    (
                    new SelectListItem()
                    {
                        Text = project.ProjectName,
                        Value = project.Id.ToString(),
                    }
                    );
            }
            return projectList;
        }

        // Get the list of staff for the dropdown
        public List<SelectListItem> GetStaffDropdown()
        {
            List<SelectListItem> staffList = new List<SelectListItem>();
            IList<Staff> staffs = staffService.GetStaffs();
            foreach (Staff staff in staffs)
            {
                staffList.Add
                    (
                    new SelectListItem()
                    {
                        Text = staff.FirstName,
                        Value = staff.StaffId.ToString(),
                        Selected = (staff.FirstName == (staffs[0].FirstName) ? true : false)
                    }
                    );
            }
            return staffList;
        }

        // Get the list of staff for the dropdown for specific manager
        public List<SelectListItem> GetStaffDropdown(Manager manager)
        {
            List<SelectListItem> staffList = new List<SelectListItem>();
            foreach (Staff staff in manager.Staffs)
            {
                staffList.Add
                    (
                    new SelectListItem()
                    {
                        Text = staff.FirstName + " " + staff.LastName,
                        Value = staff.StaffId.ToString(),
                    }
                    );
            }
            return staffList;
        }
        #endregion

        // Approve or reject a leave request
        public void ApproveReject(Leave leave, string decision, Staff staff, Manager manager)
        {
            //To make controller code light, method was moved to Helper
            //string decision is passed from the controller and button in the view
            if (decision == "Approve")
            {
                LeaveStaff approvedLeave = new LeaveStaff()
                {
                    RequestDate = leave.RequestDate,
                    StartDate = leave.StartDate,
                    EndDate = leave.EndDate,
                    Authorised = true,
                    AuthorisedDate = DateTime.Now,
                    Rejected = false,
                    Sick = leave.Sick
                };
                leaveService.CancelLeave(leave, staff);
                leaveService.AddLeave(approvedLeave, staff.StaffId, manager.StaffId);
                //SendMail(manager.WorkEmail, staff.WorkEmail, "Leave Approved", "Your leave request for " + 
                //  leave.StartDate + "-" + leave.EndDate +"has been approved on "+ approvedLeave.AuthorisedDate + ".");
                //SendMail(manager.WorkEmail, manager.WorkEmail, "Leave Approval Confirmation", "You have approved " + 
                //    staff.FirstName + " " + staff.LastName + "'s leave request for " + leave.StartDate + "-" + leave.EndDate + ".");

            }
            if (decision == "Reject")
            {
                leave.Rejected = true;
                leaveService.UpdateLeave(leave);
                //SendMail(manager.WorkEmail, staff.WorkEmail, "Leave Rejected", "Unfortunately, your leave request for " + 
                //    leave.StartDate + "-" + leave.EndDate + "has been rejected.");
                //SendMail(manager.WorkEmail, manager.WorkEmail, "Leave Rejection Confirmation", "You have rejected " +
                //    staff.FirstName + staff.LastName + "leave request for" + leave.StartDate + leave.EndDate + ".");
            }
        }

        // Get the list of users with roles
        public List<object> GetUsersWithRoles()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                return (from userRole in context.UserRoles  //join performed on two Identity tables: UserRoles and Users
                        join user in context.Users          //using UserRoles table as a common table
                        on userRole.UserId equals user.Id
                        join role in context.Roles
                        on userRole.RoleId equals role.Id
                        select new
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            RoleName = role.Name
                        }).ToList<object>();
            }
        }

        //Method to create a dictionary of events for the calendar
        public async Task<Dictionary<DateTime, string>> Calendar(string userId)
        {
                Staff unknown = staffService.GetStaff(userId);
                Dictionary<DateTime, string> leaveDictionary = new Dictionary<DateTime, string>();
                IList<Leave> leaves = leaveService.GetLeaves(unknown);
                IList<Leave> approvedLeaves = leaves.Where(leave => leave.Authorised == true && leave.Rejected == false).ToList();
                var bankHolidays = await _bankHolidaysService.GetBankHolidays(DateTime.Now.Year);

                if (bankHolidays == null)
                {
                    bankHolidays = new List<DateTime>();
                }
                foreach (var holiday in bankHolidays) // Add bank holidays to the dictionary
                {
                    if (!leaveDictionary.ContainsKey(holiday))
                    {
                        leaveDictionary.Add(holiday, "Bank Holiday");
                    }
                }
                foreach (var leave in approvedLeaves) // Add approved leave to the dictionary
                {
                    if (leave == null) { continue; }
                    DateTime start = leave.StartDate.ToDateTime(new TimeOnly(0, 0));
                    DateTime end = leave.EndDate.ToDateTime(new TimeOnly(23, 59));

                    for (var date = start; date <= end; date = date.AddDays(1))
                    {
                        if (!leaveDictionary.ContainsKey(date)) //If date is not already in the dictionary
                        {
                            string leaveType = leave.Sick ? "Sick Leave" : "Annual Leave";
                            leaveDictionary.Add(date, leaveType);
                        }
                    }
                }
                if (unknown is Manager manager) //if Staff is also a Manager
                {
                    IList<Project> projects = projectService.GetProjects(userId); // Get the projects for the manager
                    foreach (Project project in projects)
                    {
                        if (project == null) { continue; }
                        DateTime deadline = project.Deadline.ToDateTime(new TimeOnly(0, 0));
                        string projectType = project.ProjectName + " Deadline";
                        if (!leaveDictionary.ContainsKey(deadline))
                        {
                            leaveDictionary.Add(deadline, projectType);
                        }
                        else  //If project deadline overlaps with leave or bank holiday, combine the events
                        {
                            string eventCombined = projectType + "; ";
                            leaveDictionary[deadline] = eventCombined + leaveDictionary[deadline];
                        }
                    }
                }
                return leaveDictionary; // Return the dictionary
        }
            
    }
}
