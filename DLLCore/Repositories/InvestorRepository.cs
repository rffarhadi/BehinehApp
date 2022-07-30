using DLLCore.DBContext;
using DLLCore.DBContext.Entities;
using DLLCore.Utility.Excel;
using DLLCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.Repositories
{
    public class InvestorRepository
    {
        ApplicationDbContext db;
        public InvestorRepository()
        {
            db = new ApplicationDbContext();
        }


        //برای بخش بررسی و تکمیل اطلاعات دریافتی از فناوری
        public List<InvestorProfile> NotExistInvestorProfileList(List<CDSExcelFormat> cDsList)
        {
            List<InvestorProfile> ipList = new List<InvestorProfile>();
            var distinctInvestores = cDsList.Select(a => a.InvestorCodeInCds).Distinct();

            foreach (var item in distinctInvestores)
            {
                var investorCodeInCds = item;
                var IsExist = IsExistsInInvestorProfileTbl(investorCodeInCds);
                if (IsExist == false)
                {
                    var newInvestor = BindIPDefualtInformation(investorCodeInCds);
                    ipList.Add(newInvestor);
                }
            }
            return ipList;
        }
        public bool IsExistsInInvestorProfileTbl(long investorCodeInCds)
        {
            var investorCdsCodeInDB = db.InvestorProfiles.Where(a => a.InvestorCodeInCDS == investorCodeInCds).FirstOrDefault();
            if (investorCdsCodeInDB != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public InvestorProfile BindIPDefualtInformation(long investorCodeInCds)
        {
            InvestorProfile ip = new InvestorProfile
            {
                InvestorCodeInCDS = investorCodeInCds,
                FirstName = "شخص با کد" + investorCodeInCds.ToString(),
                LastName = " ",
                FatherName = " ",
                Address = "NotSet",
                BirthCertificateID = "NotSet",
                BirthDate = "NotSet",
                BourseCode = "NotSet",
                Email = "NotSet@gmail.com",
                RegisterDate = "NotSet",
                PostalCode = "NotSet",
                PhoneNumber = "NotSet",
                MobileNumber = "NotSet",
                NationalCode = "NotSet",
                Age = 0,
                ExperiencByMonth = 0,
                InitialEquity = 0,
            };
            return ip;
        }



        //عملیات عادی
        public bool InsertInvestor(InvestorProfileViewModel investorProfile)
        {
            try
            {
                var proflie = BindRegVMToInvesProf(investorProfile);
                db.InvestorProfiles.Add(proflie);
                SaveInDb();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool InsertInvestorProfileListFromEntityModel(List<InvestorProfile> investorProfileList)
        {
            try
            {
                foreach (var item in investorProfileList)
                {
                    var dbRecord = db.InvestorProfiles.Where(a => a.InvestorCodeInCDS == item.InvestorCodeInCDS);
                    int? count = dbRecord.Count();
                    if (count == 0 || count == null)
                    {
                        db.InvestorProfiles.Add(item);
                        SaveInDb();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool EditInvestor(InvestorProfileViewModel vm)
        {
            try
            {
                var investorproflie = BindRegVMToInvesProf(vm);
                var local = db.Set<InvestorProfile>()
                        .Local
                        .FirstOrDefault(f => f.InvestorID == investorproflie.InvestorID);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(investorproflie).State = EntityState.Modified;
                SaveInDb();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteInvestor(InvestorProfileViewModel vm)
        {
            try
            {
                var investorproflie = BindRegVMToInvesProf(vm);
                var local = db.Set<InvestorProfile>()
                         .Local
                         .FirstOrDefault(f => f.InvestorID == investorproflie.InvestorID);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(investorproflie).State = EntityState.Deleted;
                SaveInDb();

                return true;
            }
            catch (Exception ex)
            {
                var m = ex.Message;
                return false;
            }
        }


        public IEnumerable<InvestorProfileViewModel> GetAllInvestor()
        {
            try
            {
                List<InvestorProfileViewModel> Inestoreslistvm = new List<InvestorProfileViewModel>();
                var investors = db.InvestorProfiles.ToList();
                DisposeDb();
                foreach (var item in investors)
                {
                    var row = BindInvesProfToVM(item);
                    Inestoreslistvm.Add(row);
                }
                return Inestoreslistvm;
            }
            catch
            {
                return null;
            }
        }
        public InvestorProfileViewModel GetInvestorByInvestorId(int? id)
        {
            try
            {
                var investor = db.InvestorProfiles.Where(a => a.InvestorID == id).SingleOrDefault();
                var investorVm = BindInvesProfToVM(investor);
                return investorVm;
            }
            catch
            {
                return null;
            }
        }
        public InvestorProfile GetInvestorByInvestorCdsCode(long? id)
        {
            try
            {
                var investor = db.InvestorProfiles.Where(a => a.InvestorCodeInCDS == id).FirstOrDefault();
                return investor;
            }
            catch
            {
                return null;
            }
        }



        public bool SaveInDb()
        {
            try
            {
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public InvestorProfile BindRegVMToInvesProf(InvestorProfileViewModel vm)
        {
            try
            {
                InvestorProfile profile = new InvestorProfile
                {
                    InvestorID = vm.InvestorID,
                    FatherName = vm.FatherName,
                    FirstName = vm.FirstName,
                    LastName = vm.LastName,
                    BirthCertificateID = vm.BirthCertificateID,
                    Address = vm.Address,
                    Age = vm.Age,
                    BirthDate = vm.BirthDate,
                    Email = vm.Email,
                    ExperiencByMonth = vm.ExperiencByMonth,
                    InitialEquity = vm.InitialEquity,
                    MobileNumber = vm.MobileNumber,
                    NationalCode = vm.NationalCode,
                    BourseCode = vm.BourseCode,
                    PostalCode = vm.PostalCode,
                    PhoneNumber = vm.PhoneNumber,
                    RegisterDate = vm.RegisterDate,
                    InvestorCodeInCDS = vm.InvestorCodeInCDS
                };
                return profile;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public InvestorProfileViewModel BindInvesProfToVM(InvestorProfile em)
        {
            try
            {
                InvestorProfileViewModel vmProfile = new InvestorProfileViewModel
                {
                    InvestorID = em.InvestorID,
                    FatherName = em.FatherName,
                    FirstName = em.FirstName,
                    LastName = em.LastName,
                    BirthCertificateID = em.BirthCertificateID,
                    Address = em.Address,
                    Age = em.Age,
                    BirthDate = em.BirthDate,
                    Email = em.Email,
                    ExperiencByMonth = em.ExperiencByMonth,
                    InitialEquity = em.InitialEquity,
                    MobileNumber = em.MobileNumber,
                    NationalCode = em.NationalCode,
                    BourseCode = em.BourseCode,
                    PostalCode = em.PostalCode,
                    PhoneNumber = em.PhoneNumber,
                    RegisterDate = em.RegisterDate,
                    InvestorCodeInCDS = em.InvestorCodeInCDS
                };
                return vmProfile;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public void DisposeDb()
        {
            db.Dispose();
        }
    }
}
