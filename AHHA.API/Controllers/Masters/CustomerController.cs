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
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _CustomerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CustomerController> logger, ICustomerService CustomerService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CustomerService = CustomerService;
        }

        [HttpGet, Route("GetCustomer")]
        [Authorize]
        public async Task<ActionResult> GetCustomer([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _CustomerService.GetCustomerListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetCustomerbyid/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult<CustomerViewModel>> GetCustomerById(Int16 CustomerId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var CustomerViewModel = _mapper.Map<CustomerViewModel>(await _CustomerService.GetCustomerByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, headerViewModel.UserId));

                        if (CustomerViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, CustomerViewModel);
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

        [HttpGet, Route("GetCustomerbycode/{CustomerCode}")]
        [Authorize]
        public async Task<ActionResult<CustomerViewModel>> GetCustomerByCode(string CustomerCode, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var CustomerViewModel = _mapper.Map<CustomerViewModel>(await _CustomerService.GetCustomerByCodeAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerCode, headerViewModel.UserId));

                        if (CustomerViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, CustomerViewModel);
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

        [HttpPost, Route("AddCustomer")]
        [Authorize]
        public async Task<ActionResult<CustomerViewModel>> CreateCustomer(CustomerViewModel customerViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (customerViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);
                            else if (customerViewModel.CustomerCode == null || customerViewModel.CustomerCode == string.Empty)
                                return StatusCode(StatusCodes.Status400BadRequest, "Customer Code null");

                            var CustomerEntity = new M_Customer
                            {
                                CustomerId = customerViewModel.CustomerId,
                                CompanyId = headerViewModel.CompanyId,
                                CustomerCode = customerViewModel.CustomerCode,
                                CustomerName = customerViewModel.CustomerName,
                                CustomerOtherName = customerViewModel.CustomerOtherName,
                                CustomerShortName = customerViewModel.CustomerShortName,
                                CustomerRegNo = customerViewModel.CustomerRegNo,
                                CurrencyId = customerViewModel.CurrencyId,
                                CreditTermId = customerViewModel.CreditTermId,
                                ParentCustomerId = customerViewModel.ParentCustomerId,
                                IsCustomer = customerViewModel.IsCustomer,
                                IsVendor = customerViewModel.IsVendor,
                                IsTrader = customerViewModel.IsTrader,
                                IsSupplier = customerViewModel.IsSupplier,
                                Remarks = customerViewModel.Remarks,
                                IsActive = customerViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                            };

                            var createdCustomer = await _CustomerService.AddCustomerAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCustomer);
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
                    "Error creating new Customer record");
            }
        }

        [HttpPut, Route("UpdateCustomer/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult<CustomerViewModel>> UpdateCustomer(Int16 CustomerId, [FromBody] CustomerViewModel customerViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (CustomerId != customerViewModel.CustomerId)
                                return StatusCode(StatusCodes.Status400BadRequest, "Customer ID mismatch");

                            var CustomerToUpdate = await _CustomerService.GetCustomerByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, headerViewModel.UserId);

                            if (CustomerToUpdate == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var CustomerEntity = new M_Customer
                            {
                                CustomerId = customerViewModel.CustomerId,
                                CompanyId = headerViewModel.CompanyId,
                                CustomerCode = customerViewModel.CustomerCode,
                                CustomerName = customerViewModel.CustomerName,
                                CustomerOtherName = customerViewModel.CustomerOtherName,
                                CustomerShortName = customerViewModel.CustomerShortName,
                                CustomerRegNo = customerViewModel.CustomerRegNo,
                                CurrencyId = customerViewModel.CurrencyId,
                                CreditTermId = customerViewModel.CreditTermId,
                                ParentCustomerId = customerViewModel.ParentCustomerId,
                                IsCustomer = customerViewModel.IsCustomer,
                                IsVendor = customerViewModel.IsVendor,
                                IsTrader = customerViewModel.IsTrader,
                                IsSupplier = customerViewModel.IsSupplier,
                                Remarks = customerViewModel.Remarks,
                                IsActive = customerViewModel.IsActive,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponce = await _CustomerService.UpdateCustomerAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteCustomer/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult<M_Customer>> DeleteCustomer(Int16 CustomerId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var CustomerToDelete = await _CustomerService.GetCustomerByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, headerViewModel.UserId);

                            if (CustomerToDelete == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var sqlResponce = await _CustomerService.DeleteCustomerAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerToDelete, headerViewModel.UserId);

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