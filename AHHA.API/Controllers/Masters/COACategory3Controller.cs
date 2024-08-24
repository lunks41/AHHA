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
    public class COACategory3Controller : BaseController
    {
        private readonly ICOACategory3Service _COACategory3Service;
        private readonly ILogger<COACategory3Controller> _logger;

        public COACategory3Controller(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<COACategory3Controller> logger, ICOACategory3Service COACategory3Service)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _COACategory3Service = COACategory3Service;
        }

        [HttpGet, Route("GetCOACategory3")]
        [Authorize]
        public async Task<ActionResult> GetCOACategory3([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory3, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        

                        var cacheData = await _COACategory3Service.GetCOACategory3ListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

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

        [HttpGet, Route("GetCOACategory3byid/{COACategoryId}")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> GetCOACategory3ById(Int16 COACategoryId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var COACategoryViewModel = new COACategoryViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory3, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"COACategory3_{COACategoryId}", out COACategoryViewModel? cachedProduct))
                        {
                            COACategoryViewModel = cachedProduct;
                        }
                        else
                        {
                            COACategoryViewModel = _mapper.Map<COACategoryViewModel>(await _COACategory3Service.GetCOACategory3ByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, COACategoryId, headerViewModel.UserId));

                            if (COACategoryViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the COACategory3 with an expiration time of 10 minutes
                                _memoryCache.Set($"COACategory3_{COACategoryId}", COACategoryViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, COACategoryViewModel);
                        //return Ok(COACategoryViewModel);
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

        [HttpPost, Route("AddCOACategory3")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> CreateCOACategory3(COACategoryViewModel COACategory3, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory3, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (COACategory3 == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_COACategory3 ID mismatch");

                            var COACategory3Entity = new M_COACategory3
                            {
                                CompanyId = COACategory3.CompanyId,
                                COACategoryCode = COACategory3.COACategoryCode,
                                COACategoryId = COACategory3.COACategoryId,
                                COACategoryName = COACategory3.COACategoryName,
                                CreateById = headerViewModel.UserId,
                                IsActive = COACategory3.IsActive,
                                Remarks = COACategory3.Remarks
                            };

                            var createdCOACategory3 = await _COACategory3Service.AddCOACategory3Async(headerViewModel.RegId, headerViewModel.CompanyId, COACategory3Entity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCOACategory3);
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
                    "Error creating new COACategory3 record");
            }
        }

        [HttpPut, Route("UpdateCOACategory3/{COACategoryId}")]
        [Authorize]
        public async Task<ActionResult<COACategoryViewModel>> UpdateCOACategory3(Int16 COACategoryId, [FromBody] COACategoryViewModel COACategory3, [FromHeader] HeaderViewModel headerViewModel)
        {
            var COACategoryViewModel = new COACategoryViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory3, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (COACategoryId != COACategory3.COACategoryId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_COACategory3 ID mismatch");
                            //return BadRequest("M_COACategory3 ID mismatch");

                            // Attempt to retrieve the COACategory3 from the cache
                            if (_memoryCache.TryGetValue($"COACategory3_{COACategoryId}", out COACategoryViewModel? cachedProduct))
                            {
                                COACategoryViewModel = cachedProduct;
                            }
                            else
                            {
                                var COACategory3ToUpdate = await _COACategory3Service.GetCOACategory3ByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, COACategoryId, headerViewModel.UserId);

                                if (COACategory3ToUpdate == null)
                                    return NotFound($"M_COACategory3 with Id = {COACategoryId} not found");
                            }

                            var COACategory3Entity = new M_COACategory3
                            {
                                COACategoryCode = COACategory3.COACategoryCode,
                                COACategoryId = COACategory3.COACategoryId,
                                COACategoryName = COACategory3.COACategoryName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = COACategory3.IsActive,
                                Remarks = COACategory3.Remarks
                            };

                            var sqlResponce = await _COACategory3Service.UpdateCOACategory3Async(headerViewModel.RegId, headerViewModel.CompanyId, COACategory3Entity, headerViewModel.UserId);
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

        //[HttpDelete, Route("DeleteCOACategory3/{COACategoryId}")]
        //[Authorize]
        //public async Task<ActionResult<M_COACategory3>> DeleteCOACategory3(Int16 COACategoryId)
        //{
        //    try
        //    {
        //
        //

        //        if (ValidateHeaders(headerViewModel.RegId,headerViewModel.CompanyId, headerViewModel.UserId))
        //        {
        //            var userGroupRight = ValidateScreen(headerViewModel.RegId,headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.COACategory3, headerViewModel.UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var COACategory3ToDelete = await _COACategory3Service.GetCOACategory3ByIdAsync(headerViewModel.RegId,headerViewModel.CompanyId, COACategoryId, headerViewModel.UserId);

        //                    if (COACategory3ToDelete == null)
        //                        return NotFound($"M_COACategory3 with Id = {COACategoryId} not found");

        //                    var sqlResponce = await _COACategory3Service.DeleteCOACategory3Async(headerViewModel.RegId,headerViewModel.CompanyId, COACategory3ToDelete, headerViewModel.UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"COACategory3_{COACategoryId}");
        //                    return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
        //                }
        //                else
        //                {
        //                    return NotFound(GenrateMessage.authenticationfailed);
        //                }
        //            }
        //            else
        //            {
        //                return NotFound(GenrateMessage.authenticationfailed);
        //            }
        //        }
        //        else
        //        {
        //            return NoContent();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            "Error deleting data");
        //    }
        //}
    }
}