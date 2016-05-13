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
        public int PageSize = 15;
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

            string cookie = ToolBox.GetCookie(ToolBox.FormsAuthCookieName);
            //Convert.ToInt64(string.IsNullOrEmpty(cookie) ? "0" : cookie);//
            LoginUser = ToolBox.GetLoginUser;
            if (LoginUser!=null)
            {
                ViewBag.token = ToolBox.DESEncrypt(UserID.ToString());

            }
            var validateToken = ToolBox.DESEncrypt(UserID.ToString());
            if (LoginUser != null && validateToken == LoginUser.Token)
            {
                Token = LoginUser.Token;
                UserID = LoginUser.UserID;
                UserName = LoginUser.UserName;
                ViewBag.UserName = UserName;
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
    }
}