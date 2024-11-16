using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Accounts;
using AHHA.Application.IServices.Accounts.CB;
using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.CB;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Account.CB;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AHHA.Infra.Services.Accounts.CB
{
    public sealed class CBBankTransferService : ICBBankTransferService
    {
        private readonly IRepository<CBBankTransfer> _repository;
        private ApplicationDbContext _context;
        private readonly IAccountService _accountService;

        public CBBankTransferService(IRepository<CBBankTransfer> repository, ApplicationDbContext context, IAccountService accountService)
        {
            _repository = repository;
            _context = context;
            _accountService = accountService;
        }

        public async Task<CBBankTransferViewModelList> GetCBBankTransferListAsync(string RegId, Int16 CompanyId, short pageSize, short pageNumber, string searchString, Int16 UserId)
        {
            CBBankTransferViewModelList countViewModel = new CBBankTransferViewModelList();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM dbo.CBBankTransfer Invhd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Invhd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.TransferNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%')");
                var result = await _repository.GetQueryAsync<CBBankTransferViewModel>(RegId, $"SELECT Invhd.CompanyId,Invhd.TransferId,Invhd.TransferNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.DeliveryDate,Invhd.DueDate,Invhd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.BalAmt,Invhd.BalLocalAmt,Invhd.PayAmt,Invhd.PayLocalAmt,Invhd.ExGainLoss,Invhd.SalesOrderId,Invhd.SalesOrderNo,Invhd.OperationId,Invhd.OperationNo,Invhd.Remarks,Invhd.Address1,Invhd.Address2,Invhd.Address3,Invhd.Address4,Invhd.PinCode,Invhd.CountryId,M_Cun.CountryCode,M_Cun.CountryName,Invhd.PhoneNo,Invhd.FaxNo,Invhd.ContactName,Invhd.MobileNo,Invhd.EmailAdd,Invhd.ModuleFrom,Invhd.SupplierName,Invhd.SuppTransferNo,Invhd.APTransferId,Invhd.APTransferNo,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy FROM dbo.CBBankTransfer Invhd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Invhd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.TransferNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') ORDER BY Invhd.AccountDate Desc,Invhd.TransferNo Desc OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result == null ? null : result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Invoice,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "CBBankTransfer",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CBBankTransferViewModel> GetCBBankTransferByIdAsync(string RegId, Int16 CompanyId, Int64 TransferId, string TransferNo, Int16 UserId)
        {
            CBBankTransferViewModel CBBankTransferViewModelList = new CBBankTransferViewModel();
            try
            {
                CBBankTransferViewModelList = await _repository.GetQuerySingleOrDefaultAsync<CBBankTransferViewModel>(RegId, $"SELECT Invhd.CompanyId,Invhd.TransferId,Invhd.TransferNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.DeliveryDate,Invhd.DueDate,Invhd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.BalAmt,Invhd.BalLocalAmt,Invhd.PayAmt,Invhd.PayLocalAmt,Invhd.ExGainLoss,Invhd.SalesOrderId,Invhd.SalesOrderNo,Invhd.OperationId,Invhd.OperationNo,Invhd.Remarks,Invhd.Address1,Invhd.Address2,Invhd.Address3,Invhd.Address4,Invhd.PinCode,Invhd.CountryId,M_Cun.CountryCode,M_Cun.CountryName,Invhd.PhoneNo,Invhd.FaxNo,Invhd.ContactName,Invhd.MobileNo,Invhd.EmailAdd,Invhd.ModuleFrom,Invhd.SupplierName,Invhd.SuppTransferNo,Invhd.APTransferId,Invhd.APTransferNo,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.IsCancel,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy FROM dbo.CBBankTransfer Invhd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Invhd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.TransferId={TransferId} OR {TransferId}=0) AND (Invhd.TransferNo='{TransferNo}' OR '{TransferNo}'='{string.Empty}')");

                if (CBBankTransferViewModelList == null)
                    return CBBankTransferViewModelList;

                return CBBankTransferViewModelList;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Invoice,
                    DocumentId = TransferId,
                    DocumentNo = TransferNo,
                    TblName = "CBBankTransfer",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveCBBankTransferAsync(string RegId, Int16 CompanyId, CBBankTransfer CBBankTransfer, Int16 UserId)
        {
            bool IsEdit = false;
            string accountDate = CBBankTransfer.AccountDate.ToString("dd/MMM/yyyy");
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (CBBankTransfer.TransferId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.CBBankTransfer WHERE IsCancel=0 And CompanyId={CompanyId} And TransferId={CBBankTransfer.TransferId}");

                        if (DataExist.Count() == 0)
                            return new SqlResponce { Result = -1, Message = "Invoice Not Exist" };
                    }

                    if (!IsEdit)
                    {
                        var documentIdNo = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec S_GENERATE_NUMBER_NOANDID {CompanyId},{(short)E_Modules.AR},{(short)E_AR.Invoice},'{accountDate}'");

                        if (documentIdNo.ToList()[0].DocumentId > 0 && documentIdNo.ToList()[0].DocumentNo != string.Empty)
                        {
                            CBBankTransfer.TransferId = documentIdNo.ToList()[0].DocumentId;
                            CBBankTransfer.TransferNo = documentIdNo.ToList()[0].DocumentNo;
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Invoice Number can't generate" };
                    }
                    else
                    {
                        await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec FIN_AR_CreateHistoryRec {CompanyId},{UserId},{CBBankTransfer.TransferId},{(short)E_AR.Invoice}");
                    }

                    //Saving Header
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(CBBankTransfer);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.CompanyId).IsModified = false;
                        entityHead.Property(b => b.EditVersion).IsModified = false;

                        entityHead.Property(b => b.IsCancel).IsModified = false;
                        entityHead.Property(b => b.CancelById).IsModified = false;
                        entityHead.Property(b => b.CancelDate).IsModified = false;
                        entityHead.Property(b => b.CancelRemarks).IsModified = false;
                    }
                    else
                    {
                        CBBankTransfer.EditDate = null;
                        CBBankTransfer.EditById = null;

                        var entityHead = _context.Add(CBBankTransfer);

                        entityHead.Property(b => b.IsCancel).IsModified = false;
                        entityHead.Property(b => b.CancelById).IsModified = false;
                        entityHead.Property(b => b.CancelDate).IsModified = false;
                        entityHead.Property(b => b.CancelRemarks).IsModified = false;
                    }

                    var SaveHeader = _context.SaveChanges();

                    //Saving Details
                    if (SaveHeader > 0)
                    {
                        #region Save AuditLog

                        //Inserting the records into AR CreateStatement
                        await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec FIN_AR_CreateStatement {CompanyId},{UserId},{CBBankTransfer.TransferId},{(short)E_AR.Invoice}");

                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.AR,
                            TransactionId = (short)E_AR.Invoice,
                            DocumentId = CBBankTransfer.TransferId,
                            DocumentNo = CBBankTransfer.TransferNo,
                            TblName = "CBBankTransfer",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = CBBankTransfer.Remarks,
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            //Update Edit Version
                            if (IsEdit)
                                await _repository.UpsertExecuteScalarAsync(RegId, $"update CBBankTransfer set EditVersion=EditVersion+1 where TransferId={CBBankTransfer.TransferId}");

                            //Create / Update Ar Statement
                            await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec FIN_AR_CreateStatement {CompanyId},{UserId},{CBBankTransfer.TransferId},{(short)E_AR.Invoice}");

                            TScope.Complete();
                            return new SqlResponce { Result = CBBankTransfer.TransferId, Message = "Save Successfully" };
                        }

                        #endregion Save AuditLog
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "Id Should not be zero" };
                    }
                    return new SqlResponce();
                }
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Invoice,
                    DocumentId = CBBankTransfer.TransferId,
                    DocumentNo = CBBankTransfer.TransferNo,
                    TblName = "CBBankTransfer",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw ex;
            }
        }

        public async Task<SqlResponce> DeleteCBBankTransferAsync(string RegId, Int16 CompanyId, Int64 TransferId, string TransferNo, string CanacelRemarks, Int16 UserId)
        {
            try
            {
                using (TransactionScope TScope = new TransactionScope())
                {
                    //Get Invoice Number
                    TransferNo = await _repository.GetQuerySingleOrDefaultAsync<string>(RegId, $"SELECT TransferNo FROM dbo.CBBankTransfer WHERE TransferId={TransferId}");

                    if (TransferId > 0)
                    {
                        //Update IsCancle=1,Cancelby=userid,Canceldate=now,CancelRemarks=CancelRemarks
                        var CBBankTransferToRemove = _context.CBBankTransfer.Where(b => b.TransferId == TransferId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsCancel, true).SetProperty(b => b.CancelById, UserId).SetProperty(b => b.CancelDate, DateTime.Now).SetProperty(b => b.CancelRemarks, CanacelRemarks));

                        if (CBBankTransferToRemove > 0)
                        {
                            //Cancel the Ar Transactions.
                            await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec FIN_AR_DeleteStatement {CompanyId},{UserId},{TransferId},{(short)E_AR.Invoice}");

                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AR,
                                TransactionId = (short)E_AR.Invoice,
                                DocumentId = TransferId,
                                DocumentNo = TransferNo,
                                TblName = "CBBankTransfer",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = CanacelRemarks,
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponce { Result = 1, Message = "Cancel Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Cancel Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "Invoice Not exists" };
                    }
                    return new SqlResponce();
                }
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Invoice,
                    DocumentId = TransferId,
                    DocumentNo = TransferNo,
                    TblName = "CBBankTransfer",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
    }
}