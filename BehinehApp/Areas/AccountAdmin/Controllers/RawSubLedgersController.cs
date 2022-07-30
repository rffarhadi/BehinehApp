using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DLLCore.DBContext;
using DLLCore.DBContext.Entities.Accounting.RawAccounts;

namespace BehinehApp.Areas.AccountAdmin.Controllers
{
    [Authorize]
    public class RawSubLedgersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: AccountAdmin/RawSubLedgers
        public async Task<ActionResult> Index()
        {
            try
            {

                var rawSubLedgersTbl = db.RawSubLedgersTbl.Include(r => r.rawLedgers.RawSubCategories.RawCategoriesInSub).OrderBy(a => a.rawLedgers.RawSubCategories.RawCategoriesInSub.RawCategoryID).ThenBy(a => a.rawLedgers.RawSubCategories.SubCategoryID).ThenBy(a => a.rawLedgers.RawLedgerID).ThenBy(a => a.SubLedgerID);
                return View(await rawSubLedgersTbl.ToListAsync());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawSubLedgers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawSubLedger rawSubLedger = await db.RawSubLedgersTbl.Where(a => a.SubLedgerID == id).Include(r => r.rawLedgers.RawSubCategories.RawCategoriesInSub).OrderBy(a => a.rawLedgers.RawSubCategories.RawCategoriesInSub.RawCategoryID).ThenBy(a => a.rawLedgers.RawSubCategories.SubCategoryID).ThenBy(a => a.rawLedgers.RawLedgerID).ThenBy(a => a.SubLedgerID).FirstOrDefaultAsync();
                if (rawSubLedger == null)
                {
                    return HttpNotFound();
                }
                return View(rawSubLedger);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawSubLedgers/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.RawLedgerID = new SelectList(db.RawLedgersTbl, "RawLedgerID", "RawLedgerName");
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // POST: AccountAdmin/RawSubLedgers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RawSubLedger rawSubLedger)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    db.RawSubLedgersTbl.Add(rawSubLedger);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                ViewBag.RawLedgerID = new SelectList(db.RawLedgersTbl, "RawLedgerID", "RawLedgerName", rawSubLedger.RawLedgerID);
                return View(rawSubLedger);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawSubLedgers/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawSubLedger rawSubLedger = await db.RawSubLedgersTbl.Where(a => a.SubLedgerID == id).Include(r => r.rawLedgers.RawSubCategories.RawCategoriesInSub).OrderBy(a => a.rawLedgers.RawSubCategories.RawCategoriesInSub.RawCategoryID).ThenBy(a => a.rawLedgers.RawSubCategories.SubCategoryID).ThenBy(a => a.rawLedgers.RawLedgerID).ThenBy(a => a.SubLedgerID).FirstOrDefaultAsync();
                if (rawSubLedger == null)
                {
                    return HttpNotFound();
                }
                ViewBag.RawLedgerID = new SelectList(db.RawLedgersTbl, "RawLedgerID", "RawLedgerName", rawSubLedger.RawLedgerID);
                return View(rawSubLedger);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // POST: AccountAdmin/RawSubLedgers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RawSubLedger rawSubLedger)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(rawSubLedger).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.RawLedgerID = new SelectList(db.RawLedgersTbl, "RawLedgerID", "RawLedgerName", rawSubLedger.RawLedgerID);
                return View(rawSubLedger);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawSubLedgers/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawSubLedger rawSubLedger = await db.RawSubLedgersTbl.FindAsync(id);
                if (rawSubLedger == null)
                {
                    return HttpNotFound();
                }
                return View(rawSubLedger);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // POST: AccountAdmin/RawSubLedgers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            try
            {
                RawSubLedger rawSubLedger = await db.RawSubLedgersTbl.FindAsync(id);
                db.RawSubLedgersTbl.Remove(rawSubLedger);
                await db.SaveChangesAsync();
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
