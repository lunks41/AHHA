using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;
using AHHA.Infra.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace AHHA.Infra.Services.Admin
{
    public sealed class CompanyService : ICompanyService
    {
        private readonly IRepository<AdmCompany> _repository;
        private ApplicationDbContext _context;

        public CompanyService(IRepository<AdmCompany> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<CompanyViewModel>> GetUserCompanyListAsync(string RegId, Int32 UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<CompanyViewModel>(RegId,$"SELECT CompanyId,CompanyName FROM AdmCompany WHERE IsActive=1 AND CompanyId IN (SELECT CompanyId FROM AdmUserRights WHERE UserId={UserId})");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = 0,
                    ModuleId = (short)Modules.Admin,
                    TransactionId = (short)Admins.User,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GetUserLoginCompany",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

    }
}
