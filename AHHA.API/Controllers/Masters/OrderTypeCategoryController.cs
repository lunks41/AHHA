﻿using AHHA.Application.IServices;
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
        public async Task<ActionResult> GetOrderTypeCategory([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.OrderTypeCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _OrderTypeCategoryService.GetOrderTypeCategoryListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetOrderTypeCategorybyid/{OrderTypeCategoryId}")]
        [Authorize]
        public async Task<ActionResult<OrderTypeCategoryViewModel>> GetOrderTypeCategoryById(Int16 OrderTypeCategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.OrderTypeCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var orderTypeCategoryViewModel = _mapper.Map<OrderTypeCategoryViewModel>(await _OrderTypeCategoryService.GetOrderTypeCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, OrderTypeCategoryId, headerViewModel.UserId));

                        if (orderTypeCategoryViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, orderTypeCategoryViewModel);
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
        public async Task<ActionResult<OrderTypeCategoryViewModel>> CreateOrderTypeCategory(OrderTypeCategoryViewModel orderTypeCategoryViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.OrderTypeCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (orderTypeCategoryViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var OrderTypeCategoryEntity = new M_OrderTypeCategory
                            {
                                CompanyId = headerViewModel.CompanyId,
                                OrderTypeCategoryId = orderTypeCategoryViewModel.OrderTypeCategoryId,
                                OrderTypeCategoryCode = orderTypeCategoryViewModel.OrderTypeCategoryCode,
                                OrderTypeCategoryName = orderTypeCategoryViewModel.OrderTypeCategoryName,
                                Remarks = orderTypeCategoryViewModel.Remarks,
                                IsActive = orderTypeCategoryViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                            };

                            var createdOrderTypeCategory = await _OrderTypeCategoryService.AddOrderTypeCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, OrderTypeCategoryEntity, headerViewModel.UserId);
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
        public async Task<ActionResult<OrderTypeCategoryViewModel>> UpdateOrderTypeCategory(Int16 OrderTypeCategoryId, [FromBody] OrderTypeCategoryViewModel orderTypeCategoryViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            var OrderTypeCategoryViewModel = new OrderTypeCategoryViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.OrderTypeCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (OrderTypeCategoryId != orderTypeCategoryViewModel.OrderTypeCategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "OrderTypeCategory mismatch");

                            var OrderTypeCategoryToUpdate = await _OrderTypeCategoryService.GetOrderTypeCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, OrderTypeCategoryId, headerViewModel.UserId);

                            if (OrderTypeCategoryToUpdate == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var OrderTypeCategoryEntity = new M_OrderTypeCategory
                            {
                                CompanyId = headerViewModel.CompanyId,
                                OrderTypeCategoryId = orderTypeCategoryViewModel.OrderTypeCategoryId,
                                OrderTypeCategoryCode = orderTypeCategoryViewModel.OrderTypeCategoryCode,
                                OrderTypeCategoryName = orderTypeCategoryViewModel.OrderTypeCategoryName,
                                Remarks = orderTypeCategoryViewModel.Remarks,
                                IsActive = orderTypeCategoryViewModel.IsActive,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponce = await _OrderTypeCategoryService.UpdateOrderTypeCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, OrderTypeCategoryEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteOrderTypeCategory/{OrderTypeCategoryId}")]
        [Authorize]
        public async Task<ActionResult<M_OrderTypeCategory>> DeleteOrderTypeCategory(Int16 OrderTypeCategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.OrderTypeCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var OrderTypeCategoryToDelete = await _OrderTypeCategoryService.GetOrderTypeCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, OrderTypeCategoryId, headerViewModel.UserId);

                            if (OrderTypeCategoryToDelete == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var sqlResponce = await _OrderTypeCategoryService.DeleteOrderTypeCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, OrderTypeCategoryToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"OrderTypeCategory_{OrderTypeCategoryId}");
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