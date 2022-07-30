using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.DBContext.Entities.Accounting.RawAccounts
{
    public class RawLedgers
    {

        [Key]
        public long RawLedgerID { get; set; }

        [Display(Name = "کد دفتر کل")]
        public long RawLedgerCode { get; set; }

        [Display(Name = "نام دفتر کل")]
        public string RawLedgerName { get; set; }

        public ICollection<RawSubLedger> RawSubLedgers { get; set; }


        public long SubCategoryID { get; set; }
        public RawSubCategories RawSubCategories { get; set; }

    }

}
