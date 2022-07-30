using DLLCore.DBContext;
using DLLCore.DBContext.Entities;
using DLLCore.DBContext.Entities.InvestorSpecific;
using DLLCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.Repositories
{
    public class InvestorInfrencesConclutionRepository
    {
        private ApplicationDbContext db;
        private InvestorRepository investorRepository;
        public InvestorInfrencesConclutionRepository()
        {
            db = new ApplicationDbContext();
            investorRepository = new InvestorRepository();
        }

        public bool InsertIcSingleRow(InvestorInfrencesConclutionViewModel icVm)
        {
            try
            {
                var IcData = BindIcVmToIcEntity(icVm);
                db.InfrencesConclutionProfiles.Add(IcData);
                SaveInDb();
                return true;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
            }
        }
        public bool InsertIcMultiRows(List<InvestorInfrencesConclutionViewModel> icVmList)
        {
            try
            {
                var icDataList = BindIcVmListIcEntityList(icVmList);
                db.InfrencesConclutionProfiles.AddRange(icDataList);
                SaveInDb();
                return true;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
            }

        }

        public bool DeleteIcEntity(InvestorInfrencesConclutionViewModel icVm)
        {
            try
            {
                var icData = BindIcVmToIcEntity(icVm);
                var local = db.Set<InfrencesConclutionProfile>()
                          .Local
                          .FirstOrDefault(f => f.InfrencesConclutionID == icData.InfrencesConclutionID);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(icData).State = EntityState.Deleted;
                SaveInDb();
                return true;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
            }
        }
        public bool EditIcEntity(InvestorInfrencesConclutionViewModel icVm)
        {
            try
            {
                var icData = BindIcVmToIcEntity(icVm);
                var local = db.Set<InfrencesConclutionProfile>()
                           .Local
                           .FirstOrDefault(f => f.InfrencesConclutionID == icData.InfrencesConclutionID);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(icData).State = EntityState.Modified;
                SaveInDb();
                return true;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
            }
        }

        public List<InvestorInfrencesConclutionViewModel> GetAll()
        {
            try
            {
                var investorIcProfiles = db.InfrencesConclutionProfiles.Include(i => i.InvestProfile).ToList();
                var icVmList = BindIcEntityListToIcVmList(investorIcProfiles);
                return icVmList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public InvestorInfrencesConclutionViewModel FindById(int? id)
        {
            try
            {
                InfrencesConclutionProfile investorIcProfile = db.InfrencesConclutionProfiles.Where(a => a.InfrencesConclutionID == id).Include(i => i.InvestProfile).FirstOrDefault();
                var icVmList = BindIcEntityToIcVm(investorIcProfile);
                return icVmList;
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

        public InfrencesConclutionProfile BindIcVmToIcEntity(InvestorInfrencesConclutionViewModel icVm)
        {
            InfrencesConclutionProfile entityModel = new InfrencesConclutionProfile()
            {
                InvestorID = icVm.InvestorID,
                InfrencesConclutionID = icVm.InfrencesConclutionID,
                Inference = icVm.Inference,
                FinalConclution = icVm.FinalConclution,
                //InvestProfile = investorRepository.BindRegVMToInvesProf(osiVm.InvestProfile),
            };

            return entityModel;
        }
        public InvestorInfrencesConclutionViewModel BindIcEntityToIcVm(InfrencesConclutionProfile icEntity)
        {
            InvestorInfrencesConclutionViewModel entityModel = new InvestorInfrencesConclutionViewModel
            {
                InvestorID = icEntity.InvestorID,
                InfrencesConclutionID = icEntity.InfrencesConclutionID,
                Inference = icEntity.Inference,
                FinalConclution = icEntity.FinalConclution,
                InvestProfile = investorRepository.BindInvesProfToVM(icEntity.InvestProfile),
            };
            return entityModel;

        }
        public List<InvestorInfrencesConclutionViewModel> BindIcEntityListToIcVmList(List<InfrencesConclutionProfile> icEntity)
        {
            List<InvestorInfrencesConclutionViewModel> icVmList = new List<InvestorInfrencesConclutionViewModel>();

            foreach (var item in icEntity)
            {
                InvestorInfrencesConclutionViewModel viewModel = new InvestorInfrencesConclutionViewModel
                {
                    InvestorID = item.InvestorID,
                    InfrencesConclutionID = item.InfrencesConclutionID,
                    Inference = item.Inference,
                    FinalConclution = item.FinalConclution,
                    InvestProfile = investorRepository.BindInvesProfToVM(item.InvestProfile),
                };
                icVmList.Add(viewModel);
            }
            return icVmList;
        }
        public List<InfrencesConclutionProfile> BindIcVmListIcEntityList(List<InvestorInfrencesConclutionViewModel> icVm)
        {
            List<InfrencesConclutionProfile> icEntityList = new List<InfrencesConclutionProfile>();

            foreach (var item in icVm)
            {
                InfrencesConclutionProfile entityModel = new InfrencesConclutionProfile
                {
                    InvestorID = item.InvestorID,
                    InfrencesConclutionID = item.InfrencesConclutionID,
                    Inference = item.Inference,
                    FinalConclution = item.FinalConclution,
                    //InvestProfile = investorRepository.BindRegVMToInvesProf(item.InvestProfile),
                };
                icEntityList.Add(entityModel);
            }
            return icEntityList;
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
