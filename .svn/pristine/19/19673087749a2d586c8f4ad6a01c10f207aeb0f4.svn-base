using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Admin
{
    [Route("api/Admin")]
    [ApiController]
    public class AuditLogController : BaseController
    {
        private readonly IAuditLogService _AuditLogService;
        private readonly ILogger<AuditLogController> _logger;

        public AuditLogController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<AuditLogController> logger, IAuditLogService AuditLogService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _AuditLogService = AuditLogService;
        }

        [HttpGet, Route("GetAuditLog")]
        [Authorize]
        public async Task<ActionResult> GetAllAuditLog([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        headerViewModel.searchString = headerViewModel.searchString == null ? string.Empty : headerViewModel.searchString.Trim();

                        var AuditLogData = await _AuditLogService.GetAuditLogListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (AuditLogData == null)
                            return NotFound();

                        return StatusCode(StatusCodes.Status202Accepted, AuditLogData);
                    }
                    else
                    {
                        return NotFound(GenrateMessage.authenticationfailed);
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