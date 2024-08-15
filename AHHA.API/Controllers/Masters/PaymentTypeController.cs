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
    public class PaymentTypeController : BaseController
    {
        private readonly IPaymentTypeService _PaymentTypeService;
        private readonly ILogger<PaymentTypeController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public PaymentTypeController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<PaymentTypeController> logger, IPaymentTypeService PaymentTypeService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _PaymentTypeService = PaymentTypeService;
        }

        [HttpGet, Route("GetPaymentType")]
        [Authorize]
        public async Task<ActionResult> GetAllPaymentType()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.PaymentType, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<PaymentTypeViewModelCount>("PaymentType");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _PaymentTypeService.GetPaymentTypeListAsync(CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<PaymentTypeViewModelCount>("PaymentType", cacheData, expirationTime);

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

        [HttpGet, Route("GetPaymentTypebyid/{PaymentTypeId}")]
        [Authorize]
        public async Task<ActionResult<PaymentTypeViewModel>> GetPaymentTypeById(Int16 PaymentTypeId)
        {
            var PaymentTypeViewModel = new PaymentTypeViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.PaymentType, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"PaymentType_{PaymentTypeId}", out PaymentTypeViewModel? cachedProduct))
                        {
                            PaymentTypeViewModel = cachedProduct;
                        }
                        else
                        {
                            PaymentTypeViewModel = _mapper.Map<PaymentTypeViewModel>(await _PaymentTypeService.GetPaymentTypeByIdAsync(CompanyId, PaymentTypeId, UserId));

                            if (PaymentTypeViewModel == null)
                                return NotFound();
                            else
                                // Cache the PaymentType with an expiration time of 10 minutes
                                _memoryCache.Set($"PaymentType_{PaymentTypeId}", PaymentTypeViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, PaymentTypeViewModel);
                        //return Ok(PaymentTypeViewModel);
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

        [HttpPost, Route("AddPaymentType")]
        [Authorize]
        public async Task<ActionResult<PaymentTypeViewModel>> CreatePaymentType(PaymentTypeViewModel PaymentType)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.PaymentType, UserId);

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
                                CreateById = UserId,
                                IsActive = PaymentType.IsActive,
                                Remarks = PaymentType.Remarks
                            };

                            var createdPaymentType = await _PaymentTypeService.AddPaymentTypeAsync(CompanyId, PaymentTypeEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdPaymentType);

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
                    "Error creating new PaymentType record");
            }
        }

        [HttpPut, Route("UpdatePaymentType/{PaymentTypeId}")]
        [Authorize]
        public async Task<ActionResult<PaymentTypeViewModel>> UpdatePaymentType(Int16 PaymentTypeId, [FromBody] PaymentTypeViewModel PaymentType)
        {
            var PaymentTypeViewModel = new PaymentTypeViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.PaymentType, UserId);

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
                                var PaymentTypeToUpdate = await _PaymentTypeService.GetPaymentTypeByIdAsync(CompanyId, PaymentTypeId, UserId);

                                if (PaymentTypeToUpdate == null)
                                    return NotFound($"M_PaymentType with Id = {PaymentTypeId} not found");
                            }

                            var PaymentTypeEntity = new M_PaymentType
                            {
                                PaymentTypeCode = PaymentType.PaymentTypeCode,
                                PaymentTypeId = PaymentType.PaymentTypeId,
                                PaymentTypeName = PaymentType.PaymentTypeName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = PaymentType.IsActive,
                                Remarks = PaymentType.Remarks
                            };

                            var sqlResponce = await _PaymentTypeService.UpdatePaymentTypeAsync(CompanyId, PaymentTypeEntity, UserId);
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

        //[HttpDelete, Route("Delete/{PaymentTypeId}")]
        //[Authorize]
        //public async Task<ActionResult<M_PaymentType>> DeletePaymentType(Int16 PaymentTypeId)
        //{
        //    try
        //    {
        //        CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
        //        UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

        //        if (ValidateHeaders(CompanyId, UserId))
        //        {
        //            var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.PaymentType, UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var PaymentTypeToDelete = await _PaymentTypeService.GetPaymentTypeByIdAsync(CompanyId, PaymentTypeId, UserId);

        //                    if (PaymentTypeToDelete == null)
        //                        return NotFound($"M_PaymentType with Id = {PaymentTypeId} not found");

        //                    var sqlResponce = await _PaymentTypeService.DeletePaymentTypeAsync(CompanyId, PaymentTypeToDelete, UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"PaymentType_{PaymentTypeId}");
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


