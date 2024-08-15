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
    public sealed class AccountSetupService : IAccountSetupService
    {
        private readonly IRepository<M_AccountSetup> _repository;
        private ApplicationDbContext _context;

        public AccountSetupService(IRepository<M_AccountSetup> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<AccountSetupViewModelCount> GetAccountSetupListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            AccountSetupViewModelCount AccountSetupViewModelCount = new AccountSetupViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_AccountSetup WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.AccountSetup},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<AccountSetupViewModel>($"SELECT M_Cou.AccountSetupId,M_Cou.AccountSetupCode,M_Cou.AccountSetupName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_AccountSetup M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.AccountSetupName LIKE '%{searchString}%' OR M_Cou.AccountSetupCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.AccountSetupId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.AccountSetup},{(short)Modules.Master})) ORDER BY M_Cou.AccountSetupName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                AccountSetupViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                AccountSetupViewModelCount.accountSetupViewModels = result == null ? null : result.ToList();

                return AccountSetupViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.AccountSetup,
                    TransactionId = (short)Modules.Master,
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
        public async Task<M_AccountSetup> GetAccountSetupByIdAsync(Int16 CompanyId, Int16 AccSetupId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_AccountSetup>($"SELECT AccountSetupId,AccountSetupCode,AccountSetupName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_AccountSetup WHERE AccountSetupId={AccSetupId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.AccountSetup,
                    TransactionId = (short)Modules.Master,
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
        public async Task<SqlResponce> AddAccountSetupAsync(Int16 CompanyId, M_AccountSetup AccountSetup, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_AccountSetup WHERE CompanyId IN (SELECT DISTINCT AccountSetupId FROM dbo.Fn_Adm_GetShareCompany ({AccountSetup.CompanyId},{(short)Master.AccountSetup},{(short)Modules.Master})) AND AccountSetupCode='{AccountSetup.AccSetupId}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_AccountSetup WHERE CompanyId IN (SELECT DISTINCT AccountSetupId FROM dbo.Fn_Adm_GetShareCompany ({AccountSetup.CompanyId},{(short)Master.AccountSetup},{(short)Modules.Master})) AND AccountSetupName='{AccountSetup.AccSetupName}'");

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
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (AccountSetupId + 1) FROM dbo.M_AccountSetup WHERE (AccountSetupId + 1) NOT IN (SELECT AccountSetupId FROM dbo.M_AccountSetup)),1) AS MissId");

                        #region Saving AccountSetup

                        AccountSetup.AccSetupId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(AccountSetup);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var AccountSetupToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (AccountSetupToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.AccountSetup,
                                TransactionId = (short)Modules.Master,
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
                        #endregion

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
                        ModuleId = (short)Master.AccountSetup,
                        TransactionId = (short)Modules.Master,
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
        public async Task<SqlResponce> UpdateAccountSetupAsync(Int16 CompanyId, M_AccountSetup AccountSetup, Int32 UserId)
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
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 2 AS IsExist FROM dbo.M_AccountSetup WHERE CompanyId IN (SELECT DISTINCT AccountSetupId FROM dbo.Fn_Adm_GetShareCompany ({AccountSetup.CompanyId},{(short)Master.AccountSetup},{(short)Modules.Master})) AND AccountSetupName='{AccountSetup.AccSetupName} AND AccountSetupId <>{AccountSetup.AccSetupId}'");

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

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.AccountSetup,
                                    TransactionId = (short)Modules.Master,
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
                        ModuleId = (short)Master.AccountSetup,
                        TransactionId = (short)Modules.Master,
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
        public async Task<SqlResponce> DeleteAccountSetupAsync(Int16 CompanyId, M_AccountSetup AccountSetup, Int32 UserId)
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
                            ModuleId = (short)Master.AccountSetup,
                            TransactionId = (short)Modules.Master,
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
                    ModuleId = (short)Master.AccountSetup,
                    TransactionId = (short)Modules.Master,
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
        public async Task<DataSet> GetTrainingByIdsAsync(int Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Type", "GET_BY_TRAINING_ID", DbType.String);
                parameters.Add("Id", Id, DbType.Int32);
                return await _repository.GetExecuteDataSetStoredProcedure("USP_LMS_Training", parameters);
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
