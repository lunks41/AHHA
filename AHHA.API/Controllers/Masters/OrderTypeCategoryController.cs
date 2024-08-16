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
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public OrderTypeCategoryController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<OrderTypeCategoryController> logger, IOrderTypeCategoryService OrderTypeCategoryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _OrderTypeCategoryService = OrderTypeCategoryService;
        }

        [HttpGet, Route("GetOrderTypeCategory")]
        [Authorize]
        public async Task<ActionResult> GetAllOrderTypeCategory()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.OrderTypeCategory, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<OrderTypeCategoryViewModelCount>("OrderTypeCategory");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _OrderTypeCategoryService.GetOrderTypeCategoryListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<OrderTypeCategoryViewModelCount>("OrderTypeCategory", cacheData, expirationTime);

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

        [HttpGet, Route("GetOrderTypeCategorybyid/{OrderTypeCategoryId}")]
        [Authorize]
        public async Task<ActionResult<OrderTypeCategoryViewModel>> GetOrderTypeCategoryById(Int16 OrderTypeCategoryId)
        {
            var OrderTypeCategoryViewModel = new OrderTypeCategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.OrderTypeCategory, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"OrderTypeCategory_{OrderTypeCategoryId}", out OrderTypeCategoryViewModel? cachedProduct))
                        {
                            OrderTypeCategoryViewModel = cachedProduct;
                        }
                        else
                        {
                            OrderTypeCategoryViewModel = _mapper.Map<OrderTypeCategoryViewModel>(await _OrderTypeCategoryService.GetOrderTypeCategoryByIdAsync(RegId,CompanyId, OrderTypeCategoryId, UserId));

                            if (OrderTypeCategoryViewModel == null)
                                return NotFound();
                            else
                                // Cache the OrderTypeCategory with an expiration time of 10 minutes
                                _memoryCache.Set($"OrderTypeCategory_{OrderTypeCategoryId}", OrderTypeCategoryViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, OrderTypeCategoryViewModel);
                        //return Ok(OrderTypeCategoryViewModel);
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

        [HttpPost, Route("AddOrderTypeCategory")]
        [Authorize]
        public async Task<ActionResult<OrderTypeCategoryViewModel>> CreateOrderTypeCategory(OrderTypeCategoryViewModel OrderTypeCategory)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.OrderTypeCategory, UserId);

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
                                CreateById = UserId,
                                IsActive = OrderTypeCategory.IsActive,
                                Remarks = OrderTypeCategory.Remarks
                            };

                            var createdOrderTypeCategory = await _OrderTypeCategoryService.AddOrderTypeCategoryAsync(RegId,CompanyId, OrderTypeCategoryEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdOrderTypeCategory);

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
                    "Error creating new OrderTypeCategory record");
            }
        }

        [HttpPut, Route("UpdateOrderTypeCategory/{OrderTypeCategoryId}")]
        [Authorize]
        public async Task<ActionResult<OrderTypeCategoryViewModel>> UpdateOrderTypeCategory(Int16 OrderTypeCategoryId, [FromBody] OrderTypeCategoryViewModel OrderTypeCategory)
        {
            var OrderTypeCategoryViewModel = new OrderTypeCategoryViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.OrderTypeCategory, UserId);

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
                                var OrderTypeCategoryToUpdate = await _OrderTypeCategoryService.GetOrderTypeCategoryByIdAsync(RegId,CompanyId, OrderTypeCategoryId, UserId);

                                if (OrderTypeCategoryToUpdate == null)
                                    return NotFound($"M_OrderTypeCategory with Id = {OrderTypeCategoryId} not found");
                            }

                            var OrderTypeCategoryEntity = new M_OrderTypeCategory
                            {
                                OrderTypeCategoryCode = OrderTypeCategory.OrderTypeCategoryCode,
                                OrderTypeCategoryId = OrderTypeCategory.OrderTypeCategoryId,
                                OrderTypeCategoryName = OrderTypeCategory.OrderTypeCategoryName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = OrderTypeCategory.IsActive,
                                Remarks = OrderTypeCategory.Remarks
                            };

                            var sqlResponce = await _OrderTypeCategoryService.UpdateOrderTypeCategoryAsync(RegId,CompanyId, OrderTypeCategoryEntity, UserId);
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

        //[HttpDelete, Route("Delete/{OrderTypeCategoryId}")]
        //[Authorize]
        //public async Task<ActionResult<M_OrderTypeCategory>> DeleteOrderTypeCategory(Int16 OrderTypeCategoryId)
        //{
        //    try
        //    {
        //        CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
        //        UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

        //        if (ValidateHeaders(RegId,CompanyId, UserId))
        //        {
        //            var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.OrderTypeCategory, UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var OrderTypeCategoryToDelete = await _OrderTypeCategoryService.GetOrderTypeCategoryByIdAsync(RegId,CompanyId, OrderTypeCategoryId, UserId);

        //                    if (OrderTypeCategoryToDelete == null)
        //                        return NotFound($"M_OrderTypeCategory with Id = {OrderTypeCategoryId} not found");

        //                    var sqlResponce = await _OrderTypeCategoryService.DeleteOrderTypeCategoryAsync(RegId,CompanyId, OrderTypeCategoryToDelete, UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"OrderTypeCategory_{OrderTypeCategoryId}");
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


