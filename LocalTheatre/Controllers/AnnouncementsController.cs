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

        // GET: Announcements
        public ActionResult Index(string searchAnnouncements)
        {
            ViewBag.SearchKey = searchAnnouncements;

            return View(db.Announcements.ToList());
        }

        // GET: ViewAnnouncements
        public ActionResult ViewAnnouncement(int id)
        {
            DisplayViewModel model = new DisplayViewModel();

            model.Announcements = db.Announcements.Find(id);
            model.Announcements.Comments.OrderByDescending(comment => comment.CommentDate);
            model.AnnouncementId = id;

            return View(model);
        }

        // GET: Announcements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Announcements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Announcements/Edit/5
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

        // POST: Announcements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Announcements/Delete/5
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

        // POST: Announcements/Delete/5
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
