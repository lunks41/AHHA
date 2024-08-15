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
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _CustomerService;
        private readonly ILogger<CustomerController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;
        public CustomerController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CustomerController> logger, ICustomerService CustomerService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CustomerService = CustomerService;
        }

        [HttpGet, Route("GetCustomer")]
        [Authorize]
        public async Task<ActionResult> GetAllCustomer()
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
                        var cacheData = _memoryCache.Get<CustomerViewModelCount>("Customer");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _CustomerService.GetCustomerListAsync(CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<CustomerViewModelCount>("Customer", cacheData, expirationTime);

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

        [HttpGet, Route("GetCustomerbyid/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult<CustomerViewModel>> GetCustomerById(Int16 CustomerId)
        {
            var CustomerViewModel = new CustomerViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Customer_{CustomerId}", out CustomerViewModel? cachedProduct))
                        {
                            CustomerViewModel = cachedProduct;
                        }
                        else
                        {
                            CustomerViewModel = _mapper.Map<CustomerViewModel>(await _CustomerService.GetCustomerByIdAsync(CompanyId, CustomerId, UserId));

                            if (CustomerViewModel == null)
                                return NotFound();
                            else
                                // Cache the Customer with an expiration time of 10 minutes
                                _memoryCache.Set($"Customer_{CustomerId}", CustomerViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, CustomerViewModel);
                        //return Ok(CustomerViewModel);
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

        [HttpPost, Route("AddCustomer")]
        [Authorize]
        public async Task<ActionResult<CustomerViewModel>> CreateCustomer(CustomerViewModel Customer)
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
                            if (Customer == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Customer ID mismatch");

                            var CustomerEntity = new M_Customer
                            {
                                CompanyId = Customer.CompanyId,
                                CustomerCode = Customer.CustomerCode,
                                CustomerId = Customer.CustomerId,
                                CustomerName = Customer.CustomerName,
                                CreateById = UserId,
                                IsActive = Customer.IsActive,
                                Remarks = Customer.Remarks
                            };

                            var createdCustomer = await _CustomerService.AddCustomerAsync(CompanyId, CustomerEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCustomer);

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
                    "Error creating new Customer record");
            }
        }

        [HttpPut, Route("UpdateCustomer/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult<CustomerViewModel>> UpdateCustomer(Int16 CustomerId, [FromBody] CustomerViewModel Customer)
        {
            var CustomerViewModel = new CustomerViewModel();
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
                            if (CustomerId != Customer.CustomerId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Customer ID mismatch");
                            //return BadRequest("M_Customer ID mismatch");

                            // Attempt to retrieve the Customer from the cache
                            if (_memoryCache.TryGetValue($"Customer_{CustomerId}", out CustomerViewModel? cachedProduct))
                            {
                                CustomerViewModel = cachedProduct;
                            }
                            else
                            {
                                var CustomerToUpdate = await _CustomerService.GetCustomerByIdAsync(CompanyId, CustomerId, UserId);

                                if (CustomerToUpdate == null)
                                    return NotFound($"M_Customer with Id = {CustomerId} not found");
                            }

                            var CustomerEntity = new M_Customer
                            {
                                CustomerCode = Customer.CustomerCode,
                                CustomerId = Customer.CustomerId,
                                CustomerName = Customer.CustomerName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = Customer.IsActive,
                                Remarks = Customer.Remarks
                            };

                            var sqlResponce = await _CustomerService.UpdateCustomerAsync(CompanyId, CustomerEntity, UserId);
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
        //public async Task<ActionResult<M_Customer>> DeleteCustomer(Int16 CustomerId)
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
        //                    var CustomerToDelete = await _CustomerService.GetCustomerByIdAsync(CompanyId, CustomerId, UserId);

        //                    if (CustomerToDelete == null)
        //                        return NotFound($"M_Customer with Id = {CustomerId} not found");

        //                    var sqlResponce = await _CustomerService.DeleteCustomerAsync(CompanyId, CustomerToDelete, UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"Customer_{CustomerId}");
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


