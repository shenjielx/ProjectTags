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
    public class ProcessesController : BaseController
    {
        private EFContext db = new EFContext();

        // GET: Processes
        public ActionResult Index(long? taskID)
        {
            var processes = db.Processes.Where(x => x.TaskID == taskID)
                .Include(p => p.Create).Include(p => p.State).Include(p => p.Task).Include(p => p.Update).Include(p => p.User);
            ViewBag.Status = db.Status;
            return View(processes.ToList());
        }

        // GET: Processes/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Processes processes = db.Processes.Find(id);
            if (processes == null)
            {
                return HttpNotFound();
            }
            return View(processes);
        }

        // GET: Processes/Create
        public ActionResult Create(long? taskID)
        {
            ViewBag.TaskID = new SelectList(db.Tasks.Where(x => !taskID.HasValue || x.ID == taskID), "ID", "Name", taskID);
            var processes = db.Processes.Where(x => x.TaskID == taskID).OrderByDescending(x => x.CreateTime).Include(x=>x.State).Include(x => x.Task).FirstOrDefault();
            var status = processes != null ? db.Status.Where(x => x.Sort == (processes.State.Sort+1)) : db.Status;
            var state = status.FirstOrDefault();
            ViewBag.StateID = new SelectList(status.OrderBy(x=>x.Sort), "ID", "Name", state.ID);
            var users = db.Users.Where(x => db.Teams.Any(t => t.ProjectID == processes.Task.ProjectID && t.UserID == x.ID));
            var task = db.Tasks.Find(taskID);
            ViewBag.UserID = new SelectList(users, "ID", "UserName", state.Name=="提交"&& task !=null? task.CreateID : UserID);
            var model = new Processes();
            model.UserID = UserID;
            return View(model);
        }

        // POST: Processes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TaskID,StateID,UserID,Remark,IsEnable")] Processes processes)
        {
            if (ModelState.IsValid)
            {
                processes.CreateID = UserID;
                processes.CreateTime = DateTime.Now;
                processes.WebClientIP = ToolBox.GetIP();
                db.Processes.Add(processes);
                var result = db.SaveChanges();
                if (result==1)
                {
                    var task = db.Tasks.Find(processes.TaskID);
                    task.StateID = processes.StateID;
                    var state = db.Status.Find(processes.StateID);
                    if (state.Name == "开始")
                    {
                        task.RealStart = DateTime.Now;
                    }
                    else if (state.Name == "结束")
                    {
                        task.RealEnd = DateTime.Now;
                    }
                    db.Entry(task).State = EntityState.Modified;
                    result = db.SaveChanges();
                }
                return RedirectToAction("Details", "Tasks", new { id = processes.TaskID });
            }
            
            ViewBag.StateID = new SelectList(db.Status, "ID", "Name", processes.StateID);
            ViewBag.TaskID = new SelectList(db.Tasks, "ID", "Name", processes.TaskID);
            return View(processes);
        }

        // GET: Processes/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Processes processes = db.Processes.Find(id);
            if (processes == null)
            {
                return HttpNotFound();
            }
            ViewBag.StateID = new SelectList(db.Status, "ID", "Name", processes.StateID);
            ViewBag.TaskID = new SelectList(db.Tasks, "ID", "Name", processes.TaskID);
            return View(processes);
        }

        // POST: Processes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TaskID,StateID,CreateTime,CreateID,IsEnable")] Processes processes)
        {
            if (ModelState.IsValid)
            {
                processes.UpdateID = UserID;
                processes.UpdateTime = DateTime.Now;
                processes.WebClientIP = ToolBox.GetIP();
                db.Entry(processes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StateID = new SelectList(db.Status, "ID", "Name", processes.StateID);
            ViewBag.TaskID = new SelectList(db.Tasks, "ID", "Name", processes.TaskID);
            return View(processes);
        }

        // GET: Processes/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Processes processes = db.Processes.Find(id);
            if (processes == null)
            {
                return HttpNotFound();
            }
            return View(processes);
        }

        // POST: Processes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Processes processes = db.Processes.Find(id);
            db.Processes.Remove(processes);
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
