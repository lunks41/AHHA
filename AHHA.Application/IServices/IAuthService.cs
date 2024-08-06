using AHHA.Core.Entities.Admin;
using AHHA.Core.Models;
using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices
{
    public interface IAuthService
    {
        AdmUser GetByUserName(string userName);
        bool IsAuthenticated(string userName, string password);

        Task<LoginResponse> Login(LoginViewModel user);
        Task<LoginResponse> RefreshToken(RefreshTokenModel model);
        Task<LoginResponse> Revoke(RevokeRequestModel model);

    }
}
