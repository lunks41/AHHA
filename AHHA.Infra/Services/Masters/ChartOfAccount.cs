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
    public sealed class ChartOfAccountService : IChartOfAccountService
    {
        private readonly IRepository<M_ChartOfAccount> _repository;
        private ApplicationDbContext _context;

        public ChartOfAccountService(IRepository<M_ChartOfAccount> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<ChartOfAccountViewModelCount> GetChartOfAccountListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            ChartOfAccountViewModelCount chartOfAccountViewModelCount = new ChartOfAccountViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_ChartOfAccount M_ChtAcc INNER JOIN dbo.M_AccountType M_AccTy ON M_AccTy.AccTypeId = M_ChtAcc.AccTypeId INNER JOIN dbo.M_AccountGroup M_AccGrp ON M_AccGrp.AccGroupId = M_ChtAcc.AccGroupId INNER JOIN dbo.M_COACategory1 M_Coa1 ON M_Coa1.COACategoryId = M_ChtAcc.COACategoryId1 INNER JOIN dbo.M_COACategory2 M_Coa2 ON M_Coa2.COACategoryId = M_ChtAcc.COACategoryId2 INNER JOIN dbo.M_COACategory3 M_Coa3 ON M_Coa3.COACategoryId = M_ChtAcc.COACategoryId3 WHERE (M_ChtAcc.GLName LIKE '%{searchString}%' OR M_ChtAcc.GLCode LIKE '%{searchString}%' OR M_ChtAcc.Remarks LIKE '%{searchString}%' OR M_AccTy.AccTypeCode LIKE '%{searchString}%'OR M_AccTy.AccTypeName LIKE '%{searchString}%' OR M_AccGrp.AccGroupCode LIKE '%{searchString}%'OR M_AccGrp.AccGroupName LIKE '%{searchString}%' OR M_Coa1.COACategoryName LIKE '%{searchString}%' OR M_Coa2.COACategoryCode LIKE '%{searchString}%' OR M_Coa2.COACategoryName LIKE '%{searchString}%' OR M_Coa3.COACategoryCode LIKE '%{searchString}%' OR M_Coa3.COACategoryName LIKE '%{searchString}%') AND M_ChtAcc.GLId<>0 AND M_ChtAcc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.ChartOfAccount}))");

                var result = await _repository.GetQueryAsync<ChartOfAccountViewModel>(RegId, $"SELECT M_ChtAcc.GLId,M_ChtAcc.GLCode,M_ChtAcc.GLName,M_ChtAcc.CompanyId,M_ChtAcc.Remarks,M_ChtAcc.IsActive,M_ChtAcc.IsSysControl,M_ChtAcc.seqNo,M_AccGrp.AccGroupCode,M_AccGrp.AccGroupName,M_AccTy.AccTypeCode,M_AccTy.AccTypeName,M_Coa1.COACategoryCode,M_Coa1.COACategoryName,M_Coa2.COACategoryCode,M_Coa2.COACategoryName,M_Coa3.COACategoryCode,M_Coa3.COACategoryName,M_ChtAcc.CreateById,M_ChtAcc.CreateDate,M_ChtAcc.EditById,M_ChtAcc.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_ChartOfAccount M_ChtAcc INNER JOIN dbo.M_AccountType M_AccTy ON M_AccTy.AccTypeId = M_ChtAcc.AccTypeId INNER JOIN dbo.M_AccountGroup M_AccGrp ON M_AccGrp.AccGroupId = M_ChtAcc.AccGroupId INNER JOIN dbo.M_COACategory1 M_Coa1 ON M_Coa1.COACategoryId = M_ChtAcc.COACategoryId1 INNER JOIN dbo.M_COACategory2 M_Coa2 ON M_Coa2.COACategoryId = M_ChtAcc.COACategoryId2 INNER JOIN dbo.M_COACategory3 M_Coa3 ON M_Coa3.COACategoryId = M_ChtAcc.COACategoryId3 LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ChtAcc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ChtAcc.EditById WHERE (M_ChtAcc.GLName LIKE '%{searchString}%' OR M_ChtAcc.GLCode LIKE '%{searchString}%' OR M_ChtAcc.Remarks LIKE '%{searchString}%' OR M_AccTy.AccTypeCode LIKE '%{searchString}%'OR M_AccTy.AccTypeName LIKE '%{searchString}%' OR M_AccGrp.AccGroupCode LIKE '%{searchString}%'OR M_AccGrp.AccGroupName LIKE '%{searchString}%' OR M_Coa1.COACategoryName LIKE '%{searchString}%' OR M_Coa2.COACategoryCode LIKE '%{searchString}%' OR M_Coa2.COACategoryName LIKE '%{searchString}%' OR M_Coa3.COACategoryCode LIKE '%{searchString}%' OR M_Coa3.COACategoryName LIKE '%{searchString}%') AND M_ChtAcc.GLId<>0 AND M_ChtAcc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.ChartOfAccount})) ORDER BY M_ChtAcc.GLName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                chartOfAccountViewModelCount.responseCode = 200;
                chartOfAccountViewModelCount.responseMessage = "success";
                chartOfAccountViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                chartOfAccountViewModelCount.data = result == null ? null : result.ToList();

                return chartOfAccountViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.ChartOfAccount,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_ChartOfAccount",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_ChartOfAccount> GetChartOfAccountByIdAsync(string RegId, Int16 CompanyId, Int16 GLId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_ChartOfAccount>(RegId, $"SELECT GLId,GLCode,GLName,AccTypeId,AccGroupId,COACategoryId1,COACategoryId2,COACategoryId3,IsSysControl,CompanyId,Remarks,seqNo,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_ChartOfAccount WHERE GLId={GLId} AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.ChartOfAccount}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.ChartOfAccount,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_ChartOfAccount",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddChartOfAccountAsync(string RegId, Int16 CompanyId, M_ChartOfAccount ChartOfAccount, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_ChartOfAccount WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.ChartOfAccount})) AND GLCode='{ChartOfAccount.GLCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_ChartOfAccount WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.ChartOfAccount})) AND GLName='{ChartOfAccount.GLName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "ChartOfAccount Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "ChartOfAccount Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (GLId + 1) FROM dbo.M_ChartOfAccount WHERE (GLId + 1) NOT IN (SELECT GLId FROM dbo.M_ChartOfAccount)),1) AS MissId");

                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving ChartOfAccount

                        ChartOfAccount.GLId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(ChartOfAccount);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var ChartOfAccountToSave = _context.SaveChanges();

                        #endregion Saving ChartOfAccount

                        #region Save AuditLog

                        if (ChartOfAccountToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.ChartOfAccount,
                                DocumentId = ChartOfAccount.GLId,
                                DocumentNo = ChartOfAccount.GLCode,
                                TblName = "M_ChartOfAccount",
                                ModeId = (short)Mode.Create,
                                Remarks = "Chart of Account Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "GLId Should not be zero" };
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
                        TransactionId = (short)Master.ChartOfAccount,
                        DocumentId = 0,
                        DocumentNo = ChartOfAccount.GLCode,
                        TblName = "M_ChartOfAccount",
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

        public async Task<SqlResponce> UpdateChartOfAccountAsync(string RegId, Int16 CompanyId, M_ChartOfAccount ChartOfAccount, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ChartOfAccount.GLId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_ChartOfAccount WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.ChartOfAccount})) AND GLName='{ChartOfAccount.GLName} AND GLId <>{ChartOfAccount.GLId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "ChartOfAccount Name Exist" };
                            }
                        }

                        #region Update ChartOfAccount

                        var entity = _context.Update(ChartOfAccount);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.GLCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update ChartOfAccount

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.ChartOfAccount,
                                DocumentId = ChartOfAccount.GLId,
                                DocumentNo = ChartOfAccount.GLCode,
                                TblName = "M_ChartOfAccount",
                                ModeId = (short)Mode.Update,
                                Remarks = "ChartOfAccount Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "GLId Should not be zero" };
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
                        TransactionId = (short)Master.ChartOfAccount,
                        DocumentId = ChartOfAccount.GLId,
                        DocumentNo = ChartOfAccount.GLCode,
                        TblName = "M_ChartOfAccount",
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

        public async Task<SqlResponce> DeleteChartOfAccountAsync(string RegId, Int16 CompanyId, M_ChartOfAccount ChartOfAccount, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ChartOfAccount.GLId > 0)
                    {
                        var ChartOfAccountToRemove = _context.M_ChartOfAccount.Where(x => x.GLId == ChartOfAccount.GLId).ExecuteDelete();

                        if (ChartOfAccountToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.ChartOfAccount,
                                DocumentId = ChartOfAccount.GLId,
                                DocumentNo = ChartOfAccount.GLCode,
                                TblName = "M_ChartOfAccount",
                                ModeId = (short)Mode.Delete,
                                Remarks = "ChartOfAccount Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "GLId Should be zero" };
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
                        TransactionId = (short)Master.ChartOfAccount,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_ChartOfAccount",
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