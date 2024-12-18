﻿using AHHA.Application.IServices;
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

namespace AHHA.API.Controllers.Accounts.CB
{
    [Route("api/Account")]
    [ApiController]
    public class CBGeneralReceiptController : BaseController
    {
        private readonly ICBGeneralReceiptService _CBGeneralReceiptService;
        private readonly ILogger<CBGeneralReceiptController> _logger;

        public CBGeneralReceiptController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CBGeneralReceiptController> logger, ICBGeneralReceiptService CBGeneralReceiptService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CBGeneralReceiptService = CBGeneralReceiptService;
        }

        [HttpGet, Route("GetCBGeneralReceipt")]
        [Authorize]
        public async Task<ActionResult> GetCBGeneralReceipt([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.CB, (Int32)E_CB.CBReceipt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _CBGeneralReceiptService.GetCBGeneralReceiptListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

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
                 new SqlResponce { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpGet, Route("GetCBGeneralReceiptbyId/{ReceiptId}")]
        [Authorize]
        public async Task<ActionResult<CBGenReceiptHdViewModel>> GetCBGeneralReceiptById(string ReceiptId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.CB, (Int32)E_CB.CBReceipt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var CBGenReceiptHdViewModel = await _CBGeneralReceiptService.GetCBGeneralReceiptByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(string.IsNullOrEmpty(ReceiptId) ? 0 : ReceiptId.Trim()), string.Empty, headerViewModel.UserId);

                        if (CBGenReceiptHdViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, CBGenReceiptHdViewModel);
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
                    new SqlResponce { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpGet, Route("GetCBGeneralReceiptbyNo/{ReceiptNo}")]
        [Authorize]
        public async Task<ActionResult<CBGenReceiptHdViewModel>> GetCBGeneralReceiptByNo(string ReceiptNo, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.CB, (Int32)E_CB.CBReceipt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var CBGenReceiptHdViewModel = await _CBGeneralReceiptService.GetCBGeneralReceiptByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, 0, ReceiptNo, headerViewModel.UserId);

                        if (CBGenReceiptHdViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, CBGenReceiptHdViewModel);
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
                    new SqlResponce { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpPost, Route("SaveCBGeneralReceipt")]
        [Authorize]
        public async Task<ActionResult<CBGenReceiptHdViewModel>> SaveCBGeneralReceipt(CBGenReceiptHdViewModel cBGenReceiptHdViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.CB, (Int32)E_CB.CBReceipt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate || userGroupRight.IsEdit)
                        {
                            if (cBGenReceiptHdViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var CBGeneralReceiptEntity = new CBGenReceiptHd
                            {
                                CompanyId = headerViewModel.CompanyId,
                                ReceiptId = Convert.ToInt64(string.IsNullOrEmpty(cBGenReceiptHdViewModel.ReceiptId) ? 0 : cBGenReceiptHdViewModel.ReceiptId.Trim()),
                                ReceiptNo = cBGenReceiptHdViewModel.ReceiptNo,
                                ReferenceNo = cBGenReceiptHdViewModel.ReferenceNo == null ? string.Empty : cBGenReceiptHdViewModel.ReferenceNo,
                                TrnDate = DateHelperStatic.ParseClientDate(cBGenReceiptHdViewModel.TrnDate),
                                AccountDate = DateHelperStatic.ParseClientDate(cBGenReceiptHdViewModel.AccountDate),
                                BankId = Convert.ToInt16(cBGenReceiptHdViewModel.BankId == null ? 0 : cBGenReceiptHdViewModel.BankId),
                                CurrencyId = cBGenReceiptHdViewModel.CurrencyId,
                                ExhRate = cBGenReceiptHdViewModel.ExhRate == null ? 0 : cBGenReceiptHdViewModel.ExhRate,
                                CtyExhRate = cBGenReceiptHdViewModel.CtyExhRate == null ? 0 : cBGenReceiptHdViewModel.CtyExhRate,
                                PaymentTypeId = Convert.ToInt16(cBGenReceiptHdViewModel.PaymentTypeId == null ? 0 : cBGenReceiptHdViewModel.PaymentTypeId),
                                ChequeNo = cBGenReceiptHdViewModel.ChequeNo == null ? string.Empty : cBGenReceiptHdViewModel.ChequeNo,
                                ChequeDate = DateHelperStatic.ParseClientDate(cBGenReceiptHdViewModel.ChequeDate),
                                BankChgAmt = cBGenReceiptHdViewModel.BankChgAmt == null ? 0 : cBGenReceiptHdViewModel.BankChgAmt,
                                BankChgLocalAmt = cBGenReceiptHdViewModel.BankChgLocalAmt == null ? 0 : cBGenReceiptHdViewModel.BankChgLocalAmt,
                                TotAmt = cBGenReceiptHdViewModel.TotAmt == null ? 0 : cBGenReceiptHdViewModel.TotAmt,
                                TotLocalAmt = cBGenReceiptHdViewModel.TotLocalAmt == null ? 0 : cBGenReceiptHdViewModel.TotLocalAmt,
                                TotCtyAmt = cBGenReceiptHdViewModel.TotCtyAmt == null ? 0 : cBGenReceiptHdViewModel.TotCtyAmt,
                                GstClaimDate = DateHelperStatic.ParseClientDate(cBGenReceiptHdViewModel.GstClaimDate),
                                GstAmt = cBGenReceiptHdViewModel.GstAmt == null ? 0 : cBGenReceiptHdViewModel.GstAmt,
                                GstLocalAmt = cBGenReceiptHdViewModel.GstLocalAmt == null ? 0 : cBGenReceiptHdViewModel.GstLocalAmt,
                                GstCtyAmt = cBGenReceiptHdViewModel.GstCtyAmt == null ? 0 : cBGenReceiptHdViewModel.GstCtyAmt,
                                TotAmtAftGst = cBGenReceiptHdViewModel.TotAmtAftGst == null ? 0 : cBGenReceiptHdViewModel.TotAmtAftGst,
                                TotLocalAmtAftGst = cBGenReceiptHdViewModel.TotLocalAmtAftGst == null ? 0 : cBGenReceiptHdViewModel.TotLocalAmtAftGst,
                                TotCtyAmtAftGst = cBGenReceiptHdViewModel.TotCtyAmtAftGst == null ? 0 : cBGenReceiptHdViewModel.TotCtyAmtAftGst,
                                Remarks = cBGenReceiptHdViewModel.Remarks == null ? string.Empty : cBGenReceiptHdViewModel.Remarks,
                                PayeeTo = cBGenReceiptHdViewModel.PayeeTo == null ? string.Empty : cBGenReceiptHdViewModel.PayeeTo,
                                ModuleFrom = cBGenReceiptHdViewModel.ModuleFrom == null ? string.Empty : cBGenReceiptHdViewModel.ModuleFrom,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            //Details Table data mapping
                            var CBGeneralReceiptDtEntities = new List<CBGenReceiptDt>();

                            if (cBGenReceiptHdViewModel.data_details != null)
                            {
                                foreach (var item in cBGenReceiptHdViewModel.data_details)
                                {
                                    var CBGeneralReceiptDtEntity = new CBGenReceiptDt
                                    {
                                        ReceiptId = Convert.ToInt64(string.IsNullOrEmpty(item.ReceiptId) ? 0 : item.ReceiptId.Trim()),
                                        ReceiptNo = item.ReceiptNo,
                                        ItemNo = item.ItemNo,
                                        SeqNo = item.SeqNo,
                                        GLId = item.GLId,
                                        Remarks = item.Remarks,
                                        TotAmt = item.TotAmt,
                                        TotLocalAmt = item.TotLocalAmt,
                                        TotCtyAmt = item.TotCtyAmt,
                                        GstId = item.GstId,
                                        GstPercentage = item.GstPercentage,
                                        GstAmt = item.GstAmt,
                                        GstLocalAmt = item.GstLocalAmt,
                                        GstCtyAmt = item.GstCtyAmt,
                                        DepartmentId = item.DepartmentId,
                                        EmployeeId = item.EmployeeId,
                                        VesselId = item.VesselId,
                                        BargeId = item.BargeId,
                                        VoyageId = item.VoyageId,
                                    };

                                    CBGeneralReceiptDtEntities.Add(CBGeneralReceiptDtEntity);
                                }
                            }

                            var sqlResponce = await _CBGeneralReceiptService.SaveCBGeneralReceiptAsync(headerViewModel.RegId, headerViewModel.CompanyId, CBGeneralReceiptEntity, CBGeneralReceiptDtEntities, headerViewModel.UserId);

                            if (sqlResponce.Result > 0)
                            {
                                var customerModel = await _CBGeneralReceiptService.GetCBGeneralReceiptByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, sqlResponce.Result, string.Empty, headerViewModel.UserId);

                                return StatusCode(StatusCodes.Status202Accepted, customerModel);
                            }

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
                    "Error creating new CBGeneralReceipt record");
            }
        }

        [HttpPost, Route("DeleteCBGeneralReceipt")]
        [Authorize]
        public async Task<ActionResult<CBGenReceiptHdViewModel>> DeleteCBGeneralReceipt(DeleteViewModel deleteViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.CB, (Int32)E_CB.CBReceipt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            if (!string.IsNullOrEmpty(deleteViewModel.DocumentId))
                            {
                                var sqlResponce = await _CBGeneralReceiptService.DeleteCBGeneralReceiptAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(deleteViewModel.DocumentId), deleteViewModel.DocumentNo, deleteViewModel.CancelRemarks, headerViewModel.UserId);

                                if (sqlResponce.Result > 0)
                                {
                                    var customerModel = await _CBGeneralReceiptService.GetCBGeneralReceiptByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(deleteViewModel.DocumentId), string.Empty, headerViewModel.UserId);

                                    return StatusCode(StatusCodes.Status202Accepted, customerModel);
                                }

                                return StatusCode(StatusCodes.Status204NoContent, sqlResponce);
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
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }
}