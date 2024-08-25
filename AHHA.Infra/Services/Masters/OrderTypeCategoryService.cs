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
    public sealed class OrderTypeCategoryService : IOrderTypeCategoryService
    {
        private readonly IRepository<M_OrderTypeCategory> _repository;
        private ApplicationDbContext _context;

        public OrderTypeCategoryService(IRepository<M_OrderTypeCategory> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<OrderTypeCategoryViewModelCount> GetOrderTypeCategoryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            OrderTypeCategoryViewModelCount orderTypeCategoryViewModelCount = new OrderTypeCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_OrderTypeCategory WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.OrderTypeCategory}))");

                var result = await _repository.GetQueryAsync<OrderTypeCategoryViewModel>(RegId, $"SELECT M_Cou.OrderTypeCategoryId,M_Cou.OrderTypeCategoryCode,M_Cou.OrderTypeCategoryName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_OrderTypeCategory M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.OrderTypeCategoryName LIKE '%{searchString}%' OR M_Cou.OrderTypeCategoryCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.OrderTypeCategoryId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.OrderTypeCategory})) ORDER BY M_Cou.OrderTypeCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                orderTypeCategoryViewModelCount.responseCode = 200;
                orderTypeCategoryViewModelCount.responseMessage = "success";
                orderTypeCategoryViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                orderTypeCategoryViewModelCount.data = result == null ? null : result.ToList();

                return orderTypeCategoryViewModelCount;
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

        public async Task<M_OrderTypeCategory> GetOrderTypeCategoryByIdAsync(string RegId, Int16 CompanyId, Int32 OrderTypeCategoryId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_OrderTypeCategory>(RegId, $"SELECT OrderTypeCategoryId,OrderTypeCategoryCode,OrderTypeCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_OrderTypeCategory WHERE OrderTypeCategoryId={OrderTypeCategoryId}");

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

        public async Task<SqlResponce> AddOrderTypeCategoryAsync(string RegId, Int16 CompanyId, M_OrderTypeCategory OrderTypeCategory, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_OrderTypeCategory WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({OrderTypeCategory.CompanyId},{(short)Modules.Master},{(short)Master.OrderTypeCategory})) AND OrderTypeCategoryCode='{OrderTypeCategory.OrderTypeCategoryCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_OrderTypeCategory WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({OrderTypeCategory.CompanyId},{(short)Modules.Master},{(short)Master.OrderTypeCategory})) AND OrderTypeCategoryName='{OrderTypeCategory.OrderTypeCategoryName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "OrderTypeCategory Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "OrderTypeCategory Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (OrderTypeCategoryId + 1) FROM dbo.M_OrderTypeCategory WHERE (OrderTypeCategoryId + 1) NOT IN (SELECT OrderTypeCategoryId FROM dbo.M_OrderTypeCategory)),1) AS MissId");
                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving OrderTypeCategory

                        OrderTypeCategory.OrderTypeCategoryId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(OrderTypeCategory);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var OrderTypeCategoryToSave = _context.SaveChanges();

                        #endregion Saving OrderTypeCategory

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
                                Remarks = "Order Type Category Save Successfully",
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                return new SqlResponce { Result = 1, Message = "Save Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = 1, Message = "Save Failed" };
                        }

                        #endregion Save AuditLog
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "OrderTypeCategoryId Should not be zero" };
                    }
                    return new SqlResponce();
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

        public async Task<SqlResponce> UpdateOrderTypeCategoryAsync(string RegId, Int16 CompanyId, M_OrderTypeCategory OrderTypeCategory, Int32 UserId)
        {
            int IsActive = OrderTypeCategory.IsActive == true ? 1 : 0;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (OrderTypeCategory.OrderTypeCategoryId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_OrderTypeCategory WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({OrderTypeCategory.CompanyId},{(short)Modules.Master},{(short)Master.OrderTypeCategory})) AND OrderTypeCategoryName='{OrderTypeCategory.OrderTypeCategoryName} AND OrderTypeCategoryId <>{OrderTypeCategory.OrderTypeCategoryId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "OrderTypeCategory Name Exist" };
                            }
                        }

                        #region Update OrderTypeCategory

                        var entity = _context.Update(OrderTypeCategory);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.OrderTypeCategoryCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update OrderTypeCategory

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
                            {
                                transaction.Commit();
                                return new SqlResponce { Result = 1, Message = "Update Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Update Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "OrderTypeCategoryId Should not be zero" };
                    }
                    return new SqlResponce();
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

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> DeleteOrderTypeCategoryAsync(string RegId, Int16 CompanyId, M_OrderTypeCategory OrderTypeCategory, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
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
                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                return new SqlResponce { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "OrderTypeCategoryId Should be zero" };
                    }
                    return new SqlResponce();
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
        }
    }
}