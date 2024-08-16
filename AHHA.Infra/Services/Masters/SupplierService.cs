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
    public sealed class SupplierService : ISupplierService
    {
        private readonly IRepository<M_Supplier> _repository;
        private ApplicationDbContext _context;

        public SupplierService(IRepository<M_Supplier> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<SupplierViewModelCount> GetSupplierListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            SupplierViewModelCount SupplierViewModelCount = new SupplierViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,$"SELECT COUNT(*) AS CountId FROM M_Supplier WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Supplier},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<SupplierViewModel>(RegId,$"SELECT M_Cou.SupplierId,M_Cou.SupplierCode,M_Cou.SupplierName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Supplier M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.SupplierName LIKE '%{searchString}%' OR M_Cou.SupplierCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.SupplierId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Supplier},{(short)Modules.Master})) ORDER BY M_Cou.SupplierName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                SupplierViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                SupplierViewModelCount.supplierViewModels = result == null ? null : result.ToList();

                return SupplierViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Supplier,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Supplier",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_Supplier> GetSupplierByIdAsync(string RegId, Int16 CompanyId, Int32 SupplierId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Supplier>(RegId,$"SELECT SupplierId,SupplierCode,SupplierName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Supplier WHERE SupplierId={SupplierId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Supplier,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Supplier",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddSupplierAsync(string RegId, Int16 CompanyId, M_Supplier Supplier, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 1 AS IsExist FROM dbo.M_Supplier WHERE CompanyId IN (SELECT DISTINCT SupplierId FROM dbo.Fn_Adm_GetShareCompany ({Supplier.CompanyId},{(short)Master.Supplier},{(short)Modules.Master})) AND SupplierCode='{Supplier.SupplierCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Supplier WHERE CompanyId IN (SELECT DISTINCT SupplierId FROM dbo.Fn_Adm_GetShareCompany ({Supplier.CompanyId},{(short)Master.Supplier},{(short)Modules.Master})) AND SupplierName='{Supplier.SupplierName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "Supplier Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "Supplier Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId,"SELECT ISNULL((SELECT TOP 1 (SupplierId + 1) FROM dbo.M_Supplier WHERE (SupplierId + 1) NOT IN (SELECT SupplierId FROM dbo.M_Supplier)),1) AS MissId");

                        #region Saving Supplier

                        Supplier.SupplierId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(Supplier);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var SupplierToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (SupplierToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Supplier,
                                TransactionId = (short)Modules.Master,
                                DocumentId = Supplier.SupplierId,
                                DocumentNo = Supplier.SupplierCode,
                                TblName = "M_Supplier",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "SupplierId Should not be zero" };
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
                        ModuleId = (short)Master.Supplier,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = Supplier.SupplierCode,
                        TblName = "M_Supplier",
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
        public async Task<SqlResponce> UpdateSupplierAsync(string RegId, Int16 CompanyId, M_Supplier Supplier, Int32 UserId)
        {
            int IsActive = Supplier.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Supplier.SupplierId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId,$"SELECT 2 AS IsExist FROM dbo.M_Supplier WHERE CompanyId IN (SELECT DISTINCT SupplierId FROM dbo.Fn_Adm_GetShareCompany ({Supplier.CompanyId},{(short)Master.Supplier},{(short)Modules.Master})) AND SupplierName='{Supplier.SupplierName} AND SupplierId <>{Supplier.SupplierId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "Supplier Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update Supplier

                            var entity = _context.Update(Supplier);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.SupplierCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.Supplier,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = Supplier.SupplierId,
                                    DocumentNo = Supplier.SupplierCode,
                                    TblName = "M_Supplier",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "Supplier Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "SupplierId Should not be zero" };
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
                        ModuleId = (short)Master.Supplier,
                        TransactionId = (short)Modules.Master,
                        DocumentId = Supplier.SupplierId,
                        DocumentNo = Supplier.SupplierCode,
                        TblName = "M_Supplier",
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
        public async Task<SqlResponce> DeleteSupplierAsync(string RegId, Int16 CompanyId, M_Supplier Supplier, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (Supplier.SupplierId > 0)
                {
                    var SupplierToRemove = _context.M_Supplier.Where(x => x.SupplierId == Supplier.SupplierId).ExecuteDelete();

                    if (SupplierToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.Supplier,
                            TransactionId = (short)Modules.Master,
                            DocumentId = Supplier.SupplierId,
                            DocumentNo = Supplier.SupplierCode,
                            TblName = "M_Supplier",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Supplier Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "SupplierId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Supplier,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Supplier",
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
