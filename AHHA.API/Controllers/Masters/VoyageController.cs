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
    public class VoyageController : BaseController
    {
        private readonly IVoyageService _VoyageService;
        private readonly ILogger<VoyageController> _logger;

        public VoyageController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<VoyageController> logger, IVoyageService VoyageService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _VoyageService = VoyageService;
        }

        [HttpGet, Route("GetVoyage")]
        [Authorize]
        public async Task<ActionResult> GetAllVoyage([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Voyage, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        headerViewModel.searchString = headerViewModel.searchString == null ? string.Empty : headerViewModel.searchString.Trim();

                        var voyageData = await _VoyageService.GetVoyageListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (voyageData == null)
                            return NotFound();

                        return StatusCode(StatusCodes.Status202Accepted, voyageData);
                        //return Ok(cacheData);
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

        [HttpGet, Route("GetVoyagebyid/{VoyageId}")]
        [Authorize]
        public async Task<ActionResult<VoyageViewModel>> GetVoyageById(Int16 VoyageId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var VoyageViewModel = new VoyageViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Voyage, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Voyage_{VoyageId}", out VoyageViewModel? cachedProduct))
                        {
                            VoyageViewModel = cachedProduct;
                        }
                        else
                        {
                            VoyageViewModel = _mapper.Map<VoyageViewModel>(await _VoyageService.GetVoyageByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, VoyageId, headerViewModel.UserId));

                            if (VoyageViewModel == null)
                                return NotFound();
                            else
                                // Cache the Voyage with an expiration time of 10 minutes
                                _memoryCache.Set($"Voyage_{VoyageId}", VoyageViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, VoyageViewModel);
                        //return Ok(VoyageViewModel);
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

        [HttpPost, Route("AddVoyage")]
        [Authorize]
        public async Task<ActionResult<VoyageViewModel>> CreateVoyage(VoyageViewModel Voyage, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Voyage, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Voyage == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Voyage ID mismatch");

                            var VoyageEntity = new M_Voyage
                            {
                                CompanyId = Voyage.CompanyId,
                                VoyageNo = Voyage.VoyageNo,
                                VoyageId = Voyage.VoyageId,
                                ReferenceNo = Voyage.ReferenceNo,
                                VesselId = Voyage.VesselId,
                                BargeId = Voyage.BargeId,
                                CreateById = headerViewModel.UserId,
                                IsActive = Voyage.IsActive,
                                Remarks = Voyage.Remarks
                            };

                            var createdVoyage = await _VoyageService.AddVoyageAsync(headerViewModel.RegId, headerViewModel.CompanyId, VoyageEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdVoyage);
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
                    "Error creating new Voyage record");
            }
        }

        [HttpPut, Route("UpdateVoyage/{VoyageId}")]
        [Authorize]
        public async Task<ActionResult<VoyageViewModel>> UpdateVoyage(Int16 VoyageId, [FromBody] VoyageViewModel Voyage, [FromHeader] HeaderViewModel headerViewModel)
        {
            var VoyageViewModel = new VoyageViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Voyage, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (VoyageId != Voyage.VoyageId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Voyage ID mismatch");
                            //return BadRequest("M_Voyage ID mismatch");

                            // Attempt to retrieve the Voyage from the cache
                            if (_memoryCache.TryGetValue($"Voyage_{VoyageId}", out VoyageViewModel? cachedProduct))
                            {
                                VoyageViewModel = cachedProduct;
                            }
                            else
                            {
                                var VoyageToUpdate = await _VoyageService.GetVoyageByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, VoyageId, headerViewModel.UserId);

                                if (VoyageToUpdate == null)
                                    return NotFound($"M_Voyage with Id = {VoyageId} not found");
                            }

                            var VoyageEntity = new M_Voyage
                            {
                                CompanyId = Voyage.CompanyId,
                                VoyageNo = Voyage.VoyageNo,
                                VoyageId = Voyage.VoyageId,
                                ReferenceNo = Voyage.ReferenceNo,
                                VesselId = Voyage.VesselId,
                                BargeId = Voyage.BargeId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = Voyage.IsActive,
                                Remarks = Voyage.Remarks
                            };

                            var sqlResponce = await _VoyageService.UpdateVoyageAsync(headerViewModel.RegId, headerViewModel.CompanyId, VoyageEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteVoyage/{VoyageId}")]
        [Authorize]
        public async Task<ActionResult<M_Voyage>> DeleteVoyage(Int16 VoyageId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Voyage, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var VoyageToDelete = await _VoyageService.GetVoyageByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, VoyageId, headerViewModel.UserId);

                            if (VoyageToDelete == null)
                                return NotFound($"M_Voyage with Id = {VoyageId} not found");

                            var sqlResponce = await _VoyageService.DeleteVoyageAsync(headerViewModel.RegId, headerViewModel.CompanyId, VoyageToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Voyage_{VoyageId}");
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