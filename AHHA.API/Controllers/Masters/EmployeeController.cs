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
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Employee, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _EmployeeService.GetEmployeeListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetEmployeebyid/{EmployeeId}")]
        [Authorize]
        public async Task<ActionResult<EmployeeViewModel>> GetEmployeeById(Int16 EmployeeId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Employee, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var EmployeeViewModel = _mapper.Map<EmployeeViewModel>(await _EmployeeService.GetEmployeeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, EmployeeId, headerViewModel.UserId));

                        if (EmployeeViewModel == null)
                            return NotFound(GenrateMessage.authenticationfailed);

                        return StatusCode(StatusCodes.Status202Accepted, EmployeeViewModel);
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

        [HttpPost, Route("AddEmployee")]
        [Authorize]
        public async Task<ActionResult<EmployeeViewModel>> CreateEmployee(EmployeeViewModel Employee, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Employee, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Employee == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "Employee ID mismatch");

                            var EmployeeEntity = new M_Employee
                            {
                                CompanyId = Employee.CompanyId,
                                EmployeeCode = Employee.EmployeeCode,
                                EmployeeId = Employee.EmployeeId,
                                EmployeeName = Employee.EmployeeName,
                                CreateById = headerViewModel.UserId,
                                IsActive = Employee.IsActive,
                                Remarks = Employee.Remarks
                            };

                            var createdEmployee = await _EmployeeService.AddEmployeeAsync(headerViewModel.RegId, headerViewModel.CompanyId, EmployeeEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdEmployee);
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
                    "Error creating new Employee record");
            }
        }

        [HttpPut, Route("UpdateEmployee/{EmployeeId}")]
        [Authorize]
        public async Task<ActionResult<EmployeeViewModel>> UpdateEmployee(Int16 EmployeeId, [FromBody] EmployeeViewModel Employee, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Employee, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (EmployeeId != Employee.EmployeeId)
                                return StatusCode(StatusCodes.Status400BadRequest, "Employee ID mismatch");

                            var EmployeeToUpdate = await _EmployeeService.GetEmployeeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, EmployeeId, headerViewModel.UserId);

                            if (EmployeeToUpdate == null)
                                return NotFound($"Employee with Id = {EmployeeId} not found");

                            var EmployeeEntity = new M_Employee
                            {
                                EmployeeCode = Employee.EmployeeCode,
                                EmployeeId = Employee.EmployeeId,
                                EmployeeName = Employee.EmployeeName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = Employee.IsActive,
                                Remarks = Employee.Remarks
                            };

                            var sqlResponce = await _EmployeeService.UpdateEmployeeAsync(headerViewModel.RegId, headerViewModel.CompanyId, EmployeeEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteEmployee/{EmployeeId}")]
        [Authorize]
        public async Task<ActionResult<M_Employee>> DeleteEmployee(Int16 EmployeeId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Employee, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var EmployeeToDelete = await _EmployeeService.GetEmployeeByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, EmployeeId, headerViewModel.UserId);

                            if (EmployeeToDelete == null)
                                return NotFound($"Employee with Id = {EmployeeId} not found");

                            var sqlResponce = await _EmployeeService.DeleteEmployeeAsync(headerViewModel.RegId, headerViewModel.CompanyId, EmployeeToDelete, headerViewModel.UserId);

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