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
        
       
       
       
       
        
        public CustomerController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CustomerController> logger, ICustomerService CustomerService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CustomerService = CustomerService;
        }

        [HttpGet, Route("GetCustomer")]
        [Authorize]
        public async Task<ActionResult> GetAllCustomer([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                
                
                

                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                       
                           var cacheData = await _CustomerService.GetCustomerListAsync(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

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

        [HttpGet, Route("GetCustomerbyid/{CustomerId}")]
        [Authorize]
        public async Task<ActionResult<CustomerViewModel>> GetCustomerById(Int16 CustomerId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var CustomerViewModel = new CustomerViewModel();
            try
            {
                
                

                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Customer_{CustomerId}", out CustomerViewModel? cachedProduct))
                        {
                            CustomerViewModel = cachedProduct;
                        }
                        else
                        {
                            CustomerViewModel = _mapper.Map<CustomerViewModel>(await _CustomerService.GetCustomerByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, CustomerId, headerViewModel.UserId));

                            if (CustomerViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the Customer with an expiration time of 10 minutes
                                _memoryCache.Set($"Customer_{CustomerId}", CustomerViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, CustomerViewModel);
                        //return Ok(CustomerViewModel);
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
        public async Task<ActionResult<CustomerViewModel>> CreateCustomer(CustomerViewModel Customer, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                
                

                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

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
                                CreateById = headerViewModel.UserId,
                                IsActive = Customer.IsActive,
                                Remarks = Customer.Remarks
                            };

                            var createdCustomer = await _CustomerService.AddCustomerAsync(headerViewModel.RegId,headerViewModel.CompanyId, CustomerEntity, headerViewModel.UserId);
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
        public async Task<ActionResult<CustomerViewModel>> UpdateCustomer(Int16 CustomerId, [FromBody] CustomerViewModel Customer, [FromHeader] HeaderViewModel headerViewModel)
        {
            var CustomerViewModel = new CustomerViewModel();
            try
            {
                
                

                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

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
                                var CustomerToUpdate = await _CustomerService.GetCustomerByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, CustomerId, headerViewModel.UserId);

                                if (CustomerToUpdate == null)
                                    return NotFound($"M_Customer with Id = {CustomerId} not found");
                            }

                            var CustomerEntity = new M_Customer
                            {
                                CustomerCode = Customer.CustomerCode,
                                CustomerId = Customer.CustomerId,
                                CustomerName = Customer.CustomerName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = Customer.IsActive,
                                Remarks = Customer.Remarks
                            };

                            var sqlResponce = await _CustomerService.UpdateCustomerAsync(headerViewModel.RegId,headerViewModel.CompanyId, CustomerEntity, headerViewModel.UserId);
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

        //[HttpDelete, Route("Delete/{CustomerId}")]
        //[Authorize]
        //public async Task<ActionResult<M_Customer>> DeleteCustomer(Int16 CustomerId,[FromHeader] HeaderViewModel headerViewModel)
        //{
        //    try
        //    {
        //        
        //        

        //        if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
        //        {
        //            var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Customer, headerViewModel.UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var CustomerToDelete = await _CustomerService.GetCustomerByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, CustomerId, headerViewModel.UserId);

        //                    if (CustomerToDelete == null)
        //                        return NotFound($"M_Customer with Id = {CustomerId} not found");

        //                    var sqlResponce = await _CustomerService.DeleteCustomerAsync(headerViewModel.RegId,headerViewModel.CompanyId, CustomerToDelete, headerViewModel.UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"Customer_{CustomerId}");
        //                    return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
        //                }
        //                else
        //                {
        //                    return NotFound(GenrateMessage.authenticationfailed);
        //                }
        //            }
        //            else
        //            {
        //                return NotFound(GenrateMessage.authenticationfailed);
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


