using DLLCore.DBContext;
using DLLCore.Repositories.PriceHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BehinehApp.Areas.SecurityHistory.Controllers
{
    [Authorize]
    public class CrudeController : Controller
    {
        ApplicationDbContext db;
        Save SaveRepo;
        PriceRepository PriceRepository;
        public CrudeController()
        {
            db = new ApplicationDbContext();
            SaveRepo = new Save();
            PriceRepository = new PriceRepository();
        }
        // GET: SecurityHistory/Crude
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                var dateList = db.SecurityTbl.ToList();
                return View(dateList);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Index(string date)
        {
            try
            {
                var result = SaveRepo.UpdateSecurityTbl();
                var dateList = PriceRepository.GetAll();
                return View(dateList);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        public ActionResult UpdateTafsileCode()
        {
            try
            {
                SaveRepo.UpdateTafsileCode();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }
    }
}