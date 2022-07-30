using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.ViewModels.FinancialStatmentsViewModels
{
    public class PerformanceViewModel
    {
        [Display(Name = "کد")]
        public long AccountId { get; set; }

        [Display(Name = "کد سرمایه‌گذار")]
        public long InvestorId { get; set; }

        [Display(Name = "نام سرمایه‌گذار")]
        public string InvestorName { get; set; }

        [Display(Name = "سود کل")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long TotalPeofitOrLoss { get; set; }

        [Display(Name = "دارایی کل")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long TotalAsset { get; set; }

        [Display(Name = "نرخ بازدهی کل")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal TotalReturn { get; set; }

        [Display(Name = "آخرین تاریخ")]
        public string PersianDate { get; set; }

        [Display(Name = "تاریخ میلادی")]
        public DateTime GeorgianDate { get; set; }
    }
}
