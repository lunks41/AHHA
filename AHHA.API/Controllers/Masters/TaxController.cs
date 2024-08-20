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
    public class TaxController : BaseController
    {
        private readonly ITaxService _TaxService;
        private readonly ILogger<TaxController> _logger;

        public TaxController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<TaxController> logger, ITaxService TaxService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _TaxService = TaxService;
        }

        [HttpGet, Route("GetTax")]
        [Authorize]
        public async Task<ActionResult> GetAllTax([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Tax, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _TaxService.GetTaxListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenrateMessage.authenticationfailed);

                        return Ok(cacheData);
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

        [HttpGet, Route("GetTaxbyid/{TaxId}")]
        [Authorize]
        public async Task<ActionResult<TaxViewModel>> GetTaxById(Int16 TaxId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var TaxViewModel = new TaxViewModel();
            try
            {



                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Tax, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Tax_{TaxId}", out TaxViewModel? cachedProduct))
                        {
                            TaxViewModel = cachedProduct;
                        }
                        else
                        {
                            TaxViewModel = _mapper.Map<TaxViewModel>(await _TaxService.GetTaxByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxId, headerViewModel.UserId));

                            if (TaxViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the Tax with an expiration time of 10 minutes
                                _memoryCache.Set($"Tax_{TaxId}", TaxViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, TaxViewModel);
                        //return Ok(TaxViewModel);
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

        [HttpPost, Route("AddTax")]
        [Authorize]
        public async Task<ActionResult<TaxViewModel>> CreateTax(TaxViewModel Tax, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {



                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Tax, headerViewModel.UserId);

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
                                CreateById = headerViewModel.UserId,
                                IsActive = Tax.IsActive,
                                Remarks = Tax.Remarks
                            };

                            var createdTax = await _TaxService.AddTaxAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdTax);

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
                    "Error creating new Tax record");
            }
        }

        [HttpPut, Route("UpdateTax/{TaxId}")]
        [Authorize]
        public async Task<ActionResult<TaxViewModel>> UpdateTax(Int16 TaxId, [FromBody] TaxViewModel Tax, [FromHeader] HeaderViewModel headerViewModel)
        {
            var TaxViewModel = new TaxViewModel();
            try
            {



                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Tax, headerViewModel.UserId);

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
                                var TaxToUpdate = await _TaxService.GetTaxByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxId, headerViewModel.UserId);

                                if (TaxToUpdate == null)
                                    return NotFound($"M_Tax with Id = {TaxId} not found");
                            }

                            var TaxEntity = new M_Tax
                            {
                                TaxCode = Tax.TaxCode,
                                TaxId = Tax.TaxId,
                                TaxName = Tax.TaxName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = Tax.IsActive,
                                Remarks = Tax.Remarks
                            };

                            var sqlResponce = await _TaxService.UpdateTaxAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("Delete/{TaxId}")]
        [Authorize]
        public async Task<ActionResult<M_Tax>> DeleteTax(Int16 TaxId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {



                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Tax, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var TaxToDelete = await _TaxService.GetTaxByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxId, headerViewModel.UserId);

                            if (TaxToDelete == null)
                                return NotFound($"M_Tax with Id = {TaxId} not found");

                            var sqlResponce = await _TaxService.DeleteTaxAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Tax_{TaxId}");
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


