using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AHHA.Infra.Services.Masters
{
    public sealed class AccountGroupService : IAccountGroupService
    {
        private readonly IRepository<M_AccountGroup> _repository;
        private ApplicationDbContext _context;

        public AccountGroupService(IRepository<M_AccountGroup> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<AccountGroupViewModelCount> GetAccountGroupListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            AccountGroupViewModelCount accountGroupViewModelCount = new AccountGroupViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_AccountGroup WHERE (M_ACC.AccGroupName LIKE '%{searchString}%' OR M_ACC.AccGroupCode LIKE '%{searchString}%' OR M_ACC.Remarks LIKE '%{searchString}%' OR M_Accsc.AccGroupCategoryName LIKE '%{searchString}%' OR M_Accsc.AccGroupCategoryCode LIKE '%{searchString}%') AND M_ACC.AccGroupId<>0 AND M_ACC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.AccountGroup}))");

                var result = await _repository.GetQueryAsync<AccountGroupViewModel>(RegId, $"SELECT M_ACC.AccGroupId,M_ACC.AccGroupCode,M_ACC.AccGroupName,M_ACC.CompanyId,M_ACC.AccGroupCategoryId,M_Accsc.AccGroupCategoryCode,M_Accsc.AccGroupCategoryName,M_ACC.Remarks,M_ACC.IsActive,M_ACC.CreateById,M_ACC.CreateDate,M_ACC.EditById,M_ACC.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_AccountGroup M_ACC  LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ACC.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ACC.EditById INNER JOIN dbo.M_AccountGroupCategory M_Accsc ON M_Accsc.AccGroupCategoryId = M_ACC.AccGroupCategoryId  WHERE (M_ACC.AccGroupName LIKE '%{searchString}%' OR M_ACC.AccGroupCode LIKE '%{searchString}%' OR M_ACC.Remarks LIKE '%{searchString}%' OR M_Accsc.AccGroupCategoryName LIKE '%{searchString}%' OR M_Accsc.AccGroupCategoryCode LIKE '%{searchString}%') AND M_ACC.AccGroupId<>0 AND M_ACC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.AccountGroup})) ORDER BY M_ACC.AccGroupName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                accountGroupViewModelCount.responseCode = 200;
                accountGroupViewModelCount.responseMessage = "success";
                accountGroupViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                accountGroupViewModelCount.data = result == null ? null : result.ToList();

                return accountGroupViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.AccountGroup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountGroup",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_AccountGroup> GetAccountGroupByIdAsync(string RegId, Int16 CompanyId, Int16 AccGroupId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_AccountGroup>(RegId, $"SELECT AccountGroupId,AccountGroupCode,AccountGroupName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_AccountGroup WHERE AccountGroupId={AccGroupId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.AccountGroup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountGroup",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddAccountGroupAsync(string RegId, Int16 CompanyId, M_AccountGroup AccountGroup, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_AccountGroup WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.AccountGroup})) AND AccountGroupCode='{AccountGroup.AccGroupId}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_AccountGroup WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.AccountGroup})) AND AccountGroupName='{AccountGroup.AccGroupName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "AccountGroup Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "AccountGroup Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (AccountGroupId + 1) FROM dbo.M_AccountGroup WHERE (AccountGroupId + 1) NOT IN (SELECT AccountGroupId FROM dbo.M_AccountGroup)),1) AS MissId");

                    if (sqlMissingResponce != null)
                    {
                        #region Saving AccountGroup

                        AccountGroup.AccGroupId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(AccountGroup);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var AccountGroupToSave = _context.SaveChanges();

                        #endregion Saving AccountGroup

                        #region Save AuditLog

                        if (AccountGroupToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.AccountGroup,
                                DocumentId = AccountGroup.AccGroupId,
                                DocumentNo = AccountGroup.AccGroupCode,
                                TblName = "M_AccountGroup",
                                ModeId = (short)Mode.Create,
                                Remarks = "AccountGroup Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "AccountGroupId Should not be zero" };
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
                        TransactionId = (short)Master.AccountGroup,
                        DocumentId = 0,
                        DocumentNo = AccountGroup.AccGroupCode,
                        TblName = "M_AccountGroup",
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

        public async Task<SqlResponce> UpdateAccountGroupAsync(string RegId, Int16 CompanyId, M_AccountGroup AccountGroup, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (AccountGroup.AccGroupId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_AccountGroup WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.AccountGroup})) AND AccountGroupName='{AccountGroup.AccGroupName} AND AccountGroupId <>{AccountGroup.AccGroupId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "AccountGroup Name Exist" };
                            }
                        }

                        #region Update AccountGroup

                        var entity = _context.Update(AccountGroup);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.AccGroupCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update AccountGroup

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.AccountGroup,
                                DocumentId = AccountGroup.AccGroupId,
                                DocumentNo = AccountGroup.AccGroupCode,
                                TblName = "M_AccountGroup",
                                ModeId = (short)Mode.Update,
                                Remarks = "AccountGroup Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "AccountGroupId Should not be zero" };
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
                        TransactionId = (short)Master.AccountGroup,
                        DocumentId = AccountGroup.AccGroupId,
                        DocumentNo = AccountGroup.AccGroupCode,
                        TblName = "M_AccountGroup",
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

        public async Task<SqlResponce> DeleteAccountGroupAsync(string RegId, Int16 CompanyId, M_AccountGroup AccountGroup, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (AccountGroup.AccGroupId > 0)
                    {
                        var AccountGroupToRemove = _context.M_AccountGroup.Where(x => x.AccGroupId == AccountGroup.AccGroupId).ExecuteDelete();

                        if (AccountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.AccountGroup,
                                DocumentId = AccountGroup.AccGroupId,
                                DocumentNo = AccountGroup.AccGroupCode,
                                TblName = "M_AccountGroup",
                                ModeId = (short)Mode.Delete,
                                Remarks = "AccountGroup Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "AccountGroupId Should be zero" };
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
                        TransactionId = (short)Master.AccountGroup,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_AccountGroup",
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