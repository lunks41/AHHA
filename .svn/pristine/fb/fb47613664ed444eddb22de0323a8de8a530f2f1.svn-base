using AHHA.Application.CommonServices;
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

        public async Task<AccountSetupCategoryViewModelCount> GetAccountSetupCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            AccountSetupCategoryViewModelCount accountSetupCategoryViewModelCount = new AccountSetupCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_AccountSetupCategory M_AccSetCa  WHERE M_AccSetCa.AccSetupCategoryId<>0 AND  ( M_AccSetCa.AccSetupCategoryName LIKE '%{searchString}%' OR M_AccSetCa.AccSetupCategoryCode LIKE '%{searchString}%' OR M_AccSetCa.Remarks LIKE '%{searchString}%')");

                var result = await _repository.GetQueryAsync<AccountSetupCategoryViewModel>(RegId, $"SELECT M_AccSetCa.AccSetupCategoryId,M_AccSetCa.AccSetupCategoryCode,M_AccSetCa.AccSetupCategoryName,M_AccSetCa.Remarks,M_AccSetCa.IsActive,M_AccSetCa.CreateById,M_AccSetCa.CreateDate,M_AccSetCa.EditById,M_AccSetCa.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_AccountSetupCategory M_AccSetCa LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_AccSetCa.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_AccSetCa.EditById WHERE M_AccSetCa.AccSetupCategoryId<>0 AND  ( M_AccSetCa.AccSetupCategoryName LIKE '%{searchString}%' OR M_AccSetCa.AccSetupCategoryCode LIKE '%{searchString}%' OR M_AccSetCa.Remarks LIKE '%{searchString}%') ORDER BY M_AccSetCa.AccSetupCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                accountSetupCategoryViewModelCount.responseCode = 200;
                accountSetupCategoryViewModelCount.responseMessage = "success";
                accountSetupCategoryViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                accountSetupCategoryViewModelCount.data = result == null ? null : result.ToList();

                return accountSetupCategoryViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.AccountSetupCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetupCategory",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_AccountSetupCategory> GetAccountSetupCategoryByIdAsync(string RegId, Int16 CompanyId, Int16 AccSetupCategoryId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_AccountSetupCategory>(RegId, $"SELECT AccSetupCategoryId,AccSetupCategoryCode,AccSetupCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_AccountSetupCategory WHERE AccSetupCategoryId={AccSetupCategoryId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.AccountSetupCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetupCategory",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory accountSetupCategory, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_AccountSetupCategory WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.AccountSetupCategory})) AND AccSetupCategoryCode='{accountSetupCategory.AccSetupCategoryId}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_AccountSetupCategory WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.AccountSetupCategory})) AND AccSetupCategoryName='{accountSetupCategory.AccSetupCategoryName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "AccountSetupCategory Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "AccountSetupCategory Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (AccSetupCategoryId + 1) FROM dbo.M_AccountSetupCategory WHERE (AccSetupCategoryId + 1) NOT IN (SELECT AccSetupCategoryId FROM dbo.M_AccountSetupCategory)),1) AS MissId");

                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving AccountSetupCategory

                        accountSetupCategory.AccSetupCategoryId = Convert.ToInt16(sqlMissingResponce.MissId);

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
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.AccountSetupCategory,
                                DocumentId = accountSetupCategory.AccSetupCategoryId,
                                DocumentNo = accountSetupCategory.AccSetupCategoryCode,
                                TblName = "M_AccountSetupCategory",
                                ModeId = (short)Mode.Create,
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
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.AccountSetupCategory,
                        DocumentId = 0,
                        DocumentNo = accountSetupCategory.AccSetupCategoryCode,
                        TblName = "M_AccountSetupCategory",
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

        public async Task<SqlResponce> UpdateAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory accountSetupCategory, Int32 UserId)
        {
            int IsActive = accountSetupCategory.IsActive == true ? 1 : 0;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (accountSetupCategory.AccSetupCategoryId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, RegId, $"SELECT 2 AS IsExist FROM dbo.M_AccountSetupCategory WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.AccountSetupCategory})) AND AccSetupCategoryName='{accountSetupCategory.AccSetupCategoryName} AND AccSetupCategoryId <>{accountSetupCategory.AccSetupCategoryId}'");

                        if (StrExist.Any() && StrExist.ToList()[0].IsExist == 2)
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
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.AccountSetupCategory,
                                DocumentId = accountSetupCategory.AccSetupCategoryId,
                                DocumentNo = accountSetupCategory.AccSetupCategoryCode,
                                TblName = "M_AccountSetupCategory",
                                ModeId = (short)Mode.Update,
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
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.AccountSetupCategory,
                        DocumentId = accountSetupCategory.AccSetupCategoryId,
                        DocumentNo = accountSetupCategory.AccSetupCategoryCode,
                        TblName = "M_AccountSetupCategory",
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

        public async Task<SqlResponce> DeleteAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory accountSetupCategory, Int32 UserId)
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
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.AccountSetupCategory,
                                DocumentId = accountSetupCategory.AccSetupCategoryId,
                                DocumentNo = accountSetupCategory.AccSetupCategoryCode,
                                TblName = "M_AccountSetupCategory",
                                ModeId = (short)Mode.Delete,
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
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.AccountSetupCategory,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_AccountSetupCategory",
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