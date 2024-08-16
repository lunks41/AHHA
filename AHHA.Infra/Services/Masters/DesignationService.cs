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
    public sealed class DesignationService : IDesignationService
    {
        private readonly IRepository<M_Designation> _repository;
        private ApplicationDbContext _context;

        public DesignationService(IRepository<M_Designation> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<DesignationViewModelCount> GetDesignationListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            DesignationViewModelCount DesignationViewModelCount = new DesignationViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,$"SELECT COUNT(*) AS CountId FROM M_Designation WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Designation},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<DesignationViewModel>(RegId,$"SELECT M_Cou.DesignationId,M_Cou.DesignationCode,M_Cou.DesignationName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Designation M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.DesignationName LIKE '%{searchString}%' OR M_Cou.DesignationCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.DesignationId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Designation},{(short)Modules.Master})) ORDER BY M_Cou.DesignationName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                DesignationViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                DesignationViewModelCount.designationViewModels = result == null ? null : result.ToList();

                return DesignationViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Designation,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Designation",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_Designation> GetDesignationByIdAsync(string RegId, Int16 CompanyId, Int16 DesignationId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Designation>(RegId,$"SELECT DesignationId,DesignationCode,DesignationName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Designation WHERE DesignationId={DesignationId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Designation,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Designation",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddDesignationAsync(string RegId, Int16 CompanyId, M_Designation Designation, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 1 AS IsExist FROM dbo.M_Designation WHERE CompanyId IN (SELECT DISTINCT DesignationId FROM dbo.Fn_Adm_GetShareCompany ({Designation.CompanyId},{(short)Master.Designation},{(short)Modules.Master})) AND DesignationCode='{Designation.DesignationCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Designation WHERE CompanyId IN (SELECT DISTINCT DesignationId FROM dbo.Fn_Adm_GetShareCompany ({Designation.CompanyId},{(short)Master.Designation},{(short)Modules.Master})) AND DesignationName='{Designation.DesignationName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "Designation Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "Designation Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,"SELECT ISNULL((SELECT TOP 1 (DesignationId + 1) FROM dbo.M_Designation WHERE (DesignationId + 1) NOT IN (SELECT DesignationId FROM dbo.M_Designation)),1) AS MissId");

                        #region Saving Designation

                        Designation.DesignationId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(Designation);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var DesignationToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (DesignationToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Designation,
                                TransactionId = (short)Modules.Master,
                                DocumentId = Designation.DesignationId,
                                DocumentNo = Designation.DesignationCode,
                                TblName = "M_Designation",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "DesignationId Should not be zero" };
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
                        ModuleId = (short)Master.Designation,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = Designation.DesignationCode,
                        TblName = "M_Designation",
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
        public async Task<SqlResponce> UpdateDesignationAsync(string RegId, Int16 CompanyId, M_Designation Designation, Int32 UserId)
        {
            int IsActive = Designation.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Designation.DesignationId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 2 AS IsExist FROM dbo.M_Designation WHERE CompanyId IN (SELECT DISTINCT DesignationId FROM dbo.Fn_Adm_GetShareCompany ({Designation.CompanyId},{(short)Master.Designation},{(short)Modules.Master})) AND DesignationName='{Designation.DesignationName} AND DesignationId <>{Designation.DesignationId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "Designation Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update Designation

                            var entity = _context.Update(Designation);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.DesignationCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.Designation,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = Designation.DesignationId,
                                    DocumentNo = Designation.DesignationCode,
                                    TblName = "M_Designation",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "Designation Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "DesignationId Should not be zero" };
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
                        ModuleId = (short)Master.Designation,
                        TransactionId = (short)Modules.Master,
                        DocumentId = Designation.DesignationId,
                        DocumentNo = Designation.DesignationCode,
                        TblName = "M_Designation",
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
        public async Task<SqlResponce> DeleteDesignationAsync(string RegId, Int16 CompanyId, M_Designation Designation, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (Designation.DesignationId > 0)
                {
                    var DesignationToRemove = _context.M_Designation.Where(x => x.DesignationId == Designation.DesignationId).ExecuteDelete();

                    if (DesignationToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.Designation,
                            TransactionId = (short)Modules.Master,
                            DocumentId = Designation.DesignationId,
                            DocumentNo = Designation.DesignationCode,
                            TblName = "M_Designation",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Designation Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "DesignationId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Designation,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Designation",
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
