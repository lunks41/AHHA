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
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _DepartmentService;
        private readonly ILogger<DepartmentController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public DepartmentController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<DepartmentController> logger, IDepartmentService DepartmentService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _DepartmentService = DepartmentService;
        }

        [HttpGet, Route("GetDepartment")]
        [Authorize]
        public async Task<ActionResult> GetAllDepartment()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Department, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<DepartmentViewModelCount>("Department");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _DepartmentService.GetDepartmentListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<DepartmentViewModelCount>("Department", cacheData, expirationTime);

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

        [HttpGet, Route("GetDepartmentbyid/{DepartmentId}")]
        [Authorize]
        public async Task<ActionResult<DepartmentViewModel>> GetDepartmentById(Int16 DepartmentId)
        {
            var DepartmentViewModel = new DepartmentViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Department, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Department_{DepartmentId}", out DepartmentViewModel? cachedProduct))
                        {
                            DepartmentViewModel = cachedProduct;
                        }
                        else
                        {
                            DepartmentViewModel = _mapper.Map<DepartmentViewModel>(await _DepartmentService.GetDepartmentByIdAsync(RegId,CompanyId, DepartmentId, UserId));

                            if (DepartmentViewModel == null)
                                return NotFound();
                            else
                                // Cache the Department with an expiration time of 10 minutes
                                _memoryCache.Set($"Department_{DepartmentId}", DepartmentViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, DepartmentViewModel);
                        //return Ok(DepartmentViewModel);
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

        [HttpPost, Route("AddDepartment")]
        [Authorize]
        public async Task<ActionResult<DepartmentViewModel>> CreateDepartment(DepartmentViewModel Department)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Department, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Department == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Department ID mismatch");

                            var DepartmentEntity = new M_Department
                            {
                                CompanyId = Department.CompanyId,
                                DepartmentCode = Department.DepartmentCode,
                                DepartmentId = Department.DepartmentId,
                                DepartmentName = Department.DepartmentName,
                                CreateById = UserId,
                                IsActive = Department.IsActive,
                                Remarks = Department.Remarks
                            };

                            var createdDepartment = await _DepartmentService.AddDepartmentAsync(RegId,CompanyId, DepartmentEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdDepartment);

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
                    "Error creating new Department record");
            }
        }

        [HttpPut, Route("UpdateDepartment/{DepartmentId}")]
        [Authorize]
        public async Task<ActionResult<DepartmentViewModel>> UpdateDepartment(Int16 DepartmentId, [FromBody] DepartmentViewModel Department)
        {
            var DepartmentViewModel = new DepartmentViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Department, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (DepartmentId != Department.DepartmentId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Department ID mismatch");
                            //return BadRequest("M_Department ID mismatch");

                            // Attempt to retrieve the Department from the cache
                            if (_memoryCache.TryGetValue($"Department_{DepartmentId}", out DepartmentViewModel? cachedProduct))
                            {
                                DepartmentViewModel = cachedProduct;
                            }
                            else
                            {
                                var DepartmentToUpdate = await _DepartmentService.GetDepartmentByIdAsync(RegId,CompanyId, DepartmentId, UserId);

                                if (DepartmentToUpdate == null)
                                    return NotFound($"M_Department with Id = {DepartmentId} not found");
                            }

                            var DepartmentEntity = new M_Department
                            {
                                DepartmentCode = Department.DepartmentCode,
                                DepartmentId = Department.DepartmentId,
                                DepartmentName = Department.DepartmentName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = Department.IsActive,
                                Remarks = Department.Remarks
                            };

                            var sqlResponce = await _DepartmentService.UpdateDepartmentAsync(RegId,CompanyId, DepartmentEntity, UserId);
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

        [HttpDelete, Route("Delete/{DepartmentId}")]
        [Authorize]
        public async Task<ActionResult<M_Department>> DeleteDepartment(Int16 DepartmentId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Department, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var DepartmentToDelete = await _DepartmentService.GetDepartmentByIdAsync(RegId,CompanyId, DepartmentId, UserId);

                            if (DepartmentToDelete == null)
                                return NotFound($"M_Department with Id = {DepartmentId} not found");

                            var sqlResponce = await _DepartmentService.DeleteDepartmentAsync(RegId,CompanyId, DepartmentToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Department_{DepartmentId}");
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


