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
using DLLCore.DBContext.Entities.Accounting.Entry;
using DLLCore.Utility;
using DLLCore.Repositories;
using DLLCore.ViewModels;
using DLLCore.DBContext.Entities.Accounting.RawAccounts;

namespace BehinehApp.Areas.AccountAdmin.Controllers
{
    [Authorize]
    public class JournalsController : Controller
    {
        JournalEntryRepository EntryRepository;
        private ApplicationDbContext db;
        public JournalsController()
        {
            db = new ApplicationDbContext();
            EntryRepository = new JournalEntryRepository();
        }

        // GET: AccountAdmin/Journals
        public async Task<ActionResult> Index()
        {
            try
            {
                var journalsTbl = await EntryRepository.GetAllIncludeIPsAndRawDetails();
                return View(journalsTbl.OrderBy(a => a.EntryNo).ThenBy(a => a.EntryGregorianDate));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // GET: AccountAdmin/Journals/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var journal = await EntryRepository.GetAllIncludeIPsAndRawDetailsByEntryCode(id);
                if (journal == null)
                {
                    return HttpNotFound();
                }
                return View(journal.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // GET: AccountAdmin/Journals/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.InvestorID = new SelectList(db.InvestorProfiles.Select(a => new { a.InvestorID, InvestorFullName = a.FirstName + " " + a.LastName + " " + a.BourseCode }), "InvestorID", "InvestorFullName");
                ViewBag.DetaileID = new SelectList(db.RawDetailesTbl, "DetaileID", "DetaileName");
                //ViewBag.DetaileName = new SelectList(db.RawDetailesTbl.Where(a => a.SubLedgerID == 1).Include(a => a.rawSubLedger).Select(a => new { a.DetaileID, DetaileName = a.DetaileName + " " + a.DetaileID }), "DetaileID", "DetaileName");

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: AccountAdmin/Journals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection formCollectionData)
        {
            try
            {
                var entryViewModel = EntryRepository.BindFormCollectionForInsert(formCollectionData);
                var Journallist = EntryRepository.BindVmListToJournalListEntry(entryViewModel);
                var saveResult = EntryRepository.InsertEntryList(Journallist);
                if (saveResult == true)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.InvestorID = new SelectList(db.InvestorProfiles.Select(a => new { a.InvestorID, InvestorFullName = a.FirstName + " " + a.LastName + " " + a.BourseCode }), "InvestorID", "InvestorFullName");
                ViewBag.DetaileID = new SelectList(db.RawDetailesTbl, "DetaileID", "DetaileName");

                //ViewBag.DetaileName = new SelectList(db.RawDetailesTbl.Where(a => a.SubLedgerID == 1).Include(a => a.rawSubLedger).Select(a => new { a.DetaileID, DetaileName = a.DetaileName + " " + a.DetaileID }), "DetaileID", "DetaileName");
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // GET: AccountAdmin/Journals/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var entryCode = db.JournalsTbl.Where(a => a.EntryNo == id).Select(a => a.EntryNo).FirstOrDefault();
                var journalList = await db.JournalsTbl.Where(a => a.EntryNo == entryCode).ToListAsync();
                var journalVmList = EntryRepository.BindJournalListToJoVmList(journalList);
                if (journalVmList == null)
                {
                    return HttpNotFound();
                }
                var investorId = journalVmList.Select(a => a.InvestorID).FirstOrDefault();
                var detaileId = journalVmList.Select(a => a.DetaileID).ToList();
                List<RawDetaile> detaileList = new List<RawDetaile>();
                foreach (var item in detaileId)
                {
                    RawDetaile detaile = db.RawDetailesTbl.Where(a => a.DetaileID == item).FirstOrDefault();
                    detaileList.Add(detaile);
                }

                ViewBag.InvestorID = new SelectList(db.InvestorProfiles.Where(a => a.InvestorID == investorId).Select(a => new { a.InvestorID, InvestorFullName = a.FirstName + " " + a.LastName + " " + a.BourseCode }), "InvestorID", "InvestorFullName");
                //ViewBag.DetaileID = new SelectList(detaileList.Select(a => new { a.DetaileID, DetaileName = a.DetaileName + " " + a.DetaileID }), "DetaileID", "DetaileName");
                return View(journalVmList);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: AccountAdmin/Journals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection formCollectionData)
        {
            try
            {
                var entryViewModel = EntryRepository.BindFormCollectionForEdit(formCollectionData);
                var Journallist = EntryRepository.BindVmListToJournalListEntryForEdit(entryViewModel);
                var Result = EntryRepository.EditEntryList(Journallist);
                if (Result == true)
                {
                    return RedirectToAction("Index");
                }

                var journalVmList = EntryRepository.BindJournalListToJoVmList(Journallist);
                if (journalVmList == null)
                {
                    return HttpNotFound();
                }
                var investorId = journalVmList.Select(a => a.InvestorID).FirstOrDefault();
                var detaileId = journalVmList.Select(a => a.DetaileID).ToList();
                List<RawDetaile> detaileList = new List<RawDetaile>();
                foreach (var item in detaileId)
                {
                    RawDetaile detaile = db.RawDetailesTbl.Where(a => a.DetaileID == item).FirstOrDefault();
                    detaileList.Add(detaile);
                }

                ViewBag.InvestorID = new SelectList(db.InvestorProfiles.Where(a => a.InvestorID == investorId).Select(a => new { a.InvestorID, InvestorFullName = a.FirstName + " " + a.LastName + " " + a.BourseCode }), "InvestorID", "InvestorFullName");
                return View(journalVmList);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // GET: AccountAdmin/Journals/Edit/5
        public async Task<ActionResult> EditV(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var entryCode = db.JournalsTbl.Where(a => a.EntryNo == id).Select(a => a.EntryNo).FirstOrDefault();
                var journalList = await db.JournalsTbl.Where(a => a.EntryNo == entryCode).ToListAsync();
                var journalVmList = EntryRepository.BindJournalListToJoVmList(journalList);
                if (journalVmList == null)
                {
                    return HttpNotFound();
                }
                NormalEntryListModelForViewModel vListModel = new NormalEntryListModelForViewModel();
                vListModel.NormalEntryViewListModel = journalVmList;
                return View(vListModel);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: AccountAdmin/Journals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditV(NormalEntryListModelForViewModel vm)
        {
            try
            {
                var Journallist = EntryRepository.BindVmListToJournalListEntryForEdit(vm.NormalEntryViewListModel);
                var Result = EntryRepository.EditEntryList(Journallist);
                if (Result == true)
                {
                    return RedirectToAction("Index");
                }

                var journalVmList = EntryRepository.BindJournalListToJoVmList(Journallist);
                if (journalVmList == null)
                {
                    return HttpNotFound();
                }
                NormalEntryListModelForViewModel vListModel = new NormalEntryListModelForViewModel();
                vListModel.NormalEntryViewListModel = journalVmList;
                return View(vListModel);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // GET: AccountAdmin/Journals/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var journal = await EntryRepository.GetAllIncludeIPsAndRawDetailsByEntryCode(id);
                if (journal == null)
                {
                    return HttpNotFound();
                }
                return View(journal);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: AccountAdmin/Journals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var journal = await EntryRepository.GetAllIncludeIPsAndRawDetailsByEntryCode(id);
                var result = EntryRepository.DeleteEntryList(journal.ToList());
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
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
