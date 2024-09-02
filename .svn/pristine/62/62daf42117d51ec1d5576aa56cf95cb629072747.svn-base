using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Services.Masters;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Masters
{
    [Route("api/Master")]
    [ApiController]
    public class SupplierContactController : BaseController
    {
        private readonly ISupplierContactService _SupplierContactService;
        private readonly ILogger<SupplierContactController> _logger;

        public SupplierContactController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<SupplierContactController> logger, ISupplierContactService SupplierContactService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _SupplierContactService = SupplierContactService;
        }

        [HttpGet, Route("GetSupplierContactbySupplierId/{SupplierId}")]
        [Authorize]
        public async Task<ActionResult> GetSupplierContactBySupplierId(Int16 SupplierId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.Supplier, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var SupplierContactViewModel = await _SupplierContactService.GetSupplierContactBySupplierIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierId, headerViewModel.UserId);

                        if (SupplierContactViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, SupplierContactViewModel);
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

        [HttpGet, Route("GetSupplierContactbyid/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<SupplierContactViewModel>> GetSupplierContactById(Int16 ContactId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.SupplierContact, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var supplierContactViewModel = _mapper.Map<SupplierContactViewModel>(await _SupplierContactService.GetSupplierContactByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, ContactId, headerViewModel.UserId));

                        if (supplierContactViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, supplierContactViewModel);
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

        [HttpPost, Route("AddSupplierContact")]
        [Authorize]
        public async Task<ActionResult<SupplierContactViewModel>> CreateSupplierContact(SupplierContactViewModel supplierContactViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.SupplierContact, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate)
                        {
                            if (supplierContactViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var SupplierContactEntity = new M_SupplierContact
                            {
                                ContactId = supplierContactViewModel.ContactId,
                                SupplierId = supplierContactViewModel.SupplierId,
                                ContactName = supplierContactViewModel.ContactName,
                                OtherName = supplierContactViewModel.OtherName,
                                MobileNo = supplierContactViewModel.MobileNo,
                                OffNo = supplierContactViewModel.OffNo,
                                FaxNo = supplierContactViewModel.FaxNo,
                                EmailAdd = supplierContactViewModel.EmailAdd,
                                MessId = supplierContactViewModel.MessId,
                                ContactMessType = supplierContactViewModel.ContactMessType,
                                IsDefault = supplierContactViewModel.IsDefault,
                                IsFinance = supplierContactViewModel.IsFinance,
                                IsSales = supplierContactViewModel.IsSales,
                                IsActive = supplierContactViewModel.IsActive,
                                CreateById = headerViewModel.UserId,
                            };

                            var createdSupplierContact = await _SupplierContactService.AddSupplierContactAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierContactEntity, headerViewModel.UserId);
                            return StatusCode(StatusCodes.Status202Accepted, createdSupplierContact);
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
                    "Error creating new SupplierContact record");
            }
        }

        [HttpPut, Route("UpdateSupplierContact/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<SupplierContactViewModel>> UpdateSupplierContact(Int16 ContactId, [FromBody] SupplierContactViewModel supplierContactViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            var SupplierContactViewModel = new SupplierContactViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.SupplierContact, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsEdit)
                        {
                            if (ContactId != supplierContactViewModel.ContactId)
                                return StatusCode(StatusCodes.Status400BadRequest, "SupplierContact ID mismatch");

                            var SupplierContactToUpdate = await _SupplierContactService.GetSupplierContactByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, ContactId, headerViewModel.UserId);

                            if (SupplierContactToUpdate == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var SupplierContactEntity = new M_SupplierContact
                            {
                                ContactId = supplierContactViewModel.ContactId,
                                SupplierId = supplierContactViewModel.SupplierId,
                                ContactName = supplierContactViewModel.ContactName,
                                OtherName = supplierContactViewModel.OtherName,
                                MobileNo = supplierContactViewModel.MobileNo,
                                OffNo = supplierContactViewModel.OffNo,
                                FaxNo = supplierContactViewModel.FaxNo,
                                EmailAdd = supplierContactViewModel.EmailAdd,
                                MessId = supplierContactViewModel.MessId,
                                ContactMessType = supplierContactViewModel.ContactMessType,
                                IsDefault = supplierContactViewModel.IsDefault,
                                IsFinance = supplierContactViewModel.IsFinance,
                                IsSales = supplierContactViewModel.IsSales,
                                IsActive = supplierContactViewModel.IsActive,
                                EditById = headerViewModel.UserId,
                                EditDate = DateTime.Now,
                            };

                            var sqlResponce = await _SupplierContactService.UpdateSupplierContactAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierContactEntity, headerViewModel.UserId);
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

        [HttpDelete, Route("DeleteSupplierContact/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<M_SupplierContact>> DeleteSupplierContact(Int16 ContactId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.SupplierContact, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            var SupplierContactToDelete = await _SupplierContactService.GetSupplierContactByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, ContactId, headerViewModel.UserId);

                            if (SupplierContactToDelete == null)
                                return NotFound(GenrateMessage.datanotfound);

                            var sqlResponce = await _SupplierContactService.DeleteSupplierContactAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierContactToDelete, headerViewModel.UserId);

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