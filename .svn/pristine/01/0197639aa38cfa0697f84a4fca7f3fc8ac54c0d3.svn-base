using AHHA.API.Controllers;
using AHHA.Application.IServices;
using AHHA.Application.IServices.Accounts.CB;
using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.CB;
using AHHA.Core.Helper;
using AHHA.Core.Models.Account;
using AHHA.Core.Models.Account.CB;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.CBI.Controllers.Accounts.CB
{
    [Route("api/Account")]
    [ApiController]
    public class CBBankReconController : BaseController
    {
        private readonly ICBBankReconService _CBBankReconService;
        private readonly ILogger<CBBankReconController> _logger;

        public CBBankReconController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CBBankReconController> logger, ICBBankReconService CBBankReconService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CBBankReconService = CBBankReconService;
        }

        [HttpGet, Route("GetCBBankRecon")]
        [Authorize]
        public async Task<ActionResult> GetCBBankRecon([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.CB, (Int32)E_CB.CBBankRecon, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var cacheData = await _CBBankReconService.GetCBBankReconListAsync(
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
                _logger.LogError($"Error in GetCBBankRecon: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpGet, Route("GetCBBankReconbyIdNo/{ReconId}/{ReconNo}")]
        [Authorize]
        public async Task<ActionResult<CBBankReconHdViewModel>> GetCBBankReconByIdNo(string ReconId, string ReconNo, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if ((ReconId == "0" && ReconNo == ""))
                    return NotFound("Invalid Id");

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.CB, (Int32)E_CB.CBBankRecon, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var CBBankReconHdViewModel = await _CBBankReconService.GetCBBankReconByIdNoAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    Convert.ToInt64(string.IsNullOrEmpty(ReconId) ? 0 : ReconId?.Trim()),
                    ReconNo, headerViewModel.UserId
                );

                if (CBBankReconHdViewModel == null)
                    return StatusCode(StatusCodes.Status200OK, new SqlResponse { Result = 0, Message = "Data does not exist", Data = null });

                return Ok(new SqlResponse { Result = 1, Message = "Success", Data = CBBankReconHdViewModel });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetCBBankReconById: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpPost, Route("SaveCBBankRecon")]
        [Authorize]
        public async Task<ActionResult<CBBankReconHdViewModel>> SaveCBBankRecon(
            CBBankReconHdViewModel CBBankReconHdViewModel,
            [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (CBBankReconHdViewModel == null)
                    return NotFound(GenrateMessage.datanotfound);

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(
                    headerViewModel.RegId,
                    headerViewModel.CompanyId,
                    (short)E_Modules.CB,
                    (int)E_CB.CBBankRecon,
                    headerViewModel.UserId);

                if (userGroupRight == null || (!userGroupRight.IsCreate && !userGroupRight.IsEdit))
                    return NotFound(GenrateMessage.authenticationfailed);

                // Header Data Mapping
                var CBBankReconEntity = new CBBankReconHd
                {
                    CompanyId = headerViewModel.CompanyId,
                    ReconId = CBBankReconHdViewModel.ReconId != null ? Convert.ToInt64(CBBankReconHdViewModel.ReconId) : 0,
                    ReconNo = CBBankReconHdViewModel.ReconNo?.Trim() ?? string.Empty,
                    PrevReconId = CBBankReconHdViewModel.PrevReconId != null ? Convert.ToInt64(CBBankReconHdViewModel.PrevReconId) : 0,
                    PrevReconNo = CBBankReconHdViewModel.PrevReconNo?.Trim() ?? string.Empty,
                    ReferenceNo = CBBankReconHdViewModel.ReferenceNo?.Trim() ?? string.Empty,
                    TrnDate = DateHelperStatic.ParseClientDate(CBBankReconHdViewModel.TrnDate),
                    AccountDate = DateHelperStatic.ParseClientDate(CBBankReconHdViewModel.AccountDate),
                    BankId = CBBankReconHdViewModel.BankId,
                    CurrencyId = CBBankReconHdViewModel.CurrencyId,
                    FromDate = DateHelperStatic.ParseClientDate(CBBankReconHdViewModel.FromDate),
                    ToDate = DateHelperStatic.ParseClientDate(CBBankReconHdViewModel.ToDate),
                    TotAmt = CBBankReconHdViewModel.TotAmt,
                    OPBalAmt = CBBankReconHdViewModel.OPBalAmt,
                    CLBalAmt = CBBankReconHdViewModel.CLBalAmt,
                    Remarks = CBBankReconHdViewModel.Remarks?.Trim() ?? string.Empty,
                    CreateById = headerViewModel.UserId,
                    EditById = headerViewModel.UserId,
                    EditDate = DateTime.Now,
                };

                // Details Mapping
                var CBBankReconDtEntities = CBBankReconHdViewModel.data_details?.Select(item => new CBBankReconDt
                {
                    ReconId = item.ReconId != null ? Convert.ToInt64(item.ReconId) : 0,
                    ReconNo = item.ReconNo,
                    ItemNo = item.ItemNo,
                    IsSel = item.IsSel,
                    ModuleId = item.ModuleId,
                    TransactionId = item.TransactionId,
                    DocumentId = item.DocumentId != null ? Convert.ToInt64(item.DocumentId) : 0,
                    DocumentNo = item.DocumentNo,
                    DocReferenceNo = item.DocReferenceNo,
                    AccountDate = DateHelperStatic.ParseClientDate(item.AccountDate),
                    PaymentTypeId = item.PaymentTypeId,
                    ChequeNo = item.ChequeNo,
                    ChequeDate = DateHelperStatic.ParseClientDate(item.ChequeDate),
                    CustomerId = item.CustomerId,
                    SupplierId = item.SupplierId,
                    GLId = item.GLId,
                    IsDebit = item.IsDebit,
                    ExhRate = item.ExhRate,
                    TotAmt = item.TotAmt,
                    TotLocalAmt = item.TotLocalAmt,
                    PaymentFromTo = item.PaymentFromTo,
                    Remarks = item.Remarks?.Trim() ?? string.Empty,
                    EditVersion = item.EditVersion,
                }).ToList();

                var sqlResponse = await _CBBankReconService.SaveCBBankReconAsync(
                    headerViewModel.RegId,
                    headerViewModel.CompanyId,
                    CBBankReconEntity,
                    CBBankReconDtEntities,
                    headerViewModel.UserId);

                if (sqlResponse.Result > 0)
                {
                    var customerModel = await _CBBankReconService.GetCBBankReconByIdNoAsync(
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

        [HttpPost, Route("DeleteCBBankRecon")]
        [Authorize]
        public async Task<ActionResult<CBBankReconHdViewModel>> DeleteCBBankRecon(DeleteViewModel deleteViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.CB, (Int32)E_CB.CBBankRecon, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                if (!userGroupRight.IsDelete)
                    return Forbid("You do not have permission to delete");

                if (string.IsNullOrEmpty(deleteViewModel.DocumentId))
                    return NotFound("Data not found");

                var sqlResponse = await _CBBankReconService.DeleteCBBankReconAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    Convert.ToInt64(deleteViewModel.DocumentId),
                    deleteViewModel.CancelRemarks, headerViewModel.UserId
                );

                if (sqlResponse.Result > 0)
                {
                    var customerModel = await _CBBankReconService.GetCBBankReconByIdNoAsync(
                        headerViewModel.RegId, headerViewModel.CompanyId,
                        Convert.ToInt64(deleteViewModel.DocumentId), string.Empty, headerViewModel.UserId
                    );

                    return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = customerModel });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteCBBankRecon: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Internal Server Error");
            }
        }

        [HttpGet, Route("GetHistoryCBBankReconbyId/{ReconId}")]
        [Authorize]
        public async Task<ActionResult<CBBankReconHdViewModel>> GetHistoryCBBankReconbyId(string ReconId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ReconId == null || ReconId == "" || ReconId == "0")
                    return NoContent();

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.CB, (Int32)E_CB.CBBankRecon, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var CBBankReconHdViewModel = await _CBBankReconService.GetHistoryCBBankReconByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(string.IsNullOrEmpty(ReconId) ? 0 : ReconId?.Trim()), string.Empty, headerViewModel.UserId);

                if (CBBankReconHdViewModel == null)
                    return StatusCode(StatusCodes.Status200OK, new SqlResponse { Result = 0, Message = "Data does not exist", Data = null });

                return Ok(new SqlResponse { Result = 1, Message = "Success", Data = CBBankReconHdViewModel });
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