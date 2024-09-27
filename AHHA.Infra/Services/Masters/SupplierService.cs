﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AHHA.Infra.Services.Masters
{
    public sealed class SupplierService : ISupplierService
    {
        private readonly IRepository<M_Supplier> _repository;
        private ApplicationDbContext _context;

        public SupplierService(IRepository<M_Supplier> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<SupplierViewModelCount> GetSupplierListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            SupplierViewModelCount countViewModel = new SupplierViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_Supplier M_Sup INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_Sup.CreditTermId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Sup.CurrencyId  WHERE (M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Sup.SupplierRegNo LIKE '%{searchString}%' OR M_Sup.SupplierOtherName LIKE '%{searchString}%' OR M_Sup.SupplierShortName LIKE '%{searchString}%' OR M_Sup.SupplierName LIKE '%{searchString}%' OR M_Sup.SupplierCode LIKE '%{searchString}%' OR M_Sup.Remarks LIKE '%{searchString}%') AND M_Sup.SupplierId<>0 AND M_Sup.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Supplier}))");

                var result = await _repository.GetQueryAsync<SupplierViewModel>(RegId, $"SELECT M_Sup.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,M_Sup.SupplierOtherName,M_Sup.SupplierShortName,M_Sup.IsCustomer,M_Sup.IsVendor,M_Sup.IsTrader,M_Sup.IsSupplier,M_Sup.SupplierRegNo,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Crd.CreditTermCode,M_Crd.CreditTermName,M_Sup.ParentSupplierId,M_Sup.AccSetupId,M_Set.AccSetupCode,M_Set.AccSetupName,M_Cus.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,M_Sup.CompanyId,M_Sup.Remarks,M_Sup.IsActive,M_Sup.CreateById,M_Sup.CreateDate,M_Sup.EditById,M_Sup.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Supplier M_Sup INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_Sup.CreditTermId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Sup.CurrencyId LEFT JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_Sup.CustomerId LEFT JOIN dbo.M_AccountSetup M_Set ON M_Set.AccSetupId = M_Sup.AccSetupId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Sup.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Sup.EditById WHERE (M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Sup.SupplierRegNo LIKE '%{searchString}%' OR M_Sup.SupplierOtherName LIKE '%{searchString}%' OR M_Sup.SupplierShortName LIKE '%{searchString}%' OR M_Sup.SupplierName LIKE '%{searchString}%' OR M_Sup.SupplierCode LIKE '%{searchString}%' OR M_Sup.Remarks LIKE '%{searchString}%') AND M_Sup.SupplierId<>0 AND M_Sup.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Supplier})) ORDER BY M_Sup.SupplierName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result == null ? null : result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Supplier",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SupplierViewModel> GetSupplierByIdAsync(string RegId, Int16 CompanyId, Int32 SupplierId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<SupplierViewModel>(RegId, $"SELECT M_Sup.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,M_Sup.SupplierOtherName,M_Sup.SupplierShortName,M_Sup.IsCustomer,M_Sup.IsVendor,M_Sup.IsTrader,M_Sup.IsSupplier,M_Sup.SupplierRegNo,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Crd.CreditTermCode,M_Crd.CreditTermName,M_Sup.ParentSupplierId,M_Sup.AccSetupId,M_Set.AccSetupCode,M_Set.AccSetupName,M_Cus.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,M_Sup.CompanyId,M_Sup.Remarks,M_Sup.IsActive,M_Sup.CreateById,M_Sup.CreateDate,M_Sup.EditById,M_Sup.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Supplier M_Sup INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_Sup.CreditTermId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Sup.CurrencyId LEFT JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_Cus.CustomerId LEFT JOIN dbo.M_AccountSetup M_Set ON M_Set.AccSetupId = M_Sup.AccSetupId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Sup.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Sup.EditById WHERE SupplierId={SupplierId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Supplier",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SupplierViewModel> GetSupplierByCodeAsync(string RegId, Int16 CompanyId, string SupplierCode, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<SupplierViewModel>(RegId, $"SELECT M_Sup.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,M_Sup.SupplierOtherName,M_Sup.SupplierShortName,M_Sup.IsCustomer,M_Sup.IsVendor,M_Sup.IsTrader,M_Sup.IsSupplier,M_Sup.SupplierRegNo,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Crd.CreditTermCode,M_Crd.CreditTermName,M_Sup.ParentSupplierId,M_Sup.AccSetupId,M_Set.AccSetupCode,M_Set.AccSetupName,M_Cus.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,M_Sup.CompanyId,M_Sup.Remarks,M_Sup.IsActive,M_Sup.CreateById,M_Sup.CreateDate,M_Sup.EditById,M_Sup.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Supplier M_Sup INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_Sup.CreditTermId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Sup.CurrencyId LEFT JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_Cus.CustomerId LEFT JOIN dbo.M_AccountSetup M_Set ON M_Set.AccSetupId = M_Sup.AccSetupId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Sup.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Sup.EditById WHERE SupplierCode='{SupplierCode}'");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Supplier",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveSupplierAsync(string RegId, Int16 CompanyId, M_Supplier m_Supplier, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                bool IsEdit = false;
                try
                {
                    if (m_Supplier.SupplierId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_Supplier WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Supplier}))  AND SupplierCode='{m_Supplier.SupplierCode}' AND SupplierId <>{m_Supplier.SupplierId} UNION ALL SELECT 2 AS IsExist FROM dbo.M_Supplier WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Supplier})) AND SupplierName='{m_Supplier.SupplierName}' AND SupplierId <>{m_Supplier.SupplierId}");

                        if (DataExist.Count() > 0 && (DataExist.ToList()[0].IsExist == 1 || DataExist.ToList()[0].IsExist == 2))
                            return new SqlResponce { Result = -1, Message = "Supplier Code or Name Exist" };
                    }
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(m_Supplier);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        //Take the Next Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (SupplierId + 1) FROM dbo.M_Supplier WHERE (SupplierId + 1) NOT IN (SELECT SupplierId FROM dbo.M_Supplier)),1) AS NextId");

                        if (sqlMissingResponce != null && sqlMissingResponce.NextId > 0)
                        {
                            m_Supplier.SupplierId = Convert.ToInt32(sqlMissingResponce.NextId);

                            m_Supplier.EditDate = null;
                            m_Supplier.EditById = null;
                            _context.Add(m_Supplier);
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "SupplierId Should not be zero" };
                    }

                    var SupplierToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (SupplierToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Supplier,
                            DocumentId = m_Supplier.SupplierId,
                            DocumentNo = m_Supplier.SupplierCode,
                            TblName = "M_Supplier",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Supplier Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            transaction.Commit();
                            return new SqlResponce { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = 1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.Supplier,
                        DocumentId = 0,
                        DocumentNo = m_Supplier.SupplierCode,
                        TblName = "M_Supplier",
                        ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> DeleteSupplierAsync(string RegId, Int16 CompanyId, SupplierViewModel supplierViewModel, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (supplierViewModel.SupplierId > 0)
                    {
                        var SupplierToRemove = _context.M_Supplier.Where(x => x.SupplierId == supplierViewModel.SupplierId).ExecuteDelete();

                        if (SupplierToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Supplier,
                                DocumentId = supplierViewModel.SupplierId,
                                DocumentNo = supplierViewModel.SupplierCode,
                                TblName = "M_Supplier",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Supplier Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();
                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                return new SqlResponce { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "SupplierId Should be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.Supplier,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Supplier",
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
}