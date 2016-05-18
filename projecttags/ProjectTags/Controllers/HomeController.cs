using ProjectTags.Models;
using ProjectTags.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectTags.Controllers
{
    public class HomeController : BaseController
    {
        private EFContext db = new EFContext();

        // GET: Home
        public ActionResult Index()
        {
            var models = new HomeModels();
            var teams = db.Teams.Where(x => x.UserID == UserID).Include(x => x.User);
            var processes = db.Processes;
            ViewBag.Processes = processes.Where(x => teams.Any(t=>t.ProjectID==x.Task.ProjectID))
                .OrderByDescending(x => x.CreateTime).Take(10)
                .Include(t => t.State).Include(t=>t.Create).Include(t=>t.Task).Include(t => t.User)
                .ToList();
            var status = db.Status.Where(x => (x.Name == "分配") || (x.Name == "开始") || (x.Name == "结束") || (x.Name == "完成")).ToList();
            models.DictCount = new Dictionary<string, int>();
            var week = (int)DateTime.Today.DayOfWeek;
            week = week > 0 ? week : 7;
            var start = DateTime.Today.AddDays(1 - week);
            var end = DateTime.Today.AddDays(7 - week);
            foreach (var state in status)
            {
                var count = 0;
                if (processes != null)
                {
                    count = db.Tasks.Count(x => x.State.Name == state.Name
                    && (x.PlanStart >= start && x.PlanStart <= end)
                    && processes.Any(t => t.UserID == UserID && t.TaskID == x.ID && t.State.Name == "分配"));
                }
                models.DictCount.Add(state.Name + "#" + state.ID + "#" + state.BgColor, count);
            }
            ViewBag.WeekStart = start;
            ViewBag.WeekEnd = end;

            return View(models);
        }
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}