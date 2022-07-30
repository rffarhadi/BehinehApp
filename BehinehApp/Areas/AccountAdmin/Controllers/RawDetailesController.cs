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
    public class RawDetailesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AccountAdmin/RawDetailes
        public async Task<ActionResult> Index()
        {
            try
            {
                var rawDetailesTbl = db.RawDetailesTbl.Include(r => r.rawSubLedger).OrderBy(a => a.rawSubLedger.SubLedgerID).ThenBy(a => a.DetaileCode);
                return View(await rawDetailesTbl.ToListAsync());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawDetailes/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawDetaile rawDetaile = await db.RawDetailesTbl.Include(r => r.rawSubLedger).Where(a => a.DetaileID == id).FirstOrDefaultAsync();
                if (rawDetaile == null)
                {
                    return HttpNotFound();
                }
                return View(rawDetaile);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawDetailes/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.SubLedgerID = new SelectList(db.RawSubLedgersTbl, "SubLedgerID", "SubLedgerName");
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // POST: AccountAdmin/RawDetailes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RawDetaile rawDetaile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    long subledgerId = rawDetaile.SubLedgerID;
                    long subledgerCode = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == subledgerId).Select(a => a.SubLedgerCode).FirstOrDefault();
                    var rawDetaileList = db.RawDetailesTbl.Where(a => a.SubLedgerID == subledgerId).ToList();
                    if (rawDetaileList.Count == 0)
                    {
                        long newDetaileCode = Convert.ToInt64(subledgerCode + "1");
                        rawDetaile.DetaileCode = newDetaileCode;
                        db.RawDetailesTbl.Add(rawDetaile);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    if (rawDetaileList.Count > 0)
                    {
                        var lastDetaileCode = db.RawDetailesTbl.Where(a => a.SubLedgerID == subledgerId).Select(a => a.DetaileCode).Max();
                        rawDetaile.DetaileCode = lastDetaileCode + 1;
                        db.RawDetailesTbl.Add(rawDetaile);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                }

                ViewBag.SubLedgerID = new SelectList(db.RawSubLedgersTbl, "SubLedgerID", "SubLedgerName", rawDetaile.SubLedgerID);
                return View(rawDetaile);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawDetailes/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawDetaile rawDetaile = await db.RawDetailesTbl.FindAsync(id);
                if (rawDetaile == null)
                {
                    return HttpNotFound();
                }
                ViewBag.SubLedgerID = new SelectList(db.RawSubLedgersTbl, "SubLedgerID", "SubLedgerName", rawDetaile.SubLedgerID);
                return View(rawDetaile);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // POST: AccountAdmin/RawDetailes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RawDetaile rawDetaile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    long subledgerId = rawDetaile.SubLedgerID;
                    long subledgerCode = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == subledgerId).Select(a => a.SubLedgerCode).FirstOrDefault();
                    var rawDetaileList = db.RawDetailesTbl.Where(a => a.SubLedgerID == subledgerId).ToList();
                    if (rawDetaileList.Count == 0)
                    {
                        long newDetaileCode = Convert.ToInt64(subledgerCode + "1");
                        rawDetaile.DetaileCode = newDetaileCode;
                        var local = db.Set<RawDetaile>().Local.FirstOrDefault(f => f.DetaileID == rawDetaile.DetaileID);
                        if (local != null)
                        {
                            db.Entry(local).State = EntityState.Detached;
                        }
                        db.Entry(rawDetaile).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    if (rawDetaileList.Count > 0)
                    {
                        var lastDetaileCode = db.RawDetailesTbl.Where(a => a.SubLedgerID == subledgerId).Select(a => a.DetaileCode).Max();
                        //برای اطمینان از اینکه کد قبل بی دلیل افزایش نیابد
                        if (lastDetaileCode == rawDetaile.DetaileCode)
                        {
                            var local = db.Set<RawDetaile>().Local.FirstOrDefault(f => f.DetaileID == rawDetaile.DetaileID);
                            if (local != null)
                            {
                                db.Entry(local).State = EntityState.Detached;
                            }
                            db.Entry(rawDetaile).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            rawDetaile.DetaileCode = lastDetaileCode + 1;
                            var local = db.Set<RawDetaile>().Local.FirstOrDefault(f => f.DetaileID == rawDetaile.DetaileID);
                            if (local != null)
                            {
                                db.Entry(local).State = EntityState.Detached;
                            }
                            db.Entry(rawDetaile).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            return RedirectToAction("Index");
                        }
                    }
                }
                ViewBag.SubLedgerID = new SelectList(db.RawSubLedgersTbl, "SubLedgerID", "SubLedgerName", rawDetaile.SubLedgerID);
                return View(rawDetaile);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // GET: AccountAdmin/RawDetailes/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RawDetaile rawDetaile = await db.RawDetailesTbl.Include(r => r.rawSubLedger).Where(a => a.DetaileID == id).FirstOrDefaultAsync(); ;
                if (rawDetaile == null)
                {
                    return HttpNotFound();
                }
                return View(rawDetaile);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        // POST: AccountAdmin/RawDetailes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                RawDetaile rawDetaile = await db.RawDetailesTbl.FindAsync(id);
                db.RawDetailesTbl.Remove(rawDetaile);
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
