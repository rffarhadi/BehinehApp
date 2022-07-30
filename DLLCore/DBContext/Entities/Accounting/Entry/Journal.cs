using DLLCore.DBContext.Entities.Accounting.RawAccounts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.DBContext.Entities.Accounting.Entry
{
    public class Journal
    {
        [Key]
        public long EntryID { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "شماره سند")]
        public long EntryNo { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "تاریخ میلادی سند")]
        public DateTime EntryGregorianDate { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "تاریخ سند")]
        public string EntryDate { get; set; }

        [Display(Name = "شرح سند")]
        [DataType(DataType.MultilineText)]
        public string EntryDescription { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "بدهکار")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long EntryDebit { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "بستانکار")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long EntryCredit { get; set; }

        [Display(Name = "حجم معامله")]
        public long? EntryBuySellVolume { get; set; }

        [Display(Name = "قیمت معامله")]
        public long? TradePrice { get; set; }

        [Display(Name = "نوع معامله")]
        public string TradeType { get; set; }

        [Display(Name = "شماره اعلامیه")]
        public long? FactorNumber { get; set; }

        [Display(Name = "کارمزد کارگزاری")]
        public long? BrokerFee { get; set; }

        [Display(Name = "کارمزد کل")]
        public long? TotalFee { get; set; }

        [Display(Name = "مالیات")]
        public long? TradeTax { get; set; }

        [Display(Name = "نام کارگزار")]
        public string BrokerName { get; set; }

        [Display(Name = "کد کارگزار")]
        public int? BrokerCode { get; set; }

        //بالادستی
        public long DetaileID { get; set; }
        public RawDetaile rawDetaile { get; set; }


        //بالادستی: پروفایل سرمایه‌گذار
        public long InvestorID { get; set; }
        public InvestorProfile investorProfileInJournal { get; set; }

    }
}
