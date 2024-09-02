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
    public class PaymentTypeController : BaseController
    {
        private readonly IPaymentTypeService _PaymentTypeService;
        private readonly ILogger<PaymentTypeController> _logger;

        public PaymentTypeController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<PaymentTypeController> logger, IPaymentTypeService PaymentTypeService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _PaymentTypeService = PaymentTypeService;
        }

        [HttpGet, Route("GetPaymentType")]
        [Authorize]
        public async Task<ActionResult> GetPaymentType([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.PaymentType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _PaymentTypeService.GetPaymentTypeListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetPaymentTypebyid/{PaymentTypeId}")]
        [Authorize]
        public async Task<ActionResult<PaymentTypeViewModel>> GetPaymentTypeById(Int16 PaymentTypeId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.PaymentType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var paymentTypeViewModel = _mapper.Map<PaymentTypeViewModel>(await _PaymentTypeService.GetPaymentTypeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, PaymentTypeId, headerViewModel.UserId));

                        if (paymentTypeViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, paymentTypeViewModel);
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

        [HttpPost, Route("AddPaymentType")]
        [Authorize]
        public async Task<ActionResult<PaymentTypeViewModel>> CreatePaymentType(PaymentTypeViewModel paymentTypeViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.PaymentType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (paymentTypeViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var PaymentTypeEntity = new M_PaymentType
                            {
                                PaymentTypeId = paymentTypeViewModel.PaymentTypeId,
                                CompanyId = paymentTypeViewModel.CompanyId,
                                PaymentTypeCode = paymentTypeViewModel.PaymentTypeCode,
                                PaymentTypeName = paymentTypeViewModel.PaymentTypeName,
                                Remarks = paymentTypeViewModel.Remarks,
                                IsActive = paymentTypeViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                            };

                            var createdPaymentType = await _PaymentTypeService.AddPaymentTypeAsync(headerViewModel.RegId, headerViewModel.CompanyId, PaymentTypeEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdPaymentType);
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
                    "Error creating new PaymentType record");
            }
        }

        [HttpPut, Route("UpdatePaymentType/{PaymentTypeId}")]
        [Authorize]
        public async Task<ActionResult<PaymentTypeViewModel>> UpdatePaymentType(Int16 PaymentTypeId, [FromBody] PaymentTypeViewModel paymentTypeViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.PaymentType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (PaymentTypeId != paymentTypeViewModel.PaymentTypeId)
                                return StatusCode(StatusCodes.Status400BadRequest, "PaymentType ID mismatch");

                            var PaymentTypeToUpdate = await _PaymentTypeService.GetPaymentTypeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, PaymentTypeId, headerViewModel.UserId);

                            if (PaymentTypeToUpdate == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var PaymentTypeEntity = new M_PaymentType
                            {
                                PaymentTypeId = paymentTypeViewModel.PaymentTypeId,
                                CompanyId = paymentTypeViewModel.CompanyId,
                                PaymentTypeCode = paymentTypeViewModel.PaymentTypeCode,
                                PaymentTypeName = paymentTypeViewModel.PaymentTypeName,
                                Remarks = paymentTypeViewModel.Remarks,
                                IsActive = paymentTypeViewModel.IsActive,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponce = await _PaymentTypeService.UpdatePaymentTypeAsync(headerViewModel.RegId, headerViewModel.CompanyId, PaymentTypeEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeletePaymentType/{PaymentTypeId}")]
        [Authorize]
        public async Task<ActionResult<M_PaymentType>> DeletePaymentType(Int16 PaymentTypeId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.PaymentType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var PaymentTypeToDelete = await _PaymentTypeService.GetPaymentTypeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, PaymentTypeId, headerViewModel.UserId);

                            if (PaymentTypeToDelete == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var sqlResponce = await _PaymentTypeService.DeletePaymentTypeAsync(headerViewModel.RegId, headerViewModel.CompanyId, PaymentTypeToDelete, headerViewModel.UserId);

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