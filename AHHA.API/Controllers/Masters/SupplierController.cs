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
    public class SupplierController : BaseController
    {
        private readonly ISupplierService _SupplierService;
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<SupplierController> logger, ISupplierService SupplierService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _SupplierService = SupplierService;
        }

        [HttpGet, Route("GetSupplier")]
        [Authorize]
        public async Task<ActionResult> GetSupplier([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var SupplierData = await _SupplierService.GetSupplierListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (SupplierData == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, SupplierData);
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

        [HttpGet, Route("GetSupplierbyid/{SupplierId}")]
        [Authorize]
        public async Task<ActionResult<SupplierViewModel>> GetSupplierById(Int16 SupplierId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var supplierViewModel = await _SupplierService.GetSupplierByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierId, headerViewModel.UserId);

                        if (supplierViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, supplierViewModel);
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

        [HttpGet, Route("GetSupplierbycode/{SupplierCode}")]
        [Authorize]
        public async Task<ActionResult<SupplierViewModel>> GetSupplierByCode(string SupplierCode, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var supplierViewModel = await _SupplierService.GetSupplierByCodeAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierCode, headerViewModel.UserId);

                        if (supplierViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, supplierViewModel);
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

        [HttpPost, Route("SaveSupplier")]
        [Authorize]
        public async Task<ActionResult<SupplierViewModel>> SaveSupplier(SupplierViewModel supplierViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (supplierViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var SupplierEntity = new M_Supplier
                            {
                                SupplierId = supplierViewModel.SupplierId,
                                CompanyId = headerViewModel.CompanyId,
                                SupplierCode = supplierViewModel.SupplierCode == null ? string.Empty : supplierViewModel.SupplierCode,
                                SupplierName = supplierViewModel.SupplierName == null ? string.Empty : supplierViewModel.SupplierName,
                                SupplierOtherName = supplierViewModel.SupplierOtherName == null ? string.Empty : supplierViewModel.SupplierOtherName,
                                SupplierShortName = supplierViewModel.SupplierShortName == null ? string.Empty : supplierViewModel.SupplierShortName,
                                SupplierRegNo = supplierViewModel.SupplierRegNo == null ? string.Empty : supplierViewModel.SupplierRegNo,
                                CurrencyId = supplierViewModel.CurrencyId == null ? Convert.ToInt16(0) : supplierViewModel.CurrencyId,
                                CreditTermId = supplierViewModel.CreditTermId == null ? Convert.ToInt16(0) : supplierViewModel.CreditTermId,
                                ParentSupplierId = supplierViewModel.ParentSupplierId == null ? 0 : supplierViewModel.ParentSupplierId,
                                IsCustomer = supplierViewModel.IsCustomer == null ? false : supplierViewModel.IsCustomer,
                                IsVendor = supplierViewModel.IsVendor == null ? false : supplierViewModel.IsVendor,
                                IsTrader = supplierViewModel.IsTrader == null ? false : supplierViewModel.IsTrader,
                                IsSupplier = supplierViewModel.IsSupplier == null ? false : supplierViewModel.IsSupplier,
                                Remarks = supplierViewModel.Remarks == null ? string.Empty : supplierViewModel.Remarks,
                                IsActive = supplierViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var createdSupplier = await _SupplierService.SaveSupplierAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdSupplier);
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
                    "Error creating new Supplier record");
            }
        }

        [HttpDelete, Route("DeleteSupplier/{SupplierId}")]
        [Authorize]
        public async Task<ActionResult<SupplierViewModel>> DeleteSupplier(Int16 SupplierId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int32)E_Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var SupplierToDelete = await _SupplierService.GetSupplierByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierId, headerViewModel.UserId);

                            if (SupplierToDelete == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var sqlResponce = await _SupplierService.DeleteSupplierAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierToDelete, headerViewModel.UserId);

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