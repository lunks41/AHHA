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
    public class CustomerContactController : BaseController
    {
        private readonly ICustomerContactService _CustomerContactService;
        private readonly ILogger<CustomerContactController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public CustomerContactController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CustomerContactController> logger, ICustomerContactService CustomerContactService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CustomerContactService = CustomerContactService;
        }

        [HttpGet, Route("GetCustomerContact")]
        [Authorize]
        public async Task<ActionResult> GetAllCustomerContact()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<CustomerContactViewModelCount>("CustomerContact");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _CustomerContactService.GetCustomerContactListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<CustomerContactViewModelCount>("CustomerContact", cacheData, expirationTime);

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

        [HttpGet, Route("GetCustomerContactbyid/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<CustomerContactViewModel>> GetCustomerContactById(Int16 ContactId)
        {
            var CustomerContactViewModel = new CustomerContactViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"CustomerContact_{ContactId}", out CustomerContactViewModel? cachedProduct))
                        {
                            CustomerContactViewModel = cachedProduct;
                        }
                        else
                        {
                            CustomerContactViewModel = _mapper.Map<CustomerContactViewModel>(await _CustomerContactService.GetCustomerContactByIdAsync(RegId,CompanyId, ContactId, UserId));

                            if (CustomerContactViewModel == null)
                                return NotFound();
                            else
                                // Cache the CustomerContact with an expiration time of 10 minutes
                                _memoryCache.Set($"CustomerContact_{ContactId}", CustomerContactViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, CustomerContactViewModel);
                        //return Ok(CustomerContactViewModel);
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

        [HttpPost, Route("AddCustomerContact")]
        [Authorize]
        public async Task<ActionResult<CustomerContactViewModel>> CreateCustomerContact(CustomerContactViewModel CustomerContact)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (CustomerContact == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_CustomerContact ID mismatch");

                            var CustomerContactEntity = new M_CustomerContact
                            {
                                ContactId = CustomerContact.ContactId,
                                CustomerId = CustomerContact.CustomerId,
                                ContactName = CustomerContact.ContactName,
                                OtherName = CustomerContact.OtherName,
                                OffNo = CustomerContact.OffNo,
                                FaxNo = CustomerContact.FaxNo,
                                EmailAdd = CustomerContact.EmailAdd,
                                MessId = CustomerContact.MessId,
                                ContactMessType = CustomerContact.ContactMessType,
                                IsDefault = CustomerContact.IsDefault,
                                IsFinance = CustomerContact.IsFinance,
                                IsSales = CustomerContact.IsSales,
                                CreateById = UserId,
                                IsActive = CustomerContact.IsActive,
                                MobileNo = CustomerContact.MobileNo
                            };

                            var createdCustomerContact = await _CustomerContactService.AddCustomerContactAsync(RegId,CompanyId, CustomerContactEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCustomerContact);

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
                    "Error creating new CustomerContact record");
            }
        }

        [HttpPut, Route("UpdateCustomerContact/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<CustomerContactViewModel>> UpdateCustomerContact(Int16 ContactId, [FromBody] CustomerContactViewModel CustomerContact)
        {
            var CustomerContactViewModel = new CustomerContactViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (ContactId != CustomerContact.ContactId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_CustomerContact ID mismatch");
                            //return BadRequest("M_CustomerContact ID mismatch");

                            // Attempt to retrieve the CustomerContact from the cache
                            if (_memoryCache.TryGetValue($"CustomerContact_{ContactId}", out CustomerContactViewModel? cachedProduct))
                            {
                                CustomerContactViewModel = cachedProduct;
                            }
                            else
                            {
                                var CustomerContactToUpdate = await _CustomerContactService.GetCustomerContactByIdAsync(RegId,CompanyId, ContactId, UserId);

                                if (CustomerContactToUpdate == null)
                                    return NotFound($"M_CustomerContact with Id = {ContactId} not found");
                            }

                            var CustomerContactEntity = new M_CustomerContact
                            {
                                ContactId = CustomerContact.ContactId,
                                CustomerId = CustomerContact.CustomerId,
                                ContactName = CustomerContact.ContactName,
                                OtherName = CustomerContact.OtherName,
                                OffNo = CustomerContact.OffNo,
                                FaxNo = CustomerContact.FaxNo,
                                EmailAdd = CustomerContact.EmailAdd,
                                MessId = CustomerContact.MessId,
                                ContactMessType = CustomerContact.ContactMessType,
                                IsDefault = CustomerContact.IsDefault,
                                IsFinance = CustomerContact.IsFinance,
                                IsSales = CustomerContact.IsSales,
                                CreateById = UserId,
                                IsActive = CustomerContact.IsActive,
                                MobileNo = CustomerContact.MobileNo
                            };

                            var sqlResponce = await _CustomerContactService.UpdateCustomerContactAsync(RegId,CompanyId, CustomerContactEntity, UserId);
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

        //[HttpDelete, Route("Delete/{ContactId}")]
        //[Authorize]
        //public async Task<ActionResult<M_CustomerContact>> DeleteCustomerContact(Int16 ContactId)
        //{
        //    try
        //    {
        //        CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
        //        UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

        //        if (ValidateHeaders(RegId,CompanyId, UserId))
        //        {
        //            var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var CustomerContactToDelete = await _CustomerContactService.GetCustomerContactByIdAsync(RegId,CompanyId, ContactId, UserId);

        //                    if (CustomerContactToDelete == null)
        //                        return NotFound($"M_CustomerContact with Id = {ContactId} not found");

        //                    var sqlResponce = await _CustomerContactService.DeleteCustomerContactAsync(RegId,CompanyId, CustomerContactToDelete, UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"CustomerContact_{ContactId}");
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

