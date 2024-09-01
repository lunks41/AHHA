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
        public async Task<ActionResult> GetGstCategory([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GstCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _GstCategoryService.GetGstCategoryListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, cacheData);
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
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GstCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var gstCategoryViewModel = _mapper.Map<GstCategoryViewModel>(await _GstCategoryService.GetGstCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstCategoryId, headerViewModel.UserId));

                        if (gstCategoryViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, gstCategoryViewModel);
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
        public async Task<ActionResult<GstCategoryViewModel>> CreateGstCategory(GstCategoryViewModel gstCategoryViewModel, [FromHeader] HeaderViewModel headerViewModel)
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
                            if (gstCategoryViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var GstCategoryEntity = new M_GstCategory
                            {
                                GstCategoryId = gstCategoryViewModel.GstCategoryId,
                                CompanyId = headerViewModel.CompanyId,
                                GstCategoryCode = gstCategoryViewModel.GstCategoryCode,
                                GstCategoryName = gstCategoryViewModel.GstCategoryName,
                                Remarks = gstCategoryViewModel.Remarks,
                                IsActive = gstCategoryViewModel.IsActive,
                                CreateById = headerViewModel.UserId
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
        public async Task<ActionResult<GstCategoryViewModel>> UpdateGstCategory(Int16 GstCategoryId, [FromBody] GstCategoryViewModel gstCategoryViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GstCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (GstCategoryId != gstCategoryViewModel.GstCategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "GstCategory ID mismatch");

                            var GstCategoryToUpdate = await _GstCategoryService.GetGstCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstCategoryId, headerViewModel.UserId);

                            if (GstCategoryToUpdate == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var GstCategoryEntity = new M_GstCategory
                            {
                                GstCategoryId = gstCategoryViewModel.GstCategoryId,
                                CompanyId = headerViewModel.CompanyId,
                                GstCategoryCode = gstCategoryViewModel.GstCategoryCode,
                                GstCategoryName = gstCategoryViewModel.GstCategoryName,
                                Remarks = gstCategoryViewModel.Remarks,
                                IsActive = gstCategoryViewModel.IsActive,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
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

        [HttpDelete, Route("DeleteGstCategory/{GstCategoryId}")]
        [Authorize]
        public async Task<ActionResult<M_GstCategory>> DeleteGstCategory(Int16 GstCategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GstCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var GstCategoryToDelete = await _GstCategoryService.GetGstCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstCategoryId, headerViewModel.UserId);

                            if (GstCategoryToDelete == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var sqlResponce = await _GstCategoryService.DeleteGstCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstCategoryToDelete, headerViewModel.UserId);

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
                    "Error deleting data");
            }
        }
    }
}