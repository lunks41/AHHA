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
    public class GstController : BaseController
    {
        private readonly IGstService _GstService;
        private readonly ILogger<GstController> _logger;

        public GstController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<GstController> logger, IGstService GstService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _GstService = GstService;
        }

        #region GST_HD

        [HttpGet, Route("GetGst")]
        [Authorize]
        public async Task<ActionResult> GetGst([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Gst, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        var cacheData = await _GstService.GetGstListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetGstbyid/{GstId}")]
        [Authorize]
        public async Task<ActionResult<GstViewModel>> GetGstById(Int16 GstId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Gst, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var gstViewModel = _mapper.Map<GstViewModel>(await _GstService.GetGstByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstId, headerViewModel.UserId));

                        if (gstViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, gstViewModel);
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

        [HttpPost, Route("AddGst")]
        [Authorize]
        public async Task<ActionResult<GstViewModel>> CreateGst(GstViewModel gstViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Gst, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (gstViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var GstEntity = new M_Gst
                            {
                                GstId = gstViewModel.GstId,
                                CompanyId = headerViewModel.CompanyId,
                                GstCode = gstViewModel.GstCode,
                                Remarks = gstViewModel.Remarks,
                                GstName = gstViewModel.GstName,
                                IsActive = gstViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                            };

                            var createdGst = await _GstService.AddGstAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdGst);
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
                    "Error creating new Gst record");
            }
        }

        [HttpPut, Route("UpdateGst/{GstId}")]
        [Authorize]
        public async Task<ActionResult<GstViewModel>> UpdateGst(Int16 GstId, [FromBody] GstViewModel gstViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Gst, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (GstId != gstViewModel.GstId)
                                return StatusCode(StatusCodes.Status400BadRequest, "Gst mismatch");

                            var GstToUpdate = await _GstService.GetGstByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstId, headerViewModel.UserId);

                            if (GstToUpdate == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var GstEntity = new M_Gst
                            {
                                GstId = gstViewModel.GstId,
                                CompanyId = headerViewModel.CompanyId,
                                GstCode = gstViewModel.GstCode,
                                Remarks = gstViewModel.Remarks,
                                GstName = gstViewModel.GstName,
                                IsActive = gstViewModel.IsActive,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponce = await _GstService.UpdateGstAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteGst/{GstId}")]
        [Authorize]
        public async Task<ActionResult<M_Gst>> DeleteGst(Int16 GstId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Gst, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var GstToDelete = await _GstService.GetGstByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstId, headerViewModel.UserId);

                            if (GstToDelete == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var sqlResponce = await _GstService.DeleteGstAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstToDelete, headerViewModel.UserId);

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

        #endregion GST_HD

        #region GST_DT

        [HttpGet, Route("GetGstDt")]
        [Authorize]
        public async Task<ActionResult> GetGstDt([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GstDt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        var cacheData = await _GstService.GetGstDtListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetGstDtbyid/{GstDtId}")]
        [Authorize]
        public async Task<ActionResult<GstDtViewModel>> GetGstDtById(Int16 GstDtId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GstDt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var GstDtViewModel = _mapper.Map<GstDtViewModel>(await _GstService.GetGstDtByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstDtId, headerViewModel.UserId));

                        if (GstDtViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, GstDtViewModel);
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

        [HttpPost, Route("AddGstDt")]
        [Authorize]
        public async Task<ActionResult<GstDtViewModel>> CreateGstDt(GstDtViewModel GstDt, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GstDt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (GstDt == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var GstDtEntity = new M_GstDt
                            {
                                GstId = GstDt.GstId,
                                CompanyId = headerViewModel.CompanyId,
                                GstPercentahge = GstDt.GstPercentahge,
                                ValidFrom = GstDt.ValidFrom,
                                CreateById = headerViewModel.UserId
                            };

                            var createdGstDt = await _GstService.AddGstDtAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstDtEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdGstDt);
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
                    "Error creating new GstDt record");
            }
        }

        [HttpPut, Route("UpdateGstDt/{GstDtId}")]
        [Authorize]
        public async Task<ActionResult<GstDtViewModel>> UpdateGstDt(Int16 GstDtId, [FromBody] GstDtViewModel GstDt, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GstDt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            var GstDtToUpdate = await _GstService.GetGstDtByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstDtId, headerViewModel.UserId);

                            if (GstDtToUpdate == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var GstDtEntity = new M_GstDt
                            {
                                GstId = GstDt.GstId,
                                CompanyId = headerViewModel.CompanyId,
                                GstPercentahge = GstDt.GstPercentahge,
                                ValidFrom = GstDt.ValidFrom,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponce = await _GstService.UpdateGstDtAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstDtEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteGstDt/{GstDtId}")]
        [Authorize]
        public async Task<ActionResult<M_GstDt>> DeleteGstDt(Int16 GstDtId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GstDt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var GstDtToDelete = await _GstService.GetGstDtByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstDtId, headerViewModel.UserId);

                            if (GstDtToDelete == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var sqlResponce = await _GstService.DeleteGstDtAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstDtToDelete, headerViewModel.UserId);

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

        #endregion GST_DT
    }
}