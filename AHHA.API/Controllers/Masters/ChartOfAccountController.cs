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
    public class ChartOfAccountController : BaseController
    {
        private readonly IChartOfAccountService _ChartOfAccountService;
        private readonly ILogger<ChartOfAccountController> _logger;
        private Int16 CompanyId = 0;
        private Int32 UserId = 0;
        private Int32 RegId = 0;
        private Int16 pageSize = 10;
        private Int16 pageNumber = 1;
        private string searchString = string.Empty;

        public ChartOfAccountController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<ChartOfAccountController> logger, IChartOfAccountService ChartOfAccountService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _ChartOfAccountService = ChartOfAccountService;
        }

        [HttpGet, Route("GetChartOfAccount")]
        [Authorize]
        public async Task<ActionResult> GetAllChartOfAccount()
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));
                RegId = Convert.ToInt32(Request.Headers.TryGetValue("regId", out StringValues regIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.ChartOfAccount, UserId);

                    if (userGroupRight != null)
                    {
                        pageSize = (Request.Headers.TryGetValue("pageSize", out StringValues pageSizeValue)) == true ? Convert.ToInt16(pageSizeValue[0]) : pageSize;
                        pageNumber = (Request.Headers.TryGetValue("pageNumber", out StringValues pageNumberValue)) == true ? Convert.ToInt16(pageNumberValue[0]) : pageNumber;
                        searchString = (Request.Headers.TryGetValue("searchString", out StringValues searchStringValue)) == true ? searchStringValue.ToString() : searchString;
                        //_logger.LogWarning("Warning: Some simple condition is met."); // Log a warning

                        //Get the data from cache memory
                        var cacheData = _memoryCache.Get<ChartOfAccountViewModelCount>("ChartOfAccount");

                        if (cacheData != null)
                            return StatusCode(StatusCodes.Status202Accepted, cacheData);
                        //return Ok(cacheData);
                        else
                        {
                            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
                            cacheData = await _ChartOfAccountService.GetChartOfAccountListAsync(CompanyId, pageSize, pageNumber, searchString.Trim(), UserId);

                            if (cacheData == null)
                                return NotFound();

                            _memoryCache.Set<ChartOfAccountViewModelCount>("ChartOfAccount", cacheData, expirationTime);

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

        [HttpGet, Route("GetChartOfAccountbyid/{GLId}")]
        [Authorize]
        public async Task<ActionResult<ChartOfAccountViewModel>> GetChartOfAccountById(Int16 GLId)
        {
            var ChartOfAccountViewModel = new ChartOfAccountViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.ChartOfAccount, UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"ChartOfAccount_{GLId}", out ChartOfAccountViewModel? cachedProduct))
                        {
                            ChartOfAccountViewModel = cachedProduct;
                        }
                        else
                        {
                            ChartOfAccountViewModel = _mapper.Map<ChartOfAccountViewModel>(await _ChartOfAccountService.GetChartOfAccountByIdAsync(CompanyId, GLId, UserId));

                            if (ChartOfAccountViewModel == null)
                                return NotFound();
                            else
                                // Cache the ChartOfAccount with an expiration time of 10 minutes
                                _memoryCache.Set($"ChartOfAccount_{GLId}", ChartOfAccountViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, ChartOfAccountViewModel);
                        //return Ok(ChartOfAccountViewModel);
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

        [HttpPost, Route("AddChartOfAccount")]
        [Authorize]
        public async Task<ActionResult<ChartOfAccountViewModel>> CreateChartOfAccount(ChartOfAccountViewModel ChartOfAccount)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.ChartOfAccount, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (ChartOfAccount == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_ChartOfAccount ID mismatch");

                            var ChartOfAccountEntity = new M_ChartOfAccount
                            {
                                CompanyId = ChartOfAccount.CompanyId,
                                GLId = ChartOfAccount.GLId,
                                GLCode = ChartOfAccount.GLCode,
                                GLName = ChartOfAccount.GLName,
                                AccTypeId = ChartOfAccount.AccTypeId,
                                AccGroupId = ChartOfAccount.AccGroupId,
                                COACategoryId1 = ChartOfAccount.COACategoryId1,
                                COACategoryId2 = ChartOfAccount.COACategoryId2,
                                COACategoryId3 = ChartOfAccount.COACategoryId3,
                                IsSysControl = ChartOfAccount.IsSysControl,
                                SeqNo = ChartOfAccount.SeqNo,
                                CreateById = UserId,
                                IsActive = ChartOfAccount.IsActive,
                                Remarks = ChartOfAccount.Remarks
                            };

                            var createdChartOfAccount = await _ChartOfAccountService.AddChartOfAccountAsync(CompanyId, ChartOfAccountEntity, UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdChartOfAccount);

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
                    "Error creating new ChartOfAccount record");
            }
        }

        [HttpPut, Route("UpdateChartOfAccount/{GLId}")]
        [Authorize]
        public async Task<ActionResult<ChartOfAccountViewModel>> UpdateChartOfAccount(Int16 GLId, [FromBody] ChartOfAccountViewModel ChartOfAccount)
        {
            var ChartOfAccountViewModel = new ChartOfAccountViewModel();
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.ChartOfAccount, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (GLId != ChartOfAccount.GLId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_ChartOfAccount ID mismatch");
                            //return BadRequest("M_ChartOfAccount ID mismatch");

                            // Attempt to retrieve the ChartOfAccount from the cache
                            if (_memoryCache.TryGetValue($"ChartOfAccount_{GLId}", out ChartOfAccountViewModel? cachedProduct))
                            {
                                ChartOfAccountViewModel = cachedProduct;
                            }
                            else
                            {
                                var ChartOfAccountToUpdate = await _ChartOfAccountService.GetChartOfAccountByIdAsync(CompanyId, GLId, UserId);

                                if (ChartOfAccountToUpdate == null)
                                    return NotFound($"M_ChartOfAccount with Id = {GLId} not found");
                            }

                            var ChartOfAccountEntity = new M_ChartOfAccount
                            {
                                CompanyId = ChartOfAccount.CompanyId,
                                GLId = ChartOfAccount.GLId,
                                GLCode = ChartOfAccount.GLCode,
                                GLName = ChartOfAccount.GLName,
                                AccTypeId = ChartOfAccount.AccTypeId,
                                AccGroupId = ChartOfAccount.AccGroupId,
                                COACategoryId1 = ChartOfAccount.COACategoryId1,
                                COACategoryId2 = ChartOfAccount.COACategoryId2,
                                COACategoryId3 = ChartOfAccount.COACategoryId3,
                                IsSysControl = ChartOfAccount.IsSysControl,
                                SeqNo = ChartOfAccount.SeqNo,
                                EditById = UserId,
                                EditDate = DateTime.Now,
                                IsActive = ChartOfAccount.IsActive,
                                Remarks = ChartOfAccount.Remarks
                            };

                            var sqlResponce = await _ChartOfAccountService.UpdateChartOfAccountAsync(CompanyId, ChartOfAccountEntity, UserId);
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

        [HttpDelete, Route("Delete/{GLId}")]
        [Authorize]
        public async Task<ActionResult<M_ChartOfAccount>> DeleteChartOfAccount(Int16 GLId)
        {
            try
            {
                CompanyId = Convert.ToInt16(Request.Headers.TryGetValue("companyId", out StringValues headerValue));
                UserId = Convert.ToInt32(Request.Headers.TryGetValue("userId", out StringValues userIdValue));

                if (ValidateHeaders(CompanyId, UserId))
                {
                    var userGroupRight = ValidateScreen(CompanyId, (Int16)Modules.Master, (Int32)Master.ChartOfAccount, UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var ChartOfAccountToDelete = await _ChartOfAccountService.GetChartOfAccountByIdAsync(CompanyId, GLId, UserId);

                            if (ChartOfAccountToDelete == null)
                                return NotFound($"M_ChartOfAccount with Id = {GLId} not found");

                            var sqlResponce = await _ChartOfAccountService.DeleteChartOfAccountAsync(CompanyId, ChartOfAccountToDelete, UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"ChartOfAccount_{GLId}");
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

