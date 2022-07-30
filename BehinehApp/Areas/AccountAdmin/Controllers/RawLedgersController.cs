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
    public class RawLedgersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AccountAdmin/RawLedgers
        public ActionResult Index()
        {
            try
            {
                var rawLedgers = db.RawLedgersTbl.Include(r => r.RawSubCategories).Include(a => a.RawSubCategories.RawCategoriesInSub).OrderBy(a => a.SubCategoryID);
                return View(rawLedgers.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawLedgers/Details/5
        public ActionResult Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawLedgers rawLedgers = db.RawLedgersTbl.Where(a => a.RawLedgerID == id).Include(r => r.RawSubCategories).Include(a => a.RawSubCategories.RawCategoriesInSub).FirstOrDefault();
                if (rawLedgers == null)
                {
                    return HttpNotFound();
                }
                return View(rawLedgers);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawLedgers/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.SubCategoryID = new SelectList(db.SubCategoriesTbl, "SubCategoryID", "SubCategoryName");
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        public ActionResult CreateFromExcell()
        {
            return View();
        }

        // POST: AccountAdmin/RawLedgers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RawLedgers rawLedgers)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.RawLedgersTbl.Add(rawLedgers);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.SubCategoryID = new SelectList(db.SubCategoriesTbl, "SubCategoryID", "SubCategoryName", rawLedgers.SubCategoryID);
                return View(rawLedgers);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawLedgers/Edit/5
        public ActionResult Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawLedgers rawLedgers = db.RawLedgersTbl.Find(id);
                if (rawLedgers == null)
                {
                    return HttpNotFound();
                }
                ViewBag.SubCategoryID = new SelectList(db.SubCategoriesTbl, "SubCategoryID", "SubCategoryName", rawLedgers.SubCategoryID);
                return View(rawLedgers);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // POST: AccountAdmin/RawLedgers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RawLedgers rawLedgers)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(rawLedgers).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.SubCategoryID = new SelectList(db.SubCategoriesTbl, "SubCategoryID", "SubCategoryName", rawLedgers.SubCategoryID);
                return View(rawLedgers);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawLedgers/Delete/5
        public ActionResult Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawLedgers rawLedgers = db.RawLedgersTbl.Find(id);
                if (rawLedgers == null)
                {
                    return HttpNotFound();
                }
                return View(rawLedgers);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // POST: AccountAdmin/RawLedgers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                RawLedgers rawLedgers = db.RawLedgersTbl.Find(id);
                db.RawLedgersTbl.Remove(rawLedgers);
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
