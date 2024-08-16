using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/Master")]
    [ApiController]
    public class BargeController : BaseController
    {
        private readonly IBargeService _BargeService;
        private readonly ILogger<BargeController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public BargeController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<BargeController> logger, IBargeService BargeService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _BargeService = BargeService;
        }

        [HttpGet, Route("GetBarge")]
        [Authorize]
        public async Task<ActionResult> GetAllBarge()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Barge, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<BargeViewModelCount>("Barge");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _BargeService.GetBargeListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<BargeViewModelCount>("Barge", cacheData, expirationTime);

                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                            //return Ok(cacheData);
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
                    }
                }
                else
                {
                    if (UserId == 0)
                        return NotFound("UserId Not Found");
                    else if (CompanyId == 0)
                        return NotFound("CompanyId Not Found");
                    else
                        return NotFound();
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
        public async Task<ActionResult<BargeViewModel>> GetBargeById(Int16 BargeId)
        {
            var BargeViewModel = new BargeViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Barge, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Barge_{BargeId}", out BargeViewModel? cachedProduct))
                        {
                            BargeViewModel = cachedProduct;
                        }
                        else
                        {
                            BargeViewModel = _mapper.Map<BargeViewModel>(await _BargeService.GetBargeByIdAsync(RegId,CompanyId, BargeId, UserId));

                            if (BargeViewModel == null)
                                return NotFound();
                            else
                                // Cache the Barge with an expiration time of 10 minutes
                                _memoryCache.Set($"Barge_{BargeId}", BargeViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, BargeViewModel);
                        //return Ok(BargeViewModel);
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
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
        public async Task<ActionResult<BargeViewModel>> CreateBarge(BargeViewModel Barge)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Barge, UserId);

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
                                CreateById = UserId,
                                IsActive = Barge.IsActive,
                                Remarks = Barge.Remarks
                            };

                            var createdBarge = await _BargeService.AddBargeAsync(RegId,CompanyId, BargeEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdBarge);

                        }
                        else
                        {
                            return NotFound("Users do not have a access to delete");
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
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
        public async Task<ActionResult<BargeViewModel>> UpdateBarge(Int16 BargeId, [FromBody] BargeViewModel Barge)
        {
            var BargeViewModel = new BargeViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Barge, UserId);

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
                                var BargeToUpdate = await _BargeService.GetBargeByIdAsync(RegId,CompanyId, BargeId, UserId);

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
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = Barge.IsActive,
                                Remarks = Barge.Remarks
                            };

                            var sqlResponce = await _BargeService.UpdateBargeAsync(RegId,CompanyId, BargeEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
                        }
                        else
                        {
                            return NotFound("Users do not have a access to delete");
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
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

        [HttpDelete, Route("Delete/{BargeId}")]
        [Authorize]
        public async Task<ActionResult<M_Barge>> DeleteBarge(Int16 BargeId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Barge, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var BargeToDelete = await _BargeService.GetBargeByIdAsync(RegId,CompanyId, BargeId, UserId);

                            if (BargeToDelete == null)
                                return NotFound($"M_Barge with Id = {BargeId} not found");

                            var sqlResponce = await _BargeService.DeleteBargeAsync(RegId,CompanyId, BargeToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Barge_{BargeId}");
                            return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
                        }
                        else
                        {
                            return NotFound("Users do not have a access to delete");
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
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

