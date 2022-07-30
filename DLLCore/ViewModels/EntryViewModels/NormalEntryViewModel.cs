using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DLLCore.ViewModels
{
    public class NormalEntryViewModel
    {

        [Display(Name = "شماره سند")]
        //[DisplayFormat(DataFormatString = "{0:N6}", ApplyFormatInEditMode = true)]
        public long EntryNo { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "تاریخ سند")]
        public string EntryDate { get; set; }

        [Display(Name = "شرح سند")]
        [DataType(DataType.MultilineText)]
        public string EntryDescription { get; set; }

        [Display(Name = "بدهکار")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long EntryDebit { get; set; }

        [Display(Name = "بستانکار")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long EntryCredit { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
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


        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "نام حساب تفصیلی")]
        public long DetaileID { get; set; }
        public SelectListItem DetailesNames { get; set; }


        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "کد سرمایه‌گذار")]
        public long InvestorID { get; set; }

    }

    public class NormalEntryListModelForViewModel
    {
        public List<NormalEntryViewModel> NormalEntryViewListModel { get; set; }
    }
}
