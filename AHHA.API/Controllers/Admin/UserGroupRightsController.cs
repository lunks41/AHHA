﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Admin;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;
using AHHA.Infra.Services.Admin;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Admin
{
    [Route("api/Admin")]
    [ApiController]
    public class UserGroupRightsController : BaseController
    {
        private readonly IUserGroupRightsService _UserGroupRightsService;
        private readonly ILogger<UserGroupRightsController> _logger;

        public UserGroupRightsController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<UserGroupRightsController> logger, IUserGroupRightsService UserGroupRightsService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _UserGroupRightsService = UserGroupRightsService;
        }

        //Getlist  --UserGroupRightsid
        //Upsert   --UserGroupRightsid

        [HttpGet, Route("GetUserGroupRightsbyid/{UserGroupId}")]
        [Authorize]
        public async Task<ActionResult> GetUserGroupRightsById(Int16 UserGroupId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (UserGroupId != 0)
                {
                    if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    {
                        var UserGroupRightsRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Admin.UserGroupRights, headerViewModel.UserId);

                        if (UserGroupRightsRight != null)
                        {
                            var cacheData = await _UserGroupRightsService.GetUserGroupRightsByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, UserGroupId, headerViewModel.UserId);

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
                else
                {
                    return NotFound(GenerateMessage.DataNotFound);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [HttpPost, Route("SaveUserGroupRights")]
        [Authorize]
        public async Task<ActionResult> SaveUserGroupRights([FromBody] List<UserGroupRightsViewModel> userGroupRightsViewModels, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                // Validate headers
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                // Check user group rights
                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Admin, (Int16)E_Admin.UserGroupRights, headerViewModel.UserId);

                if (userGroupRight == null || !userGroupRight.IsCreate)
                    return NotFound(GenerateMessage.AuthenticationFailed);

                // Validate input data
                if (userGroupRightsViewModels == null || !userGroupRightsViewModels.Any())
                    return NotFound(GenerateMessage.DataNotFound);

                // Map the view model to the entity
                var UserGroupRightsEntities = userGroupRightsViewModels.Where(x => x.IsRead || x.IsCreate || x.IsEdit || x.IsExport || x.IsDelete || x.IsPrint).Select(item => new AdmUserGroupRights
                {
                    UserGroupId = item.UserGroupId,
                    ModuleId = item.ModuleId,
                    TransactionId = item.TransactionId,
                    IsRead = item.IsRead,
                    IsCreate = item.IsCreate,
                    IsEdit = item.IsEdit,
                    IsExport = item.IsExport,
                    IsDelete = item.IsDelete,
                    IsPrint = item.IsPrint,
                    CreateById = headerViewModel.UserId,
                }).ToList();

                // Save the mapped entities
                var createdUserGroupRights = await _UserGroupRightsService.SaveUserGroupRightsAsync(headerViewModel.RegId, headerViewModel.CompanyId, UserGroupRightsEntities, userGroupRightsViewModels.First().UserGroupId, headerViewModel.UserId);

                if (createdUserGroupRights.Result <= 0)
                {
                    _logger.LogWarning("Failed to save number format for CompanyId: {CompanyId}, RegId: {RegId}", headerViewModel.CompanyId, headerViewModel.RegId);
                    return Ok(new SqlResponse { Result = -1, Message = "Failed", Data = null, TotalRecords = 0 });
                }

                var cacheData = await _UserGroupRightsService.GetUserGroupRightsByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, userGroupRightsViewModels[0].UserGroupId, headerViewModel.UserId);

                return cacheData switch
                {
                    null => Ok(new SqlResponse { Result = -1, Message = "Failed", Data = null, TotalRecords = 0 }),
                    _ => Ok(new SqlResponse { Result = 1, Message = "Success", Data = cacheData, TotalRecords = 0 })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving UserGroupRights records");
                return StatusCode(StatusCodes.Status500InternalServerError, new SqlResponse { Result = -1, Message = "An error occurred while processing your request", Data = null });
            }
        }


        [HttpPost, Route("CloneUserGroupRights/{FromUserGroupId}/{ToUserGroupId}")]
        [Authorize]
        public async Task<ActionResult<UserViewModel>> CloneUserGroupRights(Int16 FromUserGroupId, Int16 ToUserGroupId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    if (FromUserGroupId == 0 || ToUserGroupId == 0)
                        return NotFound(GenerateMessage.DataNotFound);

                    var sqlResponse = await _UserGroupRightsService.CloneUserGroupRightsAsync(headerViewModel.RegId, headerViewModel.CompanyId, FromUserGroupId, ToUserGroupId, headerViewModel.UserId);

                    return StatusCode(StatusCodes.Status202Accepted, sqlResponse);
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