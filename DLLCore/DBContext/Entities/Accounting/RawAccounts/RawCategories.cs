using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.DBContext.Entities.Accounting.RawAccounts
{
    public class RawCategories
    {

        [Key]
        public long RawCategoryID { get; set; }

        [Display(Name = "کد گروه")]
        public long RawCategoryCode { get; set; }

        [Display(Name = "نام گروه")]
        public string RawCategoryName { get; set; }

        public ICollection<RawSubCategories> AccountingSubCategories { get; set; }
    }
}
