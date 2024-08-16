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
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _EmployeeService;
        private readonly ILogger<EmployeeController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public EmployeeController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<EmployeeController> logger, IEmployeeService EmployeeService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _EmployeeService = EmployeeService;
        }

        [HttpGet, Route("GetEmployee")]
        [Authorize]
        public async Task<ActionResult> GetAllEmployee()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Employee, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<EmployeeViewModelCount>("Employee");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _EmployeeService.GetEmployeeListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<EmployeeViewModelCount>("Employee", cacheData, expirationTime);

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

        [HttpGet, Route("GetEmployeebyid/{EmployeeId}")]
        [Authorize]
        public async Task<ActionResult<EmployeeViewModel>> GetEmployeeById(Int16 EmployeeId)
        {
            var EmployeeViewModel = new EmployeeViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Employee, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Employee_{EmployeeId}", out EmployeeViewModel? cachedProduct))
                        {
                            EmployeeViewModel = cachedProduct;
                        }
                        else
                        {
                            EmployeeViewModel = _mapper.Map<EmployeeViewModel>(await _EmployeeService.GetEmployeeByIdAsync(RegId,CompanyId, EmployeeId, UserId));

                            if (EmployeeViewModel == null)
                                return NotFound();
                            else
                                // Cache the Employee with an expiration time of 10 minutes
                                _memoryCache.Set($"Employee_{EmployeeId}", EmployeeViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, EmployeeViewModel);
                        //return Ok(EmployeeViewModel);
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

        [HttpPost, Route("AddEmployee")]
        [Authorize]
        public async Task<ActionResult<EmployeeViewModel>> CreateEmployee(EmployeeViewModel Employee)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Employee, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Employee == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Employee ID mismatch");

                            var EmployeeEntity = new M_Employee
                            {
                                CompanyId = Employee.CompanyId,
                                EmployeeCode = Employee.EmployeeCode,
                                EmployeeId = Employee.EmployeeId,
                                EmployeeName = Employee.EmployeeName,
                                CreateById = UserId,
                                IsActive = Employee.IsActive,
                                Remarks = Employee.Remarks
                            };

                            var createdEmployee = await _EmployeeService.AddEmployeeAsync(RegId,CompanyId, EmployeeEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdEmployee);

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
                    "Error creating new Employee record");
            }
        }

        [HttpPut, Route("UpdateEmployee/{EmployeeId}")]
        [Authorize]
        public async Task<ActionResult<EmployeeViewModel>> UpdateEmployee(Int16 EmployeeId, [FromBody] EmployeeViewModel Employee)
        {
            var EmployeeViewModel = new EmployeeViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Employee, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (EmployeeId != Employee.EmployeeId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Employee ID mismatch");
                            //return BadRequest("M_Employee ID mismatch");

                            // Attempt to retrieve the Employee from the cache
                            if (_memoryCache.TryGetValue($"Employee_{EmployeeId}", out EmployeeViewModel? cachedProduct))
                            {
                                EmployeeViewModel = cachedProduct;
                            }
                            else
                            {
                                var EmployeeToUpdate = await _EmployeeService.GetEmployeeByIdAsync(RegId,CompanyId, EmployeeId, UserId);

                                if (EmployeeToUpdate == null)
                                    return NotFound($"M_Employee with Id = {EmployeeId} not found");
                            }

                            var EmployeeEntity = new M_Employee
                            {
                                EmployeeCode = Employee.EmployeeCode,
                                EmployeeId = Employee.EmployeeId,
                                EmployeeName = Employee.EmployeeName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = Employee.IsActive,
                                Remarks = Employee.Remarks
                            };

                            var sqlResponce = await _EmployeeService.UpdateEmployeeAsync(RegId,CompanyId, EmployeeEntity, UserId);
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

        [HttpDelete, Route("Delete/{EmployeeId}")]
        [Authorize]
        public async Task<ActionResult<M_Employee>> DeleteEmployee(Int16 EmployeeId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Employee, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var EmployeeToDelete = await _EmployeeService.GetEmployeeByIdAsync(RegId,CompanyId, EmployeeId, UserId);

                            if (EmployeeToDelete == null)
                                return NotFound($"M_Employee with Id = {EmployeeId} not found");

                            var sqlResponce = await _EmployeeService.DeleteEmployeeAsync(RegId,CompanyId, EmployeeToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Employee_{EmployeeId}");
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

