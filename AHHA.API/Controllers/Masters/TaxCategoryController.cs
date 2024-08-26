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
    public class TaxCategoryController : BaseController
    {
        private readonly ITaxCategoryService _TaxCategoryService;
        private readonly ILogger<TaxCategoryController> _logger;

        public TaxCategoryController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<TaxCategoryController> logger, ITaxCategoryService TaxCategoryService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _TaxCategoryService = TaxCategoryService;
        }

        [HttpGet, Route("GetTaxCategory")]
        [Authorize]
        public async Task<ActionResult> GetTaxCategory([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.TaxCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _TaxCategoryService.GetTaxCategoryListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetTaxCategorybyid/{TaxCategoryId}")]
        [Authorize]
        public async Task<ActionResult<TaxCategoryViewModel>> GetTaxCategoryById(Int16 TaxCategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.TaxCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var taxCategoryViewModel = _mapper.Map<TaxCategoryViewModel>(await _TaxCategoryService.GetTaxCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxCategoryId, headerViewModel.UserId));

                        if (taxCategoryViewModel == null)
                            return NotFound(GenrateMessage.authenticationfailed);

                        return StatusCode(StatusCodes.Status202Accepted, taxCategoryViewModel);
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

        [HttpPost, Route("AddTaxCategory")]
        [Authorize]
        public async Task<ActionResult<TaxCategoryViewModel>> CreateTaxCategory(TaxCategoryViewModel TaxCategory, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.TaxCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (TaxCategory == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_TaxCategory ID mismatch");

                            var TaxCategoryEntity = new M_TaxCategory
                            {
                                CompanyId = TaxCategory.CompanyId,
                                TaxCategoryCode = TaxCategory.TaxCategoryCode,
                                TaxCategoryId = TaxCategory.TaxCategoryId,
                                TaxCategoryName = TaxCategory.TaxCategoryName,
                                CreateById = headerViewModel.UserId,
                                IsActive = TaxCategory.IsActive,
                                Remarks = TaxCategory.Remarks
                            };

                            var createdTaxCategory = await _TaxCategoryService.AddTaxCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxCategoryEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdTaxCategory);
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
                    "Error creating new TaxCategory record");
            }
        }

        [HttpPut, Route("UpdateTaxCategory/{TaxCategoryId}")]
        [Authorize]
        public async Task<ActionResult<TaxCategoryViewModel>> UpdateTaxCategory(Int16 TaxCategoryId, [FromBody] TaxCategoryViewModel TaxCategory, [FromHeader] HeaderViewModel headerViewModel)
        {
            var TaxCategoryViewModel = new TaxCategoryViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.TaxCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (TaxCategoryId != TaxCategory.TaxCategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_TaxCategory ID mismatch");

                            var TaxCategoryToUpdate = await _TaxCategoryService.GetTaxCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxCategoryId, headerViewModel.UserId);

                            if (TaxCategoryToUpdate == null)
                                return NotFound($"M_TaxCategory with Id = {TaxCategoryId} not found");

                            var TaxCategoryEntity = new M_TaxCategory
                            {
                                TaxCategoryCode = TaxCategory.TaxCategoryCode,
                                TaxCategoryId = TaxCategory.TaxCategoryId,
                                TaxCategoryName = TaxCategory.TaxCategoryName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = TaxCategory.IsActive,
                                Remarks = TaxCategory.Remarks
                            };

                            var sqlResponce = await _TaxCategoryService.UpdateTaxCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxCategoryEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteTaxCategory/{TaxCategoryId}")]
        [Authorize]
        public async Task<ActionResult<M_TaxCategory>> DeleteTaxCategory(Int16 TaxCategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.TaxCategory, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var TaxCategoryToDelete = await _TaxCategoryService.GetTaxCategoryByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxCategoryId, headerViewModel.UserId);

                            if (TaxCategoryToDelete == null)
                                return NotFound($"M_TaxCategory with Id = {TaxCategoryId} not found");

                            var sqlResponce = await _TaxCategoryService.DeleteTaxCategoryAsync(headerViewModel.RegId, headerViewModel.CompanyId, TaxCategoryToDelete, headerViewModel.UserId);

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