﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Accounts.AR;
using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AR;
using AHHA.Core.Models.Account.AR;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Accounts.AR
{
    [Route("api/Account")]
    [ApiController]
    public class ARInvoiceController : BaseController

    {
        private readonly IARInvoiceService _ARInvoiceService;
        private readonly ILogger<ARInvoiceController> _logger;

        public ARInvoiceController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<ARInvoiceController> logger, IARInvoiceService ARInvoiceService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _ARInvoiceService = ARInvoiceService;
        }

        //All ARINVOICE LIST
        [HttpGet, Route("GetARInvoice")]
        [Authorize]
        public async Task<ActionResult> GetARInvoice([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.Invoice, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _ARInvoiceService.GetARInvoiceListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

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
                 "Error retrieving data from the database");
            }
        }

        //GET ONE ARINVOICE BY INVOICEID
        [HttpGet, Route("GetARInvoicebyId/{InvoiceId}")]
        [Authorize]
        public async Task<ActionResult<ARInvoiceViewModel>> GetARInvoiceById(string InvoiceId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.Invoice, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var ARInvoiceViewModel = await _ARInvoiceService.GetARInvoiceByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(string.IsNullOrEmpty(InvoiceId) ? 0 : InvoiceId.Trim()), string.Empty, headerViewModel.UserId);

                        if (ARInvoiceViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, ARInvoiceViewModel);
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

        //GET ONE ARINVOICE BY INVOICEID
        [HttpGet, Route("GetARInvoicebyNo/{InvoiceNo}")]
        [Authorize]
        public async Task<ActionResult<ARInvoiceViewModel>> GetARInvoiceByNo(string InvoiceNo, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.Invoice, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var ARInvoiceViewModel = await _ARInvoiceService.GetARInvoiceByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, 0, InvoiceNo, headerViewModel.UserId);

                        if (ARInvoiceViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, ARInvoiceViewModel);
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

        //SAVE ONE ARINVOICE BY INVOICEID
        [HttpPost, Route("SaveARInvoice")]
        [Authorize]
        public async Task<ActionResult<ARInvoiceViewModel>> SaveARInvoice(ARInvoiceViewModel aRInvoiceViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.Invoice, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate || userGroupRight.IsEdit)
                        {
                            if (aRInvoiceViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            //Header Data Mapping
                            var ARInvoiceEntity = new ArInvoiceHd
                            {
                                CompanyId = headerViewModel.CompanyId,
                                InvoiceId = Convert.ToInt64(string.IsNullOrEmpty(aRInvoiceViewModel.InvoiceId) ? 0 : aRInvoiceViewModel.InvoiceId.Trim()),
                                InvoiceNo = aRInvoiceViewModel.InvoiceNo,
                                ReferenceNo = aRInvoiceViewModel.ReferenceNo == null ? string.Empty : aRInvoiceViewModel.ReferenceNo,
                                TrnDate = aRInvoiceViewModel.TrnDate,
                                AccountDate = aRInvoiceViewModel.AccountDate,
                                DeliveryDate = aRInvoiceViewModel.DeliveryDate,
                                DueDate = aRInvoiceViewModel.DueDate,
                                CustomerId = aRInvoiceViewModel.CustomerId,
                                CurrencyId = aRInvoiceViewModel.CurrencyId,
                                ExhRate = aRInvoiceViewModel.ExhRate == null ? 0 : aRInvoiceViewModel.ExhRate,
                                CtyExhRate = aRInvoiceViewModel.CtyExhRate == null ? 0 : aRInvoiceViewModel.CtyExhRate,
                                CreditTermId = aRInvoiceViewModel.CreditTermId,
                                BankId = aRInvoiceViewModel.BankId == null ? 0 : aRInvoiceViewModel.BankId,
                                TotAmt = aRInvoiceViewModel.TotAmt == null ? 0 : aRInvoiceViewModel.TotAmt,
                                TotLocalAmt = aRInvoiceViewModel.TotLocalAmt == null ? 0 : aRInvoiceViewModel.TotLocalAmt,
                                TotCtyAmt = aRInvoiceViewModel.TotCtyAmt == null ? 0 : aRInvoiceViewModel.TotCtyAmt,
                                GstClaimDate = DateOnly.FromDateTime(aRInvoiceViewModel.GstClaimDate),
                                GstAmt = aRInvoiceViewModel.GstAmt == null ? 0 : aRInvoiceViewModel.GstAmt,
                                GstLocalAmt = aRInvoiceViewModel.GstLocalAmt == null ? 0 : aRInvoiceViewModel.GstLocalAmt,
                                GstCtyAmt = aRInvoiceViewModel.GstCtyAmt == null ? 0 : aRInvoiceViewModel.GstCtyAmt,
                                TotAmtAftGst = aRInvoiceViewModel.TotAmtAftGst == null ? 0 : aRInvoiceViewModel.TotAmtAftGst,
                                TotLocalAmtAftGst = aRInvoiceViewModel.TotLocalAmtAftGst == null ? 0 : aRInvoiceViewModel.TotLocalAmtAftGst,
                                TotCtyAmtAftGst = aRInvoiceViewModel.TotCtyAmtAftGst == null ? 0 : aRInvoiceViewModel.TotCtyAmtAftGst,
                                BalAmt = aRInvoiceViewModel.BalAmt == null ? 0 : aRInvoiceViewModel.BalAmt,
                                BalLocalAmt = aRInvoiceViewModel.BalLocalAmt == null ? 0 : aRInvoiceViewModel.BalLocalAmt,
                                PayAmt = aRInvoiceViewModel.PayAmt == null ? 0 : aRInvoiceViewModel.PayAmt,
                                PayLocalAmt = aRInvoiceViewModel.PayLocalAmt == null ? 0 : aRInvoiceViewModel.PayLocalAmt,
                                ExGainLoss = aRInvoiceViewModel.ExGainLoss == null ? 0 : aRInvoiceViewModel.ExGainLoss,
                                SalesOrderId = aRInvoiceViewModel.SalesOrderId == null ? 0 : aRInvoiceViewModel.SalesOrderId,
                                SalesOrderNo = aRInvoiceViewModel.SalesOrderNo == null ? string.Empty : aRInvoiceViewModel.SalesOrderNo,
                                OperationId = aRInvoiceViewModel.OperationId == null ? 0 : aRInvoiceViewModel.OperationId,
                                OperationNo = aRInvoiceViewModel.OperationNo == null ? string.Empty : aRInvoiceViewModel.OperationNo,
                                Remarks = aRInvoiceViewModel.Remarks == null ? string.Empty : aRInvoiceViewModel.Remarks,
                                Address1 = aRInvoiceViewModel.Address1 == null ? string.Empty : aRInvoiceViewModel.Address1,
                                Address2 = aRInvoiceViewModel.Address2 == null ? string.Empty : aRInvoiceViewModel.Address2,
                                Address3 = aRInvoiceViewModel.Address3 == null ? string.Empty : aRInvoiceViewModel.Address3,
                                Address4 = aRInvoiceViewModel.Address4 == null ? string.Empty : aRInvoiceViewModel.Address4,
                                PinCode = aRInvoiceViewModel.PinCode == null ? string.Empty : aRInvoiceViewModel.PinCode,
                                CountryId = aRInvoiceViewModel.CountryId,
                                PhoneNo = aRInvoiceViewModel.PhoneNo == null ? string.Empty : aRInvoiceViewModel.PhoneNo,
                                FaxNo = aRInvoiceViewModel.FaxNo == null ? string.Empty : aRInvoiceViewModel.FaxNo,
                                ContactName = aRInvoiceViewModel.ContactName == null ? string.Empty : aRInvoiceViewModel.ContactName,
                                MobileNo = aRInvoiceViewModel.MobileNo == null ? string.Empty : aRInvoiceViewModel.MobileNo,
                                EmailAdd = aRInvoiceViewModel.EmailAdd == null ? string.Empty : aRInvoiceViewModel.EmailAdd,
                                ModuleFrom = aRInvoiceViewModel.ModuleFrom == null ? string.Empty : aRInvoiceViewModel.ModuleFrom,
                                SupplierName = aRInvoiceViewModel.SupplierName == null ? string.Empty : aRInvoiceViewModel.SupplierName,
                                SuppInvoiceNo = aRInvoiceViewModel.SuppInvoiceNo == null ? string.Empty : aRInvoiceViewModel.SuppInvoiceNo,
                                APInvoiceId = aRInvoiceViewModel.APInvoiceId,
                                APInvoiceNo = aRInvoiceViewModel.APInvoiceNo == null ? string.Empty : aRInvoiceViewModel.APInvoiceNo,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            //Details Table data mapping
                            var arInvoiceDtEntities = new List<ArInvoiceDt>();

                            if (aRInvoiceViewModel.data_details != null)
                            {
                                foreach (var item in aRInvoiceViewModel.data_details)
                                {
                                    var arInvoiceDtEntity = new ArInvoiceDt
                                    {
                                        InvoiceId = Convert.ToInt64(string.IsNullOrEmpty(item.InvoiceId) ? 0 : item.InvoiceId.Trim()),
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
                                        TotCurAmt = item.TotCurAmt,
                                        Remarks = item.Remarks,
                                        GstId = item.GstId,
                                        GstPercentage = item.GstPercentage,
                                        GstAmt = item.GstAmt,
                                        GstLocalAmt = item.GstLocalAmt,
                                        GstCurAmt = item.GstCurAmt,
                                        DeliveryDate = item.DeliveryDate,
                                        DepartmentId = item.DepartmentId,
                                        EmployeeId = item.EmployeeId,
                                        PortId = item.PortId,
                                        VesselId = item.VesselId,
                                        BargeId = item.BargeId,
                                        VoyageId = item.VoyageId,
                                        OperationId = item.OperationId,
                                        OperationNo = item.OperationNo,
                                        OPRefNo = item.OPRefNo,
                                        SalesOrderId = item.SalesOrderId,
                                        SalesOrderNo = item.SalesOrderNo,
                                        SupplyDate = item.SupplyDate,
                                        SupplierName = item.SupplierName,
                                        SuppInvoiceNo = item.SuppInvoiceNo,
                                        APInvoiceId = item.APInvoiceId,
                                        APInvoiceNo = item.APInvoiceNo,
                                    };

                                    arInvoiceDtEntities.Add(arInvoiceDtEntity);
                                }
                            }

                            var sqlResponce = await _ARInvoiceService.SaveARInvoiceAsync(headerViewModel.RegId, headerViewModel.CompanyId, ARInvoiceEntity, arInvoiceDtEntities, headerViewModel.UserId);

                            if (sqlResponce.Result > 0)
                            {
                                var customerModel = await _ARInvoiceService.GetARInvoiceByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, sqlResponce.Result, string.Empty, headerViewModel.UserId);

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
                    "Error creating new ARInvoice record");
            }
        }

        [HttpDelete, Route("DeleteARInvoice/{InvoiceId}/{CancelRemarks}")]
        [Authorize]
        public async Task<ActionResult<ArInvoiceHd>> DeleteARInvoice(string InvoiceId, string CancelRemarks, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.Invoice, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var sqlResponce = await _ARInvoiceService.DeleteARInvoiceAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(string.IsNullOrEmpty(InvoiceId) ? 0 : InvoiceId.Trim()), CancelRemarks, headerViewModel.UserId);

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