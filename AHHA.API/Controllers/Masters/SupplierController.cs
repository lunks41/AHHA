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
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, headerViewModel.UserId);

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
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var supplierViewModel = _mapper.Map<SupplierViewModel>(await _SupplierService.GetSupplierByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierId, headerViewModel.UserId));

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
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var supplierViewModel = _mapper.Map<SupplierViewModel>(await _SupplierService.GetSupplierByCodeAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierCode, headerViewModel.UserId));

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

        [HttpPost, Route("AddSupplier")]
        [Authorize]
        public async Task<ActionResult<SupplierViewModel>> CreateSupplier(SupplierViewModel supplierViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, headerViewModel.UserId);

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
                                SupplierCode = supplierViewModel.SupplierCode,
                                SupplierName = supplierViewModel.SupplierName,
                                SupplierOtherName = supplierViewModel.SupplierOtherName,
                                SupplierShortName = supplierViewModel.SupplierShortName,
                                SupplierRegNo = supplierViewModel.SupplierRegNo,
                                CurrencyId = supplierViewModel.CurrencyId,
                                CreditTermId = supplierViewModel.CreditTermId,
                                ParentSupplierId = supplierViewModel.ParentSupplierId,
                                IsCustomer = supplierViewModel.IsCustomer,
                                IsVendor = supplierViewModel.IsVendor,
                                IsTrader = supplierViewModel.IsTrader,
                                IsSupplier = supplierViewModel.IsSupplier,
                                Remarks = supplierViewModel.Remarks,
                                IsActive = supplierViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                            };

                            var createdSupplier = await _SupplierService.AddSupplierAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierEntity, headerViewModel.UserId);
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

        [HttpPut, Route("UpdateSupplier/{SupplierId}")]
        [Authorize]
        public async Task<ActionResult<SupplierViewModel>> UpdateSupplier(Int16 SupplierId, [FromBody] SupplierViewModel supplierViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (SupplierId != supplierViewModel.SupplierId)
                                return StatusCode(StatusCodes.Status400BadRequest, "data mismatch");

                            var SupplierToUpdate = await _SupplierService.GetSupplierByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierId, headerViewModel.UserId);

                            if (SupplierToUpdate == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var SupplierEntity = new M_Supplier
                            {
                                SupplierId = supplierViewModel.SupplierId,
                                CompanyId = headerViewModel.CompanyId,
                                SupplierCode = supplierViewModel.SupplierCode,
                                SupplierName = supplierViewModel.SupplierName,
                                SupplierOtherName = supplierViewModel.SupplierOtherName,
                                SupplierShortName = supplierViewModel.SupplierShortName,
                                SupplierRegNo = supplierViewModel.SupplierRegNo,
                                CurrencyId = supplierViewModel.CurrencyId,
                                CreditTermId = supplierViewModel.CreditTermId,
                                ParentSupplierId = supplierViewModel.ParentSupplierId,
                                IsCustomer = supplierViewModel.IsCustomer,
                                IsVendor = supplierViewModel.IsVendor,
                                IsTrader = supplierViewModel.IsTrader,
                                IsSupplier = supplierViewModel.IsSupplier,
                                Remarks = supplierViewModel.Remarks,
                                IsActive = supplierViewModel.IsActive,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponce = await _SupplierService.UpdateSupplierAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierEntity, headerViewModel.UserId);
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
                    "Error updating data");
            }
        }

        [HttpDelete, Route("DeleteSupplier/{SupplierId}")]
        [Authorize]
        public async Task<ActionResult<M_Supplier>> DeleteSupplier(Int16 SupplierId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, headerViewModel.UserId);

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