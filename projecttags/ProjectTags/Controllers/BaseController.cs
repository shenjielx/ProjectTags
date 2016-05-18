using Newtonsoft.Json;
using ProjectTags.Models;
using ProjectTags.Unitil;
using ProjectTags.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectTags.Controllers
{
    public class BaseController : Controller
    {
        #region 参数
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserID { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        public LoginBase LoginUser { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 分页页码
        /// </summary>
        public int pageSize = 10;
        #endregion

        #region 序列化、json
        /// <summary>
        /// 序列化设置
        /// </summary>
        protected static readonly JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// 返回JsonResult
        /// </summary>
        /// <param name="data"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        protected JsonResult JsonResult(object data, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            return Json(data, behavior);
        }
        #endregion

        #region 基类
        /// <summary>
        /// 基类
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var response = filterContext.HttpContext.Response;

            //Convert.ToInt64(string.IsNullOrEmpty(cookie) ? "0" : cookie);//
            LoginUser = ToolBox.GetLoginUser;
            var validateToken = "";
            if (LoginUser!=null)
            {
                validateToken = ToolBox.DESEncrypt(LoginUser.UserID.ToString());
            }
            if (LoginUser != null && validateToken == LoginUser.Token)
            {
                ViewBag.token = validateToken;
                Token = LoginUser.Token;
                UserID = LoginUser.UserID;
                UserName = LoginUser.UserName;
                ViewBag.UserName = UserName;
                ViewBag.UserID = UserID;
                ViewBag.Rank = LoginUser.Rank;
            }
            else
            {
                // 未登录
                string rawUrl = request.RawUrl;
                if (rawUrl != "/")
                {
                    response.Redirect("/Login?callback=" + Server.UrlEncode(rawUrl));
                }
                else
                {
                    response.Redirect("/Login");
                }
                filterContext.Result = new EmptyResult();
            }

            //获得当前URL的Controller和Action
            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();
            ViewBag.Url = string.Format("/{0}/{1}", controller, action);
            ViewBag.Controller = controller;
            ViewBag.Action = action;
            base.OnActionExecuting(filterContext);
        }

        #endregion

        #region 获取状态
        public void GetStatus()
        {
            ViewBag.ActiveStatusClass = !string.IsNullOrEmpty(Request["stateID"]) ? Convert.ToInt32(Request["stateID"]) : 0;
            using (var dbTemp = new EFContext())
            {
                var status = dbTemp.Status.ToList();
                var dictCount = new Dictionary<string, int>();
                foreach (var state in status)
                {
                    var count = 0;
                    if (state.ID==1)
                    {
                        count = dbTemp.Tasks.Count(x => x.CreateID == UserID);
                    }
                    else
                    {
                        count = dbTemp.Tasks.Count(x => x.State.Name == state.Name
                        && dbTemp.Processes.Any(t => t.UserID == UserID && t.TaskID == x.ID && t.State.Name == "分配"));
                    }
                    dictCount.Add(state.Name + "#" + state.ID + "#" + state.BgColor, count);
                }
                ViewBag.DictCount = dictCount;
                ViewBag.StatusList = status;
            }
        }
        #endregion

    }
}