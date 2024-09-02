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
        public async Task<ActionResult> GetOrderType([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.OrderType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _OrderTypeService.GetOrderTypeListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenrateMessage.datanotfound);

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

        [HttpGet, Route("GetOrderTypebyid/{OrderTypeId}")]
        [Authorize]
        public async Task<ActionResult<OrderTypeViewModel>> GetOrderTypeById(Int16 OrderTypeId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.OrderType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var orderTypeViewModel = _mapper.Map<OrderTypeViewModel>(await _OrderTypeService.GetOrderTypeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, OrderTypeId, headerViewModel.UserId));

                        if (orderTypeViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, orderTypeViewModel);
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
        public async Task<ActionResult<OrderTypeViewModel>> CreateOrderType(OrderTypeViewModel orderTypeViewModel, [FromHeader] HeaderViewModel headerViewModel)
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
                            if (orderTypeViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var OrderTypeEntity = new M_OrderType
                            {
                                CompanyId = headerViewModel.CompanyId,
                                OrderTypeId = orderTypeViewModel.OrderTypeId,
                                OrderTypeCode = orderTypeViewModel.OrderTypeCode,
                                OrderTypeName = orderTypeViewModel.OrderTypeName,
                                OrderTypeCategoryId = orderTypeViewModel.OrderTypeCategoryId,
                                Remarks = orderTypeViewModel.Remarks,
                                IsActive = orderTypeViewModel.IsActive,
                                CreateById = headerViewModel.UserId
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
        public async Task<ActionResult<OrderTypeViewModel>> UpdateOrderType(Int16 OrderTypeId, [FromBody] OrderTypeViewModel orderTypeViewModel, [FromHeader] HeaderViewModel headerViewModel)
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
                            if (OrderTypeId != orderTypeViewModel.OrderTypeId)
                                return StatusCode(StatusCodes.Status400BadRequest, "OrderType ID mismatch");

                            var OrderTypeToUpdate = await _OrderTypeService.GetOrderTypeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, OrderTypeId, headerViewModel.UserId);

                            if (OrderTypeToUpdate == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var OrderTypeEntity = new M_OrderType
                            {
                                CompanyId = headerViewModel.CompanyId,
                                OrderTypeId = orderTypeViewModel.OrderTypeId,
                                OrderTypeCode = orderTypeViewModel.OrderTypeCode,
                                OrderTypeName = orderTypeViewModel.OrderTypeName,
                                OrderTypeCategoryId = orderTypeViewModel.OrderTypeCategoryId,
                                Remarks = orderTypeViewModel.Remarks,
                                IsActive = orderTypeViewModel.IsActive,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
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

        [HttpDelete, Route("DeleteOrderType/{OrderTypeId}")]
        [Authorize]
        public async Task<ActionResult<M_OrderType>> DeleteOrderType(Int16 OrderTypeId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.OrderType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var OrderTypeToDelete = await _OrderTypeService.GetOrderTypeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, OrderTypeId, headerViewModel.UserId);

                            if (OrderTypeToDelete == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var sqlResponce = await _OrderTypeService.DeleteOrderTypeAsync(headerViewModel.RegId, headerViewModel.CompanyId, OrderTypeToDelete, headerViewModel.UserId);

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