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
    public class COACategory3Controller : BaseController
    {
        private readonly ICOACategory3Service _COACategory3Service;
        private readonly ILogger<COACategory3Controller> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public COACategory3Controller(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<COACategory3Controller> logger, ICOACategory3Service COACategory3Service)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _COACategory3Service = COACategory3Service;
        }

        [HttpGet, Route("GetCOACategory3")]
        [Authorize]
        public async Task<ActionResult> GetAllCOACategory3()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory3, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<COACategoryViewModelCount>("COACategory3");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _COACategory3Service.GetCOACategory3ListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<COACategoryViewModelCount>("COACategory3", cacheData, expirationTime);

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

        [HttpGet, Route("GetCOACategory3byid/{COACategoryId}")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> GetCOACategory3ById(Int16 COACategoryId)
        {
            var COACategoryViewModel = new COACategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory3, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"COACategory3_{COACategoryId}", out COACategoryViewModel? cachedProduct))
                        {
                            COACategoryViewModel = cachedProduct;
                        }
                        else
                        {
                            COACategoryViewModel = _mapper.Map<COACategoryViewModel>(await _COACategory3Service.GetCOACategory3ByIdAsync(RegId,CompanyId, COACategoryId, UserId));

                            if (COACategoryViewModel == null)
                                return NotFound();
                            else
                                // Cache the COACategory3 with an expiration time of 10 minutes
                                _memoryCache.Set($"COACategory3_{COACategoryId}", COACategoryViewModel, TimeSpan.FromMinutes(10));
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

        [HttpPost, Route("AddCOACategory3")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> CreateCOACategory3(COACategoryViewModel COACategory3)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory3, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (COACategory3 == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_COACategory3 ID mismatch");

                            var COACategory3Entity = new M_COACategory3
                            {
                                CompanyId = COACategory3.CompanyId,
                                COACategoryCode = COACategory3.COACategoryCode,
                                COACategoryId = COACategory3.COACategoryId,
                                COACategoryName = COACategory3.COACategoryName,
                                CreateById = UserId,
                                IsActive = COACategory3.IsActive,
                                Remarks = COACategory3.Remarks
                            };

                            var createdCOACategory3 = await _COACategory3Service.AddCOACategory3Async(RegId,CompanyId, COACategory3Entity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCOACategory3);

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
                    "Error creating new COACategory3 record");
            }
        }

        [HttpPut, Route("UpdateCOACategory3/{COACategoryId}")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> UpdateCOACategory3(Int16 COACategoryId, [FromBody] COACategoryViewModel COACategory3)
        {
            var COACategoryViewModel = new COACategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory3, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (COACategoryId != COACategory3.COACategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_COACategory3 ID mismatch");
                            //return BadRequest("M_COACategory3 ID mismatch");

                            // Attempt to retrieve the COACategory3 from the cache
                            if (_memoryCache.TryGetValue($"COACategory3_{COACategoryId}", out COACategoryViewModel? cachedProduct))
                            {
                                COACategoryViewModel = cachedProduct;
                            }
                            else
                            {
                                var COACategory3ToUpdate = await _COACategory3Service.GetCOACategory3ByIdAsync(RegId,CompanyId, COACategoryId, UserId);

                                if (COACategory3ToUpdate == null)
                                    return NotFound($"M_COACategory3 with Id = {COACategoryId} not found");
                            }

                            var COACategory3Entity = new M_COACategory3
                            {
                                COACategoryCode = COACategory3.COACategoryCode,
                                COACategoryId = COACategory3.COACategoryId,
                                COACategoryName = COACategory3.COACategoryName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = COACategory3.IsActive,
                                Remarks = COACategory3.Remarks
                            };

                            var sqlResponce = await _COACategory3Service.UpdateCOACategory3Async(RegId,CompanyId, COACategory3Entity, UserId);
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

        //[HttpDelete, Route("Delete/{COACategoryId}")]
        //[Authorize]
        //public async Task<ActionResult<M_COACategory3>> DeleteCOACategory3(Int16 COACategoryId)
        //{
        //    try
        //    {
        //        CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
        //        UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

        //        if (ValidateHeaders(RegId,CompanyId, UserId))
        //        {
        //            var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory3, UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var COACategory3ToDelete = await _COACategory3Service.GetCOACategory3ByIdAsync(RegId,CompanyId, COACategoryId, UserId);

        //                    if (COACategory3ToDelete == null)
        //                        return NotFound($"M_COACategory3 with Id = {COACategoryId} not found");

        //                    var sqlResponce = await _COACategory3Service.DeleteCOACategory3Async(RegId,CompanyId, COACategory3ToDelete, UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"COACategory3_{COACategoryId}");
        //                    return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
        //                }
        //                else
        //                {
        //                    return NotFound("Users do not have a access to delete");
        //                }
        //            }
        //            else
        //            {
        //                return NotFound("Users not have a access for this screen");
        //            }
        //        }
        //        else
        //        {
        //            return NoContent();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            "Error deleting data");
        //    }
        //}
    }
}

