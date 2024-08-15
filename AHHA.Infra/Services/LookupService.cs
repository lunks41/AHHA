using AHHA.Application.CommonServices;
using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

        public async Task<IEnumerable<CountryLookupViewModel>> GetCountryLooupListAsync(Int16 CompanyId,Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CountryLookupViewModel>($"SELECT CountryId,CountryCode,CountryName FROM M_Country WHERE CountryId<>0 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Country},{(short)Modules.Master})) order by CountryName");

                return result;

            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Country,
                    TransactionId = (short)Modules.Master,
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
    }
}