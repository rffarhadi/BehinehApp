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
    public class InvestorProfileViewModel
    {
        [Key]
        public long InvestorID { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "نام پدر")]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "تاریخ ثبت")]
        public string RegisterDate { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "تاریخ تولد")]
        public string BirthDate { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "کد ملی")]
        [MinLength(10, ErrorMessage = "لطفاً {0} را 10 رقمی وارد کنید")]
        public string NationalCode { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "کد بورسی")]
        public string BourseCode { get; set; }

        [Display(Name = "کد سپرده‌گذاری")]
        public long? InvestorCodeInCDS { get; set; }


        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "شماره شناسنامه")]
        public string BirthCertificateID { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "ایمیل")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "سن")]
        public int Age { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "شماره همراه")]
        [DataType(DataType.PhoneNumber)]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "شماره تلفن")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "آدرس")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "کدپستی")]
        [MinLength(10, ErrorMessage = "لطفاً {0} را 10 رقمی وارد کنید")]
        [MaxLength(10,ErrorMessage = "لطفاً {0} را 10 رقمی وارد کنید")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "سرمایه اولیه")]
        public long InitialEquity { get; set; }

        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Display(Name = "تجربه بورسی به ماه")]
        public int ExperiencByMonth { get; set; }


        public ICollection<InvestorOSIProfileViewModel> investorOSIProfiles { get; set; }


    }
}
