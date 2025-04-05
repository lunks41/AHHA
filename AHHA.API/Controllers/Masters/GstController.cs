using AHHA.Application.IServices;
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
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Gst, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        var cacheData = await _GstService.GetGstListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetGstbyid/{GstId}")]
        [Authorize]
        public async Task<ActionResult<GstViewModel>> GetGstById(Int16 GstId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Gst, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var gstViewModel = _mapper.Map<GstViewModel>(await _GstService.GetGstByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstId, headerViewModel.UserId));

                        if (gstViewModel == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, gstViewModel);
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

        [HttpPost, Route("SaveGst")]
        [Authorize]
        public async Task<ActionResult<GstViewModel>> SaveGst(GstViewModel gstViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Gst, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (gstViewModel == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var GstEntity = new M_Gst
                            {
                                GstId = gstViewModel.GstId,
                                CompanyId = headerViewModel.CompanyId,
                                GstCategoryId = gstViewModel.GstCategoryId,
                                GstCode = gstViewModel.GstCode?.Trim() ?? string.Empty,
                                GstName = gstViewModel.GstName?.Trim() ?? string.Empty,
                                Remarks = gstViewModel.Remarks?.Trim() ?? string.Empty,
                                IsActive = gstViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponse = await _GstService.SaveGstAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstEntity, headerViewModel.UserId);
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
                    "Error creating new Gst record");
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
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Gst, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var GstToDelete = await _GstService.GetGstByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstId, headerViewModel.UserId);

                            if (GstToDelete == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var sqlResponse = await _GstService.DeleteGstAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstToDelete, headerViewModel.UserId);

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
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.GstDt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        var cacheData = await _GstService.GetGstDtListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetGstDtbyid/{GstDtId}/{ValidFrom}")]
        [Authorize]
        public async Task<ActionResult<GstDtViewModel>> GetGstDtById(Int16 GstDtId, DateTime ValidFrom, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.GstDt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var GstDtViewModel = await _GstService.GetGstDtByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstDtId, ValidFrom, headerViewModel.UserId);

                        if (GstDtViewModel == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, GstDtViewModel);
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

        [HttpPost, Route("SaveGstDt")]
        [Authorize]
        public async Task<ActionResult<GstDtViewModel>> SaveGstDt(GstDtViewModel GstDt, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.GstDt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (GstDt == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var GstDtEntity = new M_GstDt
                            {
                                GstId = GstDt.GstId,
                                CompanyId = headerViewModel.CompanyId,
                                GstPercentage = GstDt.GstPercentage,
                                ValidFrom = GstDt.ValidFrom,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponse = await _GstService.SaveGstDtAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstDtEntity, headerViewModel.UserId);
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
                    "Error creating new GstDt record");
            }
        }

        [HttpDelete, Route("DeleteGstDt/{GstDtId}/{ValidFrom}")]
        [Authorize]
        public async Task<ActionResult<GstDtViewModel>> DeleteGstDt(Int16 GstDtId, DateTime ValidFrom, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.GstDt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var GstDtToDelete = await _GstService.GetGstDtByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstDtId, ValidFrom, headerViewModel.UserId);

                            if (GstDtToDelete == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var sqlResponse = await _GstService.DeleteGstDtAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstDtToDelete, headerViewModel.UserId);

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

        #endregion GST_DT
    }
}