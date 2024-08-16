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
    public class TaxCategoryController : BaseController
    {
        private readonly ITaxCategoryService _TaxCategoryService;
        private readonly ILogger<TaxCategoryController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public TaxCategoryController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<TaxCategoryController> logger, ITaxCategoryService TaxCategoryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _TaxCategoryService = TaxCategoryService;
        }

        [HttpGet, Route("GetTaxCategory")]
        [Authorize]
        public async Task<ActionResult> GetAllTaxCategory()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.TaxCategory, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<TaxCategoryViewModelCount>("TaxCategory");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _TaxCategoryService.GetTaxCategoryListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<TaxCategoryViewModelCount>("TaxCategory", cacheData, expirationTime);

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

        [HttpGet, Route("GetTaxCategorybyid/{TaxCategoryId}")]
        [Authorize]
        public async Task<ActionResult<TaxCategoryViewModel>> GetTaxCategoryById(Int16 TaxCategoryId)
        {
            var TaxCategoryViewModel = new TaxCategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.TaxCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"TaxCategory_{TaxCategoryId}", out TaxCategoryViewModel? cachedProduct))
                        {
                            TaxCategoryViewModel = cachedProduct;
                        }
                        else
                        {
                            TaxCategoryViewModel = _mapper.Map<TaxCategoryViewModel>(await _TaxCategoryService.GetTaxCategoryByIdAsync(RegId,CompanyId, TaxCategoryId, UserId));

                            if (TaxCategoryViewModel == null)
                                return NotFound();
                            else
                                // Cache the TaxCategory with an expiration time of 10 minutes
                                _memoryCache.Set($"TaxCategory_{TaxCategoryId}", TaxCategoryViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, TaxCategoryViewModel);
                        //return Ok(TaxCategoryViewModel);
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

        [HttpPost, Route("AddTaxCategory")]
        [Authorize]
        public async Task<ActionResult<TaxCategoryViewModel>> CreateTaxCategory(TaxCategoryViewModel TaxCategory)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.TaxCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (TaxCategory == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_TaxCategory ID mismatch");

                            var TaxCategoryEntity = new M_TaxCategory
                            {
                                CompanyId = TaxCategory.CompanyId,
                                TaxCategoryCode = TaxCategory.TaxCategoryCode,
                                TaxCategoryId = TaxCategory.TaxCategoryId,
                                TaxCategoryName = TaxCategory.TaxCategoryName,
                                CreateById = UserId,
                                IsActive = TaxCategory.IsActive,
                                Remarks = TaxCategory.Remarks
                            };

                            var createdTaxCategory = await _TaxCategoryService.AddTaxCategoryAsync(RegId,CompanyId, TaxCategoryEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdTaxCategory);

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
                    "Error creating new TaxCategory record");
            }
        }

        [HttpPut, Route("UpdateTaxCategory/{TaxCategoryId}")]
        [Authorize]
        public async Task<ActionResult<TaxCategoryViewModel>> UpdateTaxCategory(Int16 TaxCategoryId, [FromBody] TaxCategoryViewModel TaxCategory)
        {
            var TaxCategoryViewModel = new TaxCategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.TaxCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (TaxCategoryId != TaxCategory.TaxCategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_TaxCategory ID mismatch");
                            //return BadRequest("M_TaxCategory ID mismatch");

                            // Attempt to retrieve the TaxCategory from the cache
                            if (_memoryCache.TryGetValue($"TaxCategory_{TaxCategoryId}", out TaxCategoryViewModel? cachedProduct))
                            {
                                TaxCategoryViewModel = cachedProduct;
                            }
                            else
                            {
                                var TaxCategoryToUpdate = await _TaxCategoryService.GetTaxCategoryByIdAsync(RegId,CompanyId, TaxCategoryId, UserId);

                                if (TaxCategoryToUpdate == null)
                                    return NotFound($"M_TaxCategory with Id = {TaxCategoryId} not found");
                            }

                            var TaxCategoryEntity = new M_TaxCategory
                            {
                                TaxCategoryCode = TaxCategory.TaxCategoryCode,
                                TaxCategoryId = TaxCategory.TaxCategoryId,
                                TaxCategoryName = TaxCategory.TaxCategoryName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = TaxCategory.IsActive,
                                Remarks = TaxCategory.Remarks
                            };

                            var sqlResponce = await _TaxCategoryService.UpdateTaxCategoryAsync(RegId,CompanyId, TaxCategoryEntity, UserId);
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

        [HttpDelete, Route("Delete/{TaxCategoryId}")]
        [Authorize]
        public async Task<ActionResult<M_TaxCategory>> DeleteTaxCategory(Int16 TaxCategoryId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.TaxCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var TaxCategoryToDelete = await _TaxCategoryService.GetTaxCategoryByIdAsync(RegId,CompanyId, TaxCategoryId, UserId);

                            if (TaxCategoryToDelete == null)
                                return NotFound($"M_TaxCategory with Id = {TaxCategoryId} not found");

                            var sqlResponce = await _TaxCategoryService.DeleteTaxCategoryAsync(RegId,CompanyId, TaxCategoryToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"TaxCategory_{TaxCategoryId}");
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


