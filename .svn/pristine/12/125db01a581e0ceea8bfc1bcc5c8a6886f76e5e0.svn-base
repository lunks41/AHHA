using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/Master")]
    [ApiController]
    public class CustomerCreditLimitController : BaseController
    {
        private readonly ICustomerCreditLimitService _CustomerCreditLimitService;
        private readonly ILogger<CustomerCreditLimitController> _logger;

        public CustomerCreditLimitController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CustomerCreditLimitController> logger, ICustomerCreditLimitService CustomerCreditLimitService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CustomerCreditLimitService = CustomerCreditLimitService;
        }

        [HttpGet, Route("GetCustomerCreditLimit")]
        [Authorize]
        public async Task<ActionResult> GetCustomerCreditLimit([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerCreditLimit, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        

                        var cacheData = await _CustomerCreditLimitService.GetCustomerCreditLimitListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetCustomerCreditLimitbyid/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult<CustomerCreditLimitViewModel>> GetCustomerCreditLimitById(Int16 CustomerId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var CustomerCreditLimitViewModel = new CustomerCreditLimitViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerCreditLimit, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"CustomerCreditLimit_{CustomerId}", out CustomerCreditLimitViewModel? cachedProduct))
                        {
                            CustomerCreditLimitViewModel = cachedProduct;
                        }
                        else
                        {
                            CustomerCreditLimitViewModel = _mapper.Map<CustomerCreditLimitViewModel>(await _CustomerCreditLimitService.GetCustomerCreditLimitByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, headerViewModel.UserId));

                            if (CustomerCreditLimitViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the CustomerCreditLimit with an expiration time of 10 minutes
                                _memoryCache.Set($"CustomerCreditLimit_{CustomerId}", CustomerCreditLimitViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, CustomerCreditLimitViewModel);
                        //return Ok(CustomerCreditLimitViewModel);
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

        [HttpPost, Route("AddCustomerCreditLimit")]
        [Authorize]
        public async Task<ActionResult<CustomerCreditLimitViewModel>> CreateCustomerCreditLimit(CustomerCreditLimitViewModel CustomerCreditLimit, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerCreditLimit, headerViewModel.UserId);

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
                                CreateById = headerViewModel.UserId,
                                IsExpires = CustomerCreditLimit.IsExpires,
                                CreditLimitAmt = CustomerCreditLimit.CreditLimitAmt
                            };

                            var createdCustomerCreditLimit = await _CustomerCreditLimitService.AddCustomerCreditLimitAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerCreditLimitEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCustomerCreditLimit);
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
                    "Error creating new CustomerCreditLimit record");
            }
        }

        [HttpPut, Route("UpdateCustomerCreditLimit/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult<CustomerCreditLimitViewModel>> UpdateCustomerCreditLimit(Int16 CustomerId, [FromBody] CustomerCreditLimitViewModel CustomerCreditLimit, [FromHeader] HeaderViewModel headerViewModel)
        {
            var CustomerCreditLimitViewModel = new CustomerCreditLimitViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerCreditLimit, headerViewModel.UserId);

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
                                var CustomerCreditLimitToUpdate = await _CustomerCreditLimitService.GetCustomerCreditLimitByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, headerViewModel.UserId);

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
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponce = await _CustomerCreditLimitService.UpdateCustomerCreditLimitAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerCreditLimitEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteCustomerCreditLimit/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult<M_CustomerCreditLimit>> DeleteCustomerCreditLimit(Int16 CustomerId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerCreditLimit, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var CustomerCreditLimitToDelete = await _CustomerCreditLimitService.GetCustomerCreditLimitByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, headerViewModel.UserId);

                            if (CustomerCreditLimitToDelete == null)
                                return NotFound($"M_CustomerCreditLimit with Id = {CustomerId} not found");

                            var sqlResponce = await _CustomerCreditLimitService.DeleteCustomerCreditLimitAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerCreditLimitToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"CustomerCreditLimit_{CustomerId}");
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
                    "Error deleting data");
            }
        }
    }
}