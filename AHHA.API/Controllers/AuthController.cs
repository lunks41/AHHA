﻿using AHHA.Application.IServices;
using AHHA.Core.Models.Admin;
using AHHA.Core.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AHHA.API.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authServices;

        public AuthController(IConfiguration configuration, IAuthService authServices)
        {
            _configuration = configuration;
            _authServices = authServices;
        }

        //Implimenting in future
        //check the different location
        //save the user computer data.  (like which OS? OS address? Ip address? )
        //write the server log. when users try to password three times then user will be locked.

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var loginResult = await _authServices.Login(user);

            if (loginResult != null && loginResult.token != "User Not Exist")
            {
                return Ok(loginResult);
            }
            return Unauthorized("Invalid credentials");
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenModel model)
        {
            var refreshResult = await _authServices.RefreshToken(model);
            if (refreshResult != null)
            {
                if (refreshResult.token == null)
                    return StatusCode(StatusCodes.Status401Unauthorized, new RefreshResponse { token = "token not vaild" });
                else
                    return Ok(refreshResult);
            }

            return Unauthorized();
        }

        [HttpPost("revoke")]
        public IActionResult Revoke(RevokeRequestModel model)
        {
            _authServices.Revoke(model);
            return Ok();
        }
    }
}