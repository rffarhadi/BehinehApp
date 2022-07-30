using DLLCore.Repositories.Reports;
using DLLCore.ViewModels.FinancialStatmentsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BehinehApp.Areas.AccountAdmin.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {

        ReportsRepository reportRepo;
        public ReportsController()
        {
            reportRepo = new ReportsRepository();
        }
        // GET: AccountAdmin/Reports
        public ActionResult TestBanlanc()
        {
            try
            {
                var bsList = reportRepo.TestBalanceInLedger();
                return View(bsList);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }
        public ActionResult TestBanlancByInvestor(long? InvestorId)
        {
            try
            {
                List<BalanceSheetViewModel> bsList = new List<BalanceSheetViewModel>();
                bsList = reportRepo.TestBalanceForInvestorInLedger();
                if (InvestorId != null)
                {
                    bsList = reportRepo.TestBalanceForInvestorInLedger().Where(a => a.InvestorId == InvestorId).ToList();
                }
                var selectList = reportRepo.TestBalanceForInvestorInLedger().Select(a => new { a.InvestorId, a.InvestorName }).Distinct().ToList();
                ViewBag.Investors = new SelectList(selectList, "InvestorId", "InvestorName");

                return View(bsList);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        public ActionResult TestBanlancInSubLedger()
        {
            try
            {
                var bsList = reportRepo.TestBalanceInSubLedger();
                return View(bsList);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }
        public ActionResult TestBanlancInSubLedgerByInvestor(int? InvestorId)
        {
            try
            {
                List<BalanceSheetViewModel> bsList = new List<BalanceSheetViewModel>();
                bsList = reportRepo.TestBalanceForInvestorInSubLedger();
                if (InvestorId != null)
                {
                    bsList = reportRepo.TestBalanceForInvestorInSubLedger().Where(a => a.InvestorId == InvestorId).ToList();
                }

                var selectList = reportRepo.TestBalanceForInvestorInSubLedger().Select(a => new { a.InvestorId, a.InvestorName }).Distinct().ToList();
                ViewBag.Investors = new SelectList(selectList, "InvestorId", "InvestorName");

                return View(bsList);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        public ActionResult TestBanlancInDetailLevel()
        {
            try
            {
                var bsList = reportRepo.TestBalanceInDetailLevel();
                return View(bsList);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }
        public ActionResult TestBanlancInDetailLevelByInvestor(int? InvestorId)
        {
            try
            {
                List<BalanceSheetViewModel> bsList = new List<BalanceSheetViewModel>();
                bsList = reportRepo.TestBalanceForInvestorInDetailLevel();
                if (InvestorId != null)
                {
                    bsList = reportRepo.TestBalanceForInvestorInDetailLevel().Where(a => a.InvestorId == InvestorId).ToList();
                }

                var selectList = reportRepo.TestBalanceForInvestorInDetailLevel().Select(a => new { a.InvestorId, a.InvestorName }).Distinct().ToList();
                ViewBag.Investors = new SelectList(selectList, "InvestorId", "InvestorName");

                return View(bsList);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }
        public ActionResult InvestmentsInDetailLevelByInvestor(int? InvestorId)
        {
            try
            {
                List<BalanceSheetViewModel> bsList = new List<BalanceSheetViewModel>();
                bsList = reportRepo.InvestmentsRelatedForInvestorInDetailLevel();
                if(InvestorId != null)
                {
                    bsList = reportRepo.InvestmentsRelatedForInvestorInDetailLevel().Where(a=>a.InvestorId== InvestorId).ToList();
                }

                var selectList = reportRepo.InvestmentsRelatedForInvestorInDetailLevel().Select(a => new { a.InvestorId, a.InvestorName }).Distinct().ToList();
                ViewBag.Investors = new SelectList(selectList, "InvestorId", "InvestorName");

                return View(bsList);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }

        public ActionResult GetPerformance(int? InvestorId)
        {
            try
            {
                List<PerformanceViewModel> bsList = new List<PerformanceViewModel>();
                bsList = reportRepo.GetPerformance();
                if (InvestorId != null)
                {
                    bsList = reportRepo.GetPerformance().Where(a => a.InvestorId == InvestorId).ToList();
                }

                var selectList = reportRepo.GetPerformance().Select(a => new { a.InvestorId, a.InvestorName }).Distinct().ToList();
                ViewBag.Investors = new SelectList(selectList, "InvestorId", "InvestorName");

                return View(bsList);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View("Error");
            }
        }
    }
}