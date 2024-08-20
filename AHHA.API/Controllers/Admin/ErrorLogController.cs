using AHHA.API.Controllers.Masters;
using AHHA.Application.IServices.Masters;
using AHHA.Application.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using AHHA.Core.Common;
using AHHA.Infra.Services.Masters;
using Microsoft.AspNetCore.Authorization;

namespace AHHA.API.Controllers.Admin
{
    [Route("api/Admin")]
    [ApiController]
    public class ErrorLogController : BaseController
    {
        private readonly IErrorLogService _ErrorLogService;
        private readonly ILogger<ErrorLogController> _logger;

        public ErrorLogController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<ErrorLogController> logger, IErrorLogService ErrorLogService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _ErrorLogService = ErrorLogService;
        }

        [HttpGet, Route("GetErrorLog")]
        [Authorize]
        public async Task<ActionResult> GetAllErrorLog([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        headerViewModel.searchString = headerViewModel.searchString == null ? string.Empty : headerViewModel.searchString.Trim();

                        var ErrorLogData = await _ErrorLogService.GetErrorLogListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (ErrorLogData == null)
                            return NotFound();

                        return StatusCode(StatusCodes.Status202Accepted, ErrorLogData);
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
                    }
                }
                else
                {
                    if (headerViewModel.UserId == 0)
                        return NotFound("headerViewModel.UserId Not Found");
                    else if (headerViewModel.CompanyId == 0)
                        return NotFound("headerViewModel.CompanyId Not Found");
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
