using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace AHHA.API.Controllers.Masters
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : BaseController
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountryController> _logger;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;

        public CountryController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CountryController> logger, ICountryService countryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _countryService = countryService;
        }

        [HttpGet, Route("GetCountry")]
        [Authorize]
        public async Task<ActionResult> GetAllCountrys()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Country, UserId);

                    if (userGroupRight != null)
                    {
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<CountryViewModelCount>("country");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddMinutes(5);
                            cacheData = await _countryService.GetCountryListAsync(CompanyId, pageSize, pageNumber, UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<CountryViewModelCount>("country", cacheData, expirationTime);

                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                            //return Ok(cacheData);
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
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

        [HttpGet, Route("GetCountrybyid/{CountryId}")]
        [Authorize]
        public async Task<ActionResult<CountryViewModel>> GetCountryById(Int32 CountryId)
        {
            var countryViewModel = new CountryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Country, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"country_{CountryId}", out CountryViewModel? cachedProduct))
                        {
                            countryViewModel = cachedProduct;
                        }
                        else
                        {
                            countryViewModel = _mapper.Map<CountryViewModel>(await _countryService.GetCountryByIdAsync(CompanyId, CountryId, UserId));

                            if (countryViewModel == null)
                                return NotFound();
                            else
                                // Cache the country with an expiration time of 10 minutes
                                _memoryCache.Set($"Country_{CountryId}", countryViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, countryViewModel);
                        //return Ok(countryViewModel);
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
                    }
                }
                else
                {
                    return NoContent();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPost, Route("AddCountry")]
        [Authorize]
        public async Task<ActionResult<CountryViewModel>> CreateCountry(CountryViewModel country)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Country, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (country == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Country ID mismatch");

                            var countryEntity = new M_Country
                            {
                                CompanyId = country.CompanyId,
                                CountryCode = country.CountryCode,
                                CountryId = country.CountryId,
                                CountryName = country.CountryName,
                                CreateById = UserId,
                                IsActive = country.IsActive,
                                Remarks = country.Remarks
                            };

                            var createdCountry = await _countryService.AddCountryAsync(CompanyId, countryEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCountry);

                        }
                        else
                        {
                            return NotFound("Users do not have a access to delete");
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
                    }
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new country record");
            }
        }

        [HttpPut, Route("UpdateCountry/{CountryId}")]
        [Authorize]
        public async Task<ActionResult<CountryViewModel>> UpdateCountry(int CountryId, [FromBody] CountryViewModel country)
        {
            var countryViewModel = new CountryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Country, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (CountryId != country.CountryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Country ID mismatch");
                            //return BadRequest("M_Country ID mismatch");

                            // Attempt to retrieve the country from the cache
                            if (_memoryCache.TryGetValue($"country_{CountryId}", out CountryViewModel? cachedProduct))
                            {
                                countryViewModel = cachedProduct;
                            }
                            else
                            {
                                var CountryToUpdate = await _countryService.GetCountryByIdAsync(CompanyId, CountryId, UserId);

                                if (CountryToUpdate == null)
                                    return NotFound($"M_Country with Id = {CountryId} not found");
                            }

                            var countryEntity = new M_Country
                            {
                                CountryCode = country.CountryCode,
                                CountryId = country.CountryId,
                                CountryName = country.CountryName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = country.IsActive,
                                Remarks = country.Remarks
                            };

                            var sqlResponce = await _countryService.UpdateCountryAsync(CompanyId, countryEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
                        }
                        else
                        {
                            return NotFound("Users do not have a access to delete");
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
                    }
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [HttpDelete, Route("Delete/{CountryId}")]
        [Authorize]
        public async Task<ActionResult<M_Country>> DeleteCountry(int CountryId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Country, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var CountryToDelete = await _countryService.GetCountryByIdAsync(CompanyId, CountryId, UserId);

                            if (CountryToDelete == null)
                                return NotFound($"M_Country with Id = {CountryId} not found");

                            var sqlResponce = await _countryService.DeleteCountryAsync(CompanyId, CountryToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Country_{CountryId}");
                            return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
                        }
                        else
                        {
                            return NotFound("Users do not have a access to delete");
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
                    }
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }
}
