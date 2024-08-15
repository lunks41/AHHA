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
    public class GstCategoryController : BaseController
    {
        private readonly IGstCategoryService _GstCategoryService;
        private readonly ILogger<GstCategoryController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public GstCategoryController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<GstCategoryController> logger, IGstCategoryService GstCategoryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _GstCategoryService = GstCategoryService;
        }

        [HttpGet, Route("GetGstCategory")]
        [Authorize]
        public async Task<ActionResult> GetAllGstCategory()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.GstCategory, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<GstCategoryViewModelCount>("GstCategory");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _GstCategoryService.GetGstCategoryListAsync(CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<GstCategoryViewModelCount>("GstCategory", cacheData, expirationTime);

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

        [HttpGet, Route("GetGstCategorybyid/{GstCategoryId}")]
        [Authorize]
        public async Task<ActionResult<GstCategoryViewModel>> GetGstCategoryById(Int16 GstCategoryId)
        {
            var GstCategoryViewModel = new GstCategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.GstCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"GstCategory_{GstCategoryId}", out GstCategoryViewModel? cachedProduct))
                        {
                            GstCategoryViewModel = cachedProduct;
                        }
                        else
                        {
                            GstCategoryViewModel = _mapper.Map<GstCategoryViewModel>(await _GstCategoryService.GetGstCategoryByIdAsync(CompanyId, GstCategoryId, UserId));

                            if (GstCategoryViewModel == null)
                                return NotFound();
                            else
                                // Cache the GstCategory with an expiration time of 10 minutes
                                _memoryCache.Set($"GstCategory_{GstCategoryId}", GstCategoryViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, GstCategoryViewModel);
                        //return Ok(GstCategoryViewModel);
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

        [HttpPost, Route("AddGstCategory")]
        [Authorize]
        public async Task<ActionResult<GstCategoryViewModel>> CreateGstCategory(GstCategoryViewModel GstCategory)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.GstCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (GstCategory == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_GstCategory ID mismatch");

                            var GstCategoryEntity = new M_GstCategory
                            {
                                CompanyId = GstCategory.CompanyId,
                                GstCategoryCode = GstCategory.GstCategoryCode,
                                GstCategoryId = GstCategory.GstCategoryId,
                                GstCategoryName = GstCategory.GstCategoryName,
                                CreateById = UserId,
                                IsActive = GstCategory.IsActive,
                                Remarks = GstCategory.Remarks
                            };

                            var createdGstCategory = await _GstCategoryService.AddGstCategoryAsync(CompanyId, GstCategoryEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdGstCategory);

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
                    "Error creating new GstCategory record");
            }
        }

        [HttpPut, Route("UpdateGstCategory/{GstCategoryId}")]
        [Authorize]
        public async Task<ActionResult<GstCategoryViewModel>> UpdateGstCategory(Int16 GstCategoryId, [FromBody] GstCategoryViewModel GstCategory)
        {
            var GstCategoryViewModel = new GstCategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.GstCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (GstCategoryId != GstCategory.GstCategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_GstCategory ID mismatch");
                            //return BadRequest("M_GstCategory ID mismatch");

                            // Attempt to retrieve the GstCategory from the cache
                            if (_memoryCache.TryGetValue($"GstCategory_{GstCategoryId}", out GstCategoryViewModel? cachedProduct))
                            {
                                GstCategoryViewModel = cachedProduct;
                            }
                            else
                            {
                                var GstCategoryToUpdate = await _GstCategoryService.GetGstCategoryByIdAsync(CompanyId, GstCategoryId, UserId);

                                if (GstCategoryToUpdate == null)
                                    return NotFound($"M_GstCategory with Id = {GstCategoryId} not found");
                            }

                            var GstCategoryEntity = new M_GstCategory
                            {
                                GstCategoryCode = GstCategory.GstCategoryCode,
                                GstCategoryId = GstCategory.GstCategoryId,
                                GstCategoryName = GstCategory.GstCategoryName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = GstCategory.IsActive,
                                Remarks = GstCategory.Remarks
                            };

                            var sqlResponce = await _GstCategoryService.UpdateGstCategoryAsync(CompanyId, GstCategoryEntity, UserId);
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

        //[HttpDelete, Route("Delete/{GstCategoryId}")]
        //[Authorize]
        //public async Task<ActionResult<M_GstCategory>> DeleteGstCategory(Int16 GstCategoryId)
        //{
        //    try
        //    {
        //        CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
        //        UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

        //        if (ValidateHeaders(CompanyId, UserId))
        //        {
        //            var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.GstCategory, UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var GstCategoryToDelete = await _GstCategoryService.GetGstCategoryByIdAsync(CompanyId, GstCategoryId, UserId);

        //                    if (GstCategoryToDelete == null)
        //                        return NotFound($"M_GstCategory with Id = {GstCategoryId} not found");

        //                    var sqlResponce = await _GstCategoryService.DeleteGstCategoryAsync(CompanyId, GstCategoryToDelete, UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"GstCategory_{GstCategoryId}");
        //                    return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
        //                }
        //                else
        //                {
        //                    return NotFound("Users do not have a access to delete");
        //                }
        //            }
        //            else
        //            {
        //                return NotFound("Users not have a access for this screen");
        //            }
        //        }
        //        else
        //        {
        //            return NoContent();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            "Error deleting data");
        //    }
        //}
    }
}


