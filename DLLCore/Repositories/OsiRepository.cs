using DLLCore.DBContext;
using DLLCore.DBContext.Entities;
using DLLCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace DLLCore.Repositories
{
    public class OsiRepository
    {
        private ApplicationDbContext db;
        private InvestorRepository investorRepository;
        public OsiRepository()
        {
            db = new ApplicationDbContext();
            investorRepository = new InvestorRepository();
        }

        public bool InsertOsiSingleRow(InvestorOSIProfileViewModel osiVm)
        {
            try
            {
                var osiData = BindOsiVmToOsiEntity(osiVm);
                db.InvestorOSIProfiles.Add(osiData);
                SaveInDb();
                return true;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
            }
        }
        public bool InsertOsiMultiRows(List<InvestorOSIProfileViewModel> osiVmList)
        {
            try
            {
                var osiDataList = BindOsiVmListOsiEntityList(osiVmList);
                db.InvestorOSIProfiles.AddRange(osiDataList);
                SaveInDb();
                return true;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
            }

        }

        public bool DeleteOSEntity(InvestorOSIProfileViewModel osiVm)
        {
            try
            {
                var osiData = BindOsiVmToOsiEntity(osiVm);
                var local = db.Set<InvestorOSIProfile>()
                          .Local
                          .FirstOrDefault(f => f.OsiID == osiData.OsiID);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(osiData).State = EntityState.Deleted;
                SaveInDb();
                return true;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
            }
        }
        public bool EditOSEntity(InvestorOSIProfileViewModel osiVm)
        {
            try
            {
                var osiData = BindOsiVmToOsiEntity(osiVm);
                var local = db.Set<InvestorOSIProfile>()
                           .Local
                           .FirstOrDefault(f => f.OsiID == osiData.OsiID);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(osiData).State = EntityState.Modified;
                SaveInDb();
                return true;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
            }
        }

        public List<InvestorOSIProfileViewModel> GetAll()
        {
            try
            {
                var investorOSIProfiles = db.InvestorOSIProfiles.Include(i => i.InvestProfile).ToList();
                var osiVmList = BindOsiEntityListToOsiVmList(investorOSIProfiles);
                return osiVmList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public InvestorOSIProfileViewModel FindById(int? id)
        {
            try
            {
                InvestorOSIProfile investorOSIProfile = db.InvestorOSIProfiles.Where(a=>a.OsiID==id).Include(i => i.InvestProfile).FirstOrDefault();
                var osiVmList = BindOsiEntityToOsiVm(investorOSIProfile);
                return osiVmList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<InvestorProfileViewModel> GetInvestorProfiles()
        {
            var investProfiles = investorRepository.GetAllInvestor();
            return investProfiles.ToList();
        }

        public InvestorOSIProfile BindOsiVmToOsiEntity(InvestorOSIProfileViewModel osiVm)
        {
            InvestorOSIProfile entityModel = new InvestorOSIProfile()
            {
                InvestorID = osiVm.InvestorID,
                OsiID = osiVm.OsiID,
                Subjective = osiVm.Subjective,
                Objective = osiVm.Objective,
                //InvestProfile = investorRepository.BindRegVMToInvesProf(osiVm.InvestProfile),
            };

            return entityModel;
        }
        public InvestorOSIProfileViewModel BindOsiEntityToOsiVm(InvestorOSIProfile osiEntity)
        {
            InvestorOSIProfileViewModel entityModel = new InvestorOSIProfileViewModel
            {
                InvestorID = osiEntity.InvestorID,
                OsiID = osiEntity.OsiID,
                Subjective = osiEntity.Subjective,
                Objective = osiEntity.Objective,
                InvestProfile = investorRepository.BindInvesProfToVM(osiEntity.InvestProfile),
            };
            return entityModel;

        }
        public List<InvestorOSIProfileViewModel> BindOsiEntityListToOsiVmList(List<InvestorOSIProfile> osiEntity)
        {
            List<InvestorOSIProfileViewModel> osiVmList = new List<InvestorOSIProfileViewModel>();

            foreach (var item in osiEntity)
            {
                InvestorOSIProfileViewModel viewModel = new InvestorOSIProfileViewModel
                {
                    InvestorID = item.InvestorID,
                    OsiID = item.OsiID,
                    Subjective = item.Subjective,
                    Objective = item.Objective,
                    InvestProfile = investorRepository.BindInvesProfToVM(item.InvestProfile),
                };
                osiVmList.Add(viewModel);
            }
            return osiVmList;
        }
        public List<InvestorOSIProfile> BindOsiVmListOsiEntityList(List<InvestorOSIProfileViewModel> osiVm)
        {
            List<InvestorOSIProfile> osiEntityList = new List<InvestorOSIProfile>();

            foreach (var item in osiVm)
            {
                InvestorOSIProfile entityModel = new InvestorOSIProfile
                {
                    InvestorID = item.InvestorID,
                    OsiID = item.OsiID,
                    Subjective = item.Subjective,
                    Objective = item.Objective,
                    //InvestProfile = investorRepository.BindRegVMToInvesProf(item.InvestProfile),
                };
                osiEntityList.Add(entityModel);
            }
            return osiEntityList;
        }


        bool SaveInDb()
        {
            db.SaveChanges();
            return true;
        }

        public void DisposeDb()
        {
            db.Dispose();
        }

    }
}
