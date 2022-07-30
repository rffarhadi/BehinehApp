using DLLCore.DBContext.Entities;
using DLLCore.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.ViewModels
{
    public class InvestorOSIProfileViewModel
    {
        [Key]
        public long OsiID { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "هدف سرمایه‌گذار")]
        [DataType(DataType.MultilineText)]
        public string Objective { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "محدودیت‌ سرمایه‌گذار")]
        [DataType(DataType.MultilineText)]
        public string Subjective { get; set; }

   

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "نام سرمایه‌گذار")]
        public long InvestorID { get; set; }
        public InvestorProfileViewModel InvestProfile { get; set; }
    }
}
