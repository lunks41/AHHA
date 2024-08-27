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

        [HttpGet, Route("GetCustomerContact")]
        [Authorize]
        public async Task<ActionResult> GetCustomerContact([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _CustomerContactService.GetCustomerContactListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetCustomerContactbyCustomerId/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult> GetCustomerContactByCustomerId(Int16 CustomerId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var CustomerContactViewModel = await _CustomerContactService.GetCustomerContactByCustomerIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerId, headerViewModel.UserId);

                        if (CustomerContactViewModel == null)
                            return NotFound(GenrateMessage.authenticationfailed);

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

        [HttpGet, Route("GetCustomerContactbyid/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<CustomerContactViewModel>> GetCustomerContactById(Int16 ContactId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var CustomerContactViewModel = _mapper.Map<CustomerContactViewModel>(await _CustomerContactService.GetCustomerContactByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, ContactId, headerViewModel.UserId));

                        if (CustomerContactViewModel == null)
                            return NotFound(GenrateMessage.authenticationfailed);

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

        [HttpPost, Route("AddCustomerContact")]
        [Authorize]
        public async Task<ActionResult<CustomerContactViewModel>> CreateCustomerContact(CustomerContactViewModel customerContact, [FromHeader] HeaderViewModel headerViewModel)
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
                            if (customerContact == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "No input exist");

                            var CustomerContactEntity = new M_CustomerContact
                            {
                                CustomerId = customerContact.CustomerId,
                                ContactName = customerContact.ContactName,
                                OtherName = customerContact.OtherName,
                                OffNo = customerContact.OffNo,
                                FaxNo = customerContact.FaxNo,
                                EmailAdd = customerContact.EmailAdd,
                                MessId = customerContact.MessId,
                                ContactMessType = customerContact.ContactMessType,
                                IsDefault = customerContact.IsDefault,
                                IsFinance = customerContact.IsFinance,
                                IsSales = customerContact.IsSales,
                                IsActive = customerContact.IsActive,
                                MobileNo = customerContact.MobileNo,
                                CreateById = headerViewModel.UserId
                            };

                            var createdCustomerContact = await _CustomerContactService.AddCustomerContactAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerContactEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCustomerContact);
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
                    "Error creating new CustomerContact record");
            }
        }

        [HttpPut, Route("UpdateCustomerContact/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<CustomerContactViewModel>> UpdateCustomerContact(Int16 ContactId, [FromBody] CustomerContactViewModel customerContact, [FromHeader] HeaderViewModel headerViewModel)
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
                            if (ContactId != customerContact.ContactId)
                                return StatusCode(StatusCodes.Status400BadRequest, "CustomerContact ID mismatch");

                            var CustomerContactToUpdate = await _CustomerContactService.GetCustomerContactByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, ContactId, headerViewModel.UserId);

                            if (CustomerContactToUpdate == null)
                                return NotFound($"CustomerContact with Id = {ContactId} not found");

                            var CustomerContactEntity = new M_CustomerContact
                            {
                                ContactId = customerContact.ContactId,
                                CustomerId = customerContact.CustomerId,
                                ContactName = customerContact.ContactName,
                                OtherName = customerContact.OtherName,
                                OffNo = customerContact.OffNo,
                                FaxNo = customerContact.FaxNo,
                                EmailAdd = customerContact.EmailAdd,
                                MessId = customerContact.MessId,
                                ContactMessType = customerContact.ContactMessType,
                                IsDefault = customerContact.IsDefault,
                                IsFinance = customerContact.IsFinance,
                                IsSales = customerContact.IsSales,
                                IsActive = customerContact.IsActive,
                                MobileNo = customerContact.MobileNo,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponce = await _CustomerContactService.UpdateCustomerContactAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerContactEntity, headerViewModel.UserId);

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

        [HttpDelete, Route("DeleteCustomerContact/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<M_CustomerContact>> DeleteCustomerContact(Int16 ContactId, [FromHeader] HeaderViewModel headerViewModel)
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
                            var CustomerContactToDelete = await _CustomerContactService.GetCustomerContactByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, ContactId, headerViewModel.UserId);

                            if (CustomerContactToDelete == null)
                                return NotFound($"CustomerContact with Id = {ContactId} not found");

                            var sqlResponce = await _CustomerContactService.DeleteCustomerContactAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomerContactToDelete, headerViewModel.UserId);

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