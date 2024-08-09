﻿using AHHA.Application.IServices;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models;
using AHHA.Core.Models.Admin;
using AHHA.Infra.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace AHHA.Infra.Services
{
    public class AuthService : IAuthService
    {
        private ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public bool IsAuthenticated(string userName, string password)
        {
            var user = this.GetByUserName(userName);
            return this.DoesUserExists(userName) && BC.Verify(userName + password, user.UserPassword);
        }

        public bool DoesUserExists(string userName)
        {
            var user = _context.AdmUser.FirstOrDefault(x => x.UserName == userName);
            return user != null;
        }

        public AdmUser GetById(Int32 userId)
        {
            return _context.AdmUser.FirstOrDefault(c => c.UserId == userId);
        }

        public AdmUser GetByUserName(string userName)
        {
            return _context.AdmUser.Where(c => c.UserName == userName).FirstOrDefault();
        }

        public AdmUser GetByRefreshToken(string RefreshToken)
        {
            return _context.AdmUser.Where(c => c.RefreshToken == RefreshToken).FirstOrDefault();
        }

        public async Task<LoginResponse> Login(LoginViewModel user)
        {
            var response = new LoginResponse();
            var identityUser = GetByUserName(user.UserName);

            if (identityUser.RefreshToken != null)
            {
                //return new LoginResponse { token = null, refreshToken = null, message = "you alreday login in another system" }; //Disscusion Pending with Ravi sir
                return new LoginResponse { token = null, refreshToken = null };
            }

            if (identityUser is null || (IsAuthenticated(user.UserName, user.UserPassword)) == false)
            {
                return new LoginResponse { token = null, refreshToken = null };
            }

            var token = GenerateTokenString(identityUser.UserName, identityUser.UserId);
            //response.IsLogedIn = true;
            response.token = new JwtSecurityTokenHandler().WriteToken(token);
            response.refreshToken = this.GenerateRefreshTokenString();
            //response.Expiration = token.ValidTo;

            identityUser.RefreshToken = response.refreshToken;
            identityUser.RefreshTokenExpiry = DateTime.Now.AddHours(12);

            //update the user table
            var entity = _context.Update(identityUser);

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

        public async Task<RefreshResponse> RefreshToken(RefreshTokenModel model)
        {
            //Check by UserId
            //var principal = GetTokenPrincipal(model.token);

            var response = new RefreshResponse();

            //if (principal?.Identity?.Name is null)
            //    return response;

            //var identityUser = GetByUserName(principal.Identity.Name);
            var identityUser = GetByRefreshToken(model.refreshToken);

            //if (identityUser is null || identityUser.RefreshToken != model.RefreshToken || identityUser.RefreshTokenExpiry < DateTime.Now)
            //    return response;

            if (identityUser is null || identityUser.RefreshToken == null || identityUser.RefreshTokenExpiry < DateTime.Now)
            {
                return new RefreshResponse { token = null };
            }

            var token = GenerateTokenString(identityUser.UserName, identityUser.UserId);
            response.token = new JwtSecurityTokenHandler().WriteToken(token);
            identityUser.RefreshTokenExpiry = DateTime.Now.AddHours(1);

            //update the user table

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

        private JwtSecurityToken GenerateTokenString(string userName, Int32 userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,userName),
                //new Claim(UserId,userId),
            };

            var staticKey = _configuration.GetSection("Jwt:SecretKey").Value;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(staticKey));
            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var securityToken = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddSeconds(60),
                signingCredentials: signingCred
                );

            //string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return securityToken;
        }

        public void Revoke(RevokeRequestModel model)   //Diccusss with Ravi Sir
        {
            //var principal = GetTokenPrincipal(model.token);

            //var response = new LoginResponse();

            //if (principal?.Identity?.Name is null)
            //    return response;

            //var identityUser = GetByUserName(principal.Identity.Name);

            var identityUser = GetByRefreshToken(model.refreshToken);

            //if (identityUser is null || identityUser.RefreshToken != model.RefreshToken || identityUser.RefreshTokenExpiry < DateTime.Now)
            //    return response;

            //if (identityUser is null || identityUser.RefreshToken != null || identityUser.RefreshTokenExpiry < DateTime.Now)
            //{
            //    return new RevokeResponse { IsLogout = false };
            //}

            identityUser.RefreshToken = null;
            identityUser.RefreshTokenExpiry = null;
            //update the user table

            var entity = _context.Update(identityUser);

            entity.Property(b => b.UserCode).IsModified = false;
            entity.Property(b => b.UserName).IsModified = false;
            entity.Property(b => b.UserPassword).IsModified = false;
            entity.Property(b => b.UserEmail).IsModified = false;
            entity.Property(b => b.Remarks).IsModified = false;
            entity.Property(b => b.IsActive).IsModified = false;
            entity.Property(b => b.UserGroupId).IsModified = false;
            _context.SaveChanges();
            // var counToUpdate = _context.SaveChanges();

            //return response;
        }
    }
}