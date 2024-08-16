using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/Master")]
    [ApiController]
    public class SupplierAddressController : BaseController
    {
        private readonly ISupplierAddressService _SupplierAddressService;
        private readonly ILogger<SupplierAddressController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public SupplierAddressController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<SupplierAddressController> logger, ISupplierAddressService SupplierAddressService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _SupplierAddressService = SupplierAddressService;
        }

        [HttpGet, Route("GetSupplierAddress")]
        [Authorize]
        public async Task<ActionResult> GetAllSupplierAddress()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<SupplierAddressViewModelCount>("SupplierAddress");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _SupplierAddressService.GetSupplierAddressListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<SupplierAddressViewModelCount>("SupplierAddress", cacheData, expirationTime);

                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                            //return Ok(cacheData);
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
                    }
                }
                else
                {
                    if (UserId == 0)
                        return NotFound("UserId Not Found");
                    else if (CompanyId == 0)
                        return NotFound("CompanyId Not Found");
                    else
                        return NotFound();
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
        public async Task<ActionResult<SupplierAddressViewModel>> GetSupplierAddressById(Int16 SupplierAddressId)
        {
            var SupplierAddressViewModel = new SupplierAddressViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"SupplierAddress_{SupplierAddressId}", out SupplierAddressViewModel? cachedProduct))
                        {
                            SupplierAddressViewModel = cachedProduct;
                        }
                        else
                        {
                            SupplierAddressViewModel = _mapper.Map<SupplierAddressViewModel>(await _SupplierAddressService.GetSupplierAddressByIdAsync(RegId,CompanyId, SupplierAddressId, UserId));

                            if (SupplierAddressViewModel == null)
                                return NotFound();
                            else
                                // Cache the SupplierAddress with an expiration time of 10 minutes
                                _memoryCache.Set($"SupplierAddress_{SupplierAddressId}", SupplierAddressViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, SupplierAddressViewModel);
                        //return Ok(SupplierAddressViewModel);
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
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
        public async Task<ActionResult<SupplierAddressViewModel>> CreateSupplierAddress(SupplierAddressViewModel SupplierAddress)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, UserId);

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
                                CreateById = UserId,
                                IsActive = SupplierAddress.IsActive,
                            };

                            var createdSupplierAddress = await _SupplierAddressService.AddSupplierAddressAsync(RegId,CompanyId, SupplierAddressEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdSupplierAddress);

                        }
                        else
                        {
                            return NotFound("Users do not have a access to delete");
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
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
        public async Task<ActionResult<SupplierAddressViewModel>> UpdateSupplierAddress(Int16 SupplierAddressId, [FromBody] SupplierAddressViewModel SupplierAddress)
        {
            var SupplierAddressViewModel = new SupplierAddressViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (SupplierAddressId != SupplierAddress.AddressId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_SupplierAddress ID mismatch");
                            //return BadRequest("M_SupplierAddress ID mismatch");

                            // Attempt to retrieve the SupplierAddress from the cache
                            if (_memoryCache.TryGetValue($"SupplierAddress_{SupplierAddressId}", out SupplierAddressViewModel? cachedProduct))
                            {
                                SupplierAddressViewModel = cachedProduct;
                            }
                            else
                            {
                                var SupplierAddressToUpdate = await _SupplierAddressService.GetSupplierAddressByIdAsync(RegId,CompanyId, SupplierAddressId, UserId);

                                if (SupplierAddressToUpdate == null)
                                    return NotFound($"M_SupplierAddress with Id = {SupplierAddressId} not found");
                            }

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
                                CreateById = UserId,
                                IsActive = SupplierAddress.IsActive,
                            };

                            var sqlResponce = await _SupplierAddressService.UpdateSupplierAddressAsync(RegId,CompanyId, SupplierAddressEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
                        }
                        else
                        {
                            return NotFound("Users do not have a access to delete");
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
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

        [HttpDelete, Route("Delete/{SupplierAddressId}")]
        [Authorize]
        public async Task<ActionResult<M_SupplierAddress>> DeleteSupplierAddress(Int16 SupplierAddressId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var SupplierAddressToDelete = await _SupplierAddressService.GetSupplierAddressByIdAsync(RegId,CompanyId, SupplierAddressId, UserId);

                            if (SupplierAddressToDelete == null)
                                return NotFound($"M_SupplierAddress with Id = {SupplierAddressId} not found");

                            var sqlResponce = await _SupplierAddressService.DeleteSupplierAddressAsync(RegId,CompanyId, SupplierAddressToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"SupplierAddress_{SupplierAddressId}");
                            return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
                        }
                        else
                        {
                            return NotFound("Users do not have a access to delete");
                        }
                    }
                    else
                    {
                        return NotFound("Users not have a access for this screen");
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


