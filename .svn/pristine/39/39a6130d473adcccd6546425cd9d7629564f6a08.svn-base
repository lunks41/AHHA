using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Setting;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Infra.Data;

namespace AHHA.Infra.Services.Setting
{
    public sealed class BaseSettingsServices : IBaseSettingsService
    {
        private readonly IRepository<dynamic> _repository;
        private ApplicationDbContext _context;

        public BaseSettingsServices(IRepository<dynamic> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<decimal> GetExchangeRateAsync(string RegId, Int16 CompanyId, Int16 CurrencyId, DateOnly TrnsDate, Int16 UserId)
        {
            try
            {
                return await _repository.GetQuerySingleOrDefaultAsync<decimal>(RegId, $"select dbo.GetExchangeRate ({CompanyId},{CurrencyId},'{TrnsDate}')");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<decimal> GetExchangeRateLocalAsync(string RegId, Int16 CompanyId, Int16 CurrencyId, DateOnly TrnsDate, Int16 UserId)
        {
            try
            {
                return await _repository.GetQuerySingleOrDefaultAsync<decimal>(RegId, $"select dbo.GetExchangeRate_Local ({CompanyId},{CurrencyId},'{TrnsDate}')");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<bool> GetCheckPeriodClosedAsync(string RegId, Int16 CompanyId, Int16 ModuleId, DateOnly TrnsDate, Int16 UserId)
        {
            try
            {
                bool IsResult = await _repository.GetQuerySingleOrDefaultAsync<bool>(RegId, $"select  dbo.CheckPeriodClosed ({CompanyId},{ModuleId},'{TrnsDate}')");

                return IsResult;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<decimal> GetGstPercentageAsync(string RegId, Int16 CompanyId, Int16 GstId, DateOnly TrnsDate, Int16 UserId)
        {
            try
            {
                return await _repository.GetQuerySingleOrDefaultAsync<decimal>(RegId, $"select dbo.GetGSTPercentage ({CompanyId},{GstId},'{TrnsDate}')");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<decimal> GetCreditTermDayAsync(string RegId, Int16 CompanyId, Int16 CreditTermId, DateOnly TrnsDate, Int16 UserId)
        {
            try
            {
                return await _repository.GetQuerySingleOrDefaultAsync<decimal>(RegId, $"select dbo.GetCreditTermDays ({CompanyId},{CreditTermId},'{TrnsDate}')");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
                    ModeId = (short)E_Mode.View,
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