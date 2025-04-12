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
    public class ARDebitNoteController : BaseController
    {
        private readonly IARDebitNoteService _ARDebitNoteService;
        private readonly ILogger<ARDebitNoteController> _logger;

        public ARDebitNoteController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<ARDebitNoteController> logger, IARDebitNoteService ARDebitNoteService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _ARDebitNoteService = ARDebitNoteService;
        }

        [HttpGet, Route("GetARDebitNote")]
        [Authorize]
        public async Task<ActionResult> GetARDebitNote([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.DebitNote, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var cacheData = await _ARDebitNoteService.GetARDebitNoteListAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    headerViewModel.pageSize, headerViewModel.pageNumber,
                    headerViewModel.searchString?.Trim(), headerViewModel.fromDate, headerViewModel.toDate, headerViewModel.UserId
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
                _logger.LogError($"Error in GetARDebitNote: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpGet, Route("GetARDebitNotebyIdNo/{DebitNoteId}/{DebitNoteNo}")]
        [Authorize]
        public async Task<ActionResult<ARDebitNoteViewModel>> GetARDebitNoteByIdNo(string DebitNoteId, string DebitNoteNo, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (DebitNoteId == "0" && DebitNoteNo == "")
                    return NotFound("Invalid Id");

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.DebitNote, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var arDebitNoteViewModel = await _ARDebitNoteService.GetARDebitNoteByIdAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    Convert.ToInt64(string.IsNullOrEmpty(DebitNoteId) ? 0 : DebitNoteId?.Trim()),
                    DebitNoteNo, headerViewModel.UserId
                );

                if (arDebitNoteViewModel == null)
                    return StatusCode(StatusCodes.Status200OK, new SqlResponse { Result = 0, Message = "Data does not exist", Data = null });

                return Ok(new SqlResponse { Result = 1, Message = "Success", Data = arDebitNoteViewModel });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetARDebitNoteById: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpPost, Route("SaveARDebitNote")]
        [Authorize]
        public async Task<ActionResult<ARDebitNoteViewModel>> SaveARDebitNote(
            ARDebitNoteViewModel aRDebitNoteViewModel,
            [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (aRDebitNoteViewModel == null)
                    return NotFound(GenerateMessage.DataNotFound);

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(
                    headerViewModel.RegId,
                    headerViewModel.CompanyId,
                    (short)E_Modules.AR,
                    (int)E_AR.DebitNote,
                    headerViewModel.UserId);

                if (userGroupRight == null || (!userGroupRight.IsCreate && !userGroupRight.IsEdit))
                    return NotFound(GenerateMessage.AuthenticationFailed);

                // Header Data Mapping
                var ARDebitNoteEntity = new ArDebitNoteHd
                {
                    CompanyId = headerViewModel.CompanyId,
                    DebitNoteId = aRDebitNoteViewModel.DebitNoteId != null ? Convert.ToInt64(aRDebitNoteViewModel.DebitNoteId) : 0,
                    DebitNoteNo = aRDebitNoteViewModel.DebitNoteNo?.Trim() ?? string.Empty,
                    ReferenceNo = aRDebitNoteViewModel.ReferenceNo?.Trim() ?? string.Empty,
                    TrnDate = DateHelperStatic.ParseClientDate(aRDebitNoteViewModel.TrnDate),
                    AccountDate = DateHelperStatic.ParseClientDate(aRDebitNoteViewModel.AccountDate),
                    DeliveryDate = DateHelperStatic.ParseClientDate(aRDebitNoteViewModel.DeliveryDate),
                    DueDate = DateHelperStatic.ParseClientDate(aRDebitNoteViewModel.DueDate),
                    CustomerId = aRDebitNoteViewModel.CustomerId,
                    CurrencyId = aRDebitNoteViewModel.CurrencyId,
                    ExhRate = aRDebitNoteViewModel.ExhRate,
                    CtyExhRate = aRDebitNoteViewModel.CtyExhRate,
                    CreditTermId = aRDebitNoteViewModel.CreditTermId,
                    BankId = aRDebitNoteViewModel.BankId,
                    InvoiceId = aRDebitNoteViewModel.InvoiceId != null ? Convert.ToInt64(aRDebitNoteViewModel.InvoiceId) : 0,
                    InvoiceNo = aRDebitNoteViewModel.InvoiceNo?.Trim() ?? string.Empty,
                    TotAmt = aRDebitNoteViewModel.TotAmt,
                    TotLocalAmt = aRDebitNoteViewModel.TotLocalAmt,
                    TotCtyAmt = aRDebitNoteViewModel.TotCtyAmt,
                    GstClaimDate = DateHelperStatic.ParseClientDate(aRDebitNoteViewModel.GstClaimDate),
                    GstAmt = aRDebitNoteViewModel.GstAmt,
                    GstLocalAmt = aRDebitNoteViewModel.GstLocalAmt,
                    GstCtyAmt = aRDebitNoteViewModel.GstCtyAmt,
                    TotAmtAftGst = aRDebitNoteViewModel.TotAmtAftGst,
                    TotLocalAmtAftGst = aRDebitNoteViewModel.TotLocalAmtAftGst,
                    TotCtyAmtAftGst = aRDebitNoteViewModel.TotCtyAmtAftGst,
                    BalAmt = aRDebitNoteViewModel.BalAmt,
                    BalLocalAmt = aRDebitNoteViewModel.BalLocalAmt,
                    PayAmt = aRDebitNoteViewModel.PayAmt,
                    PayLocalAmt = aRDebitNoteViewModel.PayLocalAmt,
                    ExGainLoss = aRDebitNoteViewModel.ExGainLoss,
                    Remarks = aRDebitNoteViewModel.Remarks?.Trim() ?? string.Empty,
                    Address1 = aRDebitNoteViewModel.Address1?.Trim() ?? string.Empty,
                    Address2 = aRDebitNoteViewModel.Address2?.Trim() ?? string.Empty,
                    Address3 = aRDebitNoteViewModel.Address3?.Trim() ?? string.Empty,
                    Address4 = aRDebitNoteViewModel.Address4?.Trim() ?? string.Empty,
                    PinCode = aRDebitNoteViewModel.PinCode?.Trim() ?? string.Empty,
                    CountryId = aRDebitNoteViewModel.CountryId,
                    PhoneNo = aRDebitNoteViewModel.PhoneNo?.Trim() ?? string.Empty,
                    FaxNo = aRDebitNoteViewModel.FaxNo?.Trim() ?? string.Empty,
                    ContactName = aRDebitNoteViewModel.ContactName?.Trim() ?? string.Empty,
                    MobileNo = aRDebitNoteViewModel.MobileNo?.Trim() ?? string.Empty,
                    EmailAdd = aRDebitNoteViewModel.EmailAdd?.Trim() ?? string.Empty,
                    ModuleFrom = aRDebitNoteViewModel.ModuleFrom?.Trim() ?? string.Empty,
                    SupplierName = aRDebitNoteViewModel.SupplierName?.Trim() ?? string.Empty,
                    SuppDebitNoteNo = aRDebitNoteViewModel.SuppDebitNoteNo?.Trim() ?? string.Empty,
                    APDebitNoteId = !string.IsNullOrEmpty(aRDebitNoteViewModel.APDebitNoteId?.Trim()) ? Convert.ToInt64(aRDebitNoteViewModel.APDebitNoteId?.Trim()) : 0,
                    APDebitNoteNo = aRDebitNoteViewModel.APDebitNoteNo?.Trim() ?? string.Empty,
                    CreateById = headerViewModel.UserId,
                    EditById = headerViewModel.UserId,
                    EditDate = DateTime.Now,
                };

                // Details Mapping
                var arDebitNoteDtEntities = aRDebitNoteViewModel.data_details?.Select(item => new ArDebitNoteDt
                {
                    DebitNoteId = aRDebitNoteViewModel.DebitNoteId != null ? Convert.ToInt64(aRDebitNoteViewModel.DebitNoteId) : 0,
                    DebitNoteNo = item.DebitNoteNo,
                    ItemNo = item.ItemNo,
                    SeqNo = item.SeqNo,
                    DocItemNo = item.DocItemNo,
                    ProductId = item.ProductId,
                    GLId = item.GLId,
                    QTY = item.QTY,
                    BillQTY = item.BillQTY,
                    UomId = item.UomId,
                    UnitPrice = item.UnitPrice,
                    TotAmt = item.TotAmt,
                    TotLocalAmt = item.TotLocalAmt,
                    TotCtyAmt = item.TotCtyAmt,
                    Remarks = item.Remarks?.Trim() ?? string.Empty,
                    GstId = item.GstId,
                    GstPercentage = item.GstPercentage,
                    GstAmt = item.GstAmt,
                    GstLocalAmt = item.GstLocalAmt,
                    GstCtyAmt = item.GstCtyAmt,
                    DeliveryDate = string.IsNullOrEmpty(item.DeliveryDate) ? (DateTime?)null : DateHelperStatic.ParseClientDate(item.DeliveryDate),
                    DepartmentId = item.DepartmentId,
                    EmployeeId = item.EmployeeId,
                    PortId = item.PortId,
                    VesselId = item.VesselId,
                    BargeId = item.BargeId,
                    VoyageId = item.VoyageId,
                    OperationId = Convert.ToInt64(item.OperationId),
                    OperationNo = item.OperationNo?.Trim() ?? string.Empty,
                    OPRefNo = item.OPRefNo?.Trim() ?? string.Empty,
                    SalesOrderId = Convert.ToInt64(item.SalesOrderId),
                    SalesOrderNo = item.SalesOrderNo?.Trim() ?? string.Empty,
                    SupplyDate = string.IsNullOrEmpty(item.SupplyDate) ? (DateTime?)null : DateHelperStatic.ParseClientDate(item.SupplyDate),
                    SupplierName = item.SupplierName?.Trim() ?? string.Empty,
                    SuppDebitNoteNo = item.SuppDebitNoteNo?.Trim() ?? string.Empty,
                    APDebitNoteId = Convert.ToInt64(item.APDebitNoteId),
                    APDebitNoteNo = item.APDebitNoteNo?.Trim() ?? string.Empty,
                    EditVersion = item.EditVersion,
                }).ToList();

                var sqlResponse = await _ARDebitNoteService.SaveARDebitNoteAsync(
                    headerViewModel.RegId,
                    headerViewModel.CompanyId,
                    ARDebitNoteEntity,
                    arDebitNoteDtEntities,
                    headerViewModel.UserId);

                if (sqlResponse.Result > 0)
                {
                    var customerModel = await _ARDebitNoteService.GetARDebitNoteByIdAsync(
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

        [HttpPost, Route("DeleteARDebitNote")]
        [Authorize]
        public async Task<ActionResult<ARDebitNoteViewModel>> DeleteARDebitNote(DeleteViewModel deleteViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.DebitNote, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                if (!userGroupRight.IsDelete)
                    return Forbid("You do not have permission to delete");

                if (string.IsNullOrEmpty(deleteViewModel.DocumentId))
                    return NotFound("Data not found");

                var sqlResponse = await _ARDebitNoteService.DeleteARDebitNoteAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    Convert.ToInt64(deleteViewModel.DocumentId),
                    deleteViewModel.CancelRemarks, headerViewModel.UserId
                );

                if (sqlResponse.Result > 0)
                {
                    var customerModel = await _ARDebitNoteService.GetARDebitNoteByIdAsync(
                        headerViewModel.RegId, headerViewModel.CompanyId,
                        Convert.ToInt64(deleteViewModel.DocumentId), string.Empty, headerViewModel.UserId
                    );

                    return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = customerModel });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteARDebitNote: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Internal Server Error");
            }
        }

        [HttpGet, Route("GetHistoryARDebitNotebyId/{DebitNoteId}")]
        [Authorize]
        public async Task<ActionResult<ARDebitNoteViewModel>> GetHistoryARDebitNotebyId(string DebitNoteId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (DebitNoteId == null || DebitNoteId == "" || DebitNoteId == "0")
                    return NoContent();

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.DebitNote, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var arDebitNoteViewModel = await _ARDebitNoteService.GetHistoryARDebitNoteByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(string.IsNullOrEmpty(DebitNoteId) ? 0 : DebitNoteId?.Trim()), string.Empty, headerViewModel.UserId);

                if (arDebitNoteViewModel == null)
                    return StatusCode(StatusCodes.Status200OK, new SqlResponse { Result = 0, Message = "Data does not exist", Data = null });

                return Ok(new SqlResponse { Result = 1, Message = "Success", Data = arDebitNoteViewModel });
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