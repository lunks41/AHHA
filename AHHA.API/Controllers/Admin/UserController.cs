using AHHA.Application.IServices.Admin;
using AHHA.Core.Common;
using AHHA.Core.Models.Admin;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AHHA.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        
        private readonly IUserService _countryService;
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<LoginViewModel> _userManager;
        private readonly IConfiguration _configuration;

        public UserController(IMemoryCache memoryCache, IMapper mapper, ILogger<UserController> logger, IUserService countryService, UserManager<LoginViewModel> userManager, IConfiguration configuration)
    : base(memoryCache, mapper)
        {
            _logger = logger;
            _countryService = countryService;
            _userManager = userManager;
            _configuration = configuration;
          _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            //Check the user
            //check the user and password
            //genrate the token
            //regenrate the token before the time expiring
            //If logout then automalltically Token,session,cache

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.UserPassword))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }







        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] LoginViewModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new SqlResponce { Id = -1, Message = "User already exists!" });

            LoginViewModel user = new LoginViewModel()
            {
                UserName = model.UserName
            };

            var result = await _userManager.CreateAsync(user, model.UserPassword);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new SqlResponce { Id = -1, Message = "User creation failed! Please check user details and try again." });

            return Ok(new SqlResponce { Id = 1, Message = "User created successfully!" });
        }

    }
}
