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
    public class PortController : BaseController
    {
        private readonly IPortService _PortService;
        private readonly ILogger<PortController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public PortController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<PortController> logger, IPortService PortService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _PortService = PortService;
        }

        [HttpGet, Route("GetPort")]
        [Authorize]
        public async Task<ActionResult> GetAllPort()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Port, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<PortViewModelCount>("Port");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _PortService.GetPortListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<PortViewModelCount>("Port", cacheData, expirationTime);

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

        [HttpGet, Route("GetPortbyid/{PortId}")]
        [Authorize]
        public async Task<ActionResult<PortViewModel>> GetPortById(Int16 PortId)
        {
            var PortViewModel = new PortViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Port, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Port_{PortId}", out PortViewModel? cachedProduct))
                        {
                            PortViewModel = cachedProduct;
                        }
                        else
                        {
                            PortViewModel = _mapper.Map<PortViewModel>(await _PortService.GetPortByIdAsync(RegId,CompanyId, PortId, UserId));

                            if (PortViewModel == null)
                                return NotFound();
                            else
                                // Cache the Port with an expiration time of 10 minutes
                                _memoryCache.Set($"Port_{PortId}", PortViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, PortViewModel);
                        //return Ok(PortViewModel);
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

        [HttpPost, Route("AddPort")]
        [Authorize]
        public async Task<ActionResult<PortViewModel>> CreatePort(PortViewModel Port)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Port, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Port == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Port ID mismatch");

                            var PortEntity = new M_Port
                            {
                                CompanyId = Port.CompanyId,
                                PortCode = Port.PortCode,
                                PortId = Port.PortId,
                                PortName = Port.PortName,
                                CreateById = UserId,
                                IsActive = Port.IsActive,
                                Remarks = Port.Remarks
                            };

                            var createdPort = await _PortService.AddPortAsync(RegId,CompanyId, PortEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdPort);

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
                    "Error creating new Port record");
            }
        }

        [HttpPut, Route("UpdatePort/{PortId}")]
        [Authorize]
        public async Task<ActionResult<PortViewModel>> UpdatePort(Int16 PortId, [FromBody] PortViewModel Port)
        {
            var PortViewModel = new PortViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Port, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (PortId != Port.PortId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Port ID mismatch");
                            //return BadRequest("M_Port ID mismatch");

                            // Attempt to retrieve the Port from the cache
                            if (_memoryCache.TryGetValue($"Port_{PortId}", out PortViewModel? cachedProduct))
                            {
                                PortViewModel = cachedProduct;
                            }
                            else
                            {
                                var PortToUpdate = await _PortService.GetPortByIdAsync(RegId,CompanyId, PortId, UserId);

                                if (PortToUpdate == null)
                                    return NotFound($"M_Port with Id = {PortId} not found");
                            }

                            var PortEntity = new M_Port
                            {
                                PortCode = Port.PortCode,
                                PortId = Port.PortId,
                                PortName = Port.PortName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = Port.IsActive,
                                Remarks = Port.Remarks
                            };

                            var sqlResponce = await _PortService.UpdatePortAsync(RegId,CompanyId, PortEntity, UserId);
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

        [HttpDelete, Route("Delete/{PortId}")]
        [Authorize]
        public async Task<ActionResult<M_Port>> DeletePort(Int16 PortId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Port, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var PortToDelete = await _PortService.GetPortByIdAsync(RegId,CompanyId, PortId, UserId);

                            if (PortToDelete == null)
                                return NotFound($"M_Port with Id = {PortId} not found");

                            var sqlResponce = await _PortService.DeletePortAsync(RegId,CompanyId, PortToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Port_{PortId}");
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


