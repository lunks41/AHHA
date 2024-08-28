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
    public sealed class AccountTypeService : IAccountTypeService
    {
        private readonly IRepository<M_AccountType> _repository;
        private ApplicationDbContext _context;

        public AccountTypeService(IRepository<M_AccountType> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<AccountTypeViewModelCount> GetAccountTypeListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            AccountTypeViewModelCount accountTypeViewModelCount = new AccountTypeViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_AccountType WHERE (M_ACC.AccTypeName LIKE '%{searchString}%' OR M_ACC.AccTypeCode LIKE '%{searchString}%' OR M_ACC.Remarks LIKE '%{searchString}%' OR M_Accsc.AccTypeCategoryName LIKE '%{searchString}%' OR M_Accsc.AccTypeCategoryCode LIKE '%{searchString}%') AND M_ACC.AccTypeId<>0 AND M_ACC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.AccountType}))");

                var result = await _repository.GetQueryAsync<AccountTypeViewModel>(RegId, $"SELECT M_ACC.AccTypeId,M_ACC.AccTypeCode,M_ACC.AccTypeName,M_ACC.CompanyId,M_ACC.SeqNo,M_ACC.Remarks,M_ACC.IsActive,M_ACC.CreateById,M_ACC.CreateDate,M_ACC.EditById,M_ACC.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_AccountType M_ACC  LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ACC.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ACC.EditById  WHERE (M_ACC.AccTypeName LIKE '%{searchString}%' OR M_ACC.AccTypeCode LIKE '%{searchString}%' OR M_ACC.Remarks LIKE '%{searchString}%') AND M_ACC.AccTypeId<>0 AND M_ACC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.AccountType})) ORDER BY M_ACC.AccTypeName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                accountTypeViewModelCount.responseCode = 200;
                accountTypeViewModelCount.responseMessage = "success";
                accountTypeViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                accountTypeViewModelCount.data = result == null ? null : result.ToList();

                return accountTypeViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.AccountType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountType",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_AccountType> GetAccountTypeByIdAsync(string RegId, Int16 CompanyId, Int16 AccTypeId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_AccountType>(RegId, $"SELECT AccTypeId,AccTypeCode,AccTypeName,SeqNo,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_AccountType WHERE AccTypeId={AccTypeId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.AccountType}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.AccountType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountType",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddAccountTypeAsync(string RegId, Int16 CompanyId, M_AccountType AccountType, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_AccountType WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.AccountType})) AND AccTypeCode='{AccountType.AccTypeId}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_AccountType WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.AccountType})) AND AccTypeName='{AccountType.AccTypeName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "AccountType Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "AccountType Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (AccTypeId + 1) FROM dbo.M_AccountType WHERE (AccTypeId + 1) NOT IN (SELECT AccTypeId FROM dbo.M_AccountType)),1) AS MissId");

                    if (sqlMissingResponce != null)
                    {
                        #region Saving AccountType

                        AccountType.AccTypeId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(AccountType);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var AccountTypeToSave = _context.SaveChanges();

                        #endregion Saving AccountType

                        #region Save AuditLog

                        if (AccountTypeToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.AccountType,
                                DocumentId = AccountType.AccTypeId,
                                DocumentNo = AccountType.AccTypeCode,
                                TblName = "M_AccountType",
                                ModeId = (short)Mode.Create,
                                Remarks = "AccountType Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "AccTypeId Should not be zero" };
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
                        TransactionId = (short)Master.AccountType,
                        DocumentId = 0,
                        DocumentNo = AccountType.AccTypeCode,
                        TblName = "M_AccountType",
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

        public async Task<SqlResponce> UpdateAccountTypeAsync(string RegId, Int16 CompanyId, M_AccountType AccountType, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (AccountType.AccTypeId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_AccountType WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.AccountType})) AND AccTypeName='{AccountType.AccTypeName} AND AccTypeId <>{AccountType.AccTypeId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "AccountType Name Exist" };
                            }
                        }

                        #region Update AccountType

                        var entity = _context.Update(AccountType);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.AccTypeCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update AccountType

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.AccountType,
                                DocumentId = AccountType.AccTypeId,
                                DocumentNo = AccountType.AccTypeCode,
                                TblName = "M_AccountType",
                                ModeId = (short)Mode.Update,
                                Remarks = "AccountType Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "AccTypeId Should not be zero" };
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
                        TransactionId = (short)Master.AccountType,
                        DocumentId = AccountType.AccTypeId,
                        DocumentNo = AccountType.AccTypeCode,
                        TblName = "M_AccountType",
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

        public async Task<SqlResponce> DeleteAccountTypeAsync(string RegId, Int16 CompanyId, M_AccountType AccountType, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (AccountType.AccTypeId > 0)
                    {
                        var AccountTypeToRemove = _context.M_AccountType.Where(x => x.AccTypeId == AccountType.AccTypeId).ExecuteDelete();

                        if (AccountTypeToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.AccountType,
                                DocumentId = AccountType.AccTypeId,
                                DocumentNo = AccountType.AccTypeCode,
                                TblName = "M_AccountType",
                                ModeId = (short)Mode.Delete,
                                Remarks = "AccountType Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "AccTypeId Should be zero" };
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
                        TransactionId = (short)Master.AccountType,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_AccountType",
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