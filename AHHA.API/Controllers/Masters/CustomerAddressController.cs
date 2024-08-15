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
    public class CustomerAddressController : BaseController
    {
        private readonly ICustomerAddressService _CustomerAddressService;
        private readonly ILogger<CustomerAddressController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public CustomerAddressController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CustomerAddressController> logger, ICustomerAddressService CustomerAddressService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CustomerAddressService = CustomerAddressService;
        }

        [HttpGet, Route("GetCustomerAddress")]
        [Authorize]
        public async Task<ActionResult> GetAllCustomerAddress()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<CustomerAddressViewModelCount>("CustomerAddress");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _CustomerAddressService.GetCustomerAddressListAsync(CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<CustomerAddressViewModelCount>("CustomerAddress", cacheData, expirationTime);

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

        [HttpGet, Route("GetCustomerAddressbyid/{AddressId}")]
        [Authorize]
        public async Task<ActionResult<CustomerAddressViewModel>> GetCustomerAddressById(Int16 AddressId)
        {
            var CustomerAddressViewModel = new CustomerAddressViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"CustomerAddress_{AddressId}", out CustomerAddressViewModel? cachedProduct))
                        {
                            CustomerAddressViewModel = cachedProduct;
                        }
                        else
                        {
                            CustomerAddressViewModel = _mapper.Map<CustomerAddressViewModel>(await _CustomerAddressService.GetCustomerAddressByIdAsync(CompanyId, AddressId, UserId));

                            if (CustomerAddressViewModel == null)
                                return NotFound();
                            else
                                // Cache the CustomerAddress with an expiration time of 10 minutes
                                _memoryCache.Set($"CustomerAddress_{AddressId}", CustomerAddressViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, CustomerAddressViewModel);
                        //return Ok(CustomerAddressViewModel);
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

        [HttpPost, Route("AddCustomerAddress")]
        [Authorize]
        public async Task<ActionResult<CustomerAddressViewModel>> CreateCustomerAddress(CustomerAddressViewModel CustomerAddress)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (CustomerAddress == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_CustomerAddress ID mismatch");

                            var CustomerAddressEntity = new M_CustomerAddress
                            {
                                CustomerId = CustomerAddress.CustomerId,
                                AddressId = CustomerAddress.AddressId,
                                Address1 = CustomerAddress.Address1,
                                Address2 = CustomerAddress.Address2,
                                Address3 = CustomerAddress.Address3,
                                Address4 = CustomerAddress.Address4,
                                PinCode = CustomerAddress.PinCode,
                                CountryId = CustomerAddress.CountryId,
                                PhoneNo = CustomerAddress.PhoneNo,
                                FaxNo = CustomerAddress.FaxNo,
                                EmailAdd = CustomerAddress.EmailAdd,
                                WebUrl = CustomerAddress.WebUrl,
                                IsDefaultAdd = CustomerAddress.IsDefaultAdd,
                                IsDeleveryAdd = CustomerAddress.IsDeleveryAdd,
                                IsFinAdd = CustomerAddress.IsFinAdd,
                                IsSalesAdd = CustomerAddress.IsSalesAdd,
                                CreateById = UserId,
                                IsActive = CustomerAddress.IsActive,
                            };

                            var createdCustomerAddress = await _CustomerAddressService.AddCustomerAddressAsync(CompanyId, CustomerAddressEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCustomerAddress);

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
                    "Error creating new CustomerAddress record");
            }
        }

        [HttpPut, Route("UpdateCustomerAddress/{AddressId}")]
        [Authorize]
        public async Task<ActionResult<CustomerAddressViewModel>> UpdateCustomerAddress(Int16 AddressId, [FromBody] CustomerAddressViewModel CustomerAddress)
        {
            var CustomerAddressViewModel = new CustomerAddressViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (AddressId != CustomerAddress.AddressId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_CustomerAddress ID mismatch");
                            //return BadRequest("M_CustomerAddress ID mismatch");

                            // Attempt to retrieve the CustomerAddress from the cache
                            if (_memoryCache.TryGetValue($"CustomerAddress_{AddressId}", out CustomerAddressViewModel? cachedProduct))
                            {
                                CustomerAddressViewModel = cachedProduct;
                            }
                            else
                            {
                                var CustomerAddressToUpdate = await _CustomerAddressService.GetCustomerAddressByIdAsync(CompanyId, AddressId, UserId);

                                if (CustomerAddressToUpdate == null)
                                    return NotFound($"M_CustomerAddress with Id = {AddressId} not found");
                            }

                            var CustomerAddressEntity = new M_CustomerAddress
                            {
                                CustomerId = CustomerAddress.CustomerId,
                                AddressId = CustomerAddress.AddressId,
                                Address1 = CustomerAddress.Address1,
                                Address2 = CustomerAddress.Address2,
                                Address3 = CustomerAddress.Address3,
                                Address4 = CustomerAddress.Address4,
                                PinCode = CustomerAddress.PinCode,
                                CountryId = CustomerAddress.CountryId,
                                PhoneNo = CustomerAddress.PhoneNo,
                                FaxNo = CustomerAddress.FaxNo,
                                EmailAdd = CustomerAddress.EmailAdd,
                                WebUrl = CustomerAddress.WebUrl,
                                IsDefaultAdd = CustomerAddress.IsDefaultAdd,
                                IsDeleveryAdd = CustomerAddress.IsDeleveryAdd,
                                IsFinAdd = CustomerAddress.IsFinAdd,
                                IsSalesAdd = CustomerAddress.IsSalesAdd,
                                CreateById = UserId,
                                IsActive = CustomerAddress.IsActive,
                            };

                            var sqlResponce = await _CustomerAddressService.UpdateCustomerAddressAsync(CompanyId, CustomerAddressEntity, UserId);
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

        //[HttpDelete, Route("Delete/{AddressId}")]
        //[Authorize]
        //public async Task<ActionResult<M_CustomerAddress>> DeleteCustomerAddress(Int16 AddressId)
        //{
        //    try
        //    {
        //        CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
        //        UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

        //        if (ValidateHeaders(CompanyId, UserId))
        //        {
        //            var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var CustomerAddressToDelete = await _CustomerAddressService.GetCustomerAddressByIdAsync(CompanyId, AddressId, UserId);

        //                    if (CustomerAddressToDelete == null)
        //                        return NotFound($"M_CustomerAddress with Id = {AddressId} not found");

        //                    var sqlResponce = await _CustomerAddressService.DeleteCustomerAddressAsync(CompanyId, CustomerAddressToDelete, UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"CustomerAddress_{AddressId}");
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

