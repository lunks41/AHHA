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
    public class UserRightsController : BaseController
    {
        private readonly IUserRightsService _UserRightsService;
        private readonly ILogger<UserRightsController> _logger;

        public UserRightsController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<UserRightsController> logger, IUserRightsService UserRightsService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _UserRightsService = UserRightsService;
        }

        [HttpGet, Route("GetUserRightsbyid/{UserId}")]
        [Authorize]
        public async Task<ActionResult<UserRightsViewModel>> GetUserRightsById(Int16 UserId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)Core.Common.E_Admin.User, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var UserViewModel = await _UserRightsService.GetUserRightsByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, UserId, headerViewModel.UserId);

                        if (UserViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, UserViewModel);
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

        //create
        //update
        [HttpPost, Route("UpsertUserRights")]
        [Authorize]
        public async Task<ActionResult<UserRightsViewModel>> UpsertUserRightsAsync(UserViewModel User, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var UserRightsRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Admin.User, headerViewModel.UserId);

                    if (UserRightsRight != null)
                    {
                        if (UserRightsRight.IsCreate)
                        {
                            if (User == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var UserEntity = new AdmUserRights
                            {
                                CompanyId = headerViewModel.CompanyId,
                                UserId = User.UserId,
                                CreateById = headerViewModel.UserId
                            };

                            var createdUser = await _UserRightsService.UpsertUserRightsAsync(headerViewModel.RegId, headerViewModel.CompanyId, UserEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdUser);
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
                    "Error creating new User record");
            }
        }
    }
}