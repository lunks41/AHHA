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
    public sealed class CustomerAddressService : ICustomerAddressService
    {
        private readonly IRepository<M_CustomerAddress> _repository;
        private ApplicationDbContext _context;

        public CustomerAddressService(IRepository<M_CustomerAddress> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CustomerAddressViewModelCount> GetCustomerAddressListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            CustomerAddressViewModelCount customerAddressViewModelCount = new CustomerAddressViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM dbo.M_CustomerAddress M_CusAdd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_CusAdd.CustomerId  WHERE (M_CusAdd.Address1 LIKE '%{searchString}%' OR M_CusAdd.Address2 LIKE '%{searchString}%' OR M_CusAdd.Address3 LIKE '%{searchString}%' OR M_CusAdd.Address4 LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%') AND M_CusAdd.AddressId<>0 AND M_Cus.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Customer}))");

                var result = await _repository.GetQueryAsync<CustomerAddressViewModel>(RegId, $"SELECT M_CusAdd.AddressId,M_CusAdd.Address1,M_CusAdd.Address2,M_CusAdd.Address3,M_CusAdd.Address4,M_CusAdd.PhoneNo,M_CusAdd.EmailAdd,M_CusAdd.IsDefaultAdd,M_CusAdd.IsDeleveryAdd,M_CusAdd.IsFinAdd,M_CusAdd.IsSalesAdd,M_CusAdd.IsActive,M_Cus.CustomerCode,M_Cus.CustomerName,M_CusAdd.CreateById,M_CusAdd.CreateDate,M_CusAdd.EditById,M_CusAdd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CustomerAddress M_CusAdd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_CusAdd.CustomerId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CusAdd.CreateById LEFT JOIN AdmUser Usr1 ON Usr1.UserId = M_CusAdd.EditById WHERE (M_CusAdd.Address1 LIKE '%{searchString}%' OR M_CusAdd.Address2 LIKE '%{searchString}%' OR M_CusAdd.Address3 LIKE '%{searchString}%' OR M_CusAdd.Address4 LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%') AND M_CusAdd.AddressId<>0 AND M_Cus.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Customer})) ORDER BY M_CusAdd.Address1 OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                customerAddressViewModelCount.responseCode = 200;
                customerAddressViewModelCount.responseMessage = "success";
                customerAddressViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                customerAddressViewModelCount.data = result == null ? null : result.ToList();

                return customerAddressViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerAddress",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_CustomerAddress> GetCustomerAddressByIdAsync(string RegId, Int16 CompanyId, Int32 AddressId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_CustomerAddress>(RegId, $"SELECT AddressId,Address1,CustomerAddressName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_CustomerAddress WHERE AddressId={AddressId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerAddress",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddCustomerAddressAsync(string RegId, Int16 CompanyId, M_CustomerAddress CustomerAddress, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_CustomerAddress WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Customer}))  UNION ALL SELECT 2 AS IsExist FROM dbo.M_CustomerAddress WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Customer}))");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "CustomerAddress Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "CustomerAddress Name Exist" };
                        }
                    }
                    else
                    {
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (AddressId + 1) FROM dbo.M_CustomerAddress WHERE (AddressId + 1) NOT IN (SELECT AddressId FROM dbo.M_CustomerAddress)),1) AS MissId");
                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving CustomerAddress

                        CustomerAddress.AddressId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(CustomerAddress);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var CustomerAddressToSave = _context.SaveChanges();

                        #endregion Saving CustomerAddress

                        #region Save AuditLog

                        if (CustomerAddressToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Customer,
                                DocumentId = CustomerAddress.AddressId,
                                DocumentNo = CustomerAddress.Address1,
                                TblName = "M_CustomerAddress",
                                ModeId = (short)Mode.Create,
                                Remarks = "Customer Address Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "AddressId Should not be zero" };
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
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.Customer,
                        DocumentId = 0,
                        DocumentNo = CustomerAddress.Address1,
                        TblName = "M_CustomerAddress",
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

        public async Task<SqlResponce> UpdateCustomerAddressAsync(string RegId, Int16 CompanyId, M_CustomerAddress CustomerAddress, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (CustomerAddress.AddressId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_CustomerAddress WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.Customer})) AND AddressId <>{CustomerAddress.AddressId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "CustomerAddress Name Exist" };
                            }
                        }

                        #region Update CustomerAddress

                        var entity = _context.Update(CustomerAddress);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.Address1).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update CustomerAddress

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Customer,
                                DocumentId = CustomerAddress.AddressId,
                                DocumentNo = CustomerAddress.Address1,
                                TblName = "M_CustomerAddress",
                                ModeId = (short)Mode.Update,
                                Remarks = "CustomerAddress Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "AddressId Should not be zero" };
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
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.Customer,
                        DocumentId = CustomerAddress.AddressId,
                        DocumentNo = CustomerAddress.Address1,
                        TblName = "M_CustomerAddress",
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

        public async Task<SqlResponce> DeleteCustomerAddressAsync(string RegId, Int16 CompanyId, M_CustomerAddress CustomerAddress, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (CustomerAddress.AddressId > 0)
                    {
                        var CustomerAddressToRemove = _context.M_CustomerAddress.Where(x => x.AddressId == CustomerAddress.AddressId).ExecuteDelete();

                        if (CustomerAddressToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Customer,
                                DocumentId = CustomerAddress.AddressId,
                                DocumentNo = CustomerAddress.Address1,
                                TblName = "M_CustomerAddress",
                                ModeId = (short)Mode.Delete,
                                Remarks = "CustomerAddress Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "AddressId Should be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.Customer,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CustomerAddress",
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