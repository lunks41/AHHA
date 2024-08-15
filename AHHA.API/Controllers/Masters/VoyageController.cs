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
    public class VoyageController : BaseController
    {
        private readonly IVoyageService _VoyageService;
        private readonly ILogger<VoyageController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public VoyageController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<VoyageController> logger, IVoyageService VoyageService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _VoyageService = VoyageService;
        }

        [HttpGet, Route("GetVoyage")]
        [Authorize]
        public async Task<ActionResult> GetAllVoyage()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Voyage, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<VoyageViewModelCount>("Voyage");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _VoyageService.GetVoyageListAsync(CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<VoyageViewModelCount>("Voyage", cacheData, expirationTime);

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

        [HttpGet, Route("GetVoyagebyid/{VoyageId}")]
        [Authorize]
        public async Task<ActionResult<VoyageViewModel>> GetVoyageById(Int16 VoyageId)
        {
            var VoyageViewModel = new VoyageViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Voyage, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Voyage_{VoyageId}", out VoyageViewModel? cachedProduct))
                        {
                            VoyageViewModel = cachedProduct;
                        }
                        else
                        {
                            VoyageViewModel = _mapper.Map<VoyageViewModel>(await _VoyageService.GetVoyageByIdAsync(CompanyId, VoyageId, UserId));

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

        [HttpPost, Route("AddVoyage")]
        [Authorize]
        public async Task<ActionResult<VoyageViewModel>> CreateVoyage(VoyageViewModel Voyage)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Voyage, UserId);

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
                                CreateById = UserId,
                                IsActive = Voyage.IsActive,
                                Remarks = Voyage.Remarks
                            };

                            var createdVoyage = await _VoyageService.AddVoyageAsync(CompanyId, VoyageEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdVoyage);

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
                    "Error creating new Voyage record");
            }
        }

        [HttpPut, Route("UpdateVoyage/{VoyageId}")]
        [Authorize]
        public async Task<ActionResult<VoyageViewModel>> UpdateVoyage(Int16 VoyageId, [FromBody] VoyageViewModel Voyage)
        {
            var VoyageViewModel = new VoyageViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Voyage, UserId);

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
                                var VoyageToUpdate = await _VoyageService.GetVoyageByIdAsync(CompanyId, VoyageId, UserId);

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
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = Voyage.IsActive,
                                Remarks = Voyage.Remarks
                            };

                            var sqlResponce = await _VoyageService.UpdateVoyageAsync(CompanyId, VoyageEntity, UserId);
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

        [HttpDelete, Route("Delete/{VoyageId}")]
        [Authorize]
        public async Task<ActionResult<M_Voyage>> DeleteVoyage(Int16 VoyageId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Voyage, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var VoyageToDelete = await _VoyageService.GetVoyageByIdAsync(CompanyId, VoyageId, UserId);

                            if (VoyageToDelete == null)
                                return NotFound($"M_Voyage with Id = {VoyageId} not found");

                            var sqlResponce = await _VoyageService.DeleteVoyageAsync(CompanyId, VoyageToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Voyage_{VoyageId}");
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


