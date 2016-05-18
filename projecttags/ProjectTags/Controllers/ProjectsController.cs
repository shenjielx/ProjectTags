using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectTags.Models;
using ProjectTags.Unitil;

namespace ProjectTags.Controllers
{
    public class ProjectsController : BaseController
    {
        private EFContext db = new EFContext();

        // GET: Projects
        public ActionResult Index()
        {
            var list = db.Projects.Where(x => db.Teams.Any(t => t.UserID == UserID && t.ProjectID == x.ID)).ToList();
            return View(list);
        }

        // GET: Projects/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            var teams = db.Teams.Where(x => x.UserID == UserID && x.ProjectID == id).Include(x => x.User);
            ViewBag.Processes = db.Processes.Where(x => teams.Any(t => t.ProjectID == x.Task.ProjectID)).OrderByDescending(x => x.CreateTime).Take(10).Include(t => t.State).Include(t => t.Create).Include(t => t.Task).ToList();
            return View(projects);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Desc,IsEnable")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                projects.CreateID = UserID;
                projects.CreateTime = DateTime.Now;
                projects.WebClientIP = ToolBox.GetIP();
                db.Projects.Add(projects);
                var result = db.SaveChanges();
                if (result==1)
                {
                    var team = new Teams();
                    team.CreateID = UserID;
                    team.CreateTime = DateTime.Now;
                    team.ProjectID = projects.ID;
                    team.UserID = UserID;
                    team.WebClientIP= ToolBox.GetIP();
                    db.Teams.Add(team);
                    result = db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(projects);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // POST: Projects/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Desc,CreateTime,CreateID,IsEnable")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                projects.UpdateID = UserID;
                projects.UpdateTime = DateTime.Now;
                projects.WebClientIP = ToolBox.GetIP();
                db.Entry(projects).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = projects.ID });
            }
            return View(projects);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Projects projects = db.Projects.Find(id);
            db.Projects.Remove(projects);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
