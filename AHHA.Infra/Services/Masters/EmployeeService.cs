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
    public sealed class EmployeeService : IEmployeeService
    {
        private readonly IRepository<M_Employee> _repository;
        private ApplicationDbContext _context;

        public EmployeeService(IRepository<M_Employee> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<EmployeeViewModelCount> GetEmployeeListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            EmployeeViewModelCount EmployeeViewModelCount = new EmployeeViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,$"SELECT COUNT(*) AS CountId FROM M_Employee WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Employee},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<EmployeeViewModel>(RegId,$"SELECT M_Cou.EmployeeId,M_Cou.EmployeeCode,M_Cou.EmployeeName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Employee M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.EmployeeName LIKE '%{searchString}%' OR M_Cou.EmployeeCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.EmployeeId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Employee},{(short)Modules.Master})) ORDER BY M_Cou.EmployeeName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                EmployeeViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                EmployeeViewModelCount.data = result == null ? null : result.ToList();

                return EmployeeViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Employee,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Employee",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_Employee> GetEmployeeByIdAsync(string RegId, Int16 CompanyId, Int32 EmployeeId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Employee>(RegId,$"SELECT EmployeeId,EmployeeCode,EmployeeName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Employee WHERE EmployeeId={EmployeeId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Employee,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Employee",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddEmployeeAsync(string RegId, Int16 CompanyId, M_Employee Employee, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 1 AS IsExist FROM dbo.M_Employee WHERE CompanyId IN (SELECT DISTINCT EmployeeId FROM dbo.Fn_Adm_GetShareCompany ({Employee.CompanyId},{(short)Master.Employee},{(short)Modules.Master})) AND EmployeeCode='{Employee.EmployeeCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Employee WHERE CompanyId IN (SELECT DISTINCT EmployeeId FROM dbo.Fn_Adm_GetShareCompany ({Employee.CompanyId},{(short)Master.Employee},{(short)Modules.Master})) AND EmployeeName='{Employee.EmployeeName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "Employee Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "Employee Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,"SELECT ISNULL((SELECT TOP 1 (EmployeeId + 1) FROM dbo.M_Employee WHERE (EmployeeId + 1) NOT IN (SELECT EmployeeId FROM dbo.M_Employee)),1) AS MissId");

                        #region Saving Employee

                        Employee.EmployeeId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(Employee);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var EmployeeToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (EmployeeToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Employee,
                                TransactionId = (short)Modules.Master,
                                DocumentId = Employee.EmployeeId,
                                DocumentNo = Employee.EmployeeCode,
                                TblName = "M_Employee",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "EmployeeId Should not be zero" };
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
                        ModuleId = (short)Master.Employee,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = Employee.EmployeeCode,
                        TblName = "M_Employee",
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
        public async Task<SqlResponce> UpdateEmployeeAsync(string RegId, Int16 CompanyId, M_Employee Employee, Int32 UserId)
        {
            int IsActive = Employee.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Employee.EmployeeId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 2 AS IsExist FROM dbo.M_Employee WHERE CompanyId IN (SELECT DISTINCT EmployeeId FROM dbo.Fn_Adm_GetShareCompany ({Employee.CompanyId},{(short)Master.Employee},{(short)Modules.Master})) AND EmployeeName='{Employee.EmployeeName} AND EmployeeId <>{Employee.EmployeeId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "Employee Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update Employee

                            var entity = _context.Update(Employee);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.EmployeeCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.Employee,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = Employee.EmployeeId,
                                    DocumentNo = Employee.EmployeeCode,
                                    TblName = "M_Employee",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "Employee Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "EmployeeId Should not be zero" };
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
                        ModuleId = (short)Master.Employee,
                        TransactionId = (short)Modules.Master,
                        DocumentId = Employee.EmployeeId,
                        DocumentNo = Employee.EmployeeCode,
                        TblName = "M_Employee",
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
        public async Task<SqlResponce> DeleteEmployeeAsync(string RegId, Int16 CompanyId, M_Employee Employee, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (Employee.EmployeeId > 0)
                {
                    var EmployeeToRemove = _context.M_Employee.Where(x => x.EmployeeId == Employee.EmployeeId).ExecuteDelete();

                    if (EmployeeToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.Employee,
                            TransactionId = (short)Modules.Master,
                            DocumentId = Employee.EmployeeId,
                            DocumentNo = Employee.EmployeeCode,
                            TblName = "M_Employee",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Employee Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "EmployeeId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Employee,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Employee",
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
