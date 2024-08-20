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

        public LookupController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<LookupController> logger, ILookupService LookupService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _LookupService = LookupService;
        }

        //create the lookup for all masters 

        [HttpGet, Route("GetCountryLookup")]
        [Authorize]
        public async Task<ActionResult> CountryLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only lookup

                    var cacheData = await _LookupService.GetCountryLooupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                    //return Ok(cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetVesselLookup")]
        [Authorize]
        public async Task<ActionResult> VesselLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only lookup

                    var cacheData = await _LookupService.GetVesselLooupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                    //return Ok(cacheData);
                }
                else
                {
                    
                        return NotFound(GenrateMessage.authenticationfailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetBargeLookup")]
        [Authorize]
        public async Task<ActionResult> BargeLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only lookup

                    var cacheData = await _LookupService.GetBargeLooupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, cacheData);
                    //return Ok(cacheData);
                }
                else
                {
                    return NotFound(GenrateMessage.authenticationfailed);
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
