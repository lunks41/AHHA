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
    public class OrderTypeController : BaseController
    {
        private readonly IOrderTypeService _OrderTypeService;
        private readonly ILogger<OrderTypeController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public OrderTypeController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<OrderTypeController> logger, IOrderTypeService OrderTypeService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _OrderTypeService = OrderTypeService;
        }

        [HttpGet, Route("GetOrderType")]
        [Authorize]
        public async Task<ActionResult> GetAllOrderType()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.OrderType, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<OrderTypeViewModelCount>("OrderType");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _OrderTypeService.GetOrderTypeListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<OrderTypeViewModelCount>("OrderType", cacheData, expirationTime);

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

        [HttpGet, Route("GetOrderTypebyid/{OrderTypeId}")]
        [Authorize]
        public async Task<ActionResult<OrderTypeViewModel>> GetOrderTypeById(Int16 OrderTypeId)
        {
            var OrderTypeViewModel = new OrderTypeViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.OrderType, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"OrderType_{OrderTypeId}", out OrderTypeViewModel? cachedProduct))
                        {
                            OrderTypeViewModel = cachedProduct;
                        }
                        else
                        {
                            OrderTypeViewModel = _mapper.Map<OrderTypeViewModel>(await _OrderTypeService.GetOrderTypeByIdAsync(RegId,CompanyId, OrderTypeId, UserId));

                            if (OrderTypeViewModel == null)
                                return NotFound();
                            else
                                // Cache the OrderType with an expiration time of 10 minutes
                                _memoryCache.Set($"OrderType_{OrderTypeId}", OrderTypeViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, OrderTypeViewModel);
                        //return Ok(OrderTypeViewModel);
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

        [HttpPost, Route("AddOrderType")]
        [Authorize]
        public async Task<ActionResult<OrderTypeViewModel>> CreateOrderType(OrderTypeViewModel OrderType)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.OrderType, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (OrderType == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_OrderType ID mismatch");

                            var OrderTypeEntity = new M_OrderType
                            {
                                CompanyId = OrderType.CompanyId,
                                OrderTypeCode = OrderType.OrderTypeCode,
                                OrderTypeId = OrderType.OrderTypeId,
                                OrderTypeName = OrderType.OrderTypeName,
                                CreateById = UserId,
                                IsActive = OrderType.IsActive,
                                Remarks = OrderType.Remarks
                            };

                            var createdOrderType = await _OrderTypeService.AddOrderTypeAsync(RegId,CompanyId, OrderTypeEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdOrderType);

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
                    "Error creating new OrderType record");
            }
        }

        [HttpPut, Route("UpdateOrderType/{OrderTypeId}")]
        [Authorize]
        public async Task<ActionResult<OrderTypeViewModel>> UpdateOrderType(Int16 OrderTypeId, [FromBody] OrderTypeViewModel OrderType)
        {
            var OrderTypeViewModel = new OrderTypeViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.OrderType, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (OrderTypeId != OrderType.OrderTypeId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_OrderType ID mismatch");
                            //return BadRequest("M_OrderType ID mismatch");

                            // Attempt to retrieve the OrderType from the cache
                            if (_memoryCache.TryGetValue($"OrderType_{OrderTypeId}", out OrderTypeViewModel? cachedProduct))
                            {
                                OrderTypeViewModel = cachedProduct;
                            }
                            else
                            {
                                var OrderTypeToUpdate = await _OrderTypeService.GetOrderTypeByIdAsync(RegId,CompanyId, OrderTypeId, UserId);

                                if (OrderTypeToUpdate == null)
                                    return NotFound($"M_OrderType with Id = {OrderTypeId} not found");
                            }

                            var OrderTypeEntity = new M_OrderType
                            {
                                OrderTypeCode = OrderType.OrderTypeCode,
                                OrderTypeId = OrderType.OrderTypeId,
                                OrderTypeName = OrderType.OrderTypeName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = OrderType.IsActive,
                                Remarks = OrderType.Remarks
                            };

                            var sqlResponce = await _OrderTypeService.UpdateOrderTypeAsync(RegId,CompanyId, OrderTypeEntity, UserId);
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

        //[HttpDelete, Route("Delete/{OrderTypeId}")]
        //[Authorize]
        //public async Task<ActionResult<M_OrderType>> DeleteOrderType(Int16 OrderTypeId)
        //{
        //    try
        //    {
        //        CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
        //        UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

        //        if (ValidateHeaders(RegId,CompanyId, UserId))
        //        {
        //            var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.OrderType, UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var OrderTypeToDelete = await _OrderTypeService.GetOrderTypeByIdAsync(RegId,CompanyId, OrderTypeId, UserId);

        //                    if (OrderTypeToDelete == null)
        //                        return NotFound($"M_OrderType with Id = {OrderTypeId} not found");

        //                    var sqlResponce = await _OrderTypeService.DeleteOrderTypeAsync(RegId,CompanyId, OrderTypeToDelete, UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"OrderType_{OrderTypeId}");
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


