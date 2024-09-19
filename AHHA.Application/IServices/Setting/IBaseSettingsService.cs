namespace AHHA.Application.IServices.Setting
{
    public interface IBaseSettingsService
    {
        public Task<decimal> GetExchangeRateAsync(string RegId, Int16 CompanyId, Int16 CurrencyId, DateOnly TrnsDate, Int16 UserId);

        public Task<decimal> GetExchangeRateLocalAsync(string RegId, Int16 CompanyId, Int16 CurrencyId, DateOnly TrnsDate, Int16 UserId);

        public Task<bool> GetCheckPeriodClosedAsync(string RegId, Int16 CompanyId, Int16 ModuleId, DateOnly TrnsDate, Int16 UserId);

        public Task<decimal> GetGstPercentageAsync(string RegId, Int16 CompanyId, Int16 GstId, DateOnly TrnsDate, Int16 UserId);

        public Task<decimal> GetCreditTermDayAsync(string RegId, Int16 CompanyId, Int16 CreditTermId, DateOnly TrnsDate, Int16 UserId);
    }
}