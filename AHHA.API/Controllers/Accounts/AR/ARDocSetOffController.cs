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
    public class ARDocSetOffController : BaseController
    {
        private readonly IARDocSetOffService _ARDocSetOffService;
        private readonly ILogger<ARDocSetOffController> _logger;

        public ARDocSetOffController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<ARDocSetOffController> logger, IARDocSetOffService ARDocSetOffService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _ARDocSetOffService = ARDocSetOffService;
        }

        [HttpGet, Route("GetARDocSetOff")]
        [Authorize]
        public async Task<ActionResult> GetARDocSetOff([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.DocSetoff, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var cacheData = await _ARDocSetOffService.GetARDocSetOffListAsync(
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
                _logger.LogError($"Error in GetARDocSetOff: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpGet, Route("GetARDocSetOffbyIdNo/{SetoffId}/{SetoffNo}")]
        [Authorize]
        public async Task<ActionResult<ARDocSetOffViewModel>> GetARDocSetOffByIdNo(string SetoffId, string SetoffNo, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (SetoffId == "0" && SetoffNo == "")
                    return NotFound("Invalid Id");

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.DocSetoff, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var arDocSetOffViewModel = await _ARDocSetOffService.GetARDocSetOffByIdAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    Convert.ToInt64(string.IsNullOrEmpty(SetoffId) ? 0 : SetoffId?.Trim()),
                    SetoffNo, headerViewModel.UserId
                );

                if (arDocSetOffViewModel == null)
                    return StatusCode(StatusCodes.Status200OK, new SqlResponse { Result = 0, Message = "Data does not exist", Data = null });

                return Ok(new SqlResponse { Result = 1, Message = "Success", Data = arDocSetOffViewModel });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetARDocSetOffById: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        //SAVE ONE ARDocSetOff BY INVOICEID
        [HttpPost, Route("SaveARDocSetOff")]
        [Authorize]
        public async Task<ActionResult<ARDocSetOffViewModel>> SaveARDocSetOff(ARDocSetOffViewModel aRDocSetOffViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (aRDocSetOffViewModel == null)
                    return NotFound(GenerateMessage.DataNotFound);

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(
                    headerViewModel.RegId,
                    headerViewModel.CompanyId,
                    (short)E_Modules.AR,
                    (int)E_AR.DocSetoff,
                    headerViewModel.UserId);

                if (userGroupRight == null || (!userGroupRight.IsCreate && !userGroupRight.IsEdit))
                    return NotFound(GenerateMessage.AuthenticationFailed);

                //Header Data Mapping
                var ARDocSetOffEntity = new ArDocSetOffHd
                {
                    CompanyId = headerViewModel.CompanyId,
                    SetoffId = Convert.ToInt64(string.IsNullOrEmpty(aRDocSetOffViewModel.SetoffId) ? 0 : aRDocSetOffViewModel.SetoffId?.Trim()),
                    SetoffNo = aRDocSetOffViewModel.SetoffNo,
                    ReferenceNo = aRDocSetOffViewModel.ReferenceNo == null ? string.Empty : aRDocSetOffViewModel.ReferenceNo,
                    TrnDate = DateHelperStatic.ParseClientDate(aRDocSetOffViewModel.TrnDate),
                    AccountDate = DateHelperStatic.ParseClientDate(aRDocSetOffViewModel.AccountDate),
                    CustomerId = aRDocSetOffViewModel.CustomerId,
                    CurrencyId = aRDocSetOffViewModel.CurrencyId,
                    ExhRate = aRDocSetOffViewModel.ExhRate,
                    Remarks = aRDocSetOffViewModel.Remarks == null ? string.Empty : aRDocSetOffViewModel.Remarks,
                    BalanceAmt = aRDocSetOffViewModel.BalanceAmt,
                    AllocAmt = aRDocSetOffViewModel.AllocAmt,
                    UnAllocAmt = aRDocSetOffViewModel.UnAllocAmt,
                    ExhGainLoss = aRDocSetOffViewModel.ExhGainLoss,
                    ModuleFrom = aRDocSetOffViewModel.ModuleFrom == null ? string.Empty : aRDocSetOffViewModel.ModuleFrom,
                    CreateById = headerViewModel.UserId,
                    EditById = headerViewModel.UserId,
                    EditDate = DateTime.Now,
                };

                //Details Table data mapping
                var ARDocSetOffDtEntities = new List<ArDocSetOffDt>();

                if (aRDocSetOffViewModel.data_details != null)
                {
                    foreach (var item in aRDocSetOffViewModel.data_details)
                    {
                        var ARDocSetOffDtEntity = new ArDocSetOffDt
                        {
                            CompanyId = headerViewModel.CompanyId,
                            SetoffId = Convert.ToInt64(string.IsNullOrEmpty(item.SetoffId) ? 0 : item.SetoffId?.Trim()),
                            SetoffNo = item.SetoffNo,
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
                        };

                        ARDocSetOffDtEntities.Add(ARDocSetOffDtEntity);
                    }
                }

                var sqlResponse = await _ARDocSetOffService.SaveARDocSetOffAsync(
                   headerViewModel.RegId,
                   headerViewModel.CompanyId,
                   ARDocSetOffEntity,
                   ARDocSetOffDtEntities,
                   headerViewModel.UserId);

                if (sqlResponse.Result > 0)
                {
                    var customerModel = await _ARDocSetOffService.GetARDocSetOffByIdAsync(
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
                    "Error creating new ARDocSetOff record");
            }
        }

        [HttpPost, Route("DeleteARDocSetOff")]
        [Authorize]
        public async Task<ActionResult<ARDocSetOffViewModel>> DeleteARDocSetOff(DeleteViewModel deleteViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.DocSetoff, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                if (!userGroupRight.IsDelete)
                    return Forbid("You do not have permission to delete");

                if (string.IsNullOrEmpty(deleteViewModel.DocumentId))
                    return NotFound("Data not found");

                var sqlResponse = await _ARDocSetOffService.DeleteARDocSetOffAsync(
                    headerViewModel.RegId, headerViewModel.CompanyId,
                    Convert.ToInt64(deleteViewModel.DocumentId), deleteViewModel.DocumentNo,
                    deleteViewModel.CancelRemarks, headerViewModel.UserId
                );

                if (sqlResponse.Result > 0)
                {
                    var customerModel = await _ARDocSetOffService.GetARDocSetOffByIdAsync(
                        headerViewModel.RegId, headerViewModel.CompanyId,
                        Convert.ToInt64(deleteViewModel.DocumentId), string.Empty, headerViewModel.UserId
                    );

                    return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = customerModel });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteARDocSetOff: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Internal Server Error");
            }
        }

        [HttpGet, Route("GetHistoryARDocSetOffbyId/{SetoffId}")]
        [Authorize]
        public async Task<ActionResult<ARDocSetOffViewModel>> GetHistoryARDocSetOffbyId(string SetoffId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (SetoffId == null || SetoffId == "" || SetoffId == "0")
                    return NoContent();

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.DocSetoff, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                var arDocSetOffViewModel = await _ARDocSetOffService.GetHistoryARDocSetOffByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(string.IsNullOrEmpty(SetoffId) ? 0 : SetoffId?.Trim()), string.Empty, headerViewModel.UserId);

                if (arDocSetOffViewModel == null)
                    return StatusCode(StatusCodes.Status200OK, new SqlResponse { Result = 0, Message = "Data does not exist", Data = null });

                return Ok(new SqlResponse { Result = 1, Message = "Success", Data = arDocSetOffViewModel });
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