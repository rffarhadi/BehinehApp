using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.ViewModels.FinancialStatmentsViewModels
{
    public class BalanceSheetViewModel
    {
        [Display(Name ="کد")]
        public long AccountId { get; set; }

        [Display(Name = "کد معین")]
        public long SubLedgerId { get; set; }

        [Display(Name = "کد سهم در فناورری")]
        public long StockTafsileInCds { get; set; }

        [Display(Name = "کد سرمایه‌گذار")]
        public long InvestorId { get; set; }

        [Display(Name = "نام سرمایه‌گذار")]
        public string InvestorName { get; set; }

        [Display(Name = "کد بالاسری")]
        public long CategoryId { get; set; }

        [Display(Name = "کد حساب")]
        public long SubCategoryId { get; set; }

        [Display(Name = "نام حساب")]
        public string AccountName { get; set; }

        [Display(Name = "نام نماد")]
        public string AccountSymboleName { get; set; }

        [Display(Name = "گردش بدهکار")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long DebitBalance { get; set; }

        [Display(Name = "گردش بستانکار")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long CreditBalance { get; set; }

        [Display(Name = "مانده")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long TotalBalance { get; set; }

        [Display(Name = "میانگین قیمت")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long PriceAvreage { get; set; }

        [Display(Name = "حجم باقیمانده")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long VolumeBalance { get; set; }

        [Display(Name = "ماهیت")]
        public string Nature { get; set; }

        [Display(Name = "آخرین تاریخ")]
        public string LastDateOfUpdate { get; set; }
    }


}
