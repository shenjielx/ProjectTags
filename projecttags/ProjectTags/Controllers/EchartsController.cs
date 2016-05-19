using ProjectTags.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectTags.Controllers
{
    public class EchartsController : BaseController
    {
        private EFContext db = new EFContext();

        // GET: Echarts
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult List(string type)
        {
            var success = false;

            var week = (int)DateTime.Today.DayOfWeek;
            week = week > 0 ? week : 7;
            var monday = DateTime.Today.AddDays(1 - week);
            var series = new List<object>();
            var legend = new object();
            if (type == "teams")
            {
                var teams = db.Teams.Where(x => db.Teams.Any(t => t.UserID == UserID && t.ProjectID == x.ProjectID)).Include(x => x.User).ToList();
                series = GetTeamsList(monday, teams);
                legend = teams.Select(x => x.User.UserName).Distinct().ToArray();
            }
            else if (type == "projects")
            {
                var projects = db.Projects.Where(x => db.Teams.Any(t =>t.UserID == UserID && t.ProjectID == x.ID)).ToList();
                series = GetProjectsList(monday, projects);
                legend = projects.Select(x => x.Name).Distinct().ToArray();
            }
            else
            {
                var _legend = new string[] { "分配", "开始", "结束", "完成" };
                series = GetTasksList(monday, _legend);
                legend = _legend;
            }
            success = series.Count > 0;
            return Json(new { success=success, legend = legend, series= series });
        }

        /// <summary>
        /// 获取任务概况
        /// </summary>
        /// <param name="monday"></param>
        /// <param name="legend"></param>
        /// <returns></returns>
        public List<object> GetTasksList(DateTime monday,string[] legend)
        {
            var series = new List<object>();
            var status = db.Status.Where(x => legend.Any(t => t == x.Name)).ToList();

            foreach (var state in status)
            {
                var arr = new int[] { 0, 0, 0, 0, 0, 0, 0 };
                for (int i = 0; i < 7; i++)
                {
                    var start = monday.AddDays(i);
                    var end = start.AddDays(1);
                    var count = db.Tasks.Count(x => db.Processes.Any(t => t.TaskID == x.ID && t.State.Name == state.Name && t.CreateTime >= start && t.CreateTime < end)
                        && db.Processes.Any(t => t.UserID == UserID && t.TaskID == x.ID && t.State.Name == "分配"));
                    arr[i] = count;
                }
                series.Add(new { name = state.Name, value = arr });
            }
            return series;
        }

        /// <summary>
        /// 获取团队概况
        /// </summary>
        /// <param name="monday"></param>
        /// <param name="teams"></param>
        /// <returns></returns>
        public List<object> GetTeamsList(DateTime monday, List<Teams> teams)
        {
            var series = new List<object>();

            foreach (var group in teams.GroupBy(x=>x.UserID))
            {
                var team = group.FirstOrDefault();
                var arr = new int[] { 0, 0, 0, 0, 0, 0, 0 };
                for (int i = 0; i < 7; i++)
                {
                    var start = monday.AddDays(i);
                    var end = start.AddDays(1);
                    var count = db.Tasks.Count(x => db.Processes.Any(t => t.TaskID == x.ID && t.State.Name == "完成" && t.CreateTime >= start && t.CreateTime < end)
                        && db.Processes.Any(t => t.UserID == team.UserID && t.TaskID == x.ID && t.State.Name == "分配"));
                    arr[i] = count;
                }
                series.Add(new { name = team.User.UserName, value = arr });
            }
            return series;
        }

        /// <summary>
        /// 获取项目概况
        /// </summary>
        /// <param name="monday"></param>
        /// <param name="projects"></param>
        /// <returns></returns>
        public List<object> GetProjectsList(DateTime monday, List<Projects> projects)
        {
            var series = new List<object>();

            foreach (var item in projects)
            {
                var arr = new int[] { 0, 0, 0, 0, 0, 0, 0 };
                for (int i = 0; i < 7; i++)
                {
                    var start = monday.AddDays(i);
                    var end = start.AddDays(1);
                    var count = db.Tasks.Count(x => x.ProjectID == item.ID
                    && db.Processes.Any(t => t.TaskID == x.ID && t.CreateTime >= start && t.CreateTime < end));
                    arr[i] = count;
                }
                series.Add(new { name = item.Name, value = arr });
            }
            return series;
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
