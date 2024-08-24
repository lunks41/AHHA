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

        [HttpGet, Route("GetSupplierContact")]
        [Authorize]
        public async Task<ActionResult> GetSupplierContact([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.SupplierContact, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        

                        var voyageData = await _SupplierContactService.GetSupplierContactListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString, headerViewModel.UserId);

                        if (voyageData == null)
                            return NotFound(GenrateMessage.authenticationfailed);

                        return StatusCode(StatusCodes.Status202Accepted, voyageData);
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

        [HttpGet, Route("GetSupplierContactbyid/{ContactId}")]
        [Authorize]
        public async Task<ActionResult<SupplierContactViewModel>> GetSupplierContactById(Int16 ContactId, [FromHeader] HeaderViewModel headerViewModel)
        {
            var SupplierContactViewModel = new SupplierContactViewModel();
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)Modules.Master, (Int32)Master.SupplierContact, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (_memoryCache.TryGetValue($"SupplierContact_{ContactId}", out SupplierContactViewModel? cachedProduct))
                        {
                            SupplierContactViewModel = cachedProduct;
                        }
                        else
                        {
                            SupplierContactViewModel = _mapper.Map<SupplierContactViewModel>(await _SupplierContactService.GetSupplierContactByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, ContactId, headerViewModel.UserId));

                            if (SupplierContactViewModel == null)
                                return NotFound(GenrateMessage.authenticationfailed);
                            else
                                // Cache the SupplierContact with an expiration time of 10 minutes
                                _memoryCache.Set($"SupplierContact_{ContactId}", SupplierContactViewModel, TimeSpan.FromMinutes(10));
                        }
                        return StatusCode(StatusCodes.Status202Accepted, SupplierContactViewModel);
                        //return Ok(SupplierContactViewModel);
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
        public async Task<ActionResult<SupplierContactViewModel>> CreateSupplierContact(SupplierContactViewModel SupplierContact, [FromHeader] HeaderViewModel headerViewModel)
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
                            if (SupplierContact == null)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_SupplierContact ID mismatch");

                            var SupplierContactEntity = new M_SupplierContact
                            {
                                ContactId = SupplierContact.ContactId,
                                SupplierId = SupplierContact.SupplierId,
                                ContactName = SupplierContact.ContactName,
                                OtherName = SupplierContact.OtherName,
                                MobileNo = SupplierContact.MobileNo,
                                OffNo = SupplierContact.OffNo,
                                FaxNo = SupplierContact.FaxNo,
                                EmailAdd = SupplierContact.EmailAdd,
                                MessId = SupplierContact.MessId,
                                ContactMessType = SupplierContact.ContactMessType,
                                IsDefault = SupplierContact.IsDefault,
                                IsFinance = SupplierContact.IsFinance,
                                IsSales = SupplierContact.IsSales,
                                CreateById = headerViewModel.UserId,
                                IsActive = SupplierContact.IsActive
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
        public async Task<ActionResult<SupplierContactViewModel>> UpdateSupplierContact(Int16 ContactId, [FromBody] SupplierContactViewModel SupplierContact, [FromHeader] HeaderViewModel headerViewModel)
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
                            if (ContactId != SupplierContact.ContactId)
                                return StatusCode(StatusCodes.Status400BadRequest, "M_SupplierContact ID mismatch");
                            //return BadRequest("M_SupplierContact ID mismatch");

                            // Attempt to retrieve the SupplierContact from the cache
                            if (_memoryCache.TryGetValue($"SupplierContact_{ContactId}", out SupplierContactViewModel? cachedProduct))
                            {
                                SupplierContactViewModel = cachedProduct;
                            }
                            else
                            {
                                var SupplierContactToUpdate = await _SupplierContactService.GetSupplierContactByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, ContactId, headerViewModel.UserId);

                                if (SupplierContactToUpdate == null)
                                    return NotFound($"M_SupplierContact with Id = {ContactId} not found");
                            }

                            var SupplierContactEntity = new M_SupplierContact
                            {
                                ContactId = SupplierContact.ContactId,
                                SupplierId = SupplierContact.SupplierId,
                                ContactName = SupplierContact.ContactName,
                                OtherName = SupplierContact.OtherName,
                                MobileNo = SupplierContact.MobileNo,
                                OffNo = SupplierContact.OffNo,
                                FaxNo = SupplierContact.FaxNo,
                                EmailAdd = SupplierContact.EmailAdd,
                                MessId = SupplierContact.MessId,
                                ContactMessType = SupplierContact.ContactMessType,
                                IsDefault = SupplierContact.IsDefault,
                                IsFinance = SupplierContact.IsFinance,
                                IsSales = SupplierContact.IsSales,
                                CreateById = headerViewModel.UserId,
                                IsActive = SupplierContact.IsActive
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
                                return NotFound($"M_SupplierContact with Id = {ContactId} not found");

                            var sqlResponce = await _SupplierContactService.DeleteSupplierContactAsync(headerViewModel.RegId, headerViewModel.CompanyId, SupplierContactToDelete, headerViewModel.UserId);
                            // Remove data from cache by key
                            _memoryCache.Remove($"SupplierContact_{ContactId}");
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