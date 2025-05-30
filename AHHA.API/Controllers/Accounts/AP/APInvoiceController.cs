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
    public class APInvoiceController : BaseController
    {
        private readonly IAPInvoiceService _APInvoiceService;
        private readonly ILogger<APInvoiceController> _logger;

        public APInvoiceController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<APInvoiceController> logger, IAPInvoiceService APInvoiceService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _APInvoiceService = APInvoiceService;
        }

        [HttpGet, Route("GetAPInvoice")]
        [Authorize]
        public async Task<ActionResult> GetAPInvoice([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AP, (Int32)E_AP.Invoice, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var cacheData = await _APInvoiceService.GetAPInvoiceListAsync(
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
                _logger.LogError($"Error in GetAPInvoice: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpGet, Route("GetAPInvoicebyIdNo/{InvoiceId}/{InvoiceNo}")]
        [Authorize]
        public async Task<ActionResult<APInvoiceViewModel>> GetAPInvoiceByIdNo(string InvoiceId, string InvoiceNo, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if ((InvoiceId == "0" && InvoiceNo == ""))
                    return NotFound("Invalid Id");

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AP, (Int32)E_AP.Invoice, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var APInvoiceViewModel = await _APInvoiceService.GetAPInvoiceByIdNoAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    Convert.ToInt64(string.IsNullOrEmpty(InvoiceId) ? 0 : InvoiceId?.Trim()),
                    InvoiceNo, headerViewModel.UserId
                );

                if (APInvoiceViewModel == null)
                    return StatusCode(StatusCodes.Status200OK, new SqlResponse { Result = 0, Message = "Data does not exist", Data = null });

                return Ok(new SqlResponse { Result = 1, Message = "Success", Data = APInvoiceViewModel });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAPInvoiceById: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpPost, Route("SaveAPInvoice")]
        [Authorize]
        public async Task<ActionResult<APInvoiceViewModel>> SaveAPInvoice(
            APInvoiceViewModel APInvoiceViewModel,
            [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (APInvoiceViewModel == null)
                    return NotFound(GenerateMessage.DataNotFound);

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(
                    headerViewModel.RegId,
                    headerViewModel.CompanyId,
                    (short)E_Modules.AP,
                    (int)E_AP.Invoice,
                    headerViewModel.UserId);

                if (userGroupRight == null || (!userGroupRight.IsCreate && !userGroupRight.IsEdit))
                    return NotFound(GenerateMessage.AuthenticationFailed);

                // Header Data Mapping
                var APInvoiceEntity = new ApInvoiceHd
                {
                    CompanyId = headerViewModel.CompanyId,
                    InvoiceId = APInvoiceViewModel.InvoiceId != null ? Convert.ToInt64(APInvoiceViewModel.InvoiceId) : 0,
                    InvoiceNo = APInvoiceViewModel.InvoiceNo?.Trim() ?? string.Empty,
                    ReferenceNo = APInvoiceViewModel.ReferenceNo?.Trim() ?? string.Empty,
                    SuppInvoiceNo = APInvoiceViewModel.SuppInvoiceNo?.Trim() ?? string.Empty,
                    TrnDate = DateHelperStatic.ParseClientDate(APInvoiceViewModel.TrnDate),
                    AccountDate = DateHelperStatic.ParseClientDate(APInvoiceViewModel.AccountDate),
                    DeliveryDate = DateHelperStatic.ParseClientDate(APInvoiceViewModel.DeliveryDate),
                    DueDate = DateHelperStatic.ParseClientDate(APInvoiceViewModel.DueDate),
                    SupplierId = APInvoiceViewModel.SupplierId,
                    CurrencyId = APInvoiceViewModel.CurrencyId,
                    ExhRate = APInvoiceViewModel.ExhRate,
                    CtyExhRate = APInvoiceViewModel.CtyExhRate,
                    CreditTermId = APInvoiceViewModel.CreditTermId,
                    BankId = APInvoiceViewModel.BankId,
                    TotAmt = APInvoiceViewModel.TotAmt,
                    TotLocalAmt = APInvoiceViewModel.TotLocalAmt,
                    TotCtyAmt = APInvoiceViewModel.TotCtyAmt,
                    GstClaimDate = DateHelperStatic.ParseClientDate(APInvoiceViewModel.GstClaimDate),
                    GstAmt = APInvoiceViewModel.GstAmt,
                    GstLocalAmt = APInvoiceViewModel.GstLocalAmt,
                    GstCtyAmt = APInvoiceViewModel.GstCtyAmt,
                    TotAmtAftGst = APInvoiceViewModel.TotAmtAftGst,
                    TotLocalAmtAftGst = APInvoiceViewModel.TotLocalAmtAftGst,
                    TotCtyAmtAftGst = APInvoiceViewModel.TotCtyAmtAftGst,
                    BalAmt = APInvoiceViewModel.BalAmt,
                    BalLocalAmt = APInvoiceViewModel.BalLocalAmt,
                    PayAmt = APInvoiceViewModel.PayAmt,
                    PayLocalAmt = APInvoiceViewModel.PayLocalAmt,
                    ExGainLoss = APInvoiceViewModel.ExGainLoss,
                    Remarks = APInvoiceViewModel.Remarks?.Trim() ?? string.Empty,
                    Address1 = APInvoiceViewModel.Address1?.Trim() ?? string.Empty,
                    Address2 = APInvoiceViewModel.Address2?.Trim() ?? string.Empty,
                    Address3 = APInvoiceViewModel.Address3?.Trim() ?? string.Empty,
                    Address4 = APInvoiceViewModel.Address4?.Trim() ?? string.Empty,
                    PinCode = APInvoiceViewModel.PinCode?.Trim() ?? string.Empty,
                    CountryId = APInvoiceViewModel.CountryId,
                    PhoneNo = APInvoiceViewModel.PhoneNo?.Trim() ?? string.Empty,
                    FaxNo = APInvoiceViewModel.FaxNo?.Trim() ?? string.Empty,
                    ContactName = APInvoiceViewModel.ContactName?.Trim() ?? string.Empty,
                    MobileNo = APInvoiceViewModel.MobileNo?.Trim() ?? string.Empty,
                    EmailAdd = APInvoiceViewModel.EmailAdd?.Trim() ?? string.Empty,
                    ModuleFrom = APInvoiceViewModel.ModuleFrom?.Trim() ?? string.Empty,
                    CustomerName = APInvoiceViewModel.CustomerName?.Trim() ?? string.Empty,
                    ArInvoiceId = !string.IsNullOrEmpty(APInvoiceViewModel.ArInvoiceId?.Trim()) ? Convert.ToInt64(APInvoiceViewModel.ArInvoiceId?.Trim()) : 0,
                    ArInvoiceNo = APInvoiceViewModel.ArInvoiceNo?.Trim() ?? string.Empty,
                    CreateById = headerViewModel.UserId,
                    EditById = headerViewModel.UserId,
                    EditDate = DateTime.Now,
                };

                // Details Mapping
                var APInvoiceDtEntities = APInvoiceViewModel.data_details?.Select(item => new ApInvoiceDt
                {
                    InvoiceId = item.InvoiceId != null ? Convert.ToInt64(item.InvoiceId) : 0,
                    InvoiceNo = item.InvoiceNo,
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
                    PurchaseOrderId = Convert.ToInt64(item.PurchaseOrderId),
                    PurchaseOrderNo = item.PurchaseOrderNo?.Trim() ?? string.Empty,
                    SupplyDate = string.IsNullOrEmpty(item.SupplyDate) ? (DateTime?)null : DateHelperStatic.ParseClientDate(item.SupplyDate),
                    CustomerName = item.CustomerName?.Trim() ?? string.Empty,
                    CustInvoiceNo = item.CustInvoiceNo?.Trim() ?? string.Empty,
                    ArInvoiceId = Convert.ToInt64(item.ArInvoiceId),
                    ArInvoiceNo = item.ArInvoiceNo?.Trim() ?? string.Empty,
                    EditVersion = item.EditVersion,
                }).ToList();

                var sqlResponse = await _APInvoiceService.SaveAPInvoiceAsync(
                    headerViewModel.RegId,
                    headerViewModel.CompanyId,
                    APInvoiceEntity,
                    APInvoiceDtEntities,
                    headerViewModel.UserId);

                if (sqlResponse.Result > 0)
                {
                    var customerModel = await _APInvoiceService.GetAPInvoiceByIdNoAsync(
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

        [HttpPost, Route("DeleteAPInvoice")]
        [Authorize]
        public async Task<ActionResult<APInvoiceViewModel>> DeleteAPInvoice(DeleteViewModel deleteViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AP, (Int32)E_AP.Invoice, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                if (!userGroupRight.IsDelete)
                    return Forbid("You do not have permission to delete");

                if (string.IsNullOrEmpty(deleteViewModel.DocumentId))
                    return NotFound("Data not found");

                var sqlResponse = await _APInvoiceService.DeleteAPInvoiceAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    Convert.ToInt64(deleteViewModel.DocumentId),
                    deleteViewModel.CancelRemarks, headerViewModel.UserId
                );

                if (sqlResponse.Result > 0)
                {
                    var customerModel = await _APInvoiceService.GetAPInvoiceByIdNoAsync(
                        headerViewModel.RegId, headerViewModel.CompanyId,
                        Convert.ToInt64(deleteViewModel.DocumentId), string.Empty, headerViewModel.UserId
                    );

                    return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = customerModel });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteAPInvoice: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Internal Server Error");
            }
        }

        [HttpGet, Route("GetHistoryAPInvoicebyId/{InvoiceId}")]
        [Authorize]
        public async Task<ActionResult<APInvoiceViewModel>> GetHistoryAPInvoicebyId(string InvoiceId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (InvoiceId == null || InvoiceId == "" || InvoiceId == "0")
                    return NoContent();

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AP, (Int32)E_AP.Invoice, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var APInvoiceViewModel = await _APInvoiceService.GetHistoryAPInvoiceByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(string.IsNullOrEmpty(InvoiceId) ? 0 : InvoiceId?.Trim()), string.Empty, headerViewModel.UserId);

                if (APInvoiceViewModel == null)
                    return StatusCode(StatusCodes.Status200OK, new SqlResponse { Result = 0, Message = "Data does not exist", Data = null });

                return Ok(new SqlResponse { Result = 1, Message = "Success", Data = APInvoiceViewModel });
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