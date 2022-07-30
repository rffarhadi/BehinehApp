using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace IdentitySample.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "نام نقش مدیریتی")]
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {
        [Display(Name = "کد کاربر")]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "ایمیل")]
        [EmailAddress]
        public string Email { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}