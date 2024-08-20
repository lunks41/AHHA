using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace AHHA.API.Controllers.Admin
{
    [Route("api/Admin")]
    [ApiController]
    public class ModuleController : BaseController
    {
        private readonly IModuleService _moduleService;
        private readonly ILogger<ModuleController> _logger;

        public ModuleController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<ModuleController> logger, IModuleService moduleService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _moduleService = moduleService;
        }

        [HttpGet, Route("GetUsersModules")]
        [Authorize]
        public async Task<ActionResult> GetUsersModules([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var UsersModulesdata = await _moduleService.GetUsersModulesAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return Ok(UsersModulesdata);
                }
                else
                {
                    if (headerViewModel.UserId == 0)
                        return NotFound("UserId Not Found");
                    else if (headerViewModel.CompanyId == 0)
                        return NotFound("CompanyId Not Found");
                    else
                        return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

    }
}
