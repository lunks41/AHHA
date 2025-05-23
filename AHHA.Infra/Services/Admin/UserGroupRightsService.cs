﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices;
using AHHA.Application.IServices.Admin;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Helper;
using AHHA.Core.Models.Admin;
using AHHA.Infra.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace AHHA.Infra.Services.Admin
{
    public sealed class UserGroupRightsService : IUserGroupRightsService
    {
        private readonly IRepository<AdmUserGroupRights> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public UserGroupRightsService(IRepository<AdmUserGroupRights> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<IEnumerable<UserGroupRightsViewModel>> GetUserGroupRightsByIdAsync(string RegId, Int16 CompanyId, Int16 UserGroupId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<UserGroupRightsViewModel>(RegId, $"exec Adm_GetGroupAccessRights {CompanyId},{UserGroupId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.UserGroupRights,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUserGroupRights",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveUserGroupRightsAsync(string RegId, Int16 CompanyId, List<AdmUserGroupRights> admUserGroupRights, Int16 UserGroupId, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var trackedEntities = _context.ChangeTracker.Entries<AdmUserGroupRights>().ToList();
                    foreach (var entry in trackedEntities)
                    {
                        entry.State = EntityState.Detached;
                    }

                    await _repository.GetQueryAsync<SqlResponseIds>(RegId, $"DELETE FROM dbo.AdmUserGroupRights WHERE  UserGroupId={UserGroupId}");

                    _context.AdmUserGroupRights.AddRange(admUserGroupRights);
                    var saveResult = await _context.SaveChangesAsync();

                    #region Audit Log

                    if (saveResult > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Admin,
                            TransactionId = (short)E_Admin.UserGroupRights,
                            TblName = "AdmUserGroupRights",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "User Group Rights Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.AdmAuditLog.Add(auditLog);
                        await _context.SaveChangesAsync();

                        TScope.Complete();
                        return new SqlResponse { Result = 1, Message = "Save Successfully" };
                    }

                    #endregion Audit Log

                    return new SqlResponse { Result = 0, Message = "Save Failed" };
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Admin,
                        TransactionId = (short)E_Admin.UserGroupRights,
                        TblName = "AdmUserGroupRights",
                        ModeId = (short)E_Mode.Create,
                        Remarks = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty),
                        CreateById = UserId,
                        CreateDate = DateTime.Now
                    };

                    _context.AdmErrorLog.Add(errorLog);
                    await _context.SaveChangesAsync();

                    throw;
                }
            }
        }

        public async Task<SqlResponse> CloneUserGroupRightsAsync(string RegId, Int16 CompanyId, Int16 FromUserGroupId, Int16 ToUserGroupId, Int16 UserId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var CreateResult = await _repository.GetRowExecuteAsync(RegId, $"DELETE FROM dbo.AdmUserGroupRights WHERE UserGroupId = {ToUserGroupId}");

                    var insertResult = await _repository.GetRowExecuteAsync(RegId,
                         $@"
                        INSERT INTO dbo.AdmUserGroupRights
                        (
                            UserGroupId,
                            ModuleId,
                            TransactionId,
                            IsRead,
                            IsEdit,
                            IsDelete,
                            IsExport,
                            IsPrint,
                            CreateById,
                            CreateDate,
                            IsCreate
                        )
                        SELECT
	                        {ToUserGroupId} AS UserGroupId,
                            ModuleId,
                            TransactionId,
                            IsRead,
                            IsEdit,
                            IsDelete,
                            IsExport,
                            IsPrint,
                            {UserId} AS CreateById,
	                        GETDATE() AS CreateDate,
                            IsCreate
	                        FROM dbo.AdmUserGroupRights WHERE UserGroupId= {FromUserGroupId} "
                     );

                    if (insertResult == 0) // If no rows were inserted
                        return new SqlResponse { Result = 0, Message = "No records cloned for the target user." };

                    #region Save AuditLog

                    if (insertResult > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Admin,
                            TransactionId = (short)E_Admin.UserGroupRights,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "AdmUserGroupRights",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "User Grid Admin Clone Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Clone Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = 1, Message = "Clone Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Admin, E_Admin.UserGroupRights, 0, "", "AdmUserGroupRights", E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Admin, E_Admin.UserGroupRights, 0, "", "AdmUserGroupRights", E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }
    }
}