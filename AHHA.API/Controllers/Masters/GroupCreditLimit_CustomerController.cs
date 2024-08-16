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
    public class GroupCreditLimit_CustomerController : BaseController
    {
        private readonly IGroupCreditLimit_CustomerService _GroupCreditLimit_CustomerService;
        private readonly ILogger<GroupCreditLimit_CustomerController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public GroupCreditLimit_CustomerController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<GroupCreditLimit_CustomerController> logger, IGroupCreditLimit_CustomerService GroupCreditLimit_CustomerService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _GroupCreditLimit_CustomerService = GroupCreditLimit_CustomerService;
        }

        [HttpGet, Route("GetGroupCreditLimit_Customer")]
        [Authorize]
        public async Task<ActionResult> GetAllGroupCreditLimit_Customer()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt_Customer, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<GroupCreditLimt_CustomerViewModelCount>("GroupCreditLimit_Customer");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _GroupCreditLimit_CustomerService.GetGroupCreditLimit_CustomerListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<GroupCreditLimt_CustomerViewModelCount>("GroupCreditLimit_Customer", cacheData, expirationTime);

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

        [HttpGet, Route("GetGroupCreditLimit_Customerbyid/{GroupCreditLimitId}")]
        [Authorize]
        public async Task<ActionResult<GroupCreditLimt_CustomerViewModel>> GetGroupCreditLimit_CustomerById(Int16 GroupCreditLimitId)
        {
            var GroupCreditLimt_CustomerViewModel = new GroupCreditLimt_CustomerViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt_Customer, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"GroupCreditLimit_Customer_{GroupCreditLimitId}", out GroupCreditLimt_CustomerViewModel? cachedProduct))
                        {
                            GroupCreditLimt_CustomerViewModel = cachedProduct;
                        }
                        else
                        {
                            GroupCreditLimt_CustomerViewModel = _mapper.Map<GroupCreditLimt_CustomerViewModel>(await _GroupCreditLimit_CustomerService.GetGroupCreditLimit_CustomerByIdAsync(RegId,CompanyId, GroupCreditLimitId, UserId));

                            if (GroupCreditLimt_CustomerViewModel == null)
                                return NotFound();
                            else
                                // Cache the GroupCreditLimit_Customer with an expiration time of 10 minutes
                                _memoryCache.Set($"GroupCreditLimit_Customer_{GroupCreditLimitId}", GroupCreditLimt_CustomerViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, GroupCreditLimt_CustomerViewModel);
                        //return Ok(GroupCreditLimt_CustomerViewModel);
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

        [HttpPost, Route("AddGroupCreditLimit_Customer")]
        [Authorize]
        public async Task<ActionResult<GroupCreditLimt_CustomerViewModel>> CreateGroupCreditLimit_Customer(GroupCreditLimt_CustomerViewModel GroupCreditLimit_Customer)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt_Customer, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (GroupCreditLimit_Customer == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_GroupCreditLimt_Customer ID mismatch");

                            var GroupCreditLimit_CustomerEntity = new M_GroupCreditLimt_Customer
                            {
                                CompanyId = GroupCreditLimit_Customer.CompanyId,
                                GroupCreditLimitId = GroupCreditLimit_Customer.GroupCreditLimitId,
                                CustomerId = GroupCreditLimit_Customer.CustomerId,
                            };

                            var createdGroupCreditLimit_Customer = await _GroupCreditLimit_CustomerService.AddGroupCreditLimit_CustomerAsync(RegId,CompanyId, GroupCreditLimit_CustomerEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdGroupCreditLimit_Customer);

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
                    "Error creating new GroupCreditLimit_Customer record");
            }
        }

        [HttpPut, Route("UpdateGroupCreditLimit_Customer/{GroupCreditLimitId}")]
        [Authorize]
        public async Task<ActionResult<GroupCreditLimt_CustomerViewModel>> UpdateGroupCreditLimit_Customer(Int16 GroupCreditLimitId, [FromBody] GroupCreditLimt_CustomerViewModel GroupCreditLimit_Customer)
        {
            var GroupCreditLimt_CustomerViewModel = new GroupCreditLimt_CustomerViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt_Customer, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (GroupCreditLimitId != GroupCreditLimit_Customer.GroupCreditLimitId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_GroupCreditLimt_Customer ID mismatch");
                            //return BadRequest("M_GroupCreditLimt_Customer ID mismatch");

                            // Attempt to retrieve the GroupCreditLimit_Customer from the cache
                            if (_memoryCache.TryGetValue($"GroupCreditLimit_Customer_{GroupCreditLimitId}", out GroupCreditLimt_CustomerViewModel? cachedProduct))
                            {
                                GroupCreditLimt_CustomerViewModel = cachedProduct;
                            }
                            else
                            {
                                var GroupCreditLimit_CustomerToUpdate = await _GroupCreditLimit_CustomerService.GetGroupCreditLimit_CustomerByIdAsync(RegId,CompanyId, GroupCreditLimitId, UserId);

                                if (GroupCreditLimit_CustomerToUpdate == null)
                                    return NotFound($"M_GroupCreditLimt_Customer with Id = {GroupCreditLimitId} not found");
                            }

                            var GroupCreditLimit_CustomerEntity = new M_GroupCreditLimt_Customer
                            {
                                CompanyId = GroupCreditLimit_Customer.CompanyId,
                                GroupCreditLimitId = GroupCreditLimit_Customer.GroupCreditLimitId,
                                CustomerId = GroupCreditLimit_Customer.CustomerId,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponce = await _GroupCreditLimit_CustomerService.UpdateGroupCreditLimit_CustomerAsync(RegId,CompanyId, GroupCreditLimit_CustomerEntity, UserId);
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
        //public async Task<ActionResult<M_GroupCreditLimt_Customer>> DeleteGroupCreditLimit_Customer(Int16 GroupCreditLimitId)
        //{
        //    try
        //    {
        //        CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
        //        UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

        //        if (ValidateHeaders(RegId,CompanyId, UserId))
        //        {
        //            var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt_Customer, UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var GroupCreditLimit_CustomerToDelete = await _GroupCreditLimit_CustomerService.GetGroupCreditLimit_CustomerByIdAsync(RegId,CompanyId, GroupCreditLimitId, UserId);

        //                    if (GroupCreditLimit_CustomerToDelete == null)
        //                        return NotFound($"M_GroupCreditLimt_Customer with Id = {GroupCreditLimitId} not found");

        //                    var sqlResponce = await _GroupCreditLimit_CustomerService.DeleteGroupCreditLimit_CustomerAsync(RegId,CompanyId, GroupCreditLimit_CustomerToDelete, UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"GroupCreditLimit_Customer_{GroupCreditLimitId}");
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

