using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Admin;
using AHHA.Core.Entities.Admin;
using AHHA.Infra.Data;

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
    }
}