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
    public sealed class ARInvoiceService : IARInvoiceService
    {
        private readonly IRepository<ArInvoiceHd> _repository;
        private ApplicationDbContext _context;
        private readonly IAccountService _accountService;

        public ARInvoiceService(IRepository<ArInvoiceHd> repository, ApplicationDbContext context, IAccountService accountService)
        {
            _repository = repository;
            _context = context;
            _accountService = accountService;
        }

        public async Task<ARInvoiceViewModelCount> GetARInvoiceListAsync(string RegId, Int16 CompanyId, short pageSize, short pageNumber, string searchString, Int16 UserId)
        {
            ARInvoiceViewModelCount countViewModel = new ARInvoiceViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM dbo.ArInvoiceHd Invhd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Invhd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.InvoiceNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%')");

                var result = await _repository.GetQueryAsync<ARInvoiceViewModel>(RegId, $"SELECT Invhd.CompanyId,Invhd.InvoiceId,Invhd.InvoiceNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.DeliveryDate,Invhd.DueDate,Invhd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,Invhd.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyCode,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,Invhd.BankId,M_Ban.BankCode,M_Ban.BankName,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.BalAmt,Invhd.BalLocalAmt,Invhd.PayAmt,Invhd.PayLocalAmt,Invhd.ExGainLoss,Invhd.SalesOrderId,Invhd.SalesOrderNo,Invhd.OperationId,Invhd.OperationNo,Invhd.Remarks,Invhd.Address1,Invhd.Address2,Invhd.Address3,Invhd.Address4,Invhd.PinCode,Invhd.CountryId,M_Cun.CountryCode,M_Cun.CountryName,Invhd.PhoneNo,Invhd.FaxNo,Invhd.ContactName,Invhd.MobileNo,Invhd.EmailAdd,Invhd.ModuleFrom,Invhd.SupplierName,Invhd.SuppInvoiceNo,Invhd.APInvoiceId,Invhd.APInvoiceNo,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.CancelById,Invhd.CancelDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy,Usr2.UserName AS CancelBy FROM dbo.ArInvoiceHd Invhd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Invhd.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Invhd.CurrencyId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = Invhd.CreditTermId INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = Invhd.BankId LEFT JOIN dbo.M_Country M_Cun ON M_Cun.CountryId = Invhd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Invhd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Invhd.EditById LEFT JOIN dbo.AdmUser Usr2 ON Usr2.UserId = Invhd.CancelById WHERE (Invhd.InvoiceNo LIKE '%{searchString}%' OR Invhd.ReferenceNo LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%') ORDER BY Invhd.InvoiceNo,Invhd.AccountDate OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TblName = "ArInvoiceHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<ARInvoiceViewModel> GetARInvoiceByIdAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId)
        {
            ARInvoiceViewModel aRInvoiceViewModel = new ARInvoiceViewModel();
            try
            {
                aRInvoiceViewModel = await _repository.GetQuerySingleOrDefaultAsync<ARInvoiceViewModel>(RegId, $"SELECT Invhd.CompanyId,Invhd.InvoiceId,Invhd.InvoiceNo,Invhd.ReferenceNo,Invhd.TrnDate,Invhd.AccountDate,Invhd.DeliveryDate,Invhd.DueDate,Invhd.CustomerId,Invhd.CurrencyId,Invhd.ExhRate,Invhd.CtyExhRate,Invhd.CreditTermId,Invhd.BankId,Invhd.TotAmt,Invhd.TotLocalAmt,Invhd.TotCtyAmt,Invhd.GstClaimDate,Invhd.GstAmt,Invhd.GstLocalAmt,Invhd.GstCtyAmt,Invhd.TotAmtAftGst,Invhd.TotLocalAmtAftGst,Invhd.TotCtyAmtAftGst,Invhd.BalAmt,Invhd.BalLocalAmt,Invhd.PayAmt,Invhd.PayLocalAmt,Invhd.ExGainLoss,Invhd.SalesOrderId,Invhd.SalesOrderNo,Invhd.OperationId,Invhd.OperationNo,Invhd.Remarks,Invhd.Address1,Invhd.Address2,Invhd.Address3,Invhd.Address4,Invhd.PinCode,Invhd.CountryId,Invhd.PhoneNo,Invhd.FaxNo,Invhd.ContactName,Invhd.MobileNo,Invhd.EmailAdd,Invhd.ModuleFrom,Invhd.SupplierName,Invhd.SuppInvoiceNo,Invhd.APInvoiceId,Invhd.APInvoiceNo,Invhd.CreateById,Invhd.CreateDate,Invhd.EditById,Invhd.EditDate,Invhd.CancelById,Invhd.CancelDate FROM dbo.ArInvoiceHd Invhd WHERE (Invhd.InvoiceId={InvoiceId} OR {InvoiceId}=0) AND (Invhd.InvoiceNo='{InvoiceNo}' OR '{InvoiceNo}'='{string.Empty}')");

                if (aRInvoiceViewModel == null)
                    return aRInvoiceViewModel;

                //var result = await _repository.GetQueryAsync<ARInvoiceDtViewModel>(RegId, $"SELECT InvoiceId,InvoiceNo,ItemNo,SeqNo,DocItemNo,ProductId,GLId,QTY,BillQTY,UomId,UnitPrice,TotAmt,TotLocalAmt,TotCtyAmt,Remarks,GstId,GstPercentage,GstAmt,GstLocalAmt,GstCtyAmt,DeliveryDate,DepartmentId,EmployeeId,PortId,VesselId,BargeId,VoyageId,OperationId,OperationNo,OPRefNo,SalesOrderId,SalesOrderNo,SupplyDate,SupplierName,SuppInvoiceNo,APInvoiceId,APInvoiceNo FROM dbo.ArInvoiceDt WHERE InvoiceId={aRInvoiceViewModel.InvoiceId}");

                var result = await _repository.GetQueryAsync<ARInvoiceDtViewModel>(RegId, $"SELECT Invdt.InvoiceId,Invdt.InvoiceNo,Invdt.ItemNo,Invdt.SeqNo,Invdt.DocItemNo,Invdt.ProductId,M_Pro.ProductCode,M_Pro.ProductName,Invdt.GLId,M_Gs.GstCode,M_Gs.GstName,Invdt.QTY,Invdt.BillQTY,Invdt.UomId,M_um.UomCode,M_um.UomName,Invdt.UnitPrice,Invdt.TotAmt,Invdt.TotLocalAmt,Invdt.TotCtyAmt,Invdt.Remarks,Invdt.GstId,M_Gs.GstCode,M_Gs.GstName,Invdt.GstPercentage,Invdt.GstAmt,Invdt.GstLocalAmt,Invdt.GstCtyAmt,Invdt.DeliveryDate,Invdt.DepartmentId,M_Dep.DepartmentCode,M_Dep.DepartmentName,Invdt.EmployeeId,M_Emp.EmployeeCode,M_Emp.EmployeeName,Invdt.PortId,M_Po.PortCode,M_Po.PortName,Invdt.VesselId,M_Vel.VesselCode,M_Vel.VesselName,Invdt.BargeId,M_Brg.BargeCode,M_Brg.BargeName,Invdt.VoyageId,M_Vo.VoyageNo,M_Vo.ReferenceNo as VoyageReferenceNo,Invdt.OperationId,Invdt.OperationNo,Invdt.OPRefNo,Invdt.SalesOrderId,Invdt.SalesOrderNo,Invdt.SupplyDate,Invdt.SupplierName,Invdt.SuppInvoiceNo,Invdt.APInvoiceId,Invdt.APInvoiceNo,Invdt.EditVersion FROM dbo.ArInvoiceDt Invdt LEFT JOIN dbo.M_Uom M_um ON M_um.UomId = Invdt.UomId LEFT JOIN dbo.M_ChartOfAccount M_chra ON M_chra.GLId = Invdt.GLId LEFT JOIN dbo.M_Product M_Pro ON M_Pro.ProductId = Invdt.ProductId LEFT JOIN dbo.M_Gst M_Gs ON M_Gs.GstId = Invdt.GstId LEFT JOIN dbo.M_Department M_Dep ON M_Dep.DepartmentId = Invdt.DepartmentId LEFT JOIN dbo.M_Employee M_Emp ON M_Emp.EmployeeId = Invdt.EmployeeId LEFT JOIN dbo.M_Port M_Po ON M_Po.PortId = Invdt.PortId LEFT JOIN dbo.M_Vessel M_Vel ON M_Vel.VesselId = Invdt.VesselId LEFT JOIN dbo.M_Barge M_Brg ON M_Brg.BargeId = Invdt.BargeId LEFT JOIN dbo.M_Voyage M_Vo ON M_Vo.VoyageId = Invdt.VoyageId  WHERE Invdt.InvoiceId={aRInvoiceViewModel.InvoiceId}");

                aRInvoiceViewModel.data_details = result == null ? null : result.ToList();

                return aRInvoiceViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Invoice,
                    DocumentId = InvoiceId,
                    DocumentNo = InvoiceNo,
                    TblName = "ArInvoiceHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveARInvoiceAsync(string RegId, Int16 CompanyId, ArInvoiceHd arInvoiceHd, List<ArInvoiceDt> arInvoiceDt, Int16 UserId)
        {
            bool IsEdit = false;
            string accountDate = arInvoiceHd.AccountDate.ToString("dd/MMM/yyyy");
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (arInvoiceHd.InvoiceId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.ArInvoiceHd WHERE IsCancel=0 And CompanyId={CompanyId} And InvoiceId={arInvoiceHd.InvoiceId}");

                        if (DataExist.Count() == 0)
                            return new SqlResponce { Result = -1, Message = "Invoice Not Exist" };
                    }

                    if (!IsEdit)
                    {
                        var documentIdNo = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec S_GENERATE_NUMBER_NOANDID {CompanyId},{(short)E_Modules.AR},{(short)E_AR.Invoice},'{accountDate}'");

                        if (documentIdNo.ToList()[0].DocumentId > 0 && documentIdNo.ToList()[0].DocumentNo != string.Empty)
                        {
                            arInvoiceHd.InvoiceId = documentIdNo.ToList()[0].DocumentId;
                            arInvoiceHd.InvoiceNo = documentIdNo.ToList()[0].DocumentNo;
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Invoice Number can't generate" };
                    }
                    else
                    {
                        //Insert the previous arinvoice record to arinvoice history table as well as editversion also.
                        await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec FIN_AR_CreateHistoryRec {CompanyId},{UserId},{arInvoiceHd.InvoiceId},{(short)E_AR.Invoice}");

                        //arInvoiceHd.EditVersion = Convert.ToByte(arInvoiceHd.EditVersion + 1);
                    }
                    ////Document Id Generation
                    //Int64 InvoiceId = await _accountService.GenrateDocumentId(RegId, (short)E_Modules.AR, (short)E_AR.Invoice);

                    ////Document Number Generation
                    //string InvoiceNo = await _accountService.GenrateDocumentNumber(RegId, CompanyId, (short)E_Modules.AR, (short)E_AR.Invoice, arInvoiceHd.AccountDate);

                    //Saving Header
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(arInvoiceHd);
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
                        arInvoiceHd.EditDate = null;
                        arInvoiceHd.EditById = null;

                        var entityHead = _context.Add(arInvoiceHd);

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
                            _context.ArInvoiceDt.Where(x => x.InvoiceId == arInvoiceHd.InvoiceId).ExecuteDelete();

                        foreach (var item in arInvoiceDt)
                        {
                            item.InvoiceId = arInvoiceHd.InvoiceId;
                            item.InvoiceNo = arInvoiceHd.InvoiceNo;
                            //item.EditVersion = arInvoiceHd.EditVersion;
                            _context.Add(item);
                        }

                        var SaveDetails = _context.SaveChanges();

                        #region Save AuditLog

                        if (SaveDetails > 0)
                        {
                            //Inserting the records into AR CreateStatement
                            await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec FIN_AR_CreateStatement {CompanyId},{UserId},{arInvoiceHd.InvoiceId},{(short)E_AR.Invoice}");

                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AR,
                                TransactionId = (short)E_AR.Invoice,
                                DocumentId = arInvoiceHd.InvoiceId,
                                DocumentNo = arInvoiceHd.InvoiceNo,
                                TblName = "ArInvoiceHd",
                                ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                                Remarks = arInvoiceHd.Remarks,
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                //Update Edit Version
                                if (IsEdit)
                                    await _repository.UpsertExecuteScalarAsync(RegId, $"update ArinvoiceHd set EditVersion=EditVersion+1 where InvoiceId={arInvoiceHd.InvoiceId}; Update ArInvoiceDt set EditVersion=(SELECT TOP 1 EditVersion FROM dbo.ArInvoiceHd where InvoiceId={arInvoiceHd.InvoiceId}) where InvoiceId={arInvoiceHd.InvoiceId}");

                                //Create / Update Ar Statement
                                await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec FIN_AR_CreateStatement {CompanyId},{UserId},{arInvoiceHd.InvoiceId},{(short)E_AR.Invoice}");

                                TScope.Complete();
                                return new SqlResponce { Result = arInvoiceHd.InvoiceId, Message = "Save Successfully" };
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
                    TransactionId = (short)E_AR.Invoice,
                    DocumentId = arInvoiceHd.InvoiceId,
                    DocumentNo = arInvoiceHd.InvoiceNo,
                    TblName = "ArInvoiceHd",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw ex;
            }
        }

        public async Task<SqlResponce> DeleteARInvoiceAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string CanacelRemarks, Int16 UserId)
        {
            string InvoiceNo = string.Empty;
            try
            {
                using (TransactionScope TScope = new TransactionScope())
                {
                    //Get Invoice Number
                    InvoiceNo = await _repository.GetQuerySingleOrDefaultAsync<string>(RegId, $"SELECT InvoiceNo FROM dbo.ArInvoiceHd WHERE InvoiceId={InvoiceId}");

                    if (InvoiceId > 0)
                    {
                        //Update IsCancle=1,Cancelby=userid,Canceldate=now,CancelRemarks=CancelRemarks
                        var ARInvoiceToRemove = _context.ArInvoiceHd.Where(b => b.InvoiceId == InvoiceId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsCancel, true).SetProperty(b => b.CancelById, UserId).SetProperty(b => b.CancelDate, DateTime.Now).SetProperty(b => b.CancelRemarks, CanacelRemarks));

                        if (ARInvoiceToRemove > 0)
                        {
                            //Cancel the Ar Transactions.
                            await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"exec FIN_AR_DeleteStatement {CompanyId},{UserId},{InvoiceId},{(short)E_AR.Invoice}");

                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AR,
                                TransactionId = (short)E_AR.Invoice,
                                DocumentId = InvoiceId,
                                DocumentNo = InvoiceNo,
                                TblName = "ArInvoiceHd",
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
                    DocumentId = InvoiceId,
                    DocumentNo = InvoiceNo,
                    TblName = "ArInvoiceHd",
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