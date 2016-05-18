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
    public class TeamsController : BaseController
    {
        private EFContext db = new EFContext();

        // GET: Teams
        public ActionResult Index()
        {
            var self = db.Teams.Where(x => x.UserID == UserID);
            var teams = db.Teams.Where(x => x.CreateID== UserID || self.Any(t => t.ProjectID == x.ProjectID)).Include(t => t.Create).Include(t => t.Project).Include(t => t.Update).Include(t => t.User);
            return View(teams.ToList());
        }

        // GET: Teams/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teams teams = db.Teams.Find(id);
            if (teams == null)
            {
                return HttpNotFound();
            }
            return View(teams);
        }

        // GET: Teams/Create
        public ActionResult Create(long? projectID)
        {
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Name", projectID);
            ViewBag.UserID = new SelectList(db.Users.Where(x => db.Teams.Any(t => t.UserID == x.ID && (!projectID.HasValue || t.ProjectID == projectID.Value)) == false), "ID", "UserName");
            return View();
        }

        // POST: Teams/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectID,UserID,IsEnable")] Teams teams)
        {
            if (ModelState.IsValid)
            {
                teams.CreateID = UserID;
                teams.CreateTime = DateTime.Now;
                teams.WebClientIP = ToolBox.GetIP();
                db.Teams.Add(teams);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Name", teams.ProjectID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "UserName", teams.UserID);
            return View(teams);
        }

        // GET: Teams/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teams teams = db.Teams.Find(id);
            if (teams == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Name", teams.ProjectID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "UserName", teams.UserID);
            return View(teams);
        }

        // POST: Teams/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProjectID,UserID,CreateTime,CreateID,IsEnable")] Teams teams)
        {
            if (ModelState.IsValid)
            {
                teams.UpdateID = UserID;
                teams.UpdateTime = DateTime.Now;
                teams.WebClientIP = ToolBox.GetIP();
                db.Entry(teams).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Name", teams.ProjectID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "UserName", teams.UserID);
            return View(teams);
        }

        // GET: Teams/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teams teams = db.Teams.Find(id);
            if (teams == null)
            {
                return HttpNotFound();
            }
            return View(teams);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Teams teams = db.Teams.Find(id);
            db.Teams.Remove(teams);
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
