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
    public sealed class OrderTypeCategoryService : IOrderTypeCategoryService
    {
        private readonly IRepository<M_OrderTypeCategory> _repository;
        private ApplicationDbContext _context;

        public OrderTypeCategoryService(IRepository<M_OrderTypeCategory> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<OrderTypeCategoryViewModelCount> GetOrderTypeCategoryListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            OrderTypeCategoryViewModelCount OrderTypeCategoryViewModelCount = new OrderTypeCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_OrderTypeCategory WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.OrderTypeCategory},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<OrderTypeCategoryViewModel>($"SELECT M_Cou.OrderTypeCategoryId,M_Cou.OrderTypeCategoryCode,M_Cou.OrderTypeCategoryName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_OrderTypeCategory M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.OrderTypeCategoryName LIKE '%{searchString}%' OR M_Cou.OrderTypeCategoryCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.OrderTypeCategoryId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.OrderTypeCategory},{(short)Modules.Master})) ORDER BY M_Cou.OrderTypeCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                OrderTypeCategoryViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                OrderTypeCategoryViewModelCount.orderTypeCategoryViewModels = result == null ? null : result.ToList();

                return OrderTypeCategoryViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.OrderTypeCategory,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_OrderTypeCategory",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_OrderTypeCategory> GetOrderTypeCategoryByIdAsync(Int16 CompanyId, Int32 OrderTypeCategoryId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_OrderTypeCategory>($"SELECT OrderTypeCategoryId,OrderTypeCategoryCode,OrderTypeCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_OrderTypeCategory WHERE OrderTypeCategoryId={OrderTypeCategoryId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.OrderTypeCategory,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_OrderTypeCategory",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddOrderTypeCategoryAsync(Int16 CompanyId, M_OrderTypeCategory OrderTypeCategory, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_OrderTypeCategory WHERE CompanyId IN (SELECT DISTINCT OrderTypeCategoryId FROM dbo.Fn_Adm_GetShareCompany ({OrderTypeCategory.CompanyId},{(short)Master.OrderTypeCategory},{(short)Modules.Master})) AND OrderTypeCategoryCode='{OrderTypeCategory.OrderTypeCategoryCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_OrderTypeCategory WHERE CompanyId IN (SELECT DISTINCT OrderTypeCategoryId FROM dbo.Fn_Adm_GetShareCompany ({OrderTypeCategory.CompanyId},{(short)Master.OrderTypeCategory},{(short)Modules.Master})) AND OrderTypeCategoryName='{OrderTypeCategory.OrderTypeCategoryName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "OrderTypeCategory Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "OrderTypeCategory Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (OrderTypeCategoryId + 1) FROM dbo.M_OrderTypeCategory WHERE (OrderTypeCategoryId + 1) NOT IN (SELECT OrderTypeCategoryId FROM dbo.M_OrderTypeCategory)),1) AS MissId");

                        #region Saving OrderTypeCategory

                        OrderTypeCategory.OrderTypeCategoryId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(OrderTypeCategory);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var OrderTypeCategoryToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (OrderTypeCategoryToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.OrderTypeCategory,
                                TransactionId = (short)Modules.Master,
                                DocumentId = OrderTypeCategory.OrderTypeCategoryId,
                                DocumentNo = OrderTypeCategory.OrderTypeCategoryCode,
                                TblName = "M_OrderTypeCategory",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "OrderTypeCategoryId Should not be zero" };
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
                        ModuleId = (short)Master.OrderTypeCategory,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = OrderTypeCategory.OrderTypeCategoryCode,
                        TblName = "M_OrderTypeCategory",
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
        public async Task<SqlResponce> UpdateOrderTypeCategoryAsync(Int16 CompanyId, M_OrderTypeCategory OrderTypeCategory, Int32 UserId)
        {
            int IsActive = OrderTypeCategory.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (OrderTypeCategory.OrderTypeCategoryId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 2 AS IsExist FROM dbo.M_OrderTypeCategory WHERE CompanyId IN (SELECT DISTINCT OrderTypeCategoryId FROM dbo.Fn_Adm_GetShareCompany ({OrderTypeCategory.CompanyId},{(short)Master.OrderTypeCategory},{(short)Modules.Master})) AND OrderTypeCategoryName='{OrderTypeCategory.OrderTypeCategoryName} AND OrderTypeCategoryId <>{OrderTypeCategory.OrderTypeCategoryId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "OrderTypeCategory Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update OrderTypeCategory

                            var entity = _context.Update(OrderTypeCategory);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.OrderTypeCategoryCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.OrderTypeCategory,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = OrderTypeCategory.OrderTypeCategoryId,
                                    DocumentNo = OrderTypeCategory.OrderTypeCategoryCode,
                                    TblName = "M_OrderTypeCategory",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "OrderTypeCategory Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "OrderTypeCategoryId Should not be zero" };
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
                        ModuleId = (short)Master.OrderTypeCategory,
                        TransactionId = (short)Modules.Master,
                        DocumentId = OrderTypeCategory.OrderTypeCategoryId,
                        DocumentNo = OrderTypeCategory.OrderTypeCategoryCode,
                        TblName = "M_OrderTypeCategory",
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
        public async Task<SqlResponce> DeleteOrderTypeCategoryAsync(Int16 CompanyId, M_OrderTypeCategory OrderTypeCategory, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (OrderTypeCategory.OrderTypeCategoryId > 0)
                {
                    var OrderTypeCategoryToRemove = _context.M_OrderTypeCategory.Where(x => x.OrderTypeCategoryId == OrderTypeCategory.OrderTypeCategoryId).ExecuteDelete();

                    if (OrderTypeCategoryToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.OrderTypeCategory,
                            TransactionId = (short)Modules.Master,
                            DocumentId = OrderTypeCategory.OrderTypeCategoryId,
                            DocumentNo = OrderTypeCategory.OrderTypeCategoryCode,
                            TblName = "M_OrderTypeCategory",
                            ModeId = (short)Mode.Delete,
                            Remarks = "OrderTypeCategory Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "OrderTypeCategoryId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.OrderTypeCategory,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_OrderTypeCategory",
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