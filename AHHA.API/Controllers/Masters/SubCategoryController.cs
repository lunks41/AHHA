﻿using AHHA.Application.IServices;
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
    public class SubCategoryController : BaseController
    {
        private readonly ISubCategoryService _SubCategoryService;
        private readonly ILogger<SubCategoryController> _logger;

        public SubCategoryController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<SubCategoryController> logger, ISubCategoryService SubCategoryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _SubCategoryService = SubCategoryService;
        }

        [HttpGet, Route("GetSubCategory")]
        [Authorize]
        public async Task<ActionResult> GetSubCategory([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.SubCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _SubCategoryService.GetSubCategoryListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenrateMessage.authenticationfailed);

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

        [HttpGet, Route("GetSubCategorybyid/{SubCategoryId}")]
        [Authorize]
        public async Task<ActionResult<SubCategoryViewModel>> GetSubCategoryById(Int16 SubCategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.SubCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var subCategoryViewModel = _mapper.Map<SubCategoryViewModel>(await _SubCategoryService.GetSubCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SubCategoryId, headerViewModel.UserId));

                        if (subCategoryViewModel == null)
                            return NotFound(GenrateMessage.authenticationfailed);

                        return StatusCode(StatusCodes.Status202Accepted, subCategoryViewModel);
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

        [HttpPost, Route("AddSubCategory")]
        [Authorize]
        public async Task<ActionResult<SubCategoryViewModel>> CreateSubCategory(SubCategoryViewModel SubCategory, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.SubCategory, headerViewModel.UserId);

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
                                CreateById = headerViewModel.UserId,
                                IsActive = SubCategory.IsActive,
                                Remarks = SubCategory.Remarks
                            };

                            var createdSubCategory = await _SubCategoryService.AddSubCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, SubCategoryEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdSubCategory);
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
                    "Error creating new SubCategory record");
            }
        }

        [HttpPut, Route("UpdateSubCategory/{SubCategoryId}")]
        [Authorize]
        public async Task<ActionResult<SubCategoryViewModel>> UpdateSubCategory(Int16 SubCategoryId, [FromBody] SubCategoryViewModel SubCategory, [FromHeader] HeaderViewModel headerViewModel)
        {
            var SubCategoryViewModel = new SubCategoryViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.SubCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (SubCategoryId != SubCategory.SubCategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_SubCategory ID mismatch");

                            var SubCategoryToUpdate = await _SubCategoryService.GetSubCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SubCategoryId, headerViewModel.UserId);

                            if (SubCategoryToUpdate == null)
                                return NotFound($"M_SubCategory with Id = {SubCategoryId} not found");

                            var SubCategoryEntity = new M_SubCategory
                            {
                                SubCategoryCode = SubCategory.SubCategoryCode,
                                SubCategoryId = SubCategory.SubCategoryId,
                                SubCategoryName = SubCategory.SubCategoryName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = SubCategory.IsActive,
                                Remarks = SubCategory.Remarks
                            };

                            var sqlResponce = await _SubCategoryService.UpdateSubCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, SubCategoryEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteSubCategory/{SubCategoryId}")]
        [Authorize]
        public async Task<ActionResult<M_SubCategory>> DeleteSubCategory(Int16 SubCategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.SubCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var SubCategoryToDelete = await _SubCategoryService.GetSubCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SubCategoryId, headerViewModel.UserId);

                            if (SubCategoryToDelete == null)
                                return NotFound($"M_SubCategory with Id = {SubCategoryId} not found");

                            var sqlResponce = await _SubCategoryService.DeleteSubCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, SubCategoryToDelete, headerViewModel.UserId);

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