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
    public class CustomerCreditLimitController : BaseController
    {
        private readonly ICustomerCreditLimitService _CustomerCreditLimitService;
        private readonly ILogger<CustomerCreditLimitController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;
        public CustomerCreditLimitController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CustomerCreditLimitController> logger, ICustomerCreditLimitService CustomerCreditLimitService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CustomerCreditLimitService = CustomerCreditLimitService;
        }

        [HttpGet, Route("GetCustomerCreditLimit")]
        [Authorize]
        public async Task<ActionResult> GetAllCustomerCreditLimit()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerCreditLimit, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<CustomerCreditLimitViewModelCount>("CustomerCreditLimit");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _CustomerCreditLimitService.GetCustomerCreditLimitListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<CustomerCreditLimitViewModelCount>("CustomerCreditLimit", cacheData, expirationTime);

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

        [HttpGet, Route("GetCustomerCreditLimitbyid/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult<CustomerCreditLimitViewModel>> GetCustomerCreditLimitById(Int16 CustomerId)
        {
            var CustomerCreditLimitViewModel = new CustomerCreditLimitViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerCreditLimit, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"CustomerCreditLimit_{CustomerId}", out CustomerCreditLimitViewModel? cachedProduct))
                        {
                            CustomerCreditLimitViewModel = cachedProduct;
                        }
                        else
                        {
                            CustomerCreditLimitViewModel = _mapper.Map<CustomerCreditLimitViewModel>(await _CustomerCreditLimitService.GetCustomerCreditLimitByIdAsync(RegId,CompanyId, CustomerId, UserId));

                            if (CustomerCreditLimitViewModel == null)
                                return NotFound();
                            else
                                // Cache the CustomerCreditLimit with an expiration time of 10 minutes
                                _memoryCache.Set($"CustomerCreditLimit_{CustomerId}", CustomerCreditLimitViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, CustomerCreditLimitViewModel);
                        //return Ok(CustomerCreditLimitViewModel);
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

        [HttpPost, Route("AddCustomerCreditLimit")]
        [Authorize]
        public async Task<ActionResult<CustomerCreditLimitViewModel>> CreateCustomerCreditLimit(CustomerCreditLimitViewModel CustomerCreditLimit)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerCreditLimit, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (CustomerCreditLimit == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_CustomerCreditLimit ID mismatch");

                            var CustomerCreditLimitEntity = new M_CustomerCreditLimit
                            {
                                CompanyId = CustomerCreditLimit.CompanyId,
                                EffectFrom = CustomerCreditLimit.EffectFrom,
                                CustomerId = CustomerCreditLimit.CustomerId,
                                EffectUntil = CustomerCreditLimit.EffectUntil,
                                CreateById = UserId,
                                IsExpires = CustomerCreditLimit.IsExpires,
                                CreditLimitAmt = CustomerCreditLimit.CreditLimitAmt
                            };

                            var createdCustomerCreditLimit = await _CustomerCreditLimitService.AddCustomerCreditLimitAsync(RegId,CompanyId, CustomerCreditLimitEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCustomerCreditLimit);

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
                    "Error creating new CustomerCreditLimit record");
            }
        }

        [HttpPut, Route("UpdateCustomerCreditLimit/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult<CustomerCreditLimitViewModel>> UpdateCustomerCreditLimit(Int16 CustomerId, [FromBody] CustomerCreditLimitViewModel CustomerCreditLimit)
        {
            var CustomerCreditLimitViewModel = new CustomerCreditLimitViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerCreditLimit, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (CustomerId != CustomerCreditLimit.CustomerId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_CustomerCreditLimit ID mismatch");
                            //return BadRequest("M_CustomerCreditLimit ID mismatch");

                            // Attempt to retrieve the CustomerCreditLimit from the cache
                            if (_memoryCache.TryGetValue($"CustomerCreditLimit_{CustomerId}", out CustomerCreditLimitViewModel? cachedProduct))
                            {
                                CustomerCreditLimitViewModel = cachedProduct;
                            }
                            else
                            {
                                var CustomerCreditLimitToUpdate = await _CustomerCreditLimitService.GetCustomerCreditLimitByIdAsync(RegId,CompanyId, CustomerId, UserId);

                                if (CustomerCreditLimitToUpdate == null)
                                    return NotFound($"M_CustomerCreditLimit with Id = {CustomerId} not found");
                            }

                            var CustomerCreditLimitEntity = new M_CustomerCreditLimit
                            {
                                CompanyId = CustomerCreditLimit.CompanyId,
                                CustomerId = CustomerCreditLimit.CustomerId,
                                EffectFrom = CustomerCreditLimit.EffectFrom,
                                EffectUntil = CustomerCreditLimit.EffectUntil,
                                IsExpires = CustomerCreditLimit.IsExpires,
                                CreditLimitAmt = CustomerCreditLimit.CreditLimitAmt,
                                EditById = UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponce = await _CustomerCreditLimitService.UpdateCustomerCreditLimitAsync(RegId,CompanyId, CustomerCreditLimitEntity, UserId);
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

        //[HttpDelete, Route("Delete/{CustomerId}")]
        //[Authorize]
        //public async Task<ActionResult<M_CustomerCreditLimit>> DeleteCustomerCreditLimit(Int16 CustomerId)
        //{
        //    try
        //    {
        //        CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
        //        UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

        //        if (ValidateHeaders(RegId,CompanyId, UserId))
        //        {
        //            var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerCreditLimit, UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var CustomerCreditLimitToDelete = await _CustomerCreditLimitService.GetCustomerCreditLimitByIdAsync(RegId,CompanyId, CustomerId, UserId);

        //                    if (CustomerCreditLimitToDelete == null)
        //                        return NotFound($"M_CustomerCreditLimit with Id = {CustomerId} not found");

        //                    var sqlResponce = await _CustomerCreditLimitService.DeleteCustomerCreditLimitAsync(RegId,CompanyId, CustomerCreditLimitToDelete, UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"CustomerCreditLimit_{CustomerId}");
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

