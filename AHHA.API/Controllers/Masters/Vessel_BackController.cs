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
    public class Vessel_BackController : BaseController
    {
        private readonly IVessel_BackService _Vessel_BackService;
        private readonly ILogger<Vessel_BackController> _logger;

        public Vessel_BackController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<Vessel_BackController> logger, IVessel_BackService Vessel_BackService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _Vessel_BackService = Vessel_BackService;
        }

        [HttpGet, Route("GetVessel_Back")]
        [Authorize]
        public async Task<ActionResult> GetAllVessel_Back([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        headerViewModel.searchString = headerViewModel.searchString == null ? string.Empty : headerViewModel.searchString.Trim();

                        var VesselBackData = await _Vessel_BackService.GetVessel_BackListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

                        if (VesselBackData == null)
                            return NotFound();

                        return StatusCode(StatusCodes.Status202Accepted, VesselBackData);
                        //return Ok(cacheData);
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
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

        [HttpGet, Route("GetVessel_Backbyid/{Vessel_BackId}")]
        [Authorize]
        public async Task<ActionResult<Vessel_BackViewModel>> GetVessel_BackById(Int16 Vessel_BackId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var Vessel_BackViewModel = new Vessel_BackViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Vessel_Back_{Vessel_BackId}", out Vessel_BackViewModel? cachedProduct))
                        {
                            Vessel_BackViewModel = cachedProduct;
                        }
                        else
                        {
                            Vessel_BackViewModel = _mapper.Map<Vessel_BackViewModel>(await _Vessel_BackService.GetVessel_BackByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Vessel_BackId, headerViewModel.UserId));

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
        public async Task<ActionResult<Vessel_BackViewModel>> CreateVessel_Back(Vessel_BackViewModel Vessel_Back, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, headerViewModel.UserId);

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
                                CreateBy = headerViewModel.UserId,
                                IsActive = Vessel_Back.IsActive,
                                Remarks = Vessel_Back.Remarks
                            };

                            var createdVessel_Back = await _Vessel_BackService.AddVessel_BackAsync(headerViewModel.RegId, headerViewModel.CompanyId, Vessel_BackEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdVessel_Back);
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
                    "Error creating new Vessel_Back record");
            }
        }

        [HttpPut, Route("UpdateVessel_Back/{Vessel_BackId}")]
        [Authorize]
        public async Task<ActionResult<Vessel_BackViewModel>> UpdateVessel_Back(Int16 Vessel_BackId, [FromBody] Vessel_BackViewModel Vessel_Back, [FromHeader] HeaderViewModel headerViewModel)
        {
            var Vessel_BackViewModel = new Vessel_BackViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, headerViewModel.UserId);

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
                                var Vessel_BackToUpdate = await _Vessel_BackService.GetVessel_BackByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Vessel_BackId, headerViewModel.UserId);

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
                                CreateBy = headerViewModel.UserId,
                                IsActive = Vessel_Back.IsActive,
                                Remarks = Vessel_Back.Remarks
                            };

                            var sqlResponce = await _Vessel_BackService.UpdateVessel_BackAsync(headerViewModel.RegId, headerViewModel.CompanyId, Vessel_BackEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("Delete/{Vessel_BackId}")]
        [Authorize]
        public async Task<ActionResult<M_Vessel_Back>> DeleteVessel_Back(Int16 Vessel_BackId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Vessel, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var Vessel_BackToDelete = await _Vessel_BackService.GetVessel_BackByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Vessel_BackId, headerViewModel.UserId);

                            if (Vessel_BackToDelete == null)
                                return NotFound($"M_Vessel_Back with Id = {Vessel_BackId} not found");

                            var sqlResponce = await _Vessel_BackService.DeleteVessel_BackAsync(headerViewModel.RegId, headerViewModel.CompanyId, Vessel_BackToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Vessel_Back_{Vessel_BackId}");
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