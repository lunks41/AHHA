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
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, cacheData);
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
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerCreditLimit, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var customerCreditLimitViewModel = _mapper.Map<CustomerCreditLimitViewModel>(await _CustomerCreditLimitService.GetCustomerCreditLimitByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, headerViewModel.UserId));

                        if (customerCreditLimitViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, customerCreditLimitViewModel);
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
        public async Task<ActionResult<CustomerCreditLimitViewModel>> CreateCustomerCreditLimit(CustomerCreditLimitViewModel customerCreditLimitViewModel, [FromHeader] HeaderViewModel headerViewModel)
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
                            if (customerCreditLimitViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var CustomerCreditLimitEntity = new M_CustomerCreditLimit
                            {
                                CustomerId = customerCreditLimitViewModel.CustomerId,
                                CompanyId = headerViewModel.CompanyId,
                                EffectFrom = customerCreditLimitViewModel.EffectFrom,
                                EffectUntil = customerCreditLimitViewModel.EffectUntil,
                                IsExpires = customerCreditLimitViewModel.IsExpires,
                                CreditLimitAmt = customerCreditLimitViewModel.CreditLimitAmt,
                                CreateById = headerViewModel.UserId
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
        public async Task<ActionResult<CustomerCreditLimitViewModel>> UpdateCustomerCreditLimit(Int16 CustomerId, [FromBody] CustomerCreditLimitViewModel customerCreditLimitViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerCreditLimit, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (CustomerId != customerCreditLimitViewModel.CustomerId)
                                return StatusCode(StatusCodes.Status400BadRequest, "CustomerCreditLimit ID mismatch");

                            var CustomerCreditLimitToUpdate = await _CustomerCreditLimitService.GetCustomerCreditLimitByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, headerViewModel.UserId);

                            if (CustomerCreditLimitToUpdate == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var CustomerCreditLimitEntity = new M_CustomerCreditLimit
                            {
                                CustomerId = customerCreditLimitViewModel.CustomerId,
                                CompanyId = headerViewModel.CompanyId,
                                EffectFrom = customerCreditLimitViewModel.EffectFrom,
                                EffectUntil = customerCreditLimitViewModel.EffectUntil,
                                IsExpires = customerCreditLimitViewModel.IsExpires,
                                CreditLimitAmt = customerCreditLimitViewModel.CreditLimitAmt,
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
                                return NotFound(GenrateMessage.datanotfound);

                            var sqlResponce = await _CustomerCreditLimitService.DeleteCustomerCreditLimitAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerCreditLimitToDelete, headerViewModel.UserId);

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