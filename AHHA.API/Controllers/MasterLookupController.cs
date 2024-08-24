using AHHA.Application.IServices;
using AHHA.Core.Common;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers
{
    [Route("api/Master")]
    [ApiController]
    public class MasterLookupController : BaseController
    {
        private readonly IMasterLookupService _MasterLookupService;
        private readonly ILogger<MasterLookupController> _logger;

        public MasterLookupController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<MasterLookupController> logger, IMasterLookupService MasterLookupService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _MasterLookupService = MasterLookupService;
        }

        //create the MasterLookup for all masters

        [HttpGet, Route("GetCountryLookup")]
        [Authorize]
        public async Task<ActionResult> CountryLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetCountryLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

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
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetVesselLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

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
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetBargeLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

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

        [HttpGet, Route("GetCategoryLookup")]
        [Authorize]
        public async Task<ActionResult> CategoryLookup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //enable the cache from appsetting...
                    //cache use into only MasterLookup

                    var cacheData = await _MasterLookupService.GetCategoryLookupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

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