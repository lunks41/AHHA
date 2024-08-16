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
    public class SupplierController : BaseController
    {
        private readonly ISupplierService _SupplierService;
        private readonly ILogger<SupplierController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public SupplierController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<SupplierController> logger, ISupplierService SupplierService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _SupplierService = SupplierService;
        }

        [HttpGet, Route("GetSupplier")]
        [Authorize]
        public async Task<ActionResult> GetAllSupplier()
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
                        var cacheData = _memoryCache.Get<SupplierViewModelCount>("Supplier");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _SupplierService.GetSupplierListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<SupplierViewModelCount>("Supplier", cacheData, expirationTime);

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

        [HttpGet, Route("GetSupplierbyid/{SupplierId}")]
        [Authorize]
        public async Task<ActionResult<SupplierViewModel>> GetSupplierById(Int16 SupplierId)
        {
            var SupplierViewModel = new SupplierViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Supplier_{SupplierId}", out SupplierViewModel? cachedProduct))
                        {
                            SupplierViewModel = cachedProduct;
                        }
                        else
                        {
                            SupplierViewModel = _mapper.Map<SupplierViewModel>(await _SupplierService.GetSupplierByIdAsync(RegId,CompanyId, SupplierId, UserId));

                            if (SupplierViewModel == null)
                                return NotFound();
                            else
                                // Cache the Supplier with an expiration time of 10 minutes
                                _memoryCache.Set($"Supplier_{SupplierId}", SupplierViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, SupplierViewModel);
                        //return Ok(SupplierViewModel);
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

        [HttpPost, Route("AddSupplier")]
        [Authorize]
        public async Task<ActionResult<SupplierViewModel>> CreateSupplier(SupplierViewModel Supplier)
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
                            if (Supplier == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Supplier ID mismatch");

                            var SupplierEntity = new M_Supplier
                            {
                                CompanyId = Supplier.CompanyId,
                                SupplierCode = Supplier.SupplierCode,
                                SupplierId = Supplier.SupplierId,
                                SupplierName = Supplier.SupplierName,
                                CreateById = UserId,
                                IsActive = Supplier.IsActive,
                                Remarks = Supplier.Remarks
                            };

                            var createdSupplier = await _SupplierService.AddSupplierAsync(RegId,CompanyId, SupplierEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdSupplier);

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
                    "Error creating new Supplier record");
            }
        }

        [HttpPut, Route("UpdateSupplier/{SupplierId}")]
        [Authorize]
        public async Task<ActionResult<SupplierViewModel>> UpdateSupplier(Int16 SupplierId, [FromBody] SupplierViewModel Supplier)
        {
            var SupplierViewModel = new SupplierViewModel();
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
                            if (SupplierId != Supplier.SupplierId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Supplier ID mismatch");
                            //return BadRequest("M_Supplier ID mismatch");

                            // Attempt to retrieve the Supplier from the cache
                            if (_memoryCache.TryGetValue($"Supplier_{SupplierId}", out SupplierViewModel? cachedProduct))
                            {
                                SupplierViewModel = cachedProduct;
                            }
                            else
                            {
                                var SupplierToUpdate = await _SupplierService.GetSupplierByIdAsync(RegId,CompanyId, SupplierId, UserId);

                                if (SupplierToUpdate == null)
                                    return NotFound($"M_Supplier with Id = {SupplierId} not found");
                            }

                            var SupplierEntity = new M_Supplier
                            {
                                SupplierCode = Supplier.SupplierCode,
                                SupplierId = Supplier.SupplierId,
                                SupplierName = Supplier.SupplierName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = Supplier.IsActive,
                                Remarks = Supplier.Remarks
                            };

                            var sqlResponce = await _SupplierService.UpdateSupplierAsync(RegId,CompanyId, SupplierEntity, UserId);
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

        [HttpDelete, Route("Delete/{SupplierId}")]
        [Authorize]
        public async Task<ActionResult<M_Supplier>> DeleteSupplier(Int16 SupplierId)
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
                            var SupplierToDelete = await _SupplierService.GetSupplierByIdAsync(RegId,CompanyId, SupplierId, UserId);

                            if (SupplierToDelete == null)
                                return NotFound($"M_Supplier with Id = {SupplierId} not found");

                            var sqlResponce = await _SupplierService.DeleteSupplierAsync(RegId,CompanyId, SupplierToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Supplier_{SupplierId}");
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


