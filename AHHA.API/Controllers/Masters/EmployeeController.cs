﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Helper;
using AHHA.Core.Models.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/Master")]
    [ApiController]
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _EmployeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<EmployeeController> logger, IEmployeeService EmployeeService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _EmployeeService = EmployeeService;
        }

        [HttpGet, Route("GetEmployee")]
        [Authorize]
        public async Task<ActionResult> GetEmployee([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Employee, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _EmployeeService.GetEmployeeListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetEmployeebyid/{EmployeeId}")]
        [Authorize]
        public async Task<ActionResult<EmployeeViewModel>> GetEmployeeById(Int16 EmployeeId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Employee, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var EmployeeViewModel = _mapper.Map<EmployeeViewModel>(await _EmployeeService.GetEmployeeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, EmployeeId, headerViewModel.UserId));

                        if (EmployeeViewModel == null)
                            return NotFound(GenerateMessage.DataNotFound);

                        return StatusCode(StatusCodes.Status202Accepted, EmployeeViewModel);
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
                    "Error retrieving data from the database");
            }
        }

        [HttpPost, Route("SaveEmployee")]
        [Authorize]
        public async Task<ActionResult<EmployeeViewModel>> SaveEmployee([FromBody] EmployeeViewModel employeeViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Employee, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (employeeViewModel == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var EmployeeEntity = new M_Employee
                            {
                                EmployeeId = employeeViewModel.EmployeeId,
                                CompanyId = headerViewModel.CompanyId,
                                EmployeeCode = employeeViewModel.EmployeeCode?.Trim() ?? string.Empty,
                                EmployeeName = employeeViewModel.EmployeeName?.Trim() ?? string.Empty,
                                EmployeeOtherName = employeeViewModel.EmployeeOtherName?.Trim() ?? string.Empty,
                                EmployeePhoto = employeeViewModel.EmployeePhoto?.Trim() ?? string.Empty,
                                EmployeeSignature = employeeViewModel.EmployeeSignature?.Trim() ?? string.Empty,
                                DepartmentId = employeeViewModel.DepartmentId,
                                EmployeeSex = employeeViewModel.EmployeeSex?.Trim() ?? string.Empty,
                                MartialStatus = employeeViewModel.MartialStatus?.Trim() ?? string.Empty,
                                EmployeeDOB = DateHelperStatic.ParseClientDate(employeeViewModel.EmployeeDOB),
                                EmployeeJoinDate = DateHelperStatic.ParseClientDate(employeeViewModel.EmployeeJoinDate),
                                EmployeeLastDate = DateHelperStatic.ParseClientDate(employeeViewModel.EmployeeLastDate),
                                EmployeeOffEmailAdd = string.IsNullOrWhiteSpace(employeeViewModel.EmployeeOffEmailAdd) ? string.Empty : employeeViewModel.EmployeeOffEmailAdd,
                                EmployeeOtherEmailAdd = string.IsNullOrWhiteSpace(employeeViewModel.EmployeeOtherEmailAdd) ? string.Empty : employeeViewModel.EmployeeOtherEmailAdd,
                                Remarks = employeeViewModel.Remarks?.Trim() ?? string.Empty,
                                IsActive = employeeViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now
                            };

                            var sqlResponse = await _EmployeeService.SaveEmployeeAsync(headerViewModel.RegId, headerViewModel.CompanyId, EmployeeEntity, headerViewModel.UserId);
                            return Ok(new SqlResponse { Result = sqlResponse.Result, Message = sqlResponse.Message, Data = null, TotalRecords = 0 });
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
                    "Error creating new Employee record");
            }
        }

        [HttpDelete, Route("DeleteEmployee/{EmployeeId}")]
        [Authorize]
        public async Task<ActionResult<M_Employee>> DeleteEmployee(Int16 EmployeeId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Employee, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var EmployeeToDelete = await _EmployeeService.GetEmployeeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, EmployeeId, headerViewModel.UserId);

                            if (EmployeeToDelete == null)
                                return NotFound(GenerateMessage.DataNotFound);

                            var sqlResponse = await _EmployeeService.DeleteEmployeeAsync(headerViewModel.RegId, headerViewModel.CompanyId, EmployeeToDelete, headerViewModel.UserId);

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
                    "Error deleting data");
            }
        }
    }
}