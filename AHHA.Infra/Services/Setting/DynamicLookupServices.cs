using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Setting;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;
using AHHA.Infra.Data;

namespace AHHA.Infra.Services.Setting
{
    public sealed class DynamicLookupServices : IDynamicLookupService
    {
        private readonly IRepository<S_DynamicLookup> _repository;
        private ApplicationDbContext _context;

        public DynamicLookupServices(IRepository<S_DynamicLookup> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<DynamicLookupViewModel> GetDynamicLookupAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<DynamicLookupViewModel>(RegId, $"SELECT CompanyId,IsBarge,IsVessel,IsVoyage,IsCustomer,IsSupplier,IsProduct,CreateById,CreateDate,EditById,EditDate FROM S_DynamicLookup WHERE CompanyId={CompanyId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DynamicLookup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_DynamicLookup",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> UpsertDynamicLookupAsync(string RegId, Int16 CompanyId, S_DynamicLookup s_DynamicLookup, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM S_DynamicLookup WHERE CompanyId = {s_DynamicLookup.CompanyId}");

                    if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                    {
                        var entity = _context.Update(s_DynamicLookup);
                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        var entity = _context.Add(s_DynamicLookup);
                        entity.Property(b => b.EditDate).IsModified = false;
                        entity.Property(b => b.EditById).IsModified = false;
                    }

                    var FinSettingsToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (FinSettingsToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.DynamicLookup,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "S_DynamicLookup",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "Dynamic Lookup Settings Save Successfully",
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

                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Setting,
                        TransactionId = (short)E_Setting.DynamicLookup,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "S_DynamicLookup",
                        ModeId = (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }
    }
}