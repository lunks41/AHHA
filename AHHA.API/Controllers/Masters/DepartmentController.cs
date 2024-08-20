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
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _DepartmentService;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<DepartmentController> logger, IDepartmentService DepartmentService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _DepartmentService = DepartmentService;
        }

        [HttpGet, Route("GetDepartment")]
        [Authorize]
        public async Task<ActionResult> GetAllDepartment([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Department, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _DepartmentService.GetDepartmentListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenrateMessage.authenticationfailed);

                        return Ok(cacheData);
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

        [HttpGet, Route("GetDepartmentbyid/{DepartmentId}")]
        [Authorize]
        public async Task<ActionResult<DepartmentViewModel>> GetDepartmentById(Int16 DepartmentId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var DepartmentViewModel = new DepartmentViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Department, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Department_{DepartmentId}", out DepartmentViewModel? cachedProduct))
                        {
                            DepartmentViewModel = cachedProduct;
                        }
                        else
                        {
                            DepartmentViewModel = _mapper.Map<DepartmentViewModel>(await _DepartmentService.GetDepartmentByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, DepartmentId, headerViewModel.UserId));

                            if (DepartmentViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the Department with an expiration time of 10 minutes
                                _memoryCache.Set($"Department_{DepartmentId}", DepartmentViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, DepartmentViewModel);
                        //return Ok(DepartmentViewModel);
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

        [HttpPost, Route("AddDepartment")]
        [Authorize]
        public async Task<ActionResult<DepartmentViewModel>> CreateDepartment(DepartmentViewModel Department, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Department, headerViewModel.UserId);

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
                                CreateById = headerViewModel.UserId,
                                IsActive = Department.IsActive,
                                Remarks = Department.Remarks
                            };

                            var createdDepartment = await _DepartmentService.AddDepartmentAsync(headerViewModel.RegId, headerViewModel.CompanyId, DepartmentEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdDepartment);
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
                    "Error creating new Department record");
            }
        }

        [HttpPut, Route("UpdateDepartment/{DepartmentId}")]
        [Authorize]
        public async Task<ActionResult<DepartmentViewModel>> UpdateDepartment(Int16 DepartmentId, [FromBody] DepartmentViewModel Department, [FromHeader] HeaderViewModel headerViewModel)
        {
            var DepartmentViewModel = new DepartmentViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Department, headerViewModel.UserId);

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
                                var DepartmentToUpdate = await _DepartmentService.GetDepartmentByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, DepartmentId, headerViewModel.UserId);

                                if (DepartmentToUpdate == null)
                                    return NotFound($"M_Department with Id = {DepartmentId} not found");
                            }

                            var DepartmentEntity = new M_Department
                            {
                                DepartmentCode = Department.DepartmentCode,
                                DepartmentId = Department.DepartmentId,
                                DepartmentName = Department.DepartmentName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = Department.IsActive,
                                Remarks = Department.Remarks
                            };

                            var sqlResponce = await _DepartmentService.UpdateDepartmentAsync(headerViewModel.RegId, headerViewModel.CompanyId, DepartmentEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("Delete/{DepartmentId}")]
        [Authorize]
        public async Task<ActionResult<M_Department>> DeleteDepartment(Int16 DepartmentId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Department, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var DepartmentToDelete = await _DepartmentService.GetDepartmentByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, DepartmentId, headerViewModel.UserId);

                            if (DepartmentToDelete == null)
                                return NotFound($"M_Department with Id = {DepartmentId} not found");

                            var sqlResponce = await _DepartmentService.DeleteDepartmentAsync(headerViewModel.RegId, headerViewModel.CompanyId, DepartmentToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Department_{DepartmentId}");
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