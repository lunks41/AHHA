using AHHA.Core.Models.Admin;
using AHHA.Core.Models.Auth;

namespace AHHA.Application.IServices
{
    public interface IAuthService
    {
        Task<dynamic> Login(LoginViewModel user);

        Task<RefreshResponse> RefreshToken(RefreshTokenModel model);

        void Revoke(RevokeRequestModel model);

        //Task<RevokeResponse> Revoke(RevokeRequestModel model);
    }
}