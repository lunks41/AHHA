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
    public class ChartOfAccountController : BaseController
    {
        private readonly IChartOfAccountService _ChartOfAccountService;
        private readonly ILogger<ChartOfAccountController> _logger;

        public ChartOfAccountController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<ChartOfAccountController> logger, IChartOfAccountService ChartOfAccountService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _ChartOfAccountService = ChartOfAccountService;
        }

        [HttpGet, Route("GetChartOfAccount")]
        [Authorize]
        public async Task<ActionResult> GetAllChartOfAccount([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.ChartOfAccount, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _ChartOfAccountService.GetChartOfAccountListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

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

        [HttpGet, Route("GetChartOfAccountbyid/{GLId}")]
        [Authorize]
        public async Task<ActionResult<ChartOfAccountViewModel>> GetChartOfAccountById(Int16 GLId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var ChartOfAccountViewModel = new ChartOfAccountViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.ChartOfAccount, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"ChartOfAccount_{GLId}", out ChartOfAccountViewModel? cachedProduct))
                        {
                            ChartOfAccountViewModel = cachedProduct;
                        }
                        else
                        {
                            ChartOfAccountViewModel = _mapper.Map<ChartOfAccountViewModel>(await _ChartOfAccountService.GetChartOfAccountByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GLId, headerViewModel.UserId));

                            if (ChartOfAccountViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the ChartOfAccount with an expiration time of 10 minutes
                                _memoryCache.Set($"ChartOfAccount_{GLId}", ChartOfAccountViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, ChartOfAccountViewModel);
                        //return Ok(ChartOfAccountViewModel);
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

        [HttpPost, Route("AddChartOfAccount")]
        [Authorize]
        public async Task<ActionResult<ChartOfAccountViewModel>> CreateChartOfAccount(ChartOfAccountViewModel ChartOfAccount, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.ChartOfAccount, headerViewModel.UserId);

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
                                CreateById = headerViewModel.UserId,
                                IsActive = ChartOfAccount.IsActive,
                                Remarks = ChartOfAccount.Remarks
                            };

                            var createdChartOfAccount = await _ChartOfAccountService.AddChartOfAccountAsync(headerViewModel.RegId, headerViewModel.CompanyId, ChartOfAccountEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdChartOfAccount);
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
                    "Error creating new ChartOfAccount record");
            }
        }

        [HttpPut, Route("UpdateChartOfAccount/{GLId}")]
        [Authorize]
        public async Task<ActionResult<ChartOfAccountViewModel>> UpdateChartOfAccount(Int16 GLId, [FromBody] ChartOfAccountViewModel ChartOfAccount, [FromHeader] HeaderViewModel headerViewModel)
        {
            var ChartOfAccountViewModel = new ChartOfAccountViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.ChartOfAccount, headerViewModel.UserId);

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
                                var ChartOfAccountToUpdate = await _ChartOfAccountService.GetChartOfAccountByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GLId, headerViewModel.UserId);

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
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = ChartOfAccount.IsActive,
                                Remarks = ChartOfAccount.Remarks
                            };

                            var sqlResponce = await _ChartOfAccountService.UpdateChartOfAccountAsync(headerViewModel.RegId, headerViewModel.CompanyId, ChartOfAccountEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("Delete/{GLId}")]
        [Authorize]
        public async Task<ActionResult<M_ChartOfAccount>> DeleteChartOfAccount(Int16 GLId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.ChartOfAccount, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var ChartOfAccountToDelete = await _ChartOfAccountService.GetChartOfAccountByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, GLId, headerViewModel.UserId);

                            if (ChartOfAccountToDelete == null)
                                return NotFound($"M_ChartOfAccount with Id = {GLId} not found");

                            var sqlResponce = await _ChartOfAccountService.DeleteChartOfAccountAsync(headerViewModel.RegId, headerViewModel.CompanyId, ChartOfAccountToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"ChartOfAccount_{GLId}");
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