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
    public sealed class PortRegionService : IPortRegionService
    {
        private readonly IRepository<M_PortRegion> _repository;
        private ApplicationDbContext _context;

        public PortRegionService(IRepository<M_PortRegion> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<PortRegionViewModelCount> GetPortRegionListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            PortRegionViewModelCount PortRegionViewModelCount = new PortRegionViewModelCount();
            try
            {
                var totalcount = await _repository.QuerySingleORDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_PortRegion WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.PortRegion},{(short)Modules.Master}))", parameters);

                var result = await _repository.QueryIEnumerableAsync<PortRegionViewModel, dynamic>($"SELECT M_Cou.PortRegionId,M_Cou.PortRegionCode,M_Cou.PortRegionName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_PortRegion M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.PortRegion},{(short)Modules.Master})) ORDER BY M_Cou.PortRegionName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY", parameters);

                PortRegionViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                PortRegionViewModelCount.portRegionViewModels = result == null ? null : result.ToList();

                return PortRegionViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.PortRegion,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PortRegion",
                    ModeId = 0,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = 0,
                    //CreateDate = DateTime.Now

                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_PortRegion> GetPortRegionByIdAsync(Int16 CompanyId, Int32 PortRegionId, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            try
            {
                var result = await _repository.QuerySingleORDefaultAsync<M_PortRegion>($"SELECT PortRegionId,PortRegionCode,PortRegionName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_PortRegion WHERE PortRegionId={PortRegionId}", parameters);

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.PortRegion,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PortRegion",
                    ModeId = 0,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                    //CreateDate = DateTime.Now
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddPortRegionAsync(Int16 CompanyId, M_PortRegion PortRegion, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.QueryIEnumerableAsync<SqlResponceIds, dynamic>($"SELECT 1 AS IsExist FROM dbo.M_PortRegion WHERE CompanyId IN (SELECT DISTINCT PortRegionId FROM dbo.Fn_Adm_GetShareCompany ({PortRegion.CompanyId},{(short)Master.PortRegion},{(short)Modules.Master})) AND PortRegionCode='{PortRegion.PortRegionCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_PortRegion WHERE CompanyId IN (SELECT DISTINCT PortRegionId FROM dbo.Fn_Adm_GetShareCompany ({PortRegion.CompanyId},{(short)Master.PortRegion},{(short)Modules.Master})) AND PortRegionName='{PortRegion.PortRegionName}'", parameters);

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "PortRegion Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "PortRegion Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.QuerySingleORDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (PortRegionId + 1) FROM dbo.M_PortRegion WHERE (PortRegionId + 1) NOT IN (SELECT PortRegionId FROM dbo.M_PortRegion)),1) AS MissId", parameters);

                        #region Saving PortRegion

                        PortRegion.PortRegionId = Convert.ToInt32(sqlMissingResponce.MissId);

                        _context.Add(PortRegion);
                        var PortRegionToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (PortRegionToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.PortRegion,
                                TransactionId = (short)Modules.Master,
                                DocumentId = PortRegion.PortRegionId,
                                DocumentNo = PortRegion.PortRegionCode,
                                TblName = "M_PortRegion",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "PortRegionId Should not be zero" };
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
                        ModuleId = (short)Master.PortRegion,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = PortRegion.PortRegionCode,
                        TblName = "M_PortRegion",
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
        public async Task<SqlResponce> UpdatePortRegionAsync(Int16 CompanyId, M_PortRegion PortRegion, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            int IsActive = PortRegion.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (PortRegion.PortRegionId > 0)
                    {
                        //var StrExist = _context.M_PortRegion.FromSqlInterpolated($"SELECT 2 AS IsExist FROM dbo.M_PortRegion WHERE CompanyId IN (SELECT DISTINCT PortRegionId FROM dbo.Fn_Adm_GetShareCompany ({PortRegion.CompanyId},{(short)Master.PortRegion},{(short)Modules.Master})) AND PortRegionName='{PortRegion.PortRegionName}'").AsNoTracking().First();
                        ////Check the Name exist or not
                        var StrExist = await _repository.QueryIEnumerableAsync<SqlResponceIds, dynamic>($"SELECT 2 AS IsExist FROM dbo.M_PortRegion WHERE CompanyId IN (SELECT DISTINCT PortRegionId FROM dbo.Fn_Adm_GetShareCompany ({PortRegion.CompanyId},{(short)Master.PortRegion},{(short)Modules.Master})) AND PortRegionName='{PortRegion.PortRegionName} AND PortRegionId <>{PortRegion.PortRegionId}'", parameters);

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "PortRegion Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update PortRegion

                            var entity = _context.Update(PortRegion);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.PortRegionCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.PortRegion,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = PortRegion.PortRegionId,
                                    DocumentNo = PortRegion.PortRegionCode,
                                    TblName = "M_PortRegion",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "PortRegion Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "PortRegionId Should not be zero" };
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
                        ModuleId = (short)Master.PortRegion,
                        TransactionId = (short)Modules.Master,
                        DocumentId = PortRegion.PortRegionId,
                        DocumentNo = PortRegion.PortRegionCode,
                        TblName = "M_PortRegion",
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
        public async Task<SqlResponce> DeletePortRegionAsync(Int16 CompanyId, M_PortRegion PortRegion, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (PortRegion.PortRegionId > 0)
                {
                    var PortRegionToRemove = _context.M_PortRegion.Where(x => x.PortRegionId == PortRegion.PortRegionId).ExecuteDelete();

                    if (PortRegionToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.PortRegion,
                            TransactionId = (short)Modules.Master,
                            DocumentId = PortRegion.PortRegionId,
                            DocumentNo = PortRegion.PortRegionCode,
                            TblName = "M_PortRegion",
                            ModeId = (short)Mode.Delete,
                            Remarks = "PortRegion Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "PortRegionId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.PortRegion,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PortRegion",
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
                return await _repository.ExecuteDataSetStoredProcedure("USP_LMS_Training", parameters);
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
