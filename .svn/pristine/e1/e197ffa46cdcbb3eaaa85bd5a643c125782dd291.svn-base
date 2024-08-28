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
    public class UomController : BaseController
    {
        private readonly IUomService _UomService;
        private readonly ILogger<UomController> _logger;

        public UomController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<UomController> logger, IUomService UomService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _UomService = UomService;
        }

        [HttpGet, Route("GetUom")]
        [Authorize]
        public async Task<ActionResult> GetUom([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Uom, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var UomData = await _UomService.GetUomListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (UomData == null)
                            return NotFound();

                        return StatusCode(StatusCodes.Status202Accepted, UomData);
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

        [HttpGet, Route("GetUombyid/{UomId}")]
        [Authorize]
        public async Task<ActionResult<UomViewModel>> GetUomById(Int16 UomId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Uom, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var uomViewModel = _mapper.Map<UomViewModel>(await _UomService.GetUomByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, UomId, headerViewModel.UserId));

                        if (uomViewModel == null)
                            return NotFound();

                        return StatusCode(StatusCodes.Status202Accepted, uomViewModel);
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

        [HttpPost, Route("AddUom")]
        [Authorize]
        public async Task<ActionResult<UomViewModel>> CreateUom(UomViewModel Uom, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Uom, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (Uom == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "Uom ID mismatch");

                            var UomEntity = new M_Uom
                            {
                                CompanyId = Uom.CompanyId,
                                UomCode = Uom.UomCode,
                                UomId = Uom.UomId,
                                UomName = Uom.UomName,
                                CreateById = headerViewModel.UserId,
                                IsActive = Uom.IsActive,
                                Remarks = Uom.Remarks
                            };

                            var createdUom = await _UomService.AddUomAsync(headerViewModel.RegId, headerViewModel.CompanyId, UomEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdUom);
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
                    "Error creating new Uom record");
            }
        }

        [HttpPut, Route("UpdateUom/{UomId}")]
        [Authorize]
        public async Task<ActionResult<UomViewModel>> UpdateUom(Int16 UomId, [FromBody] UomViewModel Uom, [FromHeader] HeaderViewModel headerViewModel)
        {
            var UomViewModel = new UomViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Uom, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (UomId != Uom.UomId)
                                return StatusCode(StatusCodes.Status400BadRequest, "Uom ID mismatch");

                            var UomToUpdate = await _UomService.GetUomByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, UomId, headerViewModel.UserId);

                            if (UomToUpdate == null)
                                return NotFound($"Uom with Id = {UomId} not found");

                            var UomEntity = new M_Uom
                            {
                                UomCode = Uom.UomCode,
                                UomId = Uom.UomId,
                                UomName = Uom.UomName,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                                IsActive = Uom.IsActive,
                                Remarks = Uom.Remarks
                            };

                            var sqlResponce = await _UomService.UpdateUomAsync(headerViewModel.RegId, headerViewModel.CompanyId, UomEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteUom/{UomId}")]
        [Authorize]
        public async Task<ActionResult<M_Uom>> DeleteUom(Int16 UomId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Uom, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var UomToDelete = await _UomService.GetUomByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, UomId, headerViewModel.UserId);

                            if (UomToDelete == null)
                                return NotFound($"Uom with Id = {UomId} not found");

                            var sqlResponce = await _UomService.DeleteUomAsync(headerViewModel.RegId, headerViewModel.CompanyId, UomToDelete, headerViewModel.UserId);

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