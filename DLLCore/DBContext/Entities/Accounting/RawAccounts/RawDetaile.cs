using DLLCore.DBContext.Entities.Accounting.Entry;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.DBContext.Entities.Accounting.RawAccounts
{
    public class RawDetaile
    {
        [Key]
        public long DetaileID { get; set; }

        [Display(Name = "کد تفصیل")]
        public long DetaileCode { get; set; }

        [Display(Name = "کد CDS")]
        public long? TafsilCodeInCDS { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")] 
        [Display(Name = "نام حساب")]
        public string DetaileName { get; set; }

        [Display(Name = "نماد")]
        public string DetaileSymbole { get; set; }

        [Display(Name = "حق تقدم؟")]
        public bool IsPrecedence { get; set; }

        [Display(Name = "طرف بازارگردان؟")]
        public bool IsMarketMakerContrary { get; set; }

        //پایین دستی
        public ICollection<Journal> JournalsInDetaileTbl { get; set; }


        //بالادستی: معین
        public long SubLedgerID { get; set; }
        public RawSubLedger  rawSubLedger { get; set; }


    }
}
