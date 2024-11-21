using EasyTeams.Data.Models.Domain;
using EasyTeams.Data.Models.Repository;
using EasyTeams.Services.IService;
using EasyTeams.Services.Models;
using EasyTeams.Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EasyTeams.Controllers
{
    //[Authorize(Roles = "Admin, Manager")]
    public class ProjectAdminController : Controller
    {
        Helper helper;
        ProjectService projectService;
        ManagerService managerService;
        EasyTeamsContext context;

        // ProjectAdminController constructor
        public ProjectAdminController()
        {
            helper = new Helper();
            projectService = new ProjectService();
            managerService = new ManagerService();
            context = new EasyTeamsContext();
        }

        // GET: ProjectAdminController
        //Get Projects under manager/admin
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult GetProjects()
        {
            string managerId = HttpContext.Session.GetString("currentUserId");
            return View(projectService.GetProjects(managerId));
                
        }
        [Authorize(Roles = "Manager,Admin")]
        // GET: ProjectAdminController/Details/5
        //Get project details for the project owner
        public ActionResult GetProject(int id)
        {
            try
            {
                Project project = projectService.GetProject(id);
                return View(project);
            }
            catch
            {
                return View();
            }
        }

        // GET: ProjectAdminController/Create
        //Only manager/admin can create a project
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProjectAdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Project collection)
        {
            try
            {
                string currentUserId = HttpContext.Session.GetString("currentUserId");
                projectService.AddProject(collection,currentUserId);
                return RedirectToAction("GetProjectsList", "ProjectAdmin");
            }
            catch
            {
                return View(collection);
            }
        }

        // GET: ProjectAdminController/GetProjectsList
        //Method to get list of projects based on the logged in user
        public ActionResult GetProjectsList()
        {
            string managerId = HttpContext.Session.GetString("currentUserId");
            return View(projectService.GetProjects(managerId));

        }

        [Authorize(Roles = "Manager,Admin")]
        // GET: ProjectAdminController/Edit/5
        //Only manager/admin can edit the project details
        public ActionResult Edit(int id)
        {
            Project project = projectService.GetProject(id);
            return View(project);
        }

        // POST: ProjectAdminController/Edit/5
        // POST: StaffController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Project collection)
        {
            try
            {
                projectService.EditProject(collection, id);
                Project project = projectService.GetProject(id);
                return RedirectToAction("GetProject", "ProjectAdmin", new { id = project.Id });

            }
            catch
            {
                return View();
            }

        }


        // GET: ProjectAdminController/Delete/5
        //Only manager/admin can delete a project
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult Delete(int id)
        {
            Project project = projectService.GetProject(id);
            return View(project);
        }

        // POST: ProjectAdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                Project project = projectService.GetProject(id);
                projectService.DeleteProject(id);
                return RedirectToAction("GetProjectsList", "ProjectAdmin" );
            }
            catch
            {
                return View();
            }
        }

    }
}
