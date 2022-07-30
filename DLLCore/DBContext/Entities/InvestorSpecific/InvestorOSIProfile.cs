using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.DBContext.Entities
{
    public class InvestorOSIProfile
    {
        [Key]
        public long OsiID { get; set; }
        public string Objective { get; set; }
        public string Subjective { get; set; }

        public long InvestorID { get; set; }
        public InvestorProfile InvestProfile { get; set; }
    }
}
