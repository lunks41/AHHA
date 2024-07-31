using Azure;
using AHHA.Application.Services.Masters.Countries;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;
using Microsoft.Extensions.Primitives;
using AutoMapper;
using AHHA.Application.CommonServices;
using Microsoft.EntityFrameworkCore;
using AHHA.Core.Models;
using System.Globalization;
using System.Configuration;
using AHHA.Core.Entities.Admin;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : BaseController
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountryController> _logger;
        private Int16 pageSize = 10;// ("pageSize"); 
        private Int16 pageNumber = 1;

        public CountryController(IMemoryCache memoryCache, IMapper mapper, ILogger<CountryController> logger, ICountryService countryService)
    : base(memoryCache, mapper)
        {
            _logger = logger;
            _countryService = countryService;
        }

        [HttpGet, Route("GetCountry")]
        public async Task<ActionResult> GetAllCountrys()
        {
            Int16 CompanyId = 0;
            Int32 UserId = 0;
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("CompanyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("UserId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                    pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;

                    //Get the data from cache memory
                    var cacheData = _memoryCache.Get<CountryViewModelCount>("country");

                    if (cacheData != null)
                        return Ok(cacheData);
                    else
                    {
                        var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
                        cacheData = await _countryService.GetCountryListAsync(CompanyId, pageSize, pageNumber, UserId);

                        if (cacheData == null)
                            return NotFound();

                        _memoryCache.Set<CountryViewModelCount>("country", cacheData, expirationTime);

                        return Ok(cacheData);
                    }
                }
                else
                {
                    if (UserId == 0)
                        return NotFound("UserId Not Found");
                    else if(CompanyId==0)
                        return NotFound("CompanyId Not Found");
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
        public async Task<ActionResult<CountryViewModel>> GetCountryById(Int32 CountryId)
        {
            Int16 CompanyId = 0;
            Int32 UserId = 0;
            var countryViewModel = new CountryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("CompanyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("UserId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
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

                    return Ok(countryViewModel);
                }
                else { return NoContent(); }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPost, Route("AddCountry")]
        public async Task<ActionResult<CountryViewModel>> CreateCountry(CountryViewModel country)
        {
            Int16 CompanyId = 0;
            Int32 UserId = 0;
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("CompanyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("UserId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    if (country == null)
                        return BadRequest();

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

                    return CreatedAtAction(nameof(GetCountryById), new { id = createdCountry.Id }, createdCountry);
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
        public async Task<ActionResult<CountryViewModel>> UpdateCountry(int CountryId, [FromBody] CountryViewModel country)
        {
            Int16 CompanyId = 0; Int32 UserId = 0;
            var countryViewModel = new CountryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("CompanyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("UserId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {

                    if (CountryId != country.CountryId)
                        return BadRequest("M_Country ID mismatch");

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

                    await _countryService.UpdateCountryAsync(CompanyId, countryEntity, UserId);
                    return NoContent();
                }
                else { return NoContent(); }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [HttpDelete, Route("Delete/{CountryId}")]
        public async Task<ActionResult<M_Country>> DeleteCountry(int CountryId)
        {
            Int16 CompanyId = 0; Int32 UserId = 0;
            var countryViewModel = new CountryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("CompanyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("UserId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {

                    // Attempt to retrieve the country from the cache
                    if (_memoryCache.TryGetValue($"country_{CountryId}", out CountryViewModel? cachedProduct))
                    {
                        countryViewModel = cachedProduct;
                    }
                    else
                    {
                        var CountryToDelete = await _countryService.GetCountryByIdAsync(CompanyId, CountryId, UserId);

                        if (CountryToDelete == null)
                            return NotFound($"M_Country with Id = {CountryId} not found");
                    }

                    await _countryService.DeleteCountryAsync(CompanyId, CountryId, UserId);
                    // Remove data from cache by key
                    _memoryCache.Remove($"Country_{CountryId}");
                    return NoContent();
                }
                else { return NoContent(); }
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
