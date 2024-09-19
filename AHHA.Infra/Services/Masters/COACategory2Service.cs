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
    public sealed class COACategory2Service : ICOACategory2Service
    {
        private readonly IRepository<M_COACategory2> _repository;
        private ApplicationDbContext _context;

        public COACategory2Service(IRepository<M_COACategory2> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<COACategoryViewModelCount> GetCOACategory2ListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            COACategoryViewModelCount countViewModel = new COACategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_COACategory2 M_Catg WHERE (M_Catg.COACategoryName LIKE '%{searchString}%' OR M_Catg.COACategoryCode LIKE '%{searchString}%' OR M_Catg.Remarks LIKE '%{searchString}%') AND M_Catg.COACategoryId<>0 AND M_Catg.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory2}))");

                var result = await _repository.GetQueryAsync<COACategoryViewModel>(RegId, $"SELECT M_Catg.COACategoryId,M_Catg.COACategoryCode,M_Catg.COACategoryName,M_Catg.seqNo,M_Catg.CompanyId,M_Catg.Remarks,M_Catg.IsActive,M_Catg.CreateById,M_Catg.CreateDate,M_Catg.EditById,M_Catg.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_COACategory2 M_Catg LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Catg.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Catg.EditById WHERE (M_Catg.COACategoryName LIKE '%{searchString}%' OR M_Catg.COACategoryCode LIKE '%{searchString}%' OR M_Catg.Remarks LIKE '%{searchString}%') AND M_Catg.COACategoryId<>0 AND M_Catg.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory2})) ORDER BY M_Catg.COACategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_Master.COACategory2,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory2",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_COACategory2> GetCOACategory2ByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_COACategory2>(RegId, $"SELECT COACategoryId,COACategoryCode,COACategoryName,seqNo,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_COACategory2 WHERE COACategoryId={COACategoryId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory2}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory2,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory2",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddCOACategory2Async(string RegId, Int16 CompanyId, M_COACategory2 COACategory2, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_COACategory2 WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory2})) AND COACategoryCode='{COACategory2.COACategoryId}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_COACategory2 WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory2})) AND COACategoryName='{COACategory2.COACategoryName}'");

                    if (DataExist.Count() > 0)
                    {
                        if (DataExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "COACategory2 Code Exist" };
                        }
                        else if (DataExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "COACategory2 Name Exist" };
                        }
                    }

                    //Take the Next Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (COACategoryId + 1) FROM dbo.M_COACategory2 WHERE (COACategoryId + 1) NOT IN (SELECT COACategoryId FROM dbo.M_COACategory2)),1) AS NextId");
                    if (sqlMissingResponce != null && sqlMissingResponce.NextId > 0)
                    {
                        #region Saving COACategory2

                        COACategory2.COACategoryId = Convert.ToInt16(sqlMissingResponce.NextId);

                        var entity = _context.Add(COACategory2);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var COACategory2ToSave = _context.SaveChanges();

                        #endregion Saving COACategory2

                        #region Save AuditLog

                        if (COACategory2ToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.COACategory2,
                                DocumentId = COACategory2.COACategoryId,
                                DocumentNo = COACategory2.COACategoryCode,
                                TblName = "M_COACategory2",
                                ModeId = (short)E_Mode.Create,
                                Remarks = "Category Save Successfully",
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
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "COACategoryId Should not be zero" };
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
                        TransactionId = (short)E_Master.COACategory2,
                        DocumentId = 0,
                        DocumentNo = COACategory2.COACategoryCode,
                        TblName = "M_COACategory2",
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

        public async Task<SqlResponce> UpdateCOACategory2Async(string RegId, Int16 CompanyId, M_COACategory2 COACategory2, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (COACategory2.COACategoryId > 0)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_COACategory2 WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory2})) AND COACategoryName='{COACategory2.COACategoryName} AND COACategoryId <>{COACategory2.COACategoryId}'");

                        if (DataExist.Count() > 0)
                        {
                            if (DataExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "COACategory2 Name Exist" };
                            }
                        }

                        #region Update COACategory2

                        var entity = _context.Update(COACategory2);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.COACategoryCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update COACategory2

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.COACategory2,
                                DocumentId = COACategory2.COACategoryId,
                                DocumentNo = COACategory2.COACategoryCode,
                                TblName = "M_COACategory2",
                                ModeId = (short)E_Mode.Update,
                                Remarks = "COACategory2 Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "COACategoryId Should not be zero" };
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
                        TransactionId = (short)E_Master.COACategory2,
                        DocumentId = COACategory2.COACategoryId,
                        DocumentNo = COACategory2.COACategoryCode,
                        TblName = "M_COACategory2",
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

        public async Task<SqlResponce> DeleteCOACategory2Async(string RegId, Int16 CompanyId, M_COACategory2 COACategory2, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (COACategory2.COACategoryId > 0)
                    {
                        var COACategory2ToRemove = _context.M_COACategory2.Where(x => x.COACategoryId == COACategory2.COACategoryId).ExecuteDelete();

                        if (COACategory2ToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.COACategory2,
                                DocumentId = COACategory2.COACategoryId,
                                DocumentNo = COACategory2.COACategoryCode,
                                TblName = "M_COACategory2",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "COACategory2 Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "COACategoryId Should be zero" };
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
                        TransactionId = (short)E_Master.COACategory2,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_COACategory2",
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