using AHHA.Application.IServices;
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
        private static Dictionary<string, string> _refreshTokens = new Dictionary<string, string>();

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

            if (loginResult != null)
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

        // login
        //[AllowAnonymous]
        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        //{
        //    // Check user credentials (in a real application, you'd authenticate against a database)
        //    RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

        //    var user = _authServices.GetByUserName(model.UserName);

        //    if (_authServices.IsAuthenticated(model.UserName, model.UserPassword))
        //    {
        //        var token = GenerateAccessToken(model.UserName);
        //        // Generate refresh token
        //        var refreshToken = Guid.NewGuid().ToString();

        //        // Store the refresh token (in-memory for simplicity)
        //        _refreshTokens[refreshToken] = model.UserName;

        //        //return access token and refresh token
        //        return Ok(new
        //        {
        //            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
        //            RefreshToken = refreshToken
        //        });
        //    }
        //    // unauthorized
        //    return Unauthorized("Invalid credentials");
        //}

        //[HttpPost("genratetoken")]
        //// Generating token based on user information
        //private JwtSecurityToken GenerateAccessToken(string userName)
        //{
        //    // Create user claims
        //    var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.Name, userName)
        //    // Add additional claims as needed (e.g., roles, etc.)
        //};

        //    // Create a JWT
        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["JWT:ValidIssuer"],
        //        audience: _configuration["JWT:ValidAudience"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(1), // Token expiration time
        //        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"])),
        //            SecurityAlgorithms.HmacSha256)
        //    );

        //    return token;
        //}

        //[HttpPost("refresh")]
        //public IActionResult Refresh([FromBody] RefreshTokenModel request)
        //{
        //    if (_refreshTokens.TryGetValue(request.RefreshToken, out var userId))
        //    {
        //        // Generate a new access token
        //        var token = GenerateAccessToken(userId);

        //        // Return the new access token to the client
        //        return Ok(new { AccessToken = new JwtSecurityTokenHandler().WriteToken(token) });
        //    }

        //    return BadRequest("Invalid refresh token");
        //}

        //[HttpPost("revoke")]
        //// Example code to revoke a refresh token
        //public IActionResult Revoke([FromBody] RevokeRequest request)
        //{
        //    if (_refreshTokens.ContainsKey(request.RefreshToken))
        //    {
        //        // Remove the refresh token to revoke it
        //        _refreshTokens.Remove(request.RefreshToken);
        //        return Ok("Token revoked successfully");
        //    }

        //    return BadRequest("Invalid refresh token");
        //}
    }
}
