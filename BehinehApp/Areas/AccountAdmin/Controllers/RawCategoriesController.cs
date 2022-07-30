using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DLLCore.DBContext;
using DLLCore.DBContext.Entities.Accounting.RawAccounts;

namespace BehinehApp.Areas.AccountAdmin.Controllers
{
    [Authorize]
    public class RawCategoriesController : Controller
    {
        private ApplicationDbContext db;
        public RawCategoriesController()
        {
            db = new ApplicationDbContext();
        }

        // GET: AccountAdmin/RawCategories
        public ActionResult Index()
        {
            try
            {
                return View(db.RawCategoriesTbl.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawCategories/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawCategories rawCategories = db.RawCategoriesTbl.Find(id);
                if (rawCategories == null)
                {
                    return HttpNotFound();
                }
                return View(rawCategories);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountAdmin/RawCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RawCategoryID,RawCategoryCode,RawCategoryName")] RawCategories rawCategories)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.RawCategoriesTbl.Add(rawCategories);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(rawCategories);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawCategories rawCategories = db.RawCategoriesTbl.Find(id);
                if (rawCategories == null)
                {
                    return HttpNotFound();
                }
                return View(rawCategories);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // POST: AccountAdmin/RawCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RawCategoryID,RawCategoryCode,RawCategoryName")] RawCategories rawCategories)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(rawCategories).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(rawCategories);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawCategories rawCategories = db.RawCategoriesTbl.Find(id);
                if (rawCategories == null)
                {
                    return HttpNotFound();
                }
                return View(rawCategories);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // POST: AccountAdmin/RawCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                RawCategories rawCategories = db.RawCategoriesTbl.Find(id);
                db.RawCategoriesTbl.Remove(rawCategories);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
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
