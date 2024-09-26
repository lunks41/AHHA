﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Services.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/Master")]
    [ApiController]
    public class CustomerContactController : BaseController
    {
        private readonly ICustomerContactService _CustomerContactService;
        private readonly ILogger<CustomerContactController> _logger;

        public CustomerContactController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CustomerContactController> logger, ICustomerContactService CustomerContactService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CustomerContactService = CustomerContactService;
        }

        //Get the Customer Contact List
        [HttpGet, Route("getcustomercontactbycustomerid/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult> GetCustomerContactByCustomerId(Int16 CustomerId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var CustomerContactViewModel = await _CustomerContactService.GetCustomerContactByCustomerIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, headerViewModel.UserId);

                        if (CustomerContactViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, CustomerContactViewModel);
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

        //Customer Contact one record by using contactid
        [HttpGet, Route("getcustomercontactbyid/{CustomerId}/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<CustomerContactViewModel>> GetCustomerContactById(Int32 CustomerId, Int32 ContactId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var CustomerContactViewModel = await _CustomerContactService.GetCustomerContactByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, ContactId, headerViewModel.UserId);

                        if (CustomerContactViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, CustomerContactViewModel);
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

        [HttpPost, Route("SaveCustomerContact")]
        [Authorize]
        public async Task<ActionResult<CustomerContactViewModel>> SaveCustomerContact(CustomerContactViewModel customerContactViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (customerContactViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var CustomerContactEntity = new M_CustomerContact
                            {
                                CustomerId = customerContactViewModel.CustomerId,
                                ContactId = customerContactViewModel.ContactId,
                                ContactName = customerContactViewModel.ContactName == null ? string.Empty : customerContactViewModel.ContactName,
                                OtherName = customerContactViewModel.OtherName == null ? string.Empty : customerContactViewModel.OtherName,
                                OffNo = customerContactViewModel.OffNo == null ? string.Empty : customerContactViewModel.OffNo,
                                FaxNo = customerContactViewModel.FaxNo == null ? string.Empty : customerContactViewModel.FaxNo,
                                EmailAdd = customerContactViewModel.EmailAdd == null ? string.Empty : customerContactViewModel.EmailAdd,
                                MessId = customerContactViewModel.MessId == null ? string.Empty : customerContactViewModel.MessId,
                                ContactMessType = customerContactViewModel.ContactMessType == null ? string.Empty : customerContactViewModel.ContactMessType,
                                IsDefault = customerContactViewModel.IsDefault,
                                IsFinance = customerContactViewModel.IsFinance,
                                IsSales = customerContactViewModel.IsSales,
                                IsActive = customerContactViewModel.IsActive,
                                MobileNo = customerContactViewModel.MobileNo,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponce = await _CustomerContactService.SaveCustomerContactAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerContactEntity, headerViewModel.UserId);

                            if (sqlResponce.Result > 0)
                            {
                                var customerModel = await _CustomerContactService.GetCustomerContactByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, customerContactViewModel.CustomerId, Convert.ToInt32(sqlResponce.Result), headerViewModel.UserId);

                                return StatusCode(StatusCodes.Status204NoContent, customerModel);
                            }

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
                    "Error creating new Customer record");
            }
        }

        [HttpDelete, Route("DeleteCustomerContact/{CustomerId}/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<M_CustomerContact>> DeleteCustomerContact(Int32 CustomerId, Int32 ContactId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var sqlResponce = await _CustomerContactService.DeleteCustomerContactAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, ContactId, headerViewModel.UserId);

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