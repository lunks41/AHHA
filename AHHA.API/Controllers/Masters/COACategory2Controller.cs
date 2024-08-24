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
    public class COACategory2Controller : BaseController
    {
        private readonly ICOACategory2Service _COACategory2Service;
        private readonly ILogger<COACategory2Controller> _logger;

        public COACategory2Controller(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<COACategory2Controller> logger, ICOACategory2Service COACategory2Service)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _COACategory2Service = COACategory2Service;
        }

        [HttpGet, Route("GetCOACategory2")]
        [Authorize]
        public async Task<ActionResult> GetCOACategory2([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory2, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _COACategory2Service.GetCOACategory2ListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenrateMessage.authenticationfailed);

                        return Ok(cacheData);
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

        [HttpGet, Route("GetCOACategory2byid/{COACategoryId}")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> GetCOACategory2ById(Int16 COACategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var COACategoryViewModel = new COACategoryViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory2, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"COACategory2_{COACategoryId}", out COACategoryViewModel? cachedProduct))
                        {
                            COACategoryViewModel = cachedProduct;
                        }
                        else
                        {
                            COACategoryViewModel = _mapper.Map<COACategoryViewModel>(await _COACategory2Service.GetCOACategory2ByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, COACategoryId, headerViewModel.UserId));

                            if (COACategoryViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the COACategory2 with an expiration time of 10 minutes
                                _memoryCache.Set($"COACategory2_{COACategoryId}", COACategoryViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, COACategoryViewModel);
                        //return Ok(COACategoryViewModel);
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

        [HttpPost, Route("AddCOACategory2")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> CreateCOACategory2(COACategoryViewModel COACategory2, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory2, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (COACategory2 == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_COACategory2 ID mismatch");

                            var COACategory2Entity = new M_COACategory2
                            {
                                CompanyId = COACategory2.CompanyId,
                                COACategoryCode = COACategory2.COACategoryCode,
                                COACategoryId = COACategory2.COACategoryId,
                                COACategoryName = COACategory2.COACategoryName,
                                CreateById = headerViewModel.UserId,
                                IsActive = COACategory2.IsActive,
                                Remarks = COACategory2.Remarks
                            };

                            var createdCOACategory2 = await _COACategory2Service.AddCOACategory2Async(headerViewModel.RegId, headerViewModel.CompanyId, COACategory2Entity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCOACategory2);
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
                    "Error creating new COACategory2 record");
            }
        }

        [HttpPut, Route("UpdateCOACategory2/{COACategoryId}")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> UpdateCOACategory2(Int16 COACategoryId, [FromBody] COACategoryViewModel COACategory2, [FromHeader] HeaderViewModel headerViewModel)
        {
            var COACategoryViewModel = new COACategoryViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory2, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (COACategoryId != COACategory2.COACategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_COACategory2 ID mismatch");
                            //return BadRequest("M_COACategory2 ID mismatch");

                            // Attempt to retrieve the COACategory2 from the cache
                            if (_memoryCache.TryGetValue($"COACategory2_{COACategoryId}", out COACategoryViewModel? cachedProduct))
                            {
                                COACategoryViewModel = cachedProduct;
                            }
                            else
                            {
                                var COACategory2ToUpdate = await _COACategory2Service.GetCOACategory2ByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, COACategoryId, headerViewModel.UserId);

                                if (COACategory2ToUpdate == null)
                                    return NotFound($"M_COACategory2 with Id = {COACategoryId} not found");
                            }

                            var COACategory2Entity = new M_COACategory2
                            {
                                COACategoryCode = COACategory2.COACategoryCode,
                                COACategoryId = COACategory2.COACategoryId,
                                COACategoryName = COACategory2.COACategoryName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = COACategory2.IsActive,
                                Remarks = COACategory2.Remarks
                            };

                            var sqlResponce = await _COACategory2Service.UpdateCOACategory2Async(headerViewModel.RegId, headerViewModel.CompanyId, COACategory2Entity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteCOACategory2/{COACategoryId}")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> DeleteCOACategory2(Int16 COACategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory2, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var COACategory2ToDelete = await _COACategory2Service.GetCOACategory2ByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, COACategoryId, headerViewModel.UserId);

                            if (COACategory2ToDelete == null)
                                return NotFound($"M_COACategory2 with Id = {COACategoryId} not found");

                            var sqlResponce = await _COACategory2Service.DeleteCOACategory2Async(headerViewModel.RegId, headerViewModel.CompanyId, COACategory2ToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"COACategory2_{COACategoryId}");
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