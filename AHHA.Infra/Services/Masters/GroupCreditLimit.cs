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
    public sealed class GroupCreditLimitService : IGroupCreditLimitService
    {
        private readonly IRepository<M_GroupCreditLimit> _repository;
        private ApplicationDbContext _context;

        public GroupCreditLimitService(IRepository<M_GroupCreditLimit> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<GroupCreditLimitViewModelCount> GetGroupCreditLimitListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            GroupCreditLimitViewModelCount countViewModel = new GroupCreditLimitViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_GroupCreditLimit M_Grp WHERE (M_Grp.GroupCreditLimitName LIKE '%{searchString}%' OR M_Grp.Remarks LIKE '%{searchString}%') AND M_Grp.GroupCreditLimitId<>0 AND M_Grp.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GroupCreditLimit}))");

                var result = await _repository.GetQueryAsync<GroupCreditLimitViewModel>(RegId, $"SELECT M_Grp.GroupCreditLimitId,M_Grp.GroupCreditLimitCode,M_Grp.GroupCreditLimitName,M_Grp.CompanyId,M_Grp.Remarks,M_Grp.IsActive,M_Grp.CreateById,M_Grp.CreateDate,M_Grp.EditById,M_Grp.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_GroupCreditLimit M_Grp LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Grp.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Grp.EditById WHERE (M_Grp.GroupCreditLimitName LIKE '%{searchString}%' OR M_Grp.Remarks LIKE '%{searchString}%') AND M_Grp.GroupCreditLimitId<>0 AND M_Grp.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GroupCreditLimit})) ORDER BY M_Grp.GroupCreditLimitName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_Master.GroupCreditLimit,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GroupCreditLimit",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_GroupCreditLimit> GetGroupCreditLimitByIdAsync(string RegId, Int16 CompanyId, Int16 GroupCreditLimitId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_GroupCreditLimit>(RegId, $"SELECT GroupCreditLimitId,GroupCreditLimitCode,GroupCreditLimitName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_GroupCreditLimit WHERE GroupCreditLimitId={GroupCreditLimitId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GroupCreditLimit}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.GroupCreditLimit,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GroupCreditLimit",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_GroupCreditLimit GroupCreditLimit, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_GroupCreditLimit WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GroupCreditLimit})) UNION ALL SELECT 2 AS IsExist FROM dbo.M_GroupCreditLimit WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GroupCreditLimit}))'");

                    if (DataExist.Count() > 0)
                    {
                        if (DataExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "GroupCreditLimit Code Exist" };
                        }
                        else if (DataExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "GroupCreditLimit Name Exist" };
                        }
                    }

                    //Take the Next Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (GroupCreditLimitId + 1) FROM dbo.M_GroupCreditLimit WHERE (GroupCreditLimitId + 1) NOT IN (SELECT GroupCreditLimitId FROM dbo.M_GroupCreditLimit)),1) AS NextId");
                    if (sqlMissingResponce != null && sqlMissingResponce.NextId > 0)
                    {
                        #region Saving GroupCreditLimit

                        GroupCreditLimit.GroupCreditLimitId = Convert.ToInt16(sqlMissingResponce.NextId);

                        var entity = _context.Add(GroupCreditLimit);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var GroupCreditLimitToSave = _context.SaveChanges();

                        #endregion Saving GroupCreditLimit

                        #region Save AuditLog

                        if (GroupCreditLimitToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.GroupCreditLimit,
                                DocumentId = GroupCreditLimit.GroupCreditLimitId,
                                DocumentNo = "",
                                TblName = "M_GroupCreditLimit",
                                ModeId = (short)E_Mode.Create,
                                Remarks = "Group Credit Limit Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "GroupCreditLimitId Should not be zero" };
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
                        TransactionId = (short)E_Master.GroupCreditLimit,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_GroupCreditLimit",
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

        public async Task<SqlResponce> UpdateGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_GroupCreditLimit GroupCreditLimit, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (GroupCreditLimit.GroupCreditLimitId > 0)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_GroupCreditLimit WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GroupCreditLimit}))  AND GroupCreditLimitId <>{GroupCreditLimit.GroupCreditLimitId}");

                        if (DataExist.Count() > 0)
                        {
                            if (DataExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "GroupCreditLimit Name Exist" };
                            }
                        }

                        #region Update GroupCreditLimit

                        var entity = _context.Update(GroupCreditLimit);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update GroupCreditLimit

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.GroupCreditLimit,
                                DocumentId = GroupCreditLimit.GroupCreditLimitId,
                                DocumentNo = "",
                                TblName = "M_GroupCreditLimit",
                                ModeId = (short)E_Mode.Update,
                                Remarks = "GroupCreditLimit Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "GroupCreditLimitId Should not be zero" };
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
                        TransactionId = (short)E_Master.GroupCreditLimit,
                        DocumentId = GroupCreditLimit.GroupCreditLimitId,
                        DocumentNo = "",
                        TblName = "M_GroupCreditLimit",
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

        public async Task<SqlResponce> DeleteGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_GroupCreditLimit GroupCreditLimit, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (GroupCreditLimit.GroupCreditLimitId > 0)
                    {
                        var GroupCreditLimitToRemove = _context.M_GroupCreditLimit.Where(x => x.GroupCreditLimitId == GroupCreditLimit.GroupCreditLimitId).ExecuteDelete();

                        if (GroupCreditLimitToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.GroupCreditLimit,
                                DocumentId = GroupCreditLimit.GroupCreditLimitId,
                                DocumentNo = "",
                                TblName = "M_GroupCreditLimit",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "GroupCreditLimit Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "GroupCreditLimitId Should be zero" };
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
                        TransactionId = (short)E_Master.GroupCreditLimit,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_GroupCreditLimit",
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