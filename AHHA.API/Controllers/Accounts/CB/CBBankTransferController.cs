﻿using AHHA.API.Controllers;
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
    public class CBBankTransferController : BaseController
    {
        private readonly ICBBankTransferService _CBBankTransferService;
        private readonly ILogger<CBBankTransferController> _logger;

        public CBBankTransferController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CBBankTransferController> logger, ICBBankTransferService CBBankTransferService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CBBankTransferService = CBBankTransferService;
        }

        [HttpGet, Route("GetCBBankTransfer")]
        [Authorize]
        public async Task<ActionResult> GetCBBankTransfer([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.CB, (Int32)E_CB.CBBankTransfer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _CBBankTransferService.GetCBBankTransferListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

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
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpGet, Route("GetCBBankTransferbyId/{TransferId}")]
        [Authorize]
        public async Task<ActionResult<CBBankTransferViewModel>> GetCBBankTransferById(string TransferId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.CB, (Int32)E_CB.CBBankTransfer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var CBBankTransferViewModel = await _CBBankTransferService.GetCBBankTransferByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(string.IsNullOrEmpty(TransferId) ? 0 : TransferId.Trim()), string.Empty, headerViewModel.UserId);

                        if (CBBankTransferViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, CBBankTransferViewModel);
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
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpGet, Route("GetCBBankTransferbyNo/{TransferNo}")]
        [Authorize]
        public async Task<ActionResult<CBBankTransferViewModel>> GetCBBankTransferByNo(string TransferNo, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.CB, (Int32)E_CB.CBBankTransfer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var CBBankTransferViewModel = await _CBBankTransferService.GetCBBankTransferByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, 0, TransferNo, headerViewModel.UserId);

                        if (CBBankTransferViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, CBBankTransferViewModel);
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
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpPost, Route("SaveCBBankTransfer")]
        [Authorize]
        public async Task<ActionResult<CBBankTransferViewModel>> SaveCBBankTransfer(CBBankTransferViewModel cBBankTransferViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.CB, (Int32)E_CB.CBBankTransfer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate || userGroupRight.IsEdit)
                        {
                            if (cBBankTransferViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var CBBankTransferEntity = new CBBankTransfer
                            {
                                CompanyId = headerViewModel.CompanyId,
                                TransferId = Convert.ToInt64(string.IsNullOrEmpty(cBBankTransferViewModel.TransferId) ? "0" : cBBankTransferViewModel.TransferId.Trim()),
                                TransferNo = cBBankTransferViewModel.TransferNo?.Trim() ?? string.Empty,
                                ReferenceNo = cBBankTransferViewModel.ReferenceNo?.Trim() ?? string.Empty,
                                TrnDate = DateHelperStatic.ParseClientDate(cBBankTransferViewModel.TrnDate),
                                AccountDate = DateHelperStatic.ParseClientDate(cBBankTransferViewModel.AccountDate),
                                FromBankId = Convert.ToInt16(cBBankTransferViewModel.FromBankId),
                                FromCurrencyId = cBBankTransferViewModel.FromCurrencyId,
                                FromExhRate = cBBankTransferViewModel.FromExhRate,
                                PaymentTypeId = Convert.ToInt16(cBBankTransferViewModel.PaymentTypeId),
                                ChequeNo = cBBankTransferViewModel.ChequeNo?.Trim() ?? string.Empty,
                                ChequeDate = DateHelperStatic.ParseClientDate(cBBankTransferViewModel.ChequeDate),
                                FromBankChgAmt = cBBankTransferViewModel.FromBankChgAmt,
                                FromBankChgLocalAmt = cBBankTransferViewModel.FromBankChgLocalAmt,
                                FromTotAmt = cBBankTransferViewModel.FromTotAmt,
                                FromTotLocalAmt = cBBankTransferViewModel.FromTotLocalAmt,
                                ToBankId = Convert.ToInt16(cBBankTransferViewModel.ToBankId),
                                ToExhRate = cBBankTransferViewModel.ToExhRate,
                                ToBankChgAmt = cBBankTransferViewModel.ToBankChgAmt,
                                ToBankChgLocalAmt = cBBankTransferViewModel.ToBankChgLocalAmt,
                                ToTotAmt = cBBankTransferViewModel.ToTotAmt,
                                ToTotLocalAmt = cBBankTransferViewModel.ToTotLocalAmt,
                                Remarks = cBBankTransferViewModel.Remarks?.Trim() ?? string.Empty,
                                PayeeTo = cBBankTransferViewModel.PayeeTo?.Trim() ?? string.Empty,
                                ModuleFrom = cBBankTransferViewModel.ModuleFrom?.Trim() ?? string.Empty,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponse = await _CBBankTransferService.SaveCBBankTransferAsync(headerViewModel.RegId, headerViewModel.CompanyId, CBBankTransferEntity, headerViewModel.UserId);

                            if (sqlResponse.Result > 0)
                            {
                                var customerModel = await _CBBankTransferService.GetCBBankTransferByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, sqlResponse.Result, string.Empty, headerViewModel.UserId);

                                return StatusCode(StatusCodes.Status202Accepted, customerModel);
                            }

                            return StatusCode(StatusCodes.Status202Accepted, sqlResponse);
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
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new CBBankTransfer record");
            }
        }

        [HttpPost, Route("DeleteCBBankTransfer")]
        [Authorize]
        public async Task<ActionResult<CBBankTransferViewModel>> DeleteCBBankTransfer(DeleteViewModel deleteViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.CB, (Int32)E_CB.CBBankTransfer, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            if (!string.IsNullOrEmpty(deleteViewModel.DocumentId))
                            {
                                var sqlResponse = await _CBBankTransferService.DeleteCBBankTransferAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(deleteViewModel.DocumentId), deleteViewModel.DocumentNo, deleteViewModel.CancelRemarks, headerViewModel.UserId);

                                if (sqlResponse.Result > 0)
                                {
                                    var customerModel = await _CBBankTransferService.GetCBBankTransferByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(deleteViewModel.DocumentId), string.Empty, headerViewModel.UserId);

                                    return StatusCode(StatusCodes.Status202Accepted, customerModel);
                                }

                                return StatusCode(StatusCodes.Status204NoContent, sqlResponse);
                            }
                            else
                            {
                                return NotFound(GenrateMessage.datanotfound);
                            }
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
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }
}