﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Accounts.GL;
using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.GL;
using AHHA.Core.Helper;
using AHHA.Core.Models.Account.GL;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Accounts.GL
{
    [Route("api/Account")]
    [ApiController]
    public class GLPeriodCloseController : BaseController
    {
        private readonly IGLPeriodCloseService _GLPeriodCloseService;
        private readonly ILogger<GLPeriodCloseController> _logger;

        public GLPeriodCloseController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<GLPeriodCloseController> logger, IGLPeriodCloseService GLPeriodCloseService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _GLPeriodCloseService = GLPeriodCloseService;
        }

        [HttpGet, Route("GetGLPeriodClose/{FinYear}")]
        [Authorize]
        public async Task<ActionResult> GetGLPeriodClose(Int32 FinYear, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.Receipt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _GLPeriodCloseService.GetGLPeriodCloseListAsync(headerViewModel.RegId, headerViewModel.CompanyId, FinYear, headerViewModel.UserId);

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
                 new SqlResponse { Result = -1, Message = "Internal Server Error", Data = null });
            }
        }

        [HttpPost, Route("SaveGLPeriodClose")]
        [Authorize]
        public async Task<ActionResult> SaveGLPeriodClose([FromBody] List<GLPeriodCloseViewModel> glPeriodCloseViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                // Validate headers
                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                // Check user group rights
                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.GL, (Int16)E_GL.PeriodClose, headerViewModel.UserId);

                if (userGroupRight == null || !userGroupRight.IsCreate)
                    return NotFound(GenerateMessage.AuthenticationFailed);

                // Validate input data
                if (glPeriodCloseViewModel == null || !glPeriodCloseViewModel.Any())
                    return NotFound(GenerateMessage.DataNotFound);

                // Map the view model to the entity
                var glPeriodCloseEntities = glPeriodCloseViewModel.Select(item => new GLPeriodClose
                {
                    CompanyId = headerViewModel.CompanyId,
                    FinYear = item.FinYear,
                    FinMonth = item.FinMonth,
                    StartDate = DateHelperStatic.ParseClientDate(item.StartDate),
                    EndDate = DateHelperStatic.ParseClientDate(item.EndDate),
                    IsArClose = item.IsArClose,
                    ArCloseById = item.ArCloseById,
                    ArCloseDate = item.ArCloseDate == "" ? null : DateHelperStatic.ParseClientDate(item.ArCloseDate),
                    IsApClose = item.IsApClose,
                    ApCloseById = item.ApCloseById,
                    ApCloseDate = item.ApCloseDate == "" ? null : DateHelperStatic.ParseClientDate(item.ApCloseDate),
                    IsCbClose = item.IsCbClose,
                    CbCloseById = item.CbCloseById,
                    CbCloseDate = item.CbCloseDate == "" ? null : DateHelperStatic.ParseClientDate(item.CbCloseDate),
                    IsGlClose = item.IsGlClose,
                    GlCloseById = item.GlCloseById,
                    GlCloseDate = item.GlCloseDate == "" ? null : DateHelperStatic.ParseClientDate(item.GlCloseDate),
                    CreateById = headerViewModel.UserId,
                    EditById = headerViewModel.UserId,
                    EditDate = DateTime.Now,
                }).ToList();

                // Save the mapped entities
                var sqlResponse = await _GLPeriodCloseService.SaveGLPeriodCloseAsync(headerViewModel.RegId, headerViewModel.CompanyId, glPeriodCloseEntities, headerViewModel.UserId);

                if (sqlResponse.Result > 0)
                {
                    var listglperiod = await _GLPeriodCloseService.GetGLPeriodCloseListAsync(headerViewModel.RegId, headerViewModel.CompanyId, glPeriodCloseViewModel[0].FinYear, headerViewModel.UserId);

                    return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = listglperiod, TotalRecords = 0 });
                }

                return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = null, TotalRecords = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving GLPeriodClose records");
                return StatusCode(StatusCodes.Status500InternalServerError, new SqlResponse { Result = -1, Message = "An error occurred while processing your request", Data = null });
            }
        }

        [HttpPost, Route("SaveGLPeriodCloseV1")]
        [Authorize]
        public async Task<ActionResult<GLPeriodCloseViewModel>> SaveGLPeriodCloseV1(PeriodCloseViewModel periodCloseViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.Receipt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate || userGroupRight.IsEdit)
                        {
                            if (periodCloseViewModel == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var sqlResponse = await _GLPeriodCloseService.SaveGLPeriodCloseAsyncV1(headerViewModel.RegId, headerViewModel.CompanyId, periodCloseViewModel, headerViewModel.UserId);

                            return StatusCode(StatusCodes.Status202Accepted, sqlResponse);
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
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new GLPeriodClose record");
            }
        }

        [HttpDelete, Route("DeleteGlPeriodClose/{FinYear}")]
        [Authorize]
        public async Task<ActionResult<GLPeriodCloseViewModel>> DeleteGlPeriodClose(Int16 FinYear, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (FinYear == null || FinYear == 0)
                    return NoContent();

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.GL, (Int16)E_GL.PeriodClose, headerViewModel.UserId);

                if (userGroupRight == null)
                    return NotFound(GenerateMessage.AuthenticationFailed);

                if (!userGroupRight.IsCreate || !userGroupRight.IsEdit || userGroupRight == null)
                    return NotFound(GenerateMessage.AuthenticationFailed);

                var sqlResponse = await _GLPeriodCloseService.DeletePeriodClose(headerViewModel.RegId, headerViewModel.CompanyId, FinYear, headerViewModel.UserId);

                return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = null, TotalRecords = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost, Route("SaveNewPeriodClose")]
        [Authorize]
        public async Task<ActionResult<GLPeriodCloseViewModel>> SaveNewPeriodClose(NewPeriodCloseViewModel newPeriodCloseViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (newPeriodCloseViewModel == null || newPeriodCloseViewModel?.YearId == 0 || newPeriodCloseViewModel?.FinYear == 0)
                    return NoContent();

                if (!ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                    return NoContent();

                var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.GL, (Int16)E_GL.PeriodClose, headerViewModel.UserId);

                if (userGroupRight == null)
                    return NotFound(GenerateMessage.AuthenticationFailed);

                if (!userGroupRight.IsCreate || !userGroupRight.IsEdit || userGroupRight == null)
                    return NotFound(GenerateMessage.AuthenticationFailed);

                var sqlResponse = await _GLPeriodCloseService.SaveNewPeriodCloseAsync(headerViewModel.RegId, headerViewModel.CompanyId, newPeriodCloseViewModel, headerViewModel.UserId);

                return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = null, TotalRecords = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}