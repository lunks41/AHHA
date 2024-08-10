using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;
using AHHA.Core.Models.Auth;

namespace AHHA.Application.IServices
{
    public interface IAuthService
    {
        AdmUser GetByUserName(string userName);
        bool IsAuthenticated(string userName, string password);

        Task<LoginResponse> Login(LoginViewModel user);
        Task<RefreshResponse> RefreshToken(RefreshTokenModel model);
        void Revoke(RevokeRequestModel model);
        //Task<RevokeResponse> Revoke(RevokeRequestModel model);
    }
}
