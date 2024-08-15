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
    public class TaxController : BaseController
    {
        private readonly ITaxService _TaxService;
        private readonly ILogger<TaxController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public TaxController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<TaxController> logger, ITaxService TaxService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _TaxService = TaxService;
        }

        [HttpGet, Route("GetTax")]
        [Authorize]
        public async Task<ActionResult> GetAllTax()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Tax, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<TaxViewModelCount>("Tax");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _TaxService.GetTaxListAsync(CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<TaxViewModelCount>("Tax", cacheData, expirationTime);

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

        [HttpGet, Route("GetTaxbyid/{TaxId}")]
        [Authorize]
        public async Task<ActionResult<TaxViewModel>> GetTaxById(Int16 TaxId)
        {
            var TaxViewModel = new TaxViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Tax, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Tax_{TaxId}", out TaxViewModel? cachedProduct))
                        {
                            TaxViewModel = cachedProduct;
                        }
                        else
                        {
                            TaxViewModel = _mapper.Map<TaxViewModel>(await _TaxService.GetTaxByIdAsync(CompanyId, TaxId, UserId));

                            if (TaxViewModel == null)
                                return NotFound();
                            else
                                // Cache the Tax with an expiration time of 10 minutes
                                _memoryCache.Set($"Tax_{TaxId}", TaxViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, TaxViewModel);
                        //return Ok(TaxViewModel);
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

        [HttpPost, Route("AddTax")]
        [Authorize]
        public async Task<ActionResult<TaxViewModel>> CreateTax(TaxViewModel Tax)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Tax, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Tax == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Tax ID mismatch");

                            var TaxEntity = new M_Tax
                            {
                                CompanyId = Tax.CompanyId,
                                TaxCode = Tax.TaxCode,
                                TaxId = Tax.TaxId,
                                TaxName = Tax.TaxName,
                                CreateById = UserId,
                                IsActive = Tax.IsActive,
                                Remarks = Tax.Remarks
                            };

                            var createdTax = await _TaxService.AddTaxAsync(CompanyId, TaxEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdTax);

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
                    "Error creating new Tax record");
            }
        }

        [HttpPut, Route("UpdateTax/{TaxId}")]
        [Authorize]
        public async Task<ActionResult<TaxViewModel>> UpdateTax(Int16 TaxId, [FromBody] TaxViewModel Tax)
        {
            var TaxViewModel = new TaxViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Tax, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (TaxId != Tax.TaxId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Tax ID mismatch");
                            //return BadRequest("M_Tax ID mismatch");

                            // Attempt to retrieve the Tax from the cache
                            if (_memoryCache.TryGetValue($"Tax_{TaxId}", out TaxViewModel? cachedProduct))
                            {
                                TaxViewModel = cachedProduct;
                            }
                            else
                            {
                                var TaxToUpdate = await _TaxService.GetTaxByIdAsync(CompanyId, TaxId, UserId);

                                if (TaxToUpdate == null)
                                    return NotFound($"M_Tax with Id = {TaxId} not found");
                            }

                            var TaxEntity = new M_Tax
                            {
                                TaxCode = Tax.TaxCode,
                                TaxId = Tax.TaxId,
                                TaxName = Tax.TaxName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = Tax.IsActive,
                                Remarks = Tax.Remarks
                            };

                            var sqlResponce = await _TaxService.UpdateTaxAsync(CompanyId, TaxEntity, UserId);
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

        [HttpDelete, Route("Delete/{TaxId}")]
        [Authorize]
        public async Task<ActionResult<M_Tax>> DeleteTax(Int16 TaxId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Tax, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var TaxToDelete = await _TaxService.GetTaxByIdAsync(CompanyId, TaxId, UserId);

                            if (TaxToDelete == null)
                                return NotFound($"M_Tax with Id = {TaxId} not found");

                            var sqlResponce = await _TaxService.DeleteTaxAsync(CompanyId, TaxToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Tax_{TaxId}");
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


