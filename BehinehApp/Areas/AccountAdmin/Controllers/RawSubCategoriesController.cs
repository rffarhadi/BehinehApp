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
    public class RawSubCategoriesController : Controller
    {
        private ApplicationDbContext db;
        public RawSubCategoriesController()
        {
            db = new ApplicationDbContext();
        }

        // GET: AccountAdmin/RawSubCategories
        public ActionResult Index()
        {
            try
            {
                var accountingSubCategories = db.SubCategoriesTbl.Include(r => r.RawCategoriesInSub).OrderBy(a => a.RawCategoryID).ToList();
                return View(accountingSubCategories.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawSubCategories/Details/5
        public ActionResult Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawSubCategories rawSubCategories = db.SubCategoriesTbl.Include(r => r.RawCategoriesInSub).Where(a => a.SubCategoryID == id).FirstOrDefault();
                if (rawSubCategories == null)
                {
                    return HttpNotFound();
                }
                return View(rawSubCategories);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawSubCategories/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.RawCategoryID = new SelectList(db.RawCategoriesTbl, "RawCategoryID", "RawCategoryName");
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // POST: AccountAdmin/RawSubCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RawSubCategories rawSubCategories)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.SubCategoriesTbl.Add(rawSubCategories);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.RawCategoryID = new SelectList(db.RawCategoriesTbl, "RawCategoryID", "RawCategoryName", rawSubCategories.RawCategoryID);
                return View(rawSubCategories);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawSubCategories/Edit/5
        public ActionResult Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawSubCategories rawSubCategories = db.SubCategoriesTbl.Find(id);
                if (rawSubCategories == null)
                {
                    return HttpNotFound();
                }
                ViewBag.RawCategoryID = new SelectList(db.RawCategoriesTbl, "RawCategoryID", "RawCategoryName", rawSubCategories.RawCategoryID);
                return View(rawSubCategories);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // POST: AccountAdmin/RawSubCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RawSubCategories rawSubCategories)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(rawSubCategories).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.RawCategoryID = new SelectList(db.RawCategoriesTbl, "RawCategoryID", "RawCategoryName", rawSubCategories.RawCategoryID);
                return View(rawSubCategories);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawSubCategories/Delete/5
        public ActionResult Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawSubCategories rawSubCategories = db.SubCategoriesTbl.Find(id);
                if (rawSubCategories == null)
                {
                    return HttpNotFound();
                }
                return View(rawSubCategories);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // POST: AccountAdmin/RawSubCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                RawSubCategories rawSubCategories = db.SubCategoriesTbl.Find(id);
                db.SubCategoriesTbl.Remove(rawSubCategories);
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
