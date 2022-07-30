using DLLCore.DBContext;
using DLLCore.DBContext.Entities.Accounting.RawAccounts;
using DLLCore.Utility.Enums;
using DLLCore.Utility.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.Repositories.Detailes
{
    public class DetaileAccountsRepository
    {
        ApplicationDbContext db;
        public DetaileAccountsRepository()
        {
            db = new ApplicationDbContext();
        }

        public bool InsertRawDetailFromEntityModel(List<RawDetaile> rawDetaileList)
        {
            try
            {
                List<RawDetaile> rawsList = new List<RawDetaile>();
                foreach (var item in rawDetaileList)
                {
                    rawsList = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == item.TafsilCodeInCDS).ToList();
                    int? count = rawsList.Count();
                    if (count == 0 || rawsList == null)
                    {
                        db.RawDetailesTbl.Add(item);
                        SaveInDb();
                        rawsList.Clear();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        //متد ویژه کارگزاران
        public bool InsertBrokerRawDetailFromEntityModel(List<RawDetaile> rawDetaileList)
        {
            try
            {
                List<RawDetaile> rawsList = new List<RawDetaile>();
                foreach (var item in rawDetaileList)
                {
                    if (item.SubLedgerID == 28)
                    {
                        rawsList = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == item.TafsilCodeInCDS && a.SubLedgerID == 28).ToList();
                        int? count = rawsList.Count();
                        if (count == 0 || rawsList == null)
                        {
                            db.RawDetailesTbl.Add(item);
                            SaveInDb();
                            rawsList.Clear();
                        }
                    }
                    if (item.SubLedgerID == 32)
                    {
                        rawsList = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == item.TafsilCodeInCDS && a.SubLedgerID == 32).ToList();
                        int? count = rawsList.Count();
                        if (count == 0 || rawsList == null)
                        {
                            db.RawDetailesTbl.Add(item);
                            SaveInDb();
                            rawsList.Clear();
                        }
                    }

                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        //برای سهامی که در دیتابیس و جدول مربوطه وجود ندارند
        public bool StockRawDetailNotExistListInDb(List<CDSExcelFormat> cDsList)
        {
            try
            {
                List<RawDetaile> notExistListInRawDetailTbl = new List<RawDetaile>();
                var distictStocks = cDsList.Select(a => a.TafsilCode).Distinct();
                foreach (var item in distictStocks)
                {
                    var StocktafsilCode = item;
                    var isprecedence = cDsList.Where(a => a.TafsilCode == StocktafsilCode).Select(a => a.IsPrecedence).FirstOrDefault();
                    var IsExistStockAccount = IsStockAccountExistsInRawDetailsTbl(StocktafsilCode, isprecedence);
                    if (IsExistStockAccount == false)
                    {
                        var bindingItem = cDsList.Where(a => a.TafsilCode == StocktafsilCode).FirstOrDefault();
                        var newStockrawDetail = BindStockRawDetaileDefualtInformation(bindingItem, StocktafsilCode);
                        InsertRawDetailFromEntityModel(newStockrawDetail);
                        notExistListInRawDetailTbl.AddRange(newStockrawDetail);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool IsStockAccountExistsInRawDetailsTbl(long tafsilCode, bool isPrecedence)
        {
            if (isPrecedence == false)
            {
                int subLedgerIdOfStock = EnumClass.StockIdInSubLedger;
                var StockCdsTafsilCodeInDB = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == tafsilCode && a.SubLedgerID == subLedgerIdOfStock);
                int? count = StockCdsTafsilCodeInDB.Count();
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (isPrecedence == true)
            {
                int subLedgerIdOfPrecedenceStock = EnumClass.StockPrecedenceIdInSubLedger;
                var StockCdsTafsilCodeInDB = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == tafsilCode && a.SubLedgerID == subLedgerIdOfPrecedenceStock);
                int? count = StockCdsTafsilCodeInDB.Count();
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        public List<RawDetaile> BindStockRawDetaileDefualtInformation(CDSExcelFormat cdsData, long tafsilCode)
        {
            List<RawDetaile> rawList = new List<RawDetaile>();

            if (cdsData.IsPrecedence == false)
            {
                //نکته: اگر در آینده فایل فناوری شامل فیلدی شود که بتوان نوع درآمد ثابت را تشخیص داد، خوب خواهد بود که یک کد دیگر نیز برای آن ایجاد شود
                //عدد 2 نشان دهنده کد اوراق بهادار در دفتر معین است است
                int stockSubledgerId = EnumClass.StockIdInSubLedger;

                var subLedgerIdForSecurities = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == stockSubledgerId).Select(a => a.SubLedgerID).FirstOrDefault();
                var subLedgerCodeForSecurities = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == stockSubledgerId).Select(a => a.SubLedgerCode).FirstOrDefault();
                var rawDetailList = db.RawDetailesTbl.Where(a => a.SubLedgerID == subLedgerIdForSecurities).Select(a => a.DetaileCode).ToList();
                long maxCode;
                if (rawDetailList.Count == 0)
                {
                    //ایجاد کد در دفتر تفصیل در صورتی که قبلاً وجود نداشته باشد
                    maxCode = Convert.ToInt64(subLedgerCodeForSecurities + EnumClass.StarterCounter.ToString());
                }
                else
                {
                    maxCode = rawDetailList.Max() + 1;
                }
                RawDetaile ShortTermInvestRawDetaile = new RawDetaile
                {
                    //خیلی مهم برای ایجاد کد سرمایه‌گذاری کوتاه‌مدت در اوراق بهادار
                    SubLedgerID = subLedgerIdForSecurities,
                    DetaileCode = maxCode,
                    DetaileName = cdsData.FullNameOfStock,
                    DetaileSymbole = cdsData.SymbolOfStock,
                    IsMarketMakerContrary = false,
                    IsPrecedence = false,
                    TafsilCodeInCDS = tafsilCode,
                };
                rawList.Add(ShortTermInvestRawDetaile);
            }
            if (cdsData.IsPrecedence == true)
            {
                //عدد 4 نشان دهنده کد  حق تقدم در دفتر معین است
                int stockPrecedenceSubledgerId = EnumClass.StockPrecedenceIdInSubLedger;
                var subLedgerIdForSecurities = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == stockPrecedenceSubledgerId).Select(a => a.SubLedgerID).FirstOrDefault();
                var subLedgerCodeForSecurities = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == 4).Select(a => a.SubLedgerCode).FirstOrDefault();
                var rawDetailList = db.RawDetailesTbl.Where(a => a.SubLedgerID == subLedgerIdForSecurities).Select(a => a.DetaileCode).ToList();
                long maxCode;
                if (rawDetailList.Count == 0)
                {
                    //ایجاد کد در دفتر تفصیل در صورتی که قبلاً وجود نداشته باشد
                    maxCode = Convert.ToInt64(subLedgerCodeForSecurities + EnumClass.StarterCounter.ToString());
                }
                else
                {
                    maxCode = rawDetailList.Max() + 1;
                }
                RawDetaile ShortTermInvestRawDetaile = new RawDetaile
                {
                    SubLedgerID = subLedgerIdForSecurities,
                    DetaileCode = maxCode,
                    DetaileName = cdsData.FullNameOfStock,
                    DetaileSymbole = cdsData.SymbolOfStock,
                    IsMarketMakerContrary = false,
                    IsPrecedence = true,
                    TafsilCodeInCDS = tafsilCode,
                };
                rawList.Add(ShortTermInvestRawDetaile);
            }

            return rawList;
        }


        //برای حساب ارزش‌گذاری سهام موجود
        public bool StockValuationRawDetailNotExistListInDb(List<CDSExcelFormat> cDsList)
        {
            try
            {
                List<RawDetaile> notExistListInRawDetailTbl = new List<RawDetaile>();
                var distictStocks = cDsList.Select(a => a.TafsilCode).Distinct().ToList();

                foreach (var item in distictStocks)
                {
                    var StockValuationtafsilCode = item;
                    var isprecedence = cDsList.Where(a => a.TafsilCode == StockValuationtafsilCode).Select(a => a.IsPrecedence).FirstOrDefault();
                    var IsExistValuationStockAccount = IsValuationAccountExistsInRawDetailsTbl(StockValuationtafsilCode, isprecedence);
                    if (IsExistValuationStockAccount == false)
                    {
                        var bindingItem = cDsList.Where(a => a.TafsilCode == StockValuationtafsilCode).FirstOrDefault();
                        var newStockValuationrawDetail = BindValuationStockRawDetaileDefualtInformation(bindingItem, StockValuationtafsilCode);
                        db.RawDetailesTbl.AddRange(newStockValuationrawDetail);
                        SaveInDb();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool IsValuationAccountExistsInRawDetailsTbl(long tafsilCode, bool isPrecedence)
        {
            if (isPrecedence == false)
            {
                long valuationTafsilCode = tafsilCode;
                int subLedgerIdOfStock = EnumClass.StockValuationIdInSubLedger;
                var StockCdsTafsilCodeInDB = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == valuationTafsilCode && a.SubLedgerID == subLedgerIdOfStock);
                int count = StockCdsTafsilCodeInDB?.Count() ?? 0;
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (isPrecedence == true)
            {
                long valuationTafsilCode = tafsilCode;
                int subLedgerIdOfStock = EnumClass.StockPrecedenceIdInSubLedger;
                var StockCdsTafsilCodeInDB = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == valuationTafsilCode && a.SubLedgerID == subLedgerIdOfStock);
                int count = StockCdsTafsilCodeInDB?.Count() ?? 0;
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        public List<RawDetaile> BindValuationStockRawDetaileDefualtInformation(CDSExcelFormat cdsData, long tafsilCode)
        {
            List<RawDetaile> rawList = new List<RawDetaile>();
            long newTafsilCode = tafsilCode;
            if (cdsData.IsPrecedence == false)
            {
                //نکته: اگر در آینده فایل فناوری شامل فیلدی شود که بتوان نوع درآمد ثابت را تشخیص داد، خوب خواهد بود که یک کد دیگر نیز برای آن ایجاد شود
                //عدد 3 نشان دهنده کد ارزشیابی اوراق بهادار در دفتر معین است است
                var StockvaluationSubLedgerId = EnumClass.StockValuationIdInSubLedger;
                var subLedgerIdForSecurities = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == StockvaluationSubLedgerId).Select(a => a.SubLedgerID).FirstOrDefault();
                var subLedgerCodeForSecurities = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == StockvaluationSubLedgerId).Select(a => a.SubLedgerCode).FirstOrDefault();
                var rawDetailList = db.RawDetailesTbl.Where(a => a.SubLedgerID == subLedgerIdForSecurities).Select(a => a.DetaileCode).ToList();
                long maxCode;
                if (rawDetailList.Count == 0)
                {
                    //ایجاد کد در دفتر تفصیل در صورتی که قبلاً وجود نداشته باشد
                    string starter = EnumClass.StarterCounter.ToString();
                    maxCode = Convert.ToInt64(subLedgerCodeForSecurities + starter);
                }
                else
                {
                    long starter = EnumClass.StarterCounter;
                    maxCode = rawDetailList.Max() + starter;
                }
                RawDetaile ShortTermValuationInvestRawDetaile = new RawDetaile
                {
                    //خیلی مهم برای ایجاد کد ارزشیابی سرمایه‌گذاری کوتاه‌مدت در اوراق بهادار
                    SubLedgerID = subLedgerIdForSecurities,
                    DetaileCode = maxCode,
                    DetaileName = "ارزشیابی " + cdsData.FullNameOfStock,
                    DetaileSymbole = cdsData.SymbolOfStock,
                    IsMarketMakerContrary = false,
                    IsPrecedence = false,
                    TafsilCodeInCDS = newTafsilCode,
                };
                rawList.Add(ShortTermValuationInvestRawDetaile);
            }
            if (cdsData.IsPrecedence == true)
            {
                //عدد 5 نشان دهنده کد ارزشیابی حق تقدم در دفتر معین است
                var StockvaluationSubLedgerId = EnumClass.StockPrecedenceValuationIdInSubLedger;
                var subLedgerIdForSecurities = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == StockvaluationSubLedgerId).Select(a => a.SubLedgerID).FirstOrDefault();
                var subLedgerCodeForSecurities = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == StockvaluationSubLedgerId).Select(a => a.SubLedgerCode).FirstOrDefault();
                var rawDetailList = db.RawDetailesTbl.Where(a => a.SubLedgerID == subLedgerIdForSecurities).Select(a => a.DetaileCode).ToList();
                long maxCode;
                if (rawDetailList.Count == 0)
                {
                    //ایجاد کد در دفتر تفصیل در صورتی که قبلاً وجود نداشته باشد
                    string starter = EnumClass.StarterCounter.ToString();
                    maxCode = Convert.ToInt64(subLedgerCodeForSecurities + starter);
                }
                else
                {
                    long starter = EnumClass.StarterCounter;
                    maxCode = rawDetailList.Max() + starter;
                }
                RawDetaile ShortTermValuationInvestRawDetaile = new RawDetaile
                {
                    SubLedgerID = subLedgerIdForSecurities,
                    DetaileCode = maxCode,
                    DetaileName = "ارزشیابی " + cdsData.FullNameOfStock,
                    DetaileSymbole = cdsData.SymbolOfStock,
                    IsMarketMakerContrary = false,
                    IsPrecedence = true,
                    TafsilCodeInCDS = newTafsilCode,
                };
                rawList.Add(ShortTermValuationInvestRawDetaile);
            }

            return rawList;
        }


        //برای کارگزارانی که در دیتابیس و جدول مربوطه وجود ندارند
        public bool BrokerRawDetailNotExistListInDb(List<CDSExcelFormat> cDsList)
        {
            try
            {
                List<RawDetaile> notExistListOfBrokerInRawDetailTbl = new List<RawDetaile>();
                var distictBrokers = cDsList.Select(a => a.BrokerCode).Distinct();
                foreach (var item in distictBrokers)
                {
                    var brokerName = cDsList.Where(a => a.BrokerCode == item).Select(a => a.BrokerName).FirstOrDefault();
                    var brokerCode = cDsList.Where(a => a.BrokerCode == item).Select(a => a.BrokerCode).FirstOrDefault();

                    var IsExistBrokerAccount = IsBrokerAccountExistsInRawDetailsTbl(brokerName, brokerCode);
                    if (IsExistBrokerAccount == false)
                    {
                        var bindingBroker = cDsList.Where(a => a.BrokerCode == brokerCode && a.BrokerName == brokerName).FirstOrDefault();
                        var newBrokerrawDetail = BindBrokerRawDetaileDefualtInformation(bindingBroker);
                        InsertBrokerRawDetailFromEntityModel(newBrokerrawDetail);
                        notExistListOfBrokerInRawDetailTbl.AddRange(newBrokerrawDetail);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool IsBrokerAccountExistsInRawDetailsTbl(string brokerName, int brokerCode)
        {
            var DebitBrokerCdsTafsilCodeInDB = db.RawDetailesTbl.Where(a => a.DetaileName == brokerName && a.TafsilCodeInCDS == brokerCode && a.SubLedgerID == 28);
            var CreditBrokerCdsTafsilCodeInDB = db.RawDetailesTbl.Where(a => a.DetaileName == brokerName && a.TafsilCodeInCDS == brokerCode && a.SubLedgerID == 32);
            int? Debitcount = DebitBrokerCdsTafsilCodeInDB.Count();
            int? Creditcount = CreditBrokerCdsTafsilCodeInDB.Count();

            if (Debitcount > 0 && Creditcount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public List<RawDetaile> BindBrokerRawDetaileDefualtInformation(CDSExcelFormat cdsData)
        {
            //عدد 28 در جدول نشان دهنده کارگزاران سمت بدهکار است
            int brokerIdInSubLedger = EnumClass.BrokerIdIdInSubLedger;
            var subLedgerIdForDebit = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == brokerIdInSubLedger).Select(a => a.SubLedgerID).FirstOrDefault();
            var subLedgerCodeForDebit = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == subLedgerIdForDebit).Select(a => a.SubLedgerCode).FirstOrDefault();
            var rawDetailList = db.RawDetailesTbl.Where(a => a.SubLedgerID == subLedgerIdForDebit).Select(a => a.DetaileCode).ToList();
            long maxCode;
            if (rawDetailList.Count == 0)
            {
                maxCode = Convert.ToInt64(subLedgerCodeForDebit + EnumClass.StarterCounter.ToString());
            }
            else
            {
                maxCode = rawDetailList.Max() + 1;
            }
            List<RawDetaile> rawList = new List<RawDetaile>();
            RawDetaile ReciviblesRawDetaile = new RawDetaile
            {
                //خیلی مهم برای ایجاد کد بدهکاری دریافتنی‌های تجاری
                SubLedgerID = subLedgerIdForDebit,
                DetaileCode = maxCode,
                DetaileName = "کارگزاری " + cdsData.BrokerName,
                DetaileSymbole = cdsData.BrokerName,
                IsMarketMakerContrary = false,
                IsPrecedence = false,
                TafsilCodeInCDS = cdsData.BrokerCode,
            };
            rawList.Add(ReciviblesRawDetaile);

            //عدد 32 در جدول نشان دهنده کارگزاران سمت بستانکار است
            //var subLedgerIdForCredit = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == 32).Select(a => a.SubLedgerID).FirstOrDefault();
            //var subLedgerCodeForCredit = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == subLedgerIdForCredit).Select(a => a.SubLedgerCode).FirstOrDefault();
            //var rawDetailCreditList = db.RawDetailesTbl.Where(a => a.SubLedgerID == subLedgerCodeForCredit).Select(a => a.DetaileCode).ToList();
            //long maxCodeForCredit;
            //if (rawDetailCreditList.Count == 0)
            //{
            //    maxCodeForCredit = Convert.ToInt64(subLedgerCodeForCredit + "1");
            //}
            //else
            //{
            //    maxCodeForCredit = rawDetailList.Max() + 1;
            //}
            //RawDetaile PayablesRawDetaile = new RawDetaile
            //{
            //    //خیلی مهم برای ایجاد کد بستانکاری دریافتنی‌های تجاری
            //    SubLedgerID = subLedgerIdForCredit,
            //    DetaileCode= maxCodeForCredit,
            //    DetaileName = "کارگزاری " + cdsData.BrokerName + "طرف بستانکار",
            //    DetaileSymbole = cdsData.BrokerName,
            //    IsMarketMakerContrary = false,
            //    IsPrecedence = false,
            //    TafsilCodeInCDS = cdsData.BrokerCode,
            //};
            //rawList.Add(PayablesRawDetaile);
            return rawList;
        }

        //برای حساب سود که در دیتابیس و جدول راو دیتیل وجود ندارد
        public bool ProfitOfSellsRawDetailNotExistListInDb(List<CDSExcelFormat> cDsList)
        {
            try
            {
                List<RawDetaile> notExistListOfProfitOfSellInRawDetailTbl = new List<RawDetaile>();
                var distictTafsilCode = cDsList.Select(a => a.TafsilCode).Distinct();
                foreach (var TafsilCodeitem in distictTafsilCode)
                {
                    var bindingSellObject = cDsList.Where(a => a.TafsilCode == TafsilCodeitem).FirstOrDefault();
                    //var SellAccountCode = Convert.ToInt64("1" + bindingSellObject.InvestorCodeInCds.ToString() + TafsilCodeitem.ToString());
                    var SellAccountCode = TafsilCodeitem;
                    var IsExistSellAccount = IsProfitOfSellAccountExistsInRawDetailsTbl(SellAccountCode);
                    if (IsExistSellAccount == false)
                    {
                        var newSellRawDetail = BindProfitOfSellAccountRawDetaileDefualtInformation(bindingSellObject);
                        db.RawDetailesTbl.AddRange(newSellRawDetail);
                        SaveInDb();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool IsProfitOfSellAccountExistsInRawDetailsTbl(long sellAccountCodeInRawDetailtbl)
        {
            int stockPreofitsubledgerId = EnumClass.StockIdInSubLedger;
            int stockPrecedencesubledgerId = EnumClass.StockPrecedenceProfitIdInSubLedger;

            var stockProfitTafsilCodeInDB = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == sellAccountCodeInRawDetailtbl).Where(a => a.TafsilCodeInCDS == stockPreofitsubledgerId);

            int stockProfitCount = stockProfitTafsilCodeInDB.ToList()?.Count() ?? 0;

            if (stockProfitCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public List<RawDetaile> BindProfitOfSellAccountRawDetaileDefualtInformation(CDSExcelFormat cdsData)
        {
            List<RawDetaile> rawList = new List<RawDetaile>();

            if (cdsData.IsPrecedence == false)
            {
                //کد حساب درآمد حاصل از فروش سهام و اوراق در دفتر معین 37 می باشد
                var subLedgerIdForSellAccount = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == EnumClass.StockProfitIdInSubLedger).Select(a => a.SubLedgerID).FirstOrDefault();
                var subLedgerCodeForProfitOfSale = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == subLedgerIdForSellAccount).Select(a => a.SubLedgerCode).FirstOrDefault();
                var rawDetailList = db.RawDetailesTbl.Where(a => a.SubLedgerID == subLedgerIdForSellAccount).Select(a => a.DetaileCode).ToList();
                long maxCode;
                if (rawDetailList.Count == 0)
                {
                    maxCode = Convert.ToInt64(subLedgerCodeForProfitOfSale.ToString() + EnumClass.StarterCounter.ToString());
                }
                else
                {
                    maxCode = rawDetailList.Max() + 1;
                }
                RawDetaile SellRawDetaile = new RawDetaile
                {
                    //برای ایجاد حساب تفصیلی سود (زیان) حاصل از فروش سهام در جدول راو دیتیل
                    SubLedgerID = subLedgerIdForSellAccount,
                    DetaileCode = maxCode,
                    DetaileName = $"سود (زیان) سرمایه‌گذاری {cdsData.FullNameOfStock}",
                    DetaileSymbole = $"سود و زیان اوراق {cdsData.SymbolOfStock}",
                    IsMarketMakerContrary = false,
                    IsPrecedence = false,
                    //عدد 1 نشان دهنده شروع حساب تفصیلی سود (زیان) است
                    //TafsilCodeInCDS = Convert.ToInt64("1" + cdsData.InvestorCodeInCds.ToString() + cdsData.TafsilCode.ToString()),
                    TafsilCodeInCDS = cdsData.TafsilCode,
                };
                rawList.Add(SellRawDetaile);
            }
            if (cdsData.IsPrecedence == true)
            {
                //کد حساب درآمد حاصل از فروش حق تقدم سهام و اوراق در دفتر معین 38 می باشد
                var subLedgerIdForSellAccount = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == EnumClass.StockPrecedenceProfitIdInSubLedger).Select(a => a.SubLedgerID).FirstOrDefault();
                var subLedgerCodeForProfitOfSale = db.RawSubLedgersTbl.Where(a => a.SubLedgerID == subLedgerIdForSellAccount).Select(a => a.SubLedgerCode).FirstOrDefault();
                var rawDetailList = db.RawDetailesTbl.Where(a => a.SubLedgerID == subLedgerIdForSellAccount).Select(a => a.DetaileCode).ToList();
                long maxCode;
                if (rawDetailList.Count == 0)
                {
                    maxCode = Convert.ToInt64(subLedgerCodeForProfitOfSale.ToString() + EnumClass.StarterCounter.ToString());
                }
                else
                {
                    maxCode = rawDetailList.Max() + 1;
                }
                RawDetaile SellRawDetaile = new RawDetaile
                {
                    //برای ایجاد حساب تفصیلی سود (زیان) حاصل از فروش حق تقدم در جدول راو دیتیل
                    SubLedgerID = subLedgerIdForSellAccount,
                    DetaileCode = maxCode,
                    DetaileName = $"سود (زیان) سرمایه‌گذاری {cdsData.FullNameOfStock}",
                    DetaileSymbole = $"سود و زیان اوراق {cdsData.SymbolOfStock}",
                    IsMarketMakerContrary = false,
                    IsPrecedence = false,
                    //عدد 1 نشان دهنده شروع حساب تفصیلی سود (زیان) است
                    //TafsilCodeInCDS = Convert.ToInt64("1" + cdsData.InvestorCodeInCds.ToString() + cdsData.TafsilCode.ToString()),
                    TafsilCodeInCDS = cdsData.TafsilCode,
                };
                rawList.Add(SellRawDetaile);
            }

            return rawList;
        }









        public RawDetaile GetDetailAccountByStockTafsileCode(long tafsilCode)
        {
            var stockSubledgerId = EnumClass.StockIdInSubLedger;
            var detailAccount = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == tafsilCode && a.SubLedgerID == stockSubledgerId).FirstOrDefault();
            return detailAccount;
        }
        public RawDetaile GetDetailAccountByStockValuationTafsileCode(long tafsilCode)
        {
            long valuationTafsilCode = Convert.ToInt64(tafsilCode.ToString() + EnumClass.ValuationCounter.ToString());
            var detailAccount = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == valuationTafsilCode).FirstOrDefault();
            return detailAccount;
        }

        public RawDetaile GetDetailAccountByBrokerCode(long brokerCode)
        {
            var detailAccount = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == brokerCode).FirstOrDefault();
            return detailAccount;
        }
        public RawDetaile GetSellDetailAccountByCdsObject(CDSExcelFormat cdsData)
        {
            var sellCode = cdsData.TafsilCode;
            var subledgerId = EnumClass.StockProfitIdInSubLedger;
            var sellDetailAccount = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == sellCode && a.SubLedgerID == subledgerId).FirstOrDefault();
            return sellDetailAccount;
        }
        public RawDetaile GetCostDetailAccountByCdsObject(CDSExcelFormat cdsData)
        {
            var costCode = Convert.ToInt64("2" + cdsData.InvestorCodeInCds.ToString() + cdsData.TafsilCode.ToString());
            var costDetailAccount = db.RawDetailesTbl.Where(a => a.TafsilCodeInCDS == costCode).FirstOrDefault();
            return costDetailAccount;
        }

        public bool SaveInDb()
        {
            try
            {
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
