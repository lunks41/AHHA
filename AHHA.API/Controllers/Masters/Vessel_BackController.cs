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
    public class Vessel_BackController : BaseController
    {
        private readonly IVessel_BackService _Vessel_BackService;
        private readonly ILogger<Vessel_BackController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public Vessel_BackController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<Vessel_BackController> logger, IVessel_BackService Vessel_BackService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _Vessel_BackService = Vessel_BackService;
        }

        [HttpGet, Route("GetVessel_Back")]
        [Authorize]
        public async Task<ActionResult> GetAllVessel_Back()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<Vessel_BackViewModelCount>("Vessel_Back");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _Vessel_BackService.GetVessel_BackListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<Vessel_BackViewModelCount>("Vessel_Back", cacheData, expirationTime);

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

        [HttpGet, Route("GetVessel_Backbyid/{Vessel_BackId}")]
        [Authorize]
        public async Task<ActionResult<Vessel_BackViewModel>> GetVessel_BackById(Int16 Vessel_BackId)
        {
            var Vessel_BackViewModel = new Vessel_BackViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Vessel_Back_{Vessel_BackId}", out Vessel_BackViewModel? cachedProduct))
                        {
                            Vessel_BackViewModel = cachedProduct;
                        }
                        else
                        {
                            Vessel_BackViewModel = _mapper.Map<Vessel_BackViewModel>(await _Vessel_BackService.GetVessel_BackByIdAsync(RegId,CompanyId, Vessel_BackId, UserId));

                            if (Vessel_BackViewModel == null)
                                return NotFound();
                            else
                                // Cache the Vessel_Back with an expiration time of 10 minutes
                                _memoryCache.Set($"Vessel_Back_{Vessel_BackId}", Vessel_BackViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, Vessel_BackViewModel);
                        //return Ok(Vessel_BackViewModel);
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

        [HttpPost, Route("AddVessel_Back")]
        [Authorize]
        public async Task<ActionResult<Vessel_BackViewModel>> CreateVessel_Back(Vessel_BackViewModel Vessel_Back)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Vessel_Back == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Vessel_Back ID mismatch");

                            var Vessel_BackEntity = new M_Vessel_Back
                            {
                                CompanyId = Vessel_Back.CompanyId,
                                VesselCode = Vessel_Back.VesselCode,
                                VesselId = Vessel_Back.VesselId,
                                VesselName = Vessel_Back.VesselName,
                                CallSign = Vessel_Back.CallSign,
                                IMOCode = Vessel_Back.IMOCode,
                                GRT = Vessel_Back.GRT,
                                LicenseNo = Vessel_Back.LicenseNo,
                                VesselType = Vessel_Back.VesselType,
                                Flag = Vessel_Back.Flag,
                                CreateBy = UserId,
                                IsActive = Vessel_Back.IsActive,
                                Remarks = Vessel_Back.Remarks
                            };

                            var createdVessel_Back = await _Vessel_BackService.AddVessel_BackAsync(RegId,CompanyId, Vessel_BackEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdVessel_Back);

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
                    "Error creating new Vessel_Back record");
            }
        }

        [HttpPut, Route("UpdateVessel_Back/{Vessel_BackId}")]
        [Authorize]
        public async Task<ActionResult<Vessel_BackViewModel>> UpdateVessel_Back(Int16 Vessel_BackId, [FromBody] Vessel_BackViewModel Vessel_Back)
        {
            var Vessel_BackViewModel = new Vessel_BackViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (Vessel_BackId != Vessel_Back.VesselId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Vessel_Back ID mismatch");
                            //return BadRequest("M_Vessel_Back ID mismatch");

                            // Attempt to retrieve the Vessel_Back from the cache
                            if (_memoryCache.TryGetValue($"Vessel_Back_{Vessel_BackId}", out Vessel_BackViewModel? cachedProduct))
                            {
                                Vessel_BackViewModel = cachedProduct;
                            }
                            else
                            {
                                var Vessel_BackToUpdate = await _Vessel_BackService.GetVessel_BackByIdAsync(RegId,CompanyId, Vessel_BackId, UserId);

                                if (Vessel_BackToUpdate == null)
                                    return NotFound($"M_Vessel_Back with Id = {Vessel_BackId} not found");
                            }

                            var Vessel_BackEntity = new M_Vessel_Back
                            {
                                CompanyId = Vessel_Back.CompanyId,
                                VesselCode = Vessel_Back.VesselCode,
                                VesselId = Vessel_Back.VesselId,
                                VesselName = Vessel_Back.VesselName,
                                CallSign = Vessel_Back.CallSign,
                                IMOCode = Vessel_Back.IMOCode,
                                GRT = Vessel_Back.GRT,
                                LicenseNo = Vessel_Back.LicenseNo,
                                VesselType = Vessel_Back.VesselType,
                                Flag = Vessel_Back.Flag,
                                CreateBy = UserId,
                                IsActive = Vessel_Back.IsActive,
                                Remarks = Vessel_Back.Remarks
                            };

                            var sqlResponce = await _Vessel_BackService.UpdateVessel_BackAsync(RegId,CompanyId, Vessel_BackEntity, UserId);
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

        [HttpDelete, Route("Delete/{Vessel_BackId}")]
        [Authorize]
        public async Task<ActionResult<M_Vessel_Back>> DeleteVessel_Back(Int16 Vessel_BackId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var Vessel_BackToDelete = await _Vessel_BackService.GetVessel_BackByIdAsync(RegId,CompanyId, Vessel_BackId, UserId);

                            if (Vessel_BackToDelete == null)
                                return NotFound($"M_Vessel_Back with Id = {Vessel_BackId} not found");

                            var sqlResponce = await _Vessel_BackService.DeleteVessel_BackAsync(RegId,CompanyId, Vessel_BackToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Vessel_Back_{Vessel_BackId}");
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


