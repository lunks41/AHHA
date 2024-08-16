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
    public sealed class DepartmentService : IDepartmentService
    {
        private readonly IRepository<M_Department> _repository;
        private ApplicationDbContext _context;

        public DepartmentService(IRepository<M_Department> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<DepartmentViewModelCount> GetDepartmentListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            DepartmentViewModelCount DepartmentViewModelCount = new DepartmentViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,$"SELECT COUNT(*) AS CountId FROM M_Department WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Department},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<DepartmentViewModel>(RegId,$"SELECT M_Cou.DepartmentId,M_Cou.DepartmentCode,M_Cou.DepartmentName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Department M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.DepartmentName LIKE '%{searchString}%' OR M_Cou.DepartmentCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.DepartmentId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Department},{(short)Modules.Master})) ORDER BY M_Cou.DepartmentName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                DepartmentViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                DepartmentViewModelCount.departmentViewModels = result == null ? null : result.ToList();

                return DepartmentViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Department,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Department",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_Department> GetDepartmentByIdAsync(string RegId, Int16 CompanyId, Int32 DepartmentId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Department>(RegId,$"SELECT DepartmentId,DepartmentCode,DepartmentName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Department WHERE DepartmentId={DepartmentId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Department,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Department",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddDepartmentAsync(string RegId, Int16 CompanyId, M_Department Department, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 1 AS IsExist FROM dbo.M_Department WHERE CompanyId IN (SELECT DISTINCT DepartmentId FROM dbo.Fn_Adm_GetShareCompany ({Department.CompanyId},{(short)Master.Department},{(short)Modules.Master})) AND DepartmentCode='{Department.DepartmentCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Department WHERE CompanyId IN (SELECT DISTINCT DepartmentId FROM dbo.Fn_Adm_GetShareCompany ({Department.CompanyId},{(short)Master.Department},{(short)Modules.Master})) AND DepartmentName='{Department.DepartmentName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "Department Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "Department Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,"SELECT ISNULL((SELECT TOP 1 (DepartmentId + 1) FROM dbo.M_Department WHERE (DepartmentId + 1) NOT IN (SELECT DepartmentId FROM dbo.M_Department)),1) AS MissId");

                        #region Saving Department

                        Department.DepartmentId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(Department);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var DepartmentToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (DepartmentToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Department,
                                TransactionId = (short)Modules.Master,
                                DocumentId = Department.DepartmentId,
                                DocumentNo = Department.DepartmentCode,
                                TblName = "M_Department",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "DepartmentId Should not be zero" };
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
                        ModuleId = (short)Master.Department,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = Department.DepartmentCode,
                        TblName = "M_Department",
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
        public async Task<SqlResponce> UpdateDepartmentAsync(string RegId, Int16 CompanyId, M_Department Department, Int32 UserId)
        {
            int IsActive = Department.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Department.DepartmentId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 2 AS IsExist FROM dbo.M_Department WHERE CompanyId IN (SELECT DISTINCT DepartmentId FROM dbo.Fn_Adm_GetShareCompany ({Department.CompanyId},{(short)Master.Department},{(short)Modules.Master})) AND DepartmentName='{Department.DepartmentName} AND DepartmentId <>{Department.DepartmentId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "Department Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update Department

                            var entity = _context.Update(Department);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.DepartmentCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.Department,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = Department.DepartmentId,
                                    DocumentNo = Department.DepartmentCode,
                                    TblName = "M_Department",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "Department Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "DepartmentId Should not be zero" };
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
                        ModuleId = (short)Master.Department,
                        TransactionId = (short)Modules.Master,
                        DocumentId = Department.DepartmentId,
                        DocumentNo = Department.DepartmentCode,
                        TblName = "M_Department",
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
        public async Task<SqlResponce> DeleteDepartmentAsync(string RegId, Int16 CompanyId, M_Department Department, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (Department.DepartmentId > 0)
                {
                    var DepartmentToRemove = _context.M_Department.Where(x => x.DepartmentId == Department.DepartmentId).ExecuteDelete();

                    if (DepartmentToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.Department,
                            TransactionId = (short)Modules.Master,
                            DocumentId = Department.DepartmentId,
                            DocumentNo = Department.DepartmentCode,
                            TblName = "M_Department",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Department Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "DepartmentId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Department,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Department",
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
