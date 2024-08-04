using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models;

namespace AHHA.Application.IServices.Admin
{
    public interface IUserService
    {
        public Task<AdmUser> AuthenticateAsync(Int16 CompanyId, string UserName, string UserPassword);
    }
}
