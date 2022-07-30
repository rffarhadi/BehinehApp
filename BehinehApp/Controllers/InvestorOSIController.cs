using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DLLCore.DBContext;
using DLLCore.DBContext.Entities;
using DLLCore.Repositories;
using DLLCore.ViewModels;

namespace BehinehApp.Controllers
{
    [Authorize]
    public class InvestorOSIController : Controller
    {
        OsiRepository db;
        InvestorInfrencesConclutionRepository dbIc;

        public InvestorOSIController()
        {
            db = new OsiRepository();
            dbIc = new InvestorInfrencesConclutionRepository();
        }

        // GET: InvestorOSI
        public ActionResult IndexOS()
        {
            try
            {
                ViewBag.Smessage = TempData["Smessage"];
                ViewBag.Emessage = TempData["Emessage"];
                var investorOSIProfiles = db.GetAll();
                return View(investorOSIProfiles.OrderBy(a => a.InvestorID).ToList());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // GET: InvestorOSI/Create
        public ActionResult CreateOS()
        {
            try
            {
                ViewBag.Emessage = TempData["Emessage"];
                ViewBag.Smessage = TempData["Smessage"];
                var investorProfiles = db.GetInvestorProfiles();
                ViewBag.InvestorID = new SelectList(investorProfiles, "InvestorId", "InvestorName");
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: InvestorOSI/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOS([Bind(Include = "InvestorID,Objective,Subjective")] InvestorOSIProfileViewModel investorOSIProfile)
        {
            try
            {
                var investorProfiles = db.GetInvestorProfiles();
                var investor = investorProfiles.Where(a => a.InvestorID == investorOSIProfile.InvestorID).SingleOrDefault();
                if (ModelState.IsValid)
                {
                    var result = db.InsertOsiSingleRow(investorOSIProfile);
                    if (result == true)
                    {
                        TempData["Smessage"] = $"اطلاعات مربوط به محدودیت‌ها و اهداف سرمایه‌گذار با نام {investor.FirstName}و نام  خانوادگی {investor.LastName} با موفقیت ذخیره شد.";
                        TempData.Keep();
                        return RedirectToAction("IndexOS");
                    }
                    else
                    {
                        TempData["Emessage"] = $"اطلاعات مربوط به محدودیت‌ها و اهداف سرمایه‌گذار با نام {investor.FirstName}  و نام خانوادگی {investor.LastName} ذخیره نشد.";
                        TempData.Keep();
                        return View(investorOSIProfile);
                    }
                }
                TempData["Emessage"] = $"اطلاعات مربوط به محدودیت‌ها و اهداف سرمایه‌گذار با نام {investor.FirstName} و نام خانوادگی {investor.LastName} ذخیره نشد.";
                TempData.Keep();
                ViewBag.InvestorID = new SelectList(investorProfiles, "InvestorID", "FirstName", investorOSIProfile.InvestorID);
                return View(investorOSIProfile);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // GET: InvestorOSI/Details/5
        public ActionResult DetailsOS(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                InvestorOSIProfileViewModel investorOSIProfile = db.FindById(id);
                if (investorOSIProfile == null)
                {
                    return HttpNotFound();
                }
                return View(investorOSIProfile);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }



        // GET: InvestorOSI/Delete/5
        public ActionResult DeleteOS(int? id)
        {
            try
            {
                ViewBag.Emessage = TempData["Emessage"];
                ViewBag.Smessage = TempData["Smessage"];
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                InvestorOSIProfileViewModel investorOSIProfile = db.FindById(id);
                if (investorOSIProfile == null)
                {
                    return HttpNotFound();
                }
                return View(investorOSIProfile);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: InvestorOSI/Delete/5
        [HttpPost, ActionName("DeleteOS")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                InvestorOSIProfileViewModel investorOSIProfile = db.FindById(id);
                var investor = db.GetInvestorProfiles().Where(a => a.InvestorID == investorOSIProfile.InvestorID).SingleOrDefault();
                var result = db.DeleteOSEntity(investorOSIProfile);
                if (result == true)
                {
                    TempData["Smessage"] = $"اطلاعات مربوط به اهداف و محدودیت‌های سرمایه‌گذار به نام {investor.FirstName} و نام  خانوادگی {investor.LastName} با موفقیت حذف شد";
                    TempData.Keep();
                    return RedirectToAction("IndexOS");

                }
                else
                {
                    TempData["Emessage"] = $"حذف هدف و محدودیت سرمایه‌گذار به نام {investor.FirstName}  و نام خانوادگی {investor.LastName} انجام نشد";
                    TempData.Keep();
                    return View("DeleteOS", investorOSIProfile);
                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }
        // GET: InvestorOSI/Edit/5
        public ActionResult EditOS(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                InvestorOSIProfileViewModel investorOSIProfile = db.FindById(id);

                if (investorOSIProfile == null)
                {
                    return HttpNotFound();
                }
                var investorProfiles = db.GetInvestorProfiles();
                ViewBag.InvestorID = new SelectList(investorProfiles, "InvestorID", "FirstName", investorOSIProfile.InvestorID);
                return View(investorOSIProfile);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: InvestorOSI/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOS([Bind(Include = "OsiID,InvestorID,Objective,Subjective,")] InvestorOSIProfileViewModel investorOSIProfile)
        {
            try
            {
                var investorProfiles = db.GetInvestorProfiles();
                var investor = investorProfiles.Where(a => a.InvestorID == investorOSIProfile.InvestorID).SingleOrDefault();
                if (ModelState.IsValid)
                {
                    var result = db.EditOSEntity(investorOSIProfile);
                    if (result == true)
                    {
                        TempData["Smessage"] = $"اطلاعات مربوط به اهداف و محدودیت‌های سرمایه‌گذار به نام {investor.FirstName}  و نام خانوادگی {investor.LastName} با موفقیت ویرایش شد";
                        TempData.Keep();
                        return RedirectToAction("IndexOS");
                    }
                    else
                    {
                        TempData["Emessage"] = $"اطلاعات مربوط به اهداف و محدودیت‌های سرمایه‌گذار به نام {investor.FirstName} و نام  خانوادگی {investor.LastName} ویرایش نشد";
                        TempData.Keep();
                        ViewBag.InvestorID = new SelectList(investorProfiles, "InvestorID", "FirstName", investorOSIProfile.InvestorID);
                        return View(investorOSIProfile);
                    }
                }
                TempData["Emessage"] = $"اطلاعات مربوط به اهداف و محدودیت‌های سرمایه‌گذار به نام {investor.FirstName} و نام خانوادگی {investor.LastName} ویرایش نشد";
                TempData.Keep();
                ViewBag.InvestorID = new SelectList(investorProfiles, "InvestorID", "FirstName", investorOSIProfile.InvestorID);
                return View(investorOSIProfile);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }
        // GET: InvestorOSI
        public ActionResult IndexIc()
        {
            try
            {
                ViewBag.Smessage = TempData["Smessage"];
                ViewBag.Emessage = TempData["Emessage"];
                var investorOSIProfiles = dbIc.GetAll();
                return View(investorOSIProfiles.OrderBy(a => a.InvestorID).ToList());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // GET: InvestorOSI/Create
        public ActionResult CreateIc()
        {
            try
            {
                ViewBag.Smessage = TempData["Smessage"];
                ViewBag.Emessage = TempData["Emessage"];
                var investorProfiles = dbIc.GetInvestorProfiles();
                ViewBag.InvestorID = new SelectList(investorProfiles, "InvestorID", "FirstName");
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: InvestorOSI/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateIc([Bind(Include = "InvestorID,Inference,FinalConclution")] InvestorInfrencesConclutionViewModel investorIcVM)
        {
            var investorProfiles = dbIc.GetInvestorProfiles();
            var investor = investorProfiles.Where(a => a.InvestorID == investorIcVM.InvestorID).SingleOrDefault();
            try
            {
                if (ModelState.IsValid)
                {
                    var result = dbIc.InsertIcSingleRow(investorIcVM);
                    if (result == true)
                    {
                        TempData["Smessage"] = $"اطلاعات مربوط به استنباط‌ها و نتیجه‌گیری‌های سرمایه‌گذار با نام {investor.FirstName}  و نام خانوادگی {investor.LastName} با موفقیت ذخیره شد.";
                        TempData.Keep();
                        return RedirectToAction("IndexIc");
                    }
                    else
                    {
                        TempData["Emessage"] = $"اطلاعات مربوط به استنباط‌ها و نتیجه‌گیری‌های سرمایه‌گذار با نام {investor.FirstName}  و نام خانوادگی {investor.LastName}  ذخیره نشد.";
                        TempData.Keep();
                        return View(investorIcVM);
                    }
                }
                TempData["Emessage"] = $"اطلاعات مربوط به استنباط‌ها و نتیجه‌گیری‌های سرمایه‌گذار با نام {investor.FirstName}  و نام خانوادگی {investor.LastName}  ذخیره نشد.";
                TempData.Keep();
                ViewBag.InvestorID = new SelectList(investorProfiles, "InvestorID", "FirstName", investorIcVM.InvestorID);
                return View(investorIcVM);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // GET: InvestorOSI/Details/5
        public ActionResult DetailsIc(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                InvestorInfrencesConclutionViewModel investorIcProfile = dbIc.FindById(id);
                if (investorIcProfile == null)
                {
                    return HttpNotFound();
                }
                return View(investorIcProfile);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }



        // GET: InvestorOSI/Delete/5
        public ActionResult DeleteIc(int? id)
        {
            try
            {
                ViewBag.Smessage = TempData["Smessage"];
                ViewBag.Emessage = TempData["Emessage"];
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                InvestorInfrencesConclutionViewModel investorIcProfile = dbIc.FindById(id);
                if (investorIcProfile == null)
                {
                    return HttpNotFound();
                }
                return View(investorIcProfile);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: InvestorOSI/Delete/5
        [HttpPost, ActionName("DeleteIc")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedIc(int id)
        {
            try
            {
                InvestorInfrencesConclutionViewModel investorIcProfile = dbIc.FindById(id);
                var investor = db.GetInvestorProfiles().Where(a => a.InvestorID == investorIcProfile.InvestorID).SingleOrDefault();
                var result = dbIc.DeleteIcEntity(investorIcProfile);
                if (result==true)
                {
                    TempData["Smessage"] = $"اطلاعات مربوط به استنباط‌ها و نتیجه‌گیری‌های سرمایه‌گذار با نام {investor.FirstName} و  نام خانوادگی {investor.LastName} با موفقیت حذف شد.";
                    TempData.Keep();
                    return RedirectToAction("IndexIc");
                }
                else
                {
                    TempData["Emessage"] = $"اطلاعات مربوط به استنباط‌ها و نتیجه‌گیری‌های سرمایه‌گذار با نام {investor.FirstName}  و نام خانوادگی {investor.LastName} حذف نشد.";
                    TempData.Keep();
                    return View("DeleteIc", investorIcProfile);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }
        // GET: InvestorOSI/Edit/5
        public ActionResult EditIc(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                InvestorInfrencesConclutionViewModel investorIcProfile = dbIc.FindById(id);

                if (investorIcProfile == null)
                {
                    return HttpNotFound();
                }
                var investorProfiles = dbIc.GetInvestorProfiles();
                ViewBag.InvestorID = new SelectList(investorProfiles, "InvestorID", "FirstName", investorIcProfile.InvestorID);
                return View(investorIcProfile);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: InvestorOSI/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditIc([Bind(Include = "InfrencesConclutionID,InvestorID,Inference,FinalConclution")] InvestorInfrencesConclutionViewModel investorIcProfile)
        {
            try
            {
                var investorProfiles = db.GetInvestorProfiles();
                var investor = investorProfiles.Where(a => a.InvestorID == investorIcProfile.InvestorID).SingleOrDefault();
                if (ModelState.IsValid)
                {
                   var result= dbIc.EditIcEntity(investorIcProfile);
                    if (result==true)
                    {
                        TempData["Smessage"] = $"اطلاعات مربوط به استنباط‌ها و نتیجه‌گیری‌های سرمایه‌گذار با نام {investor.FirstName} و نام خانوادگی {investor.LastName} با موفقیت ویرایش شد.";
                        TempData.Keep();
                        return RedirectToAction("IndexIc");
                    }
                    else
                    {
                        TempData["Emessage"] = $"اطلاعات مربوط به استنباط‌ها و نتیجه‌گیری‌های سرمایه‌گذار با نام {investor.FirstName}  و نام خانوادگی {investor.LastName}  ویرایش نشد.";
                        TempData.Keep();
                        ViewBag.InvestorID = new SelectList(investorProfiles, "InvestorID", "FirstName", investorIcProfile.InvestorID);
                        return View(investorIcProfile);
                    }
                }
                TempData["Emessage"] = $"اطلاعات مربوط به استنباط‌ها و نتیجه‌گیری‌های سرمایه‌گذار با نام {investor.FirstName} و نام خانوادگی {investor.LastName}  ویرایش نشد.";
                TempData.Keep();
                ViewBag.InvestorID = new SelectList(investorProfiles, "InvestorID", "FirstName", investorIcProfile.InvestorID);
                return View(investorIcProfile);
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
                db.DisposeDb();
                dbIc.DisposeDb();
            }
            base.Dispose(disposing);
        }
    }
}
