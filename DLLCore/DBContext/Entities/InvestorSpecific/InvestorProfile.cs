using DLLCore.DBContext.Entities.Accounting;
using DLLCore.DBContext.Entities.Accounting.Entry;
using DLLCore.DBContext.Entities.InvestorSpecific;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.DBContext.Entities
{
    public class InvestorProfile
    {
        [Key]
        public long InvestorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string RegisterDate { get; set; }
        public string BirthDate { get; set; }
        public string NationalCode { get; set; }
        public string BourseCode { get; set; }
        public long? InvestorCodeInCDS { get; set; }
        public string BirthCertificateID { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public long InitialEquity { get; set; }
        public int ExperiencByMonth { get; set; }


        public ICollection<InvestorOSIProfile> investorOSIProfiles { get; set; }
        public ICollection<InfrencesConclutionProfile> infrencesConclutionProfiles { get; set; }
        public ICollection<InvestorOperation> investorOperations { get; set; }

        //پایین دستی: دفتر روزنامه
        public ICollection<Journal> journalsInIvestorProfile { get; set; }

    }
}
