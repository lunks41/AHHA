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
                            return NotFound(GenrateMessage.authenticationfailed);

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
            var SupplierViewModel = new SupplierViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Supplier_{SupplierId}", out SupplierViewModel? cachedProduct))
                        {
                            SupplierViewModel = cachedProduct;
                        }
                        else
                        {
                            SupplierViewModel = _mapper.Map<SupplierViewModel>(await _SupplierService.GetSupplierByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierId, headerViewModel.UserId));

                            if (SupplierViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the Supplier with an expiration time of 10 minutes
                                _memoryCache.Set($"Supplier_{SupplierId}", SupplierViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, SupplierViewModel);
                        //return Ok(SupplierViewModel);
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
        public async Task<ActionResult<SupplierViewModel>> CreateSupplier(SupplierViewModel Supplier, [FromHeader] HeaderViewModel headerViewModel)
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
                            if (Supplier == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Supplier ID mismatch");

                            var SupplierEntity = new M_Supplier
                            {
                                CompanyId = Supplier.CompanyId,
                                SupplierCode = Supplier.SupplierCode,
                                SupplierId = Supplier.SupplierId,
                                SupplierName = Supplier.SupplierName,
                                CreateById = headerViewModel.UserId,
                                IsActive = Supplier.IsActive,
                                Remarks = Supplier.Remarks
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
        public async Task<ActionResult<SupplierViewModel>> UpdateSupplier(Int16 SupplierId, [FromBody] SupplierViewModel Supplier, [FromHeader] HeaderViewModel headerViewModel)
        {
            var SupplierViewModel = new SupplierViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, headerViewModel.UserId);

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
                                var SupplierToUpdate = await _SupplierService.GetSupplierByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierId, headerViewModel.UserId);

                                if (SupplierToUpdate == null)
                                    return NotFound(GenrateMessage.authenticationfailed);
                            }

                            var SupplierEntity = new M_Supplier
                            {
                                SupplierCode = Supplier.SupplierCode,
                                SupplierId = Supplier.SupplierId,
                                SupplierName = Supplier.SupplierName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = Supplier.IsActive,
                                Remarks = Supplier.Remarks
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
                                return NotFound($"M_Supplier with Id = {SupplierId} not found");

                            var sqlResponce = await _SupplierService.DeleteSupplierAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Supplier_{SupplierId}");
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