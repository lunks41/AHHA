using AHHA.Application.CommonServices;
using AHHA.Application.IServices;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;

namespace AHHA.Infra.Services.Masters
{
    public sealed class LookupService : ILookupService
    {
        private readonly IRepository<dynamic> _repository;
        private ApplicationDbContext _context;

        public LookupService(IRepository<dynamic> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<CountryLookupViewModel>> GetCountryLooupListAsync(string RegId, Int16 CompanyId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CountryLookupViewModel>(RegId, $"SELECT CountryId,CountryCode,CountryName FROM M_Country WHERE CountryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Country},{(short)Modules.Master})) order by CountryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Country,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Country",
                    ModeId = (short)Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<VesselLookupViewModel>> GetVesselLooupListAsync(string RegId, Int16 CompanyId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<VesselLookupViewModel>(RegId, $"SELECT VesselId,VesselCode,VesselName FROM M_Vessel WHERE VesselId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Vessel},{(short)Modules.Master})) order by VesselName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Vessel,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Vessel",
                    ModeId = (short)Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<BargeLookupViewModel>> GetBargeLooupListAsync(string RegId, Int16 CompanyId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<BargeLookupViewModel>(RegId, $"SELECT BargeId,BargeCode,BargeName FROM M_Barge WHERE BargeId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Barge},{(short)Modules.Master})) order by BargeName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Barge,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Barge",
                    ModeId = (short)Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
    }
}