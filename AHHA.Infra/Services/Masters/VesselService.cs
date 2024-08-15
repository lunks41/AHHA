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
    public sealed class VesselService : IVesselService
    {
        private readonly IRepository<M_Vessel> _repository;
        private ApplicationDbContext _context;

        public VesselService(IRepository<M_Vessel> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<VesselViewModelCount> GetVesselListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            VesselViewModelCount VesselViewModelCount = new VesselViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_Vessel WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Vessel},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<VesselViewModel>($"SELECT M_Cou.VesselId,M_Cou.VesselCode,M_Cou.VesselName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Vessel M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.VesselName LIKE '%{searchString}%' OR M_Cou.VesselCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.VesselId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Vessel},{(short)Modules.Master})) ORDER BY M_Cou.VesselName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                VesselViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                VesselViewModelCount.vesselViewModels = result == null ? null : result.ToList();

                return VesselViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Vessel,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Vessel",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_Vessel> GetVesselByIdAsync(Int16 CompanyId, Int32 VesselId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Vessel>($"SELECT VesselId,VesselCode,VesselName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Vessel WHERE VesselId={VesselId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Vessel,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Vessel",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddVesselAsync(Int16 CompanyId, M_Vessel Vessel, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_Vessel WHERE CompanyId IN (SELECT DISTINCT VesselId FROM dbo.Fn_Adm_GetShareCompany ({Vessel.CompanyId},{(short)Master.Vessel},{(short)Modules.Master})) AND VesselCode='{Vessel.VesselCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Vessel WHERE CompanyId IN (SELECT DISTINCT VesselId FROM dbo.Fn_Adm_GetShareCompany ({Vessel.CompanyId},{(short)Master.Vessel},{(short)Modules.Master})) AND VesselName='{Vessel.VesselName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "Vessel Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "Vessel Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (VesselId + 1) FROM dbo.M_Vessel WHERE (VesselId + 1) NOT IN (SELECT VesselId FROM dbo.M_Vessel)),1) AS MissId");

                        #region Saving Vessel

                        Vessel.VesselId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(Vessel);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var VesselToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (VesselToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Vessel,
                                TransactionId = (short)Modules.Master,
                                DocumentId = Vessel.VesselId,
                                DocumentNo = Vessel.VesselCode,
                                TblName = "M_Vessel",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "VesselId Should not be zero" };
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
                        ModuleId = (short)Master.Vessel,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = Vessel.VesselCode,
                        TblName = "M_Vessel",
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
        public async Task<SqlResponce> UpdateVesselAsync(Int16 CompanyId, M_Vessel Vessel, Int32 UserId)
        {
            int IsActive = Vessel.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Vessel.VesselId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 2 AS IsExist FROM dbo.M_Vessel WHERE CompanyId IN (SELECT DISTINCT VesselId FROM dbo.Fn_Adm_GetShareCompany ({Vessel.CompanyId},{(short)Master.Vessel},{(short)Modules.Master})) AND VesselName='{Vessel.VesselName} AND VesselId <>{Vessel.VesselId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "Vessel Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update Vessel

                            var entity = _context.Update(Vessel);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.VesselCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.Vessel,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = Vessel.VesselId,
                                    DocumentNo = Vessel.VesselCode,
                                    TblName = "M_Vessel",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "Vessel Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "VesselId Should not be zero" };
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
                        ModuleId = (short)Master.Vessel,
                        TransactionId = (short)Modules.Master,
                        DocumentId = Vessel.VesselId,
                        DocumentNo = Vessel.VesselCode,
                        TblName = "M_Vessel",
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
        public async Task<SqlResponce> DeleteVesselAsync(Int16 CompanyId, M_Vessel Vessel, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (Vessel.VesselId > 0)
                {
                    var VesselToRemove = _context.M_Vessel.Where(x => x.VesselId == Vessel.VesselId).ExecuteDelete();

                    if (VesselToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.Vessel,
                            TransactionId = (short)Modules.Master,
                            DocumentId = Vessel.VesselId,
                            DocumentNo = Vessel.VesselCode,
                            TblName = "M_Vessel",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Vessel Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "VesselId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Vessel,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Vessel",
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
