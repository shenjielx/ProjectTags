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
using ProjectTags.ViewModels;

namespace ProjectTags.Controllers
{
    public class TasksController : BaseController
    {
        private EFContext db = new EFContext();

        [HttpPost]
        public ActionResult List(int? page,long? projectID,long? stateID,bool isMe = false)
        {
            int pageIndex = page ?? 1;//当前页
            var models = new HomeModels();
            var teams = db.Teams.Where(x => x.UserID == UserID).Include(x => x.User);

            var queryable = db.Tasks.Where(x => teams.Any(t => t.UserID == UserID && t.ProjectID == x.ProjectID));

            if (projectID.HasValue)
            {
                queryable = queryable.Where(x => x.ProjectID == projectID.Value);
            }
            if (isMe)
            {
                if (stateID.HasValue)
                {
                    if (stateID.Value==1)
                    {
                        queryable = queryable.Where(x => x.CreateID == UserID);
                    }
                    else
                    {
                        queryable = queryable.Where(x => x.StateID == stateID.Value
                        && db.Processes.Any(t => t.UserID == UserID && t.TaskID == x.ID && t.State.Name == "分配"));
                    }
                }
                queryable = queryable.Where(x => db.Processes.Any(t => t.UserID == UserID && t.TaskID == x.ID));
            }
            models.TotalCount = queryable.Count();
            models.Tasks = queryable
                .OrderByDescending(x => x.CreateTime)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .Include(t => t.Project).Include(t => t.Create).Include(t=>t.State);
            models.PageIndex = pageIndex;
            models.PageSize = pageSize;
            return View(models);
        }
        // GET: Tasks
        public ActionResult Index()
        {
            var listProjects = new List<object>();
            listProjects.Add(new { ID = "", Name = "全部" });
            listProjects.AddRange(db.Projects.OrderBy(x => x.CreateTime));
            ViewBag.ProjectID = new SelectList(listProjects, "ID", "Name");
            
            var models = new HomeModels();
            var teams = db.Teams.Where(x => x.UserID == UserID).Include(x => x.User);
            models.Processes = db.Processes.Where(x => teams.Any(t => t.ProjectID == x.Task.ProjectID))
                .OrderByDescending(x => x.CreateTime).Take(10)
                .Include(t => t.State).Include(t => t.Create).Include(t => t.Task);
            this.GetStatus();
            return View(models);
        }

        // GET: Tasks/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tasks tasks = db.Tasks.Find(id);
            if (tasks == null)
            {
                return HttpNotFound();
            }
            return View(tasks);
        }

        // GET: Tasks/Create
        public ActionResult Create(long? projectID)
        {
            var model = new Tasks();
            model.PlanStart = DateTime.Today;
            model.PlanEnd = DateTime.Today;
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Name", projectID);
            ViewBag.Type = new SelectList(ToolDict.TasksTypeDict, "Key", "Value");
            return View(model);
        }

        // POST: Tasks/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "ProjectID,Type,PlanStart,PlanEnd,Name,Desc,IsEnable")] Tasks tasks)
        {
            if (ModelState.IsValid)
            {
                tasks.CreateID = UserID;
                tasks.CreateTime = DateTime.Now;
                tasks.WebClientIP = ToolBox.GetIP();
                tasks.StateID = 1;
                db.Tasks.Add(tasks);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    var status = db.Status.Where(x => x.Group == 1).OrderBy(x => x.Sort).FirstOrDefault();
                    if (status!=null)
                    {
                        var processes = new Processes();
                        processes.UserID = UserID;
                        processes.CreateID = UserID;
                        processes.CreateTime = DateTime.Now;
                        processes.IsEnable = true;
                        processes.StateID = 1;
                        processes.TaskID = tasks.ID;
                        processes.WebClientIP = ToolBox.GetIP();
                        db.Processes.Add(processes);
                        result = db.SaveChanges();
                    }
                }
                return RedirectToAction("Index", new { stateID = 1 });
            }

            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Name", tasks.ProjectID);
            return View(tasks);
        }

        // GET: Tasks/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tasks tasks = db.Tasks.Find(id);
            if (tasks == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Name", tasks.ProjectID);
            ViewBag.Type = new SelectList(ToolDict.TasksTypeDict, "Key", "Value",(int)tasks.Type);
            return View(tasks);
        }

        // POST: Tasks/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "ID,ProjectID,Type,PlanStart,PlanEnd,Name,Desc,IsEnable")] Tasks tasks)
        {
            if (ModelState.IsValid)
            {
                var entity = db.Tasks.Find(tasks.ID);
                entity.ProjectID = tasks.ProjectID;
                entity.Type = tasks.Type;
                entity.PlanStart = tasks.PlanStart;
                entity.PlanEnd = tasks.PlanEnd;
                entity.Name = tasks.Name;
                entity.Desc = tasks.Desc;
                entity.IsEnable = tasks.IsEnable;
                entity.UpdateID = UserID;
                entity.UpdateTime = DateTime.Now;
                entity.WebClientIP = ToolBox.GetIP();
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = tasks.ID });
            }
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Name", tasks.ProjectID);
            return View(tasks);
        }

        // GET: Tasks/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tasks tasks = db.Tasks.Find(id);
            if (tasks == null)
            {
                return HttpNotFound();
            }
            return View(tasks);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Tasks tasks = db.Tasks.Find(id);
            db.Tasks.Remove(tasks);
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
