using DLLCore.DBContext;
using DLLCore.DBContext.Entities.Accounting.Entry;
using DLLCore.Utility;
using DLLCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web.Mvc;
using DLLCore.Utility.Excel;
using DLLCore.Repositories.Detailes;
using DLLCore.Utility.Enums;
using DLLCore.ViewModels.FinancialStatmentsViewModels;
using System.Data.Entity.Core.Objects;
using DLLCore.DBContext.Entities.StockHistoy;

namespace DLLCore.Repositories
{
    public class JournalEntryRepository
    {
        private ApplicationDbContext db;
        InvestorRepository ipRepo;
        DetaileAccountsRepository rawDetailRepo;
        public List<string> ErrorList { get; set; }
        public JournalEntryRepository()
        {
            db = new ApplicationDbContext();
            ipRepo = new InvestorRepository();
            rawDetailRepo = new DetaileAccountsRepository();
        }

        public bool SaveCDsDataList(List<CDSExcelFormat> cdsList)
        {
            try
            {
                bool? ipSaveResult;
                bool? stockAccountSaveResult;
                bool? stockValuationAccountSaveResult;
                bool? brokerAccountSaveResult;
                bool? profitOFSellAccountSaveResult;

                //برای شناسایی و ذخیره سرمایه گذاران که وجود ندارند
                var ipNotExistListInDb = ipRepo.NotExistInvestorProfileList(cdsList);
                ipSaveResult = ipRepo.InsertInvestorProfileListFromEntityModel(ipNotExistListInDb);
                //برای شناسایی و ذخیره سهامی که وجود ندارند
                stockAccountSaveResult = rawDetailRepo.StockRawDetailNotExistListInDb(cdsList);

                //برای شناسایی و ذخیره حساب ارزشیابی سهامی که وجود ندارند
                stockValuationAccountSaveResult = rawDetailRepo.StockValuationRawDetailNotExistListInDb(cdsList);

                //برای شناسایی و ذخیره کارگزارانی که وجود ندارند
                brokerAccountSaveResult = rawDetailRepo.BrokerRawDetailNotExistListInDb(cdsList);

                //برای شناسایی و ذخیره حساب‌های فروش که وجود ندارند
                profitOFSellAccountSaveResult = rawDetailRepo.ProfitOfSellsRawDetailNotExistListInDb(cdsList);

                if (ipSaveResult == true && stockAccountSaveResult == true && stockValuationAccountSaveResult == true && brokerAccountSaveResult == true && profitOFSellAccountSaveResult == true)
                {
                    var buyCdsList = cdsList.Where(a => a.TradeType.Contains("خرید")).ToList();
                    var sellCdsList = cdsList.Where(a => a.TradeType.Contains("فروش")).ToList();
                    bool BuyEntryResult = false;
                    bool sellEntryResult = false;
                    foreach (var BuyItem in buyCdsList)
                    {
                        var buyEntries = PreparingBuyJournalEntries(BuyItem);
                        BuyEntryResult = InsertEntryListWithoutSavingIndb(buyEntries);
                        SaveInDb();
                    }
                    foreach (var sellItem in sellCdsList)
                    {
                        var sellEntries = PreparingSellJournalEntries(sellItem);
                        sellEntryResult = InsertEntryListWithoutSavingIndb(sellEntries);
                        SaveInDb();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                return false;
            }
        }
        public bool SaveValuationList(List<BalanceSheetViewModel> valuationList)
        {
            try
            {
                foreach (var tafsileItem in valuationList)
                {
                    var entryList = PreparingValuationJournalEntries(tafsileItem);
                    var EntryResult = InsertEntryListWithoutSavingIndb(entryList);
                    SaveInDb();
                }
                return true;
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                ErrorList.Add(mess);
                return false;
            }
        }
        public List<Journal> PreparingSellJournalEntries(CDSExcelFormat cds)
        {
            DateTime tarikhMiladi = PersionToJalali.ConvertToShortGregorian(cds.PersianDate);
            long lastSanad = EntryCodeGenerateIntRule(tarikhMiladi);
            var subLedgerIdOfStock = EnumClass.StockIdInSubLedger;
            long detailIdInJournal = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == cds.TafsilCode && a.SubLedgerID == subLedgerIdOfStock).Select(a => a.DetaileID).FirstOrDefault();

            long valuationDetialId = cds.TafsilCode;
            var subLedgerIdOfValuation = EnumClass.StockValuationIdInSubLedger;
            long valuationDetailIdInJournal = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == valuationDetialId && a.SubLedgerID == subLedgerIdOfValuation).Select(a => a.DetaileID).FirstOrDefault();


            if (detailIdInJournal != 0)
            {
                var valueBalanceOfStockInDb = db.JournalsTbl.Where(a => a.EntryGregorianDate <= tarikhMiladi && a.DetaileID == detailIdInJournal).Select(a => a.EntryDebit - a.EntryCredit).ToList()?.Sum() ?? 0;
                //مانده سود و زیان ارزشیابی سهام
                var ProfitAndLossBalanceOfStockInDb = db.JournalsTbl.Where(a => a.EntryGregorianDate <= tarikhMiladi && a.DetaileID == valuationDetailIdInJournal).Select(a => a.EntryDebit - a.EntryCredit).ToList()?.Sum() ?? 0;

                long volumeBalanceOfStockInDb = db.JournalsTbl.Where(a => a.EntryGregorianDate <= tarikhMiladi && a.DetaileID == detailIdInJournal).Select(a => a.EntryBuySellVolume).ToList()?.Sum() ?? 0;
                decimal avgPriceOfStock = Convert.ToDecimal((valueBalanceOfStockInDb + ProfitAndLossBalanceOfStockInDb) / volumeBalanceOfStockInDb);
                long TotalCost = Convert.ToInt64(avgPriceOfStock * cds.TradeVolume);
                long costOfStockAccountToThisTime = (valueBalanceOfStockInDb / (valueBalanceOfStockInDb + ProfitAndLossBalanceOfStockInDb)) * TotalCost;
                long costOfValuationAccountToThisTime = (ProfitAndLossBalanceOfStockInDb / (valueBalanceOfStockInDb + ProfitAndLossBalanceOfStockInDb)) * TotalCost;

                long volumeInCdsFactor = cds.TradeVolume;
                if (volumeInCdsFactor <= volumeBalanceOfStockInDb)
                {
                    List<Journal> SellJournalList = new List<Journal>();

                    //بررسی می کنیم ببینم سهم در حساب ارزشیابی نیز نیاز مانده دارد که ثبت کنیم یا خیر
                    if (costOfValuationAccountToThisTime != 0)
                    {
                        long StockValuationdetailId = rawDetailRepo.GetDetailAccountByStockValuationTafsileCode(cds.TafsilCode).DetaileID;
                        //ثبت سطر سند مربوط به ارزشیابی اوراق بهادار
                        Journal stockValuationEntryjournal = new Journal
                        {
                            EntryNo = lastSanad,
                            EntryGregorianDate = tarikhMiladi,
                            //کد تفصیل مشخصه کدی یک سهم است
                            DetaileID = StockValuationdetailId,
                            InvestorID = ipRepo.GetInvestorByInvestorCdsCode(cds.InvestorCodeInCds).InvestorID,
                            EntryDebit = costOfValuationAccountToThisTime > 0 ? 0 : costOfValuationAccountToThisTime,
                            EntryCredit = costOfValuationAccountToThisTime < 0 ? 0 : costOfValuationAccountToThisTime,
                            EntryDescription = $"{cds.TradeType} سهم {cds.FullNameOfStock} با نماد {cds.SymbolOfStock}، به تعداد {Math.Abs(cds.TradeVolume)} و قیمت {cds.TradePrice} نزد کارگزار {cds.BrokerName}",
                            FactorNumber = cds.FactorNumber,
                            EntryDate = cds.PersianDate,
                            BrokerFee = cds.BrokerFee,
                            TotalFee = cds.TotalFee,
                            BrokerCode = cds.BrokerCode,
                            BrokerName = cds.BrokerName,
                            EntryBuySellVolume = -cds.TradeVolume,
                            TradePrice = cds.TradePrice,
                            TradeTax = cds.TradeTax,
                            TradeType = cds.TradeType,
                        };
                        SellJournalList.Add(stockValuationEntryjournal);
                    }

                    long StockdetailId = rawDetailRepo.GetDetailAccountByStockTafsileCode(cds.TafsilCode).DetaileID;
                    //ثبت سطر سند مربوط به اوراق بهادار
                    Journal stockEntryjournal = new Journal
                    {
                        EntryNo = lastSanad,
                        EntryGregorianDate = tarikhMiladi,
                        //کد تفصیل مشخصه کدی یک سهم است
                        DetaileID = StockdetailId,
                        InvestorID = ipRepo.GetInvestorByInvestorCdsCode(cds.InvestorCodeInCds).InvestorID,
                        EntryDebit = 0,
                        EntryCredit = costOfStockAccountToThisTime,
                        EntryDescription = $"{cds.TradeType} سهم {cds.FullNameOfStock} با نماد {cds.SymbolOfStock}، به تعداد {Math.Abs(cds.TradeVolume)} و قیمت {cds.TradePrice} نزد کارگزار {cds.BrokerName}",
                        FactorNumber = cds.FactorNumber,
                        EntryDate = cds.PersianDate,
                        BrokerFee = cds.BrokerFee,
                        TotalFee = cds.TotalFee,
                        BrokerCode = cds.BrokerCode,
                        BrokerName = cds.BrokerName,
                        EntryBuySellVolume = -cds.TradeVolume,
                        TradePrice = cds.TradePrice,
                        TradeTax = cds.TradeTax,
                        TradeType = cds.TradeType,
                    };
                    SellJournalList.Add(stockEntryjournal);

                    long brokerDetailId = rawDetailRepo.GetDetailAccountByBrokerCode(cds.BrokerCode).DetaileID;
                    //ثبت سطر سند مربوط به کارگزار
                    Journal BrokerEntryjournal = new Journal
                    {
                        EntryNo = lastSanad,
                        EntryGregorianDate = tarikhMiladi,
                        //کد کارگزار مشخصه کدی یک کارگزار است
                        DetaileID = brokerDetailId,
                        InvestorID = ipRepo.GetInvestorByInvestorCdsCode(cds.InvestorCodeInCds).InvestorID,
                        EntryDebit = cds.TradeValue,
                        EntryCredit = 0,
                        EntryDescription = $"{cds.TradeType} سهم {cds.FullNameOfStock} با نماد {cds.SymbolOfStock}، به تعداد {Math.Abs(cds.TradeVolume)} و قیمت {cds.TradePrice} نزد کارگزار {cds.BrokerName}",
                        FactorNumber = cds.FactorNumber,
                        EntryDate = cds.PersianDate,
                        BrokerFee = cds.BrokerFee,
                        TotalFee = cds.TotalFee,
                        BrokerCode = cds.BrokerCode,
                        BrokerName = cds.BrokerName,
                        EntryBuySellVolume = -cds.TradeVolume,
                        TradePrice = cds.TradePrice,
                        TradeTax = cds.TradeTax,
                        TradeType = cds.TradeType,
                    };
                    SellJournalList.Add(BrokerEntryjournal);

                    long sellDetailId = rawDetailRepo.GetSellDetailAccountByCdsObject(cds).DetaileID;
                    //ثبت سطر سند مربوط به حساب سود حاصل از سرمایه‌گذاری
                    Journal SellEntryjournal = new Journal
                    {
                        EntryNo = lastSanad,
                        EntryGregorianDate = tarikhMiladi,
                        DetaileID = sellDetailId,
                        InvestorID = ipRepo.GetInvestorByInvestorCdsCode(cds.InvestorCodeInCds).InvestorID,
                        EntryDebit = cds.TradeValue - TotalCost < 0 ? TotalCost - cds.TradeValue : 0,
                        EntryCredit = cds.TradeValue - TotalCost >= 0 ? cds.TradeValue - TotalCost : 0,
                        EntryDescription = $"{cds.TradeType} سهم {cds.FullNameOfStock} با نماد {cds.SymbolOfStock}، به تعداد {Math.Abs(cds.TradeVolume)} و قیمت {cds.TradePrice} نزد کارگزار {cds.BrokerName}",
                        FactorNumber = cds.FactorNumber,
                        EntryDate = cds.PersianDate,
                        BrokerFee = cds.BrokerFee,
                        TotalFee = cds.TotalFee,
                        BrokerCode = cds.BrokerCode,
                        BrokerName = cds.BrokerName,
                        EntryBuySellVolume = -cds.TradeVolume,
                        TradePrice = cds.TradePrice,
                        TradeTax = cds.TradeTax,
                        TradeType = cds.TradeType,
                    };
                    SellJournalList.Add(SellEntryjournal);
                    return SellJournalList;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public List<Journal> PreparingBuyJournalEntries(CDSExcelFormat cds)
        {
            List<Journal> buyJournalList = new List<Journal>();
            var tarikhMiladi = PersionToJalali.ConvertToShortGregorian(cds.PersianDate);
            var lastSanad = EntryCodeGenerateIntRule(tarikhMiladi);
            var StockdetailId = rawDetailRepo.GetDetailAccountByStockTafsileCode(cds.TafsilCode).DetaileID;
            Journal StockDebitjournal = new Journal
            {
                EntryNo = lastSanad,
                EntryGregorianDate = tarikhMiladi,
                //کد تفصیل مشخصه کدی یک سهم است
                DetaileID = StockdetailId,
                InvestorID = ipRepo.GetInvestorByInvestorCdsCode(cds.InvestorCodeInCds).InvestorID,
                EntryDebit = cds.TradeValue,
                EntryCredit = 0,
                EntryDescription = $"{cds.TradeType} سهم {cds.FullNameOfStock} با نماد {cds.SymbolOfStock}، به تعداد {Math.Abs(cds.TradeVolume)} و قیمت {cds.TradePrice} نزد کارگزار {cds.BrokerName}",
                FactorNumber = cds.FactorNumber,
                EntryDate = cds.PersianDate,
                BrokerFee = cds.BrokerFee,
                TotalFee = cds.TotalFee,
                BrokerCode = cds.BrokerCode,
                BrokerName = cds.BrokerName,
                EntryBuySellVolume = cds.TradeVolume,
                TradePrice = cds.TradePrice,
                TradeTax = cds.TradeTax,
                TradeType = cds.TradeType,
            };
            buyJournalList.Add(StockDebitjournal);

            var brokerDetailId = rawDetailRepo.GetDetailAccountByBrokerCode(cds.BrokerCode).DetaileID;
            Journal BrokerCreditjournal = new Journal
            {
                EntryNo = lastSanad,
                EntryGregorianDate = tarikhMiladi,
                //کد کارگزار مشخصه کدی یک کارگزار است
                DetaileID = brokerDetailId,
                InvestorID = ipRepo.GetInvestorByInvestorCdsCode(cds.InvestorCodeInCds).InvestorID,
                EntryDebit = 0,
                EntryCredit = cds.TradeValue,
                EntryDescription = $"{cds.TradeType} سهم {cds.FullNameOfStock} با نماد {cds.SymbolOfStock}، به تعداد {Math.Abs(cds.TradeVolume)} و قیمت {cds.TradePrice} نزد کارگزار {cds.BrokerName}",
                FactorNumber = cds.FactorNumber,
                EntryDate = cds.PersianDate,
                BrokerFee = cds.BrokerFee,
                TotalFee = cds.TotalFee,
                BrokerCode = cds.BrokerCode,
                BrokerName = cds.BrokerName,
                EntryBuySellVolume = cds.TradeVolume,
                TradePrice = cds.TradePrice,
                TradeTax = cds.TradeTax,
                TradeType = cds.TradeType,
            };
            buyJournalList.Add(BrokerCreditjournal);
            return buyJournalList;
        }

        public List<Journal> PreparingValuationJournalEntries(BalanceSheetViewModel item)
        {
            try
            {
                List<Journal> journalList = new List<Journal>();
                long tafsilcodeInCds = item.StockTafsileInCds;
                var listofStock = db.SecurityTbl.Where(a => a.TafsilCodeInCds == tafsilcodeInCds).ToList();
                if (listofStock.Count != 0)
                {
                    var subLedgerIdOfItem = Convert.ToInt32(item.SubLedgerId);
                    var stockSubLedgerIdInEnum = EnumClass.StockIdInSubLedger;
                    var stockPrecedenceSubLedgerIdInEnum = EnumClass.StockPrecedenceIdInSubLedger;
                    //برای سهام
                    if (subLedgerIdOfItem == stockSubLedgerIdInEnum)
                    {
                        int StockValuationSubLedgerId = EnumClass.StockValuationIdInSubLedger;
                        journalList = ValuationEntryBind(item, StockValuationSubLedgerId, listofStock);
                    }
                    //برای حق تقدم
                    if (subLedgerIdOfItem== stockPrecedenceSubLedgerIdInEnum)
                    {
                        int StockPrecedenceValuationSubLedgerId = EnumClass.StockPrecedenceValuationIdInSubLedger;
                        journalList = ValuationEntryBind(item, StockPrecedenceValuationSubLedgerId, listofStock);
                    }
                    //در آینده برای اوراق با درآمد ثابت و غیره
                }
                listofStock.Clear();
                return journalList;
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                ErrorList.Add(mess);
                return null;
            }
        }
        public List<Journal> ValuationEntryBind(BalanceSheetViewModel balanceSheetViewitem, int valuationSubledgerId,List<SecurityHistory> listofStock)
        {
            List<Journal> journalList = new List<Journal>();
            long ValuationAccountTafsilCode = balanceSheetViewitem.StockTafsileInCds;
            var ValuationdetailId = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == ValuationAccountTafsilCode && a.SubLedgerID == valuationSubledgerId).Select(a => a.DetaileID).FirstOrDefault();
            if (ValuationAccountTafsilCode != 0)
            {
                var updateLastPrice = listofStock.Select(a => a.LastPrice).LastOrDefault();
                var todayValue = balanceSheetViewitem.VolumeBalance * Convert.ToInt64(updateLastPrice);
                var beforeStockValue = balanceSheetViewitem.TotalBalance;
                var profitOrLose = todayValue - beforeStockValue;
                //اگر نیاز به ثبت بود، ثبت انجام شود، اگر سود یا زیان صفر باشد، نیازی به تعدیل نداریم
                if (profitOrLose != 0)
                {
                    string symboleName = balanceSheetViewitem.AccountSymboleName;
                    var investorId = balanceSheetViewitem.InvestorId;
                    DateTime LastGeorgianDate = (DateTime)(listofStock.Select(a => a.TradeDateGre).LastOrDefault());
                    var LastPersianDate = listofStock.Select(a => a.TradeDate).LastOrDefault();
                    var lastSanad = EntryCodeGenerateIntRule(LastGeorgianDate);
                    Journal InvestmentValuationEntry = new Journal
                    {
                        EntryNo = lastSanad,
                        EntryGregorianDate = LastGeorgianDate,
                        EntryDate = LastPersianDate,
                        DetaileID = ValuationdetailId,
                        InvestorID = investorId,
                        EntryDebit = profitOrLose > 0 ? profitOrLose : 0,
                        EntryCredit = profitOrLose < 0 ? Math.Abs(profitOrLose) : 0,
                        EntryDescription = $"ارزشیابی ورقه بهادار با نماد {symboleName} در تاریخ {LastPersianDate} با قیمت جدید {updateLastPrice} انجام شد.",
                        FactorNumber = 0,
                        BrokerFee = 0,
                        TotalFee = 0,
                        BrokerCode = 0,
                        BrokerName = " ",
                        EntryBuySellVolume = 0,
                        TradePrice = 0,
                        TradeTax = 0,
                        TradeType = "ارزشیابی",
                    };
                    journalList.Add(InvestmentValuationEntry);

                    var profitAndLossSubLedgerId = EnumClass.StockProfitIdInSubLedger;
                    var ProfitAccountdetailId = db.RawDetailesTbl.Where(a => a.SubLedgerID == profitAndLossSubLedgerId).Where(a => a.TafsilCodeInCDS == ValuationAccountTafsilCode).Select(a => a.DetaileID).FirstOrDefault();
                    Journal ValuationEntry = new Journal
                    {
                        EntryNo = lastSanad,
                        EntryGregorianDate = LastGeorgianDate,
                        EntryDate = LastPersianDate,
                        DetaileID = ProfitAccountdetailId,
                        InvestorID = investorId,
                        EntryDebit = profitOrLose < 0 ? Math.Abs(profitOrLose) : 0,
                        EntryCredit = profitOrLose > 0 ? profitOrLose : 0,
                        EntryDescription = $"سود (زیان) ارزشیابی ورقه بهادار با نماد {symboleName} در تاریخ {LastPersianDate} با قیمت جدید {updateLastPrice} انجام شد.",
                        FactorNumber = 0,
                        BrokerFee = 0,
                        TotalFee = 0,
                        BrokerCode = 0,
                        BrokerName = " ",
                        EntryBuySellVolume = 0,
                        TradePrice = 0,
                        TradeTax = 0,
                        TradeType = "ارزشیابی",
                    };
                    journalList.Add(ValuationEntry);
                }
            }
            return journalList;
        }

        public async Task<IEnumerable<Journal>> GetAllIncludeIPsAndRawDetails()
        {
            try
            {
                var allIncludeRawDetailes = await db.JournalsTbl.Include(j => j.rawDetaile).Include(a => a.investorProfileInJournal).ToListAsync();
                return allIncludeRawDetailes;
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                ErrorList.Add(mess);
                return null;
            }
        }
        public async Task<IEnumerable<Journal>> GetAllIncludeIPsAndRawDetails(string fromDate, string toDate)
        {
            try
            {
                var startDate = PersionToJalali.ConvertToGregorian(fromDate);
                var endDate = PersionToJalali.ConvertToGregorian(toDate).AddSeconds(2);
                var dat = db.JournalsTbl.Select(a => a.EntryGregorianDate).FirstOrDefault();
                var allIncludeRawDetailes = await db.JournalsTbl.Include(j => j.rawDetaile).Include(a => a.investorProfileInJournal).Where(a => a.EntryGregorianDate >= startDate && a.EntryGregorianDate <= endDate).ToListAsync();


                return allIncludeRawDetailes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<Journal>> GetAllIncludeIPsAndRawDetailsByEntryCode(long? id)
        {
            try
            {
                var allIncludeRawDetailes = await db.JournalsTbl.Where(a => a.EntryNo == id).Include(j => j.rawDetaile).Include(a => a.investorProfileInJournal).ToListAsync();


                return allIncludeRawDetailes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool InsertEntry(Journal entry)
        {
            try
            {
                db.JournalsTbl.Add(entry);
                SaveInDb();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertEntryList(List<Journal> journalsList)
        {
            try
            {
                db.JournalsTbl.AddRange(journalsList);
                SaveInDb();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertEntryListWithoutSavingIndb(List<Journal> journalsList)
        {
            try
            {
                db.JournalsTbl.AddRange(journalsList);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditEntryList(List<Journal> journalsList)
        {
            try
            {
                for (int i = 0; i < journalsList.Count; i++)
                {
                    var editItem = journalsList.ElementAt(i);
                    var detailAccountId = journalsList.Select(a => a.DetaileID).ElementAt(i);
                    var entryNo = journalsList.Select(a => a.EntryNo).ElementAt(i);
                    var entryEntityList = db.JournalsTbl.Where(a => a.EntryNo == entryNo && a.DetaileID == detailAccountId).OrderBy(a => a.EntryID).ToList();
                    var entryobj = new Journal();
                    if (entryEntityList.Count == 1)
                    {
                        entryobj = entryEntityList.FirstOrDefault();
                    }
                    else
                    {
                        entryobj = entryEntityList.ElementAt(i);
                    }
                    var entryID = entryobj.EntryID;
                    if (entryobj != null)
                    {
                        entryobj = editItem;
                        entryobj.EntryID = entryID;
                        var local = db.Set<Journal>()
                         .Local
                         .FirstOrDefault(f => f.EntryID == entryobj.EntryID);
                        if (local != null)
                        {
                            db.Entry(local).State = EntityState.Detached;
                        }
                        db.Entry(entryobj).State = EntityState.Modified;
                    }
                    else
                    {
                        return false;
                    }
                }
                SaveInDb();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteEntryList(List<Journal> journalsList)
        {
            try
            {
                db.JournalsTbl.RemoveRange(journalsList);
                SaveInDb();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public Journal BindViewModelToJournalEntry(NormalEntryViewModel vm)
        {
            try
            {
                var persianDate = vm.EntryDate;
                var tarikhMiladi = PersionToJalali.DynamicPersianToGregorian(persianDate);

                var lastSanad = EntryCodeGenerateIntRule(tarikhMiladi);
                Journal myEntry = new Journal()
                {
                    EntryNo = lastSanad,
                    EntryGregorianDate = tarikhMiladi,
                    DetaileID = vm.DetaileID,
                    EntryDescription = vm.EntryDescription,
                    EntryDebit = vm.EntryDebit,
                    EntryCredit = vm.EntryCredit,
                    EntryDate = vm.EntryDate,
                    InvestorID = vm.InvestorID,
                    EntryBuySellVolume = vm.EntryBuySellVolume,
                    BrokerFee = vm.BrokerFee,
                    TotalFee = vm.TotalFee,
                    FactorNumber = vm.FactorNumber,
                    TradePrice = vm.TradePrice,
                    TradeTax = vm.TradeTax,
                    TradeType = vm.TradeType,
                    BrokerName = vm.BrokerName,
                    BrokerCode = vm.BrokerCode,
                };
                return myEntry;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Journal> BindVmListToJournalListEntry(List<NormalEntryViewModel> vmList)
        {
            try
            {
                var persianDate = vmList.Select(a => a.EntryDate).FirstOrDefault();
                var tarikhMiladi = PersionToJalali.DynamicPersianToGregorian(persianDate);
                var lastSanad = EntryCodeGenerateIntRule(tarikhMiladi);
                List<Journal> journalList = new List<Journal>();
                if (lastSanad > 0)
                {
                    foreach (var item in vmList)
                    {
                        Journal myEntry = new Journal()
                        {
                            EntryGregorianDate = tarikhMiladi,
                            EntryNo = lastSanad,
                            DetaileID = item.DetaileID,
                            EntryDescription = item.EntryDescription,
                            EntryDebit = item.EntryDebit,
                            EntryCredit = item.EntryCredit,
                            EntryDate = item.EntryDate,
                            InvestorID = item.InvestorID,
                            EntryBuySellVolume = item.EntryBuySellVolume,
                            BrokerFee = item.BrokerFee,
                            TotalFee = item.TotalFee,
                            FactorNumber = item.FactorNumber,
                            TradePrice = item.TradePrice,
                            TradeTax = item.TradeTax,
                            TradeType = item.TradeType,
                            BrokerName = item.BrokerName,
                            BrokerCode = item.BrokerCode,
                        };
                        journalList.Add(myEntry);
                    }
                }
                return journalList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Journal> BindVmListToJournalListEntryForEdit(List<NormalEntryViewModel> vmList)
        {
            try
            {
                var persianDate = vmList.Select(a => a.EntryDate).FirstOrDefault();
                var tarikhMiladi = PersionToJalali.DynamicPersianToGregorian(persianDate);

                List<Journal> journalList = new List<Journal>();
                foreach (var item in vmList)
                {
                    Journal myEntry = new Journal()
                    {
                        EntryGregorianDate = tarikhMiladi,
                        EntryNo = item.EntryNo,
                        DetaileID = item.DetaileID,
                        EntryDescription = item.EntryDescription,
                        EntryDebit = item.EntryDebit,
                        EntryCredit = item.EntryCredit,
                        EntryDate = item.EntryDate,
                        InvestorID = item.InvestorID,
                        EntryBuySellVolume = item.EntryBuySellVolume,
                        BrokerFee = item.BrokerFee,
                        TotalFee = item.TotalFee,
                        FactorNumber = item.FactorNumber,
                        TradePrice = item.TradePrice,
                        TradeTax = item.TradeTax,
                        TradeType = item.TradeType,
                        BrokerName = item.BrokerName,
                        BrokerCode = item.BrokerCode,
                    };
                    journalList.Add(myEntry);
                }
                return journalList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NormalEntryViewModel> BindJournalListToJoVmList(List<Journal> jourList)
        {
            try
            {
                var persianDate = jourList.Select(a => a.EntryDate).FirstOrDefault();
                if (persianDate.Length <= 10)
                {
                    var tarikhMiladi = PersionToJalali.ConvertToShortGregorian(persianDate);
                }
                else
                {
                    var tarikhMiladi = PersionToJalali.ConvertToGregorian(persianDate);
                }
                List<NormalEntryViewModel> journalList = new List<NormalEntryViewModel>();
                foreach (var item in jourList)
                {
                    NormalEntryViewModel myEntry = new NormalEntryViewModel()
                    {
                        DetaileID = item.DetaileID,
                        EntryDescription = item.EntryDescription,
                        EntryDebit = item.EntryDebit,
                        EntryCredit = item.EntryCredit,
                        EntryDate = item.EntryDate,
                        EntryBuySellVolume = item.EntryBuySellVolume,
                        InvestorID = item.InvestorID,
                        EntryNo = item.EntryNo,
                        DetailesNames =
                        new SelectListItem
                        {
                            Text = db.RawDetailesTbl.Where(a => a.DetaileID == item.DetaileID).Select(a => a.DetaileName).FirstOrDefault(),
                            Value = db.RawDetailesTbl.Where(a => a.DetaileID == item.DetaileID).Select(a => a.DetaileID.ToString()).FirstOrDefault(),
                            Selected = false,
                        },
                        BrokerFee = item.BrokerFee,
                        TotalFee = item.TotalFee,
                        FactorNumber = item.FactorNumber,
                        TradePrice = item.TradePrice,
                        TradeTax = item.TradeTax,
                        TradeType = item.TradeType,
                        BrokerName = item.BrokerName,
                        BrokerCode = item.BrokerCode,
                    };
                    journalList.Add(myEntry);
                }
                return journalList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //برای دریافت اطلاعات فرم‌ها و مدل کردن آن
        public List<NormalEntryViewModel> BindFormCollectionForInsert(FormCollection formCollectionData)
        {
            try
            {
                Dictionary<string, string> KeyValue = new Dictionary<string, string>();
                foreach (var item in formCollectionData)
                {
                    string key = item.ToString();
                    string value = formCollectionData[key];
                    KeyValue.Add(key, value);
                }
                var entryDate = KeyValue.Values.ElementAt(1).ToString();
                var investorId = KeyValue.Values.ElementAt(2).ToString();

                var countofKv = KeyValue.Count - 3;
                var colNum = countofKv / 4;
                List<NormalEntryViewModel> vmList = new List<NormalEntryViewModel>();
                var j = 3;
                for (int i = 0; i < colNum; i++)
                {
                    NormalEntryViewModel vm = new NormalEntryViewModel();
                    vm.EntryDate = entryDate;
                    vm.InvestorID = int.Parse(investorId);
                    vm.DetaileID = int.Parse(KeyValue.Values.ElementAt(j));
                    j++;
                    vm.EntryDescription = KeyValue.Values.ElementAt(j);
                    j++;
                    vm.EntryDebit = Convert.ToInt64(NumberConvert.ConvertPersianToNumber(KeyValue.Values.ElementAt(j)));
                    j++;
                    vm.EntryCredit = Convert.ToInt64(NumberConvert.ConvertPersianToNumber(KeyValue.Values.ElementAt(j)));
                    j++;
                    vmList.Add(vm);
                }
                return vmList;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<NormalEntryViewModel> BindFormCollectionForEdit(FormCollection formCollectionData)
        {
            try
            {
                Dictionary<string, string> KeyValue = new Dictionary<string, string>();
                foreach (var item in formCollectionData)
                {
                    string key = item.ToString();
                    string value = formCollectionData[key];
                    KeyValue.Add(key, value);
                }
                var entryDate = KeyValue.Values.ElementAt(1).ToString();
                var investorId = KeyValue.Values.ElementAt(2).ToString();
                var entryCode = KeyValue.Values.ElementAt(3).Split(',').ElementAt(0);

                var countofKv = KeyValue.Count - 4;
                var colNum = countofKv / 4;
                List<NormalEntryViewModel> vmList = new List<NormalEntryViewModel>();
                var j = 4;
                for (int i = 0; i < colNum; i++)
                {
                    NormalEntryViewModel vm = new NormalEntryViewModel();
                    vm.EntryDate = entryDate;
                    vm.InvestorID = int.Parse(investorId);
                    vm.EntryNo = Convert.ToInt64(entryCode);
                    vm.DetaileID = int.Parse(KeyValue.Values.ElementAt(j));
                    j++;
                    vm.EntryDescription = KeyValue.Values.ElementAt(j);
                    j++;
                    vm.EntryDebit = Convert.ToInt64(NumberConvert.ConvertPersianToNumber(KeyValue.Values.ElementAt(j)));
                    j++;
                    vm.EntryCredit = Convert.ToInt64(NumberConvert.ConvertPersianToNumber(KeyValue.Values.ElementAt(j)));
                    j++;
                    vmList.Add(vm);
                }
                return vmList;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //برای ایجاد کند سند
        public long EntryCodeGenerateIntRule(DateTime tarikhMiladi)
        {
            Int64 lastSanad = 0;
            var LastCodesanadBeforeAz = db.JournalsTbl.Where(a => a.EntryGregorianDate < tarikhMiladi).Select(a => a.EntryNo).ToList();
            var LastCodesanadAfter = db.JournalsTbl.Where(a => a.EntryGregorianDate >= tarikhMiladi).Select(a => a.EntryNo).ToList();
            if (LastCodesanadBeforeAz.Count == 0 && LastCodesanadAfter.Count == 0)
            {
                lastSanad = 1;
            }
            else
            {
                var max = db.JournalsTbl.Select(a => a.EntryNo).Max();
                lastSanad = Convert.ToInt64(max + 1);

            }
            return lastSanad;
        }
        public long EntryCodeGenerateDecimalRule(DateTime tarikhMiladi)
        {
            Int64 lastSanad = 0;
            var LastIdsanadGhablAz = db.JournalsTbl.Where(a => a.EntryGregorianDate < tarikhMiladi).Select(a => a.EntryNo).ToList();
            var LastIdsanadBaedAz = db.JournalsTbl.Where(a => a.EntryGregorianDate >= tarikhMiladi).Select(a => a.EntryNo).ToList();
            if (LastIdsanadGhablAz.Count == 0 && LastIdsanadBaedAz.Count == 0)
            {
                lastSanad = 1;
            }
            else if (LastIdsanadGhablAz.Count == 0 && LastIdsanadBaedAz.Count > 0)
            {
                LastIdsanadGhablAz = db.JournalsTbl.Where(a => a.EntryGregorianDate <= tarikhMiladi).Select(a => a.EntryNo).ToList();
                LastIdsanadBaedAz = db.JournalsTbl.Where(a => a.EntryGregorianDate >= tarikhMiladi && a.EntryNo > 0).Select(a => a.EntryNo).ToList();
                if (LastIdsanadGhablAz.Count == 0)
                {
                    lastSanad = LastIdsanadBaedAz.Min() / 2;
                }
                else
                {
                    lastSanad = LastIdsanadGhablAz.Min() / 2;
                }
            }
            else if (LastIdsanadGhablAz.Count > 0 && LastIdsanadBaedAz.Count == 0)
            {
                LastIdsanadGhablAz = db.JournalsTbl.Where(a => a.EntryGregorianDate <= tarikhMiladi).Select(a => a.EntryNo).ToList();
                lastSanad = Convert.ToInt64(LastIdsanadGhablAz.Max() + 1);
            }
            else if (LastIdsanadGhablAz.Count > 0 && LastIdsanadBaedAz.Count > 0)
            {
                var MyBeforMaxId = db.JournalsTbl.Where(a => a.EntryGregorianDate < tarikhMiladi).Select(a => a.EntryNo).Max();
                var MyAfterMinId = db.JournalsTbl.Where(a => a.EntryGregorianDate >= tarikhMiladi).Select(a => a.EntryNo).Min();
                lastSanad = (MyBeforMaxId + MyAfterMinId) / 2;
            }
            return lastSanad;
        }

        public bool SaveInDb()
        {
            db.SaveChanges();
            return true;
        }
        public void DisposeDb()
        {
            db.Dispose();
        }
    }
}
