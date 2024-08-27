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
    public class CustomerAddressController : BaseController
    {
        private readonly ICustomerAddressService _CustomerAddressService;
        private readonly ILogger<CustomerAddressController> _logger;

        public CustomerAddressController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CustomerAddressController> logger, ICustomerAddressService CustomerAddressService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CustomerAddressService = CustomerAddressService;
        }

        [HttpGet, Route("GetCustomerAddress")]
        [Authorize]
        public async Task<ActionResult> GetCustomerAddress([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _CustomerAddressService.GetCustomerAddressListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenrateMessage.authenticationfailed);

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

        [HttpGet, Route("GetCustomerAddressbyCustomerId/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult> GetCustomerAddressByCustomerId(Int16 CustomerId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var customerAddressViewModel = await _CustomerAddressService.GetCustomerAddressByCustomerIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, headerViewModel.UserId);

                        if (customerAddressViewModel == null)
                            return NotFound(GenrateMessage.authenticationfailed);

                        return StatusCode(StatusCodes.Status202Accepted, customerAddressViewModel);
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

        [HttpGet, Route("GetCustomerAddressbyid/{AddressId}")]
        [Authorize]
        public async Task<ActionResult<CustomerAddressViewModel>> GetCustomerAddressById(Int16 AddressId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var customerAddressViewModel = _mapper.Map<CustomerAddressViewModel>(await _CustomerAddressService.GetCustomerAddressByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AddressId, headerViewModel.UserId));

                        if (customerAddressViewModel == null)
                            return NotFound(GenrateMessage.authenticationfailed);

                        return StatusCode(StatusCodes.Status202Accepted, customerAddressViewModel);
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

        [HttpPost, Route("AddCustomerAddress")]
        [Authorize]
        public async Task<ActionResult<CustomerAddressViewModel>> CreateCustomerAddress(CustomerAddressViewModel customerAddress, [FromHeader] HeaderViewModel headerViewModel)
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
                            if (customerAddress == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "No input exist");

                            var CustomerAddressEntity = new M_CustomerAddress
                            {
                                CustomerId = customerAddress.CustomerId,
                                Address1 = customerAddress.Address1,
                                Address2 = customerAddress.Address2,
                                Address3 = customerAddress.Address3,
                                Address4 = customerAddress.Address4,
                                PinCode = customerAddress.PinCode,
                                CountryId = customerAddress.CountryId,
                                PhoneNo = customerAddress.PhoneNo,
                                FaxNo = customerAddress.FaxNo,
                                EmailAdd = customerAddress.EmailAdd,
                                WebUrl = customerAddress.WebUrl,
                                IsDefaultAdd = customerAddress.IsDefaultAdd,
                                IsDeleveryAdd = customerAddress.IsDeleveryAdd,
                                IsFinAdd = customerAddress.IsFinAdd,
                                IsSalesAdd = customerAddress.IsSalesAdd,
                                IsActive = customerAddress.IsActive,
                                CreateById = headerViewModel.UserId
                            };

                            var createdCustomerAddress = await _CustomerAddressService.AddCustomerAddressAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerAddressEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCustomerAddress);
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
                    "Error creating new CustomerAddress record");
            }
        }

        [HttpPut, Route("UpdateCustomerAddress/{AddressId}")]
        [Authorize]
        public async Task<ActionResult<CustomerAddressViewModel>> UpdateCustomerAddress(Int16 AddressId, [FromBody] CustomerAddressViewModel customerAddress, [FromHeader] HeaderViewModel headerViewModel)
        {
            var CustomerAddressViewModel = new CustomerAddressViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (AddressId != customerAddress.AddressId)
                                return StatusCode(StatusCodes.Status400BadRequest, "No input exist");

                            var CustomerAddressToUpdate = await _CustomerAddressService.GetCustomerAddressByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AddressId, headerViewModel.UserId);

                            if (CustomerAddressToUpdate == null)
                                return NotFound($"CustomerAddress with Id = {AddressId} not found");

                            var CustomerAddressEntity = new M_CustomerAddress
                            {
                                CustomerId = customerAddress.CustomerId,
                                AddressId = customerAddress.AddressId,
                                Address1 = customerAddress.Address1,
                                Address2 = customerAddress.Address2,
                                Address3 = customerAddress.Address3,
                                Address4 = customerAddress.Address4,
                                PinCode = customerAddress.PinCode,
                                CountryId = customerAddress.CountryId,
                                PhoneNo = customerAddress.PhoneNo,
                                FaxNo = customerAddress.FaxNo,
                                EmailAdd = customerAddress.EmailAdd,
                                WebUrl = customerAddress.WebUrl,
                                IsDefaultAdd = customerAddress.IsDefaultAdd,
                                IsDeleveryAdd = customerAddress.IsDeleveryAdd,
                                IsFinAdd = customerAddress.IsFinAdd,
                                IsSalesAdd = customerAddress.IsSalesAdd,
                                IsActive = customerAddress.IsActive,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponce = await _CustomerAddressService.UpdateCustomerAddressAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerAddressEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteCustomerAddress/{AddressId}")]
        [Authorize]
        public async Task<ActionResult<M_CustomerAddress>> DeleteCustomerAddress(Int16 AddressId, [FromHeader] HeaderViewModel headerViewModel)
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
                            var CustomerAddressToDelete = await _CustomerAddressService.GetCustomerAddressByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, AddressId, headerViewModel.UserId);

                            if (CustomerAddressToDelete == null)
                                return NotFound($"CustomerAddress with Id = {AddressId} not found");

                            var sqlResponce = await _CustomerAddressService.DeleteCustomerAddressAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerAddressToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"CustomerAddress_{AddressId}");
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