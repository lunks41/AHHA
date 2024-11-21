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
    public sealed class AccountSetupCategoryService : IAccountSetupCategoryService
    {
        private readonly IRepository<M_AccountSetupCategory> _repository;
        private ApplicationDbContext _context;

        public AccountSetupCategoryService(IRepository<M_AccountSetupCategory> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<AccountSetupCategoryViewModelCount> GetAccountSetupCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            AccountSetupCategoryViewModelCount countViewModel = new AccountSetupCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_AccountSetupCategory M_AccSetCa  WHERE M_AccSetCa.AccSetupCategoryId<>0 AND  ( M_AccSetCa.AccSetupCategoryName LIKE '%{searchString}%' OR M_AccSetCa.AccSetupCategoryCode LIKE '%{searchString}%' OR M_AccSetCa.Remarks LIKE '%{searchString}%')");

                var result = await _repository.GetQueryAsync<AccountSetupCategoryViewModel>(RegId, $"SELECT M_AccSetCa.AccSetupCategoryId,M_AccSetCa.AccSetupCategoryCode,M_AccSetCa.AccSetupCategoryName,M_AccSetCa.Remarks,M_AccSetCa.IsActive,M_AccSetCa.CreateById,M_AccSetCa.CreateDate,M_AccSetCa.EditById,M_AccSetCa.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_AccountSetupCategory M_AccSetCa LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_AccSetCa.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_AccSetCa.EditById WHERE M_AccSetCa.AccSetupCategoryId<>0 AND  ( M_AccSetCa.AccSetupCategoryName LIKE '%{searchString}%' OR M_AccSetCa.AccSetupCategoryCode LIKE '%{searchString}%' OR M_AccSetCa.Remarks LIKE '%{searchString}%') AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetupCategory})) ORDER BY M_AccSetCa.AccSetupCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_Master.AccountSetupCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetupCategory",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_AccountSetupCategory> GetAccountSetupCategoryByIdAsync(string RegId, Int16 CompanyId, Int16 AccSetupCategoryId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_AccountSetupCategory>(RegId, $"SELECT AccSetupCategoryId,AccSetupCategoryCode,AccSetupCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_AccountSetupCategory WHERE AccSetupCategoryId={AccSetupCategoryId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetupCategory}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountSetupCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetupCategory",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory m_AccountSetupCategory, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                bool IsEdit = false;
                try
                {
                    if (m_AccountSetupCategory.AccSetupCategoryId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_AccountSetupCategory WHERE AccSetupCategoryId<>0 AND AccSetupCategoryId={m_AccountSetupCategory.AccSetupCategoryId} ");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(m_AccountSetupCategory);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "User Not Found" };
                    }
                    else
                    {
                        var codeExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_AccountSetupCategory WHERE AccSetupCategoryId<>0 AND AccSetupCategoryCode={m_AccountSetupCategory.AccSetupCategoryCode} AND AccSetupCategoryName={m_AccountSetupCategory.AccSetupCategoryName} ");

                        if (codeExist.Count() > 0 && codeExist.ToList()[0].IsExist == 1)
                            return new SqlResponce { Result = -1, Message = "AccSetupCategory Code Same" };

                        //Take the Next Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (AccSetupCategoryId + 1) FROM dbo.M_AccountSetupCategory WHERE (AccSetupCategoryId + 1) NOT IN (SELECT AccSetupCategoryId FROM dbo.m_AccountSetupCategory)),1) AS NextId");

                        if (sqlMissingResponce != null && sqlMissingResponce.NextId > 0)
                        {
                            m_AccountSetupCategory.AccSetupCategoryId = Convert.ToInt16(sqlMissingResponce.NextId);
                            _context.Add(m_AccountSetupCategory);
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    #region Save AuditLog

                    if (saveChangeRecord > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.AccountSetupCategory,
                            DocumentId = m_AccountSetupCategory.AccSetupCategoryId,
                            DocumentNo = m_AccountSetupCategory.AccSetupCategoryCode,
                            TblName = "m_AccountSetupCategory",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "AccSetupCategory Save Successfully",
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
                        TransactionId = (short)E_Master.AccountSetupCategory,
                        DocumentId = m_AccountSetupCategory.AccSetupCategoryId,
                        DocumentNo = m_AccountSetupCategory.AccSetupCategoryCode,
                        TblName = "AdmUser",
                        ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw ex;
                }
            }
        }

        public async Task<SqlResponce> AddAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory accountSetupCategory, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_AccountSetupCategory WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetupCategory})) AND AccSetupCategoryCode='{accountSetupCategory.AccSetupCategoryId}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_AccountSetupCategory WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetupCategory})) AND AccSetupCategoryName='{accountSetupCategory.AccSetupCategoryName}'");

                    if (DataExist.Count() > 0)
                    {
                        if (DataExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "AccountSetupCategory Code Exist" };
                        }
                        else if (DataExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "AccountSetupCategory Name Exist" };
                        }
                    }

                    //Take the Next Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (AccSetupCategoryId + 1) FROM dbo.M_AccountSetupCategory WHERE (AccSetupCategoryId + 1) NOT IN (SELECT AccSetupCategoryId FROM dbo.M_AccountSetupCategory)),1) AS NextId");

                    if (sqlMissingResponce != null && sqlMissingResponce.NextId > 0)
                    {
                        #region Saving AccountSetupCategory

                        accountSetupCategory.AccSetupCategoryId = Convert.ToInt16(sqlMissingResponce.NextId);

                        var entity = _context.Add(accountSetupCategory);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var AccountSetupCategoryToSave = _context.SaveChanges();

                        #endregion Saving AccountSetupCategory

                        #region Save AuditLog

                        if (AccountSetupCategoryToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.AccountSetupCategory,
                                DocumentId = accountSetupCategory.AccSetupCategoryId,
                                DocumentNo = accountSetupCategory.AccSetupCategoryCode,
                                TblName = "M_AccountSetupCategory",
                                ModeId = (short)E_Mode.Create,
                                Remarks = "Account Setup Category Save Successfully",
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            int auditLogSave = _context.SaveChanges();

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
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "AccSetupCategoryId Should not be zero" };
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
                        TransactionId = (short)E_Master.AccountSetupCategory,
                        DocumentId = 0,
                        DocumentNo = accountSetupCategory.AccSetupCategoryCode,
                        TblName = "M_AccountSetupCategory",
                        ModeId = (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> UpdateAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory accountSetupCategory, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (accountSetupCategory.AccSetupCategoryId > 0)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, RegId, $"SELECT 2 AS IsExist FROM dbo.M_AccountSetupCategory WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetupCategory})) AND AccSetupCategoryName='{accountSetupCategory.AccSetupCategoryName}' AND AccSetupCategoryId <>{accountSetupCategory.AccSetupCategoryId}");

                        if (DataExist.Any() && DataExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "AccountSetupCategory Name Exist" };
                        }

                        #region Update AccountSetupCategory

                        var entity = _context.Update(accountSetupCategory);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.AccSetupCategoryCode).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update AccountSetupCategory

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.AccountSetupCategory,
                                DocumentId = accountSetupCategory.AccSetupCategoryId,
                                DocumentNo = accountSetupCategory.AccSetupCategoryCode,
                                TblName = "M_AccountSetupCategory",
                                ModeId = (short)E_Mode.Update,
                                Remarks = "Account Setup Category Update Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                return new SqlResponce { Result = 1, Message = "Update Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Update Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "AccSetupCategoryId Should not be zero" };
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
                        TransactionId = (short)E_Master.AccountSetupCategory,
                        DocumentId = accountSetupCategory.AccSetupCategoryId,
                        DocumentNo = accountSetupCategory.AccSetupCategoryCode,
                        TblName = "M_AccountSetupCategory",
                        ModeId = (short)E_Mode.Update,
                        Remarks = ex.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> DeleteAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory accountSetupCategory, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (accountSetupCategory.AccSetupCategoryId > 0)
                    {
                        var accountSetupCategoryToRemove = _context.M_AccountSetupCategory.Where(x => x.AccSetupCategoryId == accountSetupCategory.AccSetupCategoryId).ExecuteDelete();

                        if (accountSetupCategoryToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.AccountSetupCategory,
                                DocumentId = accountSetupCategory.AccSetupCategoryId,
                                DocumentNo = accountSetupCategory.AccSetupCategoryCode,
                                TblName = "M_AccountSetupCategory",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "AccountSetupCategory Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "AccSetupCategoryId Should be zero" };
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
                        TransactionId = (short)E_Master.AccountSetupCategory,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_AccountSetupCategory",
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