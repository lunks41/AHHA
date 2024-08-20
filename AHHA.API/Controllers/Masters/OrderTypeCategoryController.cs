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
    public class OrderTypeCategoryController : BaseController
    {
        private readonly IOrderTypeCategoryService _OrderTypeCategoryService;
        private readonly ILogger<OrderTypeCategoryController> _logger;
        
       
       
       
       
        

        public OrderTypeCategoryController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<OrderTypeCategoryController> logger, IOrderTypeCategoryService OrderTypeCategoryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _OrderTypeCategoryService = OrderTypeCategoryService;
        }

        [HttpGet, Route("GetOrderTypeCategory")]
        [Authorize]
        public async Task<ActionResult> GetAllOrderTypeCategory([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                
                
                

                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.OrderTypeCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                       
                        
                        
                       
                            var cacheData = await _OrderTypeCategoryService.GetOrderTypeCategoryListAsync(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

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

        [HttpGet, Route("GetOrderTypeCategorybyid/{OrderTypeCategoryId}")]
        [Authorize]
        public async Task<ActionResult<OrderTypeCategoryViewModel>> GetOrderTypeCategoryById(Int16 OrderTypeCategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var OrderTypeCategoryViewModel = new OrderTypeCategoryViewModel();
            try
            {
                
                

                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.OrderTypeCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"OrderTypeCategory_{OrderTypeCategoryId}", out OrderTypeCategoryViewModel? cachedProduct))
                        {
                            OrderTypeCategoryViewModel = cachedProduct;
                        }
                        else
                        {
                            OrderTypeCategoryViewModel = _mapper.Map<OrderTypeCategoryViewModel>(await _OrderTypeCategoryService.GetOrderTypeCategoryByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, OrderTypeCategoryId, headerViewModel.UserId));

                            if (OrderTypeCategoryViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the OrderTypeCategory with an expiration time of 10 minutes
                                _memoryCache.Set($"OrderTypeCategory_{OrderTypeCategoryId}", OrderTypeCategoryViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, OrderTypeCategoryViewModel);
                        //return Ok(OrderTypeCategoryViewModel);
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

        [HttpPost, Route("AddOrderTypeCategory")]
        [Authorize]
        public async Task<ActionResult<OrderTypeCategoryViewModel>> CreateOrderTypeCategory(OrderTypeCategoryViewModel OrderTypeCategory, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                
                

                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.OrderTypeCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (OrderTypeCategory == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_OrderTypeCategory ID mismatch");

                            var OrderTypeCategoryEntity = new M_OrderTypeCategory
                            {
                                CompanyId = OrderTypeCategory.CompanyId,
                                OrderTypeCategoryCode = OrderTypeCategory.OrderTypeCategoryCode,
                                OrderTypeCategoryId = OrderTypeCategory.OrderTypeCategoryId,
                                OrderTypeCategoryName = OrderTypeCategory.OrderTypeCategoryName,
                                CreateById = headerViewModel.UserId,
                                IsActive = OrderTypeCategory.IsActive,
                                Remarks = OrderTypeCategory.Remarks
                            };

                            var createdOrderTypeCategory = await _OrderTypeCategoryService.AddOrderTypeCategoryAsync(headerViewModel.RegId,headerViewModel.CompanyId, OrderTypeCategoryEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdOrderTypeCategory);

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
                    "Error creating new OrderTypeCategory record");
            }
        }

        [HttpPut, Route("UpdateOrderTypeCategory/{OrderTypeCategoryId}")]
        [Authorize]
        public async Task<ActionResult<OrderTypeCategoryViewModel>> UpdateOrderTypeCategory(Int16 OrderTypeCategoryId, [FromBody] OrderTypeCategoryViewModel OrderTypeCategory, [FromHeader] HeaderViewModel headerViewModel)
        {
            var OrderTypeCategoryViewModel = new OrderTypeCategoryViewModel();
            try
            {
                
                

                if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.OrderTypeCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (OrderTypeCategoryId != OrderTypeCategory.OrderTypeCategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_OrderTypeCategory ID mismatch");
                            //return BadRequest("M_OrderTypeCategory ID mismatch");

                            // Attempt to retrieve the OrderTypeCategory from the cache
                            if (_memoryCache.TryGetValue($"OrderTypeCategory_{OrderTypeCategoryId}", out OrderTypeCategoryViewModel? cachedProduct))
                            {
                                OrderTypeCategoryViewModel = cachedProduct;
                            }
                            else
                            {
                                var OrderTypeCategoryToUpdate = await _OrderTypeCategoryService.GetOrderTypeCategoryByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, OrderTypeCategoryId, headerViewModel.UserId);

                                if (OrderTypeCategoryToUpdate == null)
                                    return NotFound($"M_OrderTypeCategory with Id = {OrderTypeCategoryId} not found");
                            }

                            var OrderTypeCategoryEntity = new M_OrderTypeCategory
                            {
                                OrderTypeCategoryCode = OrderTypeCategory.OrderTypeCategoryCode,
                                OrderTypeCategoryId = OrderTypeCategory.OrderTypeCategoryId,
                                OrderTypeCategoryName = OrderTypeCategory.OrderTypeCategoryName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = OrderTypeCategory.IsActive,
                                Remarks = OrderTypeCategory.Remarks
                            };

                            var sqlResponce = await _OrderTypeCategoryService.UpdateOrderTypeCategoryAsync(headerViewModel.RegId,headerViewModel.CompanyId, OrderTypeCategoryEntity, headerViewModel.UserId);
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

        //[HttpDelete, Route("Delete/{OrderTypeCategoryId}")]
        //[Authorize]
        //public async Task<ActionResult<M_OrderTypeCategory>> DeleteOrderTypeCategory(Int16 OrderTypeCategoryId)
        //{
        //    try
        //    {
        //        
        //        

        //        if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
        //        {
        //            var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.OrderTypeCategory, headerViewModel.UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var OrderTypeCategoryToDelete = await _OrderTypeCategoryService.GetOrderTypeCategoryByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, OrderTypeCategoryId, headerViewModel.UserId);

        //                    if (OrderTypeCategoryToDelete == null)
        //                        return NotFound($"M_OrderTypeCategory with Id = {OrderTypeCategoryId} not found");

        //                    var sqlResponce = await _OrderTypeCategoryService.DeleteOrderTypeCategoryAsync(headerViewModel.RegId,headerViewModel.CompanyId, OrderTypeCategoryToDelete, headerViewModel.UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"OrderTypeCategory_{OrderTypeCategoryId}");
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


