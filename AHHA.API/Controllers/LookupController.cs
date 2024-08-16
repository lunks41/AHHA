using AHHA.Application.IServices;
using AHHA.Core.Common;
using AHHA.Core.Models.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace AHHA.API.Controllers
{
    [Route("api/Master")]
    [ApiController]
    public class LookupController : BaseController
    {
        private readonly ILookupService _LookupService;
        private readonly ILogger<LookupController> _logger;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public LookupController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<LookupController> logger, ILookupService LookupService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _LookupService = LookupService;
        }

        [HttpGet, Route("GetCountryLookup")]
        [Authorize]
        public async Task<ActionResult> CountryLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var cacheData = await _LookupService.GetCountryLooupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                    //return Ok(cacheData);
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
