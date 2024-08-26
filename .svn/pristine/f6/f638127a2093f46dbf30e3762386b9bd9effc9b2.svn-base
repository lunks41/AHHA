using AHHA.Application.IServices;
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
    public class SupplierAddressController : BaseController
    {
        private readonly ISupplierAddressService _SupplierAddressService;
        private readonly ILogger<SupplierAddressController> _logger;

        public SupplierAddressController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<SupplierAddressController> logger, ISupplierAddressService SupplierAddressService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _SupplierAddressService = SupplierAddressService;
        }

        [HttpGet, Route("GetSupplierAddress")]
        [Authorize]
        public async Task<ActionResult> GetSupplierAddress([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _SupplierAddressService.GetSupplierAddressListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenrateMessage.authenticationfailed);

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

        [HttpGet, Route("GetSupplierAddressbyid/{SupplierAddressId}")]
        [Authorize]
        public async Task<ActionResult<SupplierAddressViewModel>> GetSupplierAddressById(Int16 SupplierAddressId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var supplierAddressViewModel = _mapper.Map<SupplierAddressViewModel>(await _SupplierAddressService.GetSupplierAddressByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierAddressId, headerViewModel.UserId));

                        if (supplierAddressViewModel == null)
                            return NotFound(GenrateMessage.authenticationfailed);

                        return StatusCode(StatusCodes.Status202Accepted, supplierAddressViewModel);
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

        [HttpPost, Route("AddSupplierAddress")]
        [Authorize]
        public async Task<ActionResult<SupplierAddressViewModel>> CreateSupplierAddress(SupplierAddressViewModel SupplierAddress, [FromHeader] HeaderViewModel headerViewModel)
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
                            if (SupplierAddress == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_SupplierAddress ID mismatch");

                            var SupplierAddressEntity = new M_SupplierAddress
                            {
                                SupplierId = SupplierAddress.SupplierId,
                                AddressId = SupplierAddress.AddressId,
                                Address1 = SupplierAddress.Address1,
                                Address2 = SupplierAddress.Address2,
                                Address3 = SupplierAddress.Address3,
                                Address4 = SupplierAddress.Address4,
                                PinCode = SupplierAddress.PinCode,
                                CountryId = SupplierAddress.CountryId,
                                PhoneNo = SupplierAddress.PhoneNo,
                                FaxNo = SupplierAddress.FaxNo,
                                EmailAdd = SupplierAddress.EmailAdd,
                                WebUrl = SupplierAddress.WebUrl,
                                IsDefaultAdd = SupplierAddress.IsDefaultAdd,
                                IsDeliveryAdd = SupplierAddress.IsDeliveryAdd,
                                IsFinAdd = SupplierAddress.IsFinAdd,
                                IsSalesAdd = SupplierAddress.IsSalesAdd,
                                CreateById = headerViewModel.UserId,
                                IsActive = SupplierAddress.IsActive,
                            };

                            var createdSupplierAddress = await _SupplierAddressService.AddSupplierAddressAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierAddressEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdSupplierAddress);
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
                    "Error creating new SupplierAddress record");
            }
        }

        [HttpPut, Route("UpdateSupplierAddress/{SupplierAddressId}")]
        [Authorize]
        public async Task<ActionResult<SupplierAddressViewModel>> UpdateSupplierAddress(Int16 SupplierAddressId, [FromBody] SupplierAddressViewModel SupplierAddress, [FromHeader] HeaderViewModel headerViewModel)
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
                            if (SupplierAddressId != SupplierAddress.AddressId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_SupplierAddress ID mismatch");

                            var SupplierAddressToUpdate = await _SupplierAddressService.GetSupplierAddressByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierAddressId, headerViewModel.UserId);

                            if (SupplierAddressToUpdate == null)
                                return NotFound($"M_SupplierAddress with Id = {SupplierAddressId} not found");

                            var SupplierAddressEntity = new M_SupplierAddress
                            {
                                SupplierId = SupplierAddress.SupplierId,
                                AddressId = SupplierAddress.AddressId,
                                Address1 = SupplierAddress.Address1,
                                Address2 = SupplierAddress.Address2,
                                Address3 = SupplierAddress.Address3,
                                Address4 = SupplierAddress.Address4,
                                PinCode = SupplierAddress.PinCode,
                                CountryId = SupplierAddress.CountryId,
                                PhoneNo = SupplierAddress.PhoneNo,
                                FaxNo = SupplierAddress.FaxNo,
                                EmailAdd = SupplierAddress.EmailAdd,
                                WebUrl = SupplierAddress.WebUrl,
                                IsDefaultAdd = SupplierAddress.IsDefaultAdd,
                                IsDeliveryAdd = SupplierAddress.IsDeliveryAdd,
                                IsFinAdd = SupplierAddress.IsFinAdd,
                                IsSalesAdd = SupplierAddress.IsSalesAdd,
                                CreateById = headerViewModel.UserId,
                                IsActive = SupplierAddress.IsActive,
                            };

                            var sqlResponce = await _SupplierAddressService.UpdateSupplierAddressAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierAddressEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteSupplierAddress/{SupplierAddressId}")]
        [Authorize]
        public async Task<ActionResult<M_SupplierAddress>> DeleteSupplierAddress(Int16 SupplierAddressId, [FromHeader] HeaderViewModel headerViewModel)
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
                            var SupplierAddressToDelete = await _SupplierAddressService.GetSupplierAddressByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierAddressId, headerViewModel.UserId);

                            if (SupplierAddressToDelete == null)
                                return NotFound($"M_SupplierAddress with Id = {SupplierAddressId} not found");

                            var sqlResponce = await _SupplierAddressService.DeleteSupplierAddressAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierAddressToDelete, headerViewModel.UserId);

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