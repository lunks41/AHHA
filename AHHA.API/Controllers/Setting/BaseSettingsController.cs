﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Setting;
using AHHA.Core.Common;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Setting
{
    [Route("api/Setting")]
    [ApiController]
    public class BaseSettingsController : BaseController
    {
        private readonly IBaseSettingsService _BaseSettingsService;
        private readonly ILogger<BaseSettingsController> _logger;

        public BaseSettingsController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<BaseSettingsController> logger, IBaseSettingsService BaseSettingsService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _BaseSettingsService = BaseSettingsService;
        }

        [HttpGet, Route("GetExchangeRate/{CurrencyId}/{TrnsDate}")]
        [Authorize]
        public async Task<ActionResult<decimal>> GetExchangeRate(Int16 CurrencyId, DateTime TrnsDate, [FromHeader] HeaderViewModel headerViewModel)
        {
            string trnsDate = TrnsDate.ToString("yyyy-MM-dd");
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var BaseViewModel = await _BaseSettingsService.GetExchangeRateAsync(headerViewModel.RegId, headerViewModel.CompanyId, CurrencyId, trnsDate, headerViewModel.UserId);

                    if (BaseViewModel == null)
                        return NotFound(GenerateMessage.DataNotFound);

                    return StatusCode(StatusCodes.Status202Accepted, BaseViewModel);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetExchangeRateLocal/{CurrencyId}/{TrnsDate}")]
        [Authorize]
        public async Task<ActionResult<decimal>> GetExchangeRateLocal(Int16 CurrencyId, DateTime TrnsDate, [FromHeader] HeaderViewModel headerViewModel)
        {
            string trnsDate = TrnsDate.ToString("yyyy-MM-dd");
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var BaseViewModel = await _BaseSettingsService.GetExchangeRateLocalAsync(headerViewModel.RegId, headerViewModel.CompanyId, CurrencyId, trnsDate, headerViewModel.UserId);

                    if (BaseViewModel == null)
                        return NotFound(GenerateMessage.DataNotFound);

                    return StatusCode(StatusCodes.Status202Accepted, BaseViewModel);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetCheckPeriodClosed/{ModuleId}/{TrnsDate}")]
        [Authorize]
        public async Task<ActionResult<bool>> GetCheckPeriodClosed(Int16 ModuleId, DateTime TrnsDate, [FromHeader] HeaderViewModel headerViewModel)
        {
            string trnsDate = TrnsDate.ToString("yyyy-MM-dd");
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var BaseViewModel = await _BaseSettingsService.GetCheckPeriodClosedAsync(headerViewModel.RegId, headerViewModel.CompanyId, ModuleId, trnsDate, headerViewModel.UserId);

                    if (BaseViewModel == null)
                        return NotFound(GenerateMessage.DataNotFound);

                    return StatusCode(StatusCodes.Status202Accepted, BaseViewModel);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetGstPercentage/{GstId}/{TrnsDate}")]
        [Authorize]
        public async Task<ActionResult<decimal>> GetGstPercentage(Int16 GstId, DateTime TrnsDate, [FromHeader] HeaderViewModel headerViewModel)
        {
            string trnsDate = TrnsDate.ToString("yyyy-MM-dd");
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var BaseViewModel = await _BaseSettingsService.GetGstPercentageAsync(headerViewModel.RegId, headerViewModel.CompanyId, GstId, trnsDate, headerViewModel.UserId);

                    if (BaseViewModel == null)
                        return NotFound(GenerateMessage.DataNotFound);

                    return StatusCode(StatusCodes.Status202Accepted, BaseViewModel);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpGet, Route("GetCreditTermDay/{CreditTermId}/{TrnsDate}")]
        [Authorize]
        public async Task<ActionResult<decimal>> GetCreditTermDay(Int16 CreditTermId, DateTime TrnsDate, [FromHeader] HeaderViewModel headerViewModel)
        {
            string trnsDate = TrnsDate.ToString("yyyy-MM-dd");
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var BaseViewModel = await _BaseSettingsService.GetCreditTermDayAsync(headerViewModel.RegId, headerViewModel.CompanyId, CreditTermId, trnsDate, headerViewModel.UserId);

                    if (BaseViewModel == null)
                        return NotFound(GenerateMessage.DataNotFound);

                    return StatusCode(StatusCodes.Status202Accepted, BaseViewModel);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }
    }
}