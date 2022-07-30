using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.DBContext.Entities.InvestorSpecific
{
    public class InfrencesConclutionProfile
    {

        [Key]
        public long InfrencesConclutionID { get; set; }
        public string Inference { get; set; }
        public string FinalConclution { get; set; }
        public long InvestorID { get; set; }
        public InvestorProfile InvestProfile { get; set; }
    }
}
