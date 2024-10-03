using AHHA.Application.IServices;
using AHHA.Application.IServices.Setting;
using AHHA.Core.Common;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Setting
{
    [Route("api/Setting")]
    [ApiController]
    public class VisibleFieldsController : BaseController
    {
        private readonly IVisibleFieldsServices _VisibleFieldsServices;
        private readonly ILogger<VisibleFieldsController> _logger;

        public VisibleFieldsController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<VisibleFieldsController> logger, IVisibleFieldsServices VisibleFieldsServices)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _VisibleFieldsServices = VisibleFieldsServices;
        }

        //[HttpGet, Route("GetVisibleFields")]
        //[Authorize]
        //public async Task<ActionResult> GetVisibleFields([FromHeader] HeaderViewModel headerViewModel)
        //{
        //    try
        //    {
        //        if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
        //        {
        //            var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Setting, (Int16)E_Setting.VisibleFields, headerViewModel.UserId);

        //            if (userGroupRight != null)
        //            {
        //                var cacheData = await _VisibleFieldsServices.GetVisibleFieldsListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

        //                if (cacheData == null)
        //                    return NotFound(GenrateMessage.datanotfound);

        //                return StatusCode(StatusCodes.Status202Accepted, cacheData);
        //            }
        //            else
        //            {
        //                return NotFound(GenrateMessage.authenticationfailed);
        //            }
        //        }
        //        else
        //        {
        //            return NotFound(GenrateMessage.authenticationfailed);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //         "Error retrieving data from the database");
        //    }
        //}

        [HttpGet, Route("GetVisibleFieldsbyid/{ModuleId}/{TransactionId}")]
        [Authorize]
        public async Task<ActionResult<VisibleFieldsViewModel>> GetVisibleFieldsById(Int16 ModuleId, Int16 TransactionId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Setting, (Int16)E_Setting.VisibleFields, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var VisibleFieldsViewModel = await _VisibleFieldsServices.GetVisibleFieldsByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, ModuleId, TransactionId, headerViewModel.UserId);

                        if (VisibleFieldsViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, VisibleFieldsViewModel);
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

        [HttpPost, Route("SaveVisibleFields")]
        [Authorize]
        public async Task<ActionResult<VisibleFieldsViewModel>> SaveVisibleFields(VisibleFieldsViewModel VisibleFieldsViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Setting, (Int16)E_Setting.VisibleFields, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (VisibleFieldsViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var VisibleFieldsEntity = new S_VisibleFields
                            {
                                CompanyId = headerViewModel.CompanyId,
                                ModuleId = VisibleFieldsViewModel.ModuleId,
                                TransactionId = VisibleFieldsViewModel.TransactionId,
                                M_ProductId = VisibleFieldsViewModel.M_ProductId,
                                M_QTY = VisibleFieldsViewModel.M_QTY,
                                M_BillQTY = VisibleFieldsViewModel.M_BillQTY,
                                M_UomId = VisibleFieldsViewModel.M_UomId,
                                M_UnitPrice = VisibleFieldsViewModel.M_UnitPrice,
                                M_GstId = VisibleFieldsViewModel.M_GstId,
                                M_DeliveryDate = VisibleFieldsViewModel.M_DeliveryDate,
                                M_DepartmentId = VisibleFieldsViewModel.M_DepartmentId,
                                M_EmployeeId = VisibleFieldsViewModel.M_EmployeeId,
                                M_PortId = VisibleFieldsViewModel.M_PortId,
                                M_VesselId = VisibleFieldsViewModel.M_VesselId,
                                M_BargeId = VisibleFieldsViewModel.M_BargeId,
                                M_VoyageId = VisibleFieldsViewModel.M_VoyageId,
                                M_SupplyDate = VisibleFieldsViewModel.M_SupplyDate,
                                M_BankId = VisibleFieldsViewModel.M_BankId,
                                M_CtyCurr= VisibleFieldsViewModel.M_CtyCurr,
                                CreateById = headerViewModel.UserId,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var createdVisibleFields = await _VisibleFieldsServices.SaveVisibleFieldsAsync(headerViewModel.RegId, headerViewModel.CompanyId, VisibleFieldsEntity, headerViewModel.UserId);

                            return StatusCode(StatusCodes.Status202Accepted, createdVisibleFields);
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
                    "Error creating new VisibleFields record");
            }
        }
    }
}