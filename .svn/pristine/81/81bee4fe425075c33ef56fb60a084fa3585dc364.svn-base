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
    public sealed class COACategory2Service : ICOACategory2Service
    {
        private readonly IRepository<M_COACategory2> _repository;
        private ApplicationDbContext _context;

        public COACategory2Service(IRepository<M_COACategory2> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<COACategoryViewModelCount> GetCOACategory2ListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            COACategoryViewModelCount COACategoryViewModelCount = new COACategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_COACategory2 WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.COACategory2},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<COACategoryViewModel>(RegId, $"SELECT M_Cou.COACategory2Id,M_Cou.COACategory2Code,M_Cou.COACategory2Name,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_COACategory2 M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.COACategory2Name LIKE '%{searchString}%' OR M_Cou.COACategory2Code LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.COACategory2Id<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.COACategory2},{(short)Modules.Master})) ORDER BY M_Cou.COACategory2Name OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                COACategoryViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                COACategoryViewModelCount.data = result == null ? null : result.ToList();

                return COACategoryViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.COACategory2,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory2",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_COACategory2> GetCOACategory2ByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_COACategory2>(RegId, $"SELECT COACategory2Id,COACategory2Code,COACategory2Name,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_COACategory2 WHERE COACategory2Id={COACategoryId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.COACategory2,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory2",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddCOACategory2Async(string RegId, Int16 CompanyId, M_COACategory2 COACategory2, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_COACategory2 WHERE CompanyId IN (SELECT DISTINCT COACategory2Id FROM dbo.Fn_Adm_GetShareCompany ({COACategory2.CompanyId},{(short)Master.COACategory2},{(short)Modules.Master})) AND COACategory2Code='{COACategory2.COACategoryId}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_COACategory2 WHERE CompanyId IN (SELECT DISTINCT COACategory2Id FROM dbo.Fn_Adm_GetShareCompany ({COACategory2.CompanyId},{(short)Master.COACategory2},{(short)Modules.Master})) AND COACategory2Name='{COACategory2.COACategoryName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "COACategory2 Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "COACategory2 Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (COACategory2Id + 1) FROM dbo.M_COACategory2 WHERE (COACategory2Id + 1) NOT IN (SELECT COACategory2Id FROM dbo.M_COACategory2)),1) AS MissId");

                        #region Saving COACategory2

                        COACategory2.COACategoryId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(COACategory2);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var COACategory2ToSave = _context.SaveChanges();

                        #endregion Saving COACategory2

                        #region Save AuditLog

                        if (COACategory2ToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.COACategory2,
                                DocumentId = COACategory2.COACategoryId,
                                DocumentNo = COACategory2.COACategoryCode,
                                TblName = "M_COACategory2",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "COACategory2Id Should not be zero" };
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
                        TransactionId = (short)Master.COACategory2,
                        DocumentId = 0,
                        DocumentNo = COACategory2.COACategoryCode,
                        TblName = "M_COACategory2",
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

        public async Task<SqlResponce> UpdateCOACategory2Async(string RegId, Int16 CompanyId, M_COACategory2 COACategory2, Int32 UserId)
        {
            int IsActive = COACategory2.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (COACategory2.COACategoryId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_COACategory2 WHERE CompanyId IN (SELECT DISTINCT COACategory2Id FROM dbo.Fn_Adm_GetShareCompany ({COACategory2.CompanyId},{(short)Master.COACategory2},{(short)Modules.Master})) AND COACategory2Name='{COACategory2.COACategoryName} AND COACategory2Id <>{COACategory2.COACategoryId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "COACategory2 Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update COACategory2

                            var entity = _context.Update(COACategory2);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.COACategoryCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion Update COACategory2

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Modules.Master,
                                    TransactionId = (short)Master.COACategory2,
                                    DocumentId = COACategory2.COACategoryId,
                                    DocumentNo = COACategory2.COACategoryCode,
                                    TblName = "M_COACategory2",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "COACategory2 Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "COACategory2Id Should not be zero" };
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
                        TransactionId = (short)Master.COACategory2,
                        DocumentId = COACategory2.COACategoryId,
                        DocumentNo = COACategory2.COACategoryCode,
                        TblName = "M_COACategory2",
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

        public async Task<SqlResponce> DeleteCOACategory2Async(string RegId, Int16 CompanyId, M_COACategory2 COACategory2, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (COACategory2.COACategoryId > 0)
                {
                    var COACategory2ToRemove = _context.M_COACategory2.Where(x => x.COACategoryId == COACategory2.COACategoryId).ExecuteDelete();

                    if (COACategory2ToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Modules.Master,
                            TransactionId = (short)Master.COACategory2,
                            DocumentId = COACategory2.COACategoryId,
                            DocumentNo = COACategory2.COACategoryCode,
                            TblName = "M_COACategory2",
                            ModeId = (short)Mode.Delete,
                            Remarks = "COACategory2 Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "COACategory2Id Should be zero" };
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
                    TransactionId = (short)Master.COACategory2,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory2",
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