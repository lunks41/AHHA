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
            ChartOfAccountViewModelCount ChartOfAccountViewModelCount = new ChartOfAccountViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,$"SELECT COUNT(*) AS CountId FROM M_ChartOfAccount WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.ChartOfAccount},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<ChartOfAccountViewModel>(RegId,$"SELECT M_Cou.GLId,M_Cou.GLCode,M_Cou.GLName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_ChartOfAccount M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.GLName LIKE '%{searchString}%' OR M_Cou.GLCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.GLId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.ChartOfAccount},{(short)Modules.Master})) ORDER BY M_Cou.GLName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                ChartOfAccountViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                ChartOfAccountViewModelCount.data = result == null ? null : result.ToList();

                return ChartOfAccountViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.ChartOfAccount,
                    TransactionId = (short)Modules.Master,
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
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_ChartOfAccount>(RegId,$"SELECT GLId,GLCode,GLName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_ChartOfAccount WHERE GLId={GLId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.ChartOfAccount,
                    TransactionId = (short)Modules.Master,
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
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 1 AS IsExist FROM dbo.M_ChartOfAccount WHERE CompanyId IN (SELECT DISTINCT GLId FROM dbo.Fn_Adm_GetShareCompany ({ChartOfAccount.CompanyId},{(short)Master.ChartOfAccount},{(short)Modules.Master})) AND GLCode='{ChartOfAccount.GLCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_ChartOfAccount WHERE CompanyId IN (SELECT DISTINCT GLId FROM dbo.Fn_Adm_GetShareCompany ({ChartOfAccount.CompanyId},{(short)Master.ChartOfAccount},{(short)Modules.Master})) AND GLName='{ChartOfAccount.GLName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "ChartOfAccount Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "ChartOfAccount Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,"SELECT ISNULL((SELECT TOP 1 (GLId + 1) FROM dbo.M_ChartOfAccount WHERE (GLId + 1) NOT IN (SELECT GLId FROM dbo.M_ChartOfAccount)),1) AS MissId");

                        #region Saving ChartOfAccount

                        ChartOfAccount.GLId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(ChartOfAccount);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var ChartOfAccountToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (ChartOfAccountToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.ChartOfAccount,
                                TransactionId = (short)Modules.Master,
                                DocumentId = ChartOfAccount.GLId,
                                DocumentNo = ChartOfAccount.GLCode,
                                TblName = "M_ChartOfAccount",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "GLId Should not be zero" };
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
                        ModuleId = (short)Master.ChartOfAccount,
                        TransactionId = (short)Modules.Master,
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
            int IsActive = ChartOfAccount.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ChartOfAccount.GLId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 2 AS IsExist FROM dbo.M_ChartOfAccount WHERE CompanyId IN (SELECT DISTINCT GLId FROM dbo.Fn_Adm_GetShareCompany ({ChartOfAccount.CompanyId},{(short)Master.ChartOfAccount},{(short)Modules.Master})) AND GLName='{ChartOfAccount.GLName} AND GLId <>{ChartOfAccount.GLId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "ChartOfAccount Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update ChartOfAccount

                            var entity = _context.Update(ChartOfAccount);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.GLCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.ChartOfAccount,
                                    TransactionId = (short)Modules.Master,
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
                                    transaction.Commit();
                            }
                            sqlResponce = new SqlResponce { Id = 1, Message = "Update Successfully" };
                        }
                    }
                    else
                    {
                        sqlResponce = new SqlResponce { Id = -1, Message = "GLId Should not be zero" };
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
                        ModuleId = (short)Master.ChartOfAccount,
                        TransactionId = (short)Modules.Master,
                        DocumentId = ChartOfAccount.GLId,
                        DocumentNo = ChartOfAccount.GLCode,
                        TblName = "M_ChartOfAccount",
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
        public async Task<SqlResponce> DeleteChartOfAccountAsync(string RegId, Int16 CompanyId, M_ChartOfAccount ChartOfAccount, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
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
                            ModuleId = (short)Master.ChartOfAccount,
                            TransactionId = (short)Modules.Master,
                            DocumentId = ChartOfAccount.GLId,
                            DocumentNo = ChartOfAccount.GLCode,
                            TblName = "M_ChartOfAccount",
                            ModeId = (short)Mode.Delete,
                            Remarks = "ChartOfAccount Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "GLId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.ChartOfAccount,
                    TransactionId = (short)Modules.Master,
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
