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
    public sealed class PortService : IPortService
    {
        private readonly IRepository<M_Port> _repository;
        private ApplicationDbContext _context;

        public PortService(IRepository<M_Port> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<PortViewModelCount> GetPortListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            PortViewModelCount PortViewModelCount = new PortViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,$"SELECT COUNT(*) AS CountId FROM M_Port WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Port},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<PortViewModel>(RegId,$"SELECT M_Cou.PortId,M_Cou.PortCode,M_Cou.PortName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Port M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.PortName LIKE '%{searchString}%' OR M_Cou.PortCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.PortId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Port},{(short)Modules.Master})) ORDER BY M_Cou.PortName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                PortViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                PortViewModelCount.portViewModels = result == null ? null : result.ToList();

                return PortViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Port,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Port",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_Port> GetPortByIdAsync(string RegId, Int16 CompanyId, Int32 PortId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Port>(RegId,$"SELECT PortId,PortCode,PortName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Port WHERE PortId={PortId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Port,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Port",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddPortAsync(string RegId, Int16 CompanyId, M_Port Port, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 1 AS IsExist FROM dbo.M_Port WHERE CompanyId IN (SELECT DISTINCT PortId FROM dbo.Fn_Adm_GetShareCompany ({Port.CompanyId},{(short)Master.Port},{(short)Modules.Master})) AND PortCode='{Port.PortCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Port WHERE CompanyId IN (SELECT DISTINCT PortId FROM dbo.Fn_Adm_GetShareCompany ({Port.CompanyId},{(short)Master.Port},{(short)Modules.Master})) AND PortName='{Port.PortName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "Port Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "Port Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,"SELECT ISNULL((SELECT TOP 1 (PortId + 1) FROM dbo.M_Port WHERE (PortId + 1) NOT IN (SELECT PortId FROM dbo.M_Port)),1) AS MissId");

                        #region Saving Port

                        Port.PortId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(Port);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var PortToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (PortToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Port,
                                TransactionId = (short)Modules.Master,
                                DocumentId = Port.PortId,
                                DocumentNo = Port.PortCode,
                                TblName = "M_Port",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "PortId Should not be zero" };
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
                        ModuleId = (short)Master.Port,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = Port.PortCode,
                        TblName = "M_Port",
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
        public async Task<SqlResponce> UpdatePortAsync(string RegId, Int16 CompanyId, M_Port Port, Int32 UserId)
        {
            int IsActive = Port.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Port.PortId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 2 AS IsExist FROM dbo.M_Port WHERE CompanyId IN (SELECT DISTINCT PortId FROM dbo.Fn_Adm_GetShareCompany ({Port.CompanyId},{(short)Master.Port},{(short)Modules.Master})) AND PortName='{Port.PortName} AND PortId <>{Port.PortId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "Port Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update Port

                            var entity = _context.Update(Port);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.PortCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.Port,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = Port.PortId,
                                    DocumentNo = Port.PortCode,
                                    TblName = "M_Port",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "Port Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "PortId Should not be zero" };
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
                        ModuleId = (short)Master.Port,
                        TransactionId = (short)Modules.Master,
                        DocumentId = Port.PortId,
                        DocumentNo = Port.PortCode,
                        TblName = "M_Port",
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
        public async Task<SqlResponce> DeletePortAsync(string RegId, Int16 CompanyId, M_Port Port, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (Port.PortId > 0)
                {
                    var PortToRemove = _context.M_Port.Where(x => x.PortId == Port.PortId).ExecuteDelete();

                    if (PortToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.Port,
                            TransactionId = (short)Modules.Master,
                            DocumentId = Port.PortId,
                            DocumentNo = Port.PortCode,
                            TblName = "M_Port",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Port Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "PortId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Port,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Port",
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
