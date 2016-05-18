using ProjectTags.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectTags.Controllers
{
    public class WeeksController : BaseController
    {
        private EFContext db = new EFContext();

        // GET: Weeks
        public ActionResult Index()
        {
            var week = (int)DateTime.Today.DayOfWeek;
            week = week > 0 ? week : 7;
            var start = DateTime.Today.AddDays(1 - week);
            var end = DateTime.Today.AddDays(7 - week);
            ViewBag.WeekStart = start;
            ViewBag.WeekEnd = end;
            var tasks = db.Tasks.Where(x => db.Processes.Any(t => t.TaskID == x.ID && t.UserID == UserID && t.State.Name == "分配"));

            var weeks = tasks.Where(x => x.PlanStart >= start && x.PlanStart <= end).ToList();
            var nextStart = start.AddDays(7);
            var nextEnd = end.AddDays(7);
            var nexts = tasks.Where(x => (x.PlanStart >= nextStart && x.PlanStart <= nextStart) || x.PlanEnd >= nextStart).ToList();

            ViewBag.Weeks = weeks;
            ViewBag.Nexts = nexts;

            return View();
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
