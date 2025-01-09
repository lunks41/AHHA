﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Accounts.AR;
using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AR;
using AHHA.Core.Helper;
using AHHA.Core.Models.Account;
using AHHA.Core.Models.Account.AR;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Accounts.AR
{
    [Route("api/Account")]
    [ApiController]
    public class ARAdjustmentController : BaseController
    {
        private readonly IARAdjustmentService _ARAdjustmentService;
        private readonly ILogger<ARAdjustmentController> _logger;

        public ARAdjustmentController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<ARAdjustmentController> logger, IARAdjustmentService ARAdjustmentService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _ARAdjustmentService = ARAdjustmentService;
        }

        [HttpGet, Route("GetARAdjustment")]
        [Authorize]
        public async Task<ActionResult> GetARAdjustment([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.Adjustment, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var cacheData = await _ARAdjustmentService.GetARAdjustmentListAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    headerViewModel.pageSize, headerViewModel.pageNumber,
                    headerViewModel.searchString.Trim(), headerViewModel.UserId
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
                _logger.LogError($"Error in GetARAdjustment: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpGet, Route("GetARAdjustmentbyId/{AdjustmentId}")]
        [Authorize]
        public async Task<ActionResult<ARAdjustmentViewModel>> GetARAdjustmentById(string AdjustmentId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (AdjustmentId == "0")
                    return NotFound("Invalid Id");

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.Adjustment, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var arAdjustmentViewModel = await _ARAdjustmentService.GetARAdjustmentByIdAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    Convert.ToInt64(string.IsNullOrEmpty(AdjustmentId) ? 0 : AdjustmentId.Trim()),
                    string.Empty, headerViewModel.UserId
                );

                if (arAdjustmentViewModel == null)
                    return StatusCode(StatusCodes.Status200OK, new SqlResponse { Result = 0, Message = "Data does not exist", Data = null });

                return Ok(new SqlResponse { Result = 1, Message = "Success", Data = arAdjustmentViewModel });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetARAdjustmentById: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpGet, Route("GetARAdjustmentbyNo/{AdjustmentNo}")]
        [Authorize]
        public async Task<ActionResult<ARAdjustmentViewModel>> GetARAdjustmentByNo(string AdjustmentNo, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (AdjustmentNo == "")
                    return NotFound("Invalid Id");

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.Adjustment, headerViewModel.UserId);
                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var arAdjustmentViewModel = await _ARAdjustmentService.GetARAdjustmentByIdAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId, 0, AdjustmentNo, headerViewModel.UserId
                );
                if (arAdjustmentViewModel == null)
                    return StatusCode(StatusCodes.Status200OK, new SqlResponse { Result = 0, Message = "Data does not exist", Data = null });

                return Ok(new SqlResponse { Result = 1, Message = "Success", Data = arAdjustmentViewModel });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetARAdjustmentByNo: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpPost, Route("SaveARAdjustment")]
        [Authorize]
        public async Task<ActionResult<ARAdjustmentViewModel>> SaveARAdjustment(
            ARAdjustmentViewModel aRAdjustmentViewModel,
            [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(
                    headerViewModel.RegId,
                    headerViewModel.CompanyId,
                    (short)E_Modules.AR,
                    (int)E_AR.Adjustment,
                    headerViewModel.UserId);

                if (userGroupRight == null || (!userGroupRight.IsCreate && !userGroupRight.IsEdit))
                    return NotFound(GenrateMessage.authenticationfailed);

                if (aRAdjustmentViewModel == null)
                    return NotFound(GenrateMessage.datanotfound);

                // Header Data Mapping
                var ARAdjustmentEntity = new ArAdjustmentHd
                {
                    CompanyId = headerViewModel.CompanyId,
                    AdjustmentId = aRAdjustmentViewModel.AdjustmentId != null ? Convert.ToInt64(aRAdjustmentViewModel.AdjustmentId) : 0,
                    AdjustmentNo = aRAdjustmentViewModel.AdjustmentNo ?? string.Empty,
                    ReferenceNo = aRAdjustmentViewModel.ReferenceNo ?? string.Empty,
                    TrnDate = DateHelperStatic.ParseClientDate(aRAdjustmentViewModel.TrnDate),
                    AccountDate = DateHelperStatic.ParseClientDate(aRAdjustmentViewModel.AccountDate),
                    DeliveryDate = DateHelperStatic.ParseClientDate(aRAdjustmentViewModel.DeliveryDate),
                    DueDate = DateHelperStatic.ParseClientDate(aRAdjustmentViewModel.DueDate),
                    CustomerId = aRAdjustmentViewModel.CustomerId,
                    CurrencyId = aRAdjustmentViewModel.CurrencyId,
                    ExhRate = aRAdjustmentViewModel.ExhRate,
                    CtyExhRate = aRAdjustmentViewModel.CtyExhRate,
                    CreditTermId = aRAdjustmentViewModel.CreditTermId,
                    BankId = aRAdjustmentViewModel.BankId,
                    IsDebit = aRAdjustmentViewModel.IsDebit,
                    TotAmt = aRAdjustmentViewModel.TotAmt,
                    TotLocalAmt = aRAdjustmentViewModel.TotLocalAmt,
                    TotCtyAmt = aRAdjustmentViewModel.TotCtyAmt,
                    GstClaimDate = DateHelperStatic.ParseClientDate(aRAdjustmentViewModel.GstClaimDate),
                    GstAmt = aRAdjustmentViewModel.GstAmt,
                    GstLocalAmt = aRAdjustmentViewModel.GstLocalAmt,
                    GstCtyAmt = aRAdjustmentViewModel.GstCtyAmt,
                    TotAmtAftGst = aRAdjustmentViewModel.TotAmtAftGst,
                    TotLocalAmtAftGst = aRAdjustmentViewModel.TotLocalAmtAftGst,
                    TotCtyAmtAftGst = aRAdjustmentViewModel.TotCtyAmtAftGst,
                    BalAmt = aRAdjustmentViewModel.BalAmt,
                    BalLocalAmt = aRAdjustmentViewModel.BalLocalAmt,
                    PayAmt = aRAdjustmentViewModel.PayAmt,
                    PayLocalAmt = aRAdjustmentViewModel.PayLocalAmt,
                    ExGainLoss = aRAdjustmentViewModel.ExGainLoss,
                    Remarks = aRAdjustmentViewModel.Remarks ?? string.Empty,
                    Address1 = aRAdjustmentViewModel.Address1 ?? string.Empty,
                    Address2 = aRAdjustmentViewModel.Address2 ?? string.Empty,
                    Address3 = aRAdjustmentViewModel.Address3 ?? string.Empty,
                    Address4 = aRAdjustmentViewModel.Address4 ?? string.Empty,
                    PinCode = aRAdjustmentViewModel.PinCode ?? string.Empty,
                    CountryId = aRAdjustmentViewModel.CountryId,
                    PhoneNo = aRAdjustmentViewModel.PhoneNo ?? string.Empty,
                    FaxNo = aRAdjustmentViewModel.FaxNo ?? string.Empty,
                    ContactName = aRAdjustmentViewModel.ContactName ?? string.Empty,
                    MobileNo = aRAdjustmentViewModel.MobileNo ?? string.Empty,
                    EmailAdd = aRAdjustmentViewModel.EmailAdd ?? string.Empty,
                    ModuleFrom = aRAdjustmentViewModel.ModuleFrom ?? string.Empty,
                    SupplierName = aRAdjustmentViewModel.SupplierName ?? string.Empty,
                    SuppAdjustmentNo = aRAdjustmentViewModel.SuppAdjustmentNo ?? string.Empty,
                    APAdjustmentId = !string.IsNullOrEmpty(aRAdjustmentViewModel.APAdjustmentId?.Trim()) ? Convert.ToInt64(aRAdjustmentViewModel.APAdjustmentId.Trim()) : 0,
                    APAdjustmentNo = aRAdjustmentViewModel.APAdjustmentNo ?? string.Empty,
                    CreateById = headerViewModel.UserId,
                    EditById = headerViewModel.UserId,
                    EditDate = DateTime.Now,
                };

                // Details Mapping
                var arAdjustmentDtEntities = aRAdjustmentViewModel.data_details?.Select(item => new ArAdjustmentDt
                {
                    AdjustmentId = Convert.ToInt64(item.AdjustmentId),
                    AdjustmentNo = item.AdjustmentNo,
                    ItemNo = item.ItemNo,
                    SeqNo = item.SeqNo,
                    ProductId = item.ProductId,
                    GLId = item.GLId,
                    QTY = item.QTY,
                    BillQTY = item.BillQTY,
                    UomId = item.UomId,
                    UnitPrice = item.UnitPrice,
                    TotAmt = item.TotAmt,
                    TotLocalAmt = item.TotLocalAmt,
                    TotCtyAmt = item.TotCtyAmt,
                    GstId = item.GstId,
                    GstPercentage = item.GstPercentage,
                    GstAmt = item.GstAmt,
                    GstLocalAmt = item.GstLocalAmt,
                    GstCtyAmt = item.GstCtyAmt,
                    DeliveryDate = DateHelperStatic.ParseClientDate(item.DeliveryDate),
                    SupplyDate = DateHelperStatic.ParseClientDate(item.SupplyDate),
                    Remarks = item.Remarks ?? string.Empty,
                }).ToList();

                // Save AR Debit Note
                var sqlResponse = await _ARAdjustmentService.SaveARAdjustmentAsync(
                    headerViewModel.RegId,
                    headerViewModel.CompanyId,
                    ARAdjustmentEntity,
                    arAdjustmentDtEntities,
                    headerViewModel.UserId);

                if (sqlResponse.Result > 0)
                {
                    var customerModel = await _ARAdjustmentService.GetARAdjustmentByIdAsync(
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
                     new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpPost, Route("DeleteARAdjustment")]
        [Authorize]
        public async Task<ActionResult<ARAdjustmentViewModel>> DeleteARAdjustment(DeleteViewModel deleteViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.Adjustment, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                if (!userGroupRight.IsDelete)
                    return Forbid("You do not have permission to delete");

                if (string.IsNullOrEmpty(deleteViewModel.DocumentId))
                    return NotFound("Data not found");

                var sqlResponse = await _ARAdjustmentService.DeleteARAdjustmentAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    Convert.ToInt64(deleteViewModel.DocumentId),
                    deleteViewModel.CancelRemarks, headerViewModel.UserId
                );

                if (sqlResponse.Result > 0)
                {
                    var customerModel = await _ARAdjustmentService.GetARAdjustmentByIdAsync(
                        headerViewModel.RegId, headerViewModel.CompanyId,
                        Convert.ToInt64(deleteViewModel.DocumentId), string.Empty, headerViewModel.UserId
                    );

                    return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = customerModel });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteARAdjustment: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Internal Server Error");
            }
        }
    }
}