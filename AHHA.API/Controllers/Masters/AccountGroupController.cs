﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
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
    public class AccountGroupController : BaseController

    {
        private readonly IAccountGroupService _AccountGroupService;
        private readonly ILogger<AccountGroupController> _logger;

        public AccountGroupController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<AccountGroupController> logger, IAccountGroupService AccountGroupService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _AccountGroupService = AccountGroupService;
        }

        [HttpGet, Route("GetAccountGroup")]
        [Authorize]
        public async Task<ActionResult> GetAccountGroup([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountGroup, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _AccountGroupService.GetAccountGroupListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetAccountGroupbyid/{AccGroupId}")]
        [Authorize]
        public async Task<ActionResult<AccountGroupViewModel>> GetAccountGroupById(Int16 AccGroupId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountGroup, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var accountGroupViewModel = _mapper.Map<AccountGroupViewModel>(await _AccountGroupService.GetAccountGroupByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccGroupId, headerViewModel.UserId));

                        if (accountGroupViewModel == null)
                            return NotFound(GenrateMessage.authenticationfailed);

                        return StatusCode(StatusCodes.Status202Accepted, accountGroupViewModel);
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

        [HttpPost, Route("AddAccountGroup")]
        [Authorize]
        public async Task<ActionResult<AccountGroupViewModel>> CreateAccountGroup(AccountGroupViewModel AccountGroup, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountGroup, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (AccountGroup == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "AccountGroup ID mismatch");

                            var AccountGroupEntity = new M_AccountGroup
                            {
                                CompanyId = AccountGroup.CompanyId,
                                AccGroupCode = AccountGroup.AccGroupCode,
                                AccGroupId = AccountGroup.AccGroupId,
                                AccGroupName = AccountGroup.AccGroupName,
                                CreateById = headerViewModel.UserId,
                                IsActive = AccountGroup.IsActive,
                                Remarks = AccountGroup.Remarks
                            };

                            var createdAccountGroup = await _AccountGroupService.AddAccountGroupAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccountGroupEntity, headerViewModel.UserId);

                            return StatusCode(StatusCodes.Status202Accepted, createdAccountGroup);
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
                    "Error creating new AccountGroup record");
            }
        }

        [HttpPut, Route("UpdateAccountGroup/{AccGroupId}")]
        [Authorize]
        public async Task<ActionResult<AccountGroupViewModel>> UpdateAccountGroup(Int16 AccGroupId, [FromBody] AccountGroupViewModel AccountGroup, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountGroup, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (AccGroupId != AccountGroup.AccGroupId)
                                return StatusCode(StatusCodes.Status400BadRequest, "AccountGroup ID mismatch");

                            var accountGroupToUpdate = await _AccountGroupService.GetAccountGroupByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccGroupId, headerViewModel.UserId);

                            if (accountGroupToUpdate == null)
                                return NotFound($"AccountGroup with Id = {AccGroupId} not found");

                            var AccountGroupEntity = new M_AccountGroup
                            {
                                AccGroupCode = AccountGroup.AccGroupCode,
                                AccGroupId = AccountGroup.AccGroupId,
                                AccGroupName = AccountGroup.AccGroupName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = AccountGroup.IsActive,
                                Remarks = AccountGroup.Remarks
                            };

                            var sqlResponce = await _AccountGroupService.UpdateAccountGroupAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccountGroupEntity, headerViewModel.UserId);

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

        [HttpDelete, Route("DeleteAccountGroup/{AccGroupId}")]
        [Authorize]
        public async Task<ActionResult<M_AccountGroup>> DeleteAccountGroup(Int16 AccGroupId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.AccountGroup, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var AccountGroupToDelete = await _AccountGroupService.GetAccountGroupByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccGroupId, headerViewModel.UserId);

                            if (AccountGroupToDelete == null)
                                return NotFound($"AccountGroup with Id = {AccGroupId} not found");

                            var sqlResponce = await _AccountGroupService.DeleteAccountGroupAsync(headerViewModel.RegId, headerViewModel.CompanyId, AccountGroupToDelete, headerViewModel.UserId);

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