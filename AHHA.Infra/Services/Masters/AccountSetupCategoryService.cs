using AHHA.Application.CommonServices;
using AHHA.Application.IServices;
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
            AccountSetupCategoryViewModelCount AccountSetupCategoryViewModelCount = new AccountSetupCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,$"SELECT COUNT(*) AS CountId FROM M_AccountSetupCategory WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.AccountSetupCategory},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<AccountSetupCategoryViewModel>(RegId,$"SELECT M_Cou.AccSetupCategoryId,M_Cou.AccSetupCategoryCode,M_Cou.AccSetupCategoryName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_AccountSetupCategory M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.AccSetupCategoryName LIKE '%{searchString}%' OR M_Cou.AccSetupCategoryCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.AccSetupCategoryId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.AccountSetupCategory},{(short)Modules.Master})) ORDER BY M_Cou.AccSetupCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                AccountSetupCategoryViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                AccountSetupCategoryViewModelCount.accountSetupCategoryViewModels = result == null ? null : result.ToList();

                return AccountSetupCategoryViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.AccountSetupCategory,
                    TransactionId = (short)Modules.Master,
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
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_AccountSetupCategory>(RegId,$"SELECT AccSetupCategoryId,AccSetupCategoryCode,AccSetupCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_AccountSetupCategory WHERE AccSetupCategoryId={AccSetupCategoryId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.AccountSetupCategory,
                    TransactionId = (short)Modules.Master,
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
        public async Task<SqlResponce> AddAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory AccountSetupCategory, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 1 AS IsExist FROM dbo.M_AccountSetupCategory WHERE CompanyId IN (SELECT DISTINCT AccSetupCategoryId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Master.AccountSetupCategory},{(short)Modules.Master})) AND AccSetupCategoryCode='{AccountSetupCategory.AccSetupCategoryId}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_AccountSetupCategory WHERE CompanyId IN (SELECT DISTINCT AccSetupCategoryId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Master.AccountSetupCategory},{(short)Modules.Master})) AND AccSetupCategoryName='{AccountSetupCategory.AccSetupCategoryName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "AccountSetupCategory Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "AccountSetupCategory Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,"SELECT ISNULL((SELECT TOP 1 (AccSetupCategoryId + 1) FROM dbo.M_AccountSetupCategory WHERE (AccSetupCategoryId + 1) NOT IN (SELECT AccSetupCategoryId FROM dbo.M_AccountSetupCategory)),1) AS MissId");

                        #region Saving AccountSetupCategory

                        AccountSetupCategory.AccSetupCategoryId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(AccountSetupCategory);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var AccountSetupCategoryToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (AccountSetupCategoryToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.AccountSetupCategory,
                                TransactionId = (short)Modules.Master,
                                DocumentId = AccountSetupCategory.AccSetupCategoryId,
                                DocumentNo = AccountSetupCategory.AccSetupCategoryCode,
                                TblName = "M_AccountSetupCategory",
                                ModeId = (short)Mode.Create,
                                Remarks = "Invoice Save Successfully",
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            //await _auditLogServices.AddAuditLogAsync(auditLog);
                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                sqlResponce = new SqlResponce { Id = 1, Message = "Save Successfully" };
                            }
                        }
                        #endregion

                    }
                    else
                    {
                        sqlResponce = new SqlResponce { Id = -1, Message = "AccSetupCategoryId Should not be zero" };
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
                        ModuleId = (short)Master.AccountSetupCategory,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = AccountSetupCategory.AccSetupCategoryCode,
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
        public async Task<SqlResponce> UpdateAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory AccountSetupCategory, Int32 UserId)
        {
            int IsActive = AccountSetupCategory.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (AccountSetupCategory.AccSetupCategoryId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,RegId,$"SELECT 2 AS IsExist FROM dbo.M_AccountSetupCategory WHERE CompanyId IN (SELECT DISTINCT AccSetupCategoryId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Master.AccountSetupCategory},{(short)Modules.Master})) AND AccSetupCategoryName='{AccountSetupCategory.AccSetupCategoryName} AND AccSetupCategoryId <>{AccountSetupCategory.AccSetupCategoryId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "AccountSetupCategory Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update AccountSetupCategory

                            var entity = _context.Update(AccountSetupCategory);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.AccSetupCategoryCode).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.AccountSetupCategory,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = AccountSetupCategory.AccSetupCategoryId,
                                    DocumentNo = AccountSetupCategory.AccSetupCategoryCode,
                                    TblName = "M_AccountSetupCategory",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "AccountSetupCategory Update Successfully",
                                    CreateById = UserId
                                };
                                _context.Add(auditLog);
                                var auditLogSave = await _context.SaveChangesAsync();

                                if (auditLogSave > 0)
                                    transaction.Commit();
                            }
                            sqlResponce = new SqlResponce { Id = 1, Message = "Update Successfully" };
                        }
                    }
                    else
                    {
                        sqlResponce = new SqlResponce { Id = -1, Message = "AccSetupCategoryId Should not be zero" };
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
                        ModuleId = (short)Master.AccountSetupCategory,
                        TransactionId = (short)Modules.Master,
                        DocumentId = AccountSetupCategory.AccSetupCategoryId,
                        DocumentNo = AccountSetupCategory.AccSetupCategoryCode,
                        TblName = "M_AccountSetupCategory",
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
        public async Task<SqlResponce> DeleteAccountSetupCategoryAsync(string RegId, Int16 CompanyId, M_AccountSetupCategory AccountSetupCategory, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (AccountSetupCategory.AccSetupCategoryId > 0)
                {
                    var AccountSetupCategoryToRemove = _context.M_AccountSetupCategory.Where(x => x.AccSetupCategoryId == AccountSetupCategory.AccSetupCategoryId).ExecuteDelete();

                    if (AccountSetupCategoryToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.AccountSetupCategory,
                            TransactionId = (short)Modules.Master,
                            DocumentId = AccountSetupCategory.AccSetupCategoryId,
                            DocumentNo = AccountSetupCategory.AccSetupCategoryCode,
                            TblName = "M_AccountSetupCategory",
                            ModeId = (short)Mode.Delete,
                            Remarks = "AccountSetupCategory Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "AccSetupCategoryId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.AccountSetupCategory,
                    TransactionId = (short)Modules.Master,
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
