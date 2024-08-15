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
    public class VesselController : BaseController
    {
        private readonly IVesselService _VesselService;
        private readonly ILogger<VesselController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public VesselController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<VesselController> logger, IVesselService VesselService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _VesselService = VesselService;
        }

        [HttpGet, Route("GetVessel")]
        [Authorize]
        public async Task<ActionResult> GetAllVessel()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<VesselViewModelCount>("Vessel");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _VesselService.GetVesselListAsync(CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<VesselViewModelCount>("Vessel", cacheData, expirationTime);

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

        [HttpGet, Route("GetVesselbyid/{VesselId}")]
        [Authorize]
        public async Task<ActionResult<VesselViewModel>> GetVesselById(Int16 VesselId)
        {
            var VesselViewModel = new VesselViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Vessel_{VesselId}", out VesselViewModel? cachedProduct))
                        {
                            VesselViewModel = cachedProduct;
                        }
                        else
                        {
                            VesselViewModel = _mapper.Map<VesselViewModel>(await _VesselService.GetVesselByIdAsync(CompanyId, VesselId, UserId));

                            if (VesselViewModel == null)
                                return NotFound();
                            else
                                // Cache the Vessel with an expiration time of 10 minutes
                                _memoryCache.Set($"Vessel_{VesselId}", VesselViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, VesselViewModel);
                        //return Ok(VesselViewModel);
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

        [HttpPost, Route("AddVessel")]
        [Authorize]
        public async Task<ActionResult<VesselViewModel>> CreateVessel(VesselViewModel Vessel)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Vessel == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Vessel ID mismatch");

                            var VesselEntity = new M_Vessel
                            {
                                CompanyId = Vessel.CompanyId,
                                VesselCode = Vessel.VesselCode,
                                VesselId = Vessel.VesselId,
                                VesselName = Vessel.VesselName,
                                CallSign = Vessel.CallSign,
                                IMOCode = Vessel.IMOCode,
                                GRT = Vessel.GRT,
                                LicenseNo = Vessel.LicenseNo,
                                VesselType = Vessel.VesselType,
                                Flag = Vessel.Flag,
                                CreateById = UserId,
                                IsActive = Vessel.IsActive,
                                Remarks = Vessel.Remarks
                            };

                            var createdVessel = await _VesselService.AddVesselAsync(CompanyId, VesselEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdVessel);

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
                    "Error creating new Vessel record");
            }
        }

        [HttpPut, Route("UpdateVessel/{VesselId}")]
        [Authorize]
        public async Task<ActionResult<VesselViewModel>> UpdateVessel(Int16 VesselId, [FromBody] VesselViewModel Vessel)
        {
            var VesselViewModel = new VesselViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (VesselId != Vessel.VesselId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Vessel ID mismatch");
                            //return BadRequest("M_Vessel ID mismatch");

                            // Attempt to retrieve the Vessel from the cache
                            if (_memoryCache.TryGetValue($"Vessel_{VesselId}", out VesselViewModel? cachedProduct))
                            {
                                VesselViewModel = cachedProduct;
                            }
                            else
                            {
                                var VesselToUpdate = await _VesselService.GetVesselByIdAsync(CompanyId, VesselId, UserId);

                                if (VesselToUpdate == null)
                                    return NotFound($"M_Vessel with Id = {VesselId} not found");
                            }

                            var VesselEntity = new M_Vessel
                            {
                                CompanyId = Vessel.CompanyId,
                                VesselCode = Vessel.VesselCode,
                                VesselId = Vessel.VesselId,
                                VesselName = Vessel.VesselName,
                                CallSign = Vessel.CallSign,
                                IMOCode = Vessel.IMOCode,
                                GRT = Vessel.GRT,
                                LicenseNo = Vessel.LicenseNo,
                                VesselType = Vessel.VesselType,
                                Flag = Vessel.Flag,
                                CreateById = UserId,
                                IsActive = Vessel.IsActive,
                                Remarks = Vessel.Remarks
                            };

                            var sqlResponce = await _VesselService.UpdateVesselAsync(CompanyId, VesselEntity, UserId);
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

        [HttpDelete, Route("Delete/{VesselId}")]
        [Authorize]
        public async Task<ActionResult<M_Vessel>> DeleteVessel(Int16 VesselId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var VesselToDelete = await _VesselService.GetVesselByIdAsync(CompanyId, VesselId, UserId);

                            if (VesselToDelete == null)
                                return NotFound($"M_Vessel with Id = {VesselId} not found");

                            var sqlResponce = await _VesselService.DeleteVesselAsync(CompanyId, VesselToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Vessel_{VesselId}");
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


