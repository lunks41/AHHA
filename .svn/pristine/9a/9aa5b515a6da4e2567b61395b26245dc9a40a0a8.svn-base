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
    public sealed class AccountSetupService : IAccountSetupService
    {
        private readonly IRepository<M_AccountSetup> _repository;
        private ApplicationDbContext _context;

        public AccountSetupService(IRepository<M_AccountSetup> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<AccountSetupViewModelCount> GetAccountSetupListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            AccountSetupViewModelCount AccountSetupViewModelCount = new AccountSetupViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_AccountSetup WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.AccountSetup},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<AccountSetupViewModel>(RegId, $"SELECT M_ACC.AccSetupId,M_ACC.AccSetupCode,M_ACC.AccSetupName,M_ACC.CompanyId,M_ACC.AccSetupCategoryId,M_Accsc.AccSetupCategoryCode,M_Accsc.AccSetupCategoryName,M_ACC.Remarks,M_ACC.IsActive,M_ACC.CreateById,M_ACC.CreateDate,M_ACC.EditById,M_ACC.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_AccountSetup M_ACC  LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ACC.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ACC.EditById INNER JOIN dbo.M_AccountSetupCategory M_Accsc ON M_Accsc.AccSetupCategoryId = M_ACC.AccSetupCategoryId  WHERE (M_ACC.AccSetupName LIKE '%{searchString}%' OR M_ACC.AccSetupCode LIKE '%{searchString}%' OR M_ACC.Remarks LIKE '%{searchString}%' OR M_Accsc.AccSetupCategoryName LIKE '%{searchString}%' OR M_Accsc.AccSetupCategoryCode LIKE '%{searchString}%') AND M_ACC.AccSetupId<>0 AND M_ACC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.AccountSetup},{(short)Modules.Master})) ORDER BY M_ACC.AccSetupName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                AccountSetupViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                AccountSetupViewModelCount.data = result == null ? null : result.ToList();

                return AccountSetupViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.AccountSetup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetup",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_AccountSetup> GetAccountSetupByIdAsync(string RegId, Int16 CompanyId, Int16 AccSetupId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_AccountSetup>(RegId, $"SELECT AccountSetupId,AccountSetupCode,AccountSetupName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_AccountSetup WHERE AccountSetupId={AccSetupId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.AccountSetup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetup",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddAccountSetupAsync(string RegId, Int16 CompanyId, M_AccountSetup AccountSetup, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_AccountSetup WHERE CompanyId IN (SELECT DISTINCT AccountSetupId FROM dbo.Fn_Adm_GetShareCompany ({AccountSetup.CompanyId},{(short)Master.AccountSetup},{(short)Modules.Master})) AND AccountSetupCode='{AccountSetup.AccSetupId}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_AccountSetup WHERE CompanyId IN (SELECT DISTINCT AccountSetupId FROM dbo.Fn_Adm_GetShareCompany ({AccountSetup.CompanyId},{(short)Master.AccountSetup},{(short)Modules.Master})) AND AccountSetupName='{AccountSetup.AccSetupName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "AccountSetup Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "AccountSetup Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (AccountSetupId + 1) FROM dbo.M_AccountSetup WHERE (AccountSetupId + 1) NOT IN (SELECT AccountSetupId FROM dbo.M_AccountSetup)),1) AS MissId");

                        #region Saving AccountSetup

                        AccountSetup.AccSetupId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(AccountSetup);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var AccountSetupToSave = _context.SaveChanges();

                        #endregion Saving AccountSetup

                        #region Save AuditLog

                        if (AccountSetupToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.AccountSetup,
                                DocumentId = AccountSetup.AccSetupId,
                                DocumentNo = AccountSetup.AccSetupCode,
                                TblName = "M_AccountSetup",
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

                        #endregion Save AuditLog
                    }
                    else
                    {
                        sqlResponce = new SqlResponce { Id = -1, Message = "AccountSetupId Should not be zero" };
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
                        TransactionId = (short)Master.AccountSetup,
                        DocumentId = 0,
                        DocumentNo = AccountSetup.AccSetupCode,
                        TblName = "M_AccountSetup",
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

        public async Task<SqlResponce> UpdateAccountSetupAsync(string RegId, Int16 CompanyId, M_AccountSetup AccountSetup, Int32 UserId)
        {
            int IsActive = AccountSetup.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (AccountSetup.AccSetupId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_AccountSetup WHERE CompanyId IN (SELECT DISTINCT AccountSetupId FROM dbo.Fn_Adm_GetShareCompany ({AccountSetup.CompanyId},{(short)Master.AccountSetup},{(short)Modules.Master})) AND AccountSetupName='{AccountSetup.AccSetupName} AND AccountSetupId <>{AccountSetup.AccSetupId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "AccountSetup Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update AccountSetup

                            var entity = _context.Update(AccountSetup);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.AccSetupCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion Update AccountSetup

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Modules.Master,
                                    TransactionId = (short)Master.AccountSetup,
                                    DocumentId = AccountSetup.AccSetupId,
                                    DocumentNo = AccountSetup.AccSetupCode,
                                    TblName = "M_AccountSetup",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "AccountSetup Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "AccountSetupId Should not be zero" };
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
                        TransactionId = (short)Master.AccountSetup,
                        DocumentId = AccountSetup.AccSetupId,
                        DocumentNo = AccountSetup.AccSetupCode,
                        TblName = "M_AccountSetup",
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

        public async Task<SqlResponce> DeleteAccountSetupAsync(string RegId, Int16 CompanyId, M_AccountSetup AccountSetup, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (AccountSetup.AccSetupId > 0)
                {
                    var AccountSetupToRemove = _context.M_AccountSetup.Where(x => x.AccSetupId == AccountSetup.AccSetupId).ExecuteDelete();

                    if (AccountSetupToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Modules.Master,
                            TransactionId = (short)Master.AccountSetup,
                            DocumentId = AccountSetup.AccSetupId,
                            DocumentNo = AccountSetup.AccSetupCode,
                            TblName = "M_AccountSetup",
                            ModeId = (short)Mode.Delete,
                            Remarks = "AccountSetup Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "AccountSetupId Should be zero" };
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
                    TransactionId = (short)Master.AccountSetup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetup",
                    ModeId = (short)Mode.Delete,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<DataSet> GetTrainingByIdsAsync(string RegId, int Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Type", "GET_BY_TRAINING_ID", DbType.String);
                parameters.Add("Id", Id, DbType.Int32);
                return await _repository.GetExecuteDataSetStoredProcedure(RegId, "USP_LMS_Training", parameters);
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Exception: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}