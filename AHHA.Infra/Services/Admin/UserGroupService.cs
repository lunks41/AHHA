﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Admin;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AHHA.Infra.Services.Admin
{
    public sealed class UserGroupService : IUserGroupService
    {
        private readonly IRepository<AdmUserGroup> _repository;
        private ApplicationDbContext _context;

        public UserGroupService(IRepository<AdmUserGroup> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<UserGroupViewModelCount> GetUserGroupListAsync(string RegId, short CompanyId, short pageSize, short pageNumber, string searchString, int UserId)
        {
            UserGroupViewModelCount countViewModel = new UserGroupViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM dbo.AdmUserGroup M_Ban INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Ban.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_Chr ON M_Chr.GLId = M_Ban.GLId WHERE (M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Ban.UserGroupName LIKE '%{searchString}%' OR M_Ban.UserGroupCode LIKE '%{searchString}%' OR M_Ban.AccountNo LIKE '%{searchString}%' OR M_Ban.SwiftCode LIKE '%{searchString}%' OR M_Ban.Remarks1 LIKE '%{searchString}%' OR M_Ban.Remarks2 LIKE '%{searchString}%' OR M_Chr.GLName LIKE '%{searchString}%' OR M_Chr.GLCode LIKE '%{searchString}%') AND M_Ban.UserGroupId<>0 AND M_Ban.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Admins.UserGroup}))");

                var result = await _repository.GetQueryAsync<UserGroupViewModel>(RegId, $"SELECT M_Ban.UserGroupId,M_Ban.UserGroupCode,M_Ban.UserGroupName,M_Cur.CurrencyId,M_Cur.CurrencyName,M_Cur.CurrencyCode,M_Ban.AccountNo,M_Ban.SwiftCode,M_Ban.Remarks1,M_Ban.Remarks2,M_Ban.GLId,M_Chr.GLCode,M_Chr.GLName,M_Ban.IsActive,M_Ban.CreateById,M_Ban.CreateDate,M_Ban.EditById,M_Ban.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy  FROM dbo.AdmUserGroup M_Ban INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Ban.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_Chr ON M_Chr.GLId = M_Ban.GLId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Ban.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Ban.EditById WHERE (M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Ban.UserGroupName LIKE '%{searchString}%' OR M_Ban.UserGroupCode LIKE '%{searchString}%' OR M_Ban.AccountNo LIKE '%{searchString}%' OR M_Ban.SwiftCode LIKE '%{searchString}%' OR M_Ban.Remarks1 LIKE '%{searchString}%' OR M_Ban.Remarks2 LIKE '%{searchString}%' OR M_Chr.GLName LIKE '%{searchString}%' OR M_Chr.GLCode LIKE '%{searchString}%') AND M_Ban.UserGroupId<>0 AND M_Ban.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Admins.UserGroup})) ORDER BY M_Ban.UserGroupName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Admins.UserGroup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUserGroup",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<AdmUserGroup> GetUserGroupByIdAsync(string RegId, short CompanyId, short UserGroupId, int UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<AdmUserGroup>(RegId, $"SELECT UserGroupId,CompanyId,UserGroupCode,UserGroupName,CurrencyId,AccountNo,SwiftCode,Remarks1,Remarks2,GLId,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.AdmUserGroup WHERE UserGroupId={UserGroupId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Admins.UserGroup}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Admins.UserGroup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUserGroup",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddUserGroupAsync(string RegId, short CompanyId, AdmUserGroup UserGroup, int UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.AdmUserGroup WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Admins.UserGroup})) AND UserGroupCode='{UserGroup.UserGroupCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.AdmUserGroup WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Admins.UserGroup})) AND UserGroupName='{UserGroup.UserGroupName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "UserGroup Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "UserGroup Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (UserGroupId + 1) FROM dbo.AdmUserGroup WHERE (UserGroupId + 1) NOT IN (SELECT UserGroupId FROM dbo.AdmUserGroup)),1) AS MissId");

                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving UserGroup

                        UserGroup.UserGroupId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(UserGroup);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var UserGroupToSave = _context.SaveChanges();

                        #endregion Saving UserGroup

                        #region Save AuditLog

                        if (UserGroupToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Admins.UserGroup,
                                DocumentId = UserGroup.UserGroupId,
                                DocumentNo = UserGroup.UserGroupCode,
                                TblName = "AdmUserGroup",
                                ModeId = (short)Mode.Create,
                                Remarks = "UserGroup Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "UserGroupId Should not be zero" };
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
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Admins.UserGroup,
                        DocumentId = 0,
                        DocumentNo = UserGroup.UserGroupCode,
                        TblName = "AdmUserGroup",
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

        public async Task<SqlResponce> UpdateUserGroupAsync(string RegId, short CompanyId, AdmUserGroup UserGroup, int UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (UserGroup.UserGroupId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.AdmUserGroup WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Admins.UserGroup})) AND UserGroupName='{UserGroup.UserGroupName} AND UserGroupId <>{UserGroup.UserGroupId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "UserGroup Name Exist" };
                            }
                        }

                        #region Update UserGroup

                        var entity = _context.Update(UserGroup);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.UserGroupCode).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update UserGroup

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Admins.UserGroup,
                                DocumentId = UserGroup.UserGroupId,
                                DocumentNo = UserGroup.UserGroupCode,
                                TblName = "AdmUserGroup",
                                ModeId = (short)Mode.Update,
                                Remarks = "UserGroup Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "UserGroupId Should not be zero" };
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
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Admins.UserGroup,
                        DocumentId = UserGroup.UserGroupId,
                        DocumentNo = UserGroup.UserGroupCode,
                        TblName = "AdmUserGroup",
                        ModeId = (short)Mode.Update,
                        Remarks = ex.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> DeleteUserGroupAsync(string RegId, short CompanyId, AdmUserGroup UserGroup, int UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (UserGroup.UserGroupId > 0)
                    {
                        var UserGroupToRemove = _context.AdmUserGroup.Where(x => x.UserGroupId == UserGroup.UserGroupId).ExecuteDelete();

                        if (UserGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Admins.UserGroup,
                                DocumentId = UserGroup.UserGroupId,
                                DocumentNo = UserGroup.UserGroupCode,
                                TblName = "AdmUserGroup",
                                ModeId = (short)Mode.Delete,
                                Remarks = "UserGroup Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "UserGroupId Should be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Admins.UserGroup,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "AdmUserGroup",
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
}