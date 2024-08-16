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

    [Route("api/Master")]
    [ApiController]
    public class CountryController : BaseController
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountryController> _logger;

        public CountryController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CountryController> logger, ICountryService countryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _countryService = countryService;
        }

        [HttpGet, Route("GetCountry")]
        [Authorize]
        public async Task<ActionResult> GetAllCountry([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Country, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<CountryViewModelCount>("country");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _countryService.GetCountryListAsync(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

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

        [HttpGet, Route("GetCountrybyid/{CountryId}")]
        [Authorize]
        public async Task<ActionResult<CountryViewModel>> GetCountryById(Int32 CountryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var countryViewModel = new CountryViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Country, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"country_{CountryId}", out CountryViewModel? cachedProduct))
                        {
                            countryViewModel = cachedProduct;
                        }
                        else
                        {
                            countryViewModel = _mapper.Map<CountryViewModel>(await _countryService.GetCountryByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, CountryId, headerViewModel.UserId));

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
        public async Task<ActionResult<CountryViewModel>> CreateCountry(CountryViewModel country, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Country, headerViewModel.UserId);

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
                                CreateById = headerViewModel.UserId,
                                IsActive = country.IsActive,
                                Remarks = country.Remarks
                            };

                            var createdCountry = await _countryService.AddCountryAsync(headerViewModel.RegId,headerViewModel.CompanyId, countryEntity, headerViewModel.UserId);
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
        public async Task<ActionResult<CountryViewModel>> UpdateCountry(int CountryId, [FromBody] CountryViewModel country, [FromHeader] HeaderViewModel headerViewModel)
        {
            var countryViewModel = new CountryViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Country, headerViewModel.UserId);

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
                                var CountryToUpdate = await _countryService.GetCountryByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, CountryId, headerViewModel.UserId);

                                if (CountryToUpdate == null)
                                    return NotFound($"M_Country with Id = {CountryId} not found");
                            }

                            var countryEntity = new M_Country
                            {
                                CountryCode = country.CountryCode,
                                CountryId = country.CountryId,
                                CountryName = country.CountryName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = country.IsActive,
                                Remarks = country.Remarks
                            };

                            var sqlResponce = await _countryService.UpdateCountryAsync(headerViewModel.RegId,headerViewModel.CompanyId, countryEntity, headerViewModel.UserId);
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
        public async Task<ActionResult<M_Country>> DeleteCountry(int CountryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Country, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var CountryToDelete = await _countryService.GetCountryByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, CountryId, headerViewModel.UserId);

                            if (CountryToDelete == null)
                                return NotFound($"M_Country with Id = {CountryId} not found");

                            var sqlResponce = await _countryService.DeleteCountryAsync(headerViewModel.RegId,headerViewModel.CompanyId, CountryToDelete, headerViewModel.UserId);
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
