﻿using AHHA.API.Controllers.Masters;
using AHHA.Application.IServices.Masters;
using AHHA.Application.IServices;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Admin
{
    [Route("api/Masters")]
    [ApiController]
    public class AccountTypeController : BaseController
    {
        private readonly IAccountTypeService _AccountTypeService;
        private readonly ILogger<AccountTypeController> _logger;

        public AccountTypeController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<AccountTypeController> logger, IAccountTypeService AccountTypeService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _AccountTypeService = AccountTypeService;
        }

        [HttpGet, Route("GetAccountType")]
        [Authorize]
        public async Task<ActionResult> GetAccountType([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _AccountTypeService.GetAccountTypeListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetAccountTypebyid/{AccTypeId}")]
        [Authorize]
        public async Task<ActionResult<AccountTypeViewModel>> GetAccountTypeById(Int16 AccTypeId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var accountTypeViewModel = _mapper.Map<AccountTypeViewModel>(await _AccountTypeService.GetAccountTypeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccTypeId, headerViewModel.UserId));

                        if (accountTypeViewModel == null)
                            return NotFound(GenrateMessage.authenticationfailed);

                        return StatusCode(StatusCodes.Status202Accepted, accountTypeViewModel);
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

        [HttpPost, Route("AddAccountType")]
        [Authorize]
        public async Task<ActionResult<AccountTypeViewModel>> CreateAccountType(AccountTypeViewModel AccountType, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (AccountType == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "AccountType ID mismatch");

                            var AccountTypeEntity = new M_AccountType
                            {
                                CompanyId = AccountType.CompanyId,
                                AccTypeCode = AccountType.AccTypeCode,
                                AccTypeId = AccountType.AccTypeId,
                                AccTypeName = AccountType.AccTypeName,
                                CreateById = headerViewModel.UserId,
                                IsActive = AccountType.IsActive,
                                Remarks = AccountType.Remarks
                            };

                            var createdAccountType = await _AccountTypeService.AddAccountTypeAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccountTypeEntity, headerViewModel.UserId);

                            return StatusCode(StatusCodes.Status202Accepted, createdAccountType);
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
                    "Error creating new AccountType record");
            }
        }

        [HttpPut, Route("UpdateAccountType/{AccTypeId}")]
        [Authorize]
        public async Task<ActionResult<AccountTypeViewModel>> UpdateAccountType(Int16 AccTypeId, [FromBody] AccountTypeViewModel AccountType, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (AccTypeId != AccountType.AccTypeId)
                                return StatusCode(StatusCodes.Status400BadRequest, "AccountType ID mismatch");

                            var accountTypeToUpdate = await _AccountTypeService.GetAccountTypeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccTypeId, headerViewModel.UserId);

                            if (accountTypeToUpdate == null)
                                return NotFound($"AccountType with Id = {AccTypeId} not found");

                            var AccountTypeEntity = new M_AccountType
                            {
                                AccTypeCode = AccountType.AccTypeCode,
                                AccTypeId = AccountType.AccTypeId,
                                AccTypeName = AccountType.AccTypeName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = AccountType.IsActive,
                                Remarks = AccountType.Remarks
                            };

                            var sqlResponce = await _AccountTypeService.UpdateAccountTypeAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccountTypeEntity, headerViewModel.UserId);

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

        [HttpDelete, Route("DeleteAccountType/{AccTypeId}")]
        [Authorize]
        public async Task<ActionResult<M_AccountType>> DeleteAccountType(Int16 AccTypeId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var AccountTypeToDelete = await _AccountTypeService.GetAccountTypeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccTypeId, headerViewModel.UserId);

                            if (AccountTypeToDelete == null)
                                return NotFound($"AccountType with Id = {AccTypeId} not found");

                            var sqlResponce = await _AccountTypeService.DeleteAccountTypeAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccountTypeToDelete, headerViewModel.UserId);

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