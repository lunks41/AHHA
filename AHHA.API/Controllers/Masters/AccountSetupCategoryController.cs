﻿using AHHA.Application.IServices;
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
    public class AccountSetupCategoryController : BaseController
    {
        private readonly IAccountSetupCategoryService _AccountSetupCategoryService;
        private readonly ILogger<AccountSetupCategoryController> _logger;

        public AccountSetupCategoryController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<AccountSetupCategoryController> logger, IAccountSetupCategoryService AccountSetupCategoryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _AccountSetupCategoryService = AccountSetupCategoryService;
        }

        [HttpGet, Route("GetAccountSetupCategory")]
        [Authorize]
        public async Task<ActionResult> GetAllAccountSetupCategory([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetupCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        headerViewModel.searchString = headerViewModel.searchString == null ? string.Empty : headerViewModel.searchString.Trim();

                        var AccountSetupCategoryData = await _AccountSetupCategoryService.GetAccountSetupCategoryListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                            if (AccountSetupCategoryData == null)
                                return NotFound();

                            return StatusCode(StatusCodes.Status202Accepted, AccountSetupCategoryData);
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

        [HttpGet, Route("GetAccountSetupCategorybyid/{AccSetupCategoryId}")]
        [Authorize]
        public async Task<ActionResult<AccountSetupCategoryViewModel>> GetAccountSetupCategoryById(Int16 AccSetupCategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var AccountSetupCategoryViewModel = new AccountSetupCategoryViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetupCategory, headerViewModel.UserId);
                    
                    _logger.LogInformation("Check the ValidateScrren");

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"AccountSetupCategory_{AccSetupCategoryId}", out AccountSetupCategoryViewModel? cachedProduct))
                        {
                            AccountSetupCategoryViewModel = cachedProduct;
                        }
                        else
                        {
                            AccountSetupCategoryViewModel = _mapper.Map<AccountSetupCategoryViewModel>(await _AccountSetupCategoryService.GetAccountSetupCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccSetupCategoryId, headerViewModel.UserId));

                            if (AccountSetupCategoryViewModel == null)
                                return NotFound();
                            else
                                // Cache the AccountSetupCategory with an expiration time of 10 minutes
                                _memoryCache.Set($"AccountSetupCategory_{AccSetupCategoryId}", AccountSetupCategoryViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, AccountSetupCategoryViewModel);
                        //return Ok(AccountSetupCategoryViewModel);
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

        [HttpPost, Route("AddAccountSetupCategory")]
        [Authorize]
        public async Task<ActionResult<AccountSetupCategoryViewModel>> CreateAccountSetupCategory(AccountSetupCategoryViewModel AccountSetupCategory, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetupCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (AccountSetupCategory == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_AccountSetupCategory ID mismatch");

                            var AccountSetupCategoryEntity = new M_AccountSetupCategory
                            {
                                AccSetupCategoryId = AccountSetupCategory.AccSetupCategoryId,
                                AccSetupCategoryCode = AccountSetupCategory.AccSetupCategoryCode,
                                AccSetupCategoryName = AccountSetupCategory.AccSetupCategoryName,
                                CreateById = headerViewModel.UserId,
                                IsActive = AccountSetupCategory.IsActive,
                                Remarks = AccountSetupCategory.Remarks
                            };

                            var createdAccountSetupCategory = await _AccountSetupCategoryService.AddAccountSetupCategoryAsync(headerViewModel.RegId,headerViewModel.CompanyId, AccountSetupCategoryEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdAccountSetupCategory);

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
                    "Error creating new AccountSetupCategory record");
            }
        }

        [HttpPut, Route("UpdateAccountSetupCategory/{AccSetupCategoryId}")]
        [Authorize]
        public async Task<ActionResult<AccountSetupCategoryViewModel>> UpdateAccountSetupCategory(Int16 AccSetupCategoryId, [FromBody] AccountSetupCategoryViewModel AccountSetupCategory, [FromHeader] HeaderViewModel headerViewModel)
        {
            var AccountSetupCategoryViewModel = new AccountSetupCategoryViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetupCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (AccSetupCategoryId != AccountSetupCategory.AccSetupCategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_AccountSetupCategory ID mismatch");
                            //return BadRequest("M_AccountSetupCategory ID mismatch");

                            // Attempt to retrieve the AccountSetupCategory from the cache
                            if (_memoryCache.TryGetValue($"AccountSetupCategory_{AccSetupCategoryId}", out AccountSetupCategoryViewModel? cachedProduct))
                            {
                                AccountSetupCategoryViewModel = cachedProduct;
                            }
                            else
                            {
                                var AccountSetupCategoryToUpdate = await _AccountSetupCategoryService.GetAccountSetupCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccSetupCategoryId, headerViewModel.UserId);

                                if (AccountSetupCategoryToUpdate == null)
                                    return NotFound($"M_AccountSetupCategory with Id = {AccSetupCategoryId} not found");
                            }

                            var AccountSetupCategoryEntity = new M_AccountSetupCategory
                            {
                                AccSetupCategoryCode = AccountSetupCategory.AccSetupCategoryCode,
                                AccSetupCategoryId = AccountSetupCategory.AccSetupCategoryId,
                                AccSetupCategoryName = AccountSetupCategory.AccSetupCategoryName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = AccountSetupCategory.IsActive,
                                Remarks = AccountSetupCategory.Remarks
                            };

                            var sqlResponce = await _AccountSetupCategoryService.UpdateAccountSetupCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccountSetupCategoryEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteAccountSetupCategory/{AccSetupCategoryId}")]
        [Authorize]
        public async Task<ActionResult<M_AccountSetupCategory>> DeleteAccountSetupCategory(Int16 AccSetupCategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountSetupCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var AccountSetupCategoryToDelete = await _AccountSetupCategoryService.GetAccountSetupCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccSetupCategoryId, headerViewModel.UserId);

                            if (AccountSetupCategoryToDelete == null)
                                return NotFound($"M_AccountSetupCategory with Id = {AccSetupCategoryId} not found");

                            var sqlResponce = await _AccountSetupCategoryService.DeleteAccountSetupCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccountSetupCategoryToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"AccountSetupCategory_{AccSetupCategoryId}");
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

