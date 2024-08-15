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
    public class DesignationController : BaseController
    {
        private readonly IDesignationService _DesignationService;
        private readonly ILogger<DesignationController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public DesignationController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<DesignationController> logger, IDesignationService DesignationService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _DesignationService = DesignationService;
        }

        [HttpGet, Route("GetDesignation")]
        [Authorize]
        public async Task<ActionResult> GetAllDesignation()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Designation, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<DesignationViewModelCount>("Designation");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _DesignationService.GetDesignationListAsync(CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<DesignationViewModelCount>("Designation", cacheData, expirationTime);

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

        [HttpGet, Route("GetDesignationbyid/{DesignationId}")]
        [Authorize]
        public async Task<ActionResult<DesignationViewModel>> GetDesignationById(Int16 DesignationId)
        {
            var DesignationViewModel = new DesignationViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Designation, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Designation_{DesignationId}", out DesignationViewModel? cachedProduct))
                        {
                            DesignationViewModel = cachedProduct;
                        }
                        else
                        {
                            DesignationViewModel = _mapper.Map<DesignationViewModel>(await _DesignationService.GetDesignationByIdAsync(CompanyId, DesignationId, UserId));

                            if (DesignationViewModel == null)
                                return NotFound();
                            else
                                // Cache the Designation with an expiration time of 10 minutes
                                _memoryCache.Set($"Designation_{DesignationId}", DesignationViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, DesignationViewModel);
                        //return Ok(DesignationViewModel);
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

        [HttpPost, Route("AddDesignation")]
        [Authorize]
        public async Task<ActionResult<DesignationViewModel>> CreateDesignation(DesignationViewModel Designation)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Designation, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Designation == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Designation ID mismatch");

                            var DesignationEntity = new M_Designation
                            {
                                CompanyId = Designation.CompanyId,
                                DesignationCode = Designation.DesignationCode,
                                DesignationId = Designation.DesignationId,
                                DesignationName = Designation.DesignationName,
                                CreateById = UserId,
                                IsActive = Designation.IsActive,
                                Remarks = Designation.Remarks
                            };

                            var createdDesignation = await _DesignationService.AddDesignationAsync(CompanyId, DesignationEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdDesignation);

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
                    "Error creating new Designation record");
            }
        }

        [HttpPut, Route("UpdateDesignation/{DesignationId}")]
        [Authorize]
        public async Task<ActionResult<DesignationViewModel>> UpdateDesignation(Int16 DesignationId, [FromBody] DesignationViewModel Designation)
        {
            var DesignationViewModel = new DesignationViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Designation, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (DesignationId != Designation.DesignationId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Designation ID mismatch");
                            //return BadRequest("M_Designation ID mismatch");

                            // Attempt to retrieve the Designation from the cache
                            if (_memoryCache.TryGetValue($"Designation_{DesignationId}", out DesignationViewModel? cachedProduct))
                            {
                                DesignationViewModel = cachedProduct;
                            }
                            else
                            {
                                var DesignationToUpdate = await _DesignationService.GetDesignationByIdAsync(CompanyId, DesignationId, UserId);

                                if (DesignationToUpdate == null)
                                    return NotFound($"M_Designation with Id = {DesignationId} not found");
                            }

                            var DesignationEntity = new M_Designation
                            {
                                DesignationCode = Designation.DesignationCode,
                                DesignationId = Designation.DesignationId,
                                DesignationName = Designation.DesignationName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = Designation.IsActive,
                                Remarks = Designation.Remarks
                            };

                            var sqlResponce = await _DesignationService.UpdateDesignationAsync(CompanyId, DesignationEntity, UserId);
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

        [HttpDelete, Route("Delete/{DesignationId}")]
        [Authorize]
        public async Task<ActionResult<M_Designation>> DeleteDesignation(Int16 DesignationId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Designation, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var DesignationToDelete = await _DesignationService.GetDesignationByIdAsync(CompanyId, DesignationId, UserId);

                            if (DesignationToDelete == null)
                                return NotFound($"M_Designation with Id = {DesignationId} not found");

                            var sqlResponce = await _DesignationService.DeleteDesignationAsync(CompanyId, DesignationToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Designation_{DesignationId}");
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

