﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/Master")]
    [ApiController]
    public class SupplierBankController : BaseController
    {
        private readonly ISupplierBankService _SupplierBankService;
        private readonly ILogger<SupplierBankController> _logger;

        public SupplierBankController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<SupplierBankController> logger, ISupplierBankService SupplierBankService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _SupplierBankService = SupplierBankService;
        }

        //Get the Supplier Bank List
        [HttpGet, Route("getSupplierBankbySupplierid/{SupplierId}")]
        [Authorize]
        public async Task<ActionResult> GetSupplierBankBySupplierId(Int16 SupplierId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var SupplierBankViewModel = await _SupplierBankService.GetSupplierBankBySupplierIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierId, headerViewModel.UserId);

                        if (SupplierBankViewModel == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, SupplierBankViewModel);
                    }
                    else
                    {
                        return NotFound(GenerateMessage.AuthenticationFailed);
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
                    "Error retrieving data from the database");
            }
        }

        //Supplier Bank one record by using Bankid
        [HttpGet, Route("getSupplierBankbyid/{SupplierId}/{SupplierBankId}")]
        [Authorize]
        public async Task<ActionResult<SupplierBankViewModel>> GetSupplierBankById(Int32 SupplierId, Int16 SupplierBankId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var SupplierBankViewModel = await _SupplierBankService.GetSupplierBankByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierId, SupplierBankId, headerViewModel.UserId);

                        if (SupplierBankViewModel == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, SupplierBankViewModel);
                    }
                    else
                    {
                        return NotFound(GenerateMessage.AuthenticationFailed);
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
                    "Error retrieving data from the database");
            }
        }

        [HttpPost, Route("SaveSupplierBank")]
        [Authorize]
        public async Task<ActionResult<SupplierBankViewModel>> SaveSupplierBank(SupplierBankViewModel SupplierBankViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (SupplierBankViewModel == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var SupplierBankEntity = new M_SupplierBank
                            {
                                SupplierId = SupplierBankViewModel.SupplierId,
                                SupplierBankId = SupplierBankViewModel.SupplierBankId,
                                BankId = SupplierBankViewModel.BankId,
                                BranchName = SupplierBankViewModel.BranchName == null ? string.Empty : SupplierBankViewModel.BranchName.Trim(),
                                AccountNo = SupplierBankViewModel.AccountNo == null ? string.Empty : SupplierBankViewModel.AccountNo.Trim(),
                                SwiftCode = SupplierBankViewModel.SwiftCode == null ? string.Empty : SupplierBankViewModel.SwiftCode.Trim(),
                                OtherCode = SupplierBankViewModel.OtherCode == null ? string.Empty : SupplierBankViewModel.OtherCode.Trim(),
                                Address1 = SupplierBankViewModel.Address1 == null ? string.Empty : SupplierBankViewModel.Address1.Trim(),
                                Address2 = SupplierBankViewModel.Address2 == null ? string.Empty : SupplierBankViewModel.Address2.Trim(),
                                Address3 = SupplierBankViewModel.Address3 == null ? string.Empty : SupplierBankViewModel.Address3.Trim(),
                                Address4 = SupplierBankViewModel.Address4 == null ? string.Empty : SupplierBankViewModel.Address4.Trim(),
                                PinCode = SupplierBankViewModel.PinCode == null ? string.Empty : SupplierBankViewModel.PinCode.Trim(),
                                Remarks1 = SupplierBankViewModel.Remarks1 == null ? string.Empty : SupplierBankViewModel.Remarks1.Trim(),
                                Remarks2 = SupplierBankViewModel.Remarks2 == null ? string.Empty : SupplierBankViewModel.Remarks2.Trim(),
                                CountryId = SupplierBankViewModel.CountryId,
                                IsDefault = SupplierBankViewModel.IsDefault,
                                IsActive = SupplierBankViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponse = await _SupplierBankService.SaveSupplierBankAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierBankEntity, headerViewModel.UserId);

                            if (sqlResponse.Result > 0)
                            {
                                var SupplierModel = await _SupplierBankService.GetSupplierBankByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierBankViewModel.SupplierId, Convert.ToInt16(sqlResponse.Result), headerViewModel.UserId);

                                return StatusCode(StatusCodes.Status202Accepted, SupplierModel);
                            }

                            return StatusCode(StatusCodes.Status204NoContent, sqlResponse);
                        }
                        else
                        {
                            return NotFound(GenerateMessage.AuthenticationFailed);
                        }
                    }
                    else
                    {
                        return NotFound(GenerateMessage.AuthenticationFailed);
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
                    "Error creating new SupplierBank record");
            }
        }

        [HttpDelete, Route("DeleteSupplierBank/{SupplierId}/{SupplierBankId}")]
        [Authorize]
        public async Task<ActionResult<M_SupplierBank>> DeleteSupplierBank(Int32 SupplierId, Int16 SupplierBankId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var sqlResponse = await _SupplierBankService.DeleteSupplierBankAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierId, SupplierBankId, headerViewModel.UserId);

                            return StatusCode(StatusCodes.Status202Accepted, sqlResponse);
                        }
                        else
                        {
                            return NotFound(GenerateMessage.AuthenticationFailed);
                        }
                    }
                    else
                    {
                        return NotFound(GenerateMessage.AuthenticationFailed);
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