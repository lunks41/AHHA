﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Setting;
using AHHA.Core.Common;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Setting
{
    [Route("api/Setting")]
    [ApiController]
    public class NumberFormatController : BaseController
    {
        private readonly INumberFormatServices _NumberFormatServices;
        private readonly ILogger<NumberFormatController> _logger;

        public NumberFormatController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<NumberFormatController> logger, INumberFormatServices NumberFormatServices)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _NumberFormatServices = NumberFormatServices;
        }

        //left side API --

        //get the data by using --numberid
        //get api by moduleid/transid/companyid
        //right side menu by using dropdown of year -- yearnumber/numberid
        //upsert

        //do not create the dt api because we create by using SP when the numberdt not available

        //Left Side menu list
        [HttpGet, Route("GetNumberFormat")]
        [Authorize]
        public async Task<ActionResult> GetNumberFormat([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Setting, (Int16)E_Setting.DocumentNo, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _NumberFormatServices.GetNumberFormatListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, cacheData);
                    }
                    else
                    {
                        return NotFound(GenerateMessage.AuthenticationFailed);
                    }
                }
                else
                {
                    return NotFound(GenerateMessage.AuthenticationFailed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        //While click on menu
        [HttpGet, Route("GetNumberFormatbyid/{ModuleId}/{TransactionId}")]
        [Authorize]
        public async Task<ActionResult<NumberSettingViewModel>> GetNumberFormatById(Int32 ModuleId, Int32 TransactionId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    return NoContent();
                }

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Setting, (Int16)E_Setting.DocumentNo, headerViewModel.UserId);

                if (userGroupRight == null)
                {
                    return NotFound(GenerateMessage.AuthenticationFailed);
                }

                var cacheData = await _NumberFormatServices.GetNumberFormatByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, ModuleId, TransactionId, headerViewModel.UserId);

                return cacheData switch
                {
                    null => Ok(new SqlResponse { Result = -1, Message = "failed", Data = null, TotalRecords = 0 }),
                    _ => Ok(new SqlResponse { Result = 1, Message = "Success", Data = cacheData, TotalRecords = 0 })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        //While Dropdown selection
        [HttpGet, Route("GetNumberFormatbyyear/{NumberId}/{NumYear}")]
        [Authorize]
        public async Task<ActionResult<NumberSettingDtViewModel>> GetNumberFormatbyyear(Int32 NumberId, Int32 NumYear, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    return NoContent();
                }

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Setting, (Int16)E_Setting.DocumentNo, headerViewModel.UserId);

                if (userGroupRight == null)
                {
                    return NotFound(GenerateMessage.AuthenticationFailed);
                }

                var cacheData = await _NumberFormatServices.GetNumberFormatByYearAsync(headerViewModel.RegId, headerViewModel.CompanyId, NumberId, NumYear, headerViewModel.UserId);

                return cacheData switch
                {
                    null => Ok(new SqlResponse { Result = -1, Message = "failed", Data = null, TotalRecords = 0 }),
                    _ => Ok(new SqlResponse { Result = 1, Message = "Success", Data = cacheData, TotalRecords = 0 })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPost, Route("SaveNumberFormat")]
        [Authorize]
        public async Task<ActionResult<NumberSettingViewModel>> SaveNumberFormat(NumberSettingViewModel numberSettingViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                // Validate headers
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return BadRequest("Invalid headers");

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Setting, (Int32)E_Setting.DocumentNo, headerViewModel.UserId);

                if (userGroupRight == null)
                    return Unauthorized("Authentication failed");

                if (userGroupRight == null || !userGroupRight.IsCreate)
                {
                    _logger.LogWarning("Authorization failed for user: {UserId}", headerViewModel.UserId);
                    return Unauthorized(GenerateMessage.AuthenticationFailed);
                }

                // Validate input model
                if (numberSettingViewModel == null)
                    return NotFound(GenerateMessage.DataNotFound);

                // Map to entity
                var NumberFormatEntity = new S_NumberFormat
                {
                    NumberId = numberSettingViewModel.NumberId,
                    CompanyId = headerViewModel.CompanyId,
                    ModuleId = numberSettingViewModel.ModuleId,
                    TransactionId = numberSettingViewModel.TransactionId,
                    Prefix = numberSettingViewModel.Prefix,
                    PrefixSeq = numberSettingViewModel.PrefixSeq,
                    PrefixDelimiter = numberSettingViewModel.PrefixDelimiter,
                    IncludeYear = numberSettingViewModel.IncludeYear,
                    YearSeq = numberSettingViewModel.YearSeq,
                    YearFormat = numberSettingViewModel.YearFormat,
                    YearDelimiter = numberSettingViewModel.YearDelimiter,
                    IncludeMonth = numberSettingViewModel.IncludeMonth,
                    MonthSeq = numberSettingViewModel.MonthSeq,
                    MonthFormat = numberSettingViewModel.MonthFormat,
                    MonthDelimiter = numberSettingViewModel.MonthDelimiter,
                    NoDIgits = numberSettingViewModel.NoDIgits,
                    DIgitSeq = numberSettingViewModel.DIgitSeq,
                    ResetYearly = numberSettingViewModel.ResetYearly,
                    CreateById = headerViewModel.UserId,
                    EditById = headerViewModel.UserId,
                    EditDate = DateTime.Now,
                };

                // Save the number format
                var createdNumberFormat = await _NumberFormatServices.SaveNumberFormatAsync(headerViewModel.RegId, headerViewModel.CompanyId, NumberFormatEntity, headerViewModel.UserId);

                if (createdNumberFormat.Result <= 0)
                {
                    _logger.LogWarning("Failed to save number format for CompanyId: {CompanyId}, RegId: {RegId}", headerViewModel.CompanyId, headerViewModel.RegId);
                    return Ok(new SqlResponse { Result = -1, Message = "Failed", Data = null, TotalRecords = 0 });
                }

                // Retrieve cached data
                var cacheData = await _NumberFormatServices.GetNumberFormatByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId,
                    numberSettingViewModel.ModuleId, numberSettingViewModel.TransactionId, headerViewModel.UserId);

                return cacheData switch
                {
                    null => Ok(new SqlResponse { Result = -1, Message = "Failed", Data = null, TotalRecords = 0 }),
                    _ => Ok(new SqlResponse { Result = 1, Message = "Success", Data = cacheData, TotalRecords = 0 })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new NumberFormat record");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}