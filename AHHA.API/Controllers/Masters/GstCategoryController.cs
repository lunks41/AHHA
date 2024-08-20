using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/Master")]
    [ApiController]
    public class GstCategoryController : BaseController
    {
        private readonly IGstCategoryService _GstCategoryService;
        private readonly ILogger<GstCategoryController> _logger;

        public GstCategoryController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<GstCategoryController> logger, IGstCategoryService GstCategoryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _GstCategoryService = GstCategoryService;
        }

        [HttpGet, Route("GetGstCategory")]
        [Authorize]
        public async Task<ActionResult> GetAllGstCategory([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GstCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _GstCategoryService.GetGstCategoryListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenrateMessage.authenticationfailed);

                        return Ok(cacheData);
                    }
                    else
                    {
                        return NotFound(GenrateMessage.authenticationfailed);
                    }
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

        [HttpGet, Route("GetGstCategorybyid/{GstCategoryId}")]
        [Authorize]
        public async Task<ActionResult<GstCategoryViewModel>> GetGstCategoryById(Int16 GstCategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var GstCategoryViewModel = new GstCategoryViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GstCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"GstCategory_{GstCategoryId}", out GstCategoryViewModel? cachedProduct))
                        {
                            GstCategoryViewModel = cachedProduct;
                        }
                        else
                        {
                            GstCategoryViewModel = _mapper.Map<GstCategoryViewModel>(await _GstCategoryService.GetGstCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstCategoryId, headerViewModel.UserId));

                            if (GstCategoryViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the GstCategory with an expiration time of 10 minutes
                                _memoryCache.Set($"GstCategory_{GstCategoryId}", GstCategoryViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, GstCategoryViewModel);
                        //return Ok(GstCategoryViewModel);
                    }
                    else
                    {
                        return NotFound(GenrateMessage.authenticationfailed);
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
        public async Task<ActionResult<GstCategoryViewModel>> CreateGstCategory(GstCategoryViewModel GstCategory, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GstCategory, headerViewModel.UserId);

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
                                CreateById = headerViewModel.UserId,
                                IsActive = GstCategory.IsActive,
                                Remarks = GstCategory.Remarks
                            };

                            var createdGstCategory = await _GstCategoryService.AddGstCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstCategoryEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdGstCategory);
                        }
                        else
                        {
                            return NotFound(GenrateMessage.authenticationfailed);
                        }
                    }
                    else
                    {
                        return NotFound(GenrateMessage.authenticationfailed);
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
        public async Task<ActionResult<GstCategoryViewModel>> UpdateGstCategory(Int16 GstCategoryId, [FromBody] GstCategoryViewModel GstCategory, [FromHeader] HeaderViewModel headerViewModel)
        {
            var GstCategoryViewModel = new GstCategoryViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GstCategory, headerViewModel.UserId);

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
                                var GstCategoryToUpdate = await _GstCategoryService.GetGstCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstCategoryId, headerViewModel.UserId);

                                if (GstCategoryToUpdate == null)
                                    return NotFound($"M_GstCategory with Id = {GstCategoryId} not found");
                            }

                            var GstCategoryEntity = new M_GstCategory
                            {
                                GstCategoryCode = GstCategory.GstCategoryCode,
                                GstCategoryId = GstCategory.GstCategoryId,
                                GstCategoryName = GstCategory.GstCategoryName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = GstCategory.IsActive,
                                Remarks = GstCategory.Remarks
                            };

                            var sqlResponce = await _GstCategoryService.UpdateGstCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstCategoryEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
                        }
                        else
                        {
                            return NotFound(GenrateMessage.authenticationfailed);
                        }
                    }
                    else
                    {
                        return NotFound(GenrateMessage.authenticationfailed);
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
        //
        //

        //        if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
        //        {
        //            var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GstCategory, headerViewModel.UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var GstCategoryToDelete = await _GstCategoryService.GetGstCategoryByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, GstCategoryId, headerViewModel.UserId);

        //                    if (GstCategoryToDelete == null)
        //                        return NotFound($"M_GstCategory with Id = {GstCategoryId} not found");

        //                    var sqlResponce = await _GstCategoryService.DeleteGstCategoryAsync(headerViewModel.RegId,headerViewModel.CompanyId, GstCategoryToDelete, headerViewModel.UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"GstCategory_{GstCategoryId}");
        //                    return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
        //                }
        //                else
        //                {
        //                    return NotFound(GenrateMessage.authenticationfailed);
        //                }
        //            }
        //            else
        //            {
        //                return NotFound(GenrateMessage.authenticationfailed);
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