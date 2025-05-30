﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Accounts.AP;
using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AP;
using AHHA.Core.Helper;
using AHHA.Core.Models.Account;
using AHHA.Core.Models.Account.AP;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Accounts.AP
{
    [Route("api/Account")]
    [ApiController]
    public class APPaymentController : BaseController
    {
        private readonly IAPPaymentService _APPaymentService;
        private readonly ILogger<APPaymentController> _logger;

        public APPaymentController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<APPaymentController> logger, IAPPaymentService APPaymentService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _APPaymentService = APPaymentService;
        }

        [HttpGet, Route("GetAPPayment")]
        [Authorize]
        public async Task<ActionResult> GetAPPayment([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AP, (Int32)E_AP.Payment, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var cacheData = await _APPaymentService.GetAPPaymentListAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString?.Trim(),
                    headerViewModel.fromDate, headerViewModel.toDate,
                     headerViewModel.UserId
                );

                if (cacheData == null)
                    return NotFound("Data not found");

                var sqlResponse = new SqlResponse
                {
                    Result = 1,
                    Message = "Success",
                    Data = cacheData.data,
                    TotalRecords = cacheData.totalRecords
                };

                return Ok(sqlResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAPPayment: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpGet, Route("GetAPPaymentbyIdNo/{PaymentId}/{PaymentNo}")]
        [Authorize]
        public async Task<ActionResult<APPaymentViewModel>> GetAPPaymentByIdNo(string PaymentId, string PaymentNo, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (PaymentId == "0" && PaymentNo == "")
                    return NotFound("Invalid Id");

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AP, (Int32)E_AP.Payment, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var arPaymentViewModel = await _APPaymentService.GetAPPaymentByIdAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    Convert.ToInt64(string.IsNullOrEmpty(PaymentId) ? 0 : PaymentId?.Trim()),
                    PaymentNo, headerViewModel.UserId
                );

                if (arPaymentViewModel == null)
                    return StatusCode(StatusCodes.Status200OK, new SqlResponse { Result = 0, Message = "Data does not exist", Data = null });

                return Ok(new SqlResponse { Result = 1, Message = "Success", Data = arPaymentViewModel });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAPPaymentById: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        //SAVE ONE APPayment BY INVOICEID
        [HttpPost, Route("SaveAPPayment")]
        [Authorize]
        public async Task<ActionResult<APPaymentViewModel>> SaveAPPayment(APPaymentViewModel aRPaymentViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (aRPaymentViewModel == null)
                    return NotFound(GenerateMessage.DataNotFound);

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(
                    headerViewModel.RegId,
                headerViewModel.CompanyId,
                    (short)E_Modules.AP,
                    (int)E_AP.Payment,
                    headerViewModel.UserId);

                if (userGroupRight == null || (!userGroupRight.IsCreate && !userGroupRight.IsEdit))
                    return NotFound(GenerateMessage.AuthenticationFailed);

                //Header Data Mapping
                var APPaymentEntity = new ApPaymentHd
                {
                    CompanyId = headerViewModel.CompanyId,
                    PaymentId = Convert.ToInt64(string.IsNullOrEmpty(aRPaymentViewModel.PaymentId) ? 0 : aRPaymentViewModel.PaymentId?.Trim()),
                    PaymentNo = aRPaymentViewModel.PaymentNo,
                    ReferenceNo = aRPaymentViewModel.ReferenceNo == null ? string.Empty : aRPaymentViewModel.ReferenceNo,
                    TrnDate = DateHelperStatic.ParseClientDate(aRPaymentViewModel.TrnDate),
                    AccountDate = DateHelperStatic.ParseClientDate(aRPaymentViewModel.AccountDate),
                    BankId = Convert.ToInt16(aRPaymentViewModel.BankId),
                    PaymentTypeId = Convert.ToInt16(aRPaymentViewModel.PaymentTypeId),
                    ChequeNo = aRPaymentViewModel.ChequeNo,
                    ChequeDate = DateHelperStatic.ParseClientDate(aRPaymentViewModel.ChequeDate),
                    SupplierId = aRPaymentViewModel.SupplierId,
                    CurrencyId = aRPaymentViewModel.CurrencyId,
                    ExhRate = aRPaymentViewModel.ExhRate,
                    TotAmt = aRPaymentViewModel.TotAmt,
                    TotLocalAmt = aRPaymentViewModel.TotLocalAmt,
                    UnAllocTotAmt = aRPaymentViewModel.UnAllocTotAmt,
                    UnAllocTotLocalAmt = aRPaymentViewModel.UnAllocTotLocalAmt,
                    RecCurrencyId = aRPaymentViewModel.RecCurrencyId,
                    RecExhRate = aRPaymentViewModel.RecExhRate,
                    RecTotAmt = aRPaymentViewModel.RecTotAmt,
                    RecTotLocalAmt = aRPaymentViewModel.RecTotLocalAmt,
                    ExhGainLoss = aRPaymentViewModel.ExhGainLoss,
                    AllocTotAmt = aRPaymentViewModel.AllocTotAmt,
                    AllocTotLocalAmt = aRPaymentViewModel.AllocTotLocalAmt,
                    BankChargeGLId = aRPaymentViewModel.BankChargeGLId,
                    BankChargesAmt = aRPaymentViewModel.BankChargesAmt,
                    BankChargesLocalAmt = aRPaymentViewModel.BankChargesLocalAmt,
                    Remarks = aRPaymentViewModel.Remarks == null ? string.Empty : aRPaymentViewModel.Remarks,
                    ModuleFrom = aRPaymentViewModel.ModuleFrom == null ? string.Empty : aRPaymentViewModel.ModuleFrom,
                    CreateById = headerViewModel.UserId,
                    EditById = headerViewModel.UserId,
                    EditDate = DateTime.Now,
                    DocExhRate = aRPaymentViewModel.ExhRate,
                    DocTotAmt = aRPaymentViewModel.TotAmt,
                    DocTotLocalAmt = aRPaymentViewModel.TotLocalAmt
                };

                //Details Table data mapping
                var APPaymentDtEntities = new List<ApPaymentDt>();

                if (aRPaymentViewModel.data_details != null)
                {
                    foreach (var item in aRPaymentViewModel.data_details)
                    {
                        var APPaymentDtEntity = new ApPaymentDt
                        {
                            CompanyId = headerViewModel.CompanyId,
                            PaymentId = Convert.ToInt64(string.IsNullOrEmpty(item.PaymentId) ? 0 : item.PaymentId?.Trim()),
                            PaymentNo = item.PaymentNo,
                            ItemNo = item.ItemNo,
                            TransactionId = item.TransactionId,
                            DocumentId = Convert.ToInt64(string.IsNullOrEmpty(item.DocumentId) ? 0 : item.DocumentId?.Trim()),
                            DocumentNo = item.DocumentNo,
                            ReferenceNo = item.ReferenceNo,
                            DocCurrencyId = item.DocCurrencyId,
                            DocExhRate = item.DocExhRate,
                            DocAccountDate = DateHelperStatic.ParseClientDate(item.DocAccountDate),
                            DocDueDate = DateHelperStatic.ParseClientDate(item.DocDueDate),
                            DocTotAmt = item.DocTotAmt,
                            DocTotLocalAmt = item.DocTotLocalAmt,
                            DocBalAmt = item.DocBalAmt,
                            DocBalLocalAmt = item.DocBalLocalAmt,
                            AllocAmt = item.AllocAmt,
                            AllocLocalAmt = item.AllocLocalAmt,
                            DocAllocAmt = item.DocAllocAmt,
                            DocAllocLocalAmt = item.DocAllocLocalAmt,
                            CentDiff = item.CentDiff,
                            ExhGainLoss = item.ExhGainLoss,
                            EditVersion = item.EditVersion,
                        };

                        APPaymentDtEntities.Add(APPaymentDtEntity);
                    }
                }

                var sqlResponse = await _APPaymentService.SaveAPPaymentAsync(
                   headerViewModel.RegId,
                   headerViewModel.CompanyId,
                   APPaymentEntity,
                   APPaymentDtEntities,
                   headerViewModel.UserId);

                if (sqlResponse.Result > 0)
                {
                    var customerModel = await _APPaymentService.GetAPPaymentByIdAsync(
                        headerViewModel.RegId,
                        headerViewModel.CompanyId,
                        sqlResponse.Result,
                        string.Empty,
                        headerViewModel.UserId);

                    return Ok(new SqlResponse { Result = 1, Message = sqlResponse.Message, Data = customerModel, TotalRecords = 0 });
                }

                return StatusCode(StatusCodes.Status202Accepted, sqlResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new APPayment record");
            }
        }

        [HttpPost, Route("DeleteAPPayment")]
        [Authorize]
        public async Task<ActionResult<APPaymentViewModel>> DeleteAPPayment(DeleteViewModel deleteViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AP, (Int32)E_AP.Payment, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                if (!userGroupRight.IsDelete)
                    return Forbid("You do not have permission to delete");

                if (string.IsNullOrEmpty(deleteViewModel.DocumentId))
                    return NotFound("Data not found");

                var sqlResponse = await _APPaymentService.DeleteAPPaymentAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    Convert.ToInt64(deleteViewModel.DocumentId), deleteViewModel.DocumentNo,
                    deleteViewModel.CancelRemarks, headerViewModel.UserId
                );

                if (sqlResponse.Result > 0)
                {
                    var customerModel = await _APPaymentService.GetAPPaymentByIdAsync(
                        headerViewModel.RegId, headerViewModel.CompanyId,
                        Convert.ToInt64(deleteViewModel.DocumentId), string.Empty, headerViewModel.UserId
                    );

                    return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = customerModel });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteAPPayment: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Internal Server Error");
            }
        }

        [HttpGet, Route("GetHistoryAPPaymentbyId/{PaymentId}")]
        [Authorize]
        public async Task<ActionResult<APPaymentViewModel>> GetHistoryAPPaymentbyId(string PaymentId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (PaymentId == null || PaymentId == "" || PaymentId == "0")
                    return NoContent();

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AP, (Int32)E_AP.Payment, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var arPaymentViewModel = await _APPaymentService.GetHistoryAPPaymentByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(string.IsNullOrEmpty(PaymentId) ? 0 : PaymentId?.Trim()), string.Empty, headerViewModel.UserId);

                if (arPaymentViewModel == null)
                    return StatusCode(StatusCodes.Status200OK, new SqlResponse { Result = 0, Message = "Data does not exist", Data = null });

                return Ok(new SqlResponse { Result = 1, Message = "Success", Data = arPaymentViewModel });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }
    }
}