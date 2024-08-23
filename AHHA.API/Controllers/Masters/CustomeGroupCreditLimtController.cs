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
    public class CustomeGroupCreditLimtController : BaseController
    {
        private readonly ICustomeGroupCreditLimtService _CustomeGroupCreditLimtService;
        private readonly ILogger<CustomeGroupCreditLimtController> _logger;

        public CustomeGroupCreditLimtController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CustomeGroupCreditLimtController> logger, ICustomeGroupCreditLimtService CustomeGroupCreditLimtService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CustomeGroupCreditLimtService = CustomeGroupCreditLimtService;
        }

        [HttpGet, Route("GetCustomeGroupCreditLimt")]
        [Authorize]
        public async Task<ActionResult> GetCustomeGroupCreditLimt([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerGroupCreditLimt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        headerViewModel.searchString = headerViewModel.searchString == null ? string.Empty : headerViewModel.searchString.Trim();

                        var cacheData = await _CustomeGroupCreditLimtService.GetCustomeGroupCreditLimtListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetCustomeGroupCreditLimtbyid/{GroupCreditLimitId}")]
        [Authorize]
        public async Task<ActionResult<CustomeGroupCreditLimtViewModel>> GetCustomeGroupCreditLimtById(Int16 GroupCreditLimitId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var CustomeGroupCreditLimtViewModel = new CustomeGroupCreditLimtViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerGroupCreditLimt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"CustomeGroupCreditLimt_{GroupCreditLimitId}", out CustomeGroupCreditLimtViewModel? cachedProduct))
                        {
                            CustomeGroupCreditLimtViewModel = cachedProduct;
                        }
                        else
                        {
                            CustomeGroupCreditLimtViewModel = _mapper.Map<CustomeGroupCreditLimtViewModel>(await _CustomeGroupCreditLimtService.GetCustomeGroupCreditLimtByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GroupCreditLimitId, headerViewModel.UserId));

                            if (CustomeGroupCreditLimtViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the CustomeGroupCreditLimt with an expiration time of 10 minutes
                                _memoryCache.Set($"CustomeGroupCreditLimt_{GroupCreditLimitId}", CustomeGroupCreditLimtViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, CustomeGroupCreditLimtViewModel);
                        //return Ok(CustomeGroupCreditLimtViewModel);
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

        [HttpPost, Route("AddCustomeGroupCreditLimt")]
        [Authorize]
        public async Task<ActionResult<CustomeGroupCreditLimtViewModel>> CreateCustomeGroupCreditLimt(CustomeGroupCreditLimtViewModel CustomeGroupCreditLimt, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerGroupCreditLimt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (CustomeGroupCreditLimt == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_CustomeGroupCreditLimt ID mismatch");

                            var CustomeGroupCreditLimtEntity = new M_CustomeGroupCreditLimt
                            {
                                CompanyId = CustomeGroupCreditLimt.CompanyId,
                                GroupCreditLimitCode = CustomeGroupCreditLimt.GroupCreditLimitCode,
                                GroupCreditLimitId = CustomeGroupCreditLimt.GroupCreditLimitId,
                                GroupCreditLimitName = CustomeGroupCreditLimt.GroupCreditLimitName,
                                CreateById = headerViewModel.UserId,
                                IsActive = CustomeGroupCreditLimt.IsActive,
                                Remarks = CustomeGroupCreditLimt.Remarks
                            };

                            var createdCustomeGroupCreditLimt = await _CustomeGroupCreditLimtService.AddCustomeGroupCreditLimtAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomeGroupCreditLimtEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCustomeGroupCreditLimt);
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
                    "Error creating new CustomeGroupCreditLimt record");
            }
        }

        [HttpPut, Route("UpdateCustomeGroupCreditLimt/{GroupCreditLimitId}")]
        [Authorize]
        public async Task<ActionResult<CustomeGroupCreditLimtViewModel>> UpdateCustomeGroupCreditLimt(Int16 GroupCreditLimitId, [FromBody] CustomeGroupCreditLimtViewModel CustomeGroupCreditLimt, [FromHeader] HeaderViewModel headerViewModel)
        {
            var CustomeGroupCreditLimtViewModel = new CustomeGroupCreditLimtViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerGroupCreditLimt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (GroupCreditLimitId != CustomeGroupCreditLimt.GroupCreditLimitId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_CustomeGroupCreditLimt ID mismatch");
                            //return BadRequest("M_CustomeGroupCreditLimt ID mismatch");

                            // Attempt to retrieve the CustomeGroupCreditLimt from the cache
                            if (_memoryCache.TryGetValue($"CustomeGroupCreditLimt_{GroupCreditLimitId}", out CustomeGroupCreditLimtViewModel? cachedProduct))
                            {
                                CustomeGroupCreditLimtViewModel = cachedProduct;
                            }
                            else
                            {
                                var CustomeGroupCreditLimtToUpdate = await _CustomeGroupCreditLimtService.GetCustomeGroupCreditLimtByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GroupCreditLimitId, headerViewModel.UserId);

                                if (CustomeGroupCreditLimtToUpdate == null)
                                    return NotFound($"M_CustomeGroupCreditLimt with Id = {GroupCreditLimitId} not found");
                            }

                            var CustomeGroupCreditLimtEntity = new M_CustomeGroupCreditLimt
                            {
                                CompanyId = CustomeGroupCreditLimt.CompanyId,
                                GroupCreditLimitCode = CustomeGroupCreditLimt.GroupCreditLimitCode,
                                GroupCreditLimitId = CustomeGroupCreditLimt.GroupCreditLimitId,
                                GroupCreditLimitName = CustomeGroupCreditLimt.GroupCreditLimitName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = CustomeGroupCreditLimt.IsActive,
                                Remarks = CustomeGroupCreditLimt.Remarks
                            };

                            var sqlResponce = await _CustomeGroupCreditLimtService.UpdateCustomeGroupCreditLimtAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomeGroupCreditLimtEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteCustomeGroupCreditLimt/{GroupCreditLimitId}")]
        [Authorize]
        public async Task<ActionResult<M_CustomeGroupCreditLimt>> DeleteCustomeGroupCreditLimt(Int16 GroupCreditLimitId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CustomerGroupCreditLimt, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var CustomeGroupCreditLimtToDelete = await _CustomeGroupCreditLimtService.GetCustomeGroupCreditLimtByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GroupCreditLimitId, headerViewModel.UserId);

                            if (CustomeGroupCreditLimtToDelete == null)
                                return NotFound($"M_CustomeGroupCreditLimt with Id = {GroupCreditLimitId} not found");

                            var sqlResponce = await _CustomeGroupCreditLimtService.DeleteCustomeGroupCreditLimtAsync(headerViewModel.RegId, headerViewModel.CompanyId, CustomeGroupCreditLimtToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"CustomeGroupCreditLimt_{GroupCreditLimitId}");
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