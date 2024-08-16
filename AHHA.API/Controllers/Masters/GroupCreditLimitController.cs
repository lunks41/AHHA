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
    public class GroupCreditLimitController : BaseController
    {
        private readonly IGroupCreditLimitService _GroupCreditLimitService;
        private readonly ILogger<GroupCreditLimitController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public GroupCreditLimitController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<GroupCreditLimitController> logger, IGroupCreditLimitService GroupCreditLimitService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _GroupCreditLimitService = GroupCreditLimitService;
        }

        [HttpGet, Route("GetGroupCreditLimit")]
        [Authorize]
        public async Task<ActionResult> GetAllGroupCreditLimit()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<GroupCreditLimtViewModelCount>("GroupCreditLimit");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _GroupCreditLimitService.GetGroupCreditLimitListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<GroupCreditLimtViewModelCount>("GroupCreditLimit", cacheData, expirationTime);

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

        [HttpGet, Route("GetGroupCreditLimitbyid/{GroupCreditLimitId}")]
        [Authorize]
        public async Task<ActionResult<GroupCreditLimtViewModel>> GetGroupCreditLimitById(Int16 GroupCreditLimitId)
        {
            var GroupCreditLimtViewModel = new GroupCreditLimtViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"GroupCreditLimit_{GroupCreditLimitId}", out GroupCreditLimtViewModel? cachedProduct))
                        {
                            GroupCreditLimtViewModel = cachedProduct;
                        }
                        else
                        {
                            GroupCreditLimtViewModel = _mapper.Map<GroupCreditLimtViewModel>(await _GroupCreditLimitService.GetGroupCreditLimitByIdAsync(RegId,CompanyId, GroupCreditLimitId, UserId));

                            if (GroupCreditLimtViewModel == null)
                                return NotFound();
                            else
                                // Cache the GroupCreditLimit with an expiration time of 10 minutes
                                _memoryCache.Set($"GroupCreditLimit_{GroupCreditLimitId}", GroupCreditLimtViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, GroupCreditLimtViewModel);
                        //return Ok(GroupCreditLimtViewModel);
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

        [HttpPost, Route("AddGroupCreditLimit")]
        [Authorize]
        public async Task<ActionResult<GroupCreditLimtViewModel>> CreateGroupCreditLimit(GroupCreditLimtViewModel GroupCreditLimit)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (GroupCreditLimit == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_GroupCreditLimit ID mismatch");

                            var GroupCreditLimitEntity = new M_GroupCreditLimt
                            {
                                CompanyId = GroupCreditLimit.CompanyId,
                                GroupCreditLimitCode = GroupCreditLimit.GroupCreditLimitCode,
                                GroupCreditLimitId = GroupCreditLimit.GroupCreditLimitId,
                                GroupCreditLimitName = GroupCreditLimit.GroupCreditLimitName,
                                CreateById = UserId,
                                IsActive = GroupCreditLimit.IsActive,
                                Remarks = GroupCreditLimit.Remarks
                            };

                            var createdGroupCreditLimit = await _GroupCreditLimitService.AddGroupCreditLimitAsync(RegId,CompanyId, GroupCreditLimitEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdGroupCreditLimit);

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
                    "Error creating new GroupCreditLimit record");
            }
        }

        [HttpPut, Route("UpdateGroupCreditLimit/{GroupCreditLimitId}")]
        [Authorize]
        public async Task<ActionResult<GroupCreditLimtViewModel>> UpdateGroupCreditLimit(Int16 GroupCreditLimitId, [FromBody] GroupCreditLimtViewModel GroupCreditLimit)
        {
            var GroupCreditLimtViewModel = new GroupCreditLimtViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (GroupCreditLimitId != GroupCreditLimit.GroupCreditLimitId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_GroupCreditLimit ID mismatch");
                            //return BadRequest("M_GroupCreditLimit ID mismatch");

                            // Attempt to retrieve the GroupCreditLimit from the cache
                            if (_memoryCache.TryGetValue($"GroupCreditLimit_{GroupCreditLimitId}", out GroupCreditLimtViewModel? cachedProduct))
                            {
                                GroupCreditLimtViewModel = cachedProduct;
                            }
                            else
                            {
                                var GroupCreditLimitToUpdate = await _GroupCreditLimitService.GetGroupCreditLimitByIdAsync(RegId,CompanyId, GroupCreditLimitId, UserId);

                                if (GroupCreditLimitToUpdate == null)
                                    return NotFound($"M_GroupCreditLimit with Id = {GroupCreditLimitId} not found");
                            }

                            var GroupCreditLimitEntity = new M_GroupCreditLimt
                            {
                                GroupCreditLimitCode = GroupCreditLimit.GroupCreditLimitCode,
                                GroupCreditLimitId = GroupCreditLimit.GroupCreditLimitId,
                                GroupCreditLimitName = GroupCreditLimit.GroupCreditLimitName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = GroupCreditLimit.IsActive,
                                Remarks = GroupCreditLimit.Remarks
                            };

                            var sqlResponce = await _GroupCreditLimitService.UpdateGroupCreditLimitAsync(RegId,CompanyId, GroupCreditLimitEntity, UserId);
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
        //public async Task<ActionResult<M_GroupCreditLimt>> DeleteGroupCreditLimit(Int16 GroupCreditLimitId)
        //{
        //    try
        //    {
        //        CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
        //        UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

        //        if (ValidateHeaders(RegId,CompanyId, UserId))
        //        {
        //            var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt, UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var GroupCreditLimitToDelete = await _GroupCreditLimitService.GetGroupCreditLimitByIdAsync(RegId,CompanyId, GroupCreditLimitId, UserId);

        //                    if (GroupCreditLimitToDelete == null)
        //                        return NotFound($"M_GroupCreditLimit with Id = {GroupCreditLimitId} not found");

        //                    var sqlResponce = await _GroupCreditLimitService.DeleteGroupCreditLimitAsync(RegId,CompanyId, GroupCreditLimitToDelete, UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"GroupCreditLimit_{GroupCreditLimitId}");
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

