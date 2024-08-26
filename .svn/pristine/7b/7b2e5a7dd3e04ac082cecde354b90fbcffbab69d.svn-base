using AHHA.Application.IServices.Masters;
using AHHA.Application.IServices;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditTermController : BaseController
    {
        private readonly ICreditTermService _CreditTermService;
        private readonly ILogger<CreditTermController> _logger;

        public CreditTermController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<CreditTermController> logger, ICreditTermService CreditTermService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _CreditTermService = CreditTermService;
        }

        [HttpGet, Route("GetCreditTerm")]
        [Authorize]
        public async Task<ActionResult> GetCreditTerm([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CreditTerms, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var CreditTermData = await _CreditTermService.GetCreditTermListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (CreditTermData == null)
                            return NotFound();

                        //_memoryCache.Set<CreditTermViewModelCount>("CreditTerm", cacheData, expirationTime);

                        return StatusCode(StatusCodes.Status202Accepted, CreditTermData);
                        //return StatusCode(StatusCodes.Status202Accepted, sqlResponce);
                        //}
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

        [HttpGet, Route("GetCreditTermbyid/{CreditTermId}")]
        [Authorize]
        public async Task<ActionResult<CreditTermViewModel>> GetCreditTermById(Int32 CreditTermId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CreditTerms, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var CreditTermViewModel = _mapper.Map<CreditTermViewModel>(await _CreditTermService.GetCreditTermByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CreditTermId, headerViewModel.UserId));

                        if (CreditTermViewModel == null)
                            return NotFound();

                        return StatusCode(StatusCodes.Status202Accepted, CreditTermViewModel);
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

        [HttpPost, Route("AddCreditTerm")]
        [Authorize]
        public async Task<ActionResult<CreditTermViewModel>> CreateCreditTerm(CreditTermViewModel CreditTerm, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CreditTerms, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (CreditTerm == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_CreditTerm ID mismatch");

                            var CreditTermEntity = new M_CreditTerm
                            {
                                CompanyId = CreditTerm.CompanyId,
                                CreditTermCode = CreditTerm.CreditTermCode,
                                CreditTermId = CreditTerm.CreditTermId,
                                CreditTermName = CreditTerm.CreditTermName,
                                CreateById = headerViewModel.UserId,
                                IsActive = CreditTerm.IsActive,
                                Remarks = CreditTerm.Remarks
                            };

                            var createdCreditTerm = await _CreditTermService.AddCreditTermAsync(headerViewModel.RegId, headerViewModel.CompanyId, CreditTermEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdCreditTerm);
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
                    "Error creating new CreditTerm record");
            }
        }

        [HttpPut, Route("UpdateCreditTerm/{CreditTermId}")]
        [Authorize]
        public async Task<ActionResult<CreditTermViewModel>> UpdateCreditTerm(int CreditTermId, [FromBody] CreditTermViewModel CreditTerm, [FromHeader] HeaderViewModel headerViewModel)
        {
            var CreditTermViewModel = new CreditTermViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CreditTerms, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (CreditTermId != CreditTerm.CreditTermId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_CreditTerm ID mismatch");

                            //Checking CreditTerm data available or not by using CreditTermId
                            var CreditTermToUpdate = await _CreditTermService.GetCreditTermByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CreditTermId, headerViewModel.UserId);

                            if (CreditTermToUpdate == null)
                                return NotFound($"M_CreditTerm with Id = {CreditTermId} not found");

                            var CreditTermEntity = new M_CreditTerm
                            {
                                CreditTermCode = CreditTerm.CreditTermCode,
                                CreditTermId = CreditTerm.CreditTermId,
                                CreditTermName = CreditTerm.CreditTermName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = CreditTerm.IsActive,
                                Remarks = CreditTerm.Remarks
                            };

                            var sqlResponce = await _CreditTermService.UpdateCreditTermAsync(headerViewModel.RegId, headerViewModel.CompanyId, CreditTermEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteCreditTerm/{CreditTermId}")]
        [Authorize]
        public async Task<ActionResult<M_CreditTerm>> DeleteCreditTerm(int CreditTermId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.CreditTerms, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var CreditTermToDelete = await _CreditTermService.GetCreditTermByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, CreditTermId, headerViewModel.UserId);

                            if (CreditTermToDelete == null)
                                return NotFound($"M_CreditTerm with Id = {CreditTermId} not found");

                            var sqlResponce = await _CreditTermService.DeleteCreditTermAsync(headerViewModel.RegId, headerViewModel.CompanyId, CreditTermToDelete, headerViewModel.UserId);

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