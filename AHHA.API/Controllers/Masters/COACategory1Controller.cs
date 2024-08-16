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
    public class COACategory1Controller : BaseController
    {
        private readonly ICOACategory1Service _COACategory1Service;
        private readonly ILogger<COACategory1Controller> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public COACategory1Controller(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<COACategory1Controller> logger, ICOACategory1Service COACategory1Service)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _COACategory1Service = COACategory1Service;
        }

        [HttpGet, Route("GetCOACategory1")]
        [Authorize]
        public async Task<ActionResult> GetAllCOACategory1()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory1, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<COACategoryViewModelCount>("COACategory1");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _COACategory1Service.GetCOACategory1ListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<COACategoryViewModelCount>("COACategory1", cacheData, expirationTime);

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

        [HttpGet, Route("GetCOACategory1byid/{COACategoryId}")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> GetCOACategory1ById(Int16 COACategoryId)
        {
            var COACategoryViewModel = new COACategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory1, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"COACategory1_{COACategoryId}", out COACategoryViewModel? cachedProduct))
                        {
                            COACategoryViewModel = cachedProduct;
                        }
                        else
                        {
                            COACategoryViewModel = _mapper.Map<COACategoryViewModel>(await _COACategory1Service.GetCOACategory1ByIdAsync(RegId,CompanyId, COACategoryId, UserId));

                            if (COACategoryViewModel == null)
                                return NotFound();
                            else
                                // Cache the COACategory1 with an expiration time of 10 minutes
                                _memoryCache.Set($"COACategory1_{COACategoryId}", COACategoryViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, COACategoryViewModel);
                        //return Ok(COACategoryViewModel);
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

        [HttpPost, Route("AddCOACategory1")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> CreateCOACategory1(COACategoryViewModel COACategory1)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory1, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (COACategory1 == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_COACategory1 ID mismatch");

                            var COACategory1Entity = new M_COACategory1
                            {
                                CompanyId = COACategory1.CompanyId,
                                COACategoryCode = COACategory1.COACategoryCode,
                                COACategoryId = COACategory1.COACategoryId,
                                COACategoryName = COACategory1.COACategoryName,
                                CreateById = UserId,
                                IsActive = COACategory1.IsActive,
                                Remarks = COACategory1.Remarks
                            };

                            var createdCOACategory1 = await _COACategory1Service.AddCOACategory1Async(RegId,CompanyId, COACategory1Entity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCOACategory1);

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
                    "Error creating new COACategory1 record");
            }
        }

        [HttpPut, Route("UpdateCOACategory1/{COACategoryId}")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> UpdateCOACategory1(Int16 COACategoryId, [FromBody] COACategoryViewModel COACategory1)
        {
            var COACategoryViewModel = new COACategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory1, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (COACategoryId != COACategory1.COACategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_COACategory1 ID mismatch");
                            //return BadRequest("M_COACategory1 ID mismatch");

                            // Attempt to retrieve the COACategory1 from the cache
                            if (_memoryCache.TryGetValue($"COACategory1_{COACategoryId}", out COACategoryViewModel? cachedProduct))
                            {
                                COACategoryViewModel = cachedProduct;
                            }
                            else
                            {
                                var COACategory1ToUpdate = await _COACategory1Service.GetCOACategory1ByIdAsync(RegId,CompanyId, COACategoryId, UserId);

                                if (COACategory1ToUpdate == null)
                                    return NotFound($"M_COACategory1 with Id = {COACategoryId} not found");
                            }

                            var COACategory1Entity = new M_COACategory1
                            {
                                COACategoryCode = COACategory1.COACategoryCode,
                                COACategoryId = COACategory1.COACategoryId,
                                COACategoryName = COACategory1.COACategoryName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = COACategory1.IsActive,
                                Remarks = COACategory1.Remarks
                            };

                            var sqlResponce = await _COACategory1Service.UpdateCOACategory1Async(RegId,CompanyId, COACategory1Entity, UserId);
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

        [HttpDelete, Route("Delete/{COACategoryId}")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> DeleteCOACategory1(Int16 COACategoryId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory1, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var COACategory1ToDelete = await _COACategory1Service.GetCOACategory1ByIdAsync(RegId,CompanyId, COACategoryId, UserId);

                            if (COACategory1ToDelete == null)
                                return NotFound($"M_COACategory1 with Id = {COACategoryId} not found");

                            var sqlResponce = await _COACategory1Service.DeleteCOACategory1Async(RegId,CompanyId, COACategory1ToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"COACategory1_{COACategoryId}");
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
