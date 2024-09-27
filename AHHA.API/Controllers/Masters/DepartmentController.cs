﻿using AHHA.Application.IServices;
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
        public async Task<ActionResult> GetDepartment([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Department, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _DepartmentService.GetDepartmentListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenrateMessage.datanotfound);

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

        [HttpGet, Route("GetDepartmentbyid/{DepartmentId}")]
        [Authorize]
        public async Task<ActionResult<DepartmentViewModel>> GetDepartmentById(Int16 DepartmentId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Department, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var departmentViewModel = _mapper.Map<DepartmentViewModel>(await _DepartmentService.GetDepartmentByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, DepartmentId, headerViewModel.UserId));

                        if (departmentViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, departmentViewModel);
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

        [HttpPost, Route("SaveDepartment")]
        [Authorize]
        public async Task<ActionResult<DepartmentViewModel>> SaveDepartment(DepartmentViewModel departmentViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Department, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (departmentViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var DepartmentEntity = new M_Department
                            {
                                DepartmentId = departmentViewModel.DepartmentId,
                                CompanyId = headerViewModel.CompanyId,
                                DepartmentCode = departmentViewModel.DepartmentCode,
                                DepartmentName = departmentViewModel.DepartmentName,
                                Remarks = departmentViewModel.Remarks,
                                IsActive = departmentViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponce = await _DepartmentService.SaveDepartmentAsync(headerViewModel.RegId, headerViewModel.CompanyId, DepartmentEntity, headerViewModel.UserId);

                            if (sqlResponce.Result < 0)
                                return StatusCode(StatusCodes.Status204NoContent, sqlResponce);

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
                    "Error creating new Department record");
            }
        }

        [HttpDelete, Route("DeleteDepartment/{DepartmentId}")]
        [Authorize]
        public async Task<ActionResult<M_Department>> DeleteDepartment(Int16 DepartmentId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Master, (Int16)E_Master.Department, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var DepartmentToDelete = await _DepartmentService.GetDepartmentByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, DepartmentId, headerViewModel.UserId);

                            if (DepartmentToDelete == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var sqlResponce = await _DepartmentService.DeleteDepartmentAsync(headerViewModel.RegId, headerViewModel.CompanyId, DepartmentToDelete, headerViewModel.UserId);

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