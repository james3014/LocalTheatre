using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LocalTheatre.Models;

namespace LocalTheatre.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// GET: Comments
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Staff")]
        public ActionResult Index()
        {
            return View(db.Comments.ToList());
        }


        /// <summary>
        /// GET: Comments/Details/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Staff")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comments comments = db.Comments.Find(id);

            if (comments == null)
            {
                return HttpNotFound();
            }
            return View(comments);
        }


        /// <summary>
        /// GET: Comments/Create
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Staff, User")]
        public ActionResult Create()
        {
            return View();
        }


        /// <summary>
        /// POST: Comments/Create
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Staff, User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CommentId,CommentBody,CommentDate,CommentAuthor,AnnouncementId")] Comments comments)
        {
            if (ModelState.IsValid)
            { 
                db.Comments.Add(comments);
                db.SaveChanges();
                return RedirectToAction("Index", "Announcements", null);
            }

            return View(comments);
        }


        /// <summary>
        /// GET: Comments/Edit/5
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
            Comments comments = db.Comments.Find(id);
            if (comments == null)
            {
                return HttpNotFound();
            }
            return View(comments);
        }


        /// <summary>
        /// POST: Comments/Edit/5
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommentId,CommentBody,CommentDate,CommentAuthor,AnnouncementId")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(comments);
        }


        /// <summary>
        /// GET: Comments/Delete/5
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
            Comments comments = db.Comments.Find(id);
            if (comments == null)
            {
                return HttpNotFound();
            }
            return View(comments);
        }


        /// <summary>
        /// POST: Comments/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comments comments = db.Comments.Find(id);
            db.Comments.Remove(comments);
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
