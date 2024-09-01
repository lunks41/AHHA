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
        public async Task<ActionResult> GetTax([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Tax, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _TaxService.GetTaxListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetTaxbyid/{TaxId}")]
        [Authorize]
        public async Task<ActionResult<TaxViewModel>> GetTaxById(Int16 TaxId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Tax, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var taxViewModel = _mapper.Map<TaxViewModel>(await _TaxService.GetTaxByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxId, headerViewModel.UserId));

                        if (taxViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, taxViewModel);
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
        public async Task<ActionResult<TaxViewModel>> CreateTax(TaxViewModel taxViewModel, [FromHeader] HeaderViewModel headerViewModel)
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
                            if (taxViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var TaxEntity = new M_Tax
                            {
                                TaxId = taxViewModel.TaxId,
                                CompanyId = headerViewModel.CompanyId,
                                TaxCode = taxViewModel.TaxCode,
                                TaxName = taxViewModel.TaxName,
                                Remarks = taxViewModel.Remarks,
                                IsActive = taxViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
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
        public async Task<ActionResult<TaxViewModel>> UpdateTax(Int16 TaxId, [FromBody] TaxViewModel taxViewModel, [FromHeader] HeaderViewModel headerViewModel)
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
                            if (TaxId != taxViewModel.TaxId)
                                return StatusCode(StatusCodes.Status400BadRequest, "Tax ID mismatch");

                            var TaxToUpdate = await _TaxService.GetTaxByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxId, headerViewModel.UserId);

                            if (TaxToUpdate == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var TaxEntity = new M_Tax
                            {
                                TaxId = taxViewModel.TaxId,
                                CompanyId = headerViewModel.CompanyId,
                                TaxCode = taxViewModel.TaxCode,
                                TaxName = taxViewModel.TaxName,
                                Remarks = taxViewModel.Remarks,
                                IsActive = taxViewModel.IsActive,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
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

        [HttpDelete, Route("DeleteTax/{TaxId}")]
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
                                return NotFound(GenrateMessage.datanotfound);

                            var sqlResponce = await _TaxService.DeleteTaxAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxToDelete, headerViewModel.UserId);

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