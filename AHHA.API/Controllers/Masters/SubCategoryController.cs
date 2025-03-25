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
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.SubCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _SubCategoryService.GetSubCategoryListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, cacheData);
                    }
                    else
                    {
                        return NotFound(GenerateMessage.AuthenticationFailed);
                    }
                }
                else
                {
                    return NotFound(GenerateMessage.AuthenticationFailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.SubCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var subCategoryViewModel = _mapper.Map<SubCategoryViewModel>(await _SubCategoryService.GetSubCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SubCategoryId, headerViewModel.UserId));

                        if (subCategoryViewModel == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, subCategoryViewModel);
                    }
                    else
                    {
                        return NotFound(GenerateMessage.AuthenticationFailed);
                    }
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPost, Route("SaveSubCategory")]
        [Authorize]
        public async Task<ActionResult<SubCategoryViewModel>> SaveSubCategory(SubCategoryViewModel subCategoryViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.SubCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (subCategoryViewModel == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var SubCategoryEntity = new M_SubCategory
                            {
                                SubCategoryId = subCategoryViewModel.SubCategoryId,
                                CompanyId = headerViewModel.CompanyId,
                                SubCategoryCode = subCategoryViewModel.SubCategoryCode?.Trim() ?? string.Empty,
                                SubCategoryName = subCategoryViewModel.SubCategoryName?.Trim() ?? string.Empty,
                                Remarks = subCategoryViewModel.Remarks?.Trim() ?? string.Empty,
                                IsActive = subCategoryViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponse = await _SubCategoryService.SaveSubCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, SubCategoryEntity, headerViewModel.UserId);
                            return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = null, TotalRecords = 0 });
                        }
                        else
                        {
                            return NotFound(GenerateMessage.AuthenticationFailed);
                        }
                    }
                    else
                    {
                        return NotFound(GenerateMessage.AuthenticationFailed);
                    }
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new SubCategory record");
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
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.SubCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var SubCategoryToDelete = await _SubCategoryService.GetSubCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SubCategoryId, headerViewModel.UserId);

                            if (SubCategoryToDelete == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var sqlResponse = await _SubCategoryService.DeleteSubCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, SubCategoryToDelete, headerViewModel.UserId);

                            return StatusCode(StatusCodes.Status202Accepted, sqlResponse);
                        }
                        else
                        {
                            return NotFound(GenerateMessage.AuthenticationFailed);
                        }
                    }
                    else
                    {
                        return NotFound(GenerateMessage.AuthenticationFailed);
                    }
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }
}