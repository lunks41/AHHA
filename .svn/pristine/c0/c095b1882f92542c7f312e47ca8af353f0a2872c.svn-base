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
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.PaymentType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        headerViewModel.searchString = headerViewModel.searchString == null ? string.Empty : headerViewModel.searchString.Trim();

                        var cacheData = await _PaymentTypeService.GetPaymentTypeListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetPaymentTypebyid/{PaymentTypeId}")]
        [Authorize]
        public async Task<ActionResult<PaymentTypeViewModel>> GetPaymentTypeById(Int16 PaymentTypeId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var PaymentTypeViewModel = new PaymentTypeViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.PaymentType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"PaymentType_{PaymentTypeId}", out PaymentTypeViewModel? cachedProduct))
                        {
                            PaymentTypeViewModel = cachedProduct;
                        }
                        else
                        {
                            PaymentTypeViewModel = _mapper.Map<PaymentTypeViewModel>(await _PaymentTypeService.GetPaymentTypeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, PaymentTypeId, headerViewModel.UserId));

                            if (PaymentTypeViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the PaymentType with an expiration time of 10 minutes
                                _memoryCache.Set($"PaymentType_{PaymentTypeId}", PaymentTypeViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, PaymentTypeViewModel);
                        //return Ok(PaymentTypeViewModel);
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
        public async Task<ActionResult<PaymentTypeViewModel>> CreatePaymentType(PaymentTypeViewModel PaymentType, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.PaymentType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (PaymentType == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_PaymentType ID mismatch");

                            var PaymentTypeEntity = new M_PaymentType
                            {
                                CompanyId = PaymentType.CompanyId,
                                PaymentTypeCode = PaymentType.PaymentTypeCode,
                                PaymentTypeId = PaymentType.PaymentTypeId,
                                PaymentTypeName = PaymentType.PaymentTypeName,
                                CreateById = headerViewModel.UserId,
                                IsActive = PaymentType.IsActive,
                                Remarks = PaymentType.Remarks
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
        public async Task<ActionResult<PaymentTypeViewModel>> UpdatePaymentType(Int16 PaymentTypeId, [FromBody] PaymentTypeViewModel PaymentType, [FromHeader] HeaderViewModel headerViewModel)
        {
            var PaymentTypeViewModel = new PaymentTypeViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.PaymentType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (PaymentTypeId != PaymentType.PaymentTypeId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_PaymentType ID mismatch");
                            //return BadRequest("M_PaymentType ID mismatch");

                            // Attempt to retrieve the PaymentType from the cache
                            if (_memoryCache.TryGetValue($"PaymentType_{PaymentTypeId}", out PaymentTypeViewModel? cachedProduct))
                            {
                                PaymentTypeViewModel = cachedProduct;
                            }
                            else
                            {
                                var PaymentTypeToUpdate = await _PaymentTypeService.GetPaymentTypeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, PaymentTypeId, headerViewModel.UserId);

                                if (PaymentTypeToUpdate == null)
                                    return NotFound($"M_PaymentType with Id = {PaymentTypeId} not found");
                            }

                            var PaymentTypeEntity = new M_PaymentType
                            {
                                PaymentTypeCode = PaymentType.PaymentTypeCode,
                                PaymentTypeId = PaymentType.PaymentTypeId,
                                PaymentTypeName = PaymentType.PaymentTypeName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = PaymentType.IsActive,
                                Remarks = PaymentType.Remarks
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
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.PaymentType, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var PaymentTypeToDelete = await _PaymentTypeService.GetPaymentTypeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, PaymentTypeId, headerViewModel.UserId);

                            if (PaymentTypeToDelete == null)
                                return NotFound($"M_PaymentType with Id = {PaymentTypeId} not found");

                            var sqlResponce = await _PaymentTypeService.DeletePaymentTypeAsync(headerViewModel.RegId, headerViewModel.CompanyId, PaymentTypeToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"PaymentType_{PaymentTypeId}");
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