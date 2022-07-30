using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.ViewModels
{
    public class InvestorInfrencesConclutionViewModel
    {

        [Key]
        public long InfrencesConclutionID { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "استنباط از سرمایه‌گذار")]
        [DataType(DataType.MultilineText)]
        public string Inference { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "نتیجه‌گیری")]
        [DataType(DataType.MultilineText)]
        public string FinalConclution { get; set; }


        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "نام سرمایه‌گذار")]
        public long InvestorID { get; set; }
        public InvestorProfileViewModel InvestProfile { get; set; }
    }
}
