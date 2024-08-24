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
    public class BargeController : BaseController
    {
        private readonly IBargeService _BargeService;
        private readonly ILogger<BargeController> _logger;

        public BargeController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<BargeController> logger, IBargeService BargeService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _BargeService = BargeService;
        }

        [HttpGet, Route("GetBarge")]
        [Authorize]
        public async Task<ActionResult> GetBarge([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Barge, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        headerViewModel.searchString = headerViewModel.searchString == null ? string.Empty : headerViewModel.searchString.Trim();

                        var cacheData = await _BargeService.GetBargeListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetBargebyid/{BargeId}")]
        [Authorize]
        public async Task<ActionResult<BargeViewModel>> GetBargeById(Int16 BargeId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var BargeViewModel = new BargeViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Barge, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Barge_{BargeId}", out BargeViewModel? cachedProduct))
                        {
                            BargeViewModel = cachedProduct;
                        }
                        else
                        {
                            BargeViewModel = _mapper.Map<BargeViewModel>(await _BargeService.GetBargeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, BargeId, headerViewModel.UserId));

                            if (BargeViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the Barge with an expiration time of 10 minutes
                                _memoryCache.Set($"Barge_{BargeId}", BargeViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, BargeViewModel);
                        //return Ok(BargeViewModel);
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

        [HttpPost, Route("AddBarge")]
        [Authorize]
        public async Task<ActionResult<BargeViewModel>> CreateBarge(BargeViewModel Barge, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Barge, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Barge == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Barge ID mismatch");

                            var BargeEntity = new M_Barge
                            {
                                CompanyId = Barge.CompanyId,
                                BargeCode = Barge.BargeCode,
                                BargeId = Barge.BargeId,
                                BargeName = Barge.BargeName,
                                CallSign = Barge.CallSign,
                                IMOCode = Barge.IMOCode,
                                GRT = Barge.GRT,
                                LicenseNo = Barge.LicenseNo,
                                BargeIType = Barge.BargeIType,
                                Flag = Barge.Flag,
                                CreateById = headerViewModel.UserId,
                                IsActive = Barge.IsActive,
                                Remarks = Barge.Remarks
                            };

                            var createdBarge = await _BargeService.AddBargeAsync(headerViewModel.RegId, headerViewModel.CompanyId, BargeEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdBarge);
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
                    "Error creating new Barge record");
            }
        }

        [HttpPut, Route("UpdateBarge/{BargeId}")]
        [Authorize]
        public async Task<ActionResult<BargeViewModel>> UpdateBarge(Int16 BargeId, [FromBody] BargeViewModel Barge, [FromHeader] HeaderViewModel headerViewModel)
        {
            var BargeViewModel = new BargeViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Barge, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (BargeId != Barge.BargeId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Barge ID mismatch");
                            //return BadRequest("M_Barge ID mismatch");

                            // Attempt to retrieve the Barge from the cache
                            if (_memoryCache.TryGetValue($"Barge_{BargeId}", out BargeViewModel? cachedProduct))
                            {
                                BargeViewModel = cachedProduct;
                            }
                            else
                            {
                                var BargeToUpdate = await _BargeService.GetBargeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, BargeId, headerViewModel.UserId);

                                if (BargeToUpdate == null)
                                    return NotFound($"M_Barge with Id = {BargeId} not found");
                            }

                            var BargeEntity = new M_Barge
                            {
                                BargeCode = Barge.BargeCode,
                                BargeId = Barge.BargeId,
                                BargeName = Barge.BargeName,
                                CallSign = Barge.CallSign,
                                IMOCode = Barge.IMOCode,
                                GRT = Barge.GRT,
                                LicenseNo = Barge.LicenseNo,
                                BargeIType = Barge.BargeIType,
                                Flag = Barge.Flag,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = Barge.IsActive,
                                Remarks = Barge.Remarks
                            };

                            var sqlResponce = await _BargeService.UpdateBargeAsync(headerViewModel.RegId, headerViewModel.CompanyId, BargeEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteBarge/{BargeId}")]
        [Authorize]
        public async Task<ActionResult<M_Barge>> DeleteBarge(Int16 BargeId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Barge, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var BargeToDelete = await _BargeService.GetBargeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, BargeId, headerViewModel.UserId);

                            if (BargeToDelete == null)
                                return NotFound($"M_Barge with Id = {BargeId} not found");

                            var sqlResponce = await _BargeService.DeleteBargeAsync(headerViewModel.RegId, headerViewModel.CompanyId, BargeToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Barge_{BargeId}");
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