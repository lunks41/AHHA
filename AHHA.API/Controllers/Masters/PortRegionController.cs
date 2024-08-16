using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/Master")]
    [ApiController]
    public class PortRegionController : BaseController
    {
        private readonly IPortRegionService _portRegionService;
        private readonly ILogger<PortRegionController> _logger;

        public PortRegionController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<PortRegionController> logger, IPortRegionService portRegionService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _portRegionService = portRegionService;
        }

        [HttpGet, Route("getPortRegion")]
        public async Task<ActionResult> GetAllPortRegion([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.PortRegion, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<PortRegionViewModelCount>("PortRegion");

                        if (cacheData != null)
                            return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _portRegionService.GetPortRegionListAsync(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);
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
                    if (headerViewModel.UserId == 0)
                        return NotFound("UserId Not Found");
                    else if (headerViewModel.CompanyId == 0)
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


        [HttpGet, Route("getPortRegionbyid/{PortRegionId}")]
        public async Task<ActionResult<PortRegionViewModel>> GetPortRegionById(Int32 PortRegionId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var PortRegionViewModel = new PortRegionViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.PortRegion, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"PortRegion_{PortRegionId}", out PortRegionViewModel? cachedProduct))
                        {
                            PortRegionViewModel = cachedProduct;
                        }
                        else
                        {
                            PortRegionViewModel = _mapper.Map<PortRegionViewModel>(await _portRegionService.GetPortRegionByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, PortRegionId, headerViewModel.UserId));

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


        [HttpPost, Route("addPortRegion")]
        public async Task<ActionResult<PortRegionViewModel>> CreatePortRegion(PortRegionViewModel PortRegion, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.PortRegion, headerViewModel.UserId);

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
                                CreateById = headerViewModel.UserId,
                                IsActive = PortRegion.IsActive,
                                Remarks = PortRegion.Remarks
                            };

                            var createdPortRegion = await _portRegionService.AddPortRegionAsync(headerViewModel.RegId,headerViewModel.CompanyId, PortRegionEntity, headerViewModel.UserId);
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


        [HttpPut, Route("updatePortRegion/{PortRegionId}")]
        public async Task<ActionResult<PortRegionViewModel>> UpdatePortRegion(int PortRegionId, [FromBody] PortRegionViewModel PortRegion, [FromHeader] HeaderViewModel headerViewModel)
        {
            var PortRegionViewModel = new PortRegionViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.PortRegion, headerViewModel.UserId);

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
                                var PortRegionToUpdate = await _portRegionService.GetPortRegionByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, PortRegionId, headerViewModel.UserId);

                                if (PortRegionToUpdate == null)
                                    return NotFound($"M_PortRegion with Id = {PortRegionId} not found");
                            }

                            var PortRegionEntity = new M_PortRegion
                            {
                                PortRegionCode = PortRegion.PortRegionCode,
                                PortRegionId = PortRegion.PortRegionId,
                                PortRegionName = PortRegion.PortRegionName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = PortRegion.IsActive,
                                Remarks = PortRegion.Remarks,
                                CountryId = PortRegion.CountryId
                               
                            };

                            var sqlResponce = await _portRegionService.UpdatePortRegionAsync(headerViewModel.RegId,headerViewModel.CompanyId, PortRegionEntity, headerViewModel.UserId);
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


        [HttpDelete, Route("deletePortRegion/{PortRegionId}")]
        public async Task<ActionResult<M_PortRegion>> DeletePortRegion(int PortRegionId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.PortRegion, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var PortRegionToDelete = await _portRegionService.GetPortRegionByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, PortRegionId, headerViewModel.UserId);

                            if (PortRegionToDelete == null)
                                return NotFound($"M_PortRegion with Id = {PortRegionId} not found");

                            var sqlResponce = await _portRegionService.DeletePortRegionAsync(headerViewModel.RegId,headerViewModel.CompanyId, PortRegionToDelete, headerViewModel.UserId);
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
