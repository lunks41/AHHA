﻿using AHHA.Application.IServices;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;
using AHHA.Core.Models.Auth;
using AHHA.Infra.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace AHHA.Infra.Services
{
    //Harshad
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public bool IsAuthenticated(string userName, string password, string UserPassword)
        {
            return BC.Verify(userName.ToLower().Trim() + password.Trim(), UserPassword);
        }

        public AdmUser GetByUserName(string userName) =>
            _context.AdmUser.Where(c => c.UserCode == userName).FirstOrDefault(c => c.IsActive == true);

        public AdmUser GetByRefreshToken(string RefreshToken) => _context.AdmUser.Where(c => c.RefreshToken == RefreshToken).FirstOrDefault();

        public async Task<dynamic> Login(LoginViewModel user)
        {
            try
            {
                var response = new LoginResponse();
                var identityUser = GetByUserName(user.userName.ToLower().Trim());

                if (identityUser is null || (IsAuthenticated(user.userName, user.userPassword, identityUser.UserPassword)) == false)
                {
                    var auditLog = new AdmAuditLog
                    {
                        CompanyId = 0,
                        ModuleId = (short)E_Modules.Admin,
                        TransactionId = (short)E_Admin.User,
                        DocumentId = 0,
                        DocumentNo = user.userName,
                        TblName = "AdmUser",
                        ModeId = (short)E_Mode.Login,
                        Remarks = "User Not Exist",
                        CreateById = 0,
                        CreateDate = DateTime.Now
                    };

                    _context.Add(auditLog);
                    var auditLogSave = _context.SaveChanges();

                    if (auditLogSave > 0)
                    {
                        return new LoginResponse { token = "User Not Exist", refreshToken = "" };
                    }
                    return new LoginResponse { token = "User Not Exist", refreshToken = "" };
                }

                var token = GenerateTokenString(identityUser.UserName, identityUser.UserCode, identityUser.UserId.ToString());
                response.token = new JwtSecurityTokenHandler().WriteToken(token);
                response.refreshToken = this.GenerateRefreshTokenString();

                identityUser.RefreshToken = response.refreshToken;
                identityUser.RefreshTokenExpiry = DateTime.Now.AddHours(1);

                var entity = _context.Update(identityUser);
                entity.Property(b => b.UserCode).IsModified = false;
                entity.Property(b => b.UserName).IsModified = false;
                entity.Property(b => b.UserPassword).IsModified = false;
                entity.Property(b => b.UserEmail).IsModified = false;
                entity.Property(b => b.Remarks).IsModified = false;
                entity.Property(b => b.IsActive).IsModified = false;
                entity.Property(b => b.UserGroupId).IsModified = false;

                var userloginsucess = _context.SaveChanges();

                if (userloginsucess > 0)
                {
                    var userLog = new AdmUserLog
                    {
                        UserId = identityUser.UserId,
                        IsLogin = true,
                        LoginDate = DateTime.Now,
                        Remarks = "User Exist",
                    };

                    _context.Add(userLog);
                    var userLogSave = _context.SaveChanges();

                    if (userLogSave > 0)
                    {
                        return response;
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = 0,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.User,
                    DocumentId = 0,
                    DocumentNo = user.userName,
                    TblName = "AdmUser",
                    ModeId = (short)E_Mode.Login,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = 0,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<RefreshResponse> RefreshToken(RefreshTokenModel model)
        {
            var response = new RefreshResponse();
            var identityUser = GetByRefreshToken(model.refreshToken);

            if (identityUser is null || identityUser.RefreshToken == null || identityUser.RefreshTokenExpiry < DateTime.Now)
            {
                return new RefreshResponse { token = null };//in contoller i'm sending the status
            }

            var token = GenerateTokenString(identityUser.UserName, identityUser.UserCode, identityUser.UserId.ToString());
            response.token = new JwtSecurityTokenHandler().WriteToken(token);

            identityUser.RefreshTokenExpiry = DateTime.Now.AddHours(1);

            var entity = _context.Update(identityUser);
            entity.Property(b => b.RefreshToken).IsModified = false;
            entity.Property(b => b.UserCode).IsModified = false;
            entity.Property(b => b.UserName).IsModified = false;
            entity.Property(b => b.UserPassword).IsModified = false;
            entity.Property(b => b.UserEmail).IsModified = false;
            entity.Property(b => b.Remarks).IsModified = false;
            entity.Property(b => b.IsActive).IsModified = false;
            entity.Property(b => b.UserGroupId).IsModified = false;

            var counToUpdate = _context.SaveChanges();

            return response;
        }

        private ClaimsPrincipal? GetTokenPrincipal(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:SecretKey").Value));

            var validation = new TokenValidationParameters
            {
                IssuerSigningKey = securityKey,
                ValidateLifetime = false,
                ValidateActor = false,
                ValidateIssuer = false,
                ValidateAudience = false,
            };
            return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
        }

        private string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[64];

            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        private string genrate()
        {
            var randomNumber = new byte[64];

            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        private JwtSecurityToken GenerateTokenString(string userName, string userCode, string userId)
        {
            byte[] uniqueClientIdBytes = new byte[9];
            RandomNumberGenerator.Fill(uniqueClientIdBytes);

            // Convert to a Base64 string (will be 12 characters since 9 is a multiple of 3)
            string uniqueClientId = Convert.ToBase64String(uniqueClientIdBytes);

            var claims = new List<Claim>
            {
                new Claim("userName",userName),
                new Claim("userCode",userCode),
                new Claim("userId",userId),
                new Claim("clientId ",uniqueClientId)//this for telerik reporting
            };

            var staticKey = _configuration.GetSection("Jwt:SecretKey").Value;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(staticKey));
            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var securityToken = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt16(_configuration["JWT:DurationInMinutes"])),
                signingCredentials: signingCred
                );

            return securityToken;
        }

        public void Revoke(RevokeRequestModel model)
        {
            var identityUser = GetByRefreshToken(model.refreshToken);
            try
            {
                if (identityUser != null)
                {
                    identityUser.RefreshToken = null;
                    identityUser.RefreshTokenExpiry = null;

                    var entity = _context.Update(identityUser);
                    entity.Property(b => b.UserCode).IsModified = false;
                    entity.Property(b => b.UserName).IsModified = false;
                    entity.Property(b => b.UserPassword).IsModified = false;
                    entity.Property(b => b.UserEmail).IsModified = false;
                    entity.Property(b => b.Remarks).IsModified = false;
                    entity.Property(b => b.IsActive).IsModified = false;
                    entity.Property(b => b.UserGroupId).IsModified = false;
                    var userloginsucess = _context.SaveChanges();

                    if (userloginsucess > 0)
                    {
                        var userLog = new AdmUserLog
                        {
                            UserId = identityUser.UserId,
                            IsLogin = false,
                            LoginDate = DateTime.Now,
                            Remarks = "User LogOut",
                        };

                        _context.Add(userLog);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = 0,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.User,
                    DocumentId = identityUser != null ? identityUser.UserId : 0,
                    DocumentNo = identityUser != null ? identityUser.UserCode : "",
                    TblName = "AdmUser",
                    ModeId = (short)E_Mode.Login,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = 0,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
    }
}