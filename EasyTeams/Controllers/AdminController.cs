using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EasyTeams.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EasyTeams.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
            ApplicationDbContext context;
            private readonly SignInManager<IdentityUser> _signInManager;
            Helper helper;

            // GET: Admin
            public AdminController(SignInManager<IdentityUser> signInManager)
            {
                context = new ApplicationDbContext();
                _signInManager = signInManager;
                helper = new Helper();
            }

            // GetUsers action
            public ActionResult GetUsers()
            {
                var usersWithRoles = helper.GetUsersWithRoles();

                return View(usersWithRoles);
                //return View(context.Users.ToList());
            }

            // GET: AddRole action
            public ActionResult AddRole()
            {
                return View();
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult AddRole(IFormCollection collection)
            {
                IdentityRole role = new IdentityRole();
                role.Name = collection["RoleName"].ToString();
                role.NormalizedName = collection["RoleName"].ToString().ToUpper();
                context.Roles.Add(role);
                context.SaveChanges();
                return RedirectToAction("GetUsers");
            }

        // GET: AddUserToRole action
        public ActionResult AddUserToRole()
        {
            FillInDropDowns();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: AddUserToRole
        public async Task<ActionResult> AddUserToRole(string userName, string roleName)
        {
            IdentityUser user = _signInManager.UserManager.FindByNameAsync(userName).Result;
            await _signInManager.UserManager.AddToRoleAsync(user, roleName);
            FillInDropDowns();
            return RedirectToAction("AddUserToRole");
        }

        // GET: RemoveUserFromRole action
        public ActionResult RemoveUserFromRole()
        {
            FillInDropDowns();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: RemoveUserFromRole
        public async Task<ActionResult> RemoveUserFromRole(string userName, string roleName)
        {
            IdentityUser user = _signInManager.UserManager.FindByNameAsync(userName).Result;
            await _signInManager.UserManager.RemoveFromRoleAsync(user, roleName);
            FillInDropDowns();
            return RedirectToAction("RemoveUserFromRole");
        }
        // Fill in dropdowns for AddUserToRole and RemoveUserFromRole
        void FillInDropDowns()
        {
            var userList = context.Users.OrderBy(u => u.UserName).ToList().Select
                (
                uu => new SelectListItem
                {
                    Value = uu.UserName.ToString(),
                    Text = uu.UserName
                }
                ).ToList();
            ViewData["Users"] = userList;
            //Prepare dropdown for Roles
            var roleList = context.Roles.OrderBy(
                r => r.Name).ToList().Select
                (
                rr => new SelectListItem
                {
                    Value = rr.Name.ToString(),
                    Text = rr.Name
                }
                ).ToList();
            ViewData["Roles"] = roleList;
        }
    }
}
