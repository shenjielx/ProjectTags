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
    public class UsersController : BaseController
    {
        private EFContext db = new EFContext();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                id = UserID;
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            this.GetStatus();
            return View(users);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserName,Password,Email,Mobile,Name,Gender,Rank,IsEnable")] Users users)
        {
            if (ModelState.IsValid)
            {
                users.Password = ToolBox.DESEncrypt(users.Password);
                users.CreateTime = DateTime.Now;
                users.WebClientIP = ToolBox.GetIP();
                db.Users.Add(users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(users);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserName,Password,Email,Mobile,Name,Gender,Rank,CreateTime,IsEnable")] Users users)
        {
            if (ModelState.IsValid)
            {
                users.UpdateTime = DateTime.Now;
                users.WebClientIP = ToolBox.GetIP();
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(users);
        }

        // GET: Users/Edit/5
        public ActionResult Basic()
        {
            Users users = db.Users.Find(UserID);
            if (users == null)
            {
                return HttpNotFound();
            }
            ViewBag.Gender = new SelectList(ToolDict.UsersGenderDict, "Key", "Value", (int)users.Gender);
            this.GetStatus();
            return View(users);
        }

        // POST: Users/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Basic([Bind(Include = "UserName,Email,Mobile,Name,Gender")] Users users)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(x => x.ID != UserID && x.UserName == users.UserName && !string.IsNullOrEmpty(x.UserName)))
                {
                    ViewBag.ErrorMsg = "昵称已存在！";
                }
                else if (db.Users.Any(x => x.ID != UserID && x.Email == users.Email && !string.IsNullOrEmpty(x.Email)))
                {
                    ViewBag.ErrorMsg = "邮箱已存在！";
                }
                else if (db.Users.Any(x => x.ID != UserID && x.Mobile == users.Mobile && !string.IsNullOrEmpty(x.Mobile)))
                {
                    ViewBag.ErrorMsg = "手机已存在！";
                }
                else
                {
                    var entity = db.Users.Find(UserID);
                    entity.UserName = users.UserName;
                    entity.Email = users.Email;
                    entity.Mobile = users.Mobile;
                    entity.Name = users.Name;
                    entity.Gender = users.Gender;
                    entity.UpdateTime = DateTime.Now;
                    entity.WebClientIP = ToolBox.GetIP();
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details");
                }
            }
            return View(users);
        }
        // GET: Users/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Users users = db.Users.Find(id);
            db.Users.Remove(users);
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
