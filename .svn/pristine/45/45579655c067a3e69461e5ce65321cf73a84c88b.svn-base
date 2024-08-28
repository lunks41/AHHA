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
    public class CurrencyController : BaseController
    {
        private readonly ICurrencyService _CurrencyService;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CurrencyController> logger, ICurrencyService CurrencyService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CurrencyService = CurrencyService;
        }

        [HttpGet, Route("GetCurrency")]
        [Authorize]
        public async Task<ActionResult> GetCurrency([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Currency, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var CurrencyData = await _CurrencyService.GetCurrencyListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (CurrencyData == null)
                            return NotFound();

                        return StatusCode(StatusCodes.Status202Accepted, CurrencyData);
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

        [HttpGet, Route("GetCurrencybyid/{CurrencyId}")]
        [Authorize]
        public async Task<ActionResult<CurrencyViewModel>> GetCurrencyById(Int32 CurrencyId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Currency, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var currencyViewModel = _mapper.Map<CurrencyViewModel>(await _CurrencyService.GetCurrencyByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CurrencyId, headerViewModel.UserId));

                        if (currencyViewModel == null)
                            return NotFound();

                        return StatusCode(StatusCodes.Status202Accepted, currencyViewModel);
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

        [HttpPost, Route("AddCurrency")]
        [Authorize]
        public async Task<ActionResult<CurrencyViewModel>> CreateCurrency(CurrencyViewModel Currency, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Currency, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Currency == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "Currency ID mismatch");

                            var CurrencyEntity = new M_Currency
                            {
                                CompanyId = Currency.CompanyId,
                                CurrencyCode = Currency.CurrencyCode,
                                CurrencyId = Currency.CurrencyId,
                                CurrencyName = Currency.CurrencyName,
                                CreateById = headerViewModel.UserId,
                                IsActive = Currency.IsActive,
                                Remarks = Currency.Remarks
                            };

                            var createdCurrency = await _CurrencyService.AddCurrencyAsync(headerViewModel.RegId, headerViewModel.CompanyId, CurrencyEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCurrency);
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
                    "Error creating new Currency record");
            }
        }

        [HttpPut, Route("UpdateCurrency/{CurrencyId}")]
        [Authorize]
        public async Task<ActionResult<CurrencyViewModel>> UpdateCurrency(int CurrencyId, [FromBody] CurrencyViewModel Currency, [FromHeader] HeaderViewModel headerViewModel)
        {
            var CurrencyViewModel = new CurrencyViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Currency, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (CurrencyId != Currency.CurrencyId)
                                return StatusCode(StatusCodes.Status400BadRequest, "Currency ID mismatch");

                            //Checking Currency data available or not by using CurrencyId
                            var CurrencyToUpdate = await _CurrencyService.GetCurrencyByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CurrencyId, headerViewModel.UserId);

                            if (CurrencyToUpdate == null)
                                return NotFound($"Currency with Id = {CurrencyId} not found");

                            var CurrencyEntity = new M_Currency
                            {
                                CurrencyCode = Currency.CurrencyCode,
                                CurrencyId = Currency.CurrencyId,
                                CurrencyName = Currency.CurrencyName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = Currency.IsActive,
                                Remarks = Currency.Remarks
                            };

                            var sqlResponce = await _CurrencyService.UpdateCurrencyAsync(headerViewModel.RegId, headerViewModel.CompanyId, CurrencyEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteCurrency/{CurrencyId}")]
        [Authorize]
        public async Task<ActionResult<M_Currency>> DeleteCurrency(int CurrencyId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Currency, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var CurrencyToDelete = await _CurrencyService.GetCurrencyByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CurrencyId, headerViewModel.UserId);

                            if (CurrencyToDelete == null)
                                return NotFound($"Currency with Id = {CurrencyId} not found");

                            var sqlResponce = await _CurrencyService.DeleteCurrencyAsync(headerViewModel.RegId, headerViewModel.CompanyId, CurrencyToDelete, headerViewModel.UserId);

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