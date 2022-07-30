using DLLCore.DBContext.Entities.Accounting.Entry;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.Utility.Excel
{
    public static class DataTableToList
    {
        public static List<CDSExcelFormat> ConvertDataTableToJournalList(DataTable dt)
        {
            try
            {
                List<CDSExcelFormat> excelCdsList = new List<CDSExcelFormat>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CDSExcelFormat cds = new CDSExcelFormat();
                    cds.CDSCode = Convert.ToInt64(dt.Rows[i].ItemArray.ElementAt(0).ToString());
                    cds.BrokerCode = int.Parse(dt.Rows[i].ItemArray.ElementAt(0).ToString().Substring(0, 3));
                    cds.InvestorCodeInCds = int.Parse(dt.Rows[i].ItemArray.ElementAt(0).ToString().Substring(3));

                    cds.PersianDate = dt.Rows[i].ItemArray.ElementAt(1).ToString();
                    cds.TradeType = dt.Rows[i].ItemArray.ElementAt(2).ToString();
                    cds.FullNameOfStock = dt.Rows[i].ItemArray.ElementAt(3).ToString();
                    cds.SymbolOfStock = dt.Rows[i].ItemArray.ElementAt(4).ToString();
                    cds.TafsilCode = int.Parse(dt.Rows[i].ItemArray.ElementAt(5).ToString());
                    if (dt.Rows[i].ItemArray.ElementAt(6).ToString() == "خير")
                    {
                        cds.IsPrecedence = false;
                    }
                    else
                    {
                        cds.IsPrecedence = true;
                    }
                    if (dt.Rows[i].ItemArray.ElementAt(7).ToString() == "خير")
                    {
                        cds.IsMarketMakerContrary = false;
                    }
                    else
                    {
                        cds.IsMarketMakerContrary = true;
                    }

                    cds.TradeVolume = Convert.ToInt64(dt.Rows[i].ItemArray.ElementAt(8).ToString());
                    cds.TradePrice = int.Parse(dt.Rows[i].ItemArray.ElementAt(9).ToString());
                    cds.TradeValue = Convert.ToInt64(dt.Rows[i].ItemArray.ElementAt(10).ToString());
                    cds.TradeTax = Convert.ToInt64(dt.Rows[i].ItemArray.ElementAt(11).ToString());
                    cds.TotalFee = Convert.ToInt64(dt.Rows[i].ItemArray.ElementAt(12).ToString());
                    cds.BrokerFee = Convert.ToInt64(dt.Rows[i].ItemArray.ElementAt(13).ToString());
                    cds.FactorNumber = int.Parse(dt.Rows[i].ItemArray.ElementAt(14).ToString());
                    cds.BrokerName = dt.Rows[i].ItemArray.ElementAt(15).ToString();
                    excelCdsList.Add(cds);
                }
                return excelCdsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public class CDSExcelFormat
    {

        [Display(Name = "کد سپرده‌گذاری")]
        public long CDSCode { get; set; }

        [Display(Name = "کد سرمایه‌گذار")]
        public long InvestorCodeInCds { get; set; }

        [Display(Name = "تاریخ")]
        public string PersianDate { get; set; }

        [Display(Name = "نوع معامله")]
        public string TradeType { get; set; }

        [Display(Name = "نام سهم")]
        public string FullNameOfStock { get; set; }

        [Display(Name = "نماد سهم")]
        public string SymbolOfStock { get; set; }

        [Display(Name = "کد تفصیل")]
        public long TafsilCode { get; set; }

        [Display(Name = "حق تقدم؟")]
        public bool IsPrecedence { get; set; }

        [Display(Name = "طرف بازار ساز؟")]
        public bool IsMarketMakerContrary { get; set; }

        [Display(Name = "حجم معامله")]
        public long TradeVolume { get; set; }

        [Display(Name = "قیمت معامله")]
        public int TradePrice { get; set; }

        [Display(Name = "ارزش معامله")]
        public long TradeValue { get; set; }

        [Display(Name = "مالیات")]
        public long TradeTax { get; set; }

        [Display(Name = "کارمزد کل")]
        public long TotalFee { get; set; }

        [Display(Name = "کارمزد کارگزار")]
        public long BrokerFee { get; set; }

        [Display(Name = "ش اعلامیه")]
        public int FactorNumber { get; set; }

        [Display(Name = "نام کارگزار")]
        public string BrokerName { get; set; }

        [Display(Name = "کد کارگزار")]
        public int BrokerCode { get; set; }
    }
    public class CDSExcelFormatDetail
    {
        public List<CDSExcelFormat> CDSExcelFormatObject { get; set; }
    }


    public class RawLedgerExcellFormat
    {

        [Display(Name = "کد سپرده‌گذاری")]
        public long CDSCode { get; set; }



    }


}
