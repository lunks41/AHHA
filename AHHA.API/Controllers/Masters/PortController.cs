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
    public class PortController : BaseController
    {
        private readonly IPortService _PortService;
        private readonly ILogger<PortController> _logger;

        public PortController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<PortController> logger, IPortService PortService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _PortService = PortService;
        }

        [HttpGet, Route("GetPort")]
        [Authorize]
        public async Task<ActionResult> GetPort([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Port, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _PortService.GetPortListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetPortbyid/{PortId}")]
        [Authorize]
        public async Task<ActionResult<PortViewModel>> GetPortById(Int16 PortId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Port, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var portViewModel = await _PortService.GetPortByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, PortId, headerViewModel.UserId);

                        if (portViewModel == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, portViewModel);
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

        [HttpPost, Route("SavePort")]
        [Authorize]
        public async Task<ActionResult<PortViewModel>> SavePort(PortViewModel portViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Port, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (portViewModel == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var PortEntity = new M_Port
                            {
                                PortId = portViewModel.PortId,
                                CompanyId = headerViewModel.CompanyId,
                                PortRegionId = portViewModel.PortRegionId,
                                PortCode = portViewModel.PortCode?.Trim() ?? string.Empty,
                                PortName = portViewModel.PortName?.Trim() ?? string.Empty,
                                Remarks = portViewModel.Remarks?.Trim() ?? string.Empty,
                                IsActive = portViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponse = await _PortService.SavePortAsync(headerViewModel.RegId, headerViewModel.CompanyId, PortEntity, headerViewModel.UserId);
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
                    "Error creating new Port record");
            }
        }

        [HttpDelete, Route("DeletePort/{PortId}")]
        [Authorize]
        public async Task<ActionResult<M_Port>> DeletePort(Int16 PortId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Port, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var PortToDelete = _mapper.Map<M_Port>(await _PortService.GetPortByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, PortId, headerViewModel.UserId));

                            if (PortToDelete == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var sqlResponse = await _PortService.DeletePortAsync(headerViewModel.RegId, headerViewModel.CompanyId, PortToDelete, headerViewModel.UserId);

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