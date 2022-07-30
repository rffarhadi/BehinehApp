using DLLCore.DBContext.Entities.Accounting.Entry;
using DLLCore.Repositories;
using DLLCore.Repositories.Reports;
using DLLCore.Utility.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BehinehApp.Areas.AccountAdmin.Controllers
{
    [Authorize]
    public class TradeEntriesController : Controller
    {
        JournalEntryRepository journalEntryRepo;
        ReportsRepository reportsRepository;
        public TradeEntriesController()
        {
            journalEntryRepo = new JournalEntryRepository();
            reportsRepository = new ReportsRepository();
        }
        // GET: AccountAdmin/TradeEntries
        public async Task<ActionResult> Index(string DisplayfromDate, string DisplaytoDate)
        {
            try
            {

                IEnumerable<Journal> journalList = new List<Journal>();
                if (DisplayfromDate == null && DisplaytoDate == null)
                {
                    journalList = await journalEntryRepo.GetAllIncludeIPsAndRawDetails();
                    return View(journalList.OrderBy(a => a.EntryNo));
                }
                else if (DisplayfromDate != null && DisplaytoDate != null)
                {
                    journalList = await journalEntryRepo.GetAllIncludeIPsAndRawDetails(DisplayfromDate, DisplaytoDate);
                }
                return View(journalList.OrderBy(a => a.EntryNo));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message.ToString();
                return View("Error");
            }
        }

        public async Task<ActionResult> Delete(string fromDate, string toDate)
        {
            try
            {

                IEnumerable<Journal> journalList = new List<Journal>();
                if (fromDate != null && toDate != null)
                {
                    journalList = await journalEntryRepo.GetAllIncludeIPsAndRawDetails(fromDate, toDate);
                }
                if (journalList != null)
                {
                    var result = journalEntryRepo.DeleteEntryList(journalList.ToList());
                    //sucessfullMessage
                    if (result == true)
                    {
                        return RedirectToAction("Index");
                    }
                }
                //errorMessage
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message.ToString();
                return View("Error");
            }
        }



        [HttpPost]
        public async Task<ActionResult> Importexcel()
        {
            try
            {
                HttpPostedFileBase httpPostedFile = Request.Files["ExcelFileUpload"];
                var dt = ReadfromExcel(httpPostedFile);
                var data = DataTableToList.ConvertDataTableToJournalList(dt);
                var saveResult = journalEntryRepo.SaveCDsDataList(data);
                var List = await journalEntryRepo.GetAllIncludeIPsAndRawDetails();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message.ToString();
                return View("Error");
            }

        }
        public ActionResult ViewExcel()
        {
            try
            {
                HttpPostedFileBase httpPostedFile = Request.Files["ExcelFileUpload"];
                var dt = ReadfromExcel(httpPostedFile);
                var data = DataTableToList.ConvertDataTableToJournalList(dt);
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message.ToString();
                return View("Error");
            }
        }
        [HttpPost]
        public ActionResult ImportexcelAfterView()
        {
            try
            {
                CDSExcelFormatDetail cDSExcelFormatDetail = new CDSExcelFormatDetail();
                HttpPostedFileBase httpPostedFile = Request.Files["ExcelFileUpload"];
                var dt = ReadfromExcel(httpPostedFile);
                var data = DataTableToList.ConvertDataTableToJournalList(dt);
                cDSExcelFormatDetail.CDSExcelFormatObject = data;
                return View(cDSExcelFormatDetail);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message.ToString();
                return View("Error");
            }
        }

        public ActionResult UpdateValuationEntries()
        {
            try
            {
                var list = reportsRepository.InvestmentsValuationForInvestorInDetailLevel();
                var result = journalEntryRepo.SaveValuationList(list);
                if (result == true)
                {
                    ViewBag.Message = "ثبت سندهای ارزشیابی با موفقیت انجام شد.";
                    return RedirectToAction("Index");
                }

                ViewBag.Message = "ثبت سندهای ارزشیابی با خطاء مواجه شد.";
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message.ToString();
                return View("Error");
            }
        }

        [AllowAnonymous]
        public ActionResult DownloadFile()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "Files/Excel/";
                byte[] fileBytes = System.IO.File.ReadAllBytes(path + "TradesExcell.xlsx");
                string fileName = "TradesExcell.xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message.ToString();
                return View("Error");
            }
        }




        public DataTable ReadfromExcel(HttpPostedFileBase httpPostedFile)
        {
            try
            {
                DataTable dt = new DataTable();
                if (httpPostedFile.ContentLength > 0)
                {
                    var fileName = httpPostedFile.FileName;
                    string path1 = string.Format("{0}/{1}", Server.MapPath("~/Files/Excel"), fileName);
                    if (!Directory.Exists(path1))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Files/Excel"));
                    }
                    string extension = System.IO.Path.GetExtension(httpPostedFile.FileName).ToLower();
                    string[] validFileTypes = { ".xls", ".xlsx", ".tck" };
                    if (validFileTypes.Contains(extension))
                    {
                        if (System.IO.File.Exists(path1))
                        {
                            System.IO.File.Delete(path1);
                        }
                        httpPostedFile.SaveAs(path1);

                        Accord.IO.ExcelReader db = new Accord.IO.ExcelReader(path1, true, true);
                        var workSheetName = db.GetWorksheetList();
                        dt = db.GetWorksheet(workSheetName[0]);
                       // System.IO.File.Delete(path1);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}