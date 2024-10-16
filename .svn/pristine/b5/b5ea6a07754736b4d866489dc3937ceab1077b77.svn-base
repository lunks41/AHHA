using AHHA.Application.IServices;
using AHHA.Application.IServices.Accounts.AR;
using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AR;
using AHHA.Core.Models.Account;
using AHHA.Core.Models.Account.AR;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AHHA.API.Controllers.Accounts.AR
{
    [Route("api/Account")]
    [ApiController]
    public class ARDebitNoteController : BaseController
    {
        private readonly IARDebitNoteService _ARDebitNoteService;
        private readonly ILogger<ARDebitNoteController> _logger;

        public ARDebitNoteController(IMemoryCache memoryCache, IMapper mapper, IBaseService baseServices, ILogger<ARDebitNoteController> logger, IARDebitNoteService ARDebitNoteService)
    : base(memoryCache, mapper, baseServices)
        {
            _logger = logger;
            _ARDebitNoteService = ARDebitNoteService;
        }

        //All ARDebitNote LIST
        [HttpGet, Route("GetARDebitNote")]
        [Authorize]
        public async Task<ActionResult> GetARDebitNote([FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.DebitNote, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var cacheData = await _ARDebitNoteService.GetARDebitNoteListAsync(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.pageSize, headerViewModel.pageNumber, headerViewModel.searchString.Trim(), headerViewModel.UserId);

                        if (cacheData == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, cacheData);
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

        //GET ONE ARDebitNote BY INVOICEID
        [HttpGet, Route("GetARDebitNotebyId/{DebitNoteId}")]
        [Authorize]
        public async Task<ActionResult<ARDebitNoteViewModel>> GetARDebitNoteById(string DebitNoteId, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.DebitNote, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var ARDebitNoteViewModel = await _ARDebitNoteService.GetARDebitNoteByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(string.IsNullOrEmpty(DebitNoteId) ? 0 : DebitNoteId.Trim()), string.Empty, headerViewModel.UserId);

                        if (ARDebitNoteViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, ARDebitNoteViewModel);
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

        //GET ONE ARDebitNote BY INVOICEID
        [HttpGet, Route("GetARDebitNotebyNo/{DebitNoteNo}")]
        [Authorize]
        public async Task<ActionResult<ARDebitNoteViewModel>> GetARDebitNoteByNo(string DebitNoteNo, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.DebitNote, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        var ARDebitNoteViewModel = await _ARDebitNoteService.GetARDebitNoteByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, 0, DebitNoteNo, headerViewModel.UserId);

                        if (ARDebitNoteViewModel == null)
                            return NotFound(GenrateMessage.datanotfound);

                        return StatusCode(StatusCodes.Status202Accepted, ARDebitNoteViewModel);
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

        //SAVE ONE ARDebitNote BY INVOICEID
        [HttpPost, Route("SaveARDebitNote")]
        [Authorize]
        public async Task<ActionResult<ARDebitNoteViewModel>> SaveARDebitNote(ARDebitNoteViewModel aRDebitNoteViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.DebitNote, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsCreate || userGroupRight.IsEdit)
                        {
                            if (aRDebitNoteViewModel == null)
                                return NotFound(GenrateMessage.datanotfound);

                            //Header Data Mapping
                            ArDebitNoteHd? ARDebitNoteEntity;
                            if (aRDebitNoteViewModel.BalAmt == null)
                            {
                                ARDebitNoteEntity = new ArDebitNoteHd
                                {
                                    CompanyId = headerViewModel.CompanyId,
                                    DebitNoteId = Convert.ToInt64(string.IsNullOrEmpty(aRDebitNoteViewModel.DebitNoteId) ? 0 : aRDebitNoteViewModel.DebitNoteId.Trim()),
                                    DebitNoteNo = aRDebitNoteViewModel.DebitNoteNo,
                                    ReferenceNo = aRDebitNoteViewModel.ReferenceNo == null ? string.Empty : aRDebitNoteViewModel.ReferenceNo,
                                    TrnDate = aRDebitNoteViewModel.TrnDate,
                                    AccountDate = aRDebitNoteViewModel.AccountDate,
                                    DeliveryDate = aRDebitNoteViewModel.DeliveryDate,
                                    DueDate = aRDebitNoteViewModel.DueDate,
                                    CustomerId = aRDebitNoteViewModel.CustomerId,
                                    CurrencyId = aRDebitNoteViewModel.CurrencyId,
                                    ExhRate = aRDebitNoteViewModel.ExhRate == null ? 0 : aRDebitNoteViewModel.ExhRate,
                                    CtyExhRate = aRDebitNoteViewModel.CtyExhRate == null ? 0 : aRDebitNoteViewModel.CtyExhRate,
                                    CreditTermId = aRDebitNoteViewModel.CreditTermId,
                                    BankId = Convert.ToInt16(aRDebitNoteViewModel.BankId == null ? 0 : aRDebitNoteViewModel.BankId),
                                    InvoiceId = Convert.ToInt64(string.IsNullOrEmpty(aRDebitNoteViewModel.InvoiceId) ? 0 : aRDebitNoteViewModel.InvoiceId.Trim()),
                                    InvoiceNo = aRDebitNoteViewModel.InvoiceNo,
                                    TotAmt = aRDebitNoteViewModel.TotAmt == null ? 0 : aRDebitNoteViewModel.TotAmt,
                                    TotLocalAmt = aRDebitNoteViewModel.TotLocalAmt == null ? 0 : aRDebitNoteViewModel.TotLocalAmt,
                                    TotCtyAmt = aRDebitNoteViewModel.TotCtyAmt == null ? 0 : aRDebitNoteViewModel.TotCtyAmt,
                                    GstClaimDate = aRDebitNoteViewModel.GstClaimDate,
                                    GstAmt = aRDebitNoteViewModel.GstAmt == null ? 0 : aRDebitNoteViewModel.GstAmt,
                                    GstLocalAmt = aRDebitNoteViewModel.GstLocalAmt == null ? 0 : aRDebitNoteViewModel.GstLocalAmt,
                                    GstCtyAmt = aRDebitNoteViewModel.GstCtyAmt == null ? 0 : aRDebitNoteViewModel.GstCtyAmt,
                                    TotAmtAftGst = aRDebitNoteViewModel.TotAmtAftGst == null ? 0 : aRDebitNoteViewModel.TotAmtAftGst,
                                    TotLocalAmtAftGst = aRDebitNoteViewModel.TotLocalAmtAftGst == null ? 0 : aRDebitNoteViewModel.TotLocalAmtAftGst,
                                    TotCtyAmtAftGst = aRDebitNoteViewModel.TotCtyAmtAftGst == null ? 0 : aRDebitNoteViewModel.TotCtyAmtAftGst,
                                    BalAmt = 0,
                                    BalLocalAmt = aRDebitNoteViewModel.BalLocalAmt == null ? 0 : aRDebitNoteViewModel.BalLocalAmt,
                                    PayAmt = aRDebitNoteViewModel.PayAmt == null ? 0 : aRDebitNoteViewModel.PayAmt,
                                    PayLocalAmt = aRDebitNoteViewModel.PayLocalAmt == null ? 0 : aRDebitNoteViewModel.PayLocalAmt,
                                    ExGainLoss = aRDebitNoteViewModel.ExGainLoss == null ? 0 : aRDebitNoteViewModel.ExGainLoss,
                                    SalesOrderId = Convert.ToInt64(string.IsNullOrEmpty(aRDebitNoteViewModel.SalesOrderId) == null ? 0 : aRDebitNoteViewModel.SalesOrderId.Trim()),
                                    SalesOrderNo = aRDebitNoteViewModel.SalesOrderNo == null ? string.Empty : aRDebitNoteViewModel.SalesOrderNo,
                                    OperationId = Convert.ToInt64(string.IsNullOrEmpty(aRDebitNoteViewModel.OperationId) == null ? 0 : aRDebitNoteViewModel.OperationId.Trim()),
                                    OperationNo = aRDebitNoteViewModel.OperationNo == null ? string.Empty : aRDebitNoteViewModel.OperationNo,
                                    Remarks = aRDebitNoteViewModel.Remarks == null ? string.Empty : aRDebitNoteViewModel.Remarks,
                                    Address1 = aRDebitNoteViewModel.Address1 == null ? string.Empty : aRDebitNoteViewModel.Address1,
                                    Address2 = aRDebitNoteViewModel.Address2 == null ? string.Empty : aRDebitNoteViewModel.Address2,
                                    Address3 = aRDebitNoteViewModel.Address3 == null ? string.Empty : aRDebitNoteViewModel.Address3,
                                    Address4 = aRDebitNoteViewModel.Address4 == null ? string.Empty : aRDebitNoteViewModel.Address4,
                                    PinCode = aRDebitNoteViewModel.PinCode == null ? string.Empty : aRDebitNoteViewModel.PinCode,
                                    CountryId = aRDebitNoteViewModel.CountryId,
                                    PhoneNo = aRDebitNoteViewModel.PhoneNo == null ? string.Empty : aRDebitNoteViewModel.PhoneNo,
                                    FaxNo = aRDebitNoteViewModel.FaxNo == null ? string.Empty : aRDebitNoteViewModel.FaxNo,
                                    ContactName = aRDebitNoteViewModel.ContactName == null ? string.Empty : aRDebitNoteViewModel.ContactName,
                                    MobileNo = aRDebitNoteViewModel.MobileNo == null ? string.Empty : aRDebitNoteViewModel.MobileNo,
                                    EmailAdd = aRDebitNoteViewModel.EmailAdd == null ? string.Empty : aRDebitNoteViewModel.EmailAdd,
                                    ModuleFrom = aRDebitNoteViewModel.ModuleFrom == null ? string.Empty : aRDebitNoteViewModel.ModuleFrom,
                                    SupplierName = aRDebitNoteViewModel.SupplierName == null ? string.Empty : aRDebitNoteViewModel.SupplierName,
                                    CreateById = headerViewModel.UserId,
                                    EditById = headerViewModel.UserId,
                                    EditDate = DateTime.Now,
                                };
                            }
                            else
                            {
                                if (aRDebitNoteViewModel.ExhRate == null)
                                {
                                    ARDebitNoteEntity = new ArDebitNoteHd
                                    {
                                        CompanyId = headerViewModel.CompanyId,
                                        DebitNoteId = Convert.ToInt64(string.IsNullOrEmpty(aRDebitNoteViewModel.DebitNoteId) ? 0 : aRDebitNoteViewModel.DebitNoteId.Trim()),
                                        DebitNoteNo = aRDebitNoteViewModel.DebitNoteNo,
                                        ReferenceNo = aRDebitNoteViewModel.ReferenceNo == null ? string.Empty : aRDebitNoteViewModel.ReferenceNo,
                                        TrnDate = aRDebitNoteViewModel.TrnDate,
                                        AccountDate = aRDebitNoteViewModel.AccountDate,
                                        DeliveryDate = aRDebitNoteViewModel.DeliveryDate,
                                        DueDate = aRDebitNoteViewModel.DueDate,
                                        CustomerId = aRDebitNoteViewModel.CustomerId,
                                        CurrencyId = aRDebitNoteViewModel.CurrencyId,
                                        ExhRate = 0,
                                        CtyExhRate = aRDebitNoteViewModel.CtyExhRate == null ? 0 : aRDebitNoteViewModel.CtyExhRate,
                                        CreditTermId = aRDebitNoteViewModel.CreditTermId,
                                        BankId = Convert.ToInt16(aRDebitNoteViewModel.BankId == null ? 0 : aRDebitNoteViewModel.BankId),
                                        InvoiceId = Convert.ToInt64(string.IsNullOrEmpty(aRDebitNoteViewModel.InvoiceId) ? 0 : aRDebitNoteViewModel.InvoiceId.Trim()),
                                        InvoiceNo = aRDebitNoteViewModel.InvoiceNo,
                                        TotAmt = aRDebitNoteViewModel.TotAmt == null ? 0 : aRDebitNoteViewModel.TotAmt,
                                        TotLocalAmt = aRDebitNoteViewModel.TotLocalAmt == null ? 0 : aRDebitNoteViewModel.TotLocalAmt,
                                        TotCtyAmt = aRDebitNoteViewModel.TotCtyAmt == null ? 0 : aRDebitNoteViewModel.TotCtyAmt,
                                        GstClaimDate = aRDebitNoteViewModel.GstClaimDate,
                                        GstAmt = aRDebitNoteViewModel.GstAmt == null ? 0 : aRDebitNoteViewModel.GstAmt,
                                        GstLocalAmt = aRDebitNoteViewModel.GstLocalAmt == null ? 0 : aRDebitNoteViewModel.GstLocalAmt,
                                        GstCtyAmt = aRDebitNoteViewModel.GstCtyAmt == null ? 0 : aRDebitNoteViewModel.GstCtyAmt,
                                        TotAmtAftGst = aRDebitNoteViewModel.TotAmtAftGst == null ? 0 : aRDebitNoteViewModel.TotAmtAftGst,
                                        TotLocalAmtAftGst = aRDebitNoteViewModel.TotLocalAmtAftGst == null ? 0 : aRDebitNoteViewModel.TotLocalAmtAftGst,
                                        TotCtyAmtAftGst = aRDebitNoteViewModel.TotCtyAmtAftGst == null ? 0 : aRDebitNoteViewModel.TotCtyAmtAftGst,
                                        BalAmt = aRDebitNoteViewModel.BalAmt,
                                        BalLocalAmt = aRDebitNoteViewModel.BalLocalAmt == null ? 0 : aRDebitNoteViewModel.BalLocalAmt,
                                        PayAmt = aRDebitNoteViewModel.PayAmt == null ? 0 : aRDebitNoteViewModel.PayAmt,
                                        PayLocalAmt = aRDebitNoteViewModel.PayLocalAmt == null ? 0 : aRDebitNoteViewModel.PayLocalAmt,
                                        ExGainLoss = aRDebitNoteViewModel.ExGainLoss == null ? 0 : aRDebitNoteViewModel.ExGainLoss,
                                        SalesOrderId = Convert.ToInt64(string.IsNullOrEmpty(aRDebitNoteViewModel.SalesOrderId) == null ? 0 : aRDebitNoteViewModel.SalesOrderId.Trim()),
                                        SalesOrderNo = aRDebitNoteViewModel.SalesOrderNo == null ? string.Empty : aRDebitNoteViewModel.SalesOrderNo,
                                        OperationId = Convert.ToInt64(string.IsNullOrEmpty(aRDebitNoteViewModel.OperationId) == null ? 0 : aRDebitNoteViewModel.OperationId.Trim()),
                                        OperationNo = aRDebitNoteViewModel.OperationNo == null ? string.Empty : aRDebitNoteViewModel.OperationNo,
                                        Remarks = aRDebitNoteViewModel.Remarks == null ? string.Empty : aRDebitNoteViewModel.Remarks,
                                        Address1 = aRDebitNoteViewModel.Address1 == null ? string.Empty : aRDebitNoteViewModel.Address1,
                                        Address2 = aRDebitNoteViewModel.Address2 == null ? string.Empty : aRDebitNoteViewModel.Address2,
                                        Address3 = aRDebitNoteViewModel.Address3 == null ? string.Empty : aRDebitNoteViewModel.Address3,
                                        Address4 = aRDebitNoteViewModel.Address4 == null ? string.Empty : aRDebitNoteViewModel.Address4,
                                        PinCode = aRDebitNoteViewModel.PinCode == null ? string.Empty : aRDebitNoteViewModel.PinCode,
                                        CountryId = aRDebitNoteViewModel.CountryId,
                                        PhoneNo = aRDebitNoteViewModel.PhoneNo == null ? string.Empty : aRDebitNoteViewModel.PhoneNo,
                                        FaxNo = aRDebitNoteViewModel.FaxNo == null ? string.Empty : aRDebitNoteViewModel.FaxNo,
                                        ContactName = aRDebitNoteViewModel.ContactName == null ? string.Empty : aRDebitNoteViewModel.ContactName,
                                        MobileNo = aRDebitNoteViewModel.MobileNo == null ? string.Empty : aRDebitNoteViewModel.MobileNo,
                                        EmailAdd = aRDebitNoteViewModel.EmailAdd == null ? string.Empty : aRDebitNoteViewModel.EmailAdd,
                                        ModuleFrom = aRDebitNoteViewModel.ModuleFrom == null ? string.Empty : aRDebitNoteViewModel.ModuleFrom,
                                        SupplierName = aRDebitNoteViewModel.SupplierName == null ? string.Empty : aRDebitNoteViewModel.SupplierName,
                                        CreateById = headerViewModel.UserId,
                                        EditById = headerViewModel.UserId,
                                        EditDate = DateTime.Now,
                                    };
                                }
                                else
                                {
                                    ARDebitNoteEntity = new ArDebitNoteHd
                                    {
                                        CompanyId = headerViewModel.CompanyId,
                                        DebitNoteId = Convert.ToInt64(string.IsNullOrEmpty(aRDebitNoteViewModel.DebitNoteId) ? 0 : aRDebitNoteViewModel.DebitNoteId.Trim()),
                                        DebitNoteNo = aRDebitNoteViewModel.DebitNoteNo,
                                        ReferenceNo = aRDebitNoteViewModel.ReferenceNo == null ? string.Empty : aRDebitNoteViewModel.ReferenceNo,
                                        TrnDate = aRDebitNoteViewModel.TrnDate,
                                        AccountDate = aRDebitNoteViewModel.AccountDate,
                                        DeliveryDate = aRDebitNoteViewModel.DeliveryDate,
                                        DueDate = aRDebitNoteViewModel.DueDate,
                                        CustomerId = aRDebitNoteViewModel.CustomerId,
                                        CurrencyId = aRDebitNoteViewModel.CurrencyId,
                                        ExhRate = aRDebitNoteViewModel.ExhRate,
                                        CtyExhRate = aRDebitNoteViewModel.CtyExhRate == null ? 0 : aRDebitNoteViewModel.CtyExhRate,
                                        CreditTermId = aRDebitNoteViewModel.CreditTermId,
                                        BankId = Convert.ToInt16(aRDebitNoteViewModel.BankId == null ? 0 : aRDebitNoteViewModel.BankId),
                                        InvoiceId = Convert.ToInt64(string.IsNullOrEmpty(aRDebitNoteViewModel.InvoiceId) ? 0 : aRDebitNoteViewModel.InvoiceId.Trim()),
                                        InvoiceNo = aRDebitNoteViewModel.InvoiceNo,
                                        TotAmt = aRDebitNoteViewModel.TotAmt == null ? 0 : aRDebitNoteViewModel.TotAmt,
                                        TotLocalAmt = aRDebitNoteViewModel.TotLocalAmt == null ? 0 : aRDebitNoteViewModel.TotLocalAmt,
                                        TotCtyAmt = aRDebitNoteViewModel.TotCtyAmt == null ? 0 : aRDebitNoteViewModel.TotCtyAmt,
                                        GstClaimDate = aRDebitNoteViewModel.GstClaimDate,
                                        GstAmt = aRDebitNoteViewModel.GstAmt == null ? 0 : aRDebitNoteViewModel.GstAmt,
                                        GstLocalAmt = aRDebitNoteViewModel.GstLocalAmt == null ? 0 : aRDebitNoteViewModel.GstLocalAmt,
                                        GstCtyAmt = aRDebitNoteViewModel.GstCtyAmt == null ? 0 : aRDebitNoteViewModel.GstCtyAmt,
                                        TotAmtAftGst = aRDebitNoteViewModel.TotAmtAftGst == null ? 0 : aRDebitNoteViewModel.TotAmtAftGst,
                                        TotLocalAmtAftGst = aRDebitNoteViewModel.TotLocalAmtAftGst == null ? 0 : aRDebitNoteViewModel.TotLocalAmtAftGst,
                                        TotCtyAmtAftGst = aRDebitNoteViewModel.TotCtyAmtAftGst == null ? 0 : aRDebitNoteViewModel.TotCtyAmtAftGst,
                                        BalAmt = aRDebitNoteViewModel.BalAmt,
                                        BalLocalAmt = aRDebitNoteViewModel.BalLocalAmt == null ? 0 : aRDebitNoteViewModel.BalLocalAmt,
                                        PayAmt = aRDebitNoteViewModel.PayAmt == null ? 0 : aRDebitNoteViewModel.PayAmt,
                                        PayLocalAmt = aRDebitNoteViewModel.PayLocalAmt == null ? 0 : aRDebitNoteViewModel.PayLocalAmt,
                                        ExGainLoss = aRDebitNoteViewModel.ExGainLoss == null ? 0 : aRDebitNoteViewModel.ExGainLoss,
                                        SalesOrderId = Convert.ToInt64(string.IsNullOrEmpty(aRDebitNoteViewModel.SalesOrderId) == null ? 0 : aRDebitNoteViewModel.SalesOrderId.Trim()),
                                        SalesOrderNo = aRDebitNoteViewModel.SalesOrderNo == null ? string.Empty : aRDebitNoteViewModel.SalesOrderNo,
                                        OperationId = Convert.ToInt64(string.IsNullOrEmpty(aRDebitNoteViewModel.OperationId) == null ? 0 : aRDebitNoteViewModel.OperationId.Trim()),
                                        OperationNo = aRDebitNoteViewModel.OperationNo == null ? string.Empty : aRDebitNoteViewModel.OperationNo,
                                        Remarks = aRDebitNoteViewModel.Remarks == null ? string.Empty : aRDebitNoteViewModel.Remarks,
                                        Address1 = aRDebitNoteViewModel.Address1 == null ? string.Empty : aRDebitNoteViewModel.Address1,
                                        Address2 = aRDebitNoteViewModel.Address2 == null ? string.Empty : aRDebitNoteViewModel.Address2,
                                        Address3 = aRDebitNoteViewModel.Address3 == null ? string.Empty : aRDebitNoteViewModel.Address3,
                                        Address4 = aRDebitNoteViewModel.Address4 == null ? string.Empty : aRDebitNoteViewModel.Address4,
                                        PinCode = aRDebitNoteViewModel.PinCode == null ? string.Empty : aRDebitNoteViewModel.PinCode,
                                        CountryId = aRDebitNoteViewModel.CountryId,
                                        PhoneNo = aRDebitNoteViewModel.PhoneNo == null ? string.Empty : aRDebitNoteViewModel.PhoneNo,
                                        FaxNo = aRDebitNoteViewModel.FaxNo == null ? string.Empty : aRDebitNoteViewModel.FaxNo,
                                        ContactName = aRDebitNoteViewModel.ContactName == null ? string.Empty : aRDebitNoteViewModel.ContactName,
                                        MobileNo = aRDebitNoteViewModel.MobileNo == null ? string.Empty : aRDebitNoteViewModel.MobileNo,
                                        EmailAdd = aRDebitNoteViewModel.EmailAdd == null ? string.Empty : aRDebitNoteViewModel.EmailAdd,
                                        ModuleFrom = aRDebitNoteViewModel.ModuleFrom == null ? string.Empty : aRDebitNoteViewModel.ModuleFrom,
                                        SupplierName = aRDebitNoteViewModel.SupplierName == null ? string.Empty : aRDebitNoteViewModel.SupplierName,
                                        CreateById = headerViewModel.UserId,
                                        EditById = headerViewModel.UserId,
                                        EditDate = DateTime.Now,
                                    };
                                }
                            }

                            //Details Table data mapping
                            var ARDebitNoteDtEntities = new List<ArDebitNoteDt>();

                            if (aRDebitNoteViewModel.data_details != null)
                            {
                                foreach (var item in aRDebitNoteViewModel.data_details)
                                {
                                    var ARDebitNoteDtEntity = new ArDebitNoteDt
                                    {
                                        DebitNoteId = Convert.ToInt64(string.IsNullOrEmpty(item.DebitNoteId) ? 0 : item.DebitNoteId.Trim()),
                                        DebitNoteNo = item.DebitNoteNo,
                                        ItemNo = item.ItemNo,
                                        SeqNo = item.SeqNo,
                                        DocItemNo = item.DocItemNo,
                                        ProductId = item.ProductId,
                                        GLId = item.GLId,
                                        QTY = item.QTY,
                                        BillQTY = item.BillQTY,
                                        UomId = item.UomId,
                                        UnitPrice = item.UnitPrice,
                                        TotAmt = item.TotAmt,
                                        TotLocalAmt = item.TotLocalAmt,
                                        TotCtyAmt = item.TotCtyAmt,
                                        Remarks = item.Remarks,
                                        GstId = item.GstId,
                                        GstPercentage = item.GstPercentage,
                                        GstAmt = item.GstAmt,
                                        GstLocalAmt = item.GstLocalAmt,
                                        GstCtyAmt = item.GstCtyAmt,
                                        DeliveryDate = item.DeliveryDate,
                                        DepartmentId = item.DepartmentId,
                                        EmployeeId = item.EmployeeId,
                                        PortId = item.PortId,
                                        VesselId = item.VesselId,
                                        BargeId = item.BargeId,
                                        VoyageId = item.VoyageId,
                                        OperationId = Convert.ToInt64(string.IsNullOrEmpty(item.OperationId.Trim())),
                                        OperationNo = item.OperationNo,
                                        OPRefNo = item.OPRefNo,
                                        SalesOrderId = Convert.ToInt64(string.IsNullOrEmpty(item.SalesOrderId.Trim())),
                                        SalesOrderNo = item.SalesOrderNo,
                                        SupplyDate = item.SupplyDate,
                                        SupplierName = item.SupplierName,
                                    };

                                    ARDebitNoteDtEntities.Add(ARDebitNoteDtEntity);
                                }
                            }

                            var sqlResponce = await _ARDebitNoteService.SaveARDebitNoteAsync(headerViewModel.RegId, headerViewModel.CompanyId, ARDebitNoteEntity, ARDebitNoteDtEntities, headerViewModel.UserId);

                            if (sqlResponce.Result > 0)
                            {
                                var customerModel = await _ARDebitNoteService.GetARDebitNoteByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, sqlResponce.Result, string.Empty, headerViewModel.UserId);

                                return StatusCode(StatusCodes.Status202Accepted, customerModel);
                            }

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
                    "Error creating new ARDebitNote record");
            }
        }

        [HttpPost, Route("DeleteARDebitNote")]
        [Authorize]
        public async Task<ActionResult<ARDebitNoteViewModel>> DeleteARDebitNote(DeleteViewModel deleteViewModel, [FromHeader] HeaderViewModel headerViewModel)
        {
            try
            {
                if (ValidateHeaders(headerViewModel.RegId, headerViewModel.CompanyId, headerViewModel.UserId))
                {
                    var userGroupRight = ValidateScreen(headerViewModel.RegId, headerViewModel.CompanyId, (Int16)E_Modules.AR, (Int32)E_AR.DebitNote, headerViewModel.UserId);

                    if (userGroupRight != null)
                    {
                        if (userGroupRight.IsDelete)
                        {
                            if (!string.IsNullOrEmpty(deleteViewModel.DocumentId))
                            {
                                var sqlResponce = await _ARDebitNoteService.DeleteARDebitNoteAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(deleteViewModel.DocumentId), deleteViewModel.CancelRemarks, headerViewModel.UserId);

                                if (sqlResponce.Result > 0)
                                {
                                    var customerModel = await _ARDebitNoteService.GetARDebitNoteByIdAsync(headerViewModel.RegId, headerViewModel.CompanyId, Convert.ToInt64(deleteViewModel.DocumentId), string.Empty, headerViewModel.UserId);

                                    return StatusCode(StatusCodes.Status202Accepted, customerModel);
                                }

                                return StatusCode(StatusCodes.Status204NoContent, sqlResponce);
                            }
                            else
                            {
                                return NotFound(GenrateMessage.datanotfound);
                            }
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