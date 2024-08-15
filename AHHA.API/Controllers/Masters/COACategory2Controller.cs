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
    public class COACategory2Controller : BaseController
    {
        private readonly ICOACategory2Service _COACategory2Service;
        private readonly ILogger<COACategory2Controller> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public COACategory2Controller(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<COACategory2Controller> logger, ICOACategory2Service COACategory2Service)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _COACategory2Service = COACategory2Service;
        }

        [HttpGet, Route("GetCOACategory2")]
        [Authorize]
        public async Task<ActionResult> GetAllCOACategory2()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory2, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<COACategoryViewModelCount>("COACategory2");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _COACategory2Service.GetCOACategory2ListAsync(CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<COACategoryViewModelCount>("COACategory2", cacheData, expirationTime);

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

        [HttpGet, Route("GetCOACategory2byid/{COACategoryId}")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> GetCOACategory2ById(Int16 COACategoryId)
        {
            var COACategoryViewModel = new COACategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory2, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"COACategory2_{COACategoryId}", out COACategoryViewModel? cachedProduct))
                        {
                            COACategoryViewModel = cachedProduct;
                        }
                        else
                        {
                            COACategoryViewModel = _mapper.Map<COACategoryViewModel>(await _COACategory2Service.GetCOACategory2ByIdAsync(CompanyId, COACategoryId, UserId));

                            if (COACategoryViewModel == null)
                                return NotFound();
                            else
                                // Cache the COACategory2 with an expiration time of 10 minutes
                                _memoryCache.Set($"COACategory2_{COACategoryId}", COACategoryViewModel, TimeSpan.FromMinutes(10));
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

        [HttpPost, Route("AddCOACategory2")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> CreateCOACategory2(COACategoryViewModel COACategory2)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory2, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (COACategory2 == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_COACategory2 ID mismatch");

                            var COACategory2Entity = new M_COACategory2
                            {
                                CompanyId = COACategory2.CompanyId,
                                COACategoryCode = COACategory2.COACategoryCode,
                                COACategoryId = COACategory2.COACategoryId,
                                COACategoryName = COACategory2.COACategoryName,
                                CreateById = UserId,
                                IsActive = COACategory2.IsActive,
                                Remarks = COACategory2.Remarks
                            };

                            var createdCOACategory2 = await _COACategory2Service.AddCOACategory2Async(CompanyId, COACategory2Entity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCOACategory2);

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
                    "Error creating new COACategory2 record");
            }
        }

        [HttpPut, Route("UpdateCOACategory2/{COACategoryId}")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> UpdateCOACategory2(Int16 COACategoryId, [FromBody] COACategoryViewModel COACategory2)
        {
            var COACategoryViewModel = new COACategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory2, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (COACategoryId != COACategory2.COACategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_COACategory2 ID mismatch");
                            //return BadRequest("M_COACategory2 ID mismatch");

                            // Attempt to retrieve the COACategory2 from the cache
                            if (_memoryCache.TryGetValue($"COACategory2_{COACategoryId}", out COACategoryViewModel? cachedProduct))
                            {
                                COACategoryViewModel = cachedProduct;
                            }
                            else
                            {
                                var COACategory2ToUpdate = await _COACategory2Service.GetCOACategory2ByIdAsync(CompanyId, COACategoryId, UserId);

                                if (COACategory2ToUpdate == null)
                                    return NotFound($"M_COACategory2 with Id = {COACategoryId} not found");
                            }

                            var COACategory2Entity = new M_COACategory2
                            {
                                COACategoryCode = COACategory2.COACategoryCode,
                                COACategoryId = COACategory2.COACategoryId,
                                COACategoryName = COACategory2.COACategoryName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = COACategory2.IsActive,
                                Remarks = COACategory2.Remarks
                            };

                            var sqlResponce = await _COACategory2Service.UpdateCOACategory2Async(CompanyId, COACategory2Entity, UserId);
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
        //public async Task<ActionResult<COACategoryViewModel>> DeleteCOACategory2(Int16 COACategoryId)
        //{
        //    try
        //    {
        //        CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
        //        UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

        //        if (ValidateHeaders(CompanyId, UserId))
        //        {
        //            var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory2, UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var COACategory2ToDelete = await _COACategory2Service.GetCOACategory2ByIdAsync(CompanyId, COACategoryId, UserId);

        //                    if (COACategory2ToDelete == null)
        //                        return NotFound($"M_COACategory2 with Id = {COACategoryId} not found");

        //                    var sqlResponce = await _COACategory2Service.DeleteCOACategory2Async(CompanyId, COACategory2ToDelete, UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"COACategory2_{COACategoryId}");
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

