using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;
using AHHA.Infra.Data;

namespace AHHA.Infra.Services.Admin
{
    public sealed class ErrorLogService : IErrorLogService
    {
        private readonly IRepository<AdmErrorLog> _repository;
        private ApplicationDbContext _context;

        public ErrorLogService(IRepository<AdmErrorLog> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<ErrorLogViewModel>> GetErrorLogListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<ErrorLogViewModel>(RegId, $"SELECT ErrorLogId,ErrorLogName FROM AdmErrorLog ");
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
                    TblName = "GetErrorLogListAsync",
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