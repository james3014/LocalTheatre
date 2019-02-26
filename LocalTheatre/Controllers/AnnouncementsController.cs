using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using LocalTheatre.Models;
using LocalTheatre.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LocalTheatre.Controllers
{

    public class AnnouncementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// GET: Announcements
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(string id)
        {

            return View(db.Announcements.ToList()); 
        }


        /// <summary>
        /// GET: ViewAnnouncements
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ViewAnnouncement(int id)
        {
            DisplayViewModel model = new DisplayViewModel();

            model.Announcements = db.Announcements.Find(id);
            model.Announcements.Comments.OrderByDescending(comment => comment.CommentDate);
            model.AnnouncementId = id;

            return View(model);
        }


        /// <summary>
        /// GET: Announcements/Create
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Staff")]
        public ActionResult Create()
        {
            return View();
        }


        /// <summary>
        /// POST: Announcements/Create
        /// </summary>
        /// <param name="announcements"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AnnouncementId,Title,Announcement,Date,Category,Author,Comments")] Announcements announcements)
        {
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());


            if (ModelState.IsValid)
            {
                Announcements announce = new Announcements
                {
                    Title = announcements.Title,
                    Announcement = announcements.Announcement,
                    Date = DateTime.Now,
                    Category = announcements.Category,
                    Author = user.FirstName + " " + user.Surname 
                };

                db.Announcements.Add(announce);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(announcements);
        }


        /// <summary>
        /// GET: Announcements/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Staff")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Announcements announcements = db.Announcements.Find(id);
            if (announcements == null)
            {
                return HttpNotFound();
            }
            return View(announcements);
        }

        /// <summary>
        /// POST: Announcements/Edit/5
        /// </summary>
        /// <param name="announcements"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AnnouncementId,Title,Announcement,Date,Category,Author")] Announcements announcements)
        {
            if (ModelState.IsValid)
            {
                db.Entry(announcements).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(announcements);
        }


        /// <summary>
        /// GET: Announcements/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Staff")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Announcements announcements = db.Announcements.Find(id);
            if (announcements == null)
            {
                return HttpNotFound();
            }
            return View(announcements);
        }


        /// <summary>
        /// POST: Announcements/Delete/5#
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Announcements announcements = db.Announcements.Find(id);
            db.Announcements.Remove(announcements);
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
