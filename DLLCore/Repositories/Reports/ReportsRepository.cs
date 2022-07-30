using DLLCore.DBContext;
using DLLCore.DBContext.Entities.Accounting.RawAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using DLLCore.ViewModels.FinancialStatmentsViewModels;
using DLLCore.Utility.Enums;

namespace DLLCore.Repositories.Reports
{
    public class ReportsRepository
    {
        ApplicationDbContext db;
        public ReportsRepository()
        {
            db = new ApplicationDbContext();
        }
        //در سطح کل
        public List<BalanceSheetViewModel> TestBalanceInLedger()
        {
            List<BalanceSheetViewModel> balanceSheetFromJournalList = new List<BalanceSheetViewModel>();
            var journalTbl = db.JournalsTbl.Include(a => a.investorProfileInJournal).Include(b => b.rawDetaile).Include(a => a.rawDetaile.rawSubLedger).Include(a => a.rawDetaile.rawSubLedger.rawLedgers.RawSubCategories).ToList().GroupBy(a => a.rawDetaile.rawSubLedger.rawLedgers.RawLedgerName).ToList();
            foreach (var item in journalTbl)
            {
                BalanceSheetViewModel balance = new BalanceSheetViewModel()
                {
                    InvestorId = item.Select(a => a.InvestorID).FirstOrDefault(),
                    AccountId = item.Select(a => a.rawDetaile.rawSubLedger.rawLedgers.RawLedgerID).FirstOrDefault(),
                    SubCategoryId = item.Select(a => a.rawDetaile.rawSubLedger.rawLedgers.SubCategoryID).FirstOrDefault(),
                    CategoryId = item.Select(a => a.rawDetaile.rawSubLedger.rawLedgers.RawSubCategories.RawCategoryID).FirstOrDefault(),
                    AccountName = item.Select(a => a.rawDetaile.rawSubLedger.rawLedgers.RawLedgerName).FirstOrDefault(),
                    CreditBalance = item.Select(a => a.EntryCredit).Sum(),
                    DebitBalance = item.Select(a => a.EntryDebit).Sum(),
                    TotalBalance = Math.Abs(item.Select(a => a.EntryDebit).Sum() - item.Select(a => a.EntryCredit).Sum()),
                    LastDateOfUpdate = item.Select(a => a.EntryDate).LastOrDefault(),
                    Nature = item.Select(a => a.EntryCredit - a.EntryDebit).Sum() > 0 ? "بستانکار" : item.Select(a => a.EntryCredit - a.EntryDebit).Sum() < 0 ? "بدهکار" : "-",
                };
                balanceSheetFromJournalList.Add(balance);
            }
            List<BalanceSheetViewModel> balanceSheetFromRawLedgerList = new List<BalanceSheetViewModel>();

            var rawLedgerstbl = db.RawLedgersTbl.Include(r => r.RawSubCategories.RawCategoriesInSub).OrderBy(a => a.SubCategoryID).ThenBy(a => a.RawLedgerID).ToList();
            var rawLedgerGeroupByLedgerName = rawLedgerstbl.GroupBy(a => a.RawLedgerName).ToList();
            foreach (var item in rawLedgerGeroupByLedgerName)
            {
                BalanceSheetViewModel balance = new BalanceSheetViewModel()
                {
                    AccountId = item.Select(a => a.RawLedgerID).FirstOrDefault(),
                    SubCategoryId = item.Select(a => a.SubCategoryID).FirstOrDefault(),
                    CategoryId = item.Select(a => a.RawSubCategories.RawCategoriesInSub.RawCategoryID).FirstOrDefault(),
                    AccountName = item.Select(a => a.RawLedgerName).FirstOrDefault(),
                    CreditBalance = 0,
                    DebitBalance = 0,
                    TotalBalance = 0,
                    LastDateOfUpdate = "-",
                    Nature = "-",
                };
                balanceSheetFromRawLedgerList.Add(balance);
            }
            balanceSheetFromRawLedgerList.AddRange(balanceSheetFromJournalList);
            balanceSheetFromJournalList.Clear();

            var finalList = balanceSheetFromRawLedgerList.GroupBy(a => a.AccountName).ToList();
            foreach (var item in finalList)
            {
                var rawLedgerId = item.Select(a => a.AccountId).FirstOrDefault();
                //حساب خالص دارایی‌ها کد 12 دارد
                int tblRawLedgerID = 12;
                if (rawLedgerId == tblRawLedgerID)
                {
                    BalanceSheetViewModel balance = new BalanceSheetViewModel()
                    {
                        AccountId = item.Select(a => a.AccountId).FirstOrDefault(),
                        CategoryId = item.Select(a => a.CategoryId).FirstOrDefault(),
                        SubCategoryId = item.Select(a => a.SubCategoryId).FirstOrDefault(),
                        AccountName = item.Select(a => a.AccountName).FirstOrDefault(),
                        CreditBalance = balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblRawLedgerID).Select(a => a.CreditBalance).Sum(),
                        DebitBalance = balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblRawLedgerID).Select(a => a.DebitBalance).Sum(),
                        TotalBalance = Math.Abs(balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblRawLedgerID).Select(a => a.CreditBalance).Sum() - balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblRawLedgerID).Select(a => a.DebitBalance).Sum()),
                        LastDateOfUpdate = balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblRawLedgerID).Select(a => a.LastDateOfUpdate).LastOrDefault(),
                        Nature = balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblRawLedgerID).Select(a => a.CreditBalance).Sum() > balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblRawLedgerID).Select(a => a.DebitBalance).Sum() ? "بستانکار" : balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblRawLedgerID).Select(a => a.CreditBalance).Sum() < balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblRawLedgerID).Select(a => a.DebitBalance).Sum() ? "بدهکار" : "",
                    };
                    balanceSheetFromJournalList.Add(balance);
                }
                else
                {
                    BalanceSheetViewModel balance = new BalanceSheetViewModel()
                    {
                        AccountId = item.Select(a => a.AccountId).FirstOrDefault(),
                        CategoryId = item.Select(a => a.CategoryId).FirstOrDefault(),
                        SubCategoryId = item.Select(a => a.SubCategoryId).FirstOrDefault(),
                        AccountName = item.Select(a => a.AccountName).FirstOrDefault(),
                        CreditBalance = item.Select(a => a.CreditBalance).Sum(),
                        DebitBalance = item.Select(a => a.DebitBalance).Sum(),
                        TotalBalance = item.Select(a => a.TotalBalance).Sum(),
                        LastDateOfUpdate = item.Where(a => a.LastDateOfUpdate != "-").Select(a => a.LastDateOfUpdate).LastOrDefault(),
                        Nature = item.Where(a => a.LastDateOfUpdate != "-").Select(a => a.Nature).LastOrDefault()
                    };
                    balanceSheetFromJournalList.Add(balance);
                }
            }
            return balanceSheetFromJournalList.OrderBy(a => a.AccountId).ToList();
        }
        public List<BalanceSheetViewModel> TestBalanceForInvestorInLedger()
        {
            List<BalanceSheetViewModel> balanceSheetFromJournalList = new List<BalanceSheetViewModel>();

            var journalTbl = db.JournalsTbl.Include(a => a.investorProfileInJournal).Include(a => a.rawDetaile.rawSubLedger.rawLedgers.RawSubCategories).ToList();
            var journalGroupByInvestorId = journalTbl.GroupBy(a => a.InvestorID).ToList();
            foreach (var itemJournal in journalGroupByInvestorId)
            {
                var itemGroupedByLedgerName = itemJournal.GroupBy(a => a.rawDetaile.rawSubLedger.rawLedgers.RawLedgerName).ToList();
                foreach (var itemledger in itemGroupedByLedgerName)
                {
                    BalanceSheetViewModel balance = new BalanceSheetViewModel()
                    {
                        InvestorName = itemledger.Select(a => a.investorProfileInJournal.FirstName).FirstOrDefault() + " " + itemledger.Select(a => a.investorProfileInJournal.LastName).FirstOrDefault(),
                        InvestorId = itemledger.Select(a => a.InvestorID).FirstOrDefault(),
                        AccountId = itemledger.Select(a => a.rawDetaile.rawSubLedger.rawLedgers.RawLedgerID).FirstOrDefault(),
                        SubCategoryId = itemledger.Select(a => a.rawDetaile.rawSubLedger.rawLedgers.RawLedgerCode).FirstOrDefault(),
                        CategoryId = itemledger.Select(a => a.rawDetaile.rawSubLedger.rawLedgers.RawSubCategories.SubCategoryCode).FirstOrDefault(),
                        AccountName = itemledger.Select(a => a.rawDetaile.rawSubLedger.rawLedgers.RawLedgerName).FirstOrDefault(),
                        CreditBalance = itemledger.Select(a => a.EntryCredit).Sum(),
                        DebitBalance = itemledger.Select(a => a.EntryDebit).Sum(),
                        TotalBalance = Math.Abs(itemledger.Select(a => a.EntryDebit).Sum() - itemledger.Select(a => a.EntryCredit).Sum()),
                        LastDateOfUpdate = itemledger.Select(a => a.EntryDate).LastOrDefault(),
                        Nature = itemledger.Select(a => a.EntryCredit - a.EntryDebit).Sum() > 0 ? "بستانکار" : itemledger.Select(a => a.EntryCredit - a.EntryDebit).Sum() < 0 ? "بدهکار" : "-",
                    };
                    balanceSheetFromJournalList.Add(balance);
                }
            }

            return balanceSheetFromJournalList;
        }

        //در سطح معین
        public List<BalanceSheetViewModel> TestBalanceInSubLedger()
        {

            List<BalanceSheetViewModel> balanceSheetFromJournalList = new List<BalanceSheetViewModel>();
            var journalTbl = db.JournalsTbl.Include(a => a.investorProfileInJournal).Include(b => b.rawDetaile).Include(a => a.rawDetaile.rawSubLedger.rawLedgers.RawSubCategories).ToList().GroupBy(a => a.rawDetaile.SubLedgerID).ToList();
            foreach (var item in journalTbl)
            {
                BalanceSheetViewModel balance = new BalanceSheetViewModel()
                {
                    InvestorId = item.Select(a => a.InvestorID).FirstOrDefault(),
                    AccountId = item.Select(a => a.rawDetaile.rawSubLedger.SubLedgerID).FirstOrDefault(),
                    SubCategoryId = item.Select(a => a.rawDetaile.rawSubLedger.SubLedgerCode).FirstOrDefault(),
                    CategoryId = item.Select(a => a.rawDetaile.rawSubLedger.rawLedgers.RawLedgerCode).FirstOrDefault(),
                    AccountName = item.Select(a => a.rawDetaile.rawSubLedger.SubLedgerName).FirstOrDefault(),
                    CreditBalance = item.Select(a => a.EntryCredit).Sum(),
                    DebitBalance = item.Select(a => a.EntryDebit).Sum(),
                    TotalBalance = Math.Abs(item.Select(a => a.EntryDebit).Sum() - item.Select(a => a.EntryCredit).Sum()),
                    LastDateOfUpdate = item.Select(a => a.EntryDate).LastOrDefault(),
                    Nature = item.Select(a => a.EntryCredit - a.EntryDebit).Sum() > 0 ? "بستانکار" : item.Select(a => a.EntryCredit - a.EntryDebit).Sum() < 0 ? "بدهکار" : "-",
                };
                balanceSheetFromJournalList.Add(balance);
            }
            List<BalanceSheetViewModel> balanceSheetFromRawLedgerList = new List<BalanceSheetViewModel>();

            var rawSubLedgerstbl = db.RawSubLedgersTbl.Include(r => r.rawLedgers.RawSubCategories.RawCategoriesInSub).OrderBy(a => a.SubLedgerCode).ThenBy(a => a.RawLedgerID).ToList();
            var rawGeroupByLedgerName = rawSubLedgerstbl.GroupBy(a => a.SubLedgerName).ToList();
            foreach (var item in rawGeroupByLedgerName)
            {
                BalanceSheetViewModel balance = new BalanceSheetViewModel()
                {
                    InvestorId = 0,
                    AccountId = item.Select(a => a.SubLedgerID).FirstOrDefault(),
                    SubCategoryId = item.Select(a => a.SubLedgerCode).FirstOrDefault(),
                    CategoryId = item.Select(a => a.rawLedgers.RawLedgerCode).FirstOrDefault(),
                    AccountName = item.Select(a => a.SubLedgerName).FirstOrDefault(),
                    CreditBalance = 0,
                    DebitBalance = 0,
                    TotalBalance = 0,
                    LastDateOfUpdate = "-",
                    Nature = "-",
                };
                balanceSheetFromRawLedgerList.Add(balance);
            }

            balanceSheetFromRawLedgerList.AddRange(balanceSheetFromJournalList);
            balanceSheetFromJournalList.Clear();

            var finalList = balanceSheetFromRawLedgerList.GroupBy(a => a.AccountName).ToList();
            foreach (var item in finalList)
            {
                var Id = item.Select(a => a.AccountId).FirstOrDefault();
                //حساب خالص دارایی‌ها کد 36 دارد
                int tblID = 36;
                if (Id == tblID)
                {
                    BalanceSheetViewModel balance = new BalanceSheetViewModel()
                    {
                        InvestorId = item.Select(a => a.InvestorId).FirstOrDefault(),
                        AccountId = item.Select(a => a.AccountId).FirstOrDefault(),
                        CategoryId = item.Select(a => a.CategoryId).FirstOrDefault(),
                        SubCategoryId = item.Select(a => a.SubCategoryId).FirstOrDefault(),
                        AccountName = item.Select(a => a.AccountName).FirstOrDefault(),
                        CreditBalance = balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.CreditBalance).Sum(),
                        DebitBalance = balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.DebitBalance).Sum(),
                        TotalBalance = Math.Abs(balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.CreditBalance).Sum() - balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.DebitBalance).Sum()),
                        LastDateOfUpdate = balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.LastDateOfUpdate).LastOrDefault(),
                        Nature = balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.CreditBalance).Sum() > balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.DebitBalance).Sum() ? "بستانکار" : balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.CreditBalance).Sum() < balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.DebitBalance).Sum() ? "بدهکار" : "",
                    };
                    balanceSheetFromJournalList.Add(balance);
                }
                else
                {
                    BalanceSheetViewModel balance = new BalanceSheetViewModel()
                    {
                        InvestorId = item.Select(a => a.InvestorId).FirstOrDefault(),
                        AccountId = item.Select(a => a.AccountId).FirstOrDefault(),
                        CategoryId = item.Select(a => a.CategoryId).FirstOrDefault(),
                        SubCategoryId = item.Select(a => a.SubCategoryId).FirstOrDefault(),
                        AccountName = item.Select(a => a.AccountName).FirstOrDefault(),
                        CreditBalance = item.Select(a => a.CreditBalance).Sum(),
                        DebitBalance = item.Select(a => a.DebitBalance).Sum(),
                        TotalBalance = item.Select(a => a.TotalBalance).Sum(),
                        LastDateOfUpdate = item.Where(a => a.LastDateOfUpdate != "-").Select(a => a.LastDateOfUpdate).LastOrDefault(),
                        Nature = item.Where(a => a.LastDateOfUpdate != "-").Select(a => a.Nature).LastOrDefault()
                    };
                    balanceSheetFromJournalList.Add(balance);
                }
            }
            return balanceSheetFromJournalList.OrderBy(a => a.AccountId).ToList();
        }
        public List<BalanceSheetViewModel> TestBalanceForInvestorInSubLedger()
        {
            List<BalanceSheetViewModel> balanceSheetFromJournalList = new List<BalanceSheetViewModel>();

            var journalTbl = db.JournalsTbl.Include(a => a.investorProfileInJournal).Include(a => a.rawDetaile.rawSubLedger.rawLedgers.RawSubCategories).ToList();
            var journalGroupByInvestorId = journalTbl.GroupBy(a => a.InvestorID).ToList();
            foreach (var itemJournal in journalGroupByInvestorId)
            {
                var itemGroupedByName = itemJournal.GroupBy(a => a.rawDetaile.rawSubLedger.SubLedgerName).ToList();
                foreach (var item in itemGroupedByName)
                {
                    BalanceSheetViewModel balance = new BalanceSheetViewModel()
                    {
                        InvestorName = item.Select(a => a.investorProfileInJournal.FirstName).FirstOrDefault() + " " + item.Select(a => a.investorProfileInJournal.LastName).FirstOrDefault(),
                        InvestorId = item.Select(a => a.InvestorID).FirstOrDefault(),
                        AccountId = item.Select(a => a.rawDetaile.rawSubLedger.SubLedgerID).FirstOrDefault(),
                        SubCategoryId = item.Select(a => a.rawDetaile.rawSubLedger.SubLedgerCode).FirstOrDefault(),
                        CategoryId = item.Select(a => a.rawDetaile.rawSubLedger.rawLedgers.RawLedgerCode).FirstOrDefault(),
                        AccountName = item.Select(a => a.rawDetaile.rawSubLedger.SubLedgerName).FirstOrDefault(),
                        CreditBalance = item.Select(a => a.EntryCredit).Sum(),
                        DebitBalance = item.Select(a => a.EntryDebit).Sum(),
                        TotalBalance = Math.Abs(item.Select(a => a.EntryDebit).Sum() - item.Select(a => a.EntryCredit).Sum()),
                        LastDateOfUpdate = item.Select(a => a.EntryDate).LastOrDefault(),
                        Nature = item.Select(a => a.EntryCredit - a.EntryDebit).Sum() > 0 ? "بستانکار" : item.Select(a => a.EntryCredit - a.EntryDebit).Sum() < 0 ? "بدهکار" : "-",
                    };
                    balanceSheetFromJournalList.Add(balance);
                }
            }

            return balanceSheetFromJournalList;
        }

        //در سطح تفصیل
        public List<BalanceSheetViewModel> TestBalanceInDetailLevel()
        {

            List<BalanceSheetViewModel> balanceSheetFromJournalList = new List<BalanceSheetViewModel>();
            var journalTbl = db.JournalsTbl.Include(a => a.investorProfileInJournal).Include(b => b.rawDetaile).Include(a => a.rawDetaile.rawSubLedger.rawLedgers).ToList().GroupBy(a => a.rawDetaile.DetaileID).ToList();
            foreach (var item in journalTbl)
            {
                BalanceSheetViewModel balance = new BalanceSheetViewModel()
                {
                    InvestorId = item.Select(a => a.InvestorID).FirstOrDefault(),
                    AccountId = item.Select(a => a.rawDetaile.DetaileID).FirstOrDefault(),
                    SubCategoryId = item.Select(a => a.rawDetaile.DetaileCode).FirstOrDefault(),
                    CategoryId = item.Select(a => a.rawDetaile.rawSubLedger.SubLedgerCode).FirstOrDefault(),
                    AccountName = item.Select(a => a.rawDetaile.DetaileName).FirstOrDefault(),
                    CreditBalance = item.Select(a => a.EntryCredit).Sum(),
                    DebitBalance = item.Select(a => a.EntryDebit).Sum(),
                    TotalBalance = Math.Abs(item.Select(a => a.EntryDebit).Sum() - item.Select(a => a.EntryCredit).Sum()),
                    LastDateOfUpdate = item.Select(a => a.EntryDate).LastOrDefault(),
                    Nature = item.Select(a => a.EntryCredit - a.EntryDebit).Sum() > 0 ? "بستانکار" : item.Select(a => a.EntryCredit - a.EntryDebit).Sum() < 0 ? "بدهکار" : "-",
                };
                balanceSheetFromJournalList.Add(balance);
            }
            List<BalanceSheetViewModel> balanceSheetFromRawLedgerList = new List<BalanceSheetViewModel>();

            var rawSubLedgerstbl = db.RawDetailesTbl.Include(r => r.rawSubLedger).OrderBy(a => a.DetaileCode).ThenBy(a => a.DetaileID).ToList();
            var rawGeroupByLedgerName = rawSubLedgerstbl.GroupBy(a => a.DetaileName).ToList();
            foreach (var item in rawGeroupByLedgerName)
            {
                BalanceSheetViewModel balance = new BalanceSheetViewModel()
                {
                    InvestorId = 0,
                    AccountId = item.Select(a => a.DetaileID).FirstOrDefault(),
                    SubCategoryId = item.Select(a => a.DetaileCode).FirstOrDefault(),
                    CategoryId = item.Select(a => a.rawSubLedger.SubLedgerCode).FirstOrDefault(),
                    AccountName = item.Select(a => a.DetaileName).FirstOrDefault(),
                    CreditBalance = 0,
                    DebitBalance = 0,
                    TotalBalance = 0,
                    LastDateOfUpdate = "-",
                    Nature = "-",
                };
                balanceSheetFromRawLedgerList.Add(balance);
            }

            balanceSheetFromRawLedgerList.AddRange(balanceSheetFromJournalList);
            balanceSheetFromJournalList.Clear();

            var finalList = balanceSheetFromRawLedgerList.GroupBy(a => a.AccountName).ToList();
            foreach (var item in finalList)
            {
                var Id = item.Select(a => a.AccountId).FirstOrDefault();
                //حساب خالص دارایی‌ها کد 36 دارد
                int tblID = 36;
                if (Id == tblID)
                {
                    BalanceSheetViewModel balance = new BalanceSheetViewModel()
                    {
                        InvestorId = item.Select(a => a.InvestorId).FirstOrDefault(),
                        AccountId = item.Select(a => a.AccountId).FirstOrDefault(),
                        CategoryId = item.Select(a => a.CategoryId).FirstOrDefault(),
                        SubCategoryId = item.Select(a => a.SubCategoryId).FirstOrDefault(),
                        AccountName = item.Select(a => a.AccountName).FirstOrDefault(),
                        CreditBalance = balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.CreditBalance).Sum(),
                        DebitBalance = balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.DebitBalance).Sum(),
                        TotalBalance = Math.Abs(balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.CreditBalance).Sum() - balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.DebitBalance).Sum()),
                        LastDateOfUpdate = balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.LastDateOfUpdate).LastOrDefault(),
                        Nature = balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.CreditBalance).Sum() > balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.DebitBalance).Sum() ? "بستانکار" : balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.CreditBalance).Sum() < balanceSheetFromRawLedgerList.Where(a => a.AccountId >= tblID).Select(a => a.DebitBalance).Sum() ? "بدهکار" : "",
                    };
                    balanceSheetFromJournalList.Add(balance);
                }
                else
                {
                    BalanceSheetViewModel balance = new BalanceSheetViewModel()
                    {
                        InvestorId = item.Select(a => a.InvestorId).FirstOrDefault(),
                        AccountId = item.Select(a => a.AccountId).FirstOrDefault(),
                        CategoryId = item.Select(a => a.CategoryId).FirstOrDefault(),
                        SubCategoryId = item.Select(a => a.SubCategoryId).FirstOrDefault(),
                        AccountName = item.Select(a => a.AccountName).FirstOrDefault(),
                        CreditBalance = item.Select(a => a.CreditBalance).Sum(),
                        DebitBalance = item.Select(a => a.DebitBalance).Sum(),
                        TotalBalance = item.Select(a => a.TotalBalance).Sum(),
                        LastDateOfUpdate = item.Where(a => a.LastDateOfUpdate != "-").Select(a => a.LastDateOfUpdate).LastOrDefault(),
                        Nature = item.Where(a => a.LastDateOfUpdate != "-").Select(a => a.Nature).LastOrDefault()
                    };
                    balanceSheetFromJournalList.Add(balance);
                }
            }
            return balanceSheetFromJournalList.OrderBy(a => a.AccountId).ToList();
        }
        public List<BalanceSheetViewModel> TestBalanceForInvestorInDetailLevel()
        {
            List<BalanceSheetViewModel> balanceSheetFromJournalList = new List<BalanceSheetViewModel>();

            var journalTbl = db.JournalsTbl.Include(a => a.investorProfileInJournal).Include(a => a.rawDetaile.rawSubLedger.rawLedgers.RawSubCategories).ToList();
            var journalGroupByInvestorId = journalTbl.GroupBy(a => a.InvestorID).ToList();
            foreach (var itemJournal in journalGroupByInvestorId)
            {
                var itemGroupedByName = itemJournal.GroupBy(a => a.rawDetaile.DetaileName).ToList();
                foreach (var item in itemGroupedByName)
                {
                    BalanceSheetViewModel balance = new BalanceSheetViewModel()
                    {
                        InvestorName = item.Select(a => a.investorProfileInJournal.FirstName).FirstOrDefault() + " " + item.Select(a => a.investorProfileInJournal.LastName).FirstOrDefault(),
                        StockTafsileInCds = (long)item.Select(a => a.rawDetaile.TafsilCodeInCDS).FirstOrDefault(),
                        InvestorId = item.Select(a => a.InvestorID).FirstOrDefault(),
                        AccountId = item.Select(a => a.rawDetaile.DetaileID).FirstOrDefault(),
                        SubCategoryId = item.Select(a => a.rawDetaile.DetaileCode).FirstOrDefault(),
                        CategoryId = item.Select(a => a.rawDetaile.rawSubLedger.SubLedgerCode).FirstOrDefault(),
                        AccountName = item.Select(a => a.rawDetaile.DetaileName).FirstOrDefault(),
                        CreditBalance = item.Select(a => a.EntryCredit).Sum(),
                        DebitBalance = item.Select(a => a.EntryDebit).Sum(),
                        TotalBalance = Math.Abs(item.Select(a => a.EntryDebit).Sum() - item.Select(a => a.EntryCredit).Sum()),
                        LastDateOfUpdate = item.Select(a => a.EntryDate).LastOrDefault(),
                        Nature = item.Select(a => a.EntryCredit - a.EntryDebit).Sum() > 0 ? "بستانکار" : item.Select(a => a.EntryCredit - a.EntryDebit).Sum() < 0 ? "بدهکار" : "-",
                    };
                    balanceSheetFromJournalList.Add(balance);
                }
            }

            return balanceSheetFromJournalList;
        }

        //حساب سرمایه‌گذاری و ارزشیابی در سطح تفصیل
        public List<BalanceSheetViewModel> InvestmentsRelatedForInvestorInDetailLevel()
        {
            List<BalanceSheetViewModel> InvestementsAcountFromJournalList = new List<BalanceSheetViewModel>();
            int rawLedgerId = EnumClass.InvestmentsRelatedIdInRawLedger;

            var journalTbl = db.JournalsTbl.Include(a => a.investorProfileInJournal).Include(a => a.rawDetaile.rawSubLedger.rawLedgers.RawSubCategories).Where(a => a.rawDetaile.rawSubLedger.RawLedgerID == rawLedgerId).ToList();
            var journalGroupByInvestorId = journalTbl.GroupBy(a => a.InvestorID).ToList();
            foreach (var itemJournal in journalGroupByInvestorId)
            {
                var itemGroupedByName = itemJournal.GroupBy(a => a.rawDetaile.DetaileID).ToList();
                foreach (var item in itemGroupedByName)
                {
                    long creditBalance = item.Select(a => a.EntryCredit).ToList()?.Sum() ?? 0;
                    long debitBalance = item.Select(a => a.EntryDebit).ToList()?.Sum() ?? 0;
                    long volume = item.Select(a => a.EntryBuySellVolume).ToList()?.Sum() ?? 0;
                    long totalBalance = debitBalance - creditBalance;
                    long avgPrice = 0;
                    if (volume != 0)
                    {
                        avgPrice = totalBalance / volume;
                    }
                    BalanceSheetViewModel balance = new BalanceSheetViewModel()
                    {
                        InvestorName = item.Select(a => a.investorProfileInJournal.FirstName).FirstOrDefault() + " " + item.Select(a => a.investorProfileInJournal.LastName).FirstOrDefault(),
                        InvestorId = item.Select(a => a.InvestorID).FirstOrDefault(),
                        AccountId = item.Select(a => a.rawDetaile.DetaileID).FirstOrDefault(),
                        SubCategoryId = item.Select(a => a.rawDetaile.DetaileCode).FirstOrDefault(),
                        CategoryId = item.Select(a => a.rawDetaile.rawSubLedger.SubLedgerCode).FirstOrDefault(),
                        AccountName = item.Select(a => a.rawDetaile.DetaileName).FirstOrDefault(),
                        CreditBalance = creditBalance,
                        DebitBalance = debitBalance,
                        TotalBalance = Math.Abs(totalBalance),
                        VolumeBalance = volume,
                        PriceAvreage = avgPrice,
                        LastDateOfUpdate = item.Select(a => a.EntryDate).LastOrDefault(),
                        Nature = item.Select(a => a.EntryCredit - a.EntryDebit).Sum() > 0 ? "بستانکار" : item.Select(a => a.EntryCredit - a.EntryDebit).Sum() < 0 ? "بدهکار" : "-",
                    };
                    InvestementsAcountFromJournalList.Add(balance);
                }
            }

            return InvestementsAcountFromJournalList;
        }

        public List<BalanceSheetViewModel> InvestmentsValuationForInvestorInDetailLevel()
        {
            List<BalanceSheetViewModel> InvestementsAcountFromJournalList = new List<BalanceSheetViewModel>();
            int rawLedgerId = EnumClass.InvestmentsRelatedIdInRawLedger;

            var journalTbl = db.JournalsTbl.Include(a => a.investorProfileInJournal).Include(a => a.rawDetaile.rawSubLedger.rawLedgers.RawSubCategories).Where(a => a.rawDetaile.rawSubLedger.RawLedgerID == rawLedgerId).ToList();
            var journalGroupByInvestorId = journalTbl.GroupBy(a => a.InvestorID).ToList();
            foreach (var itemJournal in journalGroupByInvestorId)
            {
                var itemGroupedByName = itemJournal.GroupBy(a => a.rawDetaile.TafsilCodeInCDS).ToList();
                foreach (var item in itemGroupedByName)
                {
                    var creditBalance = item.Select(a => a.EntryCredit).ToList()?.Sum() ?? 0;
                    var debitBalance = item.Select(a => a.EntryDebit).ToList()?.Sum() ?? 0;
                    var totalBalance = debitBalance - creditBalance;
                    BalanceSheetViewModel balance = new BalanceSheetViewModel()
                    {
                        InvestorName = item.Select(a => a.investorProfileInJournal.FirstName).FirstOrDefault() + " " + item.Select(a => a.investorProfileInJournal.LastName).FirstOrDefault(),
                        StockTafsileInCds = (long)item.Select(a => a.rawDetaile.TafsilCodeInCDS).FirstOrDefault(),
                        InvestorId = item.Select(a => a.InvestorID).FirstOrDefault(),
                        AccountId = item.Select(a => a.rawDetaile.DetaileID).FirstOrDefault(),
                        SubCategoryId = item.Select(a => a.rawDetaile.DetaileCode).FirstOrDefault(),
                        CategoryId = item.Select(a => a.rawDetaile.rawSubLedger.SubLedgerCode).FirstOrDefault(),
                        SubLedgerId = item.Select(a => a.rawDetaile.SubLedgerID).FirstOrDefault(),
                        AccountName = item.Select(a => a.rawDetaile.DetaileName).FirstOrDefault(),
                        AccountSymboleName = item.Select(a => a.rawDetaile.DetaileSymbole).FirstOrDefault(),
                        CreditBalance = creditBalance,
                        DebitBalance = debitBalance,
                        TotalBalance = totalBalance,
                        VolumeBalance = item.Select(a => a.EntryBuySellVolume).ToList()?.Sum() ?? 0,
                        LastDateOfUpdate = item.Select(a => a.EntryDate).LastOrDefault(),
                        Nature = item.Select(a => a.EntryCredit - a.EntryDebit).Sum() > 0 ? "بستانکار" : item.Select(a => a.EntryCredit - a.EntryDebit).Sum() < 0 ? "بدهکار" : "-",
                    };
                    InvestementsAcountFromJournalList.Add(balance);
                }
            }

            return InvestementsAcountFromJournalList;
        }

        public List<PerformanceViewModel> GetPerformance()
        {
            List<PerformanceViewModel> listGroupedByInvestorID = new List<PerformanceViewModel>();
            var list = db.JournalsTbl.Include(a => a.investorProfileInJournal).Include(a => a.rawDetaile.rawSubLedger.rawLedgers.RawSubCategories).ToList().GroupBy(a => a.InvestorID).ToList();
            foreach (var item in list)
            {
                long totalAssets = item.Where(a => a.rawDetaile.rawSubLedger.rawLedgers.RawSubCategories.RawCategoryID == 1 && a.rawDetaile.rawSubLedger.RawLedgerID != 6).Select(a => a.EntryDebit - a.EntryCredit).ToList()?.Sum() ?? 0;
                long totalProfit =item.Where(a => a.rawDetaile.rawSubLedger.rawLedgers.RawSubCategories.RawCategoryID >= 4).Select(a => a.EntryCredit - a.EntryDebit).ToList()?.Sum() ?? 0;
                decimal totalReturn = 0;
                if (totalAssets!=0)
                {
                    totalReturn =Convert.ToDecimal(totalProfit) / Convert.ToDecimal(totalAssets);

                }
                PerformanceViewModel model = new PerformanceViewModel
                {
                    AccountId = item.Select(a => a.rawDetaile.rawSubLedger.rawLedgers.RawSubCategories.RawCategoryID).FirstOrDefault(),
                    InvestorId = item.Select(a => a.InvestorID).FirstOrDefault(),
                    InvestorName = item.Select(a => a.investorProfileInJournal.FirstName).FirstOrDefault() + " " + item.Select(a => a.investorProfileInJournal.LastName).FirstOrDefault(),
                    PersianDate = item.Select(a => a.EntryDate).LastOrDefault(),
                    GeorgianDate = item.Select(a => a.EntryGregorianDate).LastOrDefault(),
                    TotalAsset = totalAssets,
                    TotalPeofitOrLoss = totalProfit,
                    TotalReturn = totalReturn,
                };
                listGroupedByInvestorID.Add(model);
            }

            return listGroupedByInvestorID;
        }




    }
}
