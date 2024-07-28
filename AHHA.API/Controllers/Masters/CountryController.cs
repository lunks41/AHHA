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
using AHHA.API.Models;
using AutoMapper;
using AHHA.Application.CommonServices;
using Microsoft.EntityFrameworkCore;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountryController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public CountryController(ILogger<CountryController> logger, ICountryService countryService, IMemoryCache memoryCache, IMapper mapper)
        {
            _logger = logger;
            _countryService = countryService;
            _memoryCache = memoryCache;
            _mapper = mapper;
        }

        [HttpGet, Route("GetCountry")]
        [ProducesResponseType(typeof(List<CountryViewModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetAllCountrys()
        {
            byte CompanyId = 0;
            try
            {
                //Get the data from header
                if (Request.Headers.TryGetValue("CompanyId", out StringValues headerValue))
                    CompanyId = Convert.ToByte(headerValue[0]);
                else
                    return NoContent();

                //Get the data from cache memory
                var cacheData = _memoryCache.Get<IEnumerable<CountryViewModel>>("country");

                if (cacheData != null)
                    return Ok(cacheData);
                else
                {
                    var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
                    cacheData = _mapper.Map<IEnumerable<CountryViewModel>>(await _countryService.GetCountryListAsync(Convert.ToByte(CompanyId)));

                    if (cacheData == null)
                        return NotFound();

                    _memoryCache.Set<IEnumerable<CountryViewModel>>("country", cacheData, expirationTime);
                    return Ok(cacheData);
                }

                //var Countrys = await _countryService.GetCountryListAsync(Convert.ToByte(CompanyId));
                //return Ok(_countryViewModelMapper.MapList(Countrys));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }


        [HttpGet, Route("GetCountrybyid/{id:int}")]
        public async Task<ActionResult<CountryViewModel>> GetCountryById(int id)
        {
            byte CompanyId = 0;
            var countryViewModel = new CountryViewModel();
            try
            {
                if (Request.Headers.TryGetValue("CompanyId", out StringValues headerValue))
                    CompanyId = Convert.ToByte(headerValue[0]);
                else
                    return NoContent();

                // Attempt to retrieve the country from the cache
                if (_memoryCache.TryGetValue($"country_{id}", out CountryViewModel? cachedProduct))
                {
                    countryViewModel = cachedProduct;
                }
                else
                {
                    countryViewModel = _mapper.Map<CountryViewModel>(await _countryService.GetCountryByIdAsync(id,CompanyId));

                    if (countryViewModel == null)
                        return NotFound();
                    else
                        // Cache the country with an expiration time of 10 minutes
                        _memoryCache.Set($"Country_{id}", countryViewModel, TimeSpan.FromMinutes(10));
                }

                return countryViewModel;
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
            byte CompanyId = 0;
            try
            {
                if (Request.Headers.TryGetValue("CompanyId", out StringValues headerValue))
                    CompanyId = Convert.ToByte(headerValue[0]);
                else
                    return NoContent();

                if (country == null)
                    return BadRequest();

                var countryEntity = new M_Country
                {
                    CompanyId = country.CompanyId,
                    CountryCode = country.CountryCode,
                    CountryId = country.CountryId,
                    CountryName = country.CountryName,
                    CreateById = 1,
                    //CreateDate = country.CreateDate,
                    //EditById = country.EditById,
                    //EditDate = country.EditDate,
                    IsActive = country.IsActive,
                    Remarks = country.Remarks
                };

                var createdCountry = await _countryService.AddCountryAsyncV1(countryEntity);

                return CreatedAtAction(nameof(GetCountryById), new { id = createdCountry.Id }, createdCountry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new country record");
            }
        }

        [HttpPut, Route("UpdateCountry/{id:int}")]
        public async Task<ActionResult<CountryViewModel>> UpdateCountry(int id, [FromBody] CountryViewModel country)
        {
            byte CompanyId = 0;
            var countryViewModel = new CountryViewModel();
            try
            {
                if (Request.Headers.TryGetValue("CompanyId", out StringValues headerValue))
                    CompanyId = Convert.ToByte(headerValue[0]);
                else
                    return NoContent();

                if (id != country.CountryId)
                    return BadRequest("M_Country ID mismatch");

                // Attempt to retrieve the country from the cache
                if (_memoryCache.TryGetValue($"country_{id}", out CountryViewModel? cachedProduct))
                {
                    countryViewModel = cachedProduct;
                }
                else
                {
                    var CountryToUpdate = await _countryService.GetCountryByIdAsync(id, CompanyId);

                    if (CountryToUpdate == null)
                        return NotFound($"M_Country with Id = {id} not found");
                }

                var countryEntity = new M_Country
                {
                    //CompanyId = country.CompanyId,
                    CountryCode = country.CountryCode,
                    CountryId = country.CountryId,
                    CountryName = country.CountryName,
                    //CreateById = country.CreateById,
                    //CreateDate = country.CreateDate,
                    EditById = 1,
                    EditDate = DateTime.Now,
                    IsActive = country.IsActive,
                    Remarks = country.Remarks
                };

                await _countryService.UpdateCountryAsyncV1(countryEntity);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [HttpDelete, Route("Delete/{id:int}")]
        public async Task<ActionResult<M_Country>> DeleteCountry(int id)
        {
            byte CompanyId = 0;
            var countryViewModel = new CountryViewModel();
            try
            {
                if (Request.Headers.TryGetValue("CompanyId", out StringValues headerValue))
                    CompanyId = Convert.ToByte(headerValue[0]);
                else
                    return NoContent();

                // Attempt to retrieve the country from the cache
                if (_memoryCache.TryGetValue($"country_{id}", out CountryViewModel? cachedProduct))
                {
                    countryViewModel = cachedProduct;
                }
                else
                {
                    var CountryToDelete = await _countryService.GetCountryByIdAsync(id, CompanyId);

                    if (CountryToDelete == null)
                        return NotFound($"M_Country with Id = {id} not found");
                }

                await _countryService.DeleteCountryAsync(id,CompanyId);
                // Remove data from cache by key
                _memoryCache.Remove($"Country_{id}");
                return NoContent();
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
