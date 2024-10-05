﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Accounts;
using AHHA.Application.IServices.Accounts.AR;
using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AR;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Account.AR;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AHHA.Infra.Services.Accounts.AR
{
    public sealed class ARReceiptService : IARReceiptService
    {
        private readonly IRepository<ArReceiptHd> _repository;
        private ApplicationDbContext _context;
        private readonly IAccountService _accountService;

        public ARReceiptService(IRepository<ArReceiptHd> repository, ApplicationDbContext context, IAccountService accountService)
        {
            _repository = repository;
            _context = context;
            _accountService = accountService;
        }

        public async Task<ARReceiptViewModelCount> GetARReceiptListAsync(string RegId, Int16 CompanyId, short pageSize, short pageNumber, string searchString, Int16 UserId)
        {
            ARReceiptViewModelCount countViewModel = new ARReceiptViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM dbo.ArReceiptHd Invhd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Invhd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.ReceiptNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%')");

                var result = await _repository.GetQueryAsync<ARReceiptViewModel>(RegId, $"SELECT Invhd.CompanyId,Invhd.ReceiptId,Invhd.ReceiptNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.Remarks,Invhd.ModuleFrom,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.CancelById,Invhd.CancelDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy FROM dbo.ArReceiptHd Invhd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Invhd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.ReceiptNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%')  ORDER BY Invhd.ReceiptNo,Invhd.AccountDate OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_AR.Receipt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "ArReceiptHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<ARReceiptViewModel> GetARReceiptByIdAsync(string RegId, Int16 CompanyId, Int64 ReceiptId, string ReceiptNo, Int16 UserId)
        {
            ARReceiptViewModel ARReceiptViewModel = new ARReceiptViewModel();
            try
            {
                ARReceiptViewModel = await _repository.GetQuerySingleOrDefaultAsync<ARReceiptViewModel>(RegId, $"SELECT Invhd.CompanyId,Invhd.ReceiptId,Invhd.ReceiptNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.ModuleFrom,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.CancelById,Invhd.CancelDate,Invhd.CancelRemarks,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy FROM dbo.ArReceiptHd Invhd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Invhd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.ReceiptId={ReceiptId} OR {ReceiptId}=0) AND (Invhd.ReceiptNo='{ReceiptNo}' OR '{ReceiptNo}'='{string.Empty}')");

                if (ARReceiptViewModel == null)
                    return ARReceiptViewModel;

                var result = await _repository.GetQueryAsync<ARReceiptDtViewModel>(RegId, $"SELECT Invdt.CompanyId,Invdt.ReceiptId,Invdt.ReceiptNo,Invdt.ItemNo,Invdt.TransactionId,Invdt.DocumentId,Invdt.DocumentNo,Invdt.DocCurrencyId,Invdt.DocExhRate,Invdt.DocAccountDate,Invdt.DocDueDate,Invdt.DocTotAmt,Invdt.DocTotLocalAmt,Invdt.DocBalAmt,Invdt.DocBalLocalAmt,Invdt.AllocAmt,Invdt.AllocLocalAmt,Invdt.DocAllocAmt,Invdt.DocAllocLocalAmt,Invdt.CentDiff,Invdt.ExhGainLoss FROM dbo.ArReceiptDt Invdt  WHERE Invdt.ReceiptId={ARReceiptViewModel.ReceiptId}");

                ARReceiptViewModel.data_details = result == null ? null : result.ToList();

                return ARReceiptViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Receipt,
                    DocumentId = ReceiptId,
                    DocumentNo = ReceiptNo,
                    TblName = "ArReceiptHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveARReceiptAsync(string RegId, Int16 CompanyId, ArReceiptHd ArReceiptHd, List<ArReceiptDt> ArReceiptDt, Int16 UserId)
        {
            bool IsEdit = false;
            string accountDate = ArReceiptHd.AccountDate.ToString("dd/MMM/yyyy");
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (ArReceiptHd.ReceiptId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.ArReceiptHd WHERE IsCancel=0 And CompanyId={CompanyId} And ReceiptId={ArReceiptHd.ReceiptId}");

                        if (DataExist.Count() == 0)
                            return new SqlResponce { Result = -1, Message = "Receipt Not Exist" };
                    }

                    if (!IsEdit)
                    {
                        var documentIdNo = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec S_GENERATE_NUMBER_NOANDID {CompanyId},{(short)E_Modules.AR},{(short)E_AR.Receipt},'{accountDate}'");

                        if (documentIdNo.ToList()[0].DocumentId > 0 && documentIdNo.ToList()[0].DocumentNo != string.Empty)
                        {
                            ArReceiptHd.ReceiptId = documentIdNo.ToList()[0].DocumentId;
                            ArReceiptHd.ReceiptNo = documentIdNo.ToList()[0].DocumentNo;
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Invoice Number can't generate" };
                    }
                    else
                    {
                        //Insert the previous ARReceipt record to ARReceipt history table as well as editversion also.
                        await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec FIN_AR_CreateHistoryRec {CompanyId},{UserId},{ArReceiptHd.ReceiptId},{(short)E_AR.Receipt}");
                    }

                    //Saving Header
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(ArReceiptHd);
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
                        ArReceiptHd.EditDate = null;
                        ArReceiptHd.EditById = null;

                        var entityHead = _context.Add(ArReceiptHd);

                        entityHead.Property(b => b.IsCancel).IsModified = false;
                        entityHead.Property(b => b.CancelById).IsModified = false;
                        entityHead.Property(b => b.CancelDate).IsModified = false;
                        entityHead.Property(b => b.CancelRemarks).IsModified = false;
                    }

                    var SaveHeader = _context.SaveChanges();

                    //Saving Details
                    if (SaveHeader > 0)
                    {
                        if (IsEdit)
                            _context.ArReceiptDt.Where(x => x.ReceiptId == ArReceiptHd.ReceiptId).ExecuteDelete();

                        foreach (var item in ArReceiptDt)
                        {
                            item.ReceiptId = ArReceiptHd.ReceiptId;
                            item.ReceiptNo = ArReceiptHd.ReceiptNo;
                            _context.Add(item);
                        }

                        var SaveDetails = _context.SaveChanges();

                        #region Save AuditLog

                        if (SaveDetails > 0)
                        {
                            //Inserting the records into AR CreateStatement
                            await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec FIN_AR_CreateStatement {CompanyId},{UserId},{ArReceiptHd.ReceiptId},{(short)E_AR.Receipt}");

                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AR,
                                TransactionId = (short)E_AR.Receipt,
                                DocumentId = ArReceiptHd.ReceiptId,
                                DocumentNo = ArReceiptHd.ReceiptNo,
                                TblName = "ArReceiptHd",
                                ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                                Remarks = ArReceiptHd.Remarks,
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                //Update Edit Version
                                if (IsEdit)
                                    await _repository.UpsertExecuteScalarAsync(RegId, $"update ArReceiptHd set EditVersion=EditVersion+1 where ReceiptId={ArReceiptHd.ReceiptId}; Update ArReceiptDt set EditVersion=(SELECT TOP 1 EditVersion FROM dbo.ArReceiptHd where ReceiptId={ArReceiptHd.ReceiptId}) where ReceiptId={ArReceiptHd.ReceiptId}");

                                //Create / Update Ar Statement
                                await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec FIN_AR_CreateStatement {CompanyId},{UserId},{ArReceiptHd.ReceiptId},{(short)E_AR.Receipt}");

                                TScope.Complete();
                                return new SqlResponce { Result = ArReceiptHd.ReceiptId, Message = "Save Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = 1, Message = "Save Failed" };
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
                    TransactionId = (short)E_AR.Receipt,
                    DocumentId = ArReceiptHd.ReceiptId,
                    DocumentNo = ArReceiptHd.ReceiptNo,
                    TblName = "ArReceiptHd",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw ex;
            }
        }

        public async Task<SqlResponce> DeleteARReceiptAsync(string RegId, Int16 CompanyId, Int64 ReceiptId, string CanacelRemarks, Int16 UserId)
        {
            string ReceiptNo = string.Empty;
            try
            {
                using (TransactionScope TScope = new TransactionScope())
                {
                    //Get Invoice Number
                    ReceiptNo = await _repository.GetQuerySingleOrDefaultAsync<string>(RegId, $"SELECT ReceiptNo FROM dbo.ArReceiptHd WHERE ReceiptId={ReceiptId}");

                    if (ReceiptId > 0)
                    {
                        var ARReceiptToRemove = _context.ArReceiptHd.Where(b => b.ReceiptId == ReceiptId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsCancel, true).SetProperty(b => b.CancelById, UserId).SetProperty(b => b.CancelDate, DateTime.Now).SetProperty(b => b.CancelRemarks, CanacelRemarks));

                        if (ARReceiptToRemove > 0)
                        {
                            //Cancel the Ar Transactions.
                            await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec FIN_AR_DeleteStatement {CompanyId},{UserId},{ReceiptId},{(short)E_AR.Receipt}");

                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AR,
                                TransactionId = (short)E_AR.Receipt,
                                DocumentId = ReceiptId,
                                DocumentNo = ReceiptNo,
                                TblName = "ArReceiptHd",
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
                    TransactionId = (short)E_AR.Receipt,
                    DocumentId = ReceiptId,
                    DocumentNo = ReceiptNo,
                    TblName = "ArReceiptHd",
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