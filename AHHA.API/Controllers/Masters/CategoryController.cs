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
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _CategoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CategoryController> logger, ICategoryService CategoryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CategoryService = CategoryService;
        }

        [HttpGet, Route("GetCategory")]
        [Authorize]
        public async Task<ActionResult> GetCategory([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Category, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _CategoryService.GetCategoryListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetCategorybyid/{CategoryId}")]
        [Authorize]
        public async Task<ActionResult<CategoryViewModel>> GetCategoryById(Int16 CategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Category, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var categoryViewModel = _mapper.Map<CategoryViewModel>(await _CategoryService.GetCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CategoryId, headerViewModel.UserId));

                        if (categoryViewModel == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, categoryViewModel);
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

        [HttpPost, Route("SaveCategory")]
        [Authorize]
        public async Task<ActionResult<CategoryViewModel>> SaveCategory(CategoryViewModel Category, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Category, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Category == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var CategoryEntity = new M_Category
                            {
                                CategoryId = Category.CategoryId,
                                CompanyId = headerViewModel.CompanyId,
                                CategoryCode = Category.CategoryCode?.Trim() ?? string.Empty,
                                CategoryName = Category.CategoryName?.Trim() ?? string.Empty,
                                Remarks = Category.Remarks?.Trim() ?? string.Empty,
                                IsActive = Category.IsActive,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponse = await _CategoryService.SaveCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, CategoryEntity, headerViewModel.UserId);
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
                    "Error creating new Category record");
            }
        }

        [HttpDelete, Route("DeleteCategory/{CategoryId}")]
        [Authorize]
        public async Task<ActionResult<M_Category>> DeleteCategory(Int16 CategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Category, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var CategoryToDelete = await _CategoryService.GetCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CategoryId, headerViewModel.UserId);

                            if (CategoryToDelete == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var sqlResponse = await _CategoryService.DeleteCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, CategoryToDelete, headerViewModel.UserId);

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