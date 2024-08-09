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

    }
}
