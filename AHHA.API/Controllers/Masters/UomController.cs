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
    public class UomController : BaseController
    {
        private readonly IUomService _UomService;
        private readonly ILogger<UomController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private string RegId = string.Empty;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public UomController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<UomController> logger, IUomService UomService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _UomService = UomService;
        }

        [HttpGet, Route("GetUom")]
        [Authorize]
        public async Task<ActionResult> GetAllUom()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Request.Headers.TryGetValue("regId", out StringValues regIdValue).ToString().Trim();

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Uom, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<UomViewModelCount>("Uom");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _UomService.GetUomListAsync(RegId,CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<UomViewModelCount>("Uom", cacheData, expirationTime);

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

        [HttpGet, Route("GetUombyid/{UomId}")]
        [Authorize]
        public async Task<ActionResult<UomViewModel>> GetUomById(Int16 UomId)
        {
            var UomViewModel = new UomViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Uom, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Uom_{UomId}", out UomViewModel? cachedProduct))
                        {
                            UomViewModel = cachedProduct;
                        }
                        else
                        {
                            UomViewModel = _mapper.Map<UomViewModel>(await _UomService.GetUomByIdAsync(RegId,CompanyId, UomId, UserId));

                            if (UomViewModel == null)
                                return NotFound();
                            else
                                // Cache the Uom with an expiration time of 10 minutes
                                _memoryCache.Set($"Uom_{UomId}", UomViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, UomViewModel);
                        //return Ok(UomViewModel);
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

        [HttpPost, Route("AddUom")]
        [Authorize]
        public async Task<ActionResult<UomViewModel>> CreateUom(UomViewModel Uom)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Uom, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Uom == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Uom ID mismatch");

                            var UomEntity = new M_Uom
                            {
                                CompanyId = Uom.CompanyId,
                                UomCode = Uom.UomCode,
                                UomId = Uom.UomId,
                                UomName = Uom.UomName,
                                CreateById = UserId,
                                IsActive = Uom.IsActive,
                                Remarks = Uom.Remarks
                            };

                            var createdUom = await _UomService.AddUomAsync(RegId,CompanyId, UomEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdUom);

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
                    "Error creating new Uom record");
            }
        }

        [HttpPut, Route("UpdateUom/{UomId}")]
        [Authorize]
        public async Task<ActionResult<UomViewModel>> UpdateUom(Int16 UomId, [FromBody] UomViewModel Uom)
        {
            var UomViewModel = new UomViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Uom, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (UomId != Uom.UomId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Uom ID mismatch");
                            //return BadRequest("M_Uom ID mismatch");

                            // Attempt to retrieve the Uom from the cache
                            if (_memoryCache.TryGetValue($"Uom_{UomId}", out UomViewModel? cachedProduct))
                            {
                                UomViewModel = cachedProduct;
                            }
                            else
                            {
                                var UomToUpdate = await _UomService.GetUomByIdAsync(RegId,CompanyId, UomId, UserId);

                                if (UomToUpdate == null)
                                    return NotFound($"M_Uom with Id = {UomId} not found");
                            }

                            var UomEntity = new M_Uom
                            {
                                UomCode = Uom.UomCode,
                                UomId = Uom.UomId,
                                UomName = Uom.UomName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = Uom.IsActive,
                                Remarks = Uom.Remarks
                            };

                            var sqlResponce = await _UomService.UpdateUomAsync(RegId,CompanyId, UomEntity, UserId);
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

        [HttpDelete, Route("Delete/{UomId}")]
        [Authorize]
        public async Task<ActionResult<M_Uom>> DeleteUom(Int16 UomId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(RegId,CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(RegId,CompanyId, (Int16)Modules.Master, (Int32)Master.Uom, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var UomToDelete = await _UomService.GetUomByIdAsync(RegId,CompanyId, UomId, UserId);

                            if (UomToDelete == null)
                                return NotFound($"M_Uom with Id = {UomId} not found");

                            var sqlResponce = await _UomService.DeleteUomAsync(RegId,CompanyId, UomToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"Uom_{UomId}");
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


