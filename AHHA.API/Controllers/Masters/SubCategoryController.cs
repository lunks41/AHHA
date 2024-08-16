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
    public class SubCategoryController : BaseController
    {
        private readonly ISubCategoryService _SubCategoryService;
        private readonly ILogger<SubCategoryController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public SubCategoryController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<SubCategoryController> logger, ISubCategoryService SubCategoryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _SubCategoryService = SubCategoryService;
        }

        [HttpGet, Route("GetSubCategory")]
        [Authorize]
        public async Task<ActionResult> GetAllSubCategory()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.SubCategory, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<SubCategoryViewModelCount>("SubCategory");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _SubCategoryService.GetSubCategoryListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<SubCategoryViewModelCount>("SubCategory", cacheData, expirationTime);

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

        [HttpGet, Route("GetSubCategorybyid/{SubCategoryId}")]
        [Authorize]
        public async Task<ActionResult<SubCategoryViewModel>> GetSubCategoryById(Int16 SubCategoryId)
        {
            var SubCategoryViewModel = new SubCategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.SubCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"SubCategory_{SubCategoryId}", out SubCategoryViewModel? cachedProduct))
                        {
                            SubCategoryViewModel = cachedProduct;
                        }
                        else
                        {
                            SubCategoryViewModel = _mapper.Map<SubCategoryViewModel>(await _SubCategoryService.GetSubCategoryByIdAsync(RegId,CompanyId, SubCategoryId, UserId));

                            if (SubCategoryViewModel == null)
                                return NotFound();
                            else
                                // Cache the SubCategory with an expiration time of 10 minutes
                                _memoryCache.Set($"SubCategory_{SubCategoryId}", SubCategoryViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, SubCategoryViewModel);
                        //return Ok(SubCategoryViewModel);
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

        [HttpPost, Route("AddSubCategory")]
        [Authorize]
        public async Task<ActionResult<SubCategoryViewModel>> CreateSubCategory(SubCategoryViewModel SubCategory)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.SubCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (SubCategory == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_SubCategory ID mismatch");

                            var SubCategoryEntity = new M_SubCategory
                            {
                                CompanyId = SubCategory.CompanyId,
                                SubCategoryCode = SubCategory.SubCategoryCode,
                                SubCategoryId = SubCategory.SubCategoryId,
                                SubCategoryName = SubCategory.SubCategoryName,
                                CreateById = UserId,
                                IsActive = SubCategory.IsActive,
                                Remarks = SubCategory.Remarks
                            };

                            var createdSubCategory = await _SubCategoryService.AddSubCategoryAsync(RegId,CompanyId, SubCategoryEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdSubCategory);

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
                    "Error creating new SubCategory record");
            }
        }

        [HttpPut, Route("UpdateSubCategory/{SubCategoryId}")]
        [Authorize]
        public async Task<ActionResult<SubCategoryViewModel>> UpdateSubCategory(Int16 SubCategoryId, [FromBody] SubCategoryViewModel SubCategory)
        {
            var SubCategoryViewModel = new SubCategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.SubCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (SubCategoryId != SubCategory.SubCategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_SubCategory ID mismatch");
                            //return BadRequest("M_SubCategory ID mismatch");

                            // Attempt to retrieve the SubCategory from the cache
                            if (_memoryCache.TryGetValue($"SubCategory_{SubCategoryId}", out SubCategoryViewModel? cachedProduct))
                            {
                                SubCategoryViewModel = cachedProduct;
                            }
                            else
                            {
                                var SubCategoryToUpdate = await _SubCategoryService.GetSubCategoryByIdAsync(RegId,CompanyId, SubCategoryId, UserId);

                                if (SubCategoryToUpdate == null)
                                    return NotFound($"M_SubCategory with Id = {SubCategoryId} not found");
                            }

                            var SubCategoryEntity = new M_SubCategory
                            {
                                SubCategoryCode = SubCategory.SubCategoryCode,
                                SubCategoryId = SubCategory.SubCategoryId,
                                SubCategoryName = SubCategory.SubCategoryName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = SubCategory.IsActive,
                                Remarks = SubCategory.Remarks
                            };

                            var sqlResponce = await _SubCategoryService.UpdateSubCategoryAsync(RegId,CompanyId, SubCategoryEntity, UserId);
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

        [HttpDelete, Route("Delete/{SubCategoryId}")]
        [Authorize]
        public async Task<ActionResult<M_SubCategory>> DeleteSubCategory(Int16 SubCategoryId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.SubCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var SubCategoryToDelete = await _SubCategoryService.GetSubCategoryByIdAsync(RegId,CompanyId, SubCategoryId, UserId);

                            if (SubCategoryToDelete == null)
                                return NotFound($"M_SubCategory with Id = {SubCategoryId} not found");

                            var sqlResponce = await _SubCategoryService.DeleteSubCategoryAsync(RegId,CompanyId, SubCategoryToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"SubCategory_{SubCategoryId}");
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

