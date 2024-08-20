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
    public class OrderTypeController : BaseController
    {
        private readonly IOrderTypeService _OrderTypeService;
        private readonly ILogger<OrderTypeController> _logger;

        public OrderTypeController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<OrderTypeController> logger, IOrderTypeService OrderTypeService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _OrderTypeService = OrderTypeService;
        }

        [HttpGet, Route("GetOrderType")]
        [Authorize]
        public async Task<ActionResult> GetAllOrderType([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.OrderType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _OrderTypeService.GetOrderTypeListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

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

        [HttpGet, Route("GetOrderTypebyid/{OrderTypeId}")]
        [Authorize]
        public async Task<ActionResult<OrderTypeViewModel>> GetOrderTypeById(Int16 OrderTypeId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var OrderTypeViewModel = new OrderTypeViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.OrderType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"OrderType_{OrderTypeId}", out OrderTypeViewModel? cachedProduct))
                        {
                            OrderTypeViewModel = cachedProduct;
                        }
                        else
                        {
                            OrderTypeViewModel = _mapper.Map<OrderTypeViewModel>(await _OrderTypeService.GetOrderTypeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, OrderTypeId, headerViewModel.UserId));

                            if (OrderTypeViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the OrderType with an expiration time of 10 minutes
                                _memoryCache.Set($"OrderType_{OrderTypeId}", OrderTypeViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, OrderTypeViewModel);
                        //return Ok(OrderTypeViewModel);
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

        [HttpPost, Route("AddOrderType")]
        [Authorize]
        public async Task<ActionResult<OrderTypeViewModel>> CreateOrderType(OrderTypeViewModel OrderType, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.OrderType, headerViewModel.UserId);

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
                                CreateById = headerViewModel.UserId,
                                IsActive = OrderType.IsActive,
                                Remarks = OrderType.Remarks
                            };

                            var createdOrderType = await _OrderTypeService.AddOrderTypeAsync(headerViewModel.RegId, headerViewModel.CompanyId, OrderTypeEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdOrderType);
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
                    "Error creating new OrderType record");
            }
        }

        [HttpPut, Route("UpdateOrderType/{OrderTypeId}")]
        [Authorize]
        public async Task<ActionResult<OrderTypeViewModel>> UpdateOrderType(Int16 OrderTypeId, [FromBody] OrderTypeViewModel OrderType, [FromHeader] HeaderViewModel headerViewModel)
        {
            var OrderTypeViewModel = new OrderTypeViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.OrderType, headerViewModel.UserId);

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
                                var OrderTypeToUpdate = await _OrderTypeService.GetOrderTypeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, OrderTypeId, headerViewModel.UserId);

                                if (OrderTypeToUpdate == null)
                                    return NotFound($"M_OrderType with Id = {OrderTypeId} not found");
                            }

                            var OrderTypeEntity = new M_OrderType
                            {
                                OrderTypeCode = OrderType.OrderTypeCode,
                                OrderTypeId = OrderType.OrderTypeId,
                                OrderTypeName = OrderType.OrderTypeName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = OrderType.IsActive,
                                Remarks = OrderType.Remarks
                            };

                            var sqlResponce = await _OrderTypeService.UpdateOrderTypeAsync(headerViewModel.RegId, headerViewModel.CompanyId, OrderTypeEntity, headerViewModel.UserId);
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

        //[HttpDelete, Route("Delete/{OrderTypeId}")]
        //[Authorize]
        //public async Task<ActionResult<M_OrderType>> DeleteOrderType(Int16 OrderTypeId)
        //{
        //    try
        //    {
        //
        //

        //        if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
        //        {
        //            var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.OrderType, headerViewModel.UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var OrderTypeToDelete = await _OrderTypeService.GetOrderTypeByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, OrderTypeId, headerViewModel.UserId);

        //                    if (OrderTypeToDelete == null)
        //                        return NotFound($"M_OrderType with Id = {OrderTypeId} not found");

        //                    var sqlResponce = await _OrderTypeService.DeleteOrderTypeAsync(headerViewModel.RegId,headerViewModel.CompanyId, OrderTypeToDelete, headerViewModel.UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"OrderType_{OrderTypeId}");
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