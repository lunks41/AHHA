using AHHA.Application.IServices;
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
    [Route("api/Admin")]
    [ApiController]
    public class UserController : BaseController
    {

        private readonly IUserService _countryService;
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<LoginViewModel> _userManager;
        private readonly IConfiguration _configuration;

        public UserController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<UserController> logger, IUserService countryService, UserManager<LoginViewModel> userManager, IConfiguration configuration)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _countryService = countryService;
            _userManager = userManager;
            _configuration = configuration;
            _configuration = configuration;
        }


        //[HttpPost]
        //[Route("register")]
        //public async Task<IActionResult> Create([FromBody] LoginViewModel model)
        //{
        //    var userExists = await _userManager.FindByNameAsync(model.UserName);
        //    if (userExists != null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new SqlResponce { Id = -1, Message = "User already exists!" });

        //    LoginViewModel user = new LoginViewModel()
        //    {
        //        UserName = model.UserName
        //    };

        //    var result = await _userManager.CreateAsync(user, model.UserPassword);

        //    if (!result.Succeeded)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new SqlResponce { Id = -1, Message = "User creation failed! Please check user details and try again." });

        //    return Ok(new SqlResponce { Id = 1, Message = "User created successfully!" });
        //}

    }
}
