﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AR;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Helper;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AHHA.Infra.Services.Masters
{
    public sealed class CurrencyService : ICurrencyService
    {
        private readonly IRepository<M_Currency> _repository;
        private ApplicationDbContext _context;

        public CurrencyService(IRepository<M_Currency> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        #region Headers

        public async Task<CurrencyViewModelCount> GetCurrencyListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            CurrencyViewModelCount countViewModel = new CurrencyViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_Currency M_Cur WHERE (M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.Remarks LIKE '%{searchString}%') AND M_Cur.CurrencyId<>0 AND M_Cur.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Currency}))");

                var result = await _repository.GetQueryAsync<CurrencyViewModel>(RegId, $"SELECT M_Cur.CurrencyId,M_Cur.CompanyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Cur.IsMultiply,M_Cur.Remarks,M_Cur.IsActive,M_Cur.CreateById,M_Cur.CreateDate,M_Cur.EditById,M_Cur.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_Currency M_Cur LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cur.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cur.EditById WHERE (M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.Remarks LIKE '%{searchString}%') AND M_Cur.CurrencyId<>0 AND M_Cur.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Currency})) ORDER BY M_Cur.CurrencyName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Currency,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Currency",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CurrencyViewModel> GetCurrencyByIdAsync(string RegId, Int16 CompanyId, Int32 CurrencyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CurrencyViewModel>(RegId, $"SELECT M_Cur.CurrencyId,M_Cur.CompanyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Cur.IsMultiply,M_Cur.Remarks,M_Cur.IsActive,M_Cur.CreateById,M_Cur.CreateDate,M_Cur.EditById,M_Cur.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_Currency M_Cur LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cur.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cur.EditById WHERE CurrencyId={CurrencyId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Currency}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Currency,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Currency",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveCurrencyAsync(string RegId, Int16 CompanyId, M_Currency m_Currency, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                bool IsEdit = false;
                try
                {
                    if (m_Currency.CurrencyId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_Currency WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Currency})) AND CurrencyId<>{m_Currency.CurrencyId} AND CurrencyCode='{m_Currency.CurrencyCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Currency WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Currency})) AND CurrencyId<>{m_Currency.CurrencyId} AND CurrencyName='{m_Currency.CurrencyName}'");

                        if (DataExist.Count() > 0)
                        {
                            if (DataExist.ToList()[0].IsExist == 1)
                            {
                                return new SqlResponce { Result = -1, Message = "Currency Code Exist" };
                            }
                            else if (DataExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "Currency Name Exist" };
                            }
                        }
                    }

                    if (!IsEdit)
                    {
                        //Take the Next Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (CurrencyId + 1) FROM dbo.M_Currency WHERE (CurrencyId + 1) NOT IN (SELECT CurrencyId FROM dbo.M_Currency)),1) AS NextId");
                        if (sqlMissingResponce != null && sqlMissingResponce.NextId > 0)
                            m_Currency.CurrencyId = Convert.ToInt16(sqlMissingResponce.NextId);
                        else
                            return new SqlResponce { Result = -1, Message = "CurrencyId Should not be zero" };
                    }

                    #region Saving Currency

                    if (IsEdit)
                    {
                        var entityHead = _context.Update(m_Currency);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        var entityHead = _context.Add(m_Currency);
                        entityHead.Property(b => b.EditDate).IsModified = false;
                        entityHead.Property(b => b.EditById).IsModified = false;
                    }

                    var CurrencyToSave = _context.SaveChanges();

                    #endregion Saving Currency

                    #region Save AuditLog

                    if (CurrencyToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Currency,
                            DocumentId = m_Currency.CurrencyId,
                            DocumentNo = m_Currency.CurrencyCode,
                            TblName = "M_Currency",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Currency Save Successfully",
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
                        TransactionId = (short)E_Master.Currency,
                        DocumentId = 0,
                        DocumentNo = m_Currency.CurrencyCode,
                        TblName = "M_Currency",
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

        public async Task<SqlResponce> DeleteCurrencyAsync(string RegId, Int16 CompanyId, M_Currency Currency, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Currency.CurrencyId > 0)
                    {
                        var CurrencyToRemove = _context.M_Currency.Where(x => x.CurrencyId == Currency.CurrencyId).ExecuteDelete();

                        if (CurrencyToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Currency,
                                DocumentId = Currency.CurrencyId,
                                DocumentNo = Currency.CurrencyCode,
                                TblName = "M_Currency",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Currency Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "CurrencyId Should be zero" };
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
                        TransactionId = (short)E_Master.Currency,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Currency",
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

        #endregion Headers

        #region Details

        public async Task<CurrencyDtViewModelCount> GetCurrencyDtListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            CurrencyDtViewModelCount countViewModel = new CurrencyDtViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM dbo.M_CurrencyDt M_CurDt INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_CurDt.CurrencyId WHERE (M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%') AND M_CurDt.CurrencyId<>0 AND M_CurDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CurrencyDt}))");

                var result = await _repository.GetQueryAsync<CurrencyDtViewModel>(RegId, $"SELECT M_CurDt.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_CurDt.CompanyId,M_CurDt.ExhRate,M_CurDt.ValidFrom,M_CurDt.CreateById,M_CurDt.CreateDate,M_CurDt.EditById,M_CurDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CurrencyDt M_CurDt INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_CurDt.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CurDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_CurDt.EditById WHERE (M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%') AND M_CurDt.CurrencyId<>0 AND M_CurDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CurrencyDt})) ORDER BY M_Cur.CurrencyName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CurrencyDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CurrencyDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CurrencyDtViewModel> GetCurrencyDtByIdAsync(string RegId, Int16 CompanyId, Int32 CurrencyId, DateTime ValidFrom, Int16 UserId)
        {
            string validFrom = ValidFrom.ToString("yyyy-MM-dd");
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CurrencyDtViewModel>(RegId, $"SELECT M_CurDt.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_CurDt.CompanyId,M_CurDt.ExhRate,M_CurDt.ValidFrom,M_CurDt.CreateById,M_CurDt.CreateDate,M_CurDt.EditById,M_CurDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CurrencyDt M_CurDt INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_CurDt.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CurDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_CurDt.EditById WHERE M_CurDt.CurrencyId={CurrencyId} AND M_CurDt.ValidFrom = '{validFrom}' AND M_CurDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CurrencyDt}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CurrencyDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CurrencyDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveCurrencyDtAsync(string RegId, Int16 CompanyId, M_CurrencyDt m_CurrencyDt, Int16 UserId)
        {
            //string validFrom = m_CurrencyDt.ValidFrom.ToString("yyyy-MM-dd");
            string validFrom = m_CurrencyDt.ValidFrom.ToString("dd/MMM/yyyy");
            using (var transaction = _context.Database.BeginTransaction())
            {
                bool IsEdit = false;
                try
                {
                    if (m_CurrencyDt.CurrencyId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_CurrencyDt WHERE CompanyId={CompanyId} AND CurrencyId={m_CurrencyDt.CurrencyId} AND ValidFrom ='{validFrom}'");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(m_CurrencyDt);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            var entityHead = _context.Add(m_CurrencyDt);
                            entityHead.Property(b => b.EditDate).IsModified = false;
                            entityHead.Property(b => b.EditById).IsModified = false;
                        }
                    }

                    var CurrencyDtToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (CurrencyDtToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.CurrencyDt,
                            DocumentId = m_CurrencyDt.CurrencyId,
                            DocumentNo = "",
                            TblName = "M_CurrencyDt",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "CurrencyDt Save Successfully",
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
                        TransactionId = (short)E_Master.CurrencyDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CurrencyDt",
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

        public async Task<SqlResponce> DeleteCurrencyDtAsync(string RegId, Int16 CompanyId, CurrencyDtViewModel currencyDtViewModel, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (currencyDtViewModel.CurrencyId > 0)
                    {
                        var CurrencyDtToRemove = await _context.M_CurrencyDt.Where(x => x.CurrencyId == currencyDtViewModel.CurrencyId && x.ValidFrom == DateHelperStatic.ParseClientDate(currencyDtViewModel.ValidFrom) && x.CompanyId == CompanyId).ExecuteDeleteAsync();

                        if (CurrencyDtToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.CurrencyDt,
                                DocumentId = currencyDtViewModel.CurrencyId,
                                DocumentNo = "",
                                TblName = "M_CurrencyDt",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "CurrencyDt Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "CurrencyId Should be zero" };
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
                        TransactionId = (short)E_Master.CurrencyDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CurrencyDt",
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

        #endregion Details

        #region Local Details

        public async Task<CurrencyLocalDtViewModelCount> GetCurrencyLocalDtListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            CurrencyLocalDtViewModelCount countViewModel = new CurrencyLocalDtViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM dbo.M_CurrencyLocalDt M_CurDt INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_CurDt.CurrencyId WHERE (M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%') AND M_CurDt.CurrencyId<>0 AND M_CurDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CurrencyLocalDt}))");

                var result = await _repository.GetQueryAsync<CurrencyLocalDtViewModel>(RegId, $"SELECT M_CurDt.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_CurDt.CompanyId,M_CurDt.ExhRate,M_CurDt.ValidFrom,M_CurDt.CreateById,M_CurDt.CreateDate,M_CurDt.EditById,M_CurDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CurrencyLocalDt M_CurDt INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_CurDt.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CurDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_CurDt.EditById WHERE (M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%') AND M_CurDt.CurrencyId<>0 AND M_CurDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CurrencyLocalDt})) ORDER BY M_Cur.CurrencyName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CurrencyLocalDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CurrencyLocalDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CurrencyLocalDtViewModel> GetCurrencyLocalDtByIdAsync(string RegId, Int16 CompanyId, Int32 CurrencyId, DateTime ValidFrom, Int16 UserId)
        {
            string validFrom = ValidFrom.ToString("yyyy-MM-dd");
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CurrencyLocalDtViewModel>(RegId, $"SELECT M_CurDt.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_CurDt.CompanyId,M_CurDt.ExhRate,M_CurDt.ValidFrom,M_CurDt.CreateById,M_CurDt.CreateDate,M_CurDt.EditById,M_CurDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CurrencyLocalDt M_CurDt INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_CurDt.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CurDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_CurDt.EditById WHERE M_CurDt.CurrencyId={CurrencyId} AND M_CurDt.ValidFrom = '{validFrom}' AND M_CurDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CurrencyLocalDt}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CurrencyLocalDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CurrencyLocalDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveCurrencyLocalDtAsync(string RegId, Int16 CompanyId, M_CurrencyLocalDt m_CurrencyLocalDt, Int16 UserId)
        {
            string validFrom = m_CurrencyLocalDt.ValidFrom.ToString("yyyy-MM-dd");
            using (var transaction = _context.Database.BeginTransaction())
            {
                bool IsEdit = false;
                try
                {
                    if (m_CurrencyLocalDt.CurrencyId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_CurrencyLocalDt WHERE CompanyId={CompanyId} AND CurrencyId={m_CurrencyLocalDt.CurrencyId} AND ValidFrom ='{validFrom}'");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(m_CurrencyLocalDt);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            var entityHead = _context.Add(m_CurrencyLocalDt);
                            entityHead.Property(b => b.EditDate).IsModified = false;
                            entityHead.Property(b => b.EditById).IsModified = false;
                        }
                    }

                    var CurrencyLocalDtToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (CurrencyLocalDtToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.CurrencyLocalDt,
                            DocumentId = m_CurrencyLocalDt.CurrencyId,
                            DocumentNo = "",
                            TblName = "M_CurrencyLocalDt",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "CurrencyLocalDt Save Successfully",
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
                        TransactionId = (short)E_Master.CurrencyLocalDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CurrencyLocalDt",
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

        public async Task<SqlResponce> DeleteCurrencyLocalDtAsync(string RegId, Int16 CompanyId, CurrencyLocalDtViewModel currencyLocalDtViewModel, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (currencyLocalDtViewModel.CurrencyId > 0)
                    {
                        var CurrencyLocalDtToRemove = _context.M_CurrencyLocalDt.Where(x => x.CurrencyId == currencyLocalDtViewModel.CurrencyId).ExecuteDelete();

                        if (CurrencyLocalDtToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.CurrencyLocalDt,
                                DocumentId = currencyLocalDtViewModel.CurrencyId,
                                DocumentNo = "",
                                TblName = "M_CurrencyLocalDt",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "CurrencyLocalDt Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "CurrencyId Should be zero" };
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
                        TransactionId = (short)E_Master.CurrencyLocalDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CurrencyLocalDt",
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

        #endregion Local Details
    }
}