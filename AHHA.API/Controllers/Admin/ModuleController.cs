using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
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
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;

        public ModuleController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<ModuleController> logger, IModuleService moduleService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _moduleService = moduleService;
        }

        //        To filter the modules based on the CompanyId and UserId:
        //Get call
        //http://118.189.194.191:8080/ahharestapiproject/ahha/getUsersModules/{companyid}/{userid}
        //http://118.189.194.191:8080/ahharestapiproject/ahha/getUsersModules/1/1
        ////http://118.189.194.191:8080/api/

        [HttpGet, Route("GetUsersModules")]
        [Authorize]
        public async Task<ActionResult> GetUsersModules()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId, CompanyId, UserId))
                {
                    var UsersModulesdata = await _moduleService.GetUsersModulesAsync(RegId,CompanyId, UserId);

                    return Ok(UsersModulesdata);
                }
                else
                {
                    if (UserId == 0)
                        return NotFound("UserId Not Found");
                    else if (CompanyId == 0)
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
