﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Admin;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;
using AHHA.Infra.Data;

namespace AHHA.Infra.Services.Admin
{
    public sealed class UserGroupRightsService : IUserGroupRightsService
    {
        private readonly IRepository<AdmUserGroupRights> _repository;
        private ApplicationDbContext _context;

        public UserGroupRightsService(IRepository<AdmUserGroupRights> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<UserGroupRightsViewModelCount> GetUserGroupRights(string RegId, short CompanyId, short pageSize, short pageNumber, string searchString, int UserId)
        {
            UserGroupRightsViewModelCount countViewModel = new UserGroupRightsViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM dbo.AdmUser M_Ban INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Ban.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_Chr ON M_Chr.GLId = M_Ban.GLId WHERE (M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Ban.UserName LIKE '%{searchString}%' OR M_Ban.UserCode LIKE '%{searchString}%' OR M_Ban.AccountNo LIKE '%{searchString}%' OR M_Ban.SwiftCode LIKE '%{searchString}%' OR M_Ban.Remarks1 LIKE '%{searchString}%' OR M_Ban.Remarks2 LIKE '%{searchString}%' OR M_Chr.GLName LIKE '%{searchString}%' OR M_Chr.GLCode LIKE '%{searchString}%') AND M_Ban.UserId<>0 AND M_Ban.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Admins.User}))");

                var result = await _repository.GetQueryAsync<UserGroupRightsViewModel>(RegId, $"SELECT M_Ban.UserId,M_Ban.UserCode,M_Ban.UserName,M_Cur.CurrencyId,M_Cur.CurrencyName,M_Cur.CurrencyCode,M_Ban.AccountNo,M_Ban.SwiftCode,M_Ban.Remarks1,M_Ban.Remarks2,M_Ban.GLId,M_Chr.GLCode,M_Chr.GLName,M_Ban.IsActive,M_Ban.CreateById,M_Ban.CreateDate,M_Ban.EditById,M_Ban.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy  FROM dbo.AdmUser M_Ban INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Ban.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_Chr ON M_Chr.GLId = M_Ban.GLId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Ban.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Ban.EditById WHERE (M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Ban.UserName LIKE '%{searchString}%' OR M_Ban.UserCode LIKE '%{searchString}%' OR M_Ban.AccountNo LIKE '%{searchString}%' OR M_Ban.SwiftCode LIKE '%{searchString}%' OR M_Ban.Remarks1 LIKE '%{searchString}%' OR M_Ban.Remarks2 LIKE '%{searchString}%' OR M_Chr.GLName LIKE '%{searchString}%' OR M_Chr.GLCode LIKE '%{searchString}%') AND M_Ban.UserId<>0 AND M_Ban.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Admins.User})) ORDER BY M_Ban.UserName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)Admins.User,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUser",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> UpsetUserGroupRights(string RegId, short CompanyId, AdmUserGroupRights UserGroupRights, int UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (UserGroupRights.UserGroupId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.AdmUserGroupRights WHERE  UserGroupId={UserGroupRights.UserGroupId} AND ModuleId={UserGroupRights.ModuleId} AND TransactionId={UserGroupRights.TransactionId}");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "UserGroupRights Exist" };
                            }
                        }

                        #region Update UserGroupRights

                        var entity = _context.Update(UserGroupRights);

                        entity.Property(b => b.CreateById).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update UserGroupRights

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Admins.UserGroupRights,
                                DocumentId = UserGroupRights.UserGroupId,
                                DocumentNo = "",
                                TblName = "AdmUserGroupRights",
                                ModeId = (short)Mode.Update,
                                Remarks = "UserGroupRights Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "UserId Should not be zero" };
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
                        TransactionId = (short)Admins.UserGroupRights,
                        DocumentId = UserGroupRights.UserGroupId,
                        DocumentNo = "",
                        TblName = "AdmUserGroupRights",
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
    }
}