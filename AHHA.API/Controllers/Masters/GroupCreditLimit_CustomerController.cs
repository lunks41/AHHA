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
        
       
       
       
       
        

        public GroupCreditLimit_CustomerController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<GroupCreditLimit_CustomerController> logger, IGroupCreditLimit_CustomerService GroupCreditLimit_CustomerService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _GroupCreditLimit_CustomerService = GroupCreditLimit_CustomerService;
        }

        [HttpGet, Route("GetGroupCreditLimit_Customer")]
        [Authorize]
        public async Task<ActionResult> GetAllGroupCreditLimit_Customer([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                
                
                

                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt_Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                            var cacheData = await _GroupCreditLimit_CustomerService.GetGroupCreditLimit_CustomerListAsync(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

                            if (cacheData == null)
                                return NotFound(GenrateMessage.authenticationfailed);

                           return Ok(cacheData);

                    }
                    else
                    {
                        return NotFound(GenrateMessage.authenticationfailed);
                    }
                }
                else
                {
                        return NotFound(GenrateMessage.authenticationfailed);
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
        public async Task<ActionResult<GroupCreditLimt_CustomerViewModel>> GetGroupCreditLimit_CustomerById(Int16 GroupCreditLimitId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var GroupCreditLimt_CustomerViewModel = new GroupCreditLimt_CustomerViewModel();
            try
            {
                
                

                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt_Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"GroupCreditLimit_Customer_{GroupCreditLimitId}", out GroupCreditLimt_CustomerViewModel? cachedProduct))
                        {
                            GroupCreditLimt_CustomerViewModel = cachedProduct;
                        }
                        else
                        {
                            GroupCreditLimt_CustomerViewModel = _mapper.Map<GroupCreditLimt_CustomerViewModel>(await _GroupCreditLimit_CustomerService.GetGroupCreditLimit_CustomerByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, GroupCreditLimitId, headerViewModel.UserId));

                            if (GroupCreditLimt_CustomerViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the GroupCreditLimit_Customer with an expiration time of 10 minutes
                                _memoryCache.Set($"GroupCreditLimit_Customer_{GroupCreditLimitId}", GroupCreditLimt_CustomerViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, GroupCreditLimt_CustomerViewModel);
                        //return Ok(GroupCreditLimt_CustomerViewModel);
                    }
                    else
                    {
                        return NotFound(GenrateMessage.authenticationfailed);
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
        public async Task<ActionResult<GroupCreditLimt_CustomerViewModel>> CreateGroupCreditLimit_Customer(GroupCreditLimt_CustomerViewModel GroupCreditLimit_Customer, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                
                

                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt_Customer, headerViewModel.UserId);

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

                            var createdGroupCreditLimit_Customer = await _GroupCreditLimit_CustomerService.AddGroupCreditLimit_CustomerAsync(headerViewModel.RegId,headerViewModel.CompanyId, GroupCreditLimit_CustomerEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdGroupCreditLimit_Customer);

                        }
                        else
                        {
                            return NotFound(GenrateMessage.authenticationfailed);
                        }
                    }
                    else
                    {
                        return NotFound(GenrateMessage.authenticationfailed);
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
        public async Task<ActionResult<GroupCreditLimt_CustomerViewModel>> UpdateGroupCreditLimit_Customer(Int16 GroupCreditLimitId, [FromBody] GroupCreditLimt_CustomerViewModel GroupCreditLimit_Customer, [FromHeader] HeaderViewModel headerViewModel)
        {
            var GroupCreditLimt_CustomerViewModel = new GroupCreditLimt_CustomerViewModel();
            try
            {
                
                

                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt_Customer, headerViewModel.UserId);

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
                                var GroupCreditLimit_CustomerToUpdate = await _GroupCreditLimit_CustomerService.GetGroupCreditLimit_CustomerByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, GroupCreditLimitId, headerViewModel.UserId);

                                if (GroupCreditLimit_CustomerToUpdate == null)
                                    return NotFound($"M_GroupCreditLimt_Customer with Id = {GroupCreditLimitId} not found");
                            }

                            var GroupCreditLimit_CustomerEntity = new M_GroupCreditLimt_Customer
                            {
                                CompanyId = GroupCreditLimit_Customer.CompanyId,
                                GroupCreditLimitId = GroupCreditLimit_Customer.GroupCreditLimitId,
                                CustomerId = GroupCreditLimit_Customer.CustomerId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponce = await _GroupCreditLimit_CustomerService.UpdateGroupCreditLimit_CustomerAsync(headerViewModel.RegId,headerViewModel.CompanyId, GroupCreditLimit_CustomerEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
                        }
                        else
                        {
                            return NotFound(GenrateMessage.authenticationfailed);
                        }
                    }
                    else
                    {
                        return NotFound(GenrateMessage.authenticationfailed);
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
        //        
        //        

        //        if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
        //        {
        //            var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.GroupCreditLimt_Customer, headerViewModel.UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var GroupCreditLimit_CustomerToDelete = await _GroupCreditLimit_CustomerService.GetGroupCreditLimit_CustomerByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, GroupCreditLimitId, headerViewModel.UserId);

        //                    if (GroupCreditLimit_CustomerToDelete == null)
        //                        return NotFound($"M_GroupCreditLimt_Customer with Id = {GroupCreditLimitId} not found");

        //                    var sqlResponce = await _GroupCreditLimit_CustomerService.DeleteGroupCreditLimit_CustomerAsync(headerViewModel.RegId,headerViewModel.CompanyId, GroupCreditLimit_CustomerToDelete, headerViewModel.UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"GroupCreditLimit_Customer_{GroupCreditLimitId}");
        //                    return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
        //                }
        //                else
        //                {
        //                    return NotFound(GenrateMessage.authenticationfailed);
        //                }
        //            }
        //            else
        //            {
        //                return NotFound(GenrateMessage.authenticationfailed);
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

