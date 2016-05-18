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
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginInfo model)
        {
            if (model != null && !string.IsNullOrEmpty(model.UserName) && !string.IsNullOrEmpty(model.Password))
            {
                var db = new EFContext();
                var password = ToolBox.DESEncrypt(model.Password);
                var entity = db.Users.FirstOrDefault(x => (x.UserName == model.UserName || x.Email == model.UserName || x.Mobile == model.UserName) && x.Password == password);
                if (entity!=null)
                {
                    var loginUser = new LoginBase()
                    {
                        Name = entity.Name,
                        UserID = entity.ID,
                        UserName = entity.UserName,
                        Rank = entity.Rank,
                        Token = ToolBox.DESEncrypt(entity.ID.ToString())
                    };
                    var date = model.IsRemember ? DateTime.Today.AddDays(8) : DateTime.Today.AddDays(2);
                    ToolBox.WriteUserInfo(loginUser, date);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
        // GET: Logout
        public ActionResult Logout()
        {
            ToolBox.DeleteCookie(ToolBox.FormsAuthCookieName);
            return RedirectToAction("Login");
        }
    }
}