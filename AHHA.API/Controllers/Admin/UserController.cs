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
    public class UserController : BaseController
    {
        //Create User --user/password

        //User Edit
        //Edit User  -- uesrpassword cannot be updated
        //-- password will be not shown
        //--reset password API
        //--update the password

        //Getlist
        //Get
        //Create
        //Update
        //resetpassword
        //Delete

        private readonly IUserService _UserService;
        private readonly ILogger<UserController> _logger;

        public UserController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<UserController> logger, IUserService countryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _UserService = countryService;
        }

        [HttpGet, Route("GetUser")]
        [Authorize]
        public async Task<ActionResult> GetUser([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)Core.Common.E_Admin.User, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _UserService.GetUserListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

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

        [HttpGet, Route("GetUserbyid/{UserId}")]
        [Authorize]
        public async Task<ActionResult<UserViewModel>> GetUserById(Int16 UserId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)Core.Common.E_Admin.User, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var UserViewModel = _mapper.Map<UserViewModel>(await _UserService.GetUserByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, UserId, headerViewModel.UserId));

                        if (UserViewModel == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, UserViewModel);
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

        [HttpPost, Route("SaveUser")]
        [Authorize]
        public async Task<ActionResult<UserViewModel>> SaveUser(UserViewModel userViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)Core.Common.E_Admin.User, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (userViewModel == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var UserEntity = new AdmUser
                            {
                                UserId = userViewModel.UserId,
                                UserCode = userViewModel.UserCode,
                                UserName = userViewModel.UserName,
                                UserPassword = userViewModel.UserPassword,
                                UserEmail = userViewModel.UserEmail,
                                Remarks = userViewModel.Remarks,
                                IsActive = userViewModel.IsActive,
                                UserGroupId = userViewModel.UserGroupId,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var createdUser = await _UserService.SaveUserAsync(headerViewModel.RegId, headerViewModel.CompanyId, UserEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdUser);
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
                    "Error creating new User record");
            }
        }

        [HttpDelete, Route("DeleteUser/{UserId}")]
        [Authorize]
        public async Task<ActionResult<AdmUser>> DeleteUser(Int16 UserId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)Core.Common.E_Admin.User, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var UserToDelete = await _UserService.GetUserByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, UserId, headerViewModel.UserId);

                            if (UserToDelete == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var sqlResponse = await _UserService.DeleteUserAsync(headerViewModel.RegId, headerViewModel.CompanyId, UserToDelete, headerViewModel.UserId);

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

        [HttpPost, Route("ResetPassword/{UserId}")]
        [Authorize]
        public async Task<ActionResult<UserViewModel>> ResetPassword(Int16 UserId, UserViewModel userViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    if (userViewModel == null || userViewModel.UserPassword == null)
                        return NotFound(GenerateMessage.DataNotFound);

                    var resetpasswordUser = await _UserService.GetUserByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, userViewModel.UserId, headerViewModel.UserId);

                    if (resetpasswordUser == null)
                        return NotFound(GenerateMessage.DataNotFound);

                    var UserEntity = new AdmUser
                    {
                        UserId = userViewModel.UserId,
                        UserCode = resetpasswordUser.UserCode,
                        UserPassword = userViewModel.UserPassword,
                        EditById = headerViewModel.UserId,
                        EditDate = DateTime.Now,
                    };

                    var sqlResponse = await _UserService.ResetPasswordAsync(headerViewModel.RegId, headerViewModel.CompanyId, UserEntity, headerViewModel.UserId);

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