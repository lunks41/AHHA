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
    public class GstController : BaseController
    {
        private readonly IGstService _GstService;
        private readonly ILogger<GstController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public GstController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<GstController> logger, IGstService GstService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _GstService = GstService;
        }

        [HttpGet, Route("GetGst")]
        [Authorize]
        public async Task<ActionResult> GetAllGst()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Gst, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<GstViewModelCount>("Gst");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _GstService.GetGstListAsync(CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<GstViewModelCount>("Gst", cacheData, expirationTime);

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

        [HttpGet, Route("GetGstbyid/{GstId}")]
        [Authorize]
        public async Task<ActionResult<GstViewModel>> GetGstById(Int16 GstId)
        {
            var GstViewModel = new GstViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Gst, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"Gst_{GstId}", out GstViewModel? cachedProduct))
                        {
                            GstViewModel = cachedProduct;
                        }
                        else
                        {
                            GstViewModel = _mapper.Map<GstViewModel>(await _GstService.GetGstByIdAsync(CompanyId, GstId, UserId));

                            if (GstViewModel == null)
                                return NotFound();
                            else
                                // Cache the Gst with an expiration time of 10 minutes
                                _memoryCache.Set($"Gst_{GstId}", GstViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, GstViewModel);
                        //return Ok(GstViewModel);
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

        [HttpPost, Route("AddGst")]
        [Authorize]
        public async Task<ActionResult<GstViewModel>> CreateGst(GstViewModel Gst)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Gst, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Gst == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Gst ID mismatch");

                            var GstEntity = new M_Gst
                            {
                                CompanyId = Gst.CompanyId,
                                GstCode = Gst.GstCode,
                                GstId = Gst.GstId,
                                GstName = Gst.GstName,
                                CreateById = UserId,
                                IsActive = Gst.IsActive,
                                Remarks = Gst.Remarks
                            };

                            var createdGst = await _GstService.AddGstAsync(CompanyId, GstEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdGst);

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
                    "Error creating new Gst record");
            }
        }

        [HttpPut, Route("UpdateGst/{GstId}")]
        [Authorize]
        public async Task<ActionResult<GstViewModel>> UpdateGst(Int16 GstId, [FromBody] GstViewModel Gst)
        {
            var GstViewModel = new GstViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Gst, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (GstId != Gst.GstId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_Gst ID mismatch");
                            //return BadRequest("M_Gst ID mismatch");

                            // Attempt to retrieve the Gst from the cache
                            if (_memoryCache.TryGetValue($"Gst_{GstId}", out GstViewModel? cachedProduct))
                            {
                                GstViewModel = cachedProduct;
                            }
                            else
                            {
                                var GstToUpdate = await _GstService.GetGstByIdAsync(CompanyId, GstId, UserId);

                                if (GstToUpdate == null)
                                    return NotFound($"M_Gst with Id = {GstId} not found");
                            }

                            var GstEntity = new M_Gst
                            {
                                GstCode = Gst.GstCode,
                                GstId = Gst.GstId,
                                GstName = Gst.GstName,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = Gst.IsActive,
                                Remarks = Gst.Remarks
                            };

                            var sqlResponce = await _GstService.UpdateGstAsync(CompanyId, GstEntity, UserId);
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

        //[HttpDelete, Route("Delete/{GstId}")]
        //[Authorize]
        //public async Task<ActionResult<M_Gst>> DeleteGst(Int16 GstId)
        //{
        //    try
        //    {
        //        CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
        //        UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

        //        if (ValidateHeaders(CompanyId, UserId))
        //        {
        //            var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.Gst, UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsDelete)
        //                {
        //                    var GstToDelete = await _GstService.GetGstByIdAsync(CompanyId, GstId, UserId);

        //                    if (GstToDelete == null)
        //                        return NotFound($"M_Gst with Id = {GstId} not found");

        //                    var sqlResponce = await _GstService.DeleteGstAsync(CompanyId, GstToDelete, UserId);
        //                    // Remove data from cache by key
        //                    _memoryCache.Remove($"Gst_{GstId}");
        //                    return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
        //                }
        //                else
        //                {
        //                    return NotFound("Users do not have a access to delete");
        //                }
        //            }
        //            else
        //            {
        //                return NotFound("Users not have a access for this screen");
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


