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
    public sealed class CustomerService : ICustomerService
    {
        private readonly IRepository<M_Customer> _repository;
        private ApplicationDbContext _context;

        public CustomerService(IRepository<M_Customer> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CustomerViewModelCount> GetCustomerListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            CustomerViewModelCount CustomerViewModelCount = new CustomerViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_Customer WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Customer},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<CustomerViewModel>($"SELECT M_Cou.CustomerId,M_Cou.CustomerCode,M_Cou.CustomerName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Customer M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.CustomerName LIKE '%{searchString}%' OR M_Cou.CustomerCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.CustomerId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Customer},{(short)Modules.Master})) ORDER BY M_Cou.CustomerName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                CustomerViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                CustomerViewModelCount.customerViewModels = result == null ? null : result.ToList();

                return CustomerViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Customer,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Customer",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_Customer> GetCustomerByIdAsync(Int16 CompanyId, Int32 CustomerId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Customer>($"SELECT CustomerId,CustomerCode,CustomerName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Customer WHERE CustomerId={CustomerId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Customer,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Customer",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddCustomerAsync(Int16 CompanyId, M_Customer Customer, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_Customer WHERE CompanyId IN (SELECT DISTINCT CustomerId FROM dbo.Fn_Adm_GetShareCompany ({Customer.CompanyId},{(short)Master.Customer},{(short)Modules.Master})) AND CustomerCode='{Customer.CustomerCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Customer WHERE CompanyId IN (SELECT DISTINCT CustomerId FROM dbo.Fn_Adm_GetShareCompany ({Customer.CompanyId},{(short)Master.Customer},{(short)Modules.Master})) AND CustomerName='{Customer.CustomerName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "Customer Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "Customer Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (CustomerId + 1) FROM dbo.M_Customer WHERE (CustomerId + 1) NOT IN (SELECT CustomerId FROM dbo.M_Customer)),1) AS MissId");

                        #region Saving Customer

                        Customer.CustomerId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(Customer);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var CustomerToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (CustomerToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Customer,
                                TransactionId = (short)Modules.Master,
                                DocumentId = Customer.CustomerId,
                                DocumentNo = Customer.CustomerCode,
                                TblName = "M_Customer",
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
                        ModuleId = (short)Master.Customer,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = Customer.CustomerCode,
                        TblName = "M_Customer",
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
        public async Task<SqlResponce> UpdateCustomerAsync(Int16 CompanyId, M_Customer Customer, Int32 UserId)
        {
            int IsActive = Customer.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Customer.CustomerId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 2 AS IsExist FROM dbo.M_Customer WHERE CompanyId IN (SELECT DISTINCT CustomerId FROM dbo.Fn_Adm_GetShareCompany ({Customer.CompanyId},{(short)Master.Customer},{(short)Modules.Master})) AND CustomerName='{Customer.CustomerName} AND CustomerId <>{Customer.CustomerId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "Customer Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update Customer

                            var entity = _context.Update(Customer);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.CustomerCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.Customer,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = Customer.CustomerId,
                                    DocumentNo = Customer.CustomerCode,
                                    TblName = "M_Customer",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "Customer Update Successfully",
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
                        ModuleId = (short)Master.Customer,
                        TransactionId = (short)Modules.Master,
                        DocumentId = Customer.CustomerId,
                        DocumentNo = Customer.CustomerCode,
                        TblName = "M_Customer",
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
        public async Task<SqlResponce> DeleteCustomerAsync(Int16 CompanyId, M_Customer Customer, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (Customer.CustomerId > 0)
                {
                    var CustomerToRemove = _context.M_Customer.Where(x => x.CustomerId == Customer.CustomerId).ExecuteDelete();

                    if (CustomerToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.Customer,
                            TransactionId = (short)Modules.Master,
                            DocumentId = Customer.CustomerId,
                            DocumentNo = Customer.CustomerCode,
                            TblName = "M_Customer",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Customer Delete Successfully",
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
                    ModuleId = (short)Master.Customer,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Customer",
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
