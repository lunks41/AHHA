﻿using AHHA.Application.IServices;
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
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.CustomerCreditLimit, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _CustomerCreditLimitService.GetCustomerCreditLimitListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, cacheData);
                    }
                    else
                    {
                        return NotFound(GenerateMessage.AuthenticationFailed);
                    }
                }
                else
                {
                    return NotFound(GenerateMessage.AuthenticationFailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.CustomerCreditLimit, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var customerCreditLimitViewModel = _mapper.Map<CustomerCreditLimitViewModel>(await _CustomerCreditLimitService.GetCustomerCreditLimitByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, headerViewModel.UserId));

                        if (customerCreditLimitViewModel == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, customerCreditLimitViewModel);
                    }
                    else
                    {
                        return NotFound(GenerateMessage.AuthenticationFailed);
                    }
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        //[HttpPost, Route("SaveCustomerCreditLimit")]
        //[Authorize]
        //public async Task<ActionResult<CustomerCreditLimitViewModel>> SaveCustomerCreditLimit(CustomerCreditLimitViewModel customerCreditLimitViewModel, [FromHeader] HeaderViewModel headerViewModel)
        //{
        //    try
        //    {
        //        if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
        //        {
        //            var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.CustomerCreditLimit, headerViewModel.UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsCreate)
        //                {
        //                    if (customerCreditLimitViewModel == null)
        //                        return NotFound(GenerateMessage.DataNotFound);

        //                    var CustomerCreditLimitEntity = new M_CustomerCreditLimit
        //                    {
        //                        CustomerId = customerCreditLimitViewModel.CustomerId,
        //                        CompanyId = headerViewModel.CompanyId,
        //                        EffectFrom = customerCreditLimitViewModel.EffectFrom,
        //                        EffectUntil = customerCreditLimitViewModel.EffectUntil,
        //                        IsExpires = customerCreditLimitViewModel.IsExpires,
        //                        Remarks = customerCreditLimitViewModel.Remarks.Trim(),
        //                        CreditLimitAmt = customerCreditLimitViewModel.CreditLimitAmt,
        //                        CreateById = headerViewModel.UserId,
        //                        EditById = headerViewModel.UserId,
        //                        EditDate = DateTime.Now
        //                    };

        //                    var sqlResponse = await _CustomerCreditLimitService.SaveCustomerCreditLimitAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerCreditLimitEntity, headerViewModel.UserId);
        //                    return StatusCode(StatusCodes.Status202Accepted, sqlResponse);
        //                }
        //                else
        //                {
        //                    return NotFound(GenerateMessage.AuthenticationFailed);
        //                }
        //            }
        //            else
        //            {
        //                return NotFound(GenerateMessage.AuthenticationFailed);
        //            }
        //        }
        //        else
        //        {
        //            return NoContent();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            "Error creating new CustomerCreditLimit record");
        //    }
        //}

        [HttpDelete, Route("DeleteCustomerCreditLimit/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult<M_CustomerCreditLimit>> DeleteCustomerCreditLimit(Int16 CustomerId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.CustomerCreditLimit, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var CustomerCreditLimitToDelete = await _CustomerCreditLimitService.GetCustomerCreditLimitByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, headerViewModel.UserId);

                            if (CustomerCreditLimitToDelete == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var sqlResponse = await _CustomerCreditLimitService.DeleteCustomerCreditLimitAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerCreditLimitToDelete, headerViewModel.UserId);

                            return StatusCode(StatusCodes.Status202Accepted, sqlResponse);
                        }
                        else
                        {
                            return NotFound(GenerateMessage.AuthenticationFailed);
                        }
                    }
                    else
                    {
                        return NotFound(GenerateMessage.AuthenticationFailed);
                    }
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }
}