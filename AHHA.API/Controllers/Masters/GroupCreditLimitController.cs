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
    public class GroupCreditLimitController : BaseController
    {
        private readonly IGroupCreditLimitService _GroupCreditLimitService;
        private readonly ILogger<GroupCreditLimitController> _logger;

        public GroupCreditLimitController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<GroupCreditLimitController> logger, IGroupCreditLimitService GroupCreditLimitService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _GroupCreditLimitService = GroupCreditLimitService;
        }

        [HttpGet, Route("GetGroupCreditLimit")]
        [Authorize]
        public async Task<ActionResult> GetGroupCreditLimit([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.GroupCreditLimit, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _GroupCreditLimitService.GetGroupCreditLimitListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetGroupCreditLimitbyid/{GroupCreditLimitId}")]
        [Authorize]
        public async Task<ActionResult<GroupCreditLimitViewModel>> GetGroupCreditLimitById(Int16 GroupCreditLimitId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.GroupCreditLimit, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var GroupCreditLimitViewModel = _mapper.Map<GroupCreditLimitViewModel>(await _GroupCreditLimitService.GetGroupCreditLimitByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GroupCreditLimitId, headerViewModel.UserId));

                        if (GroupCreditLimitViewModel == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, GroupCreditLimitViewModel);
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

        [HttpPost, Route("SaveGroupCreditLimit")]
        [Authorize]
        public async Task<ActionResult<GroupCreditLimitViewModel>> SaveGroupCreditLimit(GroupCreditLimitViewModel groupCreditLimitViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.GroupCreditLimit, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (groupCreditLimitViewModel == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var GroupCreditLimitEntity = new M_GroupCreditLimit
                            {
                                CompanyId = headerViewModel.CompanyId,
                                GroupCreditLimitId = groupCreditLimitViewModel.GroupCreditLimitId,
                                GroupCreditLimitCode = groupCreditLimitViewModel.GroupCreditLimitCode?.Trim() ?? string.Empty,
                                GroupCreditLimitName = groupCreditLimitViewModel.GroupCreditLimitName?.Trim() ?? string.Empty,
                                Remarks = groupCreditLimitViewModel.Remarks?.Trim() ?? string.Empty,
                                IsActive = groupCreditLimitViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponse = await _GroupCreditLimitService.SaveGroupCreditLimitAsync(headerViewModel.RegId, headerViewModel.CompanyId, GroupCreditLimitEntity, headerViewModel.UserId);
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
                    "Error creating new GroupCreditLimit record");
            }
        }

        [HttpDelete, Route("DeleteGroupCreditLimit/{GroupCreditLimitId}")]
        [Authorize]
        public async Task<ActionResult<M_GroupCreditLimit>> DeleteGroupCreditLimit(Int16 GroupCreditLimitId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.GroupCreditLimit, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var GroupCreditLimitToDelete = await _GroupCreditLimitService.GetGroupCreditLimitByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GroupCreditLimitId, headerViewModel.UserId);

                            if (GroupCreditLimitToDelete == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var sqlResponse = await _GroupCreditLimitService.DeleteGroupCreditLimitAsync(headerViewModel.RegId, headerViewModel.CompanyId, GroupCreditLimitToDelete, headerViewModel.UserId);

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