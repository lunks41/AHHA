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
    public sealed class COACategory1Service : ICOACategory1Service
    {
        private readonly IRepository<M_COACategory1> _repository;
        private readonly ApplicationDbContext _context;

        public COACategory1Service(IRepository<M_COACategory1> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<COACategoryViewModelCount> GetCOACategory1ListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            COACategoryViewModelCount countViewModel = new COACategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_COACategory1 M_Catg WHERE (M_Catg.COACategoryName LIKE '%{searchString}%' OR M_Catg.COACategoryCode LIKE '%{searchString}%' OR M_Catg.Remarks LIKE '%{searchString}%') AND M_Catg.COACategoryId<>0 AND M_Catg.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory1}))");

                var result = await _repository.GetQueryAsync<COACategoryViewModel>(RegId, $"SELECT M_Catg.COACategoryId,M_Catg.COACategoryCode,M_Catg.COACategoryName,M_Catg.seqNo,M_Catg.CompanyId,M_Catg.Remarks,M_Catg.IsActive,M_Catg.CreateById,M_Catg.CreateDate,M_Catg.EditById,M_Catg.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_COACategory1 M_Catg LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Catg.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Catg.EditById WHERE (M_Catg.COACategoryName LIKE '%{searchString}%' OR M_Catg.COACategoryCode LIKE '%{searchString}%' OR M_Catg.Remarks LIKE '%{searchString}%') AND M_Catg.COACategoryId<>0 AND M_Catg.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory1})) ORDER BY M_Catg.COACategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_Master.COACategory1,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory1",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_COACategory1> GetCOACategory1ByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_COACategory1>(RegId, $"SELECT COACategoryId,COACategoryCode,COACategoryName,seqNo,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_COACategory1 WHERE COACategoryId={COACategoryId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory1}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory1,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory1",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCOACategory1Async(string RegId, Int16 CompanyId, M_COACategory1 m_COACategory, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (m_COACategory.COACategoryId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_COACategory WHERE COACategoryId<>0 AND COACategoryId={m_COACategory.COACategoryId} ");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(m_COACategory);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "User Not Found" };
                    }
                    else
                    {
                        var codeExist = await _repository.GetQueryAsync<SqlResponseIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_COACategory WHERE COACategoryId<>0 AND COACategoryCode={m_COACategory.COACategoryCode} AND COACategoryName={m_COACategory.COACategoryName} ");

                        if (codeExist.Count() > 0 && codeExist.ToList()[0].IsExist == 1)
                            return new SqlResponse { Result = -1, Message = "COACategory Code Same" };

                        //Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (COACategoryId + 1) FROM dbo.M_COACategory WHERE (COACategoryId + 1) NOT IN (SELECT COACategoryId FROM dbo.M_COACategory)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_COACategory.COACategoryId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_COACategory);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Internal Server Error" };
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
                            TransactionId = (short)E_Master.COACategory1,
                            DocumentId = m_COACategory.COACategoryId,
                            DocumentNo = m_COACategory.COACategoryCode,
                            TblName = "M_COACategory",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "COACategory Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = 1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.COACategory1,
                        DocumentId = m_COACategory.COACategoryId,
                        DocumentNo = m_COACategory.COACategoryCode,
                        TblName = "AdmUser",
                        ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw;
                }
            }
        }

        public async Task<SqlResponse> AddCOACategory1Async(string RegId, Int16 CompanyId, M_COACategory1 m_COACategory1, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var DataExist = await _repository.GetQueryAsync<SqlResponseIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_COACategory1 WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory1})) AND COACategoryCode='{m_COACategory1.COACategoryId}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_COACategory1 WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory1})) AND COACategoryName='{m_COACategory1.COACategoryName}'");

                    if (DataExist.Count() > 0)
                    {
                        if (DataExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponse { Result = -1, Message = "COACategory Code Exist" };
                        }
                        else if (DataExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponse { Result = -2, Message = "COACategory Name Exist" };
                        }
                    }

                    //Take the Next Id From SQL
                    var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (COACategoryId + 1) FROM dbo.M_COACategory1 WHERE (COACategoryId + 1) NOT IN (SELECT COACategoryId FROM dbo.M_COACategory1)),1) AS NextId");
                    if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                    {
                        #region Saving COACategory1

                        m_COACategory1.COACategoryId = Convert.ToInt16(sqlMissingResponse.NextId);

                        var entity = _context.Add(m_COACategory1);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var COACategory1ToSave = _context.SaveChanges();

                        #endregion Saving COACategory1

                        #region Save AuditLog

                        if (COACategory1ToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.COACategory1,
                                DocumentId = m_COACategory1.COACategoryId,
                                DocumentNo = m_COACategory1.COACategoryCode,
                                TblName = "M_COACategory1",
                                ModeId = (short)E_Mode.Create,
                                Remarks = "Category Save Successfully",
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponse { Result = 1, Message = "Save Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponse { Result = 1, Message = "Save Failed" };
                        }

                        #endregion Save AuditLog
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "COACategoryId Should not be zero" };
                    }
                    return new SqlResponse();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.COACategory1,
                        DocumentId = 0,
                        DocumentNo = m_COACategory1.COACategoryCode,
                        TblName = "M_COACategory1",
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

        public async Task<SqlResponse> UpdateCOACategory1Async(string RegId, Int16 CompanyId, M_COACategory1 m_COACategory1, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (m_COACategory1.COACategoryId > 0)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_COACategory1 WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory1})) AND COACategoryName='{m_COACategory1.COACategoryName}' AND COACategoryId <>{m_COACategory1.COACategoryId}");

                        if (DataExist.Count() > 0)
                        {
                            if (DataExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponse { Result = -2, Message = "COACategory1 Name Exist" };
                            }
                        }

                        #region Update COACategory1

                        var entity = _context.Update(m_COACategory1);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.COACategoryCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update COACategory1

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.COACategory1,
                                DocumentId = m_COACategory1.COACategoryId,
                                DocumentNo = m_COACategory1.COACategoryCode,
                                TblName = "M_COACategory1",
                                ModeId = (short)E_Mode.Update,
                                Remarks = "COACategory1 Update Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponse { Result = 1, Message = "Update Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Update Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "COACategoryId Should not be zero" };
                    }
                    return new SqlResponse();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.COACategory1,
                        DocumentId = m_COACategory1.COACategoryId,
                        DocumentNo = m_COACategory1.COACategoryCode,
                        TblName = "M_COACategory1",
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

        public async Task<SqlResponse> DeleteCOACategory1Async(string RegId, Int16 CompanyId, M_COACategory1 COACategory1, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (COACategory1.COACategoryId > 0)
                    {
                        var COACategory1ToRemove = _context.M_COACategory1.Where(x => x.COACategoryId == COACategory1.COACategoryId).ExecuteDelete();

                        if (COACategory1ToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.COACategory1,
                                DocumentId = COACategory1.COACategoryId,
                                DocumentNo = COACategory1.COACategoryCode,
                                TblName = "M_COACategory1",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "COACategory1 Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();
                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponse { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "COACategoryId Should be zero" };
                    }
                    return new SqlResponse();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.COACategory1,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_COACategory1",
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