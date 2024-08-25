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
    public sealed class BargeService : IBargeService
    {
        private readonly IRepository<M_Barge> _repository;
        private ApplicationDbContext _context;

        public BargeService(IRepository<M_Barge> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<BargeViewModelCount> GetBargeListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            BargeViewModelCount BargeViewModelCount = new BargeViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_Barge WHERE (M_Cou.BargeName LIKE '%{searchString}%' OR M_Cou.BargeCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.BargeId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Barge}))");

                var result = await _repository.GetQueryAsync<BargeViewModel>(RegId, $"SELECT M_Cou.BargeId,M_Cou.BargeCode,M_Cou.BargeName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Barge M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.BargeName LIKE '%{searchString}%' OR M_Cou.BargeCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.BargeId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Barge})) ORDER BY M_Cou.BargeName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                BargeViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                BargeViewModelCount.data = result == null ? null : result.ToList();

                return BargeViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Barge,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Barge",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Barge> GetBargeByIdAsync(string RegId, Int16 CompanyId, Int16 BargeId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Barge>(RegId, $"SELECT BargeId,BargeCode,BargeName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Barge WHERE BargeId={BargeId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Barge,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Barge",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddBargeAsync(string RegId, Int16 CompanyId, M_Barge Barge, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_Barge WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({Barge.CompanyId},{(short)Master.Barge},{(short)Modules.Master})) AND BargeCode='{Barge.BargeId}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Barge WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({Barge.CompanyId},{(short)Master.Barge},{(short)Modules.Master})) AND BargeName='{Barge.BargeName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "Barge Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "Barge Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (BargeId + 1) FROM dbo.M_Barge WHERE (BargeId + 1) NOT IN (SELECT BargeId FROM dbo.M_Barge)),1) AS MissId");

                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving Barge

                        Barge.BargeId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(Barge);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var BargeToSave = _context.SaveChanges();

                        #endregion Saving Barge

                        #region Save AuditLog

                        if (BargeToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Barge,
                                DocumentId = Barge.BargeId,
                                DocumentNo = Barge.BargeCode,
                                TblName = "M_Barge",
                                ModeId = (short)Mode.Create,
                                Remarks = "Barge Save Successfully",
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                sqlResponce = new SqlResponce { Result = 1, Message = "Save Successfully" };
                            }
                        }

                        #endregion Save AuditLog
                    }
                    else
                    {
                        sqlResponce = new SqlResponce { Result = -1, Message = "BargeId Should not be zero" };
                    }
                    return sqlResponce;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.Barge,
                        DocumentId = 0,
                        DocumentNo = Barge.BargeCode,
                        TblName = "M_Barge",
                        ModeId = (short)Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> UpdateBargeAsync(string RegId, Int16 CompanyId, M_Barge Barge, Int32 UserId)
        {
            int IsActive = Barge.IsActive == true ? 1 : 0;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Barge.BargeId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_Barge WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({Barge.CompanyId},{(short)Master.Barge},{(short)Modules.Master})) AND BargeName='{Barge.BargeName} AND BargeId <>{Barge.BargeId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "Barge Name Exist" };
                            }
                        }

                        #region Update Barge

                        var entity = _context.Update(Barge);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.BargeCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update Barge

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Barge,
                                DocumentId = Barge.BargeId,
                                DocumentNo = Barge.BargeCode,
                                TblName = "M_Barge",
                                ModeId = (short)Mode.Update,
                                Remarks = "Barge Update Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                                transaction.Commit();
                        }
                        sqlResponce = new SqlResponce { Result = 1, Message = "Update Successfully" };
                    }
                    else
                    {
                        sqlResponce = new SqlResponce { Result = -1, Message = "BargeId Should not be zero" };
                    }
                    return sqlResponce;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.Barge,
                        DocumentId = Barge.BargeId,
                        DocumentNo = Barge.BargeCode,
                        TblName = "M_Barge",
                        ModeId = (short)Mode.Update,
                        Remarks = ex.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    //await _errorLogServices.AddErrorLogAsync(errorLog);

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> DeleteBargeAsync(string RegId, Int16 CompanyId, M_Barge Barge, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (Barge.BargeId > 0)
                {
                    var BargeToRemove = _context.M_Barge.Where(x => x.BargeId == Barge.BargeId).ExecuteDelete();

                    if (BargeToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Modules.Master,
                            TransactionId = (short)Master.Barge,
                            DocumentId = Barge.BargeId,
                            DocumentNo = Barge.BargeCode,
                            TblName = "M_Barge",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Barge Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Result = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Result = -1, Message = "BargeId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Barge,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Barge",
                    ModeId = (short)Mode.Delete,
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