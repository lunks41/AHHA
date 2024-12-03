﻿using AHHA.Application.IServices;
using AHHA.Application.IServices.Setting;
using AHHA.Core.Common;
using AHHA.Core.Models.Setting;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Setting
{
    [Route("api/Setting")]
    [ApiController]
    public class DocSeqNoController : BaseController
    {
        private readonly IDocSeqNoService _DocSeqNoServices;
        private readonly ILogger<DocSeqNoController> _logger;

        public DocSeqNoController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<DocSeqNoController> logger, IDocSeqNoService DocSeqNoServices)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _DocSeqNoServices = DocSeqNoServices;
        }

        //[HttpGet, Route("GetDocSeqNo")]
        //[Authorize]
        //public async Task<ActionResult> GetDocSeqNo([FromHeader] HeaderViewModel headerViewModel)
        //{
        //    try
        //    {
        //        if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
        //        {
        //            //var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Setting, (Int16)E_Setting.DocSeqNo, headerViewModel.UserId);

        //            //if (userGroupRight != null)
        //            //{
        //                var cacheData = await _DocSeqNoServices.GetDocSeqNoListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId);

        //                if (cacheData == null)
        //                    return NotFound(GenrateMessage.datanotfound);

        //                return StatusCode(StatusCodes.Status202Accepted, cacheData);
        //            //}
        //            //else
        //            //{
        //            //    return NotFound(GenrateMessage.authenticationfailed);
        //            //}
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

        [HttpGet, Route("GetDocSeqNobyTransaction")]
        [Authorize]
        public async Task<ActionResult<DocSeqNoViewModel>> GetDocSeqNoByTransactionAsync([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    //var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Setting, (Int16)E_Setting.DocSeqNo, headerViewModel.UserId);

                    //if (userGroupRight != null)
                    //{
                    var DocSeqNoViewModel = _mapper.Map<DocSeqNoViewModel>(await _DocSeqNoServices.GetDocSeqNoByTransactionAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.ModuleId, headerViewModel.TransactionId, headerViewModel.UserId));

                    if (DocSeqNoViewModel == null)
                        return NotFound(GenrateMessage.datanotfound);

                    return StatusCode(StatusCodes.Status202Accepted, DocSeqNoViewModel);
                    //}
                    //else
                    //{
                    //    return NotFound(GenrateMessage.authenticationfailed);
                    //}
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

        //[HttpPost, Route("SaveDocSeqNo")]
        //[Authorize]
        //public async Task<ActionResult<DocSeqNoViewModel>> SaveDocSeqNo(DocSeqNoViewModel DocSeqNoViewModel, [FromHeader] HeaderViewModel headerViewModel)
        //{
        //    try
        //    {
        //        if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
        //        {
        //            var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.Setting, (Int16)E_Setting.DocSeqNo, headerViewModel.UserId);

        //            if (userGroupRight != null)
        //            {
        //                if (userGroupRight.IsCreate)
        //                {
        //                    if (DocSeqNoViewModel == null)
        //                        return NotFound(GenrateMessage.datanotfound);

        //                    var DocSeqNoEntity = new S_DocSeqNo
        //                    {
        //                        CompanyId = headerViewModel.CompanyId,
        //                        ModuleId = DocSeqNoViewModel.ModuleId,
        //                        TransactionId = DocSeqNoViewModel.TransactionId,
        //                        M_ProductId = DocSeqNoViewModel.M_ProductId,
        //                        M_GLId = DocSeqNoViewModel.M_GLId,
        //                        M_QTY = DocSeqNoViewModel.M_QTY,
        //                        M_UomId = DocSeqNoViewModel.M_UomId,
        //                        M_UnitPrice = DocSeqNoViewModel.M_UnitPrice,
        //                        M_TotAmt = DocSeqNoViewModel.M_TotAmt,
        //                        M_Remarks = DocSeqNoViewModel.M_Remarks,
        //                        M_GstId = DocSeqNoViewModel.M_GstId,
        //                        M_DeliveryDate = DocSeqNoViewModel.M_DeliveryDate,
        //                        M_DepartmentId = DocSeqNoViewModel.M_DepartmentId,
        //                        M_EmployeeId = DocSeqNoViewModel.M_EmployeeId,
        //                        M_PortId = DocSeqNoViewModel.M_PortId,
        //                        M_VesselId = DocSeqNoViewModel.M_VesselId,
        //                        M_BargeId = DocSeqNoViewModel.M_BargeId,
        //                        M_VoyageId = DocSeqNoViewModel.M_VoyageId,
        //                        M_SupplyDate = DocSeqNoViewModel.M_SupplyDate,
        //                        M_ReferenceNo = DocSeqNoViewModel.M_ReferenceNo,
        //                        M_SuppInvoiceNo = DocSeqNoViewModel.M_SuppInvoiceNo,
        //                        M_BankId = DocSeqNoViewModel.M_BankId,
        //                        M_Remarks_Hd = DocSeqNoViewModel.M_Remarks_Hd,
        //                        M_Address1 = DocSeqNoViewModel.M_Address1,
        //                        M_Address2 = DocSeqNoViewModel.M_Address2,
        //                        M_Address3 = DocSeqNoViewModel.M_Address3,
        //                        M_Address4 = DocSeqNoViewModel.M_Address4,
        //                        M_PinCode = DocSeqNoViewModel.M_PinCode,
        //                        M_CountryId = DocSeqNoViewModel.M_CountryId,
        //                        M_PhoneNo = DocSeqNoViewModel.M_PhoneNo,
        //                        M_ContactName = DocSeqNoViewModel.M_ContactName,
        //                        M_MobileNo = DocSeqNoViewModel.M_MobileNo,
        //                        M_EmailAdd = DocSeqNoViewModel.M_EmailAdd,
        //                        CreateById = headerViewModel.UserId,
        //                        EditById = headerViewModel.UserId,
        //                        EditDate = DateTime.Now,
        //                    };

        //                    var createdDocSeqNo = await _DocSeqNoServices.SaveDocSeqNoAsync(headerViewModel.RegId, headerViewModel.CompanyId, DocSeqNoEntity, headerViewModel.UserId);

        //                    return StatusCode(StatusCodes.Status202Accepted, createdDocSeqNo);
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
        //            "Error creating new DocSeqNo record");
        //    }
        //}
    }
}