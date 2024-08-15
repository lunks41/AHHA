﻿using AHHA.Application.CommonServices;
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
    public sealed class CustomerCreditLimitService : ICustomerCreditLimitService
    {
        private readonly IRepository<M_CustomerCreditLimit> _repository;
        private ApplicationDbContext _context;

        public CustomerCreditLimitService(IRepository<M_CustomerCreditLimit> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CustomerCreditLimitViewModelCount> GetCustomerCreditLimitListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            CustomerCreditLimitViewModelCount CustomerCreditLimitViewModelCount = new CustomerCreditLimitViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_CustomerCreditLimit WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.CustomerCreditLimit},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<CustomerCreditLimitViewModel>($"SELECT M_Cou.CustomerId,M_Cou.CustomerCreditLimitCode,M_Cou.CustomerCreditLimitName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_CustomerCreditLimit M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.CustomerCreditLimitName LIKE '%{searchString}%' OR M_Cou.CustomerCreditLimitCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.CustomerId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.CustomerCreditLimit},{(short)Modules.Master})) ORDER BY M_Cou.CustomerCreditLimitName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                CustomerCreditLimitViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                CustomerCreditLimitViewModelCount.customerCreditLimitViewModels = result == null ? null : result.ToList();

                return CustomerCreditLimitViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.CustomerCreditLimit,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerCreditLimit",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_CustomerCreditLimit> GetCustomerCreditLimitByIdAsync(Int16 CompanyId, Int32 CustomerId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_CustomerCreditLimit>($"SELECT CustomerId,CustomerCreditLimitCode,CustomerCreditLimitName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_CustomerCreditLimit WHERE CustomerId={CustomerId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.CustomerCreditLimit,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerCreditLimit",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddCustomerCreditLimitAsync(Int16 CompanyId, M_CustomerCreditLimit CustomerCreditLimit, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_CustomerCreditLimit WHERE CompanyId IN (SELECT DISTINCT CustomerId FROM dbo.Fn_Adm_GetShareCompany ({CustomerCreditLimit.CompanyId},{(short)Master.CustomerCreditLimit},{(short)Modules.Master}))  UNION ALL SELECT 2 AS IsExist FROM dbo.M_CustomerCreditLimit WHERE CompanyId IN (SELECT DISTINCT CustomerId FROM dbo.Fn_Adm_GetShareCompany ({CustomerCreditLimit.CompanyId},{(short)Master.CustomerCreditLimit},{(short)Modules.Master}))");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "CustomerCreditLimit Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "CustomerCreditLimit Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (CustomerId + 1) FROM dbo.M_CustomerCreditLimit WHERE (CustomerId + 1) NOT IN (SELECT CustomerId FROM dbo.M_CustomerCreditLimit)),1) AS MissId");

                        #region Saving CustomerCreditLimit

                        CustomerCreditLimit.CustomerId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(CustomerCreditLimit);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var CustomerCreditLimitToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (CustomerCreditLimitToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.CustomerCreditLimit,
                                TransactionId = (short)Modules.Master,
                                DocumentId = CustomerCreditLimit.CustomerId,
                                DocumentNo = "",
                                TblName = "M_CustomerCreditLimit",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "CustomerId Should not be zero" };
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
                        ModuleId = (short)Master.CustomerCreditLimit,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CustomerCreditLimit",
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
        public async Task<SqlResponce> UpdateCustomerCreditLimitAsync(Int16 CompanyId, M_CustomerCreditLimit CustomerCreditLimit, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (CustomerCreditLimit.CustomerId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 2 AS IsExist FROM dbo.M_CustomerCreditLimit WHERE CompanyId IN (SELECT DISTINCT CustomerId FROM dbo.Fn_Adm_GetShareCompany ({CustomerCreditLimit.CompanyId},{(short)Master.CustomerCreditLimit},{(short)Modules.Master})) AND CustomerId <>{CustomerCreditLimit.CustomerId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "CustomerCreditLimit Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update CustomerCreditLimit

                            var entity = _context.Update(CustomerCreditLimit);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.CustomerCreditLimit,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = CustomerCreditLimit.CustomerId,
                                    DocumentNo = "",
                                    TblName = "M_CustomerCreditLimit",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "CustomerCreditLimit Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "CustomerId Should not be zero" };
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
                        ModuleId = (short)Master.CustomerCreditLimit,
                        TransactionId = (short)Modules.Master,
                        DocumentId = CustomerCreditLimit.CustomerId,
                        DocumentNo = "",
                        TblName = "M_CustomerCreditLimit",
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
        public async Task<SqlResponce> DeleteCustomerCreditLimitAsync(Int16 CompanyId, M_CustomerCreditLimit CustomerCreditLimit, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (CustomerCreditLimit.CustomerId > 0)
                {
                    var CustomerCreditLimitToRemove = _context.M_CustomerCreditLimit.Where(x => x.CustomerId == CustomerCreditLimit.CustomerId).ExecuteDelete();

                    if (CustomerCreditLimitToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.CustomerCreditLimit,
                            TransactionId = (short)Modules.Master,
                            DocumentId = CustomerCreditLimit.CustomerId,
                            DocumentNo = "",
                            TblName = "M_CustomerCreditLimit",
                            ModeId = (short)Mode.Delete,
                            Remarks = "CustomerCreditLimit Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "CustomerId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.CustomerCreditLimit,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerCreditLimit",
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