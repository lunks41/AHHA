using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;
using AHHA.Infra.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AHHA.Infra.Services.Admin
{
    public sealed class ModuleService : IModuleService
    {
        private readonly IRepository<AdmModule> _repository;
        private ApplicationDbContext _context;

        public ModuleService(IRepository<AdmModule> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<AdmModule>> GetUsersModulesAsync(Int16 CompanyId, Int32 UserId)
        {
            try
            {
                var parameter = new List<SqlParameter>();
                parameter.Add(new SqlParameter("@CompanyId", CompanyId));
                parameter.Add(new SqlParameter("@UserId", UserId));

                var productDetails = await Task.Run(() => _context.AdmModule
                                .FromSqlRaw(@"exec Adm_GetUserModules @CompanyId,@UserId", parameter.ToArray()).ToListAsync());

                return productDetails;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Admin,
                    TransactionId = (short)Admins.Modules,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GetUsersModulesAsync",
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
