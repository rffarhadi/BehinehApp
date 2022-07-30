using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.DBContext.Entities
{
    public class InvestorOperation
    {
        [Key]
        public long OprID { get; set; }
        public string OprExplain { get; set; }
        public DateTime OprDate { get; set; }
        public decimal? MoneyEntry { get; set; }
        public decimal? MoneyExit { get; set; }


        public long InvestorID { get; set; }
        public InvestorProfile InvestProfile { get; set; }
    }
}
