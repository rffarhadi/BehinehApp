using DLLCore.DBContext.Entities.Accounting.Entry;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.DBContext.Entities.Accounting.RawAccounts
{
    public class RawSubLedger
    {
        [Key]
        public long SubLedgerID { get; set; }

        [Display(Name = "کد معین")]
        public long SubLedgerCode { get; set; }

        [Display(Name = "نام معین")]
        public string SubLedgerName { get; set; }


        //پایین دستی
        public ICollection<RawDetaile> RawDetailes { get; set; }

        //بالادستی
        public long RawLedgerID { get; set; }
        public RawLedgers rawLedgers { get; set; }
    }
}
