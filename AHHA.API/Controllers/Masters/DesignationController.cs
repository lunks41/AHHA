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
    public class DesignationController : BaseController
    {
        private readonly IDesignationService _DesignationService;
        private readonly ILogger<DesignationController> _logger;

        public DesignationController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<DesignationController> logger, IDesignationService DesignationService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _DesignationService = DesignationService;
        }

        [HttpGet, Route("GetDesignation")]
        [Authorize]
        public async Task<ActionResult> GetDesignation([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Designation, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _DesignationService.GetDesignationListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetDesignationbyid/{DesignationId}")]
        [Authorize]
        public async Task<ActionResult<DesignationViewModel>> GetDesignationById(Int16 DesignationId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Designation, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var designationViewModel = _mapper.Map<DesignationViewModel>(await _DesignationService.GetDesignationByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, DesignationId, headerViewModel.UserId));

                        if (designationViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, designationViewModel);
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

        [HttpPost, Route("AddDesignation")]
        [Authorize]
        public async Task<ActionResult<DesignationViewModel>> CreateDesignation(DesignationViewModel designationViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Designation, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (designationViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var DesignationEntity = new M_Designation
                            {
                                DesignationId = designationViewModel.DesignationId,
                                CompanyId = headerViewModel.CompanyId,
                                DesignationCode = designationViewModel.DesignationCode,
                                DesignationName = designationViewModel.DesignationName,
                                Remarks = designationViewModel.Remarks,
                                IsActive = designationViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                            };

                            var createdDesignation = await _DesignationService.AddDesignationAsync(headerViewModel.RegId, headerViewModel.CompanyId, DesignationEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdDesignation);
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
                    "Error creating new Designation record");
            }
        }

        [HttpPut, Route("UpdateDesignation/{DesignationId}")]
        [Authorize]
        public async Task<ActionResult<DesignationViewModel>> UpdateDesignation(Int16 DesignationId, [FromBody] DesignationViewModel designationViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Designation, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (DesignationId != designationViewModel.DesignationId)
                                return StatusCode(StatusCodes.Status400BadRequest, "Designation ID mismatch");

                            var DesignationToUpdate = await _DesignationService.GetDesignationByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, DesignationId, headerViewModel.UserId);

                            if (DesignationToUpdate == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var DesignationEntity = new M_Designation
                            {
                                DesignationId = designationViewModel.DesignationId,
                                CompanyId = headerViewModel.CompanyId,
                                DesignationCode = designationViewModel.DesignationCode,
                                DesignationName = designationViewModel.DesignationName,
                                Remarks = designationViewModel.Remarks,
                                IsActive = designationViewModel.IsActive,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponce = await _DesignationService.UpdateDesignationAsync(headerViewModel.RegId, headerViewModel.CompanyId, DesignationEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteDesignation/{DesignationId}")]
        [Authorize]
        public async Task<ActionResult<M_Designation>> DeleteDesignation(Int16 DesignationId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Designation, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var DesignationToDelete = await _DesignationService.GetDesignationByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, DesignationId, headerViewModel.UserId);

                            if (DesignationToDelete == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var sqlResponce = await _DesignationService.DeleteDesignationAsync(headerViewModel.RegId, headerViewModel.CompanyId, DesignationToDelete, headerViewModel.UserId);

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