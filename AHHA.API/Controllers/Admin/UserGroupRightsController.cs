﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Admin;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;
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
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var UserGroupRightsRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Admin.UserGroupRights, headerViewModel.UserId);

                    if (UserGroupRightsRight != null)
                    {
                        var cacheData = await _UserGroupRightsService.GetUserGroupRightsByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, UserGroupId, headerViewModel.UserId);

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

        [HttpPost, Route("UpsetUserGroupRights")]
        [Authorize]
        public async Task<ActionResult<UserGroupRightsViewModel>> UpsetUserGroupRights(UserGroupRightsViewModel UserGroupRights, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var UserGroupRightsRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)Core.Common.E_Admin.UserGroupRights, headerViewModel.UserId);

                    if (UserGroupRightsRight != null)
                    {
                        if (UserGroupRightsRight.IsCreate)
                        {
                            if (UserGroupRights == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var UserGroupRightsEntity = new AdmUserGroupRights
                            {
                                UserGroupId = UserGroupRights.UserGroupId,
                                ModuleId = UserGroupRights.ModuleId,
                                TransactionId = UserGroupRights.TransactionId,
                                IsRead = UserGroupRights.IsRead,
                                IsCreate = UserGroupRights.IsCreate,
                                IsEdit = UserGroupRights.IsEdit,
                                IsDelete = UserGroupRights.IsDelete,
                                IsExport = UserGroupRights.IsExport,
                                IsPrint = UserGroupRights.IsPrint,
                                CreateById = headerViewModel.UserId
                            };

                            var createdUserGroupRights = await _UserGroupRightsService.UpsertUserGroupRightsAsync(headerViewModel.RegId, headerViewModel.CompanyId, UserGroupRightsEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdUserGroupRights);
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
                    "Error creating new UserGroupRights record");
            }
        }
    }
}