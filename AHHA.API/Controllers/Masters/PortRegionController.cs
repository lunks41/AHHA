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
    [Route("api/[controller]")]
    [ApiController]
    public class PortRegionController : BaseController
    {
        private readonly IPortRegionService _portRegionService;
        private readonly ILogger<PortRegionController> _logger;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;

        public PortRegionController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<PortRegionController> logger, IPortRegionService portRegionService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _portRegionService = portRegionService;
        }

        [HttpGet, Route("GetPortRegion")]
        [Authorize]
        public async Task<ActionResult> GetAllPortRegions()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("CompanyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("UserId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("RegId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.PortRegion, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<PortRegionViewModelCount>("PortRegion");

                        if (cacheData != null)
                            return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddMinutes(5);
                            cacheData = await _portRegionService.GetPortRegionListAsync(CompanyId, pageSize, pageNumber, UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<PortRegionViewModelCount>("PortRegion", cacheData, expirationTime);

                            return Ok(cacheData);
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

        [HttpGet, Route("GetPortRegionbyid/{PortRegionId}")]
        [Authorize]
        public async Task<ActionResult<PortRegionViewModel>> GetPortRegionById(Int32 PortRegionId)
        {
            var PortRegionViewModel = new PortRegionViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("CompanyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("UserId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.PortRegion, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"PortRegion_{PortRegionId}", out PortRegionViewModel? cachedProduct))
                        {
                            PortRegionViewModel = cachedProduct;
                        }
                        else
                        {
                            PortRegionViewModel = _mapper.Map<PortRegionViewModel>(await _portRegionService.GetPortRegionByIdAsync(CompanyId, PortRegionId, UserId));

                            if (PortRegionViewModel == null)
                                return NotFound();
                            else
                                // Cache the PortRegion with an expiration time of 10 minutes
                                _memoryCache.Set($"PortRegion_{PortRegionId}", PortRegionViewModel, TimeSpan.FromMinutes(10));
                        }

                        return Ok(PortRegionViewModel);
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

        [HttpPost, Route("AddPortRegion")]
        [Authorize]
        public async Task<ActionResult<PortRegionViewModel>> CreatePortRegion(PortRegionViewModel PortRegion)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("CompanyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("UserId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.PortRegion, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (PortRegion == null)
                                return BadRequest();

                            var PortRegionEntity = new M_PortRegion
                            {
                                CompanyId = PortRegion.CompanyId,
                                PortRegionId = PortRegion.PortRegionId,
                                PortRegionCode = PortRegion.PortRegionCode,
                                PortRegionName = PortRegion.PortRegionName,
                                CountryId = PortRegion.CountryId,
                                CreateById = UserId,
                                IsActive = PortRegion.IsActive,
                                Remarks = PortRegion.Remarks
                            };

                            var createdPortRegion = await _portRegionService.AddPortRegionAsync(CompanyId, PortRegionEntity, UserId);
                            return Ok(createdPortRegion);

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
                    "Error creating new PortRegion record");
            }
        }

        [HttpPut, Route("UpdatePortRegion/{PortRegionId}")]
        [Authorize]
        public async Task<ActionResult<PortRegionViewModel>> UpdatePortRegion(int PortRegionId, [FromBody] PortRegionViewModel PortRegion)
        {
            var PortRegionViewModel = new PortRegionViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("CompanyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("UserId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.PortRegion, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (PortRegionId != PortRegion.PortRegionId)
                                return BadRequest("M_PortRegion ID mismatch");

                            // Attempt to retrieve the PortRegion from the cache
                            if (_memoryCache.TryGetValue($"PortRegion_{PortRegionId}", out PortRegionViewModel? cachedProduct))
                            {
                                PortRegionViewModel = cachedProduct;
                            }
                            else
                            {
                                var PortRegionToUpdate = await _portRegionService.GetPortRegionByIdAsync(CompanyId, PortRegionId, UserId);

                                if (PortRegionToUpdate == null)
                                    return NotFound($"M_PortRegion with Id = {PortRegionId} not found");
                            }

                            var PortRegionEntity = new M_PortRegion
                            {
                                PortRegionCode = PortRegion.PortRegionCode,
                                PortRegionId = PortRegion.PortRegionId,
                                PortRegionName = PortRegion.PortRegionName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = PortRegion.IsActive,
                                Remarks = PortRegion.Remarks,
                                CountryId = PortRegion.CountryId
                               
                            };

                            var sqlResponce = await _portRegionService.UpdatePortRegionAsync(CompanyId, PortRegionEntity, UserId);
                            return Ok(sqlResponce);
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

        [HttpDelete, Route("Delete/{PortRegionId}")]
        [Authorize]
        public async Task<ActionResult<M_PortRegion>> DeletePortRegion(int PortRegionId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("CompanyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("UserId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.PortRegion, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var PortRegionToDelete = await _portRegionService.GetPortRegionByIdAsync(CompanyId, PortRegionId, UserId);

                            if (PortRegionToDelete == null)
                                return NotFound($"M_PortRegion with Id = {PortRegionId} not found");

                            var sqlResponce = await _portRegionService.DeletePortRegionAsync(CompanyId, PortRegionToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"PortRegion_{PortRegionId}");
                            return Ok(sqlResponce);
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
