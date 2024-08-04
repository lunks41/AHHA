using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Admin;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models;
using AHHA.Infra.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AHHA.Infra.Services.Admin
{
    public sealed class UserService : IUserService
    {
        private readonly IRepository<AdmUser> _repository;
        private ApplicationDbContext _context;

        public UserService(IRepository<AdmUser> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<AdmUser> AuthenticateAsync(Int16 CompanyId, string UserName, string UserPassword)
        {
            var parameters = new DynamicParameters();
            try
            {
               
                return null;
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
                    TblName = "AdmUser",
                    ModeId = 0,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = CompanyId,
                    //CreateDate = DateTime.Now
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

    }
}
