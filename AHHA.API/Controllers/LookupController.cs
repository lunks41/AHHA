using AHHA.Application.IServices;
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
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
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
        public async Task<ActionResult> CountryLookup()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var cacheData = _memoryCache.Get<IEnumerable<CountryLookupViewModel>>("CountryLookup");

                    if (cacheData != null)
                        return StatusCode(StatusCodes.Status202Accepted, cacheData);
                    //return Ok(cacheData);
                    else
                    {
                        var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                        cacheData = await _LookupService.GetCountryLooupListAsync(CompanyId, UserId);

                        if (cacheData == null)
                            return NotFound();

                        _memoryCache.Set< IEnumerable<CountryLookupViewModel>>("CountryLookup", cacheData, expirationTime);

                        return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                    }
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
