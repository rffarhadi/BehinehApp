using DLLCore.DBContext;
using DLLCore.DBContext.Entities;
using DLLCore.Repositories;
using DLLCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BehinehApp.Controllers
{
    [Authorize]
    public class InvestorRegisterController : Controller
    {
        InvestorRepository db;
        public InvestorRegisterController()
        {
            db = new InvestorRepository();
        }
        // GET: InvestorRegister
        public ActionResult Index()
        {
            try
            {
                ViewBag.Smessage = TempData["Smessage"];
                ViewBag.Emessage = TempData["Emessage"];
                var list = db.GetAllInvestor().ToList();
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult RegisterInvestor()
        {
            try
            {
                ViewBag.Smessage = TempData["Smessage"];
                ViewBag.Emessage = TempData["Emessage"];
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterInvestor(InvestorProfileViewModel vm)
        {
            try
            {
                var result = db.InsertInvestor(vm);
                if (result == true)
                {
                    TempData["Smessage"] = $"اطلاعات شخص با کد ملی {vm.NationalCode} و نام {vm.FirstName} {vm.LastName} با موفقیت ثبت شد";
                    TempData.Keep();
                    return RedirectToAction("Index");
                }
                TempData["Emessage"] = "اطلاعات ثبت نشد";
                TempData.Keep();
                ViewBag.Emessage = TempData["Emessage"];
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public ActionResult InvestorDetails(int? id)
        {
            try
            {
                ViewBag.Smessage = TempData["Smessage"];
                ViewBag.Emessage = TempData["Emessage"];
                var investor = db.GetInvestorByInvestorId(id);
                return View(investor);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public ActionResult EditInvestorProfile(int? id)
        {
            try
            {
                var editItem = db.GetInvestorByInvestorId(id);
                return View(editItem);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInvestorProfile(InvestorProfileViewModel vm)
        {
            try
            {
                var result = db.EditInvestor(vm);
                if (result == true)
                {
                    TempData["Smessage"] = $"اطلاعات شخص با کد ملی {vm.NationalCode} و نام {vm.FirstName} {vm.LastName} با موفقیت ویرایش شد";
                    TempData.Keep();
                    return RedirectToAction("InvestorDetails", new { id = vm.InvestorID });
                }
                TempData["Emessage"] = $"اطلاعات شخص با کد ملی {vm.NationalCode} و نام {vm.FirstName} {vm.LastName} ویرایش نشد"; ;
                TempData.Keep();
                ViewBag.Emessage = TempData["Emessage"];
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var editItem = db.GetInvestorByInvestorId(id);
                if (editItem == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Smessage = TempData["Smessage"];
                ViewBag.Emessage = TempData["Emessage"];
                return View(editItem);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: RegInvestorViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var deleteItem = db.GetInvestorByInvestorId(id);
                var result = db.DeleteInvestor(deleteItem);
                if (result == true)
                {
                    TempData["Smessage"] = $"اطلاعات شخص با کد ملی {deleteItem.NationalCode} و نام {deleteItem.FirstName} {deleteItem.LastName} با موفقیت حذف شد";
                    TempData.Keep();
                    return RedirectToAction("Index");
                }
                TempData["Emessage"] = $"اطلاعات شخص با کد ملی {deleteItem.NationalCode} و نام {deleteItem.FirstName} {deleteItem.LastName} حذف نشد";
                TempData.Keep();
                return View("Delete", deleteItem);
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
            }
            base.Dispose(disposing);
        }
    }
}