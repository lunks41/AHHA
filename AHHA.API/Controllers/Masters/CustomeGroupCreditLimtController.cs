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
    public class CustomeGroupCreditLimtController : BaseController
    {
        private readonly ICustomeGroupCreditLimtService _CustomeGroupCreditLimtService;
        private readonly ILogger<CustomeGroupCreditLimtController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public CustomeGroupCreditLimtController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CustomeGroupCreditLimtController> logger, ICustomeGroupCreditLimtService CustomeGroupCreditLimtService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CustomeGroupCreditLimtService = CustomeGroupCreditLimtService;
        }

        [HttpGet, Route("GetCustomeGroupCreditLimt")]
        [Authorize]
        public async Task<ActionResult> GetAllCustomeGroupCreditLimt()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerGroupCreditLimt, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<CustomeGroupCreditLimtViewModelCount>("CustomeGroupCreditLimt");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _CustomeGroupCreditLimtService.GetCustomeGroupCreditLimtListAsync(CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<CustomeGroupCreditLimtViewModelCount>("CustomeGroupCreditLimt", cacheData, expirationTime);

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

        [HttpGet, Route("GetCustomeGroupCreditLimtbyid/{GroupCreditLimitId}")]
        [Authorize]
        public async Task<ActionResult<CustomeGroupCreditLimtViewModel>> GetCustomeGroupCreditLimtById(Int16 GroupCreditLimitId)
        {
            var CustomeGroupCreditLimtViewModel = new CustomeGroupCreditLimtViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerGroupCreditLimt, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"CustomeGroupCreditLimt_{GroupCreditLimitId}", out CustomeGroupCreditLimtViewModel? cachedProduct))
                        {
                            CustomeGroupCreditLimtViewModel = cachedProduct;
                        }
                        else
                        {
                            CustomeGroupCreditLimtViewModel = _mapper.Map<CustomeGroupCreditLimtViewModel>(await _CustomeGroupCreditLimtService.GetCustomeGroupCreditLimtByIdAsync(CompanyId, GroupCreditLimitId, UserId));

                            if (CustomeGroupCreditLimtViewModel == null)
                                return NotFound();
                            else
                                // Cache the CustomeGroupCreditLimt with an expiration time of 10 minutes
                                _memoryCache.Set($"CustomeGroupCreditLimt_{GroupCreditLimitId}", CustomeGroupCreditLimtViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, CustomeGroupCreditLimtViewModel);
                        //return Ok(CustomeGroupCreditLimtViewModel);
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

        [HttpPost, Route("AddCustomeGroupCreditLimt")]
        [Authorize]
        public async Task<ActionResult<CustomeGroupCreditLimtViewModel>> CreateCustomeGroupCreditLimt(CustomeGroupCreditLimtViewModel CustomeGroupCreditLimt)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerGroupCreditLimt, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (CustomeGroupCreditLimt == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_CustomeGroupCreditLimt ID mismatch");

                            var CustomeGroupCreditLimtEntity = new M_CustomeGroupCreditLimt
                            {
                                CompanyId = CustomeGroupCreditLimt.CompanyId,
                                GroupCreditLimitCode = CustomeGroupCreditLimt.GroupCreditLimitCode,
                                GroupCreditLimitId = CustomeGroupCreditLimt.GroupCreditLimitId,
                                GroupCreditLimitName = CustomeGroupCreditLimt.GroupCreditLimitName,
                                CreateById = UserId,
                                IsActive = CustomeGroupCreditLimt.IsActive,
                                Remarks = CustomeGroupCreditLimt.Remarks
                            };

                            var createdCustomeGroupCreditLimt = await _CustomeGroupCreditLimtService.AddCustomeGroupCreditLimtAsync(CompanyId, CustomeGroupCreditLimtEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCustomeGroupCreditLimt);

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
                    "Error creating new CustomeGroupCreditLimt record");
            }
        }

        [HttpPut, Route("UpdateCustomeGroupCreditLimt/{GroupCreditLimitId}")]
        [Authorize]
        public async Task<ActionResult<CustomeGroupCreditLimtViewModel>> UpdateCustomeGroupCreditLimt(Int16 GroupCreditLimitId, [FromBody] CustomeGroupCreditLimtViewModel CustomeGroupCreditLimt)
        {
            var CustomeGroupCreditLimtViewModel = new CustomeGroupCreditLimtViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerGroupCreditLimt, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (GroupCreditLimitId != CustomeGroupCreditLimt.GroupCreditLimitId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_CustomeGroupCreditLimt ID mismatch");
                            //return BadRequest("M_CustomeGroupCreditLimt ID mismatch");

                            // Attempt to retrieve the CustomeGroupCreditLimt from the cache
                            if (_memoryCache.TryGetValue($"CustomeGroupCreditLimt_{GroupCreditLimitId}", out CustomeGroupCreditLimtViewModel? cachedProduct))
                            {
                                CustomeGroupCreditLimtViewModel = cachedProduct;
                            }
                            else
                            {
                                var CustomeGroupCreditLimtToUpdate = await _CustomeGroupCreditLimtService.GetCustomeGroupCreditLimtByIdAsync(CompanyId, GroupCreditLimitId, UserId);

                                if (CustomeGroupCreditLimtToUpdate == null)
                                    return NotFound($"M_CustomeGroupCreditLimt with Id = {GroupCreditLimitId} not found");
                            }

                            var CustomeGroupCreditLimtEntity = new M_CustomeGroupCreditLimt
                            {
                                CompanyId = CustomeGroupCreditLimt.CompanyId,
                                GroupCreditLimitCode = CustomeGroupCreditLimt.GroupCreditLimitCode,
                                GroupCreditLimitId = CustomeGroupCreditLimt.GroupCreditLimitId,
                                GroupCreditLimitName = CustomeGroupCreditLimt.GroupCreditLimitName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = CustomeGroupCreditLimt.IsActive,
                                Remarks = CustomeGroupCreditLimt.Remarks
                            };

                            var sqlResponce = await _CustomeGroupCreditLimtService.UpdateCustomeGroupCreditLimtAsync(CompanyId, CustomeGroupCreditLimtEntity, UserId);
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

        //[HttpDelete, Route("Delete/{GroupCreditLimitId}")]
        //[Authorize]
        //public async Task<ActionResult<M_CustomeGroupCreditLimt>> DeleteCustomeGroupCreditLimt(Int16 GroupCreditLimitId)
        //{
        //    try
        //    {
        //        CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
        //        UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

        //        if (ValidateHeaders(CompanyId, UserId))
        //        {
        //            var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerGroupCreditLimt, UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var CustomeGroupCreditLimtToDelete = await _CustomeGroupCreditLimtService.GetCustomeGroupCreditLimtByIdAsync(CompanyId, GroupCreditLimitId, UserId);

        //                    if (CustomeGroupCreditLimtToDelete == null)
        //                        return NotFound($"M_CustomeGroupCreditLimt with Id = {GroupCreditLimitId} not found");

        //                    var sqlResponce = await _CustomeGroupCreditLimtService.DeleteCustomeGroupCreditLimtAsync(CompanyId, CustomeGroupCreditLimtToDelete, UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"CustomeGroupCreditLimt_{GroupCreditLimitId}");
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


