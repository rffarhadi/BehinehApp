using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.DBContext.Entities.Accounting.RawAccounts
{
    public class RawSubCategories
    {

        [Key]
        public long SubCategoryID { get; set; }

        [Display(Name = "کد زیر گروه")]
        public long SubCategoryCode { get; set; }

        [Display(Name = "نام زیر گروه")]
        public string SubCategoryName { get; set; }



        //بالادستی
        public long RawCategoryID { get; set; }
        public RawCategories RawCategoriesInSub { get; set; }

        //پایین دستی
        public ICollection<RawLedgers> RawLedgers { get; set; }


    }
}
