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
    public class COACategory1Controller : BaseController
    {
        private readonly ICOACategory1Service _COACategory1Service;
        private readonly ILogger<COACategory1Controller> _logger;

        public COACategory1Controller(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<COACategory1Controller> logger, ICOACategory1Service COACategory1Service)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _COACategory1Service = COACategory1Service;
        }

        [HttpGet, Route("GetCOACategory1")]
        [Authorize]
        public async Task<ActionResult> GetCOACategory1([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.COACategory1, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _COACategory1Service.GetCOACategory1ListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetCOACategory1byid/{COACategoryId}")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> GetCOACategory1ById(Int16 COACategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.COACategory1, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cOACategoryViewModel = _mapper.Map<COACategoryViewModel>(await _COACategory1Service.GetCOACategory1ByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, COACategoryId, headerViewModel.UserId));

                        if (cOACategoryViewModel == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, cOACategoryViewModel);
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

        [HttpPost, Route("SaveCOACategory1")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> SaveCOACategory1(COACategoryViewModel cOACategoryViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.COACategory1, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (cOACategoryViewModel == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var COACategory1Entity = new M_COACategory1
                            {
                                COACategoryId = cOACategoryViewModel.COACategoryId,
                                CompanyId = headerViewModel.CompanyId,
                                COACategoryCode = cOACategoryViewModel.COACategoryCode?.Trim() ?? string.Empty,
                                COACategoryName = cOACategoryViewModel.COACategoryName?.Trim() ?? string.Empty,
                                SeqNo = cOACategoryViewModel.SeqNo,
                                Remarks = cOACategoryViewModel.Remarks?.Trim() ?? string.Empty,
                                IsActive = cOACategoryViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponse = await _COACategory1Service.SaveCOACategory1Async(headerViewModel.RegId, headerViewModel.CompanyId, COACategory1Entity, headerViewModel.UserId);
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
                    "Error creating new COACategory1 record");
            }
        }

        [HttpDelete, Route("DeleteCOACategory1/{COACategoryId}")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> DeleteCOACategory1(Int16 COACategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (headerViewModel == null || !ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    return NoContent();
                }

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.COACategory1, headerViewModel.UserId);

                if (userGroupRight == null)
                {
                    return NotFound(GenerateMessage.AuthenticationFailed);
                }

                if (!userGroupRight.IsDelete)
                {
                    return NotFound(GenerateMessage.AuthenticationFailed);
                }

                var sqlResponse = await _COACategory1Service.DeleteCOACategory1Async(headerViewModel.RegId, headerViewModel.CompanyId, COACategoryId, headerViewModel.UserId);

                return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = null, TotalRecords = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
            }
        }
    }
}