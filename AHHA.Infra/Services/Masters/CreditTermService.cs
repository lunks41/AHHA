﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AHHA.Infra.Services.Masters
{
    public sealed class CreditTermService : ICreditTermService
    {
        private readonly IRepository<M_CreditTerm> _repository;
        private ApplicationDbContext _context;

        public CreditTermService(IRepository<M_CreditTerm> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        #region Header

        public async Task<CreditTermViewModelCount> GetCreditTermListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            CreditTermViewModelCount countViewModel = new CreditTermViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_CreditTerm M_Crd WHERE (M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Crd.Remarks LIKE '%{searchString}%') AND M_Crd.CreditTermId<>0 AND M_Crd.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CreditTerms}))");

                var result = await _repository.GetQueryAsync<CreditTermViewModel>(RegId, $"SELECT M_Crd.CreditTermId,M_Crd.CreditTermCode,M_Crd.CompanyId,M_Crd.CreditTermName,M_Crd.NoDays,M_Crd.Remarks,M_Crd.IsActive,M_Crd.CreateById,M_Crd.CreateDate,M_Crd.EditById,M_Crd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CreditTerm M_Crd LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Crd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Crd.EditById WHERE (M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Crd.Remarks LIKE '%{searchString}%') AND M_Crd.CreditTermId<>0 AND M_Crd.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CreditTerms})) ORDER BY M_Crd.CreditTermName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_Master.CreditTerms,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CreditTerm",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_CreditTerm> GetCreditTermByIdAsync(string RegId, Int16 CompanyId, Int16 CreditTermId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_CreditTerm>(RegId, $"SELECT CreditTermId,CreditTermCode,CreditTermName,CompanyId,NoDays,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_CreditTerm WHERE CreditTermId={CreditTermId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CreditTerms}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CreditTerms,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CreditTerm",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveCreditTermAsync(string RegId, Int16 CompanyId, M_CreditTerm m_CreditTerm, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (m_CreditTerm.CreditTermId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_CreditTerm WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Master.CreditTerms},{(short)E_Modules.Master})) AND CreditTermId<>{m_CreditTerm.CreditTermId} AND CreditTermCode='{m_CreditTerm.CreditTermCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_CreditTerm WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Master.CreditTerms},{(short)E_Modules.Master})) AND CreditTermId<>{m_CreditTerm.CreditTermId} AND CreditTermName='{m_CreditTerm.CreditTermName}'");

                        if (DataExist.Count() > 0)
                        {
                            if (DataExist.ToList()[0].IsExist == 1)
                            {
                                return new SqlResponce { Result = -1, Message = "CreditTerm Code Exist" };
                            }
                            else if (DataExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "CreditTerm Name Exist" };
                            }
                        }
                    }

                    if (!IsEdit)
                    {
                        //Take the Next Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (CreditTermId + 1) FROM dbo.M_CreditTerm WHERE (CreditTermId + 1) NOT IN (SELECT CreditTermId FROM dbo.M_CreditTerm)),1) AS NextId");
                        if (sqlMissingResponce != null && sqlMissingResponce.NextId > 0)
                            m_CreditTerm.CreditTermId = Convert.ToInt16(sqlMissingResponce.NextId);
                        else
                            return new SqlResponce { Result = -1, Message = "CreditTermId Should not be zero" };
                    }

                    #region Saving CreditTerm

                    if (IsEdit)
                    {
                        var entityHead = _context.Update(m_CreditTerm);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        var entityHead = _context.Add(m_CreditTerm);
                        entityHead.Property(b => b.EditDate).IsModified = false;
                        entityHead.Property(b => b.EditById).IsModified = false;
                    }

                    var CreditTermToSave = _context.SaveChanges();

                    #endregion Saving CreditTerm

                    #region Save AuditLog

                    if (CreditTermToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.CreditTerms,
                            DocumentId = m_CreditTerm.CreditTermId,
                            DocumentNo = m_CreditTerm.CreditTermCode,
                            TblName = "M_CreditTerm",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "CreditTerm Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
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
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.CreditTerms,
                        DocumentId = 0,
                        DocumentNo = m_CreditTerm.CreditTermCode,
                        TblName = "M_CreditTerm",
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

        public async Task<SqlResponce> DeleteCreditTermAsync(string RegId, Int16 CompanyId, M_CreditTerm CreditTerm, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (CreditTerm.CreditTermId > 0)
                    {
                        var CreditTermToRemove = _context.M_CreditTerm.Where(x => x.CreditTermId == CreditTerm.CreditTermId).ExecuteDelete();

                        if (CreditTermToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.CreditTerms,
                                DocumentId = CreditTerm.CreditTermId,
                                DocumentNo = CreditTerm.CreditTermCode,
                                TblName = "M_CreditTerm",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "CreditTerm Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();
                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
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
                        return new SqlResponce { Result = -1, Message = "CreditTermId Should be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.CreditTerms,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CreditTerm",
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

        #endregion Header

        #region Details

        public async Task<CreditTermDtViewModelCount> GetCreditTermDtListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            CreditTermDtViewModelCount countViewModel = new CreditTermDtViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM dbo.M_CreditTermDt M_CrdDt INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_CrdDt.CreditTermId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Crd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Crd.EditById WHERE (M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%') AND M_CrdDt.CreditTermId<>0 AND M_CrdDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CreditTermDt}))");

                var result = await _repository.GetQueryAsync<CreditTermDtViewModel>(RegId, $"SELECT M_CrdDt.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,M_CrdDt.CompanyId,M_CrdDt.FromDay,M_CrdDt.ToDay,M_CrdDt.IsEndOfMonth,M_CrdDt.DueDay,M_CrdDt.NoMonth,M_CrdDt.CreateById,M_CrdDt.CreateDate,M_CrdDt.EditById,M_CrdDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CreditTermDt M_CrdDt INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_CrdDt.CreditTermId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Crd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Crd.EditById WHERE (M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%') AND M_CrdDt.CreditTermId<>0 AND M_CrdDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CreditTermDt})) ORDER BY M_Crd.CreditTermName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_Master.CreditTermDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CreditTermDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CreditTermDtViewModel> GetCreditTermDtByIdAsync(string RegId, Int16 CompanyId, Int16 CreditTermId, Int16 FromDay, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CreditTermDtViewModel>(RegId, $"SELECT M_CrdDt.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,M_CrdDt.CompanyId,M_CrdDt.FromDay,M_CrdDt.ToDay,M_CrdDt.IsEndOfMonth,M_CrdDt.DueDay,M_CrdDt.NoMonth,M_CrdDt.CreateById,M_CrdDt.CreateDate,M_CrdDt.EditById,M_CrdDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CreditTermDt M_CrdDt INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_CrdDt.CreditTermId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Crd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Crd.EditById WHERE CreditTermId={CreditTermId} AND CompanyId={CompanyId} AND FromDay={FromDay}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CreditTermDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CreditTermDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveCreditTermDtAsync(string RegId, Int16 CompanyId, M_CreditTermDt m_CreditTermDt, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (m_CreditTermDt.CreditTermId != 0)
                    {
                        IsEdit = true;
                    }

                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_CreditTermDt WHERE CreditTermId={m_CreditTermDt.CreditTermId} AND CompanyId={CompanyId} AND FromDay={m_CreditTermDt.FromDay}");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(m_CreditTermDt);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            var entityHead = _context.Add(m_CreditTermDt);
                            entityHead.Property(b => b.EditDate).IsModified = false;
                            entityHead.Property(b => b.EditById).IsModified = false;
                        }
                    }

                    var CreditTermDtToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (CreditTermDtToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.CreditTermDt,
                            DocumentId = m_CreditTermDt.CreditTermId,
                            DocumentNo = "",
                            TblName = "M_CreditTermDt",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "CreditTermDt Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
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
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.CreditTermDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CreditTermDt",
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

        public async Task<SqlResponce> DeleteCreditTermDtAsync(string RegId, Int16 CompanyId, CreditTermDtViewModel CreditTermDt, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (CreditTermDt.CreditTermId > 0)
                    {
                        var CreditTermDtToRemove = await _context.M_CreditTermDt.Where(x => x.CreditTermId == CreditTermDt.CreditTermId && x.FromDay == CreditTermDt.FromDay && x.CompanyId == CompanyId).ExecuteDeleteAsync();

                        if (CreditTermDtToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.CreditTermDt,
                                DocumentId = CreditTermDt.CreditTermId,
                                DocumentNo = "",
                                TblName = "M_CreditTermDt",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "CreditTermDt Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();
                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
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
                        return new SqlResponce { Result = -1, Message = "CreditTermId Should be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.CreditTermDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CreditTermDt",
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
    }
}